﻿/*--------------------------------------

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
	public class MSSQL2012DataBase: MSSQL2005DataBase  
	{
        /// <summary>
        /// Regex per cercare primo statement di select che puo' avere o meno distinct e top
        /// </summary>
        private static Regex _PAGED_REGEX = new Regex(@"[\s]*(SELECT)[\s]+(?:(DISTINCT)[\s]+)?(?:(TOP[\s]+[\d]+)[\s]+)?", System.Text.RegularExpressions.RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
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
        protected override void preparePagedQuery(int positionIn, int offsetIn)
        {
            //Azzera contatore record
            this.setTotPagedRecords(0);
            //Sostituisce qualunque "SELECT" con "SELECT TOP 10000000000" per consentire gli order BY
            String sTemp = _PAGED_REGEX.Replace(this.SQL, @" $1 $2 TOP 10000000000 ", 1);

 
            //NEW
            System.Text.StringBuilder sb = new System.Text.StringBuilder(400);
            sb.Append("SELECT *, TotRecords = COUNT(*) OVER() ");
            sb.Append("FROM ( ");
            sb.Append(sTemp);
            sb.Append(" ) AS tmpTab ");
            sb.Append("ORDER BY CURRENT_TIMESTAMP ");
            sb.AppendFormat("OFFSET {0} ROWS ", positionIn);
            sb.AppendFormat("FETCH NEXT {0} ROWS ONLY ", offsetIn);


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

                //Ritorna
                return oRetTab;
            }
            finally
            {
                this.EndThreadSafeWork();
            }
        }


	}
}
