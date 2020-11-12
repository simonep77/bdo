using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Utils.Encryption
{
    /// <summary>
    /// Interfaccia di Crypt/Decrypt
    /// </summary>
    interface ICryptoAlgo
    {
        /// <summary>
        /// Ritorna dati criptati in formato stringa base64
        /// </summary>
        /// <param name="cleardata"></param>
        /// <returns></returns>
        string EncryptData(byte[] cleardata);

        /// <summary>
        /// Ritorna stringa criptata in formato stringa base64
        /// </summary>
        /// <param name="cleardata"></param>
        /// <returns></returns>
        string EncryptString(string cleardata);

        /// <summary>
        /// Ritorna dati decriptati da testo codificato ed in base64
        /// </summary>
        /// <param name="encdata"></param>
        /// <returns></returns>
        byte[] DecryptData(string encdata);

        /// <summary>
        /// Ritorna stringa decriptata da testo codificato ed in base64
        /// </summary>
        /// <param name="encdata"></param>
        /// <returns></returns>
        string DecryptString(string encdata);

    }
}
