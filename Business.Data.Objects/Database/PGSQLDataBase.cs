/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Bdo.Database 
{
	/// <summary>
	/// Classe di gestione Database PostgrsSQL - PGSQLDataBase.
	/// 
	/// La connection string deve essere del tipo: DataSource=server;database=db;user=xxx;password=xxx
	/// </summary>
	public class PGSQLDataBase: CommonDataBase 
	{
        private readonly string PARAM_CHAR = @"";
        private readonly string PARAM_REPLACE = @":$2";
        private readonly string PARAM_REGEX = @"(\B[@])([\w]*\b)";

        #region "PROPERTY"

        /// <summary>
        /// Eseguito override per convertire il nome parametro @ con ?
        /// </summary>
        public override string SQL
        {
            get {
                return base.SQL;
            }
            set {
                value = Regex.Replace(value, PARAM_REGEX, PARAM_REPLACE, RegexOptions.Compiled);
                base.SQL = value;
            }
        }

        #endregion

        public PGSQLDataBase(string connString): base(connString)
		{
			const string nomeAssembly = "Npgsql";
            const string tipoFactory = "Npgsql.NpgsqlFactory";
			
			//Carica
			this.LoadAssemblyAndInitByFactory(nomeAssembly, tipoFactory);
        }



        /// <summary>
        /// Eseguito override per convertire il nome parametro @ con ?
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public override void AddParameter(string name, object value)
        {
            base.AddParameter(name.Replace(@"@", PARAM_CHAR), value);
        }
		
		#region "PUBLIC"
		
	
        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        public override long GetLastAutoId()
        {
            this.SQL = @"SELECT lastval()";
            return Convert.ToInt64(this.ExecScalar());
        }


        /// <summary>
        /// Ritorna un nuovo id ottenuto attraverso un generatore
        /// </summary>
        /// <param name="generatorName"></param>
        /// <returns></returns>
        public override long  GetNewGeneratorId(string generatorName)
        {
            this.SQL = string.Concat(@"SELECT nextval('", generatorName, "')");
            return Convert.ToInt64(this.ExecScalar());
        }


        ///// <summary>
        ///// Esegue una query e ritorna parte del risultato su dataset come da specifiche
        ///// </summary>
        ///// <param name="positionIn"></param>
        ///// <param name="offsetIn"></param>
        ///// <returns></returns>
        //public override DataTable Select(int positionIn, int offsetIn)
        //{
        //    Exception exLog = null;

        //    lock (this._GlobalLock)
        //    {
        //        try
        //        {
        //            this._TotRecordQueryPaginata = 0;
        //            //Salva Query Base
        //            string qBase = this._command.CommandText;
        //            //Aggiusta query con LIMIT, OFFSET
        //            this._command.CommandText = string.Format("{0} LIMIT {1} OFFSET {2}", this._command.CommandText, positionIn, offsetIn);
        //            //Crea tab output
        //            DataTable oRetTab = new DataTable(TABLE_NAME);
        //            using (DbDataAdapter dA = this._FactoryCorrente.CreateDataAdapter())
        //            {
        //                //imposta adapter
        //                dA.SelectCommand = this._command;

        //                //esegue query e riempie
        //                dA.Fill(oRetTab);
        //            }
        //            this.Stats.IncNumeroSelect();
        //            //quindi esegue query per numero record:
        //            // -- cerca ultima clausola ORDER BY
        //            int orderPos = qBase.ToUpper().LastIndexOf(" ORDER ");
        //            // -- esegue parsing
        //            this._command.CommandText = string.Format("SELECT COUNT(*) FROM ( {0} ) AS COUNTTAB", qBase.Substring(0, (orderPos != -1) ? orderPos : qBase.Length));
        //            // -- esegue query
        //            this._TotRecordQueryPaginata = Convert.ToInt32(this._command.ExecuteScalar());
        //            //Stats
        //            this.Stats.IncNumeroSelect();
        //            //Ritorna
        //            return oRetTab;

        //        }
        //        catch (Exception ex)
        //        {
        //            exLog = ex;
        //            throw;
        //        }
        //        finally
        //        {
        //            this.TraceStatement("SelectPag", exLog);
        //            //Resetta comando
        //            this.clearCommand();
        //        }
        //    }
        //}

		
		#endregion
		
	}
}

