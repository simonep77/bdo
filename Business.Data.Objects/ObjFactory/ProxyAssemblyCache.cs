using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using Bdo.Objects;
using Bdo.Objects.Base;
using Bdo.Schema.Definition;

namespace Bdo.ObjFactory
{
    /// <summary>
    /// Classe per la gestione sicura dei caricamenti delle classi proxy e della loro gestione
    /// </summary>
    internal class ProxyAssemblyCache
    {

        private ProxyAssemblyDaoDiz mCacheDao = new ProxyAssemblyDaoDiz(5);
        private ProxyAssemblyBizDiz mCacheBiz = new ProxyAssemblyBizDiz(2);
        private Dictionary<string, string> mResolver = new Dictionary<string, string>(5);

        /// <summary>
        /// Singleton main instance
        /// </summary>
        internal readonly static ProxyAssemblyCache Instance = new ProxyAssemblyCache();
        internal static Int64 _ObjeRefIdCounter = Int64.MinValue;

        #region INTERNAL CLASSES

        #region DAO

        /// <summary>
        /// Dizionario per la gestione dei proxy DAO
        /// </summary>
        internal class ProxyAssemblyDaoDiz : Dictionary<string, ProxyAssemblyDao>
        {
            public object WriteLock = new object();
            public ProxyAssemblyDaoDiz(int capacity): base(capacity)
            { }

            public ProxyAssemblyDaoDiz(ProxyAssemblyDaoDiz other)
                : base(other)
            { }
        }

        /// <summary>
        /// Classe per la gestione interna dei lookup DAO
        /// </summary>
        internal class ProxyAssemblyDao 
        {
            public string Key;
            public Assembly SrcAss;
            public Assembly DynAss;
            public ProxyEntryDaoDic TypeDaoEntries;
        }

        #endregion

        #region BIZ

        /// <summary>
        /// Dizionario per la gestione dei proxy BIZ
        /// </summary>
        internal class ProxyAssemblyBizDiz : Dictionary<long, ProxyAssemblyBiz>
        {
            public object WriteLock = new object();
            public ProxyAssemblyBizDiz(int capacity)
                : base(capacity)
            { }

            public ProxyAssemblyBizDiz(ProxyAssemblyBizDiz other)
                : base(other)
            { }
        }

        /// <summary>
        /// Classe per la gestione interna dei lookup BIZ
        /// </summary>
        internal class ProxyAssemblyBiz
        {
            public long Key;
            public Assembly SrcAss;
            public ProxyEntryBizDic TypeBizEntries;
        }

        #endregion

        #endregion

        #region public

        //Nel costruttore della classe aggiunge la routine base di risoluzione
        public ProxyAssemblyCache()
        {
            AppDomain.CurrentDomain.AssemblyResolve += this.Domain_Proxy_Resolver;
        }

        /// <summary>
        /// Risoluzione assembly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Assembly Domain_Proxy_Resolver(object sender, ResolveEventArgs args)
        {
            string assName = null;

            if (this.mResolver.TryGetValue(args.Name, out assName))
                return this.mCacheDao[assName].DynAss;

            return null;
        }

        #region DAO

        /// <summary>
        /// Get Entry
        /// </summary>
        /// <param name="entryType"></param>
        /// <returns></returns>
        public ProxyEntryDao GetDaoEntry(Type entryType)
        {
            var lKey = entryType.Assembly.FullName;
            ProxyAssemblyDao pxa = null;

            //Controlla presenza
            if (!this.mCacheDao.TryGetValue(lKey, out pxa))
            {
                //Lock Scrittura
                lock (this.mCacheDao.WriteLock)
                {
                    //Ricontrolla
                    if (!this.mCacheDao.TryGetValue(lKey, out pxa))
                    {
                        //Invia evento
                        pxa = new ProxyAssemblyDao()
                        {
                            Key = lKey,
                            SrcAss = entryType.Assembly,
                            TypeDaoEntries = new ProxyEntryDaoDic(100),
                        };

                        //Avvia evento
                        ProxyTypeBuilder.BuildDaoProxyFromAssembly(pxa);

                        //Rigenera nuovo elenco Proxy
                        var newCache = new ProxyAssemblyDaoDiz(this.mCacheDao);
                        newCache.Add(lKey, pxa);

                        //Rigenera nuovo elenco Resolvers
                        Dictionary<string, string> newResolver = new Dictionary<string, string>(this.mResolver);
                        newResolver.Add(pxa.DynAss.FullName, lKey);

                        //Assegnazione
                        this.mCacheDao = newCache;
                        this.mResolver = newResolver;
                    }
                }
               
            }

            //Ritorna entry
            return pxa.TypeDaoEntries[entryType.TypeHandle.Value.ToInt64()];
        }

        /// <summary>
        /// Crea oggetto senza dataschema
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal object CreateDaoObj(Type type)
        {
            return this.CreateDaoObj(type, false);
        }


        /// <summary>
        /// Crea un oggetto specificando se valorizzare o meno il dataschema
        /// </summary>
        /// <param name="type"></param>
        /// <param name="withData"></param>
        /// <returns></returns>
        internal object CreateDaoObj(Type type, bool withData)
        {
            try
            {
                //Crea istanza
                ProxyEntryDao oTypeEntry = this.GetDaoEntry(type);

                //Istanzia
                var o = oTypeEntry.Create() as DataObjectBase;

                //Imposta schema su oggetto
                o.mClassSchema = oTypeEntry.ClassSchema;
                o.ObjectRefId = Interlocked.Increment(ref _ObjeRefIdCounter);

                if(withData)
                    o.mDataSchema = new Schema.Usage.DataSchema(o.mClassSchema.Properties.Count, o.mClassSchema.ObjCount);

                //Ritorna
                return o;
            }
            catch (KeyNotFoundException)
            {
                throw new TypeFactoryException("Tipo {0} non trovato. E' possibile che si siano verificati errori in fase di generazione schema.", type.Name);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw;
                }
            }
        }





        /// <summary>
        /// Crea oggetto del tipo definito senza schema (es. tipo lista)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal object CreateDaoNoSchemaObj(Type type)
        {
            try
            {
                //Crea istanza
                ProxyEntryDao oTypeEntry = this.GetDaoEntry(type);

                //Istanzia
                object o = oTypeEntry.Create();

                //Ritorna
                return o;
            }
            catch (KeyNotFoundException)
            {
                throw new TypeFactoryException("Tipo {0} non trovato. E' possibile che si siano verificati errori in fase di generazione schema.", type.Name);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Ritorna schema associato a tipo
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal ClassSchema GetClassSchema(Type type)
        {
            return this.GetDaoEntry(type).ClassSchema;
        }

        #endregion




        #region BIZ

        public ProxyEntryBiz GetBizEntry(Type tBiz)
        {
            long lKey = tBiz.Assembly.ManifestModule.MetadataToken;
            ProxyAssemblyBiz pxa = null;

            //Controlla presenza
            if (!this.mCacheBiz.TryGetValue(lKey, out pxa))
            {
                //Lock Scrittura
                lock (this.mCacheBiz.WriteLock)
                {
                    //Ricontrolla
                    if (!this.mCacheBiz.TryGetValue(lKey, out pxa))
                    {
                        //Invia evento
                        pxa = new ProxyAssemblyBiz()
                        {
                            Key = lKey,
                            SrcAss = tBiz.Assembly,
                            TypeBizEntries = new ProxyEntryBizDic(30)
                        };

                        //Avvia evento
                        ProxyTypeBuilder.BuildBizProxyFromAssembly(pxa);

                        //Rigenera nuovo elenco Proxy
                        var newCache = new ProxyAssemblyBizDiz(this.mCacheBiz);
                        newCache.Add(lKey, pxa);

                        //Assegnazione
                        this.mCacheBiz = newCache;
                    }
                }

            }

            //Ritorna entry
            return pxa.TypeBizEntries[tBiz.TypeHandle.Value.ToInt64()];
        }

        /// <summary>
        /// Crea istanza di BusinessObject utilizzando un costruttore dinamico
        /// </summary>
        /// <param name="tBiz"></param>
        /// <param name="slot"></param>
        /// <param name="keyName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal BusinessObjectBase CreateBizObj(Type tBiz, BusinessSlot slot, bool createNew,string keyName, params object[] args)
        {
            try
            {
                var entry = ProxyAssemblyCache.Instance.GetBizEntry(tBiz);

                DataObjectBase oDal;

                if (string.IsNullOrEmpty(keyName))
                    oDal = slot.CreateObjectByType(entry.DalType);
                else
                {
                    if (createNew)
                        oDal = slot.LoadObjOrNewInternalByKEY(keyName, entry.DalType, args);
                    else
                        oDal = slot.LoadObjectInternalByKEY(keyName, entry.DalType, true, args);
                }

                //Crea oggetto
                return CreateBizObj(entry, oDal);

            }
            catch (KeyNotFoundException)
            {
                throw new TypeFactoryException("Tipo {0} non trovato nella cache. Anomalia grave!", tBiz.Name);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw ex;
            }
        }


        /// <summary>
        /// Istanzia Biz object da una entry
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="dalObj"></param>
        /// <returns></returns>
        internal BusinessObjectBase CreateBizObj(ProxyEntryBiz entry, DataObjectBase dalObj)
        {
            if (entry.Factory == null)
                return entry.Create(dalObj);
            else
                return entry.Factory.Create(dalObj);
        }


        #endregion

        #endregion


    }
}
