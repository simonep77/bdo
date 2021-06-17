using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Extension method di utility per vari tipi
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Operatore IN
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] args)
           where T : IComparable<T>
        {
            if (args == null || args.Length == 0)
                return false;

            return args.Contains(obj);
        }

        /// <summary>
        /// Operatore BEETWEEN
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="argLow"></param>
        /// <param name="argHigh"></param>
        /// <returns></returns>
        public static bool Between<T>(this T obj, T argLow, T argHigh)
            where T : IComparable<T>
        {
            if (argLow == null || argHigh == null)
                return false;

            if (obj.CompareTo(argLow) < 0)
                return false;

            if (obj.CompareTo(argHigh) > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Esegue un confronto con espressione regolare. Se usato in linq SQL valgono solo i wildcard del db di destinazione
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool Like(this string obj, string pattern)
        {
            return Regex.IsMatch(obj, pattern);
        }


    }
}
