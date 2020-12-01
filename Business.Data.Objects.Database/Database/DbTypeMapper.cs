using System;
using System.Collections.Generic;
using System.Data;

namespace Business.Data.Objects.Database
{
    /// <summary>
    /// Casse per la mappatura dei tipi NET -> DB
    /// </summary>
    public class DbTypeMapper 
	{

        private static Dictionary<Type, DbType> _StaticMap;

        private Dictionary<Type, DbType> mInstanceMap;
        private bool mInstanceCustom;

        /// <summary>
        /// Espone la mappa di associazione statica e immutabile
        /// </summary>
        public static IReadOnlyDictionary<Type, DbType> StaticMap
        {
            get
            {
                return _StaticMap;
            }
        }

        /// <summary>
        /// Espone la mappa di associazione dell'istanza corrente. Se non vengono richieste modifiche coincide con la StaticMap
        /// </summary>
        public IReadOnlyDictionary<Type, DbType> CurrentMap { 
            get
            {
                return this.mInstanceMap;
            } 
        }

        static DbTypeMapper()
        {
            //carica una volta sola il mapper di default
            _StaticMap = new Dictionary<Type, DbType>(37)
            {
                [typeof(byte)] = DbType.Byte,
                [typeof(sbyte)] = DbType.SByte,
                [typeof(short)] = DbType.Int16,
                [typeof(ushort)] = DbType.UInt16,
                [typeof(int)] = DbType.Int32,
                [typeof(uint)] = DbType.UInt32,
                [typeof(long)] = DbType.Int64,
                [typeof(ulong)] = DbType.UInt64,
                [typeof(float)] = DbType.Single,
                [typeof(double)] = DbType.Double,
                [typeof(decimal)] = DbType.Decimal,
                [typeof(bool)] = DbType.Boolean,
                [typeof(string)] = DbType.String,
                [typeof(char)] = DbType.StringFixedLength,
                [typeof(Guid)] = DbType.Guid,
                [typeof(DateTime)] = DbType.DateTime,
                [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof(TimeSpan)] = DbType.Time,
                [typeof(byte[])] = DbType.Binary,
                [typeof(byte?)] = DbType.Byte,
                [typeof(sbyte?)] = DbType.SByte,
                [typeof(short?)] = DbType.Int16,
                [typeof(ushort?)] = DbType.UInt16,
                [typeof(int?)] = DbType.Int32,
                [typeof(uint?)] = DbType.UInt32,
                [typeof(long?)] = DbType.Int64,
                [typeof(ulong?)] = DbType.UInt64,
                [typeof(float?)] = DbType.Single,
                [typeof(double?)] = DbType.Double,
                [typeof(decimal?)] = DbType.Decimal,
                [typeof(bool?)] = DbType.Boolean,
                [typeof(char?)] = DbType.StringFixedLength,
                [typeof(Guid?)] = DbType.Guid,
                [typeof(DateTime?)] = DbType.DateTime,
                [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
                [typeof(TimeSpan?)] = DbType.Time,
                [typeof(object)] = DbType.Object
            };
        }

        public DbTypeMapper()
        {
            this.mInstanceMap = _StaticMap;
        }

        /// <summary>
        /// Modifica la mappatura dell'istanza corrente. Attenzione: la modifica di un mapping comporta la clonazione del mapping di default solo per il database per cui è richiesto
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dbt"></param>
        public void ChangeCurrentMap(Type t, DbType dbt)
        {
            if (!this.mInstanceCustom)
            {
                this.mInstanceMap = this.cloneStaticMap();
                this.mInstanceCustom = true;
            }

            this.mInstanceMap[t] = dbt;
        }

        /// <summary>
        /// Modifica il mapping corrente per questa istanza con tipo esplicito
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbt"></param>
        public void ChangeCurrentMap<T>(DbType dbt)
        {
            this.ChangeCurrentMap(typeof(T), dbt);
        }


        /// <summary>
        /// Modifica la mappatura statica. la modifica ha effetto su tutti i db instanziati che non hanno modificato la mappatura
        /// La chiamata va inserita in un metodo statico
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dbt"></param>
        public static void ChangeStaticMap(Type t, DbType dbt)
        {
            _StaticMap[t] = dbt;
        }

        /// <summary>
        ///Modifica mappatura statica con tipo esplicito
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbt"></param>
        public static void ChangeStaticMap<T>(DbType dbt)
        {
            ChangeStaticMap(typeof(T), dbt);
        }


        /// <summary>
        /// Dato un type ritorna il corrispondente dbtype
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DbType GetDbTypeFor(Type t)
        {
            try
            {
                return this.mInstanceMap[t];
            }
            catch (Exception)
            {
                throw new DataBaseException($"DbType per il tipo {t} non trovato.");
            }
        }


        /// <summary>
        /// Ritorna una copia clonata dello static Map
        /// </summary>
        /// <returns></returns>
        private Dictionary<Type, DbType> cloneStaticMap()
        {
            return new Dictionary<Type, DbType>(_StaticMap);
        }
      
    }
}
