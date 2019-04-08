using System.Collections.Generic;
using System.Threading;

/*==================================================================================================
    CacheManager.cs
    
    Cache ThreadSafe con strategia di tipo FIFO
==================================================================================================*/
namespace Bdo.Cache
{
    public class CacheSimple<TKey, T> : ICache<TKey, T>
    {
        private Dictionary<TKey, T> mDictionary;
        private int mCacheMaxSize = 512;
        private object mSyncLock = new object();

        #region "INTERNAL CLASSES"

        /// <summary>
        /// Oggetto mantenuto in cache
        /// </summary>
        /// <remarks></remarks>
        protected class CacheItem<TK, TV>
        {
            public List<TK> Keys = new List<TK>();
            public TV Value;
            
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

        public CacheSimple(int cacheMaxSize)
        {
            this.mCacheMaxSize = cacheMaxSize;
            this.mDictionary = new Dictionary<TKey, T>(cacheMaxSize);

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


        /// <summary>
        /// Aggiunge un'altra chiave di riferimento per un oggetto gia' in cache
        /// </summary>
        /// <param name="keyBase"></param>
        /// <param name="keyNew"></param>
        public void AddAlternateKey(TKey keyBase, TKey keyNew)
        {
            this.SetObject(keyNew, this.GetObject(keyBase));
        }


        /// <summary>
        /// Aggiunge o Aggiorna oggetto in cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public void SetObject(TKey key, T value)
        {
            T oNode;

            lock (this.mSyncLock)
            {
                //In caso di eccesso elementi timuove il 20 %
                if (this.mDictionary.Count >= this.mCacheMaxSize)
                    this.FreeCacheSlots(this.mCacheMaxSize / 20);

                if (!this.mDictionary.TryGetValue(key, out oNode))
                    this.mDictionary.Add(key, value);
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
            T oNode;

            if (this.mDictionary.TryGetValue(key, out oNode))
                return oNode;

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

                foreach (var item in this.mDictionary)
                {
                    i++;
                    oSb.Append("Item ");
                    oSb.AppendLine(i.ToString("D4"));
                    oSb.Append(" > Key: ");
                    oSb.AppendLine(item.Key.ToString());
                    if (item.Value != null)
                    {
                        oSb.Append(" > Value: ");
                        oSb.AppendLine(item.Value.ToString());
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
        protected void FreeCacheSlots(int numSlots)
        {
            lock (this.mSyncLock)
            {
                //Deve scrivere warning
                //System.Console.WriteLine("{0} Attenzione! La cache ha superato il numero di oggetti consentiti ({1}). Verra' eseguita una cancellazione di {2} oggetti.", this.GetType().Name, this.mCacheMaxSize, numSlots);

                while (numSlots > 0 && this.mDictionary.Count > 0)
                {
                    TKey key = default(TKey);

                    foreach (var item in this.mDictionary)
                    {
                        key = item.Key;
                        break;
                    }

                    this.RemoveObject(key);

                    //Decrementa
                    numSlots--;
                }
            }
        }

        #endregion
    }
}