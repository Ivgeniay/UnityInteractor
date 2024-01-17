using UnityEditor.Experimental.GraphView;
using System.Collections.Generic; 
using NodeEngine.Database.Error;
using NodeEngine.Database.Save; 
using UnityEngine.UIElements;
using NodeEngine.Utilities;
using NodeEngine.MiniMaps;
using InteractionSystem;
using NodeEngine.Groups;
using NodeEngine.Nodes;
using NodeEngine.Ports;
using NodeEngine.Edges;
using NodeEngine.Text;
using System.Linq;
using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;

namespace NodeEngine.Window
{
    public class DSGraphView : GraphView
    {
        public DSGraphModel Model { get; protected set; }
        public InteractionObject InteractionInstance { get => editorWindow.InteractionInstance; }
        internal DSMiniMap MiniMap;

        private const string GRAPH_STYLE_LINK = "Assets/Plugins/InteractionSystem/NodeEngine/Resources/Front/NodeEngineStyles.uss";
        private const string NODE_STYLE_LINK = "Assets/Plugins/InteractionSystem/NodeEngine/Resources/Front/NodeEngineNodeStyles.uss";
        private const string DSEDGE_STYLE_LINK = "Assets/Plugins/InteractionSystem/NodeEngine/Resources/Front/NodeEngineEdgeStyle.uss";
         
        private DSSearchWindow searchWindow;
        private DSEditorWindow editorWindow;

        private Dictionary<string, DSNodeErrorData> ungroupedNodes;
        private Dictionary<string, DSGroupErrorData> groups;
        private Dictionary<BaseGroup, Dictionary<string, DSNodeErrorData>> groupedNodes;

        internal List<BaseNode> i_Nodes { get; set; } = new List<BaseNode>();
        internal List<BaseGroup> i_Groups { get; set; } = new List<BaseGroup>();
        private Color nodeExecutionColor = new Color32(145, 187, 16, 50);


        private int _repeatedNameAmount;
        private int repeatedNameAmount
        {
            get => _repeatedNameAmount;
            set
            {
                _repeatedNameAmount = value;
                OnValidate();
            }
        }

        internal DSGraphView(DSEditorWindow editorWindow)
        {
            Model = new DSGraphModel() 
            { 
                InstanceID = editorWindow.InteractionInstance.gameObject.GetInstanceID().ToString(),
            };
            this.editorWindow = editorWindow;
            ungroupedNodes = new();
            groups = new();
            groupedNodes = new();

            AddManipulators();
            AddSearchWindow();
            AddMinimap();
            AddGridBackground();

            OnElementDeleted();
            OnGroupElementAdded();
            OnGroupElementRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
        }

        internal void Initialize()
        {
            Sequence sequences = InteractionInstance.GetSequence();

            this.CreateNode(typeof(StartNode), sequences.Position, sequences);

            foreach (BaseInteractionAction sequence in sequences.Sequences)
                this.CreateNode(typeof(ActionNode), sequence.Position, sequence);

            ToMakeConnections();
        }
        private void ToMakeConnections()
        {
            List<BasePort> outputs = new();
            List<BasePort> inputs = new();

            foreach (BaseNode baseNode in i_Nodes)
            {
                inputs.AddRange(baseNode.GetInputPorts());
                outputs.AddRange(baseNode.GetOutputPorts());
            }

            foreach (BasePort output in outputs)
            {
                if (output.Value == null) continue;
                if (output.Name == DSConstants.NEXT_PN)
                {
                    List<BasePort> otherInputs = inputs.Where(input => input.ID == output.ID && input.Name == DSConstants.ACTION_PN).ToList();
                    foreach (BasePort input in otherInputs)
                    {
                        ConnectPorts(output, input);
                    }
                }
            }

            foreach (BasePort input in inputs)
            {
                if (input.Value == null) continue;
                if (input.Name == DSConstants.PARALLEL_PN)
                {
                    List<BasePort> otherOutputs = outputs.Where(output =>
                    {
                        BaseNode outputNode = output.node as BaseNode;
                        if (outputNode is StartNode) return false;
                        return outputNode.INode.ID == input.ID && outputNode != input.node;
                    })
                        .ToList();

                    foreach (BasePort output in otherOutputs)
                        ConnectPorts(output, input);
                }

                if (input.Name == DSConstants.REFERENCE_PN)
                {
                    List<BasePort> otherOutputs = outputs.Where(output =>
                    {
                        BaseNode outputNode = output.node as BaseNode;
                        if (outputNode is StartNode) return false;
                        return outputNode.INode.ID == input.ID && outputNode != input.node;
                    })
                        .ToList();

                    foreach (BasePort output in otherOutputs)
                        ConnectPorts(output, input);
                }
            }
        }
        private DSEdge ConnectPorts(BasePort outputPort, BasePort inputPort)
        {
            DSEdge edge = new DSEdge
            {
                output = outputPort,
                input = inputPort
            };
            edge.input.Connect(edge);
            edge.output.Connect(edge);
            AddElement(edge);
            return edge;
        }

        #region Overrides
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) return;
                if (startPort.node == port.node) return;
                if (startPort.direction == port.direction) return;

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        #endregion
        #region Manipulators
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateGroupContextualMenu());
        }
        #endregion
        #region CreatingElements
        internal T CreateNode<T>(Vector2 position, INode baseInteractionAction) where T : BaseNode
        {
            var type = typeof(T);
            return (T)CreateNode(type, position, baseInteractionAction);
        }
        internal BaseNode CreateNode(Type type, Vector2 position, INode baseInteractionAction)
        {
            BaseNode node = DSUtilities.CreateNode(this, type, baseInteractionAction, position);
            AddElement(node);
            i_Nodes.Add(node);
            OnValidate();
            if (node.IAction != null)
            {
                node.IAction.OnExecutingEvent += NodeExecutingHandler;

                if (node.IAction.IsExecuting) node.SetIndicatorExecutionsStyle(nodeExecutionColor);
                else node.ResetIndicatorExecutionStyle();
            }
            return node;
        }
        private void NodeExecutingHandler(BaseInteractionAction arg1, bool arg2)
        {
            BaseNode target = i_Nodes
                .Where(e => e.IAction == arg1 && 
                    (e.GetType().IsSubclassOf(typeof(ActionNode)) || e.GetType() == typeof(ActionNode)))
                .FirstOrDefault();

            if (target != null)
            {
                if (arg2 == true) target.SetIndicatorExecutionsStyle(nodeExecutionColor);
                else if (arg2 == false) target.ResetIndicatorExecutionStyle();
            }
        }

        internal BaseGroup CreateGroup(Type type, Vector2 mousePosition, string title = "DialogueGroup", string tooltip = null)
        {
            var group = DSUtilities.CreateGroup(this, type, mousePosition, title, tooltip);
            AddElement(group);

            List<BaseNode> innerNode = new List<BaseNode>();
            foreach (GraphElement graphElement in selection)
            {
                if (graphElement is BaseNode node)
                {
                    innerNode.Add(node);
                    group.AddElement(node);
                }
                else continue;
            }

            group.OnCreate(innerNode);
            i_Groups.Add(group);
            return group;
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new(e =>
            {
                e.menu.AppendAction("Add Group", a =>
                    CreateGroup(typeof(BaseGroup), GetLocalMousePosition(a.eventInfo.mousePosition, false)));

                e.menu.AppendAction("Add Bl", a =>
                {
                    Blackboard blackboard = new(this);
                    blackboard.title = "Title";
                    blackboard.subTitle = "SubTitle";
                    blackboard.tooltip = "Tooltip";
                    blackboard.name = "Name";
                    blackboard.scrollable = true; 
                    
                    BlackboardField blackboardField = new BlackboardField();
                    BlackboardField blackboardField2 = new BlackboardField(); 
                    blackboardField.text = "FieldFieldFieldFieldFieldFieldFieldField1";
                    blackboardField2.text = "FieldFieldFieldFieldFieldFieldFieldField2"; 

                    var stringPropertyRow = new BlackboardRow(new TextField("String Property"), blackboardField);
                    var intPropertyRow = new BlackboardRow(new IntegerField("Int Property"), blackboardField);

                    blackboard.addItemRequested += (e) =>
                    {
                        Debug.Log("Plus");
                    };

                    blackboard.Add(stringPropertyRow);
                    blackboard.Add(intPropertyRow);
                    AddElement(blackboard);
                });
            });

            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextMenu(string actionTitle, Type type)
        {
            ContextualMenuManipulator contextualMenuManipulator = new(e =>
            {
                e.menu.AppendAction(actionTitle, a =>
                    AddElement(CreateNode(type, GetLocalMousePosition(a.eventInfo.mousePosition, false), null)));
            });

            return contextualMenuManipulator;
        }

        private void AddSearchWindow()
        {
            if (!searchWindow)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
                searchWindow.Initialize(this);
            }
            nodeCreationRequest = e => SearchWindow.Open(new SearchWindowContext(e.screenMousePosition), searchWindow);
        }
        #endregion
        #region Callbacks
        private void OnElementDeleted()
        {
            deleteSelection += (operationName, askUser) =>
            {
                List<BaseNode> nodesToDelete = new();
                List<BaseGroup> groupToDelete = new();
                List<Edge> edgesToDelete = new();

                foreach (GraphElement element in selection)
                {
                    if (element is BaseNode node)
                    {
                        nodesToDelete.Add(node);
                        continue;
                    }
                    if (element is BaseGroup group)
                    {
                        groupToDelete.Add(group);
                        continue;
                    }
                    if (element is Edge edge)
                    {
                        edgesToDelete.Add(edge);
                        continue;
                    }
                }

                DeleteElements(edgesToDelete);

                foreach (var node in nodesToDelete)
                {
                    if (node.INode is Sequence) continue;

                    node.OnDestroy();
                    if (node.Group is not null)
                        node.Group.RemoveElement(node);

                    RemoveUngroupedNode(node);
                    node.DisconnectAllPorts();
                    if (!string.IsNullOrWhiteSpace(node.Model.GroupID))
                    {
                        BaseGroup gr = GetGroupById(node.Model.GroupID);
                        if (gr != null) gr.OnNodeRemove(node);
                    }
                    i_Nodes.Remove(node);
                    RemoveElement(node);

                    InteractionInstance.RemoveAction(node.INode);
                }

                groupToDelete.ForEach(group =>
                {
                    List<BaseNode> nodes = new();
                    foreach (GraphElement elem in group.containedElements)
                    {
                        if (elem is BaseNode node)
                            nodes.Add(node);
                    }

                    group.OnDestroy();
                    group.RemoveElements(nodes);
                    RemoveGroup(group);
                    i_Groups.Remove(group);
                    RemoveElement(group);
                });
                OnValidate();
                SafeDirty();
            };
        }
        private void OnGroupElementAdded()
        {
            elementsAddedToGroup += (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is BaseNode node)
                    {
                        BaseGroup nodeGroup = (BaseGroup)group;
                        RemoveUngroupedNode(node);
                        node.OnGroupUp(nodeGroup);
                        nodeGroup.OnNodeAdded(node);
                        AddGroupNode(nodeGroup, node);
                    }
                    else continue;
                }
                OnValidate();
                SafeDirty();
            };
        }
        private void OnGroupElementRemoved()
        {
            elementsRemovedFromGroup += (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is BaseNode node)
                    {
                        BaseGroup nodeGroup = (BaseGroup)group;
                        RemoveGroupedNode(nodeGroup, node);
                        node.OnUnGroup(nodeGroup);
                        nodeGroup.OnNodeRemove(node);
                        AddUngroupedNode(node);
                    }
                    else continue;
                }
                OnValidate();
                SafeDirty();
            };
        }
        private void OnGroupRenamed()
        {
            groupTitleChanged += (group, newTitle) =>
            {
                BaseGroup baseGroup = (BaseGroup)group;
                group.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();
                RemoveGroup(baseGroup);

                baseGroup.OnTitleChanged(group.title);
                AddGroup(baseGroup);
                OnValidate();
                SafeDirty();
            };
        }
        private void OnGraphViewChanged()
        {
            graphViewChanged += (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        BaseNode nextNode = edge.input.node as BaseNode;
                        BaseNode outputNode = edge.output.node as BaseNode;

                        outputNode.OnConnectOutputPort(edge.output as BasePort, edge);
                        nextNode.OnConnectInputPort(edge.input as BasePort, edge);
                    }
                    OnValidate();
                }
                if (changes.movedElements != null)
                {
                    foreach (var elem in changes.movedElements)
                    {
                        if (elem is BaseNode node)
                        {
                            node.OnChangePosition(elem.GetPosition().position, changes.moveDelta);
                        }
                        if (elem is BaseGroup group)
                        {
                            group.OnChangePosition(elem.GetPosition().position, changes.moveDelta);
                        }
                    }
                }
                if (changes.elementsToRemove != null)
                {
                    foreach (var elem in changes.elementsToRemove)
                    {
                        if (elem is Edge edge)
                        {
                            BaseNode nextNode = edge.input.node as BaseNode;
                            BaseNode prevNode = edge.output.node as BaseNode;

                            prevNode?.OnDestroyConnectionOutput(edge.output as BasePort, edge);
                            nextNode?.OnDestroyConnectionInput(edge.input as BasePort, edge);
                        }
                    }
                    OnValidate();
                }
                SafeDirty();
                return changes;
            };
        }
        #endregion
        #region Entities Manipulations
        private void AddMinimap()
        {
            MiniMap = new DSMiniMap()
            {
                anchored = true,
            };
            MiniMap.SetPosition(new Rect(10f, 40f, 200, 100));
            Add(MiniMap);
        }
        internal void AddUngroupedNode(BaseNode node)
        {
            string nodeName = node.Model.NodeName.ToLower();

            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new();
                nodeErrorData.Nodes.Add(node);
                ungroupedNodes.Add(nodeName, nodeErrorData);
                return;
            }
            List<BaseNode> ungroupedNodeList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodeList.Add(node);
            Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);

            if (ungroupedNodeList.Count == 2)
            {
                ++repeatedNameAmount;
                ungroupedNodeList[0].SetErrorStyle(errorColor);
            }
        }
        internal void RemoveUngroupedNode(BaseNode node)
        {
            var nodeName = node.Model.NodeName.ToLower();
            List<BaseNode> ungroupedNodeList = ungroupedNodes[nodeName].Nodes;
            ungroupedNodeList.Remove(node);
            node.ResetStyle();

            if (ungroupedNodeList.Count == 1)
            {
                --repeatedNameAmount;
                ungroupedNodeList[0].ResetStyle();
                return;
            }

            if (ungroupedNodeList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
                return;
            }
        }
        internal void AddGroupNode(BaseGroup group, BaseNode node)
        {
            string nodeName = node.Model.NodeName.ToLower();

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new Dictionary<string, DSNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                DSNodeErrorData errorData = new();
                errorData.Nodes.Add(node);
                groupedNodes[group].Add(nodeName, errorData);
                return;
            }
            List<BaseNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;
            groupedNodesList.Add(node);
            Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);

            if (groupedNodesList.Count == 2)
            {
                ++repeatedNameAmount;
                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        internal void RemoveGroupedNode(BaseGroup group, BaseNode node)
        {
            string nodeName = node.Model.NodeName.ToLower();

            List<BaseNode> groupedNodeList = groupedNodes[group][nodeName].Nodes;
            groupedNodeList.Remove(node);

            node.ResetStyle();
            if (groupedNodeList.Count == 1)
            {
                --repeatedNameAmount;
                groupedNodeList[0].ResetStyle();
                return;
            }

            if (groupedNodeList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);
                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
        }
        public void AddGroup(BaseGroup group)
        {
            string groupName = group.title.ToLower();
            if (!groups.ContainsKey(groupName))
            {
                DSGroupErrorData error = new();
                error.Groups.Add(group);
                groups.Add(groupName, error);
                return;
            }

            List<BaseGroup> groupsList = groups[groupName].Groups;
            groupsList.Add(group);
            Color errorColor = groups[groupName].ErrorData.Color;
            group.SetErrorStyle(errorColor);

            if (groupsList.Count == 2)
            {
                ++repeatedNameAmount;
                groupsList[0].SetErrorStyle(errorColor);
            }
        }
        private void RemoveGroup(BaseGroup group)
        {
            string oldGroupName = group.Model.GroupName.ToLower();
            List<BaseGroup> groupList = groups[oldGroupName].Groups;
            groupList.Remove(group);
            //i_Groups.Remove(group);
            group.ResetStyle();

            if (groupList.Count == 1)
            {
                --repeatedNameAmount;
                groupList[0].ResetStyle();
                return;
            }

            if (groupList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }
        }

        #endregion
        #region Styles
        private void AddStyles() =>
            this.LoadAndAddStyleSheets(GRAPH_STYLE_LINK, NODE_STYLE_LINK, DSEDGE_STYLE_LINK);
        
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
        #endregion
        #region Utilities
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMP = mousePosition;

            if (isSearchWindow)
                worldMP -= editorWindow.position.position;

            Vector2 local = contentViewContainer.WorldToLocal(worldMP);
            return local;
        }
        internal List<T> GetListNodesOfType<T>() =>
            i_Nodes.OfType<T>().ToList();

        internal T[] GetArrayNodesOfType<T>() =>
            i_Nodes.OfType<T>().ToArray();

        internal void SafeDirty()
        {
            UnityEditor.EditorUtility.SetDirty(InteractionInstance);
            UnityEditor.AssetDatabase.SaveAssets();
        }


        internal void OnValidate()
        {
            //OnSaveValidationHandler();
        }

        internal BaseGroup GetGroupById(string id) => i_Groups.FirstOrDefault(e => e.Model.ID == id);
        internal void CleanGraph()
        {
            List<GraphElement> graphElements = new List<GraphElement>();
            foreach (var item in i_Nodes) graphElements.Add(item);
            foreach (var item in i_Groups) graphElements.Add(item);
            foreach (var item in graphElements) AddToSelection(item);
            deleteSelection?.Invoke("delete", AskUser.DontAskUser);
            DeleteSelection();
        }
        #endregion
    }
}
