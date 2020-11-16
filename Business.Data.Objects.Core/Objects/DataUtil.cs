using Business.Data.Objects.Core.Base;

namespace Business.Data.Objects.Core
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
