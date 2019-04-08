using System;
using System.Text;
using System.IO;

namespace Bdo.Logging
{
    /// <summary>
    /// Classe per log su file di testo
    /// </summary>
    public class FileStreamLogger: LoggerBase  
    {
        #region FIELDS

        private string mFilePath;
        private StreamWriter mWriter;

        #endregion 

        #region PUBLIC

        /// <summary>
        /// Crea logger su file fornito in modalità append e apre lo stream
        /// </summary>
        /// <param name="filePath"></param>
        public FileStreamLogger(string filePath)
        {
            this.mFilePath = filePath;
            this.StartLog();
        }

        #endregion

        #region ILogger Membri di

        /// <summary>
        /// Path completo file
        /// </summary>
        public override string LogPath
        {
            get { return this.mFilePath; }
        }

        

        /// <summary>
        /// Scrive Messaggio Log
        /// </summary>
        /// <param name="msgIn"></param>
        /// <param name="args"></param>
        public override void LogMessage(string msgIn, params object[] args)
        {
            this.writeDataToFile(LoggerBase.FormatLogTextMessage(this.DateFormat, this.WriteThreadId, msgIn, args));
        }


        /// <summary>
        /// Scrive informazioni su eccezione
        /// </summary>
        /// <param name="e"></param>
        /// <param name="includeStack"></param>
        public override void LogException(Exception e, bool includeStack)
        {
            this.writeDataToFile(LoggerBase.FormatLogTextException(this.DateFormat, this.WriteThreadId, e, includeStack));
        }


        /// <summary>
        /// Avvio del log
        /// </summary>
        public override void StartLog()
        { }


        /// <summary>
        /// Ferma istanza log
        /// </summary>
        public override void StopLog()
        {
            lock (this.SyncLock)
            {
                if (this.mWriter == null)
                    return;

                this.mWriter.Dispose();

                this.mWriter = null;
            }
        }


        /// <summary>
        /// Scrive su file
        /// </summary>
        /// <param name="data"></param>
        private void writeDataToFile(string data)
        {
            lock (this.SyncLock)
            {
                if (this.mWriter == null)
                    this.mWriter = new StreamWriter(new FileStream(this.mFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));

                this.mWriter.WriteLine(data);
                this.mWriter.Flush();
            }
        }


        #endregion

    }
}
