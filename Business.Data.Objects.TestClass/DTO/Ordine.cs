using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.TestClass.DTO
{
    public class OrdineDTO
    {
        public uint Id { get; set; }

        public uint AnagraficaId { get; set; }


        public string CodiceOrdine { get; set; }

        public byte StatoId { get; set; }

        public DateTime DataPagamanto { get; set; }

        public DateTime DataInserimento { get; set; }

        public DateTime DataAggiornamento { get; set; }

        public string Utente { get; set; }

        public sbyte Cancellato { get; set; }
    }
}
