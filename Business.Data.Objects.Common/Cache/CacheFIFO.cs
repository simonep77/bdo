using System;
using System.Collections.Generic;
using System.Threading;

/*==================================================================================================
    CacheFIFO.cs
    
    Cache ThreadSafe con strategia di tipo FIFO
==================================================================================================*/
namespace Business.Data.Objects.Common.Cache
{
    public class CacheFIFO<TKey, T> : CacheBase<TKey, T>
    {

        #region "PUBLIC"

        public CacheFIFO(int cacheMaxSize)
            :base(cacheMaxSize)
        {  }


        /// <summary>
        /// Aggiunge o Aggiorna oggetto in cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public override void SetObject(TKey key, T value)
        {
            LinkedListNode<CacheItem<TKey, T>> oNode = null;
            CacheItem<TKey, T> cacheItem = default(CacheItem<TKey, T>);

            lock (this.SyncLock)
            {
                if (!this.Dictionary.TryGetValue(key, out oNode))
                {
                    //Se necessario Libera Elementi
                    if (this.CurrentSize >= this.MaxSize)
                    {
                        this.FreeCacheSlots(Convert.ToInt32(this.MaxSize * 0.20));
                    }

                    //Imposta dati inserimento
                    cacheItem = new CacheItem<TKey, T>();
                    cacheItem.Keys.Add(key);

                    //Aggiunge all'inizio
                    oNode = new LinkedListNode<CacheItem<TKey, T>>(cacheItem);

                    //Aggiunge a dizionario
                    this.Dictionary.Add(key, oNode);

                    //Aggiunge in testa a lista
                    this.LinkedList.AddFirst(oNode);
                }
            }

            //Imposta Valore
            oNode.Value.Value = value;
        }



        /// <summary>
        /// Ritorna Oggetto
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override T GetObject(TKey key)
        {
            //Se esiste aggiorna, altrimenti lo aggiunge
            LinkedListNode<CacheItem<TKey, T>> aNode = null;

            if (this.Dictionary.TryGetValue(key, out aNode))
            {
                //Ritorna Valore
                return aNode.Value.Value;
            }
            else
            {
                return default(T);
            }
        }

        #endregion

    }
}