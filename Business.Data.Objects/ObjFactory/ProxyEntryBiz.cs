using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects.Base;

namespace Bdo.ObjFactory
{

    /// <summary>
    /// Classe interna per la gestione dei costruttori degli oggetti business
    /// </summary>
    internal class ProxyEntryBiz
    {
        /// <summary>
        /// Definizione del delegato per la creazione del business object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate BusinessObjectBase FastCreateBizObj(object obj);

        /// <summary>
        /// Chiave univoca del tipo
        /// </summary>
        public long TypeKey;

        /// <summary>
        /// Tipo di riferimento del DAL
        /// </summary>
        public Type DalType;

        /// <summary>
        /// Metodo
        /// </summary>
        public FastCreateBizObj Create;

        /// <summary>
        /// Factory da utilizzare
        /// </summary>
        public IBusinessObjFactory Factory;
    }
}
