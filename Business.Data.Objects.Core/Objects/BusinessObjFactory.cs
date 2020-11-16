using Business.Data.Objects.Core.Base;
using System;

namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Classe base per oggetti Business
    /// </summary>
    public abstract class BusinessObjFactory<T>: IBusinessObjFactory
        where T: BusinessObjectBase
    {
        /// <summary>
        /// Espone il tipo base del factory
        /// </summary>
        public Type BizType
        {
            get
            {
                return typeof(T);
            }
        }

        public abstract BusinessObjectBase Create(DataObjectBase dalObj);
        
    }
}
