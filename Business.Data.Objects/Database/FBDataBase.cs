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
	/// Description of FBDataBase.
	/// 
	/// La connection string deve essere del tipo: DataSource=server;database=db;user=xxx;password=xxx
	/// </summary>
	public class FBDataBase: CommonDataBase 
	{
		
		public FBDataBase(string connString):base(connString)
		{
			const string nomeAssembly = "FirebirdSql.Data.FirebirdClient";
			const string tipoFactory = "FirebirdSql.Data.FirebirdClient.FirebirdClientFactory";
				
			base.LoadAssemblyAndInitByFactory(nomeAssembly, tipoFactory);
		}
		
		#region "PUBLIC"
		
        /// <summary>
        /// Ritorna un nuovo id ottenuto attraverso un generatore
        /// </summary>
        /// <param name="generatorName"></param>
        /// <returns></returns>
        public override long  GetNewGeneratorId(string generatorName)
        {
            this.SQL = string.Concat(@"SELECT NEXT VALUE FOR ", generatorName, " FROM RDB$DATABASE");
            return Convert.ToInt64(this.ExecScalar());
        }
        
		#endregion
		
	}
}
