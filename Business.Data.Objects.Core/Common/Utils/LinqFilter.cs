using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Casse filtro per combinare dinamicamente statement linq di ricerca
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinqFilter<T>
    {
        private Expression<Func<T, bool>> mResult;

        /// <summary>
        /// Ritorna espressione risultato delle operazioni eseguite.
        /// In caso di nessuna operazione ritorna una generica espressione sempre vera (1=1)
        /// </summary>
        public Expression<Func<T, bool>> Result
        {
            get
            {
                return this.mResult ?? ((t) => 1 == 1);
            }
        }

        /// <summary>
        /// Aggiunge Condizione AND
        /// </summary>
        /// <param name="exp"></param>
        public void And(Expression<Func<T, bool>> exp)
        {
            if (this.mResult == null)
                this.mResult = exp;
            else
                this.mResult = LinqExt.AndAlso(this.mResult, exp);
        }

        /// <summary>
        /// Aggiunge condizione OR
        /// </summary>
        /// <param name="exp"></param>
        public void Or(Expression<Func<T, bool>> exp)
        {
            if (this.mResult == null)
                this.mResult = exp;
            else
                this.mResult = LinqExt.OrElse(this.mResult, exp);
        }

    }

}