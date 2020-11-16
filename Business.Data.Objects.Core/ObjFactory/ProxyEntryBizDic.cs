using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Business.Data.Objects.Core.ObjFactory
{
    /// <summary>
    /// Identifica un dizionario di ProxyEntryBiz
    /// </summary>
    internal class ProxyEntryBizDic : Dictionary<long, ProxyEntryBiz>
    {
        public ProxyEntryBizDic(int capacity)
            : base(capacity)
        {
        }

    }
}
