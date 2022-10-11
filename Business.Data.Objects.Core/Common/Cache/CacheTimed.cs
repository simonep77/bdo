using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/*==================================================================================================
    CacheManager.cs
    
    Cache ThreadSafe con strategia di tipo FIFO
==================================================================================================*/
namespace Business.Data.Objects.Common.Cache
{
    public class CacheTimed<TKey, T>
    {
        private Dictionary<TKey, CacheItem<TKey, T>> mDictionary;
        private int mCacheMaxSize = 512;
        private object mSyncLock = new object();
        public int DefaultTimeoutMinuti { get; set; } = 20;

        #region "INTERNAL CLASSES"

        /// <summary>
        /// Oggetto mantenuto in cache
        /// </summary>
        /// <remarks></remarks>
        protected class CacheItem<TK, TV>
        {
            public TK Key;
            public TV Value;
            public DateTime DtScad;

        }

        #endregion

        #region "PROPERTY"

        protected object SyncLock
        {
            get
            {
                return this.mSyncLock;
            }
        }


        public int MaxSize
        {
            get { return this.mCacheMaxSize; }
        }

        public int CurrentSize
        {
            get { return this.mDictionary.Count; }
        }

        #endregion

        #region "PUBLIC"

        public CacheTimed(int cacheMaxSize)
        {
            this.mCacheMaxSize = cacheMaxSize;
            this.mDictionary = new Dictionary<TKey, CacheItem<TKey, T>>(cacheMaxSize);
        }

        /// <summary>
        /// Svuota completamente cache
        /// </summary>
        /// <remarks></remarks>
        public void Reset()
        {
            lock (this.mSyncLock)
            {
                this.mDictionary.Clear();
            }
        }

        private decimal calcolaPercFree()
        {
            return (1m - Convert.ToDecimal(Math.Min(this.mDictionary.Count, this.mCacheMaxSize)) / Convert.ToDecimal(this.mCacheMaxSize));
        }



        /// <summary>
        /// Aggiunge o Aggiorna oggetto in cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="span"></param>
        public void SetObject(TKey key, T value)
        {
            lock (this.mSyncLock)
            {
                //In caso di eccesso elementi rimuove il 5% di quelli piu' vecchi
                if (this.CurrentSize >= this.MaxSize)
                {
                    this.mDictionary.Values
                        .OrderBy(x => x.DtScad)
                        .Take(this.MaxSize / 20).ToList()
                        .ForEach(x => this.mDictionary.Remove(x.Key));

                }

                if (!this.mDictionary.TryGetValue(key, out CacheItem<TKey, T> oNode))
                    this.mDictionary.Add(key,
                        new CacheItem<TKey, T> { Key = key, Value = value, DtScad = DateTime.Now.AddMinutes(this.DefaultTimeoutMinuti) });
            }


        }


        /// <summary>
        /// Ritorna Oggetto
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public T GetObject(TKey key)
        {
            lock (this.mSyncLock)
            {
                if (this.mDictionary.TryGetValue(key, out CacheItem<TKey, T> oNode))
                {
                    var dtNow = DateTime.Now;

                    if (oNode.DtScad >= dtNow)
                    {
                        //oNode.DtScad = dtNow.AddMinutes(this.DefaultTimeoutMinuti);
                        return oNode.Value;
                    }
                    else
                    {
                        //Se becchiamo uno scaduto eliminiamo tutti gli scaduti!
                        this.mDictionary.Values
                        .Where(x => x.DtScad < dtNow)
                        .ToList()
                        .ForEach(x => this.mDictionary.Remove(x.Key));
                    }
                }
            }

            return default(T);
        }


        /// <summary>
        /// Elimina oggetto da Cache se presente
        /// </summary>
        /// <param name="key"></param>
        /// <remarks></remarks>
        public void RemoveObject(TKey key)
        {
            lock (this.mSyncLock)
            {
                if (this.mDictionary.ContainsKey(key))
                    this.mDictionary.Remove(key);
            }
        }


        /// <summary>
        /// Verifica se oggetto già presente in cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsObject(TKey key)
        {
            lock (this.mSyncLock)
            { 
                return this.mDictionary.ContainsKey(key);
            }
        }



        /// <summary>
        /// Ritorna stringa con rappresentazione dei dati contenuti
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            lock (this.mSyncLock)
            {
                System.Text.StringBuilder oSb = new System.Text.StringBuilder(1000);
                int i = 0;

                oSb.Append("Key Type: ");
                oSb.AppendLine(typeof(TKey).Name);
                oSb.Append("Value Type: ");
                oSb.AppendLine(typeof(T).Name);
                oSb.Append("Size: ");
                oSb.Append(this.CurrentSize.ToString());
                oSb.Append("/");
                oSb.AppendLine(this.MaxSize.ToString());
                oSb.AppendLine(typeof(T).Name);
                oSb.AppendLine(string.Empty.PadRight(40, '-'));

                foreach (var item in this.mDictionary.Values)
                {
                    i++;
                    oSb.Append("Item ");
                    oSb.AppendLine(i.ToString("D4"));
                    oSb.Append(" > DtFine: ");
                    oSb.AppendLine(item.DtScad.ToString("yyyyMMddHHmmssfff"));
                    oSb.Append(" > Key: ");
                    oSb.AppendLine(item.Key.ToString());
                    if (item.Value != null)
                    {
                        oSb.Append(" > Value: ");
                        oSb.AppendLine(item.Value.ToString());

                        if (item.Value is System.Data.DataTable)
                        {
                            var t = item.Value as System.Data.DataTable;
                            using (var tw = new System.IO.StringWriter(oSb))
                            {
                                t.WriteXml(tw);
                            }
                        }
                    }
                    else
                    {
                        oSb.AppendLine(" > Value: NULL");
                    }
                    oSb.AppendLine();
                }

                return oSb.ToString();
            }
        }

        #endregion

    }
}