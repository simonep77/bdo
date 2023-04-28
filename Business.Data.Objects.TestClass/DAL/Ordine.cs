using System;
using System.Collections.Generic;
using System.Text;
using Business.Data.Objects.Core;
using Business.Data.Objects.Core.Attributes;
using Business.Data.Objects.Core.Attributes.Structure;

namespace Business.Data.Objects.TestClass.DAL
{
    [Table(@"ordini")]
    [History]
    public abstract class Ordine: DataObject<Ordine>
    {
        [PrimaryKey, AutoIncrement]
        public abstract uint Id { get; }

        public abstract uint AnagraficaId { get; set; }

        [PropertyMap(nameof(AnagraficaId))]
        public abstract Anagrafica Anagrafica { get; }

        [MaxLength(30)]
        public abstract string CodiceOrdine { get; set; }

        public abstract sbyte StatoId { get; set; }

        [PropertyMap(nameof(StatoId))]
        public abstract OrdineStato Stato { get; }

        [AcceptNull]
        public abstract DateTime DataPagamanto { get; set; }

        [AutoInsertTimestamp]
        public abstract DateTime DataInserimento { get; }

        [AutoUpdateTimestamp]
        public abstract DateTime DataAggiornamento { get; }

        //[MaxLength(50), AcceptNull, UserInfo]
        //public abstract string Utente { get; }

        [LogicalDelete]
        public abstract bool Flag_Canc { get; }
    }
}
