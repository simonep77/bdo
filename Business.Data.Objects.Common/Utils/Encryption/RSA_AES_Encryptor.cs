using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Bdo.Utils.Encryption
{
    /// <summary>
    /// Classe per la codifica/decodifica di dati tramite RSA (chiavi) e AES (per i dati)
    /// </summary>
    public class RSA_AES_Encryptor
    {
        /// <summary>
        /// Crea istanza Rijandel (AES) con parametri base AES 256
        /// </summary>
        /// <returns></returns>
        private static RijndaelManaged createRijandelAES256()
        {
            var AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Mode = CipherMode.CBC;

            return AES;
        }

        /// <summary>
        /// Crea istanza Rijandel (AES) con parametri base AES 256 da chiave
        /// </summary>
        /// <returns></returns>
        private static RijndaelManaged createRijandelAES256(string key)
        {
            var AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Mode = CipherMode.CBC;

            var keybytes = AES.KeySize / 8;
            var ivbytes = AES.BlockSize / 8;
            var bufkey = Encoding.ASCII.GetBytes(key.PadRight(keybytes, '0').Substring(0, keybytes));
            var bufiv = new byte[ivbytes];
            Array.Copy(bufkey, bufiv, ivbytes);
            Array.Reverse(bufiv);

            AES.Key = bufkey;
            AES.IV = bufiv;

            return AES;
        }


        #region RSA

        /// <summary>
        /// Cripta tramite chiave RSA un buffer su altro stream
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        public static void RSA_AES_Encrypt(RSA key, byte[] data, Stream output)
        {
            //Genera un IV e chiave di criptazione random
            var AES = createRijandelAES256();
            try
            {
                AES.GenerateIV();
                AES.GenerateKey();

                var fmt = new RSAPKCS1KeyExchangeFormatter(key);
                var aesKeyEnc = fmt.CreateKeyExchange(AES.Key);

                //Scrive dimensione IV e IV in chiaro
                var ivBufLen = BitConverter.GetBytes(AES.IV.Length);
                output.Write(ivBufLen, 0, ivBufLen.Length);
                output.Write(AES.IV, 0, AES.IV.Length);

                //Scrive dimensione KEY e KEY criptata RSA in chiaro
                var keyBufLen = BitConverter.GetBytes(aesKeyEnc.Length);
                output.Write(keyBufLen, 0, keyBufLen.Length);
                output.Write(aesKeyEnc, 0, aesKeyEnc.Length);

                //Scrive resto del file
                using (var cs = new CryptoStream(output, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }
            }
            finally
            {
                AES.Clear();
            }

        }

        /// <summary>
        /// Decripta tramite chiave RSA uno stream su altro stream
        /// </summary>
        /// <param name="key"></param>
        /// <param name="encInput"></param>
        /// <param name="output"></param>
        public static void RSA_AES_Decrypt(RSA key, Stream encInput, Stream output)
        {
            //Genera un IV e chiave di criptazione random
            var AES = createRijandelAES256();
            try
            {
                var sizeBuffer = new byte[sizeof(int)];
                int sizeLen;

                //Legge IV (size e data) non criptato
                encInput.Read(sizeBuffer, 0, sizeBuffer.Length);
                sizeLen = BitConverter.ToInt32(sizeBuffer, 0);
                var aesIvData = new byte[sizeLen];
                encInput.Read(aesIvData, 0, sizeLen);
                //Imposta Iv su AES
                AES.IV = aesIvData;

                //Legge keylen e AES criptata
                encInput.Read(sizeBuffer, 0, sizeBuffer.Length);
                sizeLen = BitConverter.ToInt32(sizeBuffer, 0);
                var aesKeyData = new byte[sizeLen];
                encInput.Read(aesKeyData, 0, aesKeyData.Length);

                //Decripta AES Key ed imposta su AES
                var def = new RSAPKCS1KeyExchangeDeformatter(key);
                AES.Key = def.DecryptKeyExchange(aesKeyData);
                
                //Legge resto del file criptato
                using (var cs = new CryptoStream(encInput, AES.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    var buffer = new byte[sizeof(Int16)];
                    int iRead;

                    while ((iRead = cs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, iRead);
                    }
                }

            }
            finally
            {
                AES.Clear();
            }
  
        }

        #endregion


        #region RSA ONLY

        /// <summary>
        /// Cripta tramite chiave RSA un buffer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clearData"></param>
        /// <param name="output"></param>
        public static byte[] RSA_Encrypt(RSACryptoServiceProvider key, byte[] clearData)
        {
            return key.Encrypt(clearData, true);
        }

        /// <summary>
        /// Decripta tramite chiave RSA
        /// </summary>
        /// <param name="key"></param>
        /// <param name="encData"></param>
        /// <returns></returns>
        public static byte[] RSA_Decrypt(RSACryptoServiceProvider key, byte[] encData)
        {
            return key.Decrypt(encData, true);
        }

        /// <summary>
        /// Cripta buffer e ritorna un testo base 64
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clearData"></param>
        /// <returns></returns>
        public static string RSA_Encrypt_B64(RSACryptoServiceProvider key, byte[] clearData)
        {
            return Convert.ToBase64String(key.Encrypt(clearData, true));
        }

        /// <summary>
        /// Decripta un testo codificato ed in base 64 e ritorna un buffer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="encData64"></param>
        /// <returns></returns>
        public static byte[] RSA_Decrypt_B64(RSACryptoServiceProvider key, string encData64)
        {
            return key.Decrypt(Convert.FromBase64String(encData64), true);
        }

        /// <summary>
        /// Cripta un testo (fornendo encoding) e ritorna testo codificato ed in base 64
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clearData"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string RSA_Encrypt_B64_Text(RSACryptoServiceProvider key, string clearData, Encoding enc)
        {
            return Convert.ToBase64String(key.Encrypt(enc.GetBytes(clearData), true));
        }


        /// <summary>
        /// Decripta un testo codificato ed in base 64 e ritorna un testo nell'encoding specificato
        /// </summary>
        /// <param name="key"></param>
        /// <param name="encData64"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string RSA_Decrypt_B64_Text(RSACryptoServiceProvider key, string encData64, Encoding enc)
        {
            return enc.GetString(key.Decrypt(Convert.FromBase64String(encData64), true));
        }


        #endregion


        #region AES ONLY

        /// <summary>
        /// Cripta AES
        /// </summary>
        /// <param name="clearData"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] AES_Encrypt(byte[] clearData, string key)
        {
            var AES = createRijandelAES256(key);
            try
            {
                using (var enc = AES.CreateEncryptor())
                {
                    // Prepend cryptotext with IV
                    byte[] output = enc.TransformFinalBlock(clearData, 0, clearData.Length);

                    return output;
                }
            }
            finally
            {
                AES.Clear();
            }

        }

        /// <summary>
        /// Decripta tramite chiave RSA
        /// </summary>
        /// <param name="key"></param>
        /// <param name="encData"></param>
        /// <returns></returns>
        public static byte[] AES_Decrypt(byte[] encData, string key)
        {
            var AES = createRijandelAES256(key);
            try
            {
                using (var enc = AES.CreateDecryptor())
                {
                    // Prepend cryptotext with IV
                    byte[] output = enc.TransformFinalBlock(encData, 0, encData.Length);

                    return output;
                }
            }
            finally
            {
                AES.Clear();
            }

        }

        /// <summary>
        /// Cripta un testo (fornendo encoding) e ritorna testo codificato ed in base 64
        /// </summary>
        /// <param name="key"></param>
        /// <param name="clearData"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string AES_Encrypt_B64_Text(string clearData, string key, Encoding enc)
        {
            return Convert.ToBase64String(AES_Encrypt(enc.GetBytes(clearData), key));
        }


        /// <summary>
        /// Decripta un testo codificato ed in base 64 e ritorna un testo nell'encoding specificato
        /// </summary>
        /// <param name="key"></param>
        /// <param name="encData64"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string AES_Decrypt_B64_Text(string encData64, string key, Encoding enc)
        {
            return enc.GetString(AES_Decrypt(Convert.FromBase64String(encData64), key));
        }

        #endregion
    }
}
