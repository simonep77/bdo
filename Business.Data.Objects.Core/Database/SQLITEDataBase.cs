/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;

namespace Business.Data.Objects.Database
{
	/// <summary>
	/// Description of SQLITEDataBase.
	/// 
	/// La connection string deve essere del tipo: DataSource=server;database=db;user=xxx;password=xxx
	/// </summary>
	public class SQLITEDataBase: CommonDataBase 
	{

        protected override string ProviderAssembly { get; } =  @"System.Data.SQLite";
        protected override string ProviderFactoryClass => @"System.Data.SQLite.SQLiteFactory";

        public SQLITEDataBase(string connString): base(connString)
		{
		}
		
		#region "PUBLIC"

        /// <summary>
        /// Ritorna nome funzione per ultimo id inserito
        /// </summary>
        public override string LastAutoIdFunction
        {
            get
            {
                return @"last_insert_rowid()";
            }
        }
			

        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        public override long GetLastAutoId()
        {
            this.SQL = @"SELECT last_insert_rowid()";
            return Convert.ToInt64(this.ExecScalar());
        }

		
		#endregion

        /// <summary>
        /// Pulizia comando - SQLITE ritorna errore se si modifica un commandtext
        /// a datareader aperto
        /// </summary>
        protected override void clearCommand()
        {
            //Pulisce
            this.ClearParameters();
        }


        public override DataTable Select(int positionIn, int offsetIn)
        {
            this.BeginThreadSafeWork();
            try
            {
                this.setTotPagedRecords(0);
                //Manipola Query
                string sQpart;
                System.Text.StringBuilder sb = new System.Text.StringBuilder(500);
                //Aggiunge in query Totale Righe
                int iFrom = this.SQL.ToUpper().IndexOf(@" FROM ");
                //Manipola Query
                sb.Append(this.SQL.Substring(0, iFrom));
                //Scrive query di conteggio record
                sb.Append(@", (SELECT COUNT(*) FROM (SELECT NULL ");
                sQpart = this.SQL.Substring(iFrom);
                sb.Append(sQpart);
                sb.Append(@") TM) AS BdoTotRows ");
                sb.Append(sQpart);
                sb.Append(@" LIMIT ");
                sb.Append(positionIn);
                sb.Append(@",");
                sb.Append(offsetIn);
                //Imposta query
                this.SQL = sb.ToString();
                //Crea tab
                DataTable oRetTab = this.Select();

                //Imposta valore totale righe
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

