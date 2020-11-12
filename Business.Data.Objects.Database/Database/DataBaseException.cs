/*
 * Creato da SharpDevelop.
 * Utente: spelaia
 * Data: 17/07/2007
 * Ora: 17.11
 * 
 */

using System;

namespace Business.Data.Objects.Database
{
	/// <summary>
	/// Description of DataBaseException.
	/// </summary>
	public class DataBaseException: ApplicationException 
	{
        public DataBaseException(string messageFmt, params object[] args)
            : base(string.Format(messageFmt, args))
		{
		}
	}
}
