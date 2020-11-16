using Business.Data.Objects.Core;
using Business.Data.Objects.Core.Base;
using System;
using System.Collections.Generic;

namespace Business.Data.Objects.Cores.Utils.BdoOnly
{

    /// <summary>
    /// Coda per la gestione degli eventi POST
    /// </summary>
    internal class SlotEventSimpleQueuePOST : List<BusinessSlot.BDEventPostHandler>
    {
        /// <summary>
        /// Tipo gestito
        /// </summary>
        internal Type HandledType;

        /// <summary>
        /// Esegue tutti gli eventi
        /// </summary>
        /// <param name="value"></param>
        internal void Run(DataObjectBase value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i](value);
            }
        }

    }


    /// <summary>
    /// Coda eventi  per tipo oggetto
    /// </summary>
    internal class SlotEventForTypeQueuePOST : Dictionary<long, SlotEventSimpleQueuePOST>
    {
        /// <summary>
        /// Aggiunge Item
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        internal void Add(Type t, BusinessSlot.BDEventPostHandler handler)
        {
            SlotEventSimpleQueuePOST q = null;
            long lKey = t.TypeHandle.Value.ToInt64();

            if (!this.TryGetValue(lKey, out q))
            {
                q = new SlotEventSimpleQueuePOST();
                q.HandledType = t;
                this.Add(lKey, q);
            }

            q.Add(handler);
        }


        /// <summary>
        /// Esegue la coda
        /// </summary>
        /// <param name="value"></param>
        internal void Run(Type t, DataObjectBase value)
        {
            if (value == null)
                return;

            SlotEventSimpleQueuePOST q = null;

            //Se non trovato nulla esce
            if (!this.TryGetValue(t.TypeHandle.Value.ToInt64(), out q))
                return;

            q.Run(value);
        }

    }


    /// <summary>
    /// Gestore coda per tipo evento
    /// </summary>
    internal class SlotEventMainQueuePOST : Dictionary<BusinessSlot.EObjectEvent, SlotEventForTypeQueuePOST>
    {
        /// <summary>
        /// Aggiunge Item
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        internal void Add(BusinessSlot.EObjectEvent evt, Type t, BusinessSlot.BDEventPostHandler handler)
        {
            SlotEventForTypeQueuePOST q = null;

            if (!this.TryGetValue(evt, out q))
            {
                q = new SlotEventForTypeQueuePOST();
                this.Add(evt, q);
            }

            q.Add(t, handler);
        }

        /// <summary>
        /// Esegue coda per tipo evento
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        internal void Run(BusinessSlot.EObjectEvent evt, Type t, DataObjectBase value)
        {
            SlotEventForTypeQueuePOST q = null;

            if (!this.TryGetValue(evt, out q))
                return;

            q.Run(t, value);
        }

    }
}
