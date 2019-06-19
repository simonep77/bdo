using System;
using System.Text;
using System.IO;

namespace Bdo.Logging
{
    /// <summary>
    /// Classe per log su file di testo
    /// </summary>
    public class ConsoleLogger: LoggerBase  
    {
        #region FIELDS

        private bool _Active = true;

        #endregion 


        #region ILogger Membri di




        /// <summary>
        /// Path completo file
        /// </summary>
        public override string LogPath
        {
            get { return @"Console"; }
        }

        

        /// <summary>
        /// Scrive Messaggio Log
        /// </summary>
        /// <param name="msgIn"></param>
        /// <param name="args"></param>
        public override void LogMessage(string msgIn, params object[] args)
        {
            if (!this._Active)
                return;

            Console.WriteLine(LoggerBase.FormatLogTextMessage(this.DateFormat, this.WriteThreadId,msgIn, args));
        }


        /// <summary>
        /// Scrive informazioni su eccezione
        /// </summary>
        /// <param name="e"></param>
        /// <param name="includeStack"></param>
        public override void LogException(Exception e, bool includeStack)
        {
            if (!this._Active)
                return;

            var msg = LoggerBase.FormatLogTextException(this.DateFormat, this.WriteThreadId, e, includeStack);

            Console.Write(msg);
        }

        /// <summary>
        /// Avvia istanza log (la new lo esegue in automatico)
        /// </summary>
        public override void StartLog()
        {
            this._Active = true;
        }

        /// <summary>
        /// Ferma istanza log
        /// </summary>
        public override void StopLog()
        {
            this._Active = false;
        }


        #endregion

    }
}
