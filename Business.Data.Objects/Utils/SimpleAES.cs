using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Bdo.Utils
{
    /// <summary>
    /// Simple encryption/decryption using a random initialization vector
    /// and prepending it to the crypto text.
    /// </summary>
    /// <remarks>Based on multiple answers in http://stackoverflow.com/questions/165808/simple-two-way-encryption-for-c-sharp </remarks>
    public class SimpleAes : IDisposable
    {
        /// <summary>
        ///     Initialization vector length in bytes.
        /// </summary>
        private const int IvBytes = 16;


        private readonly UTF8Encoding _encoder;
        private readonly RijndaelManaged _rijndael;

        public SimpleAes(string key)
        {
            //Key: 16, 24, 32 chars
            _rijndael = new RijndaelManaged { Key = Encoding.ASCII.GetBytes(key.PadRight(32, '0').Substring(0, 32)) };
            _encoder = new UTF8Encoding();
        }

        /// <summary>
        /// Pulizia
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)_rijndael).Dispose();
        }

        /// <summary>
        /// Decripta stringa in notazione Base64
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public string Decrypt(string encrypted)
        {
            return _encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
        }


        /// <summary>
        /// Cripta stringa e ritorna notazione Base64
        /// </summary>
        /// <param name="unencrypted"></param>
        /// <returns></returns>
        public string Encrypt(string unencrypted)
        {
            return Convert.ToBase64String(Encrypt(_encoder.GetBytes(unencrypted)));
        }

        public byte[] Decrypt(byte[] buffer)
        {
            // IV is prepended to cryptotext

            var iv = new byte[IvBytes];
            buffer.CopyTo(iv, 0);

            using (ICryptoTransform decryptor = _rijndael.CreateDecryptor(_rijndael.Key, iv))
            {
                return decryptor.TransformFinalBlock(buffer, IvBytes, buffer.Length - IvBytes);
            }
        }

        public byte[] Encrypt(byte[] buffer)
        {
            //Rigenera IV
            _rijndael.GenerateIV();
            using (var enc = _rijndael.CreateEncryptor())
            {
                // Prepend cryptotext with IV
                byte[] inputBuffer = enc.TransformFinalBlock(buffer, 0, buffer.Length);

                //Crea un buffer in uscita con IV e dati
                byte[] output = new byte[inputBuffer.Length + _rijndael.IV.Length];
                _rijndael.IV.CopyTo(output, 0);
                inputBuffer.CopyTo(output, IvBytes);

                return output;
            }


        }
    }
}
