using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Bdo.ObjFactory
{
    /// <summary>
    /// Identifica un dizionario di TypeEntry
    /// </summary>
    internal class ProxyEntryDaoDic : Dictionary<long, ProxyEntryDao>
    {
        public ProxyEntryDaoDic(int capacity): base(capacity)
        {
        }

    }
}
