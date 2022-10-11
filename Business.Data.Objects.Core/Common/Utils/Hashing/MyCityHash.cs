using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Utils.Hashing.External;

namespace Bdo.Utils.Hashing
{
    class MyCityHash : IHashAlgo
    {
        uint IHashAlgo.Hash(byte[] data)
        {
            return CityHash.CityHash32(data);
        }

        uint IHashAlgo.Hash(string data)
        {
            return CityHash.CityHash32(data);
        }

        ulong IHashAlgo.Hash64(byte[] data)
        {
            return CityHash.CityHash64(data);
        }

        ulong IHashAlgo.Hash64(string data)
        {
            return CityHash.CityHash64(data);
        }
    }
}
