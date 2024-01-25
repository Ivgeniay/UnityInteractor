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
using UnityEditor.UIElements;
using UnityEditor;

namespace NodeEngine.Utilities
{
    public class DSTitle
    {
        private const string TITLE_UXML_LINK = "Assets/Plugins/InteractionSystem/Editor/Resources/Front/Title/Title.uxml";
        private static VisualTreeAsset TitleOnNode;

        public VisualElement Root;

        public TextField TitleTextField;
        public Label StatusLabel;
        public VisualElement Lamb;

        public DSTitle()
        {
            if (TitleOnNode == null) TitleOnNode = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TITLE_UXML_LINK);

            Root = new VisualElement();
            TitleOnNode.CloneTree(Root);

            TitleTextField = Root.Q<TextField>("textField");
            StatusLabel = Root.Q<Label>("statusLabel");
            Lamb = Root.Q("lamb");
        }

        public void SetTFCallback(EventCallback<ChangeEvent<string>> onChange) => TitleTextField.RegisterValueChangedCallback(onChange); 
        public void SetTFStyles(string[] styles) => TitleTextField.AddToClassList(styles);
        public void SetStatus(string value) => StatusLabel.text = value;
        public void SetLambColor(Color color) => Lamb.style.backgroundColor = color;
    }

    public static class DSUtilities
    { 
        public static DSTitle CreateTitle()
        {
            return new DSTitle();
        } 

        internal static Vector3Field CreateVectorField(Vector3 value, string label = "", EventCallback<ChangeEvent<Vector3>> onChange = null, string[] styles = null)
        {
            Vector3Field field = new Vector3Field()
            {
                label = label,
                value = value
            };
            if (onChange != null) field.RegisterCallback(onChange);
            field.AddToClassList(styles);
            return field;
        }

        internal static ObjectField CreateUnityObjectField(string label = "", UnityEngine.Object value = null, EventCallback<ChangeEvent<UnityEngine.Object>> onChange = null, string[] styles = null)
        {
            ObjectField field = new ObjectField()
            {
                label = label,
                value = value,
            };
            if (onChange != null) field.RegisterCallback(onChange);
            field.AddToClassList(styles);
            return field;
        }

        internal static DropdownField CreateDropdownField(string label = "", string value = null, List<string> choices = null, EventCallback<ChangeEvent<string>> onChange = null, string[] styles = null)
        {
            DropdownField dd = new DropdownField()
            {
                value = value,
                label = label,
                choices = choices
            };
            if (onChange != null) dd.RegisterCallback(onChange);
            dd.AddToClassList(styles);
            return dd;
        }
        internal static Label CreateLabel(string value = null, EventCallback<ChangeEvent<string>> onClick = null, string[] styles = null)
        {
            Label label = new Label()
            {
                text = value,
            };
            if (onClick != null) label.RegisterCallback(onClick);
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
            if (onChange != null) toggle.RegisterValueChangedCallback(onChange);
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
            if (onChange != null) floatField.RegisterValueChangedCallback(onChange);
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
            if (onChange != null) integerField.RegisterValueChangedCallback(onChange);
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
            if (onChange != null) progressBar.RegisterValueChangedCallback(onChange);
            progressBar.AddToClassList(styles);
            return progressBar;
        }
        internal static TextField CreateTextField (string value = null, string label = null, EventCallback<ChangeEvent<string>> onChange = null, string[] styles = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label,
            };

            if (onChange != null) textField.RegisterValueChangedCallback(onChange);
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
            DSTextField textField = new DSTextField()
            {
                value = value,
                label = label,
            };
            textField.Initialize(graphView);
            if (onChange != null) textField.RegisterValueChangedCallback(onChange);
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
        
    }
}
