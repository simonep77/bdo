using System;
using System.Collections.Generic;
using System.Threading;

/*==================================================================================================
    CacheManager.cs
    
    Cache ThreadSafe con strategia di tipo FIFO
==================================================================================================*/
namespace Bdo.Cache
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
            public DateTime DtFine;
            
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
            lock(this.mSyncLock)
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
        /// <remarks></remarks>
        public void SetObject(TKey key, T value)
        {
            this.SetObject(key, value, TimeSpan.FromMinutes(this.DefaultTimeoutMinuti));
        }


        /// <summary>
        /// Aggiunge o Aggiorna oggetto in cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="span"></param>
        public void SetObject(TKey key, T value, TimeSpan span)
        {
            lock (this.mSyncLock)
            {
                //In caso di eccesso elementi rimuove quelli scaduti
                var perc = this.calcolaPercFree();
                var mult = 0;
                while (perc <0.2m)
                {
                    this.FreeByDate(DateTime.Now.AddMinutes(mult));
                    //Aggiunge 2 minuti a data val
                    mult += 2;
                    //Ricalcola percentuale
                    perc = calcolaPercFree();
                }
                    

                if (!this.mDictionary.TryGetValue(key, out CacheItem<TKey, T> oNode))
                    this.mDictionary.Add(key, 
                        new CacheItem<TKey, T> { Key=key, Value = value, DtFine=DateTime.Now.AddTicks(span.Ticks) }  );
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
                    if (DateTime.Now <= oNode.DtFine)
                        return oNode.Value;
                    else
                        this.RemoveObject(key); //Scaduto
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
            return this.mDictionary.ContainsKey(key);
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
                    oSb.AppendLine(item.DtFine.ToString("yyyyMMddHHmmssfff"));
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

        #region "PROTECTED"

        /// <summary>
        /// Se la cache ha raggiunto il massimo inizia ad eliminare elementi
        /// E' possibile specificare il numero di elementi da liberare
        /// </summary>
        /// <remarks></remarks>
        protected void FreeByDate(DateTime dtInput)
        {
            lock (this.mSyncLock)
            {
                var listDel = new List<TKey>();

                foreach (var item in this.mDictionary.Values)
                {
                    if (item.DtFine < dtInput)
                        listDel.Add(item.Key);
                }

                foreach (var item in listDel)
                {
                    this.RemoveObject(item);
                }
            }
        }

        #endregion
    }
}