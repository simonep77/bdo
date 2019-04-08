using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Cache
{
    /// <summary>
    /// Interfaccia per cache
    /// </summary>
    /// <typeparam name="TKey">Chiave di ricerca</typeparam>
    /// <typeparam name="T">Oggetto in cache</typeparam>
    interface ICache<TKey, T>
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
        /// Aggiunge una chiave alternativa ad un oggetto gia' in cache
        /// </summary>
        /// <param name="keyBase"></param>
        /// <param name="keyNew"></param>
        void AddAlternateKey(TKey keyBase, TKey keyNew);


        /// <summary>
        /// Verifica se oggetto è già presente in cache 
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
