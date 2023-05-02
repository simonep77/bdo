using Business.Data.Objects.Core.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Objects
{
    /// <summary>
    /// Classe che rappresenta una voce di storico da inviare al gestore
    /// </summary>
    public class HistoryEntry
    {
        /// <summary>
        /// Identificativo dello slot che ha registrato la modifica
        /// </summary>
        public string SlotId { get; set; }

        /// <summary>
        /// Operazione esguita
        /// </summary>
        public HistoryOpType Operation { get; set; }

        /// <summary>
        /// Indicazione principale dell'oggetto di history
        /// </summary>
        public HistoryRef PrimaryRef { get; set; }

        /// <summary>
        /// Chiave dell'entità principale del dato
        /// </summary>
        public HistoryRef TopLevelEntityRef { get; set; }


        /// <summary>
        /// Collegamento ad entità principale collegata
        /// </summary>
        public HistoryRef ForeignRef { get; set;}

        /// <summary>
        /// Oggetto Movimentato
        /// </summary>
        public DataObjectBase DataObject { get; set; }

        /// <summary>
        /// Oggetto Movimentato in notazione dictionary con le sole proprietà "semplici" e senza dati di servizio
        /// </summary>
        public Dictionary<string, object> SlimObject { get; set; }

        public DateTime TimeStamp { get; set; }
    }


    /// <summary>
    /// Entity di riferimento
    /// </summary>
    public class HistoryRef
    {
        public Type Type { get; set;}

        public string Value { get; set;}
    }

    /// <summary>
    /// Tipo Operazione
    /// </summary>
    public enum HistoryOpType : short
    {
        Insert = 1,
        Update = 2,
        Delete = 3,
    }


}
