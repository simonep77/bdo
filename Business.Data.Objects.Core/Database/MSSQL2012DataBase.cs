/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace Business.Data.Objects.Database
{
    /// <summary>
    /// Description of MSSQLDataBase.
    /// </summary>
    public class MSSQL2012DataBase : MSSQL2005DataBase
    {
        /// <summary>
        /// Regex per cercare primo statement di select che puo' avere o meno distinct e top
        /// </summary>
        private static Regex _PAGED_REGEX_ORDER = new Regex(@"order[\s]+by[\s]+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static Regex _PAGED_REGEX_SELECT = new Regex(@"(select[\s]+?)([\S\s]+?)(from[\s]+)(:?[\S\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// Costruttore base 
        /// </summary>
        /// <param name="connString"></param>
        public MSSQL2012DataBase(string connString)
            : base(connString)
        {
        }

        /// <summary>
        /// Costruttore specifico
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        public MSSQL2012DataBase(DbConnection conn, DbTransaction tran)
            : base(conn, tran)
        {

        }

        /// <summary>
        /// Imposta query paginata
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        protected override void preparePagedQuery(int positionIn, int offsetIn)
        {
            //Azzera contatore record
            this.setTotPagedRecords(0);

            var sb = new StringBuilder(_PAGED_REGEX_SELECT.Replace(this.SQL, @"$1$2, COUNT(1) OVER() AS ROW_COUNT $3$4", 1));

            if (!_PAGED_REGEX_ORDER.IsMatch(this.SQL))
                sb.Append(" ORDER BY CURRENT_TIMESTAMP ");

            sb.Append($" OFFSET {positionIn} ROWS ");
            sb.Append($" FETCH NEXT {offsetIn} ROWS ONLY ");

            this.SQL = sb.ToString();
        }

        /// <summary>
        /// Esegue query paginata
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        public override DataTable Select(int positionIn, int offsetIn)
        {
            //Imposta
            this.preparePagedQuery(positionIn, offsetIn);

            DataTable oRetTab = this.Select();

            //Se presente almento una riga ne cattura l'ultima che rappresenta il totale righe
            if (oRetTab.Rows.Count > 0)
                this.setTotPagedRecords(Convert.ToInt32(oRetTab.Rows[0][oRetTab.Columns.Count - 1]));

            //Rimuove colonne di servizio per nasconderle
            oRetTab.Columns.RemoveAt(oRetTab.Columns.Count - 1);

            //Ritorna
            return oRetTab;
        }


    }
}
