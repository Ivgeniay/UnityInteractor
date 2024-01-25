using NodeEngine.Nodes;
using System.Collections.Generic;

namespace NodeEngine.Database.Error
{
    internal class DSNodeErrorData
    {
        internal DSErrorData ErrorData { get; set; }
        internal List<BaseNode> Nodes { get; set; }

        internal DSNodeErrorData()
        {
            ErrorData = new DSErrorData();
            Nodes = new List<BaseNode>();
        }
    }
}
