using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using NodeEngine.Database.Save; 
using UnityEngine.UIElements;
using NodeEngine.UIElement;
using NodeEngine.Utilities; 
using InteractionSystem;
using NodeEngine.Groups;
using NodeEngine.Window;
using NodeEngine.Ports;
using NodeEngine.Edges;
using NodeEngine.Text;
using UnityEngine;
using System.Linq;
using System;

namespace NodeEngine.Nodes
{
    internal abstract class BaseNode : Node
    {
        internal DSNodeModel Model { get; private set; }
        internal BaseGroup Group { get; private set; }
        public virtual string Name => Model.NodeName;
        public INode INode { get; private set; }
        public virtual BaseInteractionAction IAction { get; protected set; }
        public NodeContext NodeContext { get; protected set; }
        protected TextField titleTF { get; set; }
        protected DSGraphView graphView { get; set; } 
        protected List<BasePort> inputPorts = new();
        protected List<BasePort> outputPorts = new();

        private Color defaultMainContainerColor;
        private Color defaultTitleColor;


        internal virtual void Initialize(DSGraphView graphView, Vector2 position, INode iNode)
        {
            this.INode = iNode;

            NodeContext = new(INode);
            NodeContext.Initialize();

            Model = new()
            {
                ID = Guid.NewGuid().ToString(),
                NodeName = string.IsNullOrEmpty(iNode.Name) ? iNode.GetType().Name : iNode.Name,
                Position = position,
            };

            iNode.Position = position;
            defaultMainContainerColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
            this.graphView = graphView;
            this.SetPosition(new Rect(position, Vector2.zero));
            AddStyles();
        }

        #region Draw
        protected virtual void DrawTitleContainer(VisualElement container)
        {
            titleTF = DSUtilities.CreateTextField(
                Model.NodeName,
                null,
                callback =>
                {
                    TextField target = callback.target as TextField;
                    string result = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                    target.value = result;
                    INode.Name = result;

                    if (Group is null)
                    {
                        graphView.RemoveUngroupedNode(this);
                        Model.NodeName = target.value;
                        graphView.AddUngroupedNode(this);
                    }
                    else
                    {
                        BaseGroup currentGroup = Group;
                        graphView.RemoveGroupedNode(Group, this);
                        Model.NodeName = target.value;
                        graphView.AddGroupNode(currentGroup, this);
                    }
                },
                styles: new string[]
                    {
                        "ds-node__textfield",
                        "ds-node__filename-textfield",
                        "ds-node__textfield__hidden"
                    }
                );

            container.Insert(0, titleTF);
        }
        protected virtual void DrawInputContainer(VisualElement container)
        {
             foreach (PortInfo input in Model.Inputs)
                AddPortByType(input);
        }
        protected virtual void DrawOutputContainer(VisualElement container)
        {
            foreach (PortInfo output in Model.Outputs)
                AddPortByType(output);
        }
        protected virtual void DrawMainContainer(VisualElement container) { }
        protected virtual void DrawExtensionContainer(VisualElement container) { }


        protected virtual void Draw()
        {
            DrawTitleContainer(titleContainer);
            DrawOutputContainer(outputContainer);
            DrawInputContainer(inputContainer);
            DrawMainContainer(mainContainer);
            DrawExtensionContainer(extensionContainer);

            RefreshExpandedState();
            defaultTitleColor = titleContainer.style.backgroundColor.value;
            ResetIndicatorExecutionStyle();
        }
        #endregion

        #region Overrided
        public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type) =>
            BasePort.CreateBasePort<DSEdge>(orientation, direction, capacity, type);
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", e => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", e => DisconnectOutputPorts());
            base.BuildContextualMenu(evt);
        }
        #endregion

        #region Style
        private void AddStyles()
        {
            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }
        internal virtual void SetErrorStyle(Color color) => mainContainer.style.backgroundColor = color;
        internal virtual void ResetStyle() => mainContainer.style.backgroundColor = defaultMainContainerColor;

        internal virtual void SetIndicatorExecutionsStyle(Color color) => titleContainer.style.backgroundColor = color;
        internal virtual void ResetIndicatorExecutionStyle() => titleContainer.style.backgroundColor = defaultTitleColor;
        #endregion

        #region Utilits
        internal void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private List<BasePort> GetAllOutputPortsRecursive(VisualElement element)
        {
            List<BasePort> outputPorts = new List<BasePort>();

            foreach (VisualElement child in element.Children())
            {
                if (child is BasePort basePort)
                {
                    outputPorts.Add(basePort);
                }

                if (child.childCount > 0)
                {
                    outputPorts.AddRange(GetAllOutputPortsRecursive(child));
                }
            }
            return outputPorts;
        }
        private void DisconnectInputPorts() => DisconnectPort(inputContainer);
        private void DisconnectOutputPorts() => DisconnectPort(outputContainer);
        private void DisconnectPort(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (port.connected)
                {
                    graphView.DeleteElements(port.connections);
                }
            }
        }
        #endregion

        #region MonoEvents
        public virtual void OnConnectOutputPort(BasePort port, Edge edge) { }
        public virtual void OnConnectInputPort(BasePort port, Edge edge) { }
        public virtual void OnDestroyConnectionOutput(BasePort port, Edge edge) { }
        public virtual void OnDestroyConnectionInput(BasePort port, Edge edge) { }

        public virtual void OnChangePosition(Vector2 position, Vector2 delta) =>
            INode.Position += delta;
        
        
        public virtual void OnCreate() => Draw();
        public virtual void OnDestroy() 
        {
            List<BasePort> ports = new();
            ports.AddRange(GetInputPorts());
            ports.AddRange(GetOutputPorts());
            foreach (var item in ports) item.OnDistroy();

            List<DSTextField> dsText = this.GetElementsByType<DSTextField>();
            foreach (var item in dsText) item.OnDistroy();
        }
        public virtual void OnGroupUp(BaseGroup group)
        {
            Model.GroupID = group.Model.ID;
            Group = group;
        }
        public virtual void OnUnGroup(BaseGroup group)
        {
            Model.GroupID = "";
            Group = null;
        }

        internal virtual void Update() { }
        #endregion

        #region Ports

        protected virtual BasePort AddPortByType(Direction direction, BaseInteractionAction interactionAction, string portName = "Port", Type type = null, Port.Capacity capacity = Port.Capacity.Single, Orientation orientation = Orientation.Horizontal)
        {
            PortInfo portInfo = new PortInfo()
            {
                InteractionAction = interactionAction,
                PortName = portName,
                Type = type,
                Direction = direction,
                Capacity = capacity,
                Orientation = orientation,
                Node = this,
            };
            Model.AddPort(portInfo);
            return AddPortByType(portInfo);
        }
        protected virtual BasePort AddPortByType(PortInfo data)
        {
            BasePort port = null;
            port = this.CreatePort(
                action: data.InteractionAction,
                ID: "",
                portname: data.PortName,
                orientation: data.Orientation,
                direction: data.Direction,
                capacity: data.Capacity,
                type: data.Type); 
            port.AvailableTypes = new Type[] { data.Type };
            port.GrathView = graphView;

            //if (data.IsField && data.Type != null)
            //{
            //    switch (data.Type)
            //    {
            //        case Type t when t == typeof(float):
            //            float def = 0;
            //            if (float.TryParse(data.Value, out def)) {}
            //            FloatField floatField = DSUtilities.CreateFloatField(
            //            value: def,
            //            onChange: callback =>
            //            {
            //                FloatField target = callback.target as FloatField;
            //                target.value = callback.newValue;
            //                data.Value = callback.newValue.ToString();
            //                port.SetValue(callback.newValue);
            //            },
            //            styles: new string[]
            //                {
            //                    "ds-node__floatfield",
            //                    "ds-node__choice-textfield",
            //                    "ds-node__textfield__hidden"
            //                }
            //            );
            //            port.Add(floatField);
            //            break;

            //        case Type t when t == typeof(bool):
            //            bool bDef = data.Value.ToLower() == "true" ? true : false;
            //            Toggle toggle = DSUtilities.CreateToggle(
            //                "",
            //                "",
            //                onChange: callBack =>
            //                {
            //                    Toggle target = callBack.target as Toggle;
            //                    target.value = callBack.newValue;
            //                    data.Value = callBack.newValue.ToString();
            //                    port.SetValue(data.Value);
            //                },
            //                styles: new string[]
            //                {
            //                    "ds-node__toglefield",
            //                    "ds-node__choice-textfield",
            //                    "ds-node__textfield__hidden"
            //                },
            //                value: bDef);
            //            port.Add(toggle);
            //            break;

            //        case Type t when t == typeof(int):
            //            int iDef = 0;
            //            if (int.TryParse(data.Value, out iDef)) { }
            //            IntegerField integetField = DSUtilities.CreateIntegerField(
            //            value: iDef,
            //            onChange: callback =>
            //            {
            //                IntegerField target = callback.target as IntegerField;
            //                target.value = callback.newValue;
            //                data.Value = callback.newValue.ToString();
            //                port.SetValue(callback.newValue);

            //            },
            //            styles: new string[]
            //                {
            //                    "ds-node__integerfield",
            //                    "ds-node__choice-textfield",
            //                    "ds-node__textfield__hidden"
            //                }
            //            );
            //            port.Add(integetField);
            //            break;

            //        case Type t when t == typeof(string) ||  t == typeof(Dialogue):
            //            TextField Text = DSUtilities.CreateTextField(
            //            data.Value,
            //            onChange: callback =>
            //            {
            //                TextField target = callback.target as TextField;
            //                target.value = callback.newValue;
            //                data.Value = callback.newValue;
            //                port.SetValue(callback.newValue);
            //            },
            //            styles: new string[]
            //                {
            //                    "ds-node__textfield",
            //                    "ds-node__choice-textfield",
            //                    "ds-node__textfield__hidden"
            //                }
            //            );
            //            port.Add(Text);
            //            break;

            //        case Type t when t == typeof(double):
            //            def = 0;
            //            if (float.TryParse(data.Value, out def)) { }
            //            FloatField floatField2 = DSUtilities.CreateFloatField(
            //            def,
            //            onChange: callback =>
            //            {
            //                FloatField target = callback.target as FloatField;
            //                target.value = callback.newValue;
            //                data.Value = callback.newValue.ToString();
            //                port.SetValue(callback.newValue);
            //            },
            //            styles: new string[]
            //                {
            //                    "ds-node__floatfield",
            //                    "ds-node__choice-textfield",
            //                    "ds-node__textfield__hidden"
            //                }
            //            );
            //            port.Add(floatField2);
            //            break;

            //        default:
            //            Console.WriteLine("Неизвестный тип");
            //            break;
            //    }
            //}
            //if (data.Cross)
            //{
            //    Button crossBtn = DSUtilities.CreateButton(
            //    "X",
            //    () =>
            //    {
            //        port.OnDistroy();
            //        if (!string.IsNullOrEmpty(data.IfPortSourceId) && !string.IsNullOrWhiteSpace(data.IfPortSourceId))
            //        {
            //            BasePort sourcePort = graphView.GetPortById(data.IfPortSourceId);
            //            sourcePort.IfPortSource = null;
            //        }
            //        if (data.IsInput)
            //        {
            //            if (Model.Inputs.Count == Model.Minimal)
            //                return;
            //        }
            //        else if (Model.Outputs.Count == Model.Minimal) return;

            //        if (port.connected)
            //        {
            //            var connect = port.connections.ToList();
            //            for (int i = 0; i < connect.Count(); i++)
            //            {
            //                BasePort inp = connect[i].input as BasePort;
            //                BasePort outp = connect[i].output as BasePort;
            //                inp.Disconnect(connect[i]);
            //                outp.Disconnect(connect[i]);
            //            }
            //            graphView.DeleteElements(port.connections);
            //        }
            //        if (data.IsInput)
            //        {
            //            Model.Inputs.Remove(data);
            //            inputPorts.Remove(port);
            //        }
            //        else
            //        {
            //            Model.Outputs.Remove(data);
            //            outputPorts.Remove(port);
            //        }
            //        graphView.RemoveElement(port);
            //    },
            //    styles: new string[]
            //    {
            //        "ds-node__button"
            //    });

            //    port.Add(crossBtn);
            //}
            //if (data.PlusIf)
            //{
            //    Button plusBtn = DSUtilities.CreateButton(
            //    "+if",
            //    () =>
            //    {
            //        var t = AddPortByType(
            //            ID: Guid.NewGuid().ToString(),
            //            portText: $"If({DSConstants.Bool})",
            //            portSide: PortSide.Input,
            //            type: typeof(bool),
            //            value: "Choice",
            //            isInput: false,
            //            isSingle: false,
            //            isField: false,
            //            cross: true,
            //            isIfPort: true,
            //            availableTypes: new Type[] { typeof(bool) },
            //            ifPortSourceId: port.ID);
            //    },
            //    styles: new string[]
            //    {
            //        "ds-node__button"
            //    });

            //    port.Add(plusBtn);
            //}
            //if (data.IsIfPort)
            //{
            //    if (string.IsNullOrEmpty(data.IfPortSourceId))
            //    {
            //        var outputs = outputContainer.Children().ToList();
            //        BasePort lastPort = outputs[outputs.Count - 1] as BasePort;
            //        port.IfPortSource = lastPort;
            //        lastPort.IfPortSource = port;
            //        lastPort.Add(port);
            //        data.IfPortSourceId = lastPort.ID;
            //    }
            //    else
            //    {
            //        BasePort outPort = GetOutputPorts().Where(x => x != null && x.ID == data.IfPortSourceId).FirstOrDefault();
            //        if (outPort != null)
            //        {
            //            outPort.Add(port);
            //            port.IfPortSource = outPort;
            //            outPort.IfPortSource = port;
            //        }

            //    }
            //}
            //else
            //{
            //    if (data.IsInput)
            //    {
            //        inputContainer.Add(port);
            //        inputPorts.Add(port);
            //    }
            //    else
            //    {
            //        outputContainer.Add(port);
            //        outputPorts.Add(port);
            //    }
            //}

            if (data.Direction == Direction.Input)
            {
                inputContainer.Add(port);
                inputPorts.Add(port);
            }
            else
            {
                outputContainer.Add(port);
                outputPorts.Add(port);
            }

            return port;
        }

        internal protected BasePort GetFirstOutput() => GetOutputPorts().First();
        internal protected IEnumerable<BasePort> GetInputPorts() => inputPorts;
        internal protected IEnumerable<BasePort> GetOutputPorts() => outputPorts;
        #endregion

    }

    internal class PortInfo
    {
        internal string PortName;
        internal Type Type;
        internal Port.Capacity Capacity;
        internal Direction Direction;
        internal Orientation Orientation;
        internal BaseInteractionAction InteractionAction;
        internal BaseNode Node;
    }
}
