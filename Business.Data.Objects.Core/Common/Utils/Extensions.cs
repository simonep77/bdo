using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Business.Data.Objects.Core;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Extension method di utility per vari tipi
    /// </summary>
    public static class Extensions
    {


        /// <summary>
        /// Operatore IN applicabile ad array parametrico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] args)
           where T : IComparable<T>
        {
            return obj.In(args as IEnumerable<T>);
        }

        /// <summary>
        /// Operatore IN applicabile ad enumerabili
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, IEnumerable<T> args)
            where T : IComparable<T>
        {
            if (args == null || !args.Any())
                return false;

            return args.Contains(obj);
        }

        /// <summary>
        /// Operatore in per oggetti non tipizzati. Viene eseguito internamente la conversione ove necessario
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool In(this IComparable obj, params IComparable[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            var tipo = obj.GetType();

            foreach (var item in args)
            {
                if (obj.Equals(item.GetType().Equals(tipo) ? item : Convert.ChangeType(item, tipo)))
                    return true;
            }

            return false;
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
        /// Verifica se un valore è ricompreso in un intervallo, estremi inclusi
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="argLow"></param>
        /// <param name="argHigh"></param>
        /// <returns></returns>
        public static bool Between(this IComparable obj, IComparable argLow, IComparable argHigh)
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
        /// Estensione fake per consentire query con NULL tramite linq su proprieta' di tipo valore.
        /// Non usare nel codice poichè ritorna sempre FALSE
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this IComparable obj)
        {
            return false;
        }

        /// <summary>
        /// Estensione fake per consentire query con NULL tramite linq su proprieta' di tipo valore
        /// Non usare nel codice poichè ritorna sempre TRUE
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNull(this IComparable obj)
        {
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

        /// <summary>
        /// Metodo che non esegue nulla ma utile nelle linq sql bdo per indicare l'inserimento di codice sql raw
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool RawBdoLinqSql(this string input)
        {
            return false;
        }

        /// <summary>
        /// Trasforma qualsiasi enumerabile di dataobject in BusinessObject List. Eventualmente è possibile eseguire del codice sul business object in creazione
        /// </summary>
        /// <typeparam name="TB"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static List<TB> ToBizObjectList<TB, T>(this IEnumerable<T> input, Action<TB> act = null)
            where T : DataObject<T>
            where TB : BusinessObject<T>
        {
            return new List<TB>(input.Select(x =>
            {
                var bo = x.ToBizObject<TB>();
                act?.Invoke(bo);
                return bo;
            }));
        }



    }
}
