using System;
using System.Collections.Generic;
using System.Text;
using Business.Data.Objects.Common.Utils;

namespace Business.Data.Objects.Common
{
    /// <summary>
    /// Classe che ritorna lista di risultati ed informazioni di paginazione associate
    /// </summary>
    public class PageableResult<T>
    {
        /// <summary>
        /// Risultati
        /// </summary>
        public List<T> Result { get; internal set; } = new List<T>();

        /// <summary>
        /// paginatore
        /// </summary>
        public DataPager Pager { get; internal set; } = new DataPager();

    }
}
