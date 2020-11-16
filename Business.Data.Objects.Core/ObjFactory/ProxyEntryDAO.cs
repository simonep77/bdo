using Business.Data.Objects.Core.Schema.Definition;
using System;

namespace Business.Data.Objects.Core.ObjFactory
{
    /// <summary>
    /// Identifica una entry con tutte le informazioni necessarie per
    /// la gestione del nuovo tipo
    /// </summary>
    internal class ProxyEntryDAO
    {
        /// <summary>
        /// Definizione del delegato costruttore
        /// </summary>
        /// <returns></returns>
        public delegate object FastConstructor();

        /// <summary>
        /// Il nuovo tipo generato
        /// </summary>
        public Type ProxyType;

        /// <summary>
        /// Lo schema associato che descrive la mappatura
        /// </summary>
        public ClassSchema ClassSchema;

        /// <summary>
        /// Il costruttore da chiamare per creare un istanza
        /// </summary>
        public FastConstructor Create;
    }
}
