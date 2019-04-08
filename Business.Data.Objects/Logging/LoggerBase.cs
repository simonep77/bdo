using System;
using System.Text;
using System.IO;

namespace Bdo.Logging
{
    /// <summary>
    /// Classe per log su file di testo
    /// </summary>
    public abstract class LoggerBase: IDisposable   
    {
        #region FIELDS

        protected readonly object SyncLock = new object();
        private string mDateFormat = @"dd/MM/yyyy HH:mm:ss";
        private bool mWriteThreadId;

        #endregion 

        #region ILogger Membri di

        /// <summary>
        /// Path completo log
        /// </summary>
        public abstract string LogPath {get;}

        /// <summary>
        /// Formato data
        /// </summary>
        public string DateFormat
        {
            get
            {
                return this.mDateFormat;
            }
            set
            {
                //Evita che si cambi il file di log durante una scrittura
                lock (this.SyncLock)
                {
                    try
                    {
                        DateTime.Now.ToString(value);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Il formato data impostato non risulta valido.", e);
                    }

                    this.mDateFormat = value;
                }
            }
        }

        /// <summary>
        /// Indica se scrivere nel log l'id del thread
        /// </summary>
        public bool WriteThreadId
        {
            get { return this.mWriteThreadId; }
            set { this.mWriteThreadId = value; }
        }

        /// <summary>
        /// Log messaggio
        /// </summary>
        /// <param name="msgIn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract void LogMessage(string msgIn, params object[] args);


        /// <summary>
        /// Log eccezione
        /// </summary>
        /// <param name="e"></param>
        /// <param name="includeStack"></param>
        /// <returns></returns>
        public abstract void LogException(Exception e, bool includeStack);


        /// <summary>
        /// Formatta una messaggio per scrittura log con la specifica del formato data e scrittura thread
        /// </summary>
        /// <param name="dateFromat"></param>
        /// <param name="writeThreadId"></param>
        /// <param name="msgIn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatLogTextMessage(string dateFromat, bool writeThreadId, string msgIn, params object[] args)
        {
            StringBuilder sbLogMsg = new StringBuilder();

            //DataOra
            sbLogMsg.Append(DateTime.Now.ToString(dateFromat));

            //Thread
            if (writeThreadId)
                sbLogMsg.Append(LoggerBase.getLogThreadInfo());

            //Cattura eventuali errori di formattazione
            try
            {
                sbLogMsg.AppendFormat(msgIn, args);
            }
            catch (Exception e)
            {
                sbLogMsg.Append(msgIn);

                if (args != null)
                {
                    sbLogMsg.Append(@" - Args: ");

                    for (int i = 0; i < args.Length; i++)
                    {
                        sbLogMsg.Append(args[i]);
                        sbLogMsg.Append(@", ");
                    }
                        
                }

                sbLogMsg.Append(e.Message);
            }

            //Ritorna
            return sbLogMsg.ToString();
        }

        /// <summary>
        /// Formatta una messaggio per scrittura log con default data dd/MM/yyyy HH:mm:ss e scrittura thread
        /// </summary>
        /// <param name="msgIn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatLogTextMessage(string msgIn, params object[] args)
        {
            return FormatLogTextMessage(@"dd/MM/yyyy HH:mm:ss", true, msgIn, args);
        }


        /// <summary>
        /// Ritorna testo log per eccezione con la specifica del formato data e scrittura thread
        /// </summary>
        /// <param name="e"></param>
        /// <param name="includeStack"></param>
        /// <param name="dateFromat"></param>
        /// <param name="writeThreadId"></param>
        /// <returns></returns>
        public static string FormatLogTextException(string dateFromat, bool writeThreadId, Exception e, bool includeStack)
        {

                //Scrive
            StringBuilder sb = new StringBuilder(1024);
            Exception oException = e;
            int iIndentEx = 0;
            int iInnerCount = 0;
            string sDate = string.Empty;
            string sThread = string.Empty;

            //Se il formato data non e' fornito non lo scrive
            if (!string.IsNullOrEmpty(dateFromat))
                sDate = DateTime.Now.ToString(dateFromat);

            //Se necessario scrive Id del thread
            if (writeThreadId)
                sThread = getLogThreadInfo();

            sb.AppendLine(string.Concat(sDate, sThread, @" - ", Bdo.Common.Constants.LOG_SEPARATOR));

            while (oException != null)
            {
                string sIndent = string.Empty.PadRight(iIndentEx);

                sb.AppendLine(string.Concat(sDate, sThread, @" - ", sIndent, @"ECCEZIONE! Livello ", iInnerCount.ToString("D2")));
                sb.AppendLine(string.Concat(sDate, sThread, @" - ", sIndent, @"  + Tipo     : ", oException.GetType().Name));
                sb.AppendLine(string.Concat(sDate, sThread, @" - ", sIndent, @"  + Messaggio: ", oException.Message));
                sb.AppendLine(string.Concat(sDate, sThread, @" - ", sIndent, @"  + Source   : ", oException.Source));
                sb.AppendLine(string.Concat(sDate, sThread, @" - ", sIndent, @"  + Classe   : ", oException.TargetSite.DeclaringType.Name));
                sb.AppendLine(string.Concat(sDate, sThread, @" - ", sIndent, @"  + Metodo   : ", oException.TargetSite.Name));
                sb.AppendLine(string.Concat(sDate, sThread, @" - ", sIndent, @"  + Namespace: ", oException.TargetSite.DeclaringType.Namespace));
                if (includeStack)
                {
                    sb.AppendLine(string.Concat(sDate, sThread, @" - ", sIndent, @"  + Stack     : ", oException.StackTrace));
                }

                //Successiva
                oException = oException.InnerException;
                iInnerCount++;
                iIndentEx += 4;
            }

            sb.Append(string.Concat(sDate, sThread, @" - ", Bdo.Common.Constants.LOG_SEPARATOR));

            return sb.ToString();
        }


        /// <summary>
        /// Ritorna testo log per eccezione con default data dd/MM/yyyy HH:mm:ss e scrittura thread
        /// </summary>
        /// <param name="e"></param>
        /// <param name="includeStack"></param>
        /// <returns></returns>
        public static string FormatLogTextException(Exception e, bool includeStack)
        {
            return FormatLogTextException(@"dd/MM/yyyy HH:mm:ss", true, e, includeStack);
        }


        /// <summary>
        /// Entra in modalita' scrittura esclusiva (thread-safe)
        /// </summary>
        public void BeginSafeWrite()
        {
            System.Threading.Monitor.Enter(this.SyncLock);
        }


        /// <summary>
        /// Esce dalla modalita' scrittura esclusiva (thread-safe)
        /// </summary>
        public void EndSafeWrite()
        {
            System.Threading.Monitor.Exit(this.SyncLock);
        }

        /// <summary>
        /// Ferma flusso log
        /// </summary>
        public abstract void StartLog();


        /// <summary>
        /// Avvia/riavvia flusso log
        /// </summary>
        public abstract void StopLog();

        /// <summary>
        /// Libera risorse
        /// </summary>
        public virtual void Dispose()
        {
            this.StopLog();
        }

        #endregion

        #region PRIVATE

        //Thread
        private static string getLogThreadInfo()
        {
            return string.Concat(@" - T",
                                 System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("D4"),
                                 @" ");
        }
            
        #endregion

    }
}
