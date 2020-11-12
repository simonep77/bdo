using System.Collections.Generic;
using System.Linq;
using System.Threading;

/*==================================================================================================
    CacheManager.cs
    
    Cache ThreadSafe con strategia di tipo FIFO
==================================================================================================*/
namespace Business.Data.Objects.Common.Cache
{
    public abstract class CacheBase<TKey, T> : ICache<TKey, T>
    {
        private LinkedList<CacheItem<TKey, T>> mLinkedList;
        private Dictionary<TKey, LinkedListNode<CacheItem<TKey, T>>> mDictionary;
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

        protected LinkedList<CacheItem<TKey, T>> LinkedList
        {
            get
            { return this.mLinkedList; }
        }

        protected Dictionary<TKey, LinkedListNode<CacheItem<TKey, T>>> Dictionary
        {
            get
            { return this.mDictionary; }
        }

        public int MaxSize
        {
            get { return this.mCacheMaxSize; }
        }

        public int CurrentSize
        {
            get { return this.mLinkedList.Count; }
        }

        #endregion

        #region "PUBLIC"

        public CacheBase(int cacheMaxSize)
        {
            this.mCacheMaxSize = cacheMaxSize;
            this.mLinkedList = new LinkedList<CacheItem<TKey, T>>();
            this.mDictionary = new Dictionary<TKey, LinkedListNode<CacheItem<TKey, T>>>(cacheMaxSize);

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
                this.mLinkedList.Clear();
            }
        }


        /// <summary>
        /// Aggiunge un'altra chiave di riferimento per un oggetto gia' in cache
        /// </summary>
        /// <param name="keyBase"></param>
        /// <param name="keyNew"></param>
        public void AddAlternateKey(TKey keyBase, TKey keyNew)
        {
            LinkedListNode<CacheItem<TKey, T>> oNode = this.mDictionary[keyBase];
            oNode.Value.Keys.Add(keyNew);
            this.mDictionary.Add(keyNew, oNode);
        }


        /// <summary>
        /// Aggiunge o Aggiorna oggetto in cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public abstract void SetObject(TKey key, T value);


        /// <summary>
        /// Ritorna Oggetto
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract T GetObject(TKey key);


        /// <summary>
        /// Elimina oggetto da Cache se presente
        /// </summary>
        /// <param name="key"></param>
        /// <remarks></remarks>
        public void RemoveObject(TKey key)
        {
            LinkedListNode<CacheItem<TKey, T>> oNode = null;

            lock ((this.mSyncLock))
            {
                if (this.mDictionary.TryGetValue(key, out oNode))
                {
                    //Rimuove nodo da lista
                    this.mLinkedList.Remove(oNode);
                    //Rimuove tutte le chiavi presenti nel dictionary associate ad un nodo
                    while (oNode.Value.Keys.Count > 0)
                    {
                        this.mDictionary.Remove(oNode.Value.Keys[0]);
                        oNode.Value.Keys.RemoveAt(0);
                    }
                }

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
                LinkedListNode<CacheItem<TKey, T>> oNode = this.mLinkedList.First;
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

                while (oNode != null)
                {
                    i++;
                    oSb.Append("Item ");
                    oSb.AppendLine(i.ToString("D4"));
                    oSb.Append(" > Keys: ");
                    oSb.AppendLine(string.Join(",", oNode.Value.Keys.Select(k=>k.ToString())));
                    if (oNode.Value.Value != null)
                    {
                        oSb.Append(" > Value: ");
                        oSb.AppendLine(oNode.Value.Value.ToString());
                    }
                    else
                    {
                        oSb.AppendLine(" > Value: NULL");

                    }
                    oSb.AppendLine();

                    //Passa a successivo
                    oNode = oNode.Next;
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
                System.Console.WriteLine("{0} Attenzione! La cache ha superato il numero di oggetti consentiti ({1}). Verra' eseguita una cancellazione di {2} oggetti.", this.GetType().Name, this.mCacheMaxSize, numSlots);

                //Elimina ultimi elementi lista
                for (int i = 0; i < numSlots; i++)
                {
                    //Rimuove dal dizionario
                    this.RemoveObject(this.mLinkedList.Last.Value.Keys[0]);
                }
            }
        }

        #endregion
    }
}