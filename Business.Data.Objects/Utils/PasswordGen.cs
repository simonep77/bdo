using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Utils
{
    /// <summary>
    /// Classe per la generazione di password
    /// </summary>
    public static class PasswordGen
    {
        private const string _CharsLower = @"abcdefghjkmnpqrstuvwxyz";
        private const string _CharsUpper = @"ABCDEFGHJKMNPQRSTUVWXYZ";
        private const string _CharsNumber = @"23456789";
        private const string _CharsSpecial = @".:;-_$=?!@#";
        private const string _CharsAny = @"abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789.:;-_$=?!@#";

        /// <summary>
        /// Genera password fornendo una maschera di generazione.
        /// Ammessi: [A]ny, [L]ower, [U]pper, [N]umber, [S]pecial
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string Generate(string mask)
        {
            //Verifica maschera
            if (string.IsNullOrEmpty(mask))
            {
                throw new ArgumentException("Fornire un valore per la maschera di generazione password");
            }

            //Genera random
            Random oRnd = new Random(DateTime.Now.Millisecond);
            
            //Loop generazione password
            StringBuilder sbPass = new StringBuilder(mask.Length);
            int iIndex;

            for (int i = 0; i < mask.Length; i++)
            {
                switch (mask[i])
                {
                    case 'A':
                        iIndex = oRnd.Next(_CharsAny.Length-1);
                        sbPass.Append(_CharsAny[iIndex]);
                        break;
                    case 'L':
                        iIndex = oRnd.Next(_CharsLower.Length - 1);
                        sbPass.Append(_CharsLower[iIndex]);
                        break;
                    case 'U':
                        iIndex = oRnd.Next(_CharsUpper.Length - 1);
                        sbPass.Append(_CharsUpper[iIndex]);
                        break;
                    case 'N':
                        iIndex = oRnd.Next(_CharsNumber.Length - 1);
                        sbPass.Append(_CharsNumber[iIndex]);
                        break;
                    case 'S':
                        iIndex = oRnd.Next(_CharsSpecial.Length - 1);
                        sbPass.Append(_CharsSpecial[iIndex]);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Il carattere {0} non e' valido. Ammessi: [A]ny, [L]ower, [U]pper, [N]umber, [S]pecial", mask[i]));
                }
            }


            return sbPass.ToString();
        }

    }
}
