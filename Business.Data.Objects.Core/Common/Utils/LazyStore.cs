using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Data.Objects.Core.Common.Utils
{
    /// <summary>
    /// Oggetto per caching Lazy
    /// </summary>
    public class LazyStore: IDisposable
    {

        private Dictionary<string, object> LazyDic = new Dictionary<string, object>();

        /// <summary>
        /// Funzione di caricamento oggetto lazy tipizzato
        /// </summary>
        /// <returns></returns>
        public delegate T1 LazyLoadFunc<T1>();


        /// <summary>
        /// Ritorna oggetto precedentemente caricato oppure lo carica tramite la funzione in input e lo memorizza per accessi successivi
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uniqueKey"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public T1 Get<T1>(string uniqueKey, LazyLoadFunc<T1> fn)
        {
            if (!this.LazyDic.TryGetValue(uniqueKey, out object obj))
            {
                obj = fn();
                this.LazyDic.Add(uniqueKey, obj);
            }

            return (T1)obj;
        }

        /// <summary>
        /// Resetta dei dati eventualmente cached sull'oggetto in modo che l'accesso successivo esegua il refresh
        /// </summary>
        /// <param name="uniqueKey"></param>
        public void Reset(string uniqueKey) => this.LazyDic.Remove(uniqueKey);

        /// <summary>
        /// Resetta tutti i dati cache
        /// </summary>
        /// <param name="uniqueKey"></param>
        public void ResetAll() => this.LazyDic.Clear();

        /// <summary>
        /// Forza l'impostazione di un valore Lazy (per usi successivi)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uniqueKey"></param>
        /// <param name="value"></param>
        public void Set<T1>(string uniqueKey, T1 value) => this.LazyDic[uniqueKey] = value;

        public void Dispose()
        {
            //Esegue il dispose di tutte le istanze Idisposable agganciate al lazy store
            foreach (var item in this.LazyDic.Values)
            {
                (item as IDisposable)?.Dispose();
            }
        }
    }
}
