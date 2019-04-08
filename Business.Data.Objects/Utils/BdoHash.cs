using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Utils
{
    /// <summary>
    /// Hashing globale BDO
    /// </summary>
    public class BdoHash
    {
        public readonly static Bdo.Utils.Hashing.IHashAlgo Instance = new Bdo.Utils.Hashing.MyMurmurHash3();
    }
}
