using Business.Data.Objects.Common;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.Schema.Definition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Utils
{
    /// <summary>
    /// Classe appoggio con metodi utili agli oggetti
    /// </summary>
    internal static class ObjectHelper
    {
        /// <summary>
        /// Ritorna stringa con stringa operatore
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        internal static string OperatorToString(EOperator op)
        {
            switch (op)
            {
                case EOperator.Equal:
                    return @"=";
                case EOperator.Differs:
                    return @"<>";
                case EOperator.GreaterThan:
                    return @">";
                case EOperator.GreaterEquals:
                    return @">=";
                case EOperator.LessThan:
                    return @"<";
                case EOperator.LessEquals:
                    return @"<=";
                case EOperator.Like:
                    return @" LIKE ";
                case EOperator.NotLike:
                    return @" NOT LIKE ";
                case EOperator.IsNull:
                    return @" IS NULL ";
                case EOperator.IsNotNull:
                    return @" IS NOT NULL ";
                case EOperator.In:
                    return @" IN ";
                case EOperator.Between:
                    return @" BETWEEN ";
                default:
                    throw new ArgumentException("Operatore sconosciuto");
            }
        }


        /// <summary>
        /// Dato un array ritorna una rappresentazione in stringa tipo v1, v2
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static string ObjectEnumerableToString(IEnumerable values)
        {
            return JoinString(", ", values);
        }
        

        /// <summary>
        /// Versione che va in ricorsione in presenza di sotto array
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static string ObjectArrayToStringRecursive(Array values)
        {
            const string SEP = @"|";
            StringBuilder sb = new StringBuilder();

            int valLen = values.Length;

            //Loop array
            for (int i = 0; i < valLen; i++)
            {
                Array oArr = values.GetValue(i) as Array;
                if (oArr == null)
                {
                    sb.Append(values.GetValue(i));
                }
                else
                {
                    sb.Append("(");
                    sb.Append(JoinString(SEP, oArr));
                    sb.Append(")");
                }
                
                sb.Append(SEP);
            }

            //Rimuove ,
            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }


        /// <summary>
        /// Crea un codice hash in formato stringa a partire da oggetto DAL
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string GetObjectHashString(DataObjectBase obj)
        {
            if (obj == null)
                return string.Empty;

            return GetObjectHashString(obj.GetSlot(), 
                obj.mClassSchema, 
                obj.mClassSchema.PrimaryKey.GetValues(obj));
        }

        /// <summary>
        /// Crea un codice hash in formato stringa leggibile
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="schema"></param>
        /// <param name="pkvalues"></param>
        /// <returns></returns>
        internal static string GetObjectHashString(BusinessSlot slot, ClassSchema schema, object[] pkvalues)
        {
            const string SEP = @"|";
            var db = slot.DbGet(schema);

            return string.Concat((schema.IsDefaultDb ? Constants.STR_DB_DEFAULT : schema.DbConnDef.Name),
                SEP, db.HashCode.ToString(),
                SEP, schema.InternalID.ToString(),
                SEP, slot.DbPrefixGetTableName(schema.TableDef),
                SEP, JoinString(SEP, pkvalues));

            //Versione compressa con doppio hash inverso
            //var sHashBase = string.Concat((schema.IsDefaultDb ? Constants.STR_DB_DEFAULT : schema.DbConnDef.Name),
            //    SEP, db.HashCode.ToString(),
            //    SEP, slot.DbPrefixGetTableName(schema.TableDef),
            //    SEP, JoinString(SEP, pkvalues));

            //return string.Concat(sHashBase.GetHashCode().ToString(),SEP, StringHelper.Reverse(sHashBase).GetHashCode().ToString() );

        }

        /// <summary>
        /// Concatena array di oggetti con separatore specificato
        /// </summary>
        /// <param name="sep"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string JoinString(string sep, IEnumerable values)
        {
            if (values == null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var item in values)
            {
                sb.Append(item);
                sb.Append(sep);
            }

            //Se valorizzato qualcosa allora rimuoviamo l'ultimo separatore
            if (sb.Length > 0)
                sb.Remove(sb.Length - sep.Length, sep.Length);

            return sb.ToString();
        }


        /// <summary>
        /// Crea un array di object a partire da un qualunque tipo di array
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static object[] ToObjectArray(Array values)
        {
           
            //Se gia' array di oggetti lo ritorna
            if (values is object[])
                return (object[])values;

            //Se null ritorna array
            if (values == null)
                return new object[] { };

            object[] ret = new object[values.Length];

            Array.Copy(values, ret, values.Length);

            return ret;
        }


        /// <summary>
        /// Crea un array di object a partire da un qualunque tipo di enumerabile
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static object[] ToObjectArray(IEnumerable values)
        {

            //Se gia' array di oggetti lo ritorna
            if (values is object[])
                return (object[])values;

            //Se null ritorna array
            if (values == null)
                return new object[] { };

            //Copia su lista
            List<object> ret = new List<object>(5);

            foreach (var item in values)
            {
                ret.Add(item);
            }

            return ret.ToArray();
        }

    }
}
