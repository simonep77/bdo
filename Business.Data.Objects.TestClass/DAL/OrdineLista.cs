using System;
using System.Collections.Generic;
using System.Text;
using Business.Data.Objects.Core;
using Business.Data.Objects.Core.Attributes;

namespace Business.Data.Objects.TestClass.DAL
{
    public abstract class OrdineLista : DataList<OrdineLista, Ordine>
    {

        public OrdineLista CercaProva()
        {
            this.Slot.DB.SQL = "SELECT o.Id, a.Cognome, a.nome FROM ordini o INNER JOIN anagrafica a ON a.Id=o.AnagraficaId WHERE a.Cognome like 'a%' ";

            return this.DoSearch();
        }

    }
}
