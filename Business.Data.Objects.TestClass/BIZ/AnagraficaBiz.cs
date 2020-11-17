using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business.Data.Objects.Core;
using Business.Data.Objects.Core.Attributes;
using Business.Data.Objects.TestClass.DAL;

namespace Business.Data.Objects.TestClass.BIZ
{
    public class AnagraficaBiz : BusinessObject<Anagrafica>
    {

        public AnagraficaBiz(Anagrafica obj)
            :base(obj)
        {
            this.ListaOrdini = new Lazy<OrdineLista>(() =>
            {
                return this.Slot.CreateList<OrdineLista>().SearchByColumn(new FilterEQUAL(nameof(Ordine.AnagraficaId), this.DataObj.Id));
            });

            //this.ListaOrdini = new Lazy<OrdineLista>(() => this.Slot.CreateList<OrdineLista>().SearchByColumn(new FilterEQUAL(nameof(Ordine.AnagraficaId), this.DataObj.Id)));

        }

        public Lazy<OrdineLista> ListaOrdini { get; }

    }
}
