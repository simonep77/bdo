using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Contiene metodi statici di supporto alla gestione dei tipi 
    /// </summary>
    public class TypeHelper
    {

        #region TYPE UTILITY


        /// <summary>
        /// Verifica se trattasi di tipo numerico
        /// </summary>
        /// <returns></returns>
        public static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indica se tipo stringa
        /// </summary>
        /// <returns></returns>
        public static bool IsString(Type type)
        {
            return (Type.GetTypeCode(type) == TypeCode.String);
        }

        /// <summary>
        /// Indica se il tipo e' una data
        /// </summary>
        /// <returns></returns>
        public static bool IsDate(Type type)
        {
            return (Type.GetTypeCode(type) == TypeCode.DateTime);
        }


        /// <summary>
        /// Indica se tipo decimale (decimal, float, double)
        /// </summary>
        /// <returns></returns>
        public static bool IsDecimalType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indica se tipo intero (byte, int16, int32, int64, ...)
        /// </summary>
        /// <returns></returns>
        public static bool IsIntegerType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Dato un tipo intero ritorna la dimensione
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int IntegerSize(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    return sizeof(byte);
                case TypeCode.SByte:
                    return sizeof(sbyte);
                case TypeCode.UInt16:
                    return sizeof(UInt16);
                case TypeCode.UInt32:
                    return sizeof(UInt32);
                case TypeCode.UInt64:
                    return sizeof(UInt64);
                case TypeCode.Int16:
                    return sizeof(Int16);
                case TypeCode.Int32:
                    return sizeof(Int32);
                case TypeCode.Int64:
                    return sizeof(Int64);
                default:
                    return -1;
            }
        }


        /// <summary>
        /// Indica se trattasi di array di byte
        /// </summary>
        /// <returns></returns>
        public static bool IsByteArray(Type type)
        {
            return (type.IsArray && type.Equals(typeof(byte[])));
        }

        #endregion

    }
}
