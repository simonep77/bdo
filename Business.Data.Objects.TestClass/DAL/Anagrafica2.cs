using System;
using System.Collections.Generic;
using System.Text;
using Business.Data.Objects.Core;
using Business.Data.Objects.Core.Attributes;

namespace Business.Data.Objects.TestClass.DAL
{
    [Table(@"anagrafica")]
    public class Anagrafica2:DataObject<Anagrafica2>
    {
        public const string KEY_CF = @"KEY_CF";

        [PrimaryKey, AutoIncrement]
        public virtual uint Id { get; }

        [MaxLength(50), Trim()]
        public virtual string Nome { get; set; }

        [MaxLength(50)]
        public virtual string Cognome { get; set; }

        [MaxLength(20), SearchKey(KEY_CF), CustomDbType(System.Data.DbType.AnsiString)]
        public virtual string CodiceFiscale { get; set; }

        public virtual DateTime DataNascita { get; set; }

        [MaxLength(50)]
        public virtual string LuogoNascita { get; set; }

        [DefaultValue(@"1")]
        public virtual sbyte Attivo { get; set; }

        [MaxLength(150)]
        public virtual string Email { get; set; }

        [AutoInsertTimestamp]
        public virtual DateTime DataInserimento { get; }

        [AutoUpdateTimestamp]
        public virtual DateTime DataAggiornamento { get; }

    }
}
