using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Business.Data.Objects.Core.ObjFactory
{
    /// <summary>
    /// Identifica un dizionario di TypeEntry
    /// </summary>
    internal class ProxyEntryDaoDic : Dictionary<long, ProxyEntryDAO>
    {
        public ProxyEntryDaoDic(int capacity): base(capacity)
        {
        }

    }
}
