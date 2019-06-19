using System;
using System.Threading;
using System.Text;
using System.IO;

namespace Bdo.Logging
{
    /// <summary>
    /// Classe per log su file di testo
    /// </summary>
    public class AsyncFileLogger: LoggerBase  
    {
        #region FIELDS
        private const int I_LOG_BASE_SIZE = 4096;

        private string mFilePath;
        private volatile bool mWriteStopped;
        private Thread mWriterThd;
        private ManualResetEvent mWriteStopEvent;
        private StringBuilder mCurrentLog = new StringBuilder(I_LOG_BASE_SIZE);
        private int mWriteIntervalMsec = 10000;

        #endregion 

        #region PUBLIC

        /// <summary>
        /// Path completo file
        /// </summary>
        public override string LogPath
        {
            get { return this.mFilePath; }
        }

        /// <summary>
        /// Millisecondi dopo i quali viene effettuata la scrittura
        /// </summary>
        public int WriteIntervalMsec
        {
            get { return this.mWriteIntervalMsec; }
            set { this.mWriteIntervalMsec = (value > 0) ? value : 15000; }
        }


        /// <summary>
        /// Crea logger su file fornito in modalità append e apre lo stream
        /// </summary>
        /// <param name="filePath"></param>
        public AsyncFileLogger(string filePath)
        {
            this.mFilePath = filePath;
            this.mWriteStopEvent = new ManualResetEvent(false);
            
            //Avvia
            this.StartLog();
        }

        #endregion

        #region ILogger Membri di

        /// <summary>
        /// Avvio del log
        /// </summary>
        public override void StartLog()
        {
            //Reimposta stato avvio
            this.mWriteStopped = false;
            this.mWriteStopEvent.Reset();

            //Crea nuovo thread
            this.mWriterThd = new Thread(new ThreadStart(asyncWriteRun));

            //Avvia thread asincrono
            this.mWriterThd.Start();
        }


        /// <summary>
        /// Ferma istanza log
        /// </summary>
        public override void StopLog()
        {
            //imposta flag di chiusura thread
            this.mWriteStopped = true;

            //Se il thread e' attivo e' necessario attendere fine e chiuderlo
            if (this.mWriterThd != null && this.mWriterThd.IsAlive)
            {
                //Sblocca eventualmente il thread
                this.mWriteStopEvent.Set();
                //attende fine thread
                this.mWriterThd.Join();
                //lo annulla
                this.mWriterThd = null;
            }

            //salva eventualo dati rimasti
            this.saveLogData();
        }

        /// <summary>
        /// Scrive Messaggio Log
        /// </summary>
        /// <param name="msgIn"></param>
        /// <param name="args"></param>
        public override void LogMessage(string msgIn, params object[] args)
        {
            //string s = LoggerBase.FormatLogTextMessage(msgIn, args, this.DateFormat, this.WriteThreadId);
            string s = LoggerBase.FormatLogTextMessage(this.DateFormat, this.WriteThreadId, msgIn, args);
            this.appendLogData(ref s);
        }


        /// <summary>
        /// Scrive informazioni su eccezione
        /// </summary>
        /// <param name="e"></param>
        /// <param name="includeStack"></param>
        public override void LogException(Exception e, bool includeStack)
        {
            //this.logExceptionBase(this.mWriter, e, includeStack);
            string s = LoggerBase.FormatLogTextException(this.DateFormat, this.WriteThreadId, e, includeStack);
            this.appendLogData(ref s);
        }

        #endregion

        #region PRIVATE

        /// <summary>
        /// Esegue l'accodamento del log 
        /// </summary>
        /// <param name="logmsg"></param>
        private void appendLogData(ref string logmsg)
        {
            if (this.mWriteStopped)
                throw new ApplicationException(@"Il log su cui si vuole scrivere e' in fase di chiusura. Non e' possibile aggiungere ulteriori messaggi.");

            //Scrive
            lock (this.SyncLock)
            {
                this.mCurrentLog.AppendLine(logmsg);
            }

        }

        /// <summary>
        /// Esegue salvataggio del log
        /// </summary>
        private void saveLogData()
        {
            StringBuilder sbLog;

            //lock + Copia log e switch
            lock (this.SyncLock)
            {
                sbLog = this.mCurrentLog;
                this.mCurrentLog = new StringBuilder(I_LOG_BASE_SIZE);
            }

            //Scrive i dati su file
            if (sbLog.Length > 0)
            {
                //verifica apertura stream
                using (StreamWriter sw = new StreamWriter(new FileStream(this.mFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                {
                    //scrive
                    sw.Write(sbLog.ToString());
                    sw.Flush();
                }
                sbLog.Length = 0;
            }
        }

        /// <summary>
        /// Esecuzione scrittura asincrona
        /// </summary>
        private void asyncWriteRun()
        {
            while (!this.mWriteStopped)
            {
                //Ogni xx secondi verifica se necessario scrivere
                if (this.mWriteStopEvent.WaitOne(mWriteIntervalMsec))
                    break;

                //Esegue salvataggio
                this.saveLogData();
            }
        }

        #endregion


    }
}
