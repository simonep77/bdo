using Business.Data.Objects.Core.Base;
using System;
using System.Collections.Generic;

namespace Business.Data.Objects.Core.Utils.BdoOnly
{

    /// <summary>
    /// Coda per la gestione degli eventi PRE
    /// </summary>
    internal class SlotEventSimpleQueuePRE : List<BusinessSlot.BDEventPreHandler>
    {
        /// <summary>
        /// Tipo Gestito
        /// </summary>
        internal Type HandledType;

        /// <summary>
        /// Esegue tutti gli eventi
        /// </summary>
        /// <param name="value"></param>
        internal void Run(DataObjectBase value, ref bool cancel)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i](value, ref cancel);
            }
        }

    }


    /// <summary>
    /// Coda eventi  per tipo oggetto
    /// </summary>
    internal class SlotEventForTypeQueuePRE: Dictionary<long, SlotEventSimpleQueuePRE>
    {
        /// <summary>
        /// Aggiunge Item
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        internal void Add(Type t, BusinessSlot.BDEventPreHandler handler)
        {
            SlotEventSimpleQueuePRE q = null;
            long lKey = t.TypeHandle.Value.ToInt64();

            if (!this.TryGetValue(lKey, out q))
            {
                q = new SlotEventSimpleQueuePRE();
                q.HandledType = t;
                this.Add(lKey, q);
            }

            q.Add(handler);
        }


        /// <summary>
        /// Esegue la coda
        /// </summary>
        /// <param name="value"></param>
        internal void Run(Type t, DataObjectBase value, ref bool cancel)
        {
            if (value == null)
                return;

            SlotEventSimpleQueuePRE q = null;
            
            //Se non trovato esce
            if (!this.TryGetValue(t.TypeHandle.Value.ToInt64(), out q))
                return;

            q.Run(value, ref cancel);
        }

    }


    /// <summary>
    /// Gestore coda per tipo evento
    /// </summary>
    internal class SlotEventMainQueuePRE : Dictionary<BusinessSlot.EObjectEvent, SlotEventForTypeQueuePRE>
    {
        /// <summary>
        /// Aggiunge Item
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        internal void Add(BusinessSlot.EObjectEvent evt, Type t, BusinessSlot.BDEventPreHandler handler)
        {
            SlotEventForTypeQueuePRE q = null;

            if (!this.TryGetValue(evt, out q))
            {
                q = new SlotEventForTypeQueuePRE();
                this.Add(evt, q);
            }

            q.Add(t, handler);
        }

        /// <summary>
        /// Esegue coda per tipo evento
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        internal void Run(BusinessSlot.EObjectEvent evt, Type t, DataObjectBase value, ref bool cancel)
        {
            SlotEventForTypeQueuePRE q = null;

            if (!this.TryGetValue(evt, out q))
                return;

            q.Run(t, value, ref cancel);
        }

    }
}
