using InteractionSystem;
using NodeEngine.Actor;
using NodeEngine.Groups;
using NodeEngine.Nodes;
using NodeEngine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEngine.Window
{
    internal class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(DSGraphView graphView)
        {
            this.graphView = graphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<Type> listNodeTypes = DSUtilities.GetListExtendedClasses(typeof(BaseInteractionAction));
            List<ExtendedDO> dtos = GenerateExtendedDOList(typeof(BaseInteractionAction), listNodeTypes);
            dtos = dtos.OrderBy((a) => a.Depth).ToList();

            List<SearchTreeEntry> searchTreeEntries = new();

            CreateMenuTitle(searchTreeEntries, "Create Element");
            CreateMenuItem(searchTreeEntries, "Nodes", 1);

            GenerateNodeMenu(searchTreeEntries, dtos);

            CreateMenuItem(searchTreeEntries, "Group", 1);
            CreateMenuChoice(searchTreeEntries, "Simple Group", 2, new Type[] { typeof(BaseGroup) }, indentationIcon);

            try
            {
                var assembly = Assembly.Load("Assembly-CSharp");
                if (assembly != null)
                {
                    var actors = DSUtilities.GetListExtendedIntefaces(typeof(IActor), assembly);
                    if (actors != null && actors.Count > 0)
                    {
                        CreateMenuItem(searchTreeEntries, "Actors", 1);
                        actors.ForEach(a => CreateMenuChoice(searchTreeEntries, DSUtilities.GenerateWindowSearchNameFromType(a), 2, new Type[] { typeof(IActor), a }, indentationIcon));
                    } 
                } 
            }
            catch (Exception ex)
            { 
                Debug.LogException(ex);
                Debug.Log("There is no assembly (\"Assembly-CSharp\")");
            }
            return searchTreeEntries;
        }

        private void GenerateNodeMenu(List<SearchTreeEntry> searchTreeEntries, List<ExtendedDO> dtos)
        {
            int counter = 0;
            while (dtos.Count > 0)
            {
                if (counter > 200) break;

                ExtendedDO abstractChoice = dtos.FirstOrDefault(dtos => dtos.IsAbstract);
                
                if (abstractChoice == null)
                {
                    ExtendedDO dto = dtos[0];
                    CreateMenuChoice(searchTreeEntries, DSUtilities.GenerateWindowSearchNameFromType(dto.Type), dto.Depth, new Type[] { dto.Type }, indentationIcon);
                    dtos.Remove(dto);
                }
                else
                {
                    dtos.Remove(abstractChoice);
                    GenerateAbstractNodeMenuChain(searchTreeEntries, abstractChoice, dtos);
                }
                counter++;
            }
        }
        private void GenerateAbstractNodeMenuChain(List<SearchTreeEntry> searchTreeEntries, ExtendedDO dto, List<ExtendedDO> dtos)
        {
            CreateMenuItem(searchTreeEntries, DSUtilities.GenerateWindowSearchNameFromType(dto.Type), dto.Depth);
            foreach (Type type in dto.Types) 
            {
                ExtendedDO dto2 = dtos.Where(e => e.Type ==  type).FirstOrDefault();
                if (dto2 == null) continue;
                
                dtos.Remove(dto2);
                if (dto2.IsAbstract)
                {
                    GenerateAbstractNodeMenuChain(searchTreeEntries, dto2, dtos);
                }
                else
                {
                    CreateMenuChoice(searchTreeEntries, DSUtilities.GenerateWindowSearchNameFromType(dto2.Type), dto2.Depth, new Type[] { dto2.Type }, indentationIcon);
                }
            }
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
            Type[] data = SearchTreeEntry.userData as Type[];
            Type type = data[0];

            if (type == typeof(BaseGroup))
            {
                graphView.CreateGroup(type, localMousePosition);
                return true;
            }
            else if (typeof(BaseInteractionAction).IsAssignableFrom(type))
            {
                BaseInteractionAction interactionAction = Activator.CreateInstance(type) as BaseInteractionAction;
                graphView.InteractionInstance.AddAction(interactionAction);
                var node = graphView.CreateNode(typeof(ActionNode), localMousePosition, interactionAction);
                return true;
            }
            return false;
        }

        private List<SearchTreeEntry> CreateMenuTitle(List<SearchTreeEntry> entries, string title)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(title)));
            return entries;
        }
        private List<SearchTreeEntry> CreateMenuItem(List<SearchTreeEntry> entries, string itemName, int layer)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(itemName), layer));
            return entries;
        }
        private List<SearchTreeEntry> CreateMenuChoice(List<SearchTreeEntry> entries, string itemName, int layer, object context, Texture2D indentationIcon = null)
        {
            entries.Add(new SearchTreeEntry(new GUIContent(new GUIContent(itemName, indentationIcon)))
            {
                level = layer,
                userData = context
            });
            return entries;
        }

        static List<ExtendedDO> GenerateExtendedDOList(Type baseType, List<Type> types)
        {
            List<ExtendedDO> extendedDOList = new List<ExtendedDO>();
            foreach (Type type in types)
            {
                int depth = 2;
                Type currentType = type.BaseType;

                while (currentType != baseType)
                {
                    depth++;
                    currentType = currentType.BaseType;
                }

                List<Type> derivedTypes = types
                    .Where(t => t.BaseType == type)
                    .ToList();

                ExtendedDO extendedDO = new ExtendedDO
                {
                    IsAbstract = type.IsAbstract,
                    Depth = depth,
                    Type = type,
                    FatherType = type.BaseType,
                    Types = derivedTypes
                };

                extendedDOList.Add(extendedDO);
            }

            return extendedDOList;
        }
    }

    public class ExtendedDO
    {
        public int Depth { get; set; }
        public bool IsAbstract { get; set; } = false;
        public Type Type { get; set; }
        public Type FatherType { get; set; }
        public List<Type> Types { get; set; } = new List<Type>();
    }
}
