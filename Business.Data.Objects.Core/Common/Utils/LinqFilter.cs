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

        public Expression<Func<T, bool>> Result { get; private set; }

        /// <summary>
        /// Aggiunge Condizione AND
        /// </summary>
        /// <param name="exp"></param>
        public void And(Expression<Func<T, bool>> exp)
        {
            if (this.Result == null)
                this.Result = exp;

            var tm = this.Result;
            this.Result = LinqExt.AndAlso(tm, exp);
        }

        /// <summary>
        /// Aggiunge condizione OR
        /// </summary>
        /// <param name="exp"></param>
        public void Or(Expression<Func<T, bool>> exp)
        {
            if (this.Result == null)
                this.Result = exp;

            var tm = this.Result;
            this.Result = LinqExt.OrElse(tm, exp);
        }

    }

}