/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;

namespace Bdo.Database  
{
	/// <summary>
	/// Description of SQLITEDataBase.
	/// 
	/// La connection string deve essere del tipo: DataSource=server;database=db;user=xxx;password=xxx
	/// </summary>
	public class ACCESSDataBase: CommonDataBase 
	{
		
		
		public ACCESSDataBase(string connString):base(connString)
		{
            //Inizializza da factory
            this.InitByFactory(System.Data.OleDb.OleDbFactory.Instance);
		}
		
		#region "PUBLIC"


        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        public override long GetLastAutoId()
        {
            this.SQL = @"SELECT @@IDENTITY";
            return Convert.ToInt64(this.ExecScalar());
        }

		#endregion
		
	}
}

