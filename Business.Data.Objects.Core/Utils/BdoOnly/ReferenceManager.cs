using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Utils
{
    /// <summary>
    /// Classe per il mantenimento di riferimenti di oggetti
    /// </summary>
    internal class ReferenceManager<TKEY, TVALUE>: IDisposable
    {
        private bool mIsActive = true;
        private Dictionary<TKEY, InnerReference> mStore;
        private object mSyncRoot = new object();
        private long mRefCounter = long.MinValue;

        #region inner classes

        /// <summary>
        /// Classe di contenimento del riferimento anche multiplo
        /// </summary>
        private class InnerReference
        {
            public long RefId;
            public TKEY Key;
            public WeakReference Wref;

            public InnerReference(long refId, TKEY key, TVALUE value)
            {
                this.RefId = refId;
                this.Key = key;
                this.Wref = new WeakReference(value, false);
            }
        }

        #endregion

        #region public

        /// <summary>
        /// Indica se Tracking Attivo
        /// </summary>
        public bool IsActive
        {
            get
            {
                lock (this.mSyncRoot)
                {
                    return this.mIsActive;
                }
            }
            set
            {
                lock (this.mSyncRoot)
                {
                    this.mIsActive = value;
                }
            }
        }

        /// <summary>
        /// Ritorna numero elementi
        /// </summary>
        public int Count
        {
            get 
            {
                lock (this.mSyncRoot)
                {
                    return this.mStore.Count;     
                }
                
            }
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="capacity"></param>
        public ReferenceManager(int capacity)
        {
            this.mStore = new Dictionary<TKEY, InnerReference>(capacity);
        }


        /// <summary>
        /// Acquisisce il lock del reference manager per eseguire più operazioni in modalità atomica
        /// </summary>
        public void Lock()
        {
            System.Threading.Monitor.Enter(this.mStore) ;
        }


        /// <summary>
        /// Libera il lock acquisito
        /// </summary>
        public void Unlock()
        {
            System.Threading.Monitor.Exit(this.mStore);
        }

        /// <summary>
        /// Elimina tutti i riferimenti
        /// </summary>
        public void Clear()
        {
            lock (this.mSyncRoot)
            {
                this.mStore.Clear();
            }
        }


        /// <summary>
        /// Rimuove chiave singola
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKEY key)
        {
            lock (this.mSyncRoot)
            {
                if (!this.mIsActive)
                    return;

                this.mStore.Remove(key);

            }

        }


        ///// <summary>
        ///// Rimuove oggetti multipli
        ///// </summary>
        ///// <param name="keys"></param>
        //public void RemoveM(IEnumerable<TKEY> keys)
        //{
        //    lock (this.mSyncRoot)
        //    {
        //        if (!this.mIsActive)
        //            return;

        //        foreach (var key in keys)
        //        {
        //            this.mStore.Remove(key);
        //        }
        //    }
        //}


        /// <summary>
        /// imposta oggetto con chiave
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(TKEY key, TVALUE value)
        {
            lock (this.mSyncRoot)
            {
                if (!this.mIsActive)
                    return;

                InnerReference oRefItem = null;

                InnerReference oRef = null;
                if (this.mStore.TryGetValue(key, out oRef))
                {
                    //esiste, verifichiamo se medesimo riferimento
                    if (!oRef.Wref.IsAlive)
                    {
                        //Oggetto scaduto: lo reimpostiamo e usciamo
                        oRef.Wref.Target = value;
                        return;
                    }
                }
                else
                {
                    //Non esiste: lo aggiungiamo ma creando un solo riferimento
                    if (oRefItem == null)
                        oRefItem = new InnerReference(System.Threading.Interlocked.Increment(ref this.mRefCounter), key, value);

                    this.mStore.Add(key, oRefItem);
                }
            }
        }


        ///// <summary>
        ///// imposta oggetto con chiavi multiple
        ///// </summary>
        ///// <param name="keys"></param>
        ///// <param name="value"></param>
        //public void SetM(IEnumerable<TKEY> keys, TVALUE value)
        //{
        //    lock (this.mSyncRoot)
        //    {
        //        if (!this.mIsActive)
        //            return;

        //        InnerReference oRefItem = null;

        //        foreach (var key in keys)
        //        {
        //            InnerReference oRef = null;
        //            if (this.mStore.TryGetValue(key, out oRef))
        //            {
        //                //esiste, verifichiamo se medesimo riferimento
        //                if (!oRef.Wref.IsAlive)
        //                {
        //                    //Oggetto scaduto: lo reimpostiamo e usciamo
        //                    oRef.Wref.Target = value;
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                //Non esiste: lo aggiungiamo ma creando un solo riferimento
        //                if (oRefItem == null)
        //                    oRefItem = new InnerReference(System.Threading.Interlocked.Increment(ref this.mRefCounter), keys, value);

        //                this.mStore.Add(key, oRefItem);
        //            } 
        //        }
        //    }
        //}


        /// <summary>
        /// Cerca oggetto
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TVALUE Get(TKEY key)
        {
            lock (this.mSyncRoot)
            {
                if (!this.mIsActive)
                    return default(TVALUE);

                InnerReference oRef = null;

                if (this.mStore.TryGetValue(key, out oRef))
                {
                    //esiste, verifichiamo se medesimo riferimento
                    if (oRef.Wref.IsAlive)
                        //oggetto vivo: lo ritorniamo
                        return (TVALUE)oRef.Wref.Target;
                    else
                        //Oggetto scaduto: lo eliminiamo
                        this.mStore.Remove(key);
                }

                return default(TVALUE);
            }

        }

        /// <summary>
        /// Esegue pulizia entry non più referenziate
        /// </summary>
        /// <param name="issync"></param>
        public void CleanDeadEntries(bool issync)
        {
            if (issync)
            {
                this.performCleanDeadEntries();
            }
            else
            {
                System.Threading.ThreadStart pr = this.performCleanDeadEntries;
                pr.BeginInvoke(null, null);
            }
        }

        /// <summary>
        /// Ritorna dump "basico" con quanto presente nello Store
        /// </summary>
        /// <returns></returns>
        public string PrintDebug()
        {
            var sb = new StringBuilder(10000);
            long idx = 0;
            foreach (var item in this.mStore)
            {
                idx++;
                sb.AppendFormat(@"#{0} > RefId: {1}, Key: {2}, Value: {3}", idx.ToString().PadLeft(10, '0'), item.Value.RefId, item.Key, item.Value.Wref.Target);
                sb.AppendLine();
            }

            return sb.ToString();
        }



        #endregion

        #region private

        /// <summary>
        /// esegue pulizia dei riferimenti non più attivi
        /// </summary>
        private void performCleanDeadEntries()
        {
            lock (this.mSyncRoot)
            {
                List<TKEY> lstRemove = new List<TKEY>(10);
                //Scan
                foreach (var pair in this.mStore)
                {
                    if (pair.Value.Wref.IsAlive)
                        lstRemove.Add(pair.Value.Key);
                }
                //Remove
                for (int i = 0; i < lstRemove.Count; i++)
                {
                    this.Remove(lstRemove[i]);
                }
            }
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            this.Clear();
        }

        #endregion
    }
}
