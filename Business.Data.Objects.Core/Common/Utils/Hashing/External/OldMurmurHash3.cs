﻿// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

using System;

namespace Bdo.Utils.Hashing.External
{
    public class OldMurmurHash3
    {
        private const uint UINT_SEED = 63;


        private static uint Finalize(uint hash)
        {
            hash ^= hash >> 16;
            hash *= 0x85ebca6b;
            hash ^= hash >> 13;
            hash *= 0xc2b2ae35;
            hash ^= hash >> 16;

            return hash;
        }

        /// <summary>
        /// Hash di una stringa
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static uint Hash(string key)
        {
            byte[] bKey = System.Text.Encoding.ASCII.GetBytes(key);
            return Hash(bKey, bKey.Length, UINT_SEED);
        }

        public static uint Hash(byte[] key)
        {
            return Hash(key, key.Length, UINT_SEED);
        }

        private static uint Hash(byte[] key, int length, uint seed)
        {
            uint hash = seed;
            uint coefficient1 = 0xcc9e2d51;
            uint coefficient2 = 0x1b873593;
            uint coefficient3 = 5 + 0xe6546b64;

            int index = 0;
            while (length - index >= 4)
            {
                uint segment = BitConverter.ToUInt32(key, index);
                segment *= coefficient1;
                segment = ((segment << 15) | (segment >> 17)); //Rotate(segment, 15); 
                segment *= coefficient2;

                hash ^= segment;
                hash = ((hash << 13) | (hash >> 19)); //Rotate(hash, 13);
                hash = hash * coefficient3;

                index += 4;
            }

            uint tailSegment = 0;
            int rem = length % 4;
            if (rem > 0)
            {
                switch (rem)
                {
                    case 3:
                        tailSegment ^= (uint)key[index + 2] << 16;
                        tailSegment ^= (uint)key[index + 1] << 8;
                        tailSegment ^= (uint)key[index];
                        break;
                    case 2:
                        tailSegment ^= (uint)key[index + 1] << 8;
                        tailSegment ^= (uint)key[index];
                        break;
                    case 1:
                        tailSegment ^= (uint)key[index];
                        break;
                }

                tailSegment *= coefficient1;
                tailSegment = ((tailSegment << 15) | (tailSegment >> 17));//Rotate(tailSegment, 15);
                tailSegment *= coefficient2;
                hash ^= tailSegment;
            }

            hash ^= (uint)length;
            hash = Finalize(hash);
            return hash;
        }

    }
}