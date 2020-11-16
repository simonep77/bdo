using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Schema.Definition
{

    /// <summary>
    /// Flag di gestione dati BDO
    /// </summary>
    [Flags]
    internal enum DataFlags : byte
    {
        None = 0,
        Changed = 1 << 1,
        Loaded = 1 << 2,
        ObjLoaded = 1 << 3
    }

}
