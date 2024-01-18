using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using NodeEngine.Utilities;
using System.Reflection; 
using NodeEngine.Window;
using System.Collections.Generic;
using UnityEditor.UIElements;
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

            IAction.OnExecutingEvent += OnExecutingHandler;
           
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

        public override void OnDestroy()
        {
            IAction.OnExecutingEvent -= OnExecutingHandler;
            base.OnDestroy();
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

            BasePort otherPort = edge.input as BasePort;
            if (otherPort != null)
            {
                if (otherPort.Name == DSConstants.ACTION_PN)
                {
                    port.Value = null;
                    IAction.NextIAction = null;
                }
            }
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
                    IAction.ReferenceAction = otherNode.IAction;
                }
                else if (port.Name == DSConstants.PARALLEL_PN)
                {
                    port.Value = otherNode.IAction;
                    IAction.ParallelAction = otherNode.IAction;
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

        #region Attributes
        private void ExecuteAttributes(VisualElement container)
        {
            foreach (FieldInfo field in NodeContext.FieldsSerAttribute)
            {
                var veField = CreateVEFromSerialized(field);
                if (veField != null)
                {
                    veField.name = field.Name;
                    serializedVisualElements.Add(veField);
                    container.Add(veField);
                }
            }
            foreach (PropertyInfo property in NodeContext.PropSerAttribute)
            {
                var veProperty = CreateVEFromSerialized(property);
                if (veProperty != null)
                {
                    veProperty.name = property.Name;
                    serializedVisualElements.Add(veProperty);
                    container.Add(veProperty);
                }
            } 

            List<DescriptionAttribute> t = NodeContext.GetGeneral<DescriptionAttribute>();
            if (t != null && t.Count > 0)
                this.tooltip = t[0].Description;
        }

        private VisualElement CreateVEFromSerialized(FieldInfo field)
        {
            string tooltip = string.Empty;
            if (Attribute.GetCustomAttribute(field, typeof(SerializeFieldNode)) is SerializeFieldNode attribute)
                tooltip = attribute.Description;

            switch (field.FieldType)
            {
                case Type type when type.Equals(typeof(string)):
                    var tf = DSUtilities.CreateTextField(
                        (string)NodeContext.GetValue(field),
                        field.Name,
                        onChange: (e) =>
                        {
                            TextField trgt = e.target as TextField;
                            trgt.value = e.newValue == null ? "" : e.newValue;
                            NodeContext.SetValue(field, e.newValue);
                            graphView.SafeDirty();
                        });
                    tf.tooltip = tooltip;
                    return tf;

                case Type type when type.Equals(typeof(float)):
                    var ff = DSUtilities.CreateFloatField(
                        (float)NodeContext.GetValue(field),
                        field.Name, (e) =>
                        {
                            FloatField trgt = e.target as FloatField;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(field, e.newValue);
                            graphView.SafeDirty();
                        });
                    ff.tooltip = tooltip;
                    return ff;

                case Type type when type.Equals(typeof(int)):
                    var intf = DSUtilities.CreateIntegerField(
                        (int)NodeContext.GetValue(field),
                        field.Name, (e) =>
                        {
                            IntegerField trgt = e.target as IntegerField;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(field, e.newValue);
                            graphView.SafeDirty();
                        });
                    intf.tooltip = tooltip;
                    return intf;

                case Type type when type.Equals(typeof(bool)):
                    var bf = DSUtilities.CreateToggle(
                        label: field.Name,
                        value: (bool)NodeContext.GetValue(field),
                        onChange: (e) =>
                        {
                            Toggle trgt = e.target as Toggle;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(field, e.newValue);
                            graphView.SafeDirty();
                        });
                    bf.tooltip = tooltip;
                    return bf;

                case Type type when type.Equals(typeof(Vector3)):
                    var vecf = DSUtilities.CreateVectorField(
                        label: field.Name,
                        value: (Vector3)NodeContext.GetValue(field),
                        onChange: (e) =>
                        {
                            Vector3Field trgt = e.target as Vector3Field;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(field, e.newValue);
                            graphView.SafeDirty();
                        });

                    vecf.tooltip = tooltip;
                    return vecf;

                case Type type when type.Equals(typeof(UnityEngine.GameObject)):
                    var objF = DSUtilities.CreateUnityObjectField(
                        label: field.Name,
                        value: (UnityEngine.Object)NodeContext.GetValue(field),
                        onChange: (e) =>
                        {
                            ObjectField trgt = e.target as ObjectField;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(field, e.newValue);
                            graphView.SafeDirty();
                        });

                    objF.tooltip = tooltip;
                    return objF;

                case Type type when type.IsEnum:

                    List<string> choices = new();
                    var enums = Enum.GetValues(field.FieldType);
                    foreach (var enumItem in enums)
                        choices.Add(enumItem.ToString());

                    var ddf = DSUtilities.CreateDropdownField(
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

                    ddf.tooltip = tooltip;
                    return ddf;
            }
            return null;
        }
        private VisualElement CreateVEFromSerialized(PropertyInfo property)
        {
            string tooltip = string.Empty;
            if (Attribute.GetCustomAttribute(property, typeof(SerializeFieldNode)) is SerializeFieldNode attribute)
                tooltip = attribute.Description;

            switch (property.PropertyType)
            {
                case Type type when type.Equals(typeof(string)): 
                    var tf =  DSUtilities.CreateTextField(
                        (string)NodeContext.GetValue(property),
                        property.Name, (e) =>
                        {
                            TextField trgt = e.target as TextField;
                            trgt.value = e.newValue == null ? "" : e.newValue;
                            NodeContext.SetValue(property, e.newValue);
                            graphView.SafeDirty();
                        });
                    tf.tooltip = tooltip;
                    return tf;

                case Type type when type.Equals(typeof(float)):
                    var ff=  DSUtilities.CreateFloatField(
                        (float)NodeContext.GetValue(property),
                        property.Name, (e) =>
                        {
                            FloatField trgt = e.target as FloatField;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(property, e.newValue);
                            graphView.SafeDirty();
                        });
                    ff.tooltip = tooltip;
                    return ff;

                case Type type when type.Equals(typeof(int)):
                    var intF = DSUtilities.CreateIntegerField(
                        (int)NodeContext.GetValue(property),
                        property.Name, (e) =>
                        {
                            IntegerField trgt = e.target as IntegerField;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(property, e.newValue);
                            graphView.SafeDirty();
                        });
                    intF.tooltip = tooltip;
                    return intF;

                case Type type when type.Equals(typeof(bool)):
                    var togleF = DSUtilities.CreateToggle(
                        label: property.Name,
                        value: (bool)NodeContext.GetValue(property),
                        onChange: (e) =>
                        {
                            Toggle trgt = e.target as Toggle;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(property, e.newValue);
                            graphView.SafeDirty();
                        });
                    togleF.tooltip = tooltip;
                    return togleF;

                case Type type when type.Equals(typeof(Vector3)):
                    var vecF = DSUtilities.CreateVectorField(
                        label: property.Name,
                        value: (Vector3)NodeContext.GetValue(property),
                        onChange: (e) =>
                        {
                            Vector3Field trgt = e.target as Vector3Field;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(property, e.newValue);
                            graphView.SafeDirty();
                        });
                    vecF.tooltip = tooltip;
                    return vecF;

                case Type type when type.Equals(typeof(UnityEngine.Object)):
                    var objF = DSUtilities.CreateUnityObjectField(
                        label: property.Name,
                        value: (UnityEngine.Object)NodeContext.GetValue(property),
                        onChange: (e) =>
                        {
                            ObjectField trgt = e.target as ObjectField;
                            trgt.value = e.newValue;
                            NodeContext.SetValue(property, e.newValue);
                            graphView.SafeDirty();
                        });
                    objF.tooltip = tooltip;
                    return objF;

                case Type type when type.IsEnum:
                    List<string> choices = new();
                    var enums = Enum.GetValues(property.PropertyType);
                    foreach (var enumItem in enums)
                        choices.Add(enumItem.ToString());

                    var ddf = DSUtilities.CreateDropdownField(
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
                    ddf.tooltip = tooltip;
                    return ddf;
            }
            return null;
        }

        private void SetValue(VisualElement elem, object value)
        {
            if (elem is TextField tf) tf.value = (string)value;
            else if (elem is FloatField ff) ff.value = (float)value;
            else if (elem is IntegerField @if) @if.value = (int)value;
            else if (elem is Toggle bf) bf.value = (bool)value;
            else if (elem is ObjectField of) of.value = (UnityEngine.Object)value;
            else if (elem is Vector3Field vf3) vf3.value = (Vector3)value;
        }

        #endregion
        private void OnExecutingHandler(BaseInteractionAction arg1, Sequence.ActionExecutionType arg2)
        {
            foreach (FieldInfo field in NodeContext.FieldsSerAttribute)
            {
                VisualElement ve = serializedVisualElements.FirstOrDefault(e => e.name == field.Name);
                if (ve != null) SetValue(ve, field.GetValue(arg1));
            }
            foreach (PropertyInfo property in NodeContext.PropSerAttribute)
            {
                VisualElement ve = serializedVisualElements.FirstOrDefault(e => e.name == property.Name);
                if (ve != null) SetValue(ve, property.GetValue(arg1));
            }
        }
    }
}
