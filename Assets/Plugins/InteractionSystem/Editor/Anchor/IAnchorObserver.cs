using NodeEngine.Ports;
using System;

namespace NodeEngine.Anchor
{
    internal interface IAnchorObserver
    {
        public void OnAnchorUpdate(BasePort port, string newRegex);
        public void OnAnchorDelete(BasePort port);
    }
}
