using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using NodeEngine.UIElement;
using UnityEngine.UIElements;
using NodeEngine.Groups;
using NodeEngine.Window;
using NodeEngine.Nodes;
using NodeEngine.Ports;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;
using InteractionSystem;

namespace NodeEngine.Utilities
{
    public static class DSUtilities
    {
        internal static Label CreateLabel(string value = null, EventCallback<ChangeEvent<string>> onClick = null, string[] styles = null)
        {
            Label label = new Label()
            {
                text = value,
            };
            if (onClick is not null) label.RegisterCallback(onClick);
            label.AddToClassList(styles);
            return label;
        }
        internal static Toggle CreateToggle(string text = null, string label = null, EventCallback<ChangeEvent<bool>> onChange = null, string[] styles = null, bool value = false)
        {
            Toggle toggle = new Toggle
            {
                value = value,
                text = text,
                label = label,
            };
            if (onChange is not null) toggle.RegisterValueChangedCallback(onChange);
            toggle.AddToClassList(styles);
            return toggle;
        }
        internal static FloatField CreateFloatField(float value = 0, string label = null, EventCallback<ChangeEvent<float>> onChange = null, string[] styles = null)
        {
            FloatField floatField = new FloatField()
            {
                value = value,
                label = label
            };
            if (onChange is not null) floatField.RegisterValueChangedCallback(onChange);
            floatField.AddToClassList(styles);
            return floatField;
        }
        internal static IntegerField CreateIntegerField(int value = 0, string label = null, EventCallback<ChangeEvent<int>> onChange = null, string[] styles = null)
        {
            IntegerField integerField = new IntegerField()
            {
                value = value,
                label = label
            };
            if (onChange is not null) integerField.RegisterValueChangedCallback(onChange);
            integerField.AddToClassList(styles);
            return integerField;
        }
        internal static ProgressBar CreateProgressBar(float value = 0, float lowValue = 0, float maxValue = 1, string title = "", EventCallback<ChangeEvent<float>> onChange = null, string[] styles = null)
        {
            ProgressBar progressBar = new ProgressBar()
            {
                lowValue = lowValue,
                highValue = maxValue,
                value = value,
                title = title,
            };
            if (onChange is not null) progressBar.RegisterValueChangedCallback(onChange);
            progressBar.AddToClassList(styles);
            return progressBar;
        }
        internal static TextField CreateTextField (string value = null, string label = null, EventCallback<ChangeEvent<string>> onChange = null, string[] styles = null)
        {
            TextField textField = new()
            {
                value = value,
                label = label,
            };

            if (onChange is not null) textField.RegisterValueChangedCallback(onChange);
            textField.AddToClassList(styles);
            return textField;
        }
        internal static TextField CreateTextArea(string value = null, string label = null, EventCallback < ChangeEvent<string>> onChange = null, string[] styles = null)
        {
            TextField textField = CreateTextField(value, label, onChange, styles);
            textField.multiline = true;
            return textField;
        }

        internal static DSTextField CreateDSTextField(DSGraphView graphView, string value = null, string label = null, EventCallback<ChangeEvent<string>> onChange = null, string[] styles = null)
        {
            DSTextField textField = new()
            {
                value = value,
                label = label,
            };
            textField.Initialize(graphView);
            if (onChange is not null) textField.RegisterValueChangedCallback(onChange);
            textField.AddToClassList(styles);
            return textField;
        }
        internal static DSTextField CreateDSTextArea(DSGraphView graphView, string value = null, string label = null, EventCallback<ChangeEvent<string>> onChange = null, string[] styles = null)
        {
            DSTextField textField = CreateDSTextField(graphView, value, label, onChange, styles);
            textField.multiline = true;
            return textField;
        }

        internal static Foldout CreateFoldout(string title, bool collapsed = false, string[] styles = null)
        {
            var foldout = new Foldout()
            {
                text = title,
                value = !collapsed
            };

            foldout.AddToClassList(styles);
            return foldout;
        }
        internal static Button CreateButton(string text, Action onClick = null, string[] styles = null)
        {
            Button btn = new Button(onClick)
            {
                text = text,
            };
            btn.AddToClassList(styles);
            return btn;
        }
        internal static BasePort CreatePort(this BaseNode baseNode, BaseInteractionAction action, string ID, string portname = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single, Color color = default, Type type = null)
        {
            if (color == default) color = Color.white;
            type = type == null ? typeof(bool) : type;
            BasePort port = baseNode.InstantiatePort(orientation, direction, capacity, type) as BasePort;

            port.portName = portname;
            port.portColor = color;
            port.Value = action;
            port.Type = type;
            port.ID = ID;

            return port;
        }

        internal static BaseGroup CreateGroup(DSGraphView graphView, Type type, Vector2 mousePosition, string title = "Group", string tooltip = null)
        {
            var group = new BaseGroup(title, mousePosition)
            {
                tooltip = tooltip == null ? title : tooltip,
            };

            graphView.AddGroup(group);
            return group;
        }

        internal static BaseNode CreateNode(DSGraphView graphView, Type type, INode baseInteraction, Vector2 position)
        {
            if (typeof(INode).IsAssignableFrom(baseInteraction.GetType()))
            {
                BaseNode node = (BaseNode)Activator.CreateInstance(type);
                node.Initialize(graphView, position, baseInteraction);
                node.OnCreate();

                graphView.AddUngroupedNode(node);
                return node;
            }
            else
                throw new ArgumentException("Type must be derived from BaseNode", nameof(type));
        }

        internal static object GetDefaultValue(Type type)
        {
            if (type == null) throw new ArgumentNullException();
            object result = default(object);
            if (type.IsValueType) result = CreateInstance(type);
            else if (type == typeof(string)) result = string.Empty;
            else result = CreateInstance(type);

            if (result == null)
            {
                string fullTypeName = type.FullName + ", " + DSConstants.DEFAULT_ASSEMBLY;// "Namespace.TypeName, AssemblyName";
                result = CreateInstance(GetType(fullTypeName));
            }
            return result;
        }
        internal static object CreateInstance(Type type)
        {
            if (type.Namespace != null && type.Namespace.StartsWith("UnityEngine")) return null;// .IsAssignableFrom(typeof(UnityEngine.Object))) return null;
            return Activator.CreateInstance(type);
        }

        internal static List<Type> GetListExtendedClasses(Type baseType)
        {
            var nodeTypes = GetListExtendedClasses(baseType, Assembly.GetExecutingAssembly());
            try
            {
                Assembly assemblyCSharp = Assembly.Load("Assembly-CSharp-Editor");
                List<Type> derivedTypesFromCSharp = GetListExtendedClasses(baseType, assemblyCSharp);
                foreach (Type type in derivedTypesFromCSharp)
                    nodeTypes.Add(type);
            }
            catch { }
            return nodeTypes;
        }
        internal static List<Type> GetListExtendedClasses(Type baseType, Assembly assembly) =>
            assembly.GetTypes()
                .Where(t => t != baseType && baseType.IsAssignableFrom(t))
                .ToList();
        internal static List<Type> GetListExtendedIntefaces(Type interfaceType, Assembly assembly) =>
            assembly.GetTypes()
                .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass)
                .ToList();

        internal static bool IsAvalilableType(Type type) => DSConstants.AvalilableTypes.Contains(type);
        internal static bool IsPrimitiveType(Type type) => DSConstants.PrimitiveTypes.Contains(type);
        internal static string GenerateWindowSearchNameFromType(Type t)
        {
            var name = t.Name.Replace("node", "", StringComparison.OrdinalIgnoreCase);
            name = name.Replace("base", "", StringComparison.OrdinalIgnoreCase);
            name = char.ToUpper(name[0]) + name.Substring(1);
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    name = name.Insert(i, " ");
                    i++;
                }
            }
            return name;
        }

        internal static string GenerateClassNameFromType(Type t)
        {
            var name = t.Name.Replace("node", "", StringComparison.OrdinalIgnoreCase);
            name = name.Replace("base", "", StringComparison.OrdinalIgnoreCase);
            name = char.ToUpper(name[0]) + name.Substring(1);
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    name = name.Insert(i, "_");
                    i++;
                }
            }
            name = name.Insert(0, "DS");
            return name;
        }
        internal static string GenerateClassPefixFromType(Type t)
        {
            switch (t)
            {
                case var _ when t == typeof(byte): return "b";
                case var _ when t == typeof(int): return "i";
                case var _ when t == typeof(long): return "l";
                case var _ when t == typeof(short): return "sh";
                case var _ when t == typeof(decimal): return "de";
                case var _ when t == typeof(double): return "d";
                case var _ when t == typeof(float): return "f";
                case var _ when t == typeof(string): return "s";
                case var _ when t == typeof(char): return "c";
                case var _ when t == typeof(bool): return "b";
                default: return t.Name.Length >= 2 ? t.Name.Substring(0, 2) : t.Name;
            }
        }

        internal static Type GetType(string fullTypeName)
        {
            Type type = null;
            if (!fullTypeName.Contains("["))
            {
                type = Type.GetType(fullTypeName);
                if (type != null) return type;

                Assembly assembly = Assembly.Load(DSConstants.DEFAULT_ASSEMBLY);
                type = assembly.GetType(fullTypeName, true);
                if (type != null) return type;
            }
            else
            {
                string[] typeParts = fullTypeName.Split('[');
                string typeNameWithoutGeneric = typeParts[0];
                string genericArgumentPart = typeParts[1].TrimEnd(']');

                string fullTypeName_ = typeNameWithoutGeneric + "[" + DSConstants.DEFAULT_ASSEMBLY + "." + genericArgumentPart + "]";

                Type argType = Type.GetType(genericArgumentPart);
                if (argType == null)
                {
                    Assembly assembly = Assembly.Load(DSConstants.DEFAULT_ASSEMBLY);
                    argType = assembly.GetType(genericArgumentPart, true);
                }
                if (argType == null) throw new Exception();

                if (typeNameWithoutGeneric.Contains("List"))
                {
                    type = typeof(List<>).MakeGenericType(argType);
                }

                return type;
            }

            throw new Exception($"There is no type {fullTypeName}");
        }
    }
}
