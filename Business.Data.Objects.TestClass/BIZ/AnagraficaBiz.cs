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

        }

        public OrdineLista ListaOrdini => this.GetLazy<OrdineLista>("OrdLst", ()=> this.Slot.CreateList<OrdineLista>().SearchByLinq(x => x.AnagraficaId == this.DataObj.Id));

    }
}
