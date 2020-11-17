using System;
using System.Collections.Generic;
using System.Text;
using Business.Data.Objects.Core;
using Business.Data.Objects.Core.Attributes;

namespace Business.Data.Objects.TestClass.DAL
{
    [Table(@"anagrafica")]
    public abstract class Anagrafica:DataObject<Anagrafica>
    {
        [PrimaryKey, AutoIncrement]
        public abstract uint Id { get; }

        [MaxLength(50), Trim()]
        public abstract string Nome { get; set; }

        [MaxLength(50)]
        public abstract string Cognome { get; set; }

        [MaxLength(20)]
        public abstract string CodiceFiscale { get; set; }

        public abstract DateTime DataNascita { get; set; }

        [MaxLength(50)]
        public abstract string LuogoNascita { get; set; }

        [DefaultValue(@"1")]
        public abstract sbyte Attivo { get; set; }

        [MaxLength(150)]
        public abstract string Email { get; set; }

        [AutoInsertTimestamp]
        public abstract DateTime DataInserimento { get; }

        [AutoUpdateTimestamp]
        public abstract DateTime DataAggiornamento { get; }

    }
}
