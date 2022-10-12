using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Data.Objects.Core.Utils
{
    /// <summary>
    /// Classe per il mantenimento di riferimenti di oggetti. NON THREADSAFE!
    /// </summary>
    internal class ReferenceManager<TKEY, TVALUE> : IDisposable
    {
        private Dictionary<TKEY, InnerReference> mStore;
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
        public bool IsActive { get; set; } = true;


        /// <summary>
        /// Ritorna numero elementi
        /// </summary>
        public int Count => this.mStore.Count;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="capacity"></param>
        public ReferenceManager(int capacity)
        {
            this.mStore = new Dictionary<TKEY, InnerReference>(capacity);
        }


        /// <summary>
        /// Elimina tutti i riferimenti
        /// </summary>
        public void Clear()
        {
            this.mStore.Clear();
        }


        /// <summary>
        /// Rimuove chiave singola
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKEY key)
        {
            if (!this.IsActive)
                return;

            this.mStore.Remove(key);

        }


        /// <summary>
        /// imposta oggetto con chiave
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(TKEY key, TVALUE value)
        {
            if (!this.IsActive)
                return;

            if (this.mStore.TryGetValue(key, out var oRef))
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
                this.mStore.Add(key, new InnerReference(System.Threading.Interlocked.Increment(ref this.mRefCounter), key, value));
            }
        }



        /// <summary>
        /// Cerca oggetto
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TVALUE Get(TKEY key)
        {
            if (!this.IsActive)
                return default(TVALUE);

            if (this.mStore.TryGetValue(key, out var oRef))
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

        /// <summary>
        /// Esegue pulizia entry non più referenziate
        /// </summary>
        public void CleanDeadEntries()
        {
            if (!this.IsActive)
                return;

            this.performCleanDeadEntries();
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
            this.mStore.Where(x => x.Value.Wref.IsAlive).AsParallel().ToList().ForEach(x => this.mStore.Remove(x.Key));
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
