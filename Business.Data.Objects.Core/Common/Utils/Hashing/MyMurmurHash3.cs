using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Utils.Hashing.External;

namespace Bdo.Utils.Hashing
{
    class MyMurmurHash3 : IHashAlgo
    {
        uint IHashAlgo.Hash(byte[] data)
        {
            return MurMurHash3.Hash(data);
        }

        uint IHashAlgo.Hash(string data)
        {
            return MurMurHash3.Hash( Encoding.UTF8.GetBytes(data) );
        }

        ulong IHashAlgo.Hash64(byte[] data)
        {
            throw new NotImplementedException();
        }

        ulong IHashAlgo.Hash64(string data)
        {
            throw new NotImplementedException();
        }
    }
}
