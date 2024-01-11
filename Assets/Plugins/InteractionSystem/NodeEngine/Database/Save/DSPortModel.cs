using UnityEngine;
using System;
using System.Collections.Generic;
using NodeEngine.Ports;
using NodeEngine.Nodes;
using System.Linq;

namespace NodeEngine.Database.Save
{
    [Serializable]
    internal class DSPortModel
    {
        //Собственный ID для сериализации из памяти и соединения с другим портом
        [SerializeField] public string PortID;
        //ID ноды к которой присоединен
        [SerializeField] public List<NodePortModel> NodeIDs;
        //Значение по-умолчанию или в процессе работы внутри порта
        //[SerializeField] public object Value;
        [SerializeField] public string Value;
        //Порт текст
        [SerializeField] public string PortText;
        //Количество присоединяемых соединение (Одиночное, множественное)
        [SerializeField] public bool IsSingle;
        //Является ли порт инпутом. (input/output)
        [SerializeField] public bool IsInput;
        //Является ли порт присоединяемым к другому порту (IfPort)
        [SerializeField] public bool IsIfPort;
        //Output id для к которому ifPort присоединен
        [SerializeField] public string IfPortSourceId;
        //Нужен ли этому порту кнопка удаления
        [SerializeField] public bool Cross;
        //Нужен для кнопки добавления if поля
        [SerializeField] public bool PlusIf;
        //Нужно ли поле для этого порта
        [SerializeField] public bool IsField;
        //Является ли этот порт выходной функцией или содержит только лишь значение
        [SerializeField] public bool IsFunction;
        //Тип порта
        [SerializeField] public Type Type;
        //Список типов для присоединения порта
        [SerializeField] public string[] AvailableTypes;
        //С какой стороны расположен порт
        [SerializeField] public PortSide PortSide;
        //Возможность назначать Anchor для порта
        [SerializeField] public bool IsAnchorable;
        //Имя якоря
        [SerializeField] public string Anchor;
        [SerializeField] public UnityEngine.Object AssetSource;

        internal DSPortModel(Type[] availableTypes, PortSide PortSide)
        {
            PortID = Guid.NewGuid().ToString();
            this.PortSide = PortSide;
            if (PortSide == PortSide.Input) IsInput = true;
            else IsInput = false;
            this.AvailableTypes = availableTypes.Select(el => el.ToString()).ToArray();
        }

        internal void AddPort(BaseNode node, BasePort port)
        {
            if (NodeIDs == null) NodeIDs = new();

            var findNodeModel = NodeIDs.Where(el => el.NodeID == node.Model.ID).FirstOrDefault();
            if (findNodeModel == null)
            {
                NodeIDs.Add(new NodePortModel()
                {
                    NodeID = node.Model.ID,
                    PortIDs = new() { port.ID }
                });
            }
            else
            {
                var findPort = findNodeModel.PortIDs.Where(el => el == port.ID).FirstOrDefault();
                if (findPort == null) 
                { 
                    findNodeModel.PortIDs.Add(port.ID); 
                }
            }
        }
        internal void RemovePort(BaseNode node, BasePort port)
        {
            if (NodeIDs == null) NodeIDs = new();

            var findNodeModel = NodeIDs.Where(el => el.NodeID == node.Model.ID).FirstOrDefault();
            if (findNodeModel == null)
            {
                return;
            }
            else
            {
                var findPort = findNodeModel.PortIDs.Where(el => el == port.ID).FirstOrDefault();
                if (findPort != null)
                    findNodeModel.PortIDs.Remove(port.ID);

                if (findNodeModel.PortIDs.Count == 0)
                    NodeIDs.Remove(findNodeModel);
            }
        }
    }
}
