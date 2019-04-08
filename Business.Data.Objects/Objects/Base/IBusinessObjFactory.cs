using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects.Base
{
    /// <summary>
    /// Interfaccia per la definizione di factory personalizzati di BusinessObjects
    /// </summary>
    public interface IBusinessObjFactory
    {

        /// <summary>
        /// Crea business object a partire da un dal
        /// </summary>
        /// <param name="dalObj"></param>
        /// <returns></returns>
        BusinessObjectBase Create(DataObjectBase dalObj);

    }
}
