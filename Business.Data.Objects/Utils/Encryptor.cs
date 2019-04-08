using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Bdo.Utils
{
    /// <summary>
    /// Classe per la gestione delle operazioni di crypt/decrypt
    /// </summary>
    public class Encryptor
    {
        private static byte[] KEY_DEFAULT_ALGO = new byte[] {112, 21, 31, 6, 54, 98, 1, 48, 99, 82, 200, 142, 69, 59, 126, 74};

        /// <summary>
        /// Definisce gli algoritmi per il Crypt/Decrypt
        /// </summary>
        public enum Algorithm
        { 
            /// <summary>
            /// Algoritmo DES
            /// </summary>
            DES = 1,

            /// <summary>
            /// Algoritmo RC2
            /// </summary>
            RC2 = 2,

            /// <summary>
            /// Algoritmo Rijandel
            /// </summary>
            Rijndael = 3,

            /// <summary>
            /// Algoritmo TripleDES
            /// </summary>
            TripleDES = 4
        }


        #region METODI PRIVATI

        /// <summary>
        /// Crea algoritmo a partire dalla definizione
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        private static SymmetricAlgorithm getAlgorithm(Encryptor.Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.DES:
                    return DESCryptoServiceProvider.Create();
                case Algorithm.RC2:
                    return RC2CryptoServiceProvider.Create();
                case Algorithm.Rijndael:
                    return RijndaelManaged.Create();
                case Algorithm.TripleDES:
                    return TripleDESCryptoServiceProvider.Create();
                default:
                    break;
            }
            //Default
            return null;
        }

        /// <summary>
        /// Dato un testo ritorna il corrispondente array di byte
        /// </summary>
        /// <param name="text">
        /// Testo da esplodere
        /// </param>
        /// <returns></returns>
        private static byte[] generateByteString(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        private static byte[] Transform(byte[] input, ICryptoTransform CryptoTransform)
        {
            byte[] result;
            using(MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Write))
                {
                    cryptStream.Write(input, 0, input.Length);
                    cryptStream.FlushFinalBlock();

                    memStream.Position = 0;
                    result = new byte[Convert.ToInt32(memStream.Length)];
                    memStream.Read(result, 0, Convert.ToInt32(result.Length));
                }
            }

            return result;
        }


        /// <summary>
        /// Controlla correttezza formale chiave
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="key"></param>
        private static void checkAlgoKey(SymmetricAlgorithm algorithm, byte[] key)
        {
            if (!algorithm.ValidKeySize(key.Length * 8))
            {
                throw new ArgumentException(string.Format("La lunghezza della chiave fornita ({0} Bytes / {1} bits) non risulta compatibile con l'algoritmo selezionato {2}. Lunghezze supportate: Min-> {3} bits, Max-> {4} bits", key.Length, key.Length * 8, algorithm.GetType().Name, algorithm.LegalKeySizes[0].MinSize, algorithm.LegalKeySizes[0].MaxSize));
            }
        }


        #endregion


        #region METODI PUBBLICI


        /// <summary>
        /// Cripta un buffer utilizzando una chiave, un IV ed un algoritmo forniti
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Encrypt(Encryptor.Algorithm algorithm, byte[] key, byte[] iv, byte[] input)
        {
            try
            {
                //Crea algoritmo
                SymmetricAlgorithm oAlgo = getAlgorithm(algorithm);

                //Esegue validazione Key
                checkAlgoKey(oAlgo, key);

                //Crypt
                return Transform(input, oAlgo.CreateEncryptor(key, iv));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Concat("Si e' verificato un errore durante l'operazione di crypt: ", ex.Message));
            }
        }

        /// <summary>
        /// Decripta un buffer utilizzando una chiave, un IV ed un algoritmo forniti
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Decrypt(Encryptor.Algorithm algorithm, byte[] key, byte[] iv, byte[] input)
        {
            try
            {
                //Crea algoritmo
                SymmetricAlgorithm oAlgo = getAlgorithm(algorithm);
                
                //Esegue validazione Key
                checkAlgoKey(oAlgo, key);

                //Decrypt
                return Transform(input, oAlgo.CreateDecryptor(key, iv));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Concat("Si e' verificato un errore durante l'operazione di decrypt: ", ex.Message));
                
            }
        }


        /// <summary>
        /// Cripta un buffer utilizzando una chiave ed un algoritmo forniti
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Encrypt(Encryptor.Algorithm algorithm, string key, byte[] input)
        {
            byte[] buffkey = generateByteString(key);
            byte[] buff_iv = buffkey;

            return Encrypt(algorithm, buffkey, buff_iv, input);
        }


        /// <summary>
        /// Decripta un buffer utilizzando una chiave ed un algoritmo forniti
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Decrypt(Encryptor.Algorithm algorithm, string key, byte[] input)
        {
            byte[] buffkey = generateByteString(key);
            byte[] buff_iv = buffkey;

            return Decrypt(algorithm, buffkey, buff_iv, input);
        }

        /// <summary>
        /// Cripta un testo in base 64 utilizzando una chiave ed un algoritmo forniti
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptToBase64String(Encryptor.Algorithm algorithm, string key, string input)
        {
            byte[] buffkey = generateByteString(key);
            byte[] buff_iv = buffkey;
            byte[] buff_input = generateByteString(input);

            return Convert.ToBase64String(Encrypt(algorithm, buffkey, buff_iv, buff_input));
        }

        /// <summary>
        /// Decripta un testo in base 64 utilizzando una chiave ed un algoritmo forniti
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="key"></param>
        /// <param name="inputb64"></param>
        /// <returns></returns>
        public static string DecryptFromBase64String(Encryptor.Algorithm algorithm, string key, string inputb64)
        {
            byte[] buffkey = generateByteString(key);
            byte[] buff_iv = buffkey;
            byte[] buff_input = Convert.FromBase64String(inputb64);

            return Encoding.UTF8.GetString(Decrypt(algorithm, buffkey, buff_iv, buff_input));
        }


        /// <summary>
        /// Decripta un testo in base 64 utilizzando una chiave fornita ed algoritmo di default
        /// </summary>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptToBase64String(string key, string input)
        {
            byte[] buffkey = generateByteString(key);
            byte[] buff_iv = buffkey;
            byte[] buff_input = generateByteString(input);

            return Convert.ToBase64String(Encrypt(Algorithm.TripleDES, buffkey, buff_iv, buff_input));
        }

        /// <summary>
        /// cripta un testo in base 64 utilizzando una chiave fornita ed algoritmo di default
        /// </summary>
        /// <param name="key"></param>
        /// <param name="inputb64"></param>
        /// <returns></returns>
        public static string DecryptFromBase64String(string key, string inputb64)
        {
            byte[] buffkey = generateByteString(key);
            byte[] buff_iv = buffkey;
            byte[] buff_input = Convert.FromBase64String(inputb64);

            return Encoding.UTF8.GetString(Decrypt(Algorithm.TripleDES, buffkey, buff_iv, buff_input));
        }


        /// <summary>
        /// Cripta un testo in base 64 utilizzando chiave ed algoritmo di default
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptToBase64String(string input)
        {
            byte[] buff_input = generateByteString(input);

            return Convert.ToBase64String(Encrypt(Algorithm.TripleDES, KEY_DEFAULT_ALGO, KEY_DEFAULT_ALGO, buff_input));
        }

        /// <summary>
        /// Decripta un testo in base 64 utilizzando chiave ed algoritmo di default
        /// </summary>
        /// <param name="inputb64"></param>
        /// <returns></returns>
        public static string DecryptFromBase64String(string inputb64)
        {
            byte[] buff_input = Convert.FromBase64String(inputb64);

            return Encoding.UTF8.GetString(Decrypt(Algorithm.TripleDES, KEY_DEFAULT_ALGO, KEY_DEFAULT_ALGO, buff_input));
        }

        #endregion



    }

}
