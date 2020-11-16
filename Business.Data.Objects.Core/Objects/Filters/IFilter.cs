using System;
using System.Collections.Generic;
using System.Text;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Database;

namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Interfaccia filtro
    /// </summary>
    public interface IFilter:IEnumerable<IFilter>
    {
        /// <summary>
        /// Filter Field Name
        /// </summary>
        string Name{ get; }

        /// <summary>
        /// Filter Inner operator
        /// </summary>
        EOperator Operator { get; }

        /// <summary>
        /// Valore RAW del filtro
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Operatore AND
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        IFilter And(IFilter f);

        IFilter And(string name, EOperator op, object value);


        /// <summary>
        /// Operatore OR
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        IFilter Or(IFilter f);

        IFilter Or(string name, EOperator op, object value);

        /// <summary>
        /// Testa la proprieta' di un oggetto
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool PropertyTest(DataObjectBase obj);


        /// <summary>
        /// Appende la traduzione SQL del filtro
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="paramIndex"></param>
        void AppendFilterSql(IDataBase db, StringBuilder sql, int paramIndex);


    }
}
