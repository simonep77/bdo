using System;
using System.Text;
using System.IO;
using Bdo.Common;

namespace Bdo.Logging
{
    /// <summary>
    /// Classe per log su file di testo
    /// </summary>
    public class CustomLogger: LoggerBase  
    {
        #region FIELDS

        private bool _Active = true;
        private LogText _FnLog;
        public delegate void LogText(string message);

        #endregion 


        #region ILogger Membri di

        public CustomLogger(LogText fnLogText)
        {
            this._FnLog = fnLogText;
        }


        /// <summary>
        /// Path completo file
        /// </summary>
        public override string LogPath
        {
            get { return @"Custom"; }
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

            this._FnLog(string.Format(msgIn, args));
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

            //Scrive
            Exception oException = e;
            int iIndentEx = 0;
            int iInnerCount = 0;


            this.LogMessage(Constants.LOG_SEPARATOR);

            while (oException != null)
            {
                string sIndent = string.Empty.PadRight(iIndentEx);

                this.LogMessage( @"{0}ECCEZIONE! Livello {1}", sIndent, iInnerCount.ToString());
                this.LogMessage( @"{0}  + Tipo     : {1}", sIndent, oException.GetType().Name);
                this.LogMessage( @"{0}  + Messaggio: {1}", sIndent, oException.Message);
                //Dati variabili
                if (!string.IsNullOrEmpty(oException.Source))
                    this.LogMessage( @"{0}  + Source   : {1}", sIndent, oException.Source);

                if (oException.TargetSite != null)
                {
                    this.LogMessage( @"{0}  + Classe   : {1}", sIndent, oException.TargetSite.DeclaringType.Name);
                    this.LogMessage( @"{0}  + Metodo   : {1}", sIndent, oException.TargetSite.Name);
                    this.LogMessage( @"{0}  + Namespace: {1}", sIndent, oException.TargetSite.DeclaringType.Namespace);
                }

                if (oException.StackTrace != null)
                {
                    this.LogMessage( @"{0}  + Stack    :", sIndent);

                    using (System.IO.StringReader reader = new System.IO.StringReader(oException.StackTrace))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            this.LogMessage( @"{0}             > {1}", sIndent, line);
                        }
                    }
                }

                //Successiva
                oException = oException.InnerException;
                iInnerCount++;
                iIndentEx += 4;
            }

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
