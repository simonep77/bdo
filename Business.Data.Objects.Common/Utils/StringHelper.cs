using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Classe con metodi comuni relativi alle stringhe
    /// </summary>
    public class StringHelper
    {

        #region METODI PUBBLICI

        /// <summary>
        /// Rimuove eventuali accenti e sostituisce con apici
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveAccents(string value)
        {
            StringBuilder sOut = new StringBuilder();
            int valLen = value.Length;
            bool appendApos = false;

            for (int i = 0; i < valLen; i++)
            {
                switch (value[i])
                {
                    case 'à':
                        sOut.Append('a');
                        appendApos = true;
                        break;
                    case 'é':
                        sOut.Append('e');
                        appendApos = true;
                        break;
                    case 'è':
                        sOut.Append('e');
                        appendApos = true;
                        break;
                    case 'ì':
                        sOut.Append('i');
                        appendApos = true;
                        break;
                    case 'ò':
                        sOut.Append('o');
                        appendApos = true;
                        break;
                    case 'ù':
                        sOut.Append('u');
                        appendApos = true;
                        break;
                    case 'À':
                        sOut.Append('A');
                        appendApos = true;
                        break;
                    case 'È':
                        sOut.Append('E');
                        appendApos = true;
                        break;
                    case 'É':
                        sOut.Append('E');
                        appendApos = true;
                        break;
                    case 'Ì':
                        sOut.Append('I');
                        appendApos = true;
                        break;
                    case 'Ò':
                        sOut.Append('O');
                        appendApos = true;
                        break;
                    case 'Ù':
                        sOut.Append('U');
                        appendApos = true;
                        break;
                    default:
                        //Se è necessario apostrofo lo si scrive solo se il carattere corrente è WhiteSpace
                        if (appendApos && char.IsWhiteSpace(value[i]))
                        {
                            sOut.Append('\'');
                        }
                        //Aggiunge carattere a coda output
                        sOut.Append(value[i]);
                        //Resetta scrittura apostrofo
                        appendApos = false;
                        break;
                }
            }

            //Se necessario apostrofo lo scrive (ultimo)
            if (appendApos)
            {
                sOut.Append('\'');
            }

            //Ritorna
            return sOut.ToString();
        }


        /// <summary>
        /// Routine di rimozione e riconduzione caratteri strani
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(text.Length);

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        /// <summary>
        /// Tronca una stringa alla dimensione fornita se piu' lunga
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxChars"></param>
        /// <returns></returns>
        public static string TruncateMaxChar(string value, int maxChars)
        {
            if (maxChars <= 0)
            {
                return value;
            }

            return (value.Length <= maxChars) ? value : value.Substring(0, maxChars);
        }

        /// <summary>
        /// Suddivide la stringa originale in N sottostringhe di dimensioni uguali a size (tranne l'ultima)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string[] SplitSize(string value, int size)
        {
            if (size <= 0)
            {
                return new string[1]{value};
            }

            int iNumChunk = Convert.ToInt32(Math.Ceiling(Convert.ToSingle(value.Length) / Convert.ToSingle(size)));
            int iPos = 0;
            string[] sArr = new string[iNumChunk];

            for (int i = 0; i < iNumChunk; i++)
            {
                sArr[i] = value.Substring(iPos, size);
                iPos += size;
            }

            return sArr;
        }


        /// <summary>
        /// Data una string ritorna la stessa inversa
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Reverse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var charArray = value.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        #endregion
    }
}
