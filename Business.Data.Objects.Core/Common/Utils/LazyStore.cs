using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Common.Utils
{
    /// <summary>
    /// Oggetto per caching Lazy
    /// </summary>
    public class LazyStore
    {


        private Dictionary<string, object> mLazyDic = new Dictionary<string, object>();

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
            if (!mLazyDic.TryGetValue(uniqueKey, out object obj))
            {
                obj = fn();
                mLazyDic.Add(uniqueKey, obj);
            }

            return (T1)obj;
        }


        /// <summary>
        /// Resetta dei dati eventualmente cached sull'oggetto in modo che l'accesso successivo esegua il refresh
        /// </summary>
        /// <param name="uniqueKey"></param>
        public void Reset(string uniqueKey)
        {
            this.mLazyDic.Remove(uniqueKey);
        }

        /// <summary>
        /// Resetta tutti i dati cache
        /// </summary>
        /// <param name="uniqueKey"></param>
        public void ResetAll()
        {
            this.mLazyDic.Clear();
        }

        /// <summary>
        /// Forza l'impostazione di un valore Lazy (per usi successivi)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uniqueKey"></param>
        /// <param name="value"></param>
        public void Set<T1>(string uniqueKey, T1 value)
        {
            this.mLazyDic[uniqueKey] = value;
        }

    }
}
