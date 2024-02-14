using System;
using System.Collections.Generic;
using System.Text;
using Business.Data.Objects.Common.Utils;

namespace Business.Data.Objects.Common
{
    /// <summary>
    /// Classe lista con pager associato
    /// </summary>
    public class PagedList<T>: List<T>
    {

        public PagedList(): base() { }

        public PagedList(IEnumerable<T> coll): base(coll) { }

        /// <summary>
        /// paginatore
        /// </summary>
        public DataPager Pager { get; set; }

    }
}
