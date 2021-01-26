/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : $(DATE) $(TIME)
 --------------------------------------*/

using Business.Data.Objects.Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Business.Data.Objects.Database 
{
	/// <summary>
	/// Interfaccia IDataBase.
	/// Fornisce i metodi e le proprietà per accedere ad un database, a prescindere dal tipo
	/// </summary>
	public interface IDataBase: IDisposable 
	{
        /// <summary>
        /// Nome funzione che ritorna ultimo Id inserito automaticamente
        /// </summary>
        string LastAutoIdFunction { get; }

        /// <summary>
        /// Codice hash relativo alla connessi db (connectionstring)
        /// </summary>
        UInt32 HashCode { get; }

        /// <summary>
        /// Indica se la connessione può essere chiusa quando necessario
        /// </summary>
        bool AutoCloseConnection { get; set; }

		/// <summary>
		/// SQL da eseguire
		/// </summary>
        string SQL { get; set;}

        /// <summary>
        /// Indica il tipo di comando da eseguire
        /// </summary>
        CommandType CommandType { get; set;}

        /// <summary>
        /// Tipo Database
        /// </summary>
        string DbType { get; }

        /// <summary>
        /// Stringa di connessione
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Statistiche di utilizzo
        /// </summary>
        DBStats Stats { get; }

        /// <summary>
        /// Indica se ci si trova in un contesto transazionale
        /// </summary>
        bool IsInTransaction { get; }


        /// <summary>
        /// Abilita il trace dei soli errori
        /// </summary>
        bool TraceOnlyErrors { get; set; }


        /// <summary>
        /// Ritorna o imposta il tempo massimo di attesa esecuzione statement (secondi)
        /// </summary>
        int ExecutionTimeout { get; set; }

        /// <summary>
        /// Mappatore tipi .NET -> tipi DB
        /// </summary>
        DbTypeMapper TypeMapper { get; }

        /// <summary>
        /// Clona Database
        /// </summary>
        IDataBase Clone();

        /// <summary>
        /// Abilita il trace fornendo un logger specifico
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="onlyErrors"></param>
        void EnableTrace(LoggerBase logger, bool onlyErrors);


        /// <summary>
        /// Abilita trace degli statement SQL su file
        /// </summary>
        /// <param name="traceFilePath"></param>
        /// <param name="onlyErrors"></param>
        void EnableTrace(string traceFilePath, bool onlyErrors);


        /// <summary>
        /// Disabilita trace degli statement SQL
        /// </summary>
        void DisableTrace();


        /// <summary>
        /// Ritorna un codice hash rappresentativo della query che si sta per eseguire
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        UInt32 GetCurrentQueryHashCode(int pos, int offset);

        /// <summary>
        /// Ritorna una string rappresentativa della query che si sta per eseguire
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        string GetCurrentQueryHashString(int pos, int offset);

		
		/// <summary>
		/// Numero Totale Record calcolati nell'ultima OpenQuery paginata
		/// </summary>
		int TotRecordQueryPaginata{ get; }
		
        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        long GetLastAutoId();

        /// <summary>
        /// Ritorna un nuovo id ottenuto attraverso un generatore
        /// </summary>
        /// <param name="generatorName"></param>
        /// <returns></returns>
        long GetNewGeneratorId(string generatorName);

		/// <summary>
		/// Inizia una transazione
		/// </summary>
        void BeginTransaction();

        /// <summary>
        /// Inizia una transazione con un dato IsolationLevel
        /// </summary>
        /// <param name="level"></param>
        void BeginTransaction(IsolationLevel level);
		
		
		/// <summary>
		/// Commit Transazione in corso
		/// </summary>
		void CommitTransaction();
		
		
		/// <summary>
		/// Rollback Transazione corrente
		/// </summary>
		void RollbackTransaction();
		
		
		/// <summary>
		/// Esegue statement SQL impostato senza tornare dati
		/// </summary>
		/// <returns></returns>
		int ExecQuery();
		
		
		/// <summary>
		/// Esegue statement SQL tornando solo la prima colonna della prima riga (altrimenti NULL)
		/// </summary>
		/// <returns></returns>
		object ExecScalar();
		
		/// <summary>
		/// Esegue statement SQL tornando un Oggetto DbDataReader
		/// </summary>
		/// <returns></returns>
		DbDataReader ExecReader();
		

        /// <summary>
        /// Esegue statement SQL impostato e ritorna dati su datatable
        /// </summary>
        /// <returns></returns>
        DataTable Select();


        /// <summary>
        /// Esegue statement SQL impostato e ritorna la parte dati richiesta su datatable
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        DataTable Select(int positionIn, int offsetIn);
		

		/// <summary>
        /// Aggiunge paramtro con nome, valore e tipo
		/// </summary>
		/// <returns></returns>
        DbParameter AddParameter(string name, object value);

        /// <summary>
        /// Aggiunge paramtro con nome, valore e tipo .NET
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        DbParameter AddParameter(string name, object value, Type type);

        /// <summary>
        /// Aggiunge paramtro con nome, valore e tipo DB (driver)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        DbParameter AddParameter(string name, object value, DbType dbtype);

        /// <summary>
        /// Aggiunge parametro creato in precedenza
        /// </summary>
        /// <param name="parameter"></param>
        void AddParameter(DbParameter parameter);


        /// <summary>
        /// Aggiunge lista di parametri
        /// </summary>
        /// <param name="paramlist"></param>
        void AddParameters(IList<DbParameter> paramlist);

        /// <summary>
        /// Elimina tutti i parametri correnti
        /// </summary>
        void ClearParameters();


        /// <summary>
        /// Crea paramtro con nome, valore e tipo db senza aggiungerlo
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        DbParameter CreateParameter(string name, object value, Type type);

        /// <summary>
        /// Crea paramtro con nome, valore e tipo db (driver) senza aggiungerlo
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        DbParameter CreateParameter(string name, object value, DbType dbtype);

        /// <summary>
        /// Ritorna nome parametro compatibile con il database specificato.
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        string CreateParamName(string baseName);
		
		
		/// <summary>
		/// Forza la chiusura della connessione
		/// </summary>
		/// <param name="rollbackUnCommitted">
		/// Specifica se bisogna eseguire il RollBack (oppure il Commit) della transazione eventualmente aperta
		/// </param>
		void CloseConnection(bool rollbackUnCommitted);


        /// <summary>
        /// Forza apertura connessione
        /// </summary>
        void OpenConnection();
		
		
		/// <summary>
		/// Inizia una sessione thread-safe di lavoro
		/// </summary>
		void BeginThreadSafeWork();
		
		
		
		/// <summary>
        /// Termina una sessione thread-safe di lavoro
		/// </summary>
        void EndThreadSafeWork();


        /// <summary>
        /// Ottiene un lock globale a livello di applicazione
        /// per una risorsa identificata da un nome
        /// </summary>
        /// <param name="lockName"></param>
        /// <param name="timeoutsec"></param>
        void GetLock(string lockName, int timeoutsec);


        /// <summary>
        /// Rilascia un lock globale precedentemente
        /// acquisito
        /// </summary>
        /// <param name="lockName"></param>
        void ReleaseLock(string lockName);


        /// <summary>
        /// Indica se presente un lock con nome
        /// </summary>
        /// <param name="lockName"></param>
        /// <returns></returns>
        bool HasLock(string lockName);

        /// <summary>
        /// Resetta lo stato di esecuzione del database
        /// In particolare elimina eventuali parametri
        /// mai utilizzati
        /// </summary>
        void Reset();


        /// <summary>
        /// Ritorna connessione sottostante
        /// </summary>
        /// <returns></returns>
        DbConnection GetAdoConnection();

        /// <summary>
        /// Ritorna transazione sottostante
        /// </summary>
        /// <returns></returns>
        DbTransaction GetAdoTransaction();


        #region Locked Transactions

        /// <summary>
        /// Richiede lock ed avvia transazione
        /// </summary>
        /// <param name="lockName"></param>
        /// <param name="timeOutSec"></param>
        void LockedTransactionBegin(string lockName, int timeOutSec);

        /// <summary>
        ///  Committa e rilascia il lock. In caso di eccezione della transazione rilascia comunque il lock e propaga l'eccezione
        /// </summary>
        /// <param name="lockName"></param>
        void LockedTransactionCommit(string lockName);

        /// <summary>
        ///  Rollback e rilascia il lock. In caso di eccezione della transazione rilascia comunque il lock e propaga l'eccezione
        /// </summary>
        /// <param name="lockName"></param>
        void LockedTransactionRollback(string lockName);

        #endregion
    }
}
