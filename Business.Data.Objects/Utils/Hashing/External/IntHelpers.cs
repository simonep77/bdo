using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Utils.Hashing.External
{

    public static class IntHelpers
    {
        public static ulong RotateLeft(ulong original, int bits)
        {
            return (original << bits) | (original >> (64 - bits));
        }

        public static ulong RotateRight(ulong original, int bits)
        {
            return (original >> bits) | (original << (64 - bits));
        }

        public static ulong GetUInt64(byte[] bb, int pos)
        {
            return BitConverter.ToUInt64(bb, pos);
        }

    }

}
