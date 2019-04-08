using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects.Base;
using Bdo.Objects;
using Bdo.Common;
using Bdo.Schema.Definition;

namespace Bdo.Utils.BdoOnly
{
    /// <summary>
    /// Gestore Eventi associato allo Slot
    /// </summary>
    public class SlotEventManager: IDisposable
    {
        private SlotEventMainQueuePOST mPostEventQ;
        private SlotEventMainQueuePRE mPreEventQ;

        
        #region PUBLIC

        /// <summary>
        /// Ritorna coda POST
        /// </summary>
        internal SlotEventMainQueuePOST PostEventQ
        {
            get {
                if (this.mPostEventQ == null)
                    this.mPostEventQ = new SlotEventMainQueuePOST();

                return this.mPostEventQ;
            }
        }

        /// <summary>
        /// Coda di PRE
        /// </summary>
        internal SlotEventMainQueuePRE PreEventQ
        {
            get
            {
                if (this.mPreEventQ == null)
                    this.mPreEventQ = new SlotEventMainQueuePRE();

                return this.mPreEventQ;
            }
        }

        /// <summary>
        /// Elimina tutti gli eventi associati
        /// </summary>
        internal void Clear()
        {
            if (this.mPostEventQ != null)
                this.mPostEventQ.Clear();

            if (this.mPreEventQ != null)
                this.mPreEventQ.Clear();
        }

        #region POST EVENT QUEUE

        /// <summary>
        /// Registra evento per qualunque tipo
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="func"></param>
        /// <param name="inputData"></param>
        /// <param name="outputData"></param>
        public void RegisterPostEventHandlerForAny(BusinessSlot.EObjectEvent evt, BusinessSlot.BDEventPostHandler func)
        {
            //Aggiunge alla coda Post con tipo: oggetto
            this.PostEventQ.Add(evt, typeof(object), func);
        }

        /// <summary>
        /// Registra evento per tipo su coda
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="evt"></param>
        /// <param name="func"></param>
        public void RegisterPostEventHandler<T>(BusinessSlot.EObjectEvent evt, BusinessSlot.BDEventPostHandler func)
            where T : DataObject<T>
        {
            //Aggiunge alla coda Post
            this.PostEventQ.Add(evt, typeof(T), func);
        }


        /// <summary>
        /// Esegue la coda di eventi. Attenzione! eccezioni non gestite interrompono la coda di esecuzione
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        internal void RunPostEventHandlerQueue(BusinessSlot.EObjectEvent evt, DataObjectBase value)
        {
            if (value == null)
                return;


            //Esegue coda per tipo oggetto
            //TIP: avendo registrato l'evento con <T> viene utilizzato il riferimento interno della classe non proxy
            //quindi la dobbiamo riprendere
            this.PostEventQ.Run(evt, value.mClassSchema.OriginalType, value);

            //Esegue la coda globale (catchall)
            this.PostEventQ.Run(evt, typeof(object), value);
        }


        #endregion


        #region PRE EVENT QUEUE

        /// <summary>
        /// Registra evento per qualunque tipo
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="func"></param>
        /// <param name="inputData"></param>
        /// <param name="outputData"></param>
        public void RegisterPreEventHandlerForAny(BusinessSlot.EObjectEvent evt, BusinessSlot.BDEventPreHandler func)
        {
            //Aggiunge alla coda Pre con tipo: oggetto
            this.PreEventQ.Add(evt, typeof(object), func);
        }


        /// <summary>
        /// Registra evento per tipo su coda
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="evt"></param>
        /// <param name="func"></param>
        public void RegisterPreEventHandler<T>(BusinessSlot.EObjectEvent evt, BusinessSlot.BDEventPreHandler func) 
            where T : DataObject<T>
        { 
            //Aggiunge alla coda Post
            this.PreEventQ.Add(evt, typeof(T), func);
        }


        /// <summary>
        /// Esegue la coda di eventi. Attenzione! eccezioni non gestite interrompono la coda di esecuzione
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        internal void RunPreEventHandlerQueue(BusinessSlot.EObjectEvent evt, DataObjectBase value, ref bool cancel)
        {
            if (value == null)
                return;


            //Esegue coda per tipo oggetto
            //TIP: avendo registrato l'evento con <T> viene utilizzato il riferimento interno della classe non proxy
            //quindi la dobbiamo riprendere
            this.PreEventQ.Run(evt, value.mClassSchema.OriginalType, value, ref cancel);

            //Esegue la coda globale (catchall)
            this.PreEventQ.Run(evt, typeof(object), value, ref cancel);

        }


        #endregion


        #endregion


        public void Dispose()
        {
            this.Clear();
        }


        public string PrintInfo()
        {
            StringBuilder sbInfo = new StringBuilder();

            //Pre Event
            sbInfo.AppendFormat("Event Queue: PRE");
            sbInfo.AppendLine();
            foreach (var item in this.mPreEventQ)
            {
                sbInfo.AppendFormat(" * Event: {0}", item.Key);
                sbInfo.AppendLine();

                foreach (var itemT in item.Value)
                {
                    string sTypeName = itemT.Value.HandledType.Equals(typeof(object)) ? "ANY" : itemT.Value.HandledType.Name;
                    sbInfo.AppendFormat("    > Type: {0} - N. Handlers: {1}", sTypeName.PadRight(30), itemT.Value.Count);
                    sbInfo.AppendLine();
                }
            }
            sbInfo.AppendLine();

            //Post Event
            sbInfo.AppendFormat("Event Queue: POST");
            sbInfo.AppendLine();
            foreach (var item in this.mPostEventQ)
            {
                sbInfo.AppendFormat(" * Event: {0}", item.Key);
                sbInfo.AppendLine();

                foreach (var itemT in item.Value)
                {
                    string sTypeName = itemT.Value.HandledType.Equals(typeof(object)) ? "ANY" : itemT.Value.HandledType.Name;
                    sbInfo.AppendFormat("    > Type: {0} - N. Handlers: {1}", sTypeName.PadRight(30), itemT.Value.Count);
                    sbInfo.AppendLine();
                }
            }

            return sbInfo.ToString();
        }
    }
}
