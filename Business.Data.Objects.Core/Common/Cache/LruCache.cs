using Business.Data.Objects.Core.Schema.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/*==================================================================================================
    LruCache
    
    Cache ThreadSafe con strategia di tipo LRU
==================================================================================================*/
namespace Business.Data.Objects.Common.Cache
{
    /// <summary>
    /// Cache di tipo LRU
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class LruCache<K, V>: IDisposable
    {
        private Dictionary<K, CacheItem<K, V>> mDictionary;
        
        /// <summary>
        /// Dimensione massima
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// tempo di scadenza oggetti
        /// </summary>
        public TimeSpan Expire { get; }

               /// <summary>
        /// Indica se attiva la scadenza
        /// </summary>
        public bool DoExpire { get; }

        private ReaderWriterLockSlim mRW;

        private Timer mTimer;


        #region "INTERNAL CLASSES"

        /// <summary>
        /// Oggetto mantenuto in cache
        /// </summary>
        /// <remarks></remarks>
        protected class CacheItem<TK, TV>
        {
            public TK Key;
            public TV Value;
            public long Hits;
            public long Ticks;

        }

        #endregion

        #region "PROPERTY"

        /// <summary>
        /// Dimensione corrente
        /// </summary>
        public int CurrentSize
        {
            get
            {
                this.mRW.EnterReadLock();
                try
                {
                    return this.mDictionary.Count;
                }
                finally
                {
                    this.mRW.ExitReadLock();
                }
            }
        }


        /// <summary>
        /// Ritorna tutte le chiavi registrate
        /// </summary>
        public List<K> AllKeys
        {
            get
            {
                this.mRW.EnterReadLock();
                try
                {
                    return this.mDictionary.Keys.ToList();
                }
                finally
                {
                    this.mRW.ExitReadLock();
                }
            }
        }


        //public List<V> AllValues
        //{
        //    get
        //    {
        //        this.mRW.EnterReadLock();
        //        try
        //        {
        //            return this.mDictionary.Values.Select(x => x.Value).ToList();
        //        }
        //        finally
        //        {
        //            this.mRW.ExitReadLock();
        //        }
        //    }
        //}

        #endregion

        #region "PUBLIC"

        /// <summary>
        /// Crea una cache LRU.
        /// </summary>
        /// <param name="capacity">Numero massimo elementi</param>
        /// <param name="expire">Dopo quanto un elemento viene considerato scaduto. Default nessuna scadenza</param>
        /// <param name="refreshOnAccess">Indica se aggiornare la scadenza ad ogni accesso</param>
        public LruCache(int capacity = 512, TimeSpan expire = default)
        {
            this.mRW = new ReaderWriterLockSlim();
            this.Capacity = capacity;
            this.Expire = expire;
            this.DoExpire = this.Expire != TimeSpan.Zero;
            this.mDictionary = new Dictionary<K, CacheItem<K, V>>(this.Capacity);

            //Se richiesta verifica degli scaduti
            if (this.DoExpire)
            {
                this.mTimer = new Timer(s =>
                {
                    this.mRW.EnterWriteLock();
                    try
                    {
                        var dNow = DateTime.Now.Ticks;
                        this.mDictionary.Values.AsParallel().Where(x => x.Ticks < dNow).ToList().ForEach(x => this.mDictionary.Remove(x.Key));
                    }
                    finally
                    {
                        this.mRW.ExitWriteLock();
                    }
                },
                this, this.Expire, this.Expire);
            }
        }

        /// <summary>
        /// Svuota completamente cache
        /// </summary>
        /// <remarks></remarks>
        public void Clear()
        {
            this.mRW.EnterWriteLock();
            try
            {
                this.mDictionary.Clear();
            }
            finally
            {
                this.mRW.ExitWriteLock();
            }
        }

        /// <summary>
        /// Libera % slot da cache
        /// </summary>
        private void performClean()
        {
            this.mDictionary.Values
                .OrderBy(x => x.Ticks)
                .ThenBy(x => x.Hits)
                .Take((this.Capacity / 100) * 20)
                .ToList()
                .ForEach(x => this.mDictionary.Remove(x.Key));
        }


        /// <summary>
        /// Aggiunge o Aggiorna oggetto in cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="span"></param>
        public void AddOrUpdate(K key, V value)
        {
            this.mRW.EnterWriteLock();
            try
            {
                //Verifichiamo se gia esiste
                if (!this.mDictionary.TryGetValue(key, out var item))
                {
                    //Se siamo arrivati a tappo liberiamo
                    if (this.mDictionary.Count >= this.Capacity)
                        this.performClean();

                    //Creiamo item con chiave e lo aggiungiamo
                    item = new CacheItem<K, V>
                    {
                        Key = key,
                    };
                    this.mDictionary[key] = item;
                }

                //Impostiamo i dati
                item.Value = value;

                if (this.DoExpire)
                    item.Ticks = DateTime.Now.Add(this.Expire).Ticks;
            }
            finally
            {
                this.mRW.ExitWriteLock();
            }
        }


        public V GetOrAdd(K key, Func<V> fn)
        {
            this.mRW.EnterReadLock();
            try
            {
                //Verifichiamo se gia esiste o se non scaduta
                if (!this.mDictionary.TryGetValue(key, out var item) || (this.DoExpire && item.Ticks < DateTime.Now.Ticks))
                {
                    //Dobbiamo rilasciare la lettura
                    this.mRW.ExitReadLock();
                    //Avviamo la scrittura
                    this.mRW.EnterWriteLock();
                    try
                    {
                        //Riverifichiamo se qualcuno è entrato prima di me nel write
                        if (!this.mDictionary.TryGetValue(key, out item))
                        {
                            //Se siamo arrivati a tappo liberiamo
                            if (this.mDictionary.Count >= this.Capacity)
                                this.performClean();

                            //Aggiungiamo
                            item = new CacheItem<K, V>
                            {
                                Key = key,
                                Value = fn.Invoke(),
                            };
                            this.mDictionary[key] = item;

                            //Imposta scadenza
                            if (this.DoExpire)
                                item.Ticks = DateTime.Now.Add(this.Expire).Ticks;

                            return item.Value;
                        }
                    }
                    finally
                    {
                        this.mRW.ExitWriteLock();
                    }
                }

                //aggiorna utilizzo
                Interlocked.Increment(ref item.Hits);

                return item.Value;
            }
            finally
            {
                //Rilascia la lettura se ancora impostata
                if (this.mRW.IsReadLockHeld)
                    this.mRW.ExitReadLock();
            }
        }

        /// <summary>
        /// Ritorna valore o default
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V Get(K key)
        {
            this.TryGet(key, out var v);

            return v;
        }


        /// <summary>
        /// Tenta di leggere il valore e ritorna true o false se trovato. 
        /// In caso positivo value contiene il valore trovato altrimenti il valore di default del tipo
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(K key, out V value)
        {
            this.mRW.EnterReadLock();
            try
            {
                //Verifichiamo se gia esiste
                if (this.mDictionary.TryGetValue(key, out var item))
                {

                    //aggiorna utilizzo
                    Interlocked.Increment(ref item.Hits);

                    //Questa è soggetta a race...
                    if (this.DoExpire)
                        Interlocked.Exchange(ref item.Ticks, DateTime.Now.Ticks);

                    //Imposta
                    value = item.Value;
                    return true;
                }

                //Non trovato
                value = default;
                return false;
            }
            finally
            {
                this.mRW.ExitReadLock();
            }

        }

        /// <summary>
        /// Elimina oggetto da Cache se presente
        /// </summary>
        /// <param name="key"></param>
        /// <remarks></remarks>
        public void Remove(K key)
        {
            this.mRW.EnterWriteLock();
            try
            {
                this.mDictionary.Remove(key);
            }
            finally
            {
                this.mRW.ExitWriteLock();
            }
        }


        /// <summary>
        /// Verifica se oggetto già presente in cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(K key)
        {
            return this.TryGet(key, out _);
        }



        /// <summary>
        /// Ritorna stringa con rappresentazione dei dati contenuti
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            this.mRW.EnterReadLock();
            try
            {
                System.Text.StringBuilder oSb = new System.Text.StringBuilder(1000);
                int i = 0;

                oSb.Append("Key Type: ");
                oSb.AppendLine(typeof(K).Name);
                oSb.Append("Value Type: ");
                oSb.AppendLine(typeof(V).Name);
                oSb.Append("Size: ");
                oSb.Append(this.mDictionary.Count.ToString());
                oSb.Append("/");
                oSb.AppendLine(this.Capacity.ToString());
                oSb.AppendLine(string.Empty.PadRight(40, '-'));

                foreach (var item in this.mDictionary.Values)
                {
                    i++;
                    oSb.Append("Item ");
                    oSb.AppendLine(i.ToString("D4"));
                    oSb.Append(" > Key: ");
                    oSb.AppendLine(item.Key.ToString());
                    oSb.Append(" > Hits: ");
                    oSb.AppendLine(item.Hits.ToString());
                    oSb.Append(" > Scad: ");
                    oSb.AppendLine($"{DateTime.FromBinary(item.Ticks):yyyy-MM-dd-HH:mm:ss}");
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
            finally
            {
                this.mRW.ExitReadLock();
            }
        }

        public void Dispose()
        {
            ////Disattiva timer
            this.mTimer?.Dispose();
            //Svuota cache
            this.Clear();
        }

        #endregion

    }
}