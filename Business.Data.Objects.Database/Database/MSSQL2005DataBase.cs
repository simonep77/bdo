/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Business.Data.Objects.Database
{
	/// <summary>
	/// Description of MSSQLDataBase.
	/// </summary>
	public class MSSQL2005DataBase: MSSQLDataBase  
	{
        /// <summary>
        /// Regex per cercare primo statement di selct che puo' avere o meno distinct e top
        /// </summary>
        private static Regex _PAGED_REGEX = new Regex(@"[\s]*(SELECT)[\s]+(?:(DISTINCT)[\s]+)?(?:(TOP[\s]+[\d]+)[\s]+)?", System.Text.RegularExpressions.RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Costruttore base 
        /// </summary>
        /// <param name="connString"></param>
        public MSSQL2005DataBase(string connString)
            : base(connString)
		{
		}

        /// <summary>
        /// Costruttore specifico
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        public MSSQL2005DataBase(DbConnection conn, DbTransaction tran)
            : base(conn.ConnectionString)
        {
            //Carica
            this.InitByADO(conn, tran, SqlClientFactory.Instance);
        }

        /// <summary>
        /// Imposta query paginata
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        protected virtual void preparePagedQuery(int positionIn, int offsetIn)
        {
            //Azzera contatore record
            this.setTotPagedRecords(0);
            //Sostituisce qualunque "SELECT" con "SELECT TOP 10000000000" per consentire gli order BY
            String sTemp = _PAGED_REGEX.Replace(this.SQL, @" $1 $2 TOP 10000000000 ", 1);

 
            //NEW
            System.Text.StringBuilder sb = new System.Text.StringBuilder(400);
            sb.Append("WITH __VarTabName AS ( ");
            sb.Append("SELECT *, ROW_NUMBER() OVER (ORDER BY CURRENT_TIMESTAMP) AS [__RowNum] ");
            sb.Append("FROM ( ");
            sb.Append(sTemp);
            sb.Append(") AS tmpTab ");
            sb.Append(") ");
            sb.Append("SELECT *, (SELECT COUNT(*) FROM __VarTabName) AS TotRecords ");
            sb.Append("FROM __VarTabName ");
            sb.AppendFormat("WHERE [__RowNum] > {0} ", positionIn);
            sb.AppendFormat("AND [__RowNum] <= {0} ", positionIn + offsetIn);
            sb.Append("ORDER BY [__RowNum] ASC ");

            this.SQL = sb.ToString();
        }


        public override string LastAutoIdFunction
        {
            get
            {
                return @"SCOPE_IDENTITY()";
            }
        }


        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        public override long GetLastAutoId()
        {
            this.SQL = @"SELECT SCOPE_IDENTITY()";
            return Convert.ToInt64(this.ExecScalar());
        }


        /// <summary>
        /// Esegue query paginata
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        public override DataTable Select(int positionIn, int offsetIn)
        {
            this.BeginThreadSafeWork();
            try
            {
                //Imposta
                this.preparePagedQuery(positionIn, offsetIn);

                DataTable oRetTab = this.Select();

                //Se presente almento una riga ne cattura l'ultima che rappresenta il totale righe
                if (oRetTab.Rows.Count > 0)
                    this.setTotPagedRecords(Convert.ToInt32(oRetTab.Rows[0][oRetTab.Columns.Count - 1]));

                //Rimuove colonne di servizio per nasconderle
                oRetTab.Columns.RemoveAt(oRetTab.Columns.Count - 1);
                oRetTab.Columns.RemoveAt(oRetTab.Columns.Count - 1);

                //Ritorna
                return oRetTab;
            }
            finally
            {
                this.EndThreadSafeWork();
            }
        }

        /// <summary>
        /// Esegue query paginata con reader
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        public override DbDataReader ExecReaderPaged(int positionIn, int offsetIn)
        {
            this.BeginThreadSafeWork();
            try
            {
                //Imposta
                this.preparePagedQuery(positionIn, offsetIn);

                //Esegue
                return this.ExecReader();
            }
            finally
            {
                this.EndThreadSafeWork();
            }
        }

	}
}
