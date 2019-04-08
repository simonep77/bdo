using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Database;
using Bdo.Objects;

namespace Bdo.Utils
{
    /// <summary>
    /// Classe per la scrittura di sql a codice
    /// </summary>
    public class SqlSelect
    {
        private StringBuilder _query = new StringBuilder("SELECT ", 200);

        #region PUBLIC

        #region STATEMENT

        /// <summary>
        /// Inizia statement SELECT
        /// </summary>
        /// <returns></returns>
        public SqlSelect Select()
        {
            _query.Append(@"SELECT ");
            return this;
        }

        /// <summary>
        /// Inizia statement UPDATE
        /// </summary>
        /// <returns></returns>
        public SqlSelect Update()
        {
            _query.Append(@"UPDATE ");
            return this;
        }

        /// <summary>
        /// Inizia statement DELETE
        /// </summary>
        /// <returns></returns>
        public SqlSelect Delete()
        {
            _query.Append(@"DELETE ");
            return this;
        }

        #endregion

        #region FIELDS


        /// <summary>
        /// Elenco di campi
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public SqlSelect Field(string field)
        {
            this.makeField(field);

            return this;
        }

        /// <summary>
        /// Un campo
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlSelect Fields(params string[] fields)
        {
            foreach (var item in fields)
                this.makeField(item);

            return this;
        }

        #endregion


        #region "TABLES"


        public SqlSelect From(string table)
        {
            return this.makeTable(@"FROM", table);
        }

        public SqlSelect InnerJoin(string table)
        {
            return this.makeTable(@"INNER JOIN", table);
        }

        public SqlSelect LeftJoin(string table)
        {
            return this.makeTable(@"LEFT JOIN", table);
        }

        public SqlSelect RightJoin(string table)
        {
            return this.makeTable(@"RIGHT JOIN", table);
        }

        #endregion

        #endregion

        public SqlSelect Clear()
        {
            _query.Length = 0;
            _query.Append(@"SELECT ");
            return this;
        }






        #region PRIVATE


        /// <summary>
        /// Elimina virgole finali
        /// </summary>
        private void removeComa()
        {
            var c = _query[_query.Length - 1];
            while (c == ' ' || c == ',')
            {
                _query.Remove(_query.Length - 1, 1);
                c = _query.Length > 0 ? _query[_query.Length - 1] : char.MinValue;
            }
        }

        private SqlSelect makeField(string field)
        {
            _query.Append(@" ");
            _query.Append(field);
            _query.Append(@", ");

            return this;
        }

        private SqlSelect makeTable(string join, string table)
        {
            this.removeComa();
            _query.Append(@" ");
            _query.Append(join);
            _query.Append(@" ");
            _query.Append(table);

            return this;
        }


        private SqlSelect makeStatement(string keyword, string clause)
        {
            this.removeComa();
            _query.Append(@" ");
            _query.Append(keyword);
            _query.Append(@" (");
            _query.Append(clause);
            _query.Append(@") ");
            return this;
        }

        #endregion

        public SqlSelect On(string clause)
        {
            return makeStatement(@"ON", clause);
        }

        public SqlSelect Where(string clause)
        {
            return makeStatement(@"WHERE", clause);
        }

        public SqlSelect And(string clause)
        {
            return makeStatement(@"AND", clause);
        }

        public SqlSelect Or(string clause)
        {
            return makeStatement(@"OR", clause);
        }

        public SqlSelect GroupBy(params string[] fields)
        {
            return makeStatement(@"GROUP BY", string.Join(@",", fields));
        }

        public SqlSelect OrderBy(params string[] fields)
        {
            return makeStatement(@"ORDER BY", string.Join(@",", fields));
        }

        public SqlSelect OrderBy(OrderBy order)
        {
            _query.Append(order.ToString());
            return this;
        }

        public SqlSelect Having(string clause)
        {
            return makeStatement(@"HAVING", clause);
        }

        public override string ToString()
        {
            this.removeComa();
            return this._query.ToString();
        }

    }
}
