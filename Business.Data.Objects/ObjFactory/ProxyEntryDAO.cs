using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Schema.Definition;

namespace Bdo.ObjFactory
{
    /// <summary>
    /// Identifica una entry con tutte le informazioni necessarie per
    /// la gestione del nuovo tipo
    /// </summary>
    internal class ProxyEntryDao
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
