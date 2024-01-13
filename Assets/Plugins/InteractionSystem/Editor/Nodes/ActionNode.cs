using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using NodeEngine.Utilities;
using System.Reflection; 
using NodeEngine.Window;
using NodeEngine.Ports; 
using NodeEngine.Nodes;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace InteractionSystem
{
    internal class ActionNode : BaseNode
    {
        public override BaseInteractionAction IAction { get => INode as BaseInteractionAction; }

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

            AddAttributes(container);
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
                        if (otherNode.IAction.PerformerType == PerformerType.Object) IAction.PerformerType = PerformerType.Subject;
                        else if (otherNode.IAction.PerformerType == PerformerType.Subject) IAction.PerformerType = PerformerType.Object;
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


        private void AddAttributes(VisualElement container)
        {
            FieldInfo[] stringFields = NodeContext.GetFields(typeof(StringFieldContextAttribute));
            FieldInfo[] floatFields = NodeContext.GetFields(typeof(FloatFieldContextAttribute));
            FieldInfo[] integersFields = NodeContext.GetFields(typeof(IntFieldContextAttribute));
            FieldInfo[] boolFields = NodeContext.GetFields(typeof(BoolFieldContextAttribute));

            FieldInfo[] enumFields = NodeContext.GetFields(typeof(EnumFieldContextAttribute));
            PropertyInfo[] enumProperty = NodeContext.GetProperties(typeof(EnumPropContextAttribute));

            PropertyInfo[] textProp = NodeContext.GetProperties(typeof(StringFieldContextAttribute));
            PropertyInfo[] flProp = NodeContext.GetProperties(typeof(FloatPropContextAttribute));
            PropertyInfo[] integersProp = NodeContext.GetProperties(typeof(IntPropContextAttribute)); 
            PropertyInfo[] boolProp = NodeContext.GetProperties(typeof(BoolPropContextAttribute)); 


            foreach (FieldInfo field in stringFields)
            {
                TextField textField = DSUtilities.CreateTextField(
                    (string)NodeContext.GetValue(field),
                    field.Name, (e) =>
                    {
                        TextField trgt = e.target as TextField;
                        trgt.value = e.newValue == null ? "" : e.newValue;
                        NodeContext.SetValue(field, e.newValue);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }
            foreach (PropertyInfo prop in textProp)
            {
                TextField textField = DSUtilities.CreateTextField(
                    (string)NodeContext.GetValue(prop),
                    prop.Name, (e) =>
                    {
                        TextField trgt = e.target as TextField;
                        trgt.value = e.newValue == null ? "" : e.newValue;
                        NodeContext.SetValue(prop, e.newValue);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }
            foreach (FieldInfo field in floatFields)
            {
                FloatField textField = DSUtilities.CreateFloatField(
                    (float)NodeContext.GetValue(field),
                    field.Name, (e) =>
                    {
                        FloatField trgt = e.target as FloatField;
                        trgt.value = e.newValue;
                        NodeContext.SetValue(field, e.newValue);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }
            foreach (PropertyInfo prop in flProp)
            {
                FloatField textField = DSUtilities.CreateFloatField(
                    (float)NodeContext.GetValue(prop),
                    prop.Name, (e) =>
                    {
                        FloatField trgt = e.target as FloatField;
                        trgt.value = e.newValue;
                        NodeContext.SetValue(prop, e.newValue);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }
            foreach (FieldInfo field in integersFields)
            {
                IntegerField textField = DSUtilities.CreateIntegerField(
                    (int)NodeContext.GetValue(field),
                    field.Name, (e) =>
                    {
                        IntegerField trgt = e.target as IntegerField;
                        trgt.value = e.newValue;
                        NodeContext.SetValue(field, e.newValue);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }
            foreach (PropertyInfo prop in integersProp)
            {
                IntegerField textField = DSUtilities.CreateIntegerField(
                    (int)NodeContext.GetValue(prop),
                    prop.Name, (e) =>
                    {
                        IntegerField trgt = e.target as IntegerField;
                        trgt.value = e.newValue;
                        NodeContext.SetValue(prop, e.newValue);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }
            foreach (FieldInfo field in boolFields)
            {
                Toggle textField = DSUtilities.CreateToggle(
                    label: field.Name,
                    value: (bool)NodeContext.GetValue(field),
                    onChange: (e) =>
                    {
                        Toggle trgt = e.target as Toggle;
                        trgt.value = e.newValue;
                        NodeContext.SetValue(field, e.newValue);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }
            foreach (PropertyInfo field in boolProp)
            {
                Toggle textField = DSUtilities.CreateToggle(
                    label: field.Name,
                    value: (bool)NodeContext.GetValue(field),
                    onChange: (e) =>
                    {
                        Toggle trgt = e.target as Toggle;
                        trgt.value = e.newValue;
                        NodeContext.SetValue(field, e.newValue);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }

            foreach (FieldInfo field in enumFields)
            {
                List<string> choices = new();
                var enums = Enum.GetValues(field.FieldType);
                foreach (var item in enums)
                    choices.Add(item.ToString());
                
                 
                DropdownField textField = DSUtilities.CreateDropdownField(
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
                container.Add(textField);
            }
            foreach (PropertyInfo field in enumProperty)
            {
                List<string> choices = new();
                var enums = Enum.GetValues(field.PropertyType);
                foreach (var item in enums)
                    choices.Add(item.ToString());

                DropdownField textField = DSUtilities.CreateDropdownField(
                    label: field.Name,
                    value: NodeContext.GetValue(field).ToString(),
                    choices: choices,
                    onChange: (e) =>
                    {
                        DropdownField trgt = e.target as DropdownField;
                        trgt.value = e.newValue;
                        var t = field.PropertyType;
                        var res = Enum.Parse(t, e.newValue);
                        NodeContext.SetValue(field, res);
                        graphView.SafeDirty();
                    });
                container.Add(textField);
            }

            var t = NodeContext.GetGeneral<DescriptionAttribute>();
            if (t != null && t.Count > 0)
                this.tooltip = t[0].Description;
            
        }
    }
}
