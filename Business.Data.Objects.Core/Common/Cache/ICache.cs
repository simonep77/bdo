using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Cache
{
    /// <summary>
    /// Interfaccia per cache
    /// </summary>
    /// <typeparam name="TKey">Chiave di ricerca</typeparam>
    /// <typeparam name="T">Oggetto in cache</typeparam>
    public interface ICache<TKey, T>
    {
        /// <summary>
        /// Dimensione attuale
        /// </summary>
        int CurrentSize { get; }

        /// <summary>
        /// Dimensione massima
        /// </summary>
        int MaxSize { get; }

        /// <summary>
        /// Svuota Cache
        /// </summary>
        void Reset();


        /// <summary>
        /// Aggiunge o Aggiorna oggetto in cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks></remarks>
        void SetObject(TKey key, T value);



        /// <summary>
        /// Verifica se oggetto � gi� presente in cache 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsObject(TKey key);


        /// <summary>
        /// Ritorna Oggetto
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        T GetObject(TKey key);


        /// <summary>
        /// Elimina oggetto da Cache se presente
        /// </summary>
        /// <param name="key"></param>
        /// <remarks></remarks>
        void RemoveObject(TKey key);


        /// <summary>
        /// Ritorna stringa con rappresentazione contenuto cache
        /// </summary>
        /// <returns></returns>
        string Print();
        
    }
}
