using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using NodeEngine.Utilities;
using System.Reflection; 
using NodeEngine.Window;
using System.Collections.Generic;
using NodeEngine.Ports; 
using NodeEngine.Nodes;
using UnityEngine;
using System.Linq;
using System;

namespace InteractionSystem
{
    internal class ActionNode : BaseNode
    {
        public override BaseInteractionAction IAction { get => INode as BaseInteractionAction; }
        List<VisualElement> serializedVisualElements = new List<VisualElement>();

        internal override void Initialize(DSGraphView graphView, Vector2 position, INode _iAction)
        {
            base.Initialize(graphView, position, _iAction);
           
            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction,
                Orientation = Orientation.Horizontal,
                PortName = DSConstants.ACTION_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction.ReferenceAction,
                Orientation = Orientation.Horizontal,
                PortName = DSConstants.REFERENCE_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single,
                InteractionAction = IAction.ParallelAction,
                Orientation = Orientation.Horizontal,
                PortName = DSConstants.PARALLEL_PN
            });

            Model.AddPort(new PortInfo()
            {
                Type = typeof(BaseInteractionAction),
                Direction = Direction.Output,
                Capacity = Port.Capacity.Multi,
                InteractionAction = IAction.NextIAction,
                Orientation = Orientation.Horizontal,
                PortName = DSConstants.NEXT_PN
            });
        }

        protected override void DrawMainContainer(VisualElement container)
        {
            base.DrawMainContainer(container);

            ExecuteAttributes(container);
        }


        public override void OnConnectOutputPort(BasePort port, Edge edge)
        {
            base.OnConnectOutputPort(port, edge);
            BasePort otherPort = edge.input as BasePort;
            if (otherPort != null)
            {
                if (otherPort.Name == DSConstants.ACTION_PN)
                {
                    port.Value = otherPort.Value;
                    IAction.NextIAction = port.Value;

                }
                else if (otherPort.Name == DSConstants.PARALLEL_PN)
                {
                    BaseNode otherNode = otherPort.node as BaseNode;
                    if (otherNode.IAction != null)
                    {
                        VisualElement dd = serializedVisualElements.Where(e => e.name == nameof(IAction.PerformerType)).FirstOrDefault();
                        if (dd != null && dd is DropdownField ddField)
                        {
                            if (otherNode.IAction.PerformerType == PerformerType.Object)
                                ddField.value = PerformerType.Subject.ToString();
                                
                            else if (otherNode.IAction.PerformerType == PerformerType.Subject)
                                ddField.value = PerformerType.Object.ToString();
                        }
                    }
                }
            }
        }

        public override void OnDestroyConnectionOutput(BasePort port, Edge edge)
        {
            base.OnDestroyConnectionOutput(port, edge);

            port.Value = null;
            IAction.NextIAction = null;
        }

        public override void OnConnectInputPort(BasePort port, Edge edge)
        {
            base.OnConnectInputPort(port, edge);

            BaseNode otherNode = edge.output.node as BaseNode;
            if (otherNode != null)
            {
                if (port.Name == DSConstants.REFERENCE_PN)
                {
                    port.Value = otherNode.IAction;
                    IAction.ReferenceAction = port.Value;
                }
                else if (port.Name == DSConstants.PARALLEL_PN)
                {
                    port.Value = otherNode.IAction;
                    IAction.ParallelAction = port.Value;
                }
            }
        }

        public override void OnDestroyConnectionInput(BasePort port, Edge edge)
        {
            base.OnDestroyConnectionInput(port, edge);

            if (port.Name == DSConstants.REFERENCE_PN)
                IAction.ReferenceAction = null;

            else if (port.Name == DSConstants.PARALLEL_PN)
                IAction.ParallelAction = null;
        }

        private void ExecuteAttributes(VisualElement container)
        {
            foreach (FieldInfo field in NodeContext.FieldsSerAttribute)
            {
                switch (field.FieldType)
                {
                    case Type type when type.Equals(typeof(string)):

                        TextField textField = DSUtilities.CreateTextField(
                            (string)NodeContext.GetValue(field),
                            field.Name,
                            (e) =>
                            {
                                TextField trgt = e.target as TextField;
                                trgt.value = e.newValue == null ? "" : e.newValue;
                                NodeContext.SetValue(field, e.newValue);
                                graphView.SafeDirty();
                            });

                        textField.name = field.Name;
                        serializedVisualElements.Add(textField);
                        container.Add(textField);
                        break;

                    case Type type when type.Equals(typeof(float)):
                            FloatField floatTF = DSUtilities.CreateFloatField(
                                (float)NodeContext.GetValue(field),
                                field.Name, (e) =>
                                {
                                    FloatField trgt = e.target as FloatField;
                                    trgt.value = e.newValue;
                                    NodeContext.SetValue(field, e.newValue);
                                    graphView.SafeDirty();
                                });
                            floatTF.name = field.Name;
                            serializedVisualElements.Add(floatTF);
                            container.Add(floatTF);
                        break;

                    case Type type when type.Equals(typeof(int)):
                            IntegerField integerField = DSUtilities.CreateIntegerField(
                                (int)NodeContext.GetValue(field),
                                field.Name, (e) =>
                                {
                                    IntegerField trgt = e.target as IntegerField;
                                    trgt.value = e.newValue;
                                    NodeContext.SetValue(field, e.newValue);
                                    graphView.SafeDirty();
                                });
                            integerField.name = field.Name;
                            serializedVisualElements.Add(integerField);
                            container.Add(integerField);
                        break;

                    case Type type when type.Equals(typeof(bool)):
                            Toggle boolField = DSUtilities.CreateToggle(
                                label: field.Name,
                                value: (bool)NodeContext.GetValue(field),
                                onChange: (e) =>
                                {
                                    Toggle trgt = e.target as Toggle;
                                    trgt.value = e.newValue;
                                    NodeContext.SetValue(field, e.newValue);
                                    graphView.SafeDirty();
                                });
                            boolField.name = field.Name;
                            serializedVisualElements.Add(boolField);
                            container.Add(boolField);
                        break;

                    case Type type when type.IsEnum:
                        
                            List<string> choices = new();
                            var enums = Enum.GetValues(field.FieldType);
                            foreach (var enumItem in enums)
                                choices.Add(enumItem.ToString());

                            DropdownField ddField = DSUtilities.CreateDropdownField(
                                label: field.Name,
                                value: NodeContext.GetValue(field).ToString(),
                                choices: choices,
                                onChange: (e) =>
                                {
                                    DropdownField trgt = e.target as DropdownField;
                                    trgt.value = e.newValue;
                                    var t = field.FieldType;
                                    var res = Enum.Parse(t, e.newValue);
                                    Type rr = res.GetType();
                                    NodeContext.SetValue(field, res);
                                    graphView.SafeDirty();
                                });
                            ddField.name = field.Name;
                            serializedVisualElements.Add(ddField);
                            container.Add(ddField);
                        
                        break;

                }
            }
            foreach (PropertyInfo property in NodeContext.PropSerAttribute)
            {
                switch (property.PropertyType)
                {
                    case Type type when type.Equals(typeof(string)):

                            TextField textField = DSUtilities.CreateTextField(
                                (string)NodeContext.GetValue(property),
                                property.Name, (e) =>
                                {
                                    TextField trgt = e.target as TextField;
                                    trgt.value = e.newValue == null ? "" : e.newValue;
                                    NodeContext.SetValue(property, e.newValue);
                                    graphView.SafeDirty();
                                });
                            textField.name = property.Name;
                            serializedVisualElements.Add(textField);
                            container.Add(textField);
                        break;

                    case Type type when type.Equals(typeof(float)): 
                            FloatField floatField = DSUtilities.CreateFloatField(
                                (float)NodeContext.GetValue(property),
                                property.Name, (e) =>
                                {
                                    FloatField trgt = e.target as FloatField;
                                    trgt.value = e.newValue;
                                    NodeContext.SetValue(property, e.newValue);
                                    graphView.SafeDirty();
                                });
                            floatField.name = property.Name;
                            serializedVisualElements.Add(floatField);
                            container.Add(floatField);
                        break;

                    case Type type when type.Equals(typeof(int)):
                            IntegerField integerField = DSUtilities.CreateIntegerField(
                                (int)NodeContext.GetValue(property),
                                property.Name, (e) =>
                                {
                                    IntegerField trgt = e.target as IntegerField;
                                    trgt.value = e.newValue;
                                    NodeContext.SetValue(property, e.newValue);
                                    graphView.SafeDirty();
                                });
                            integerField.name = property.Name;
                            serializedVisualElements.Add(integerField);
                            container.Add(integerField);
                        break;

                    case Type type when type.Equals(typeof(bool)):
                            Toggle boolField = DSUtilities.CreateToggle(
                                label: property.Name,
                                value: (bool)NodeContext.GetValue(property),
                                onChange: (e) =>
                                {
                                    Toggle trgt = e.target as Toggle;
                                    trgt.value = e.newValue;
                                    NodeContext.SetValue(property, e.newValue);
                                    graphView.SafeDirty();
                                });
                            boolField.name = property.Name;
                            serializedVisualElements.Add(boolField);
                            container.Add(boolField);
                        
                        break;

                    case Type type when type.IsEnum:
                            List<string> choices = new();
                            var enums = Enum.GetValues(property.PropertyType);
                            foreach (var enumItem in enums)
                                choices.Add(enumItem.ToString());

                            DropdownField ddField = DSUtilities.CreateDropdownField(
                                label: property.Name,
                                value: NodeContext.GetValue(property).ToString(),
                                choices: choices,
                                onChange: (e) =>
                                {
                                    DropdownField trgt = e.target as DropdownField;
                                    trgt.value = e.newValue;
                                    var t = property.PropertyType;
                                    var res = Enum.Parse(t, e.newValue);
                                    NodeContext.SetValue(property, res);
                                    graphView.SafeDirty();
                                });
                            ddField.name = property.Name;
                            serializedVisualElements.Add(ddField);
                            container.Add(ddField);
                        break;
                }
            } 

            List<DescriptionAttribute> t = NodeContext.GetGeneral<DescriptionAttribute>();
            if (t != null && t.Count > 0)
                this.tooltip = t[0].Description;
        }
    }
}
