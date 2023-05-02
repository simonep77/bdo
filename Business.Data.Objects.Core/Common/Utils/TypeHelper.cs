﻿using Business.Data.Objects.Core.Base;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Contiene metodi statici di supporto alla gestione dei tipi 
    /// </summary>
    public static class TypeHelper
    {

        #region TYPE UTILITY

        /// <summary>
        /// Indica se tipo è un oggetto Dal
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBdoDalType(this Type type)
        {
            return type.IsSubclassOf(typeof(DataObjectBase));
        }

        /// <summary>
        /// Verifica se trattasi di tipo numerico
        /// </summary>
        /// <returns></returns>
        public static bool IsNumericType(Type type)
        {
            return type.IsIntegerType() || type.IsDecimalType();
        }

        /// <summary>
        /// Indica se tipo stringa
        /// </summary>
        /// <returns></returns>
        public static bool IsString(this Type type)
        {
            return (Type.GetTypeCode(type) == TypeCode.String);
        }

        /// <summary>
        /// Indica se il tipo e' una data
        /// </summary>
        /// <returns></returns>
        public static bool IsDate(this Type type)
        {
            return (Type.GetTypeCode(type) == TypeCode.DateTime);
        }

        /// <summary>
        /// Indica se un tipo e' boolean
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBool(this Type type)
        {
            return (Type.GetTypeCode(type) == TypeCode.Boolean);
        }


        /// <summary>
        /// Indica se tipo decimale (decimal, float, double)
        /// </summary>
        /// <returns></returns>
        public static bool IsDecimalType(this Type type)
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
        /// Indica se tipo è guid
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsGuid(this Type type)
        {
            return type.Equals(typeof(Guid));
        }

        /// <summary>
        /// Indica se tipo intero (byte, int16, int32, int64, ...)
        /// </summary>
        /// <returns></returns>
        public static bool IsIntegerType(this Type type)
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
        public static int IntegerSize(this Type type)
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
        public static bool IsByteArray(this Type type)
        {
            return (type.IsArray && type.Equals(typeof(byte[])));
        }


        #endregion

    }
}
