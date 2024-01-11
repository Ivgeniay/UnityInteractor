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
        private Dictionary<Type, int> necessaryTypes { get; set; } = new Dictionary<Type, int>()
        {
            
        };


        private int _repeatedNameAmount;
        private int repeatedNameAmount
        {
            get => _repeatedNameAmount;
            set
            {
                _repeatedNameAmount = value;
                OnValidate();
                //OnSaveValidationHandler();
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
            List<BasePort> output = new();
            List<BasePort> inputs = new();

            foreach (BaseNode baseNode in i_Nodes)
            {
                inputs.AddRange(baseNode.GetInputPorts());
                output.AddRange(baseNode.GetOutputPorts());
            }

            foreach (BaseNode baseNode in i_Nodes)
            {
                BasePort port = baseNode.GetOutputPorts().First();
                foreach (BaseInteractionAction e in baseNode.INode.Connections)
                {
                    if (e == null) continue;
                    BasePort otherPort = inputs.Where(t => e == t.Value).FirstOrDefault();
                    if (otherPort != null)
                    {
                        DSEdge edge = new DSEdge
                        {
                            output = port,
                            input = otherPort
                        };
                        edge.input.Connect(edge);
                        edge.output.Connect(edge);
                        Debug.Log($"{baseNode.Name} {port.Name} connect with {otherPort.node}{otherPort.Name}");
                        AddElement(edge);

                        //var portNode = port.node as BaseNode;
                        //var otherPortNode = otherPort.node as BaseNode;
                        //portNode?.OnConnectOutputPort(port, edge);
                        //otherPortNode?.OnConnectInputPort(otherPort, edge);
                    }
                }
            }


            //foreach (BasePort port in output)
            //{
            //    if (port.Value == null) continue;
            //    List<BasePort> otherPorts = inputs.Where(e => e.Value == port.Value).ToList();
            //    foreach (BasePort otherPort in otherPorts)
            //    {
            //        DSEdge edge = new DSEdge
            //        {
            //            output = port,
            //            input = otherPort
            //        };
            //        edge.input.Connect(edge);
            //        edge.output.Connect(edge);
            //        AddElement(edge);

            //        var portNode = port.node as BaseNode;
            //        var otherPortNode = otherPort.node as BaseNode;
            //        portNode?.OnConnectOutputPort(port, edge);
            //        otherPortNode?.OnConnectInputPort(otherPort, edge);
            //    }
            //}
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
            return node;
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

        private void UpdateNode()
        {
            //List<BaseNode> stNums = new();
            //foreach (BaseNode node in i_Nodes)
            //{
            //    IEnumerable<BasePort> inputs = node.GetInputPorts();
            //    if (inputs.Count() == 0)
            //    {
            //        stNums.Add(node);
            //        continue;
            //    }
            //    bool allInputPortsOff = inputs.All(t => !t.connected);
            //    if (allInputPortsOff) stNums.Add(node);
            //}
            //foreach (BaseNode node in stNums) node.Update();
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

        private void SafeDirty()
        {
            UnityEditor.EditorUtility.SetDirty(InteractionInstance);
            UnityEditor.AssetDatabase.SaveAssets();
        }


        internal void OnValidate()
        {
            //OnSaveValidationHandler();
            UpdateNode();
        }
        //private void OnSaveValidationHandler()
        //{
        //    bool repeatednames = repeatedNameAmount == 0;
        //    bool isNesseseryNodes = ValidationNessesaryNodes(i_Nodes, necessaryTypes);
        //    //bool isInstanceNodeNotNullValue = i_Nodes.OfType<InstanceNode>().All(e => e.GetValue().Item2 != null);

        //    OnCanSaveGraphEvent?.Invoke(repeatednames && isNesseseryNodes);// && isInstanceNodeNotNullValue);
        //}
        private bool ValidationNessesaryNodes(List<BaseNode> i_Nodes, Dictionary<Type, int> nessesaryTypes)
        {
            if (i_Nodes.Count == 0) return true;

            Dictionary<Type, int> nodeTypeCounts = i_Nodes
                .GroupBy(node => node.GetType())
                .ToDictionary(group => group.Key, group => group.Count());

            foreach (var kvp in nessesaryTypes)
            {
                Type targetType = kvp.Key;
                int necessaryCount = kvp.Value;

                if (necessaryCount > 0)
                    if (!nodeTypeCounts.TryGetValue(targetType, out int actualCount) || actualCount < necessaryCount) return false;
            }
            return true;
        }

        internal BaseNode GetNodeById(string id) => i_Nodes.FirstOrDefault(e => e.Model.ID == id);
        internal BaseGroup GetGroupById(string id) => i_Groups.FirstOrDefault(e => e.Model.ID == id);
        internal BaseNode GetNodeByPortId(string id)
        {
            BasePort port = GetPortById(id);
            return port.node as BaseNode;
        }
        internal BasePort GetPortById(string id)
        {
            BasePort port = null;
            foreach (var e in i_Nodes)
            {
                var innp = e.GetInputPorts();
                var outnp = e.GetOutputPorts();
                port = innp.FirstOrDefault(p => p.ID == id);
                if (port == null) port = outnp.FirstOrDefault(p => p.ID == id);
                if (port != null) return port;
            }
            throw new NullReferenceException($"There is not port with id {id} in the graph");
        }

        internal void CleanGraph()
        {
            List<GraphElement> graphElements = new List<GraphElement>();
            foreach (var item in i_Nodes) graphElements.Add(item);
            foreach (var item in i_Groups) graphElements.Add(item);
            foreach (var item in graphElements) AddToSelection(item);
            deleteSelection?.Invoke("delete", AskUser.DontAskUser);
            DeleteSelection();
        }

        internal string Load(string filePath)
        {
            //if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath)) return Model.InstanceID;
            //if (!File.Exists(filePath)) throw new FileNotFoundException();
            //filePath = filePath.Substring(filePath.IndexOf("Assets"));
            //GraphSO graphSO = AssetDatabase.LoadAssetAtPath<GraphSO>(filePath);

            //if (graphSO == null) return graphSO.FileName;

            //CleanGraph();
            //Model.InstanceID = graphSO.FileName;
            //foreach (var groupModel in graphSO.GroupModels)
            //{
            //    BaseGroup group = CreateGroup(DSUtilities.GetType(groupModel.Type), groupModel.Position, groupModel.GroupName);
            //    group.Model.ID = groupModel.ID;
            //}

            //foreach (var nodeModel in graphSO.NodeModels)
            //{
            //    BaseNode node = CreateNode(DSUtilities.GetType(nodeModel.DialogueType), nodeModel.Position, new List<object> { nodeModel });
            //    if (!string.IsNullOrWhiteSpace(node.Model.GroupID))
            //    {
            //        BaseGroup group = GetGroupById(node.Model.GroupID);
            //        if (group != null)
            //            group.AddElement(node);
            //    }
            //}

            //foreach (BaseNode node in i_Nodes)
            //{
            //    if (node.Model.Outputs == null || node.Model.Outputs.Count == 0) continue;
            //    foreach (var output in node.Model.Outputs)
            //        ToMakeConnections(output, node.GetOutputPorts()); 
            //}

            return Model.InstanceID;
        }


        private void ToMakeConnections(DSPortModel portModel, IEnumerable<BasePort> ports)
        {
            var port = ports.Where(el => el.ID == portModel.PortID).FirstOrDefault();
            if (port != null)
            {
                if (portModel.NodeIDs == null) { }
                else
                {
                    foreach (NodePortModel portIdModel in portModel.NodeIDs)
                    {
                        BaseNode inputNode = i_Nodes.Where(e => e.Model.ID == portIdModel.NodeID).FirstOrDefault();
                        if (inputNode != null)
                        {
                            foreach (string portId in portIdModel.PortIDs)
                            {
                                IEnumerable<BasePort> inp = inputNode.GetInputPorts();
                                BasePort neededInputPort = inp.Where(e => e.ID == portId).FirstOrDefault();
                                if (neededInputPort != null)
                                {
                                    DSEdge edge = new DSEdge
                                    {
                                        output = port,
                                        input = neededInputPort
                                    };

                                    edge.input.Connect(edge);
                                    edge.output.Connect(edge);
                                    AddElement(edge);

                                    BaseNode outputNode = edge.output.node as BaseNode;
                                    inputNode.OnConnectInputPort(neededInputPort, edge);
                                    outputNode.OnConnectOutputPort(port, edge);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
