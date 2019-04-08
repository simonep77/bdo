using System;
using Bdo.Objects.Base;

namespace Bdo.Objects
{
    /// <summary>
    /// Eccezioni generate da business object
    /// </summary>
    [Serializable]
    public class BusinessObjectException: BdoBaseException 
    {
        /// <summary>
        /// Oggetto Business da cui si e' generata l'eccezione
        /// </summary>
        public BusinessObjectBase BizObj { get; }

        public BusinessObjectException(BusinessObjectBase obj, string msgFormat, params object[] args)
            : base(string.Concat(string.Format(msgFormat, args)))
        {
            this.BizObj = obj;
            var slot = obj.GetSlot();

            //Esegue log debug
            slot.LogDebugException(DebugLevel.Error_3, this);

            //Se presente handler di eccezione lo richiama
            slot.BizObjectExceptionCatch(this);
        }

    }
}
