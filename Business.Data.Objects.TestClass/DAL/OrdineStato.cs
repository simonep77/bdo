using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Attributes;
using Bdo.Objects;

namespace Business.Data.Objects.TestClass.DAL
{
    [Table(@"ordini_stati"), GlobalCache()]
    public abstract class OrdineStato:DataObject<OrdineStato>
    {
        [PrimaryKey, AutoIncrement]
        public abstract uint Id { get; }

        [MaxLength(50)]
        public abstract string Nome { get;}
        

    }
}
