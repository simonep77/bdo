using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects.Base;

namespace Bdo.Objects
{
    /// <summary>
    /// Classe astratta per la definizione dei metodi di
    /// business per una generica classe
    /// </summary>
    public abstract class DataUtil: SlotAwareObject
    {

        #region PROPRIETA'


        #endregion

        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="slotIn"></param>
        public DataUtil(BusinessSlot slotIn)
        {
            //Imposta oggetto
            this.SetSlot(slotIn);
        }
    }
}
