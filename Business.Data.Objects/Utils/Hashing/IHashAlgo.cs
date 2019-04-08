using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Utils.Hashing
{
    public interface IHashAlgo
    {

        /// <summary>
        /// Hash di dati arbitrari
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        uint Hash(string data);

        /// <summary>
        /// Hash di una stringa
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        uint Hash(byte[] data);


        /// <summary>
        /// Hash 64 di dati arbitrari
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ulong Hash64(string data);

        /// <summary>
        /// Hash 64 di una stringa
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ulong Hash64(byte[] data);


    }
}
