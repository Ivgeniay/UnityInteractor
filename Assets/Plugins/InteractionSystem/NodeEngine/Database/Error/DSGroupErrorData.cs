using NodeEngine.Groups;
using System.Collections.Generic;

namespace NodeEngine.Database.Error
{
    internal class DSGroupErrorData
    {
        internal DSErrorData ErrorData { get; set; }
        internal List<BaseGroup> Groups { get; set; }

        public DSGroupErrorData()
        {
            ErrorData = new DSErrorData();
            Groups = new List<BaseGroup>();
        }
    }
}
