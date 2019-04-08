using System;
using System.Text;
using System.IO;

namespace Bdo.Logging
{
    /// <summary>
    /// Classe per log su file di testo
    /// </summary>
    public class FileLogger: LoggerBase  
    {
        #region FIELDS

        private string mFilePath;

        #endregion 

        #region FIELDS

        /// <summary>
        /// Crea logger su file fornito in modalità append fornita e attivo specificato
        /// </summary>
        /// <param name="filePath"></param>
        public FileLogger(string filePath)
        {
            this.mFilePath = filePath;
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
            lock (this.SyncLock)
            {
                //scrive
                using (StreamWriter oStream = File.AppendText(this.mFilePath))
                {
                    oStream.WriteLine(LoggerBase.FormatLogTextMessage(msgIn, args, this.DateFormat, this.WriteThreadId));
                    oStream.Flush();
                }
            }
            
        }


        /// <summary>
        /// Scrive informazioni su eccezione
        /// </summary>
        /// <param name="e"></param>
        /// <param name="includeStack"></param>
        public override void LogException(Exception e, bool includeStack)
        {
            lock (this.SyncLock)
            {
                //scrive
                using (StreamWriter oStream = File.AppendText(this.mFilePath))
                {
                    oStream.Write(LoggerBase.FormatLogTextException(this.DateFormat, this.WriteThreadId, e, includeStack));
                    oStream.Flush();
                }
            }
        }

        /// <summary>
        /// Avvia istanza log (la new lo esegue in automatico)
        /// </summary>
        public override void StartLog()
        {
            //Non fa nulla
        }

        /// <summary>
        /// Ferma istanza log
        /// </summary>
        public override void StopLog()
        {
            //non Fa nulla
        }


        #endregion

    }
}
