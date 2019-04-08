using System;
using System.Text;
using System.IO;

namespace Bdo.Logging
{
    /// <summary>
    /// Classe scrittura su file di testo senza la specifica di data e ora
    /// </summary>
    public class SimpleTextFileAppend  
    {
        #region FIELDS

        private string mFilePath;
        private object mSyncLock = new object();
        #endregion 

        #region FIELDS

        /// <summary>
        /// Crea logger su file fornito in modalità append fornita e attivo specificato
        /// </summary>
        /// <param name="filePath"></param>
        public SimpleTextFileAppend(string filePath)
        {
            this.mFilePath = filePath;

            Directory.CreateDirectory(Path.GetDirectoryName(this.mFilePath));
        }

        #endregion

        #region ILogger Membri di

        /// <summary>
        /// Path completo file
        /// </summary>
        public string LogPath
        {
            get { return this.mFilePath; }
        }

        

        /// <summary>
        /// Appende testo
        /// </summary>
        /// <param name="text"></param>
        public void AppendText(string text)
        {
            lock (this.mSyncLock)
            {
                //scrive
                using (StreamWriter oStream = File.AppendText(this.mFilePath))
                {
                    oStream.Write(text);
                    oStream.Flush();
                }
            }
            
        }


        /// <summary>
        /// Appende testo formattato 
        /// </summary>
        /// <param name="textFormat"></param>
        /// <param name="args"></param>
        public void AppendText(string textFormat, params object[] args)
        {
            this.AppendText(string.Format(textFormat, args));
        }


        /// <summary>
        /// Appende testo aggiungendo newline
        /// </summary>
        /// <param name="text"></param>
        public void AppendTextLine(string text)
        {
            this.AppendText(text + Environment.NewLine);
        }

        /// <summary>
        /// Appende testo formattato aggiungendo newline
        /// </summary>
        /// <param name="textFormat"></param>
        /// <param name="args"></param>
        public void AppendTextLine(string textFormat, params object[] args)
        {
            this.AppendText(string.Format(textFormat, args) + Environment.NewLine);
        }


        #endregion

    }
}
