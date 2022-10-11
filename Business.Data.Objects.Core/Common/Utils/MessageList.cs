using System.Collections.Generic;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Classe di tipo elenco messaggi
    /// </summary>
    public class MessageList: List<Message>
    {

        /// <summary>
        /// Indica se presenti messaggi di errore
        /// </summary>
        public bool HasErrors
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Severity == ESeverity.Error)
                        return true;
                }

                return false;
            }
        }


        /// <summary>
        /// Ritorna il numero di messaggi di una certa severità
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        public int CountForSeverity(ESeverity severity)
        {
            int iTemp = 0;

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Severity == severity)
                {
                    iTemp++;
                }
            }

            return iTemp;
        }


        /// <summary>
        /// Ritorna la rappresentazione Xml.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public string ToXml()
        {
            using (XmlWrite xw = new XmlWrite()) 
            {

                //Crea Xml Standard
                xw.WriteStartElement("MESSAGELIST");
                try
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        xw.WriteRaw(this[i].ToXml());
                    }
                }
                finally
                {
                    xw.WriteEndElement();
                }


                return xw.ToString();
            }
            
        }

        /// <summary>
        /// Aggiunge messaggio di tipo impostabile
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        /// <param name="uifiled"></param>
        public void Add(int code, string message, ESeverity severity, string uifiled)
        { 
            this.Add(new Message(code, message, severity, uifiled));
        }

        /// <summary>
        /// Aggiunge messaggio di tipo impostabile senza nome campo UI
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        public void Add(int code, string message, ESeverity severity)
        { 
            this.Add(code, message, severity, string.Empty);
        }

        /// <summary>
        /// Aggiunge messaggio errore
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="uifield"></param>
        public void AddError(int code, string message, string uifield)
        {
            this.Add(code, message, ESeverity.Error, uifield);
        }

        /// <summary>
        /// Aggiunge messaggio errore senza campo UI assosciato
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public void AddError(int code, string message)
        { 
            this.Add(code, message, ESeverity.Error, string.Empty);
        }

        /// <summary>
        /// Aggiunge messaggio errore solo testuale
        /// </summary>
        /// <param name="message"></param>
        public void AddError(string message)
        {
            this.Add(-1, message, ESeverity.Error, string.Empty);
        }

        /// <summary>
        /// Aggiunge messaggio info
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="uifield"></param>
        public void AddInfo(int code, string message, string uifield)
        {
            this.Add(code, message, ESeverity.Info, uifield);
        }

        /// <summary>
        /// Aggiunge messaggio info senza campo UI associato
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public void AddInfo(int code, string message)
        {
            this.Add(code, message, ESeverity.Info, string.Empty);
        }

        /// <summary>
        /// Aggiunge messaggio info solo testuale
        /// </summary>
        /// <param name="message"></param>
        public void AddInfo(string message)
        {
            this.Add(-1, message, ESeverity.Info, string.Empty);
        }

        /// <summary>
        /// Aggiunge messaggio warning
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="uifield"></param>
        public void AddWarn(int code, string message, string uifield)
        {
            this.Add(code, message, ESeverity.Warn, uifield);
        }

        /// <summary>
        /// Aggiunge messaggio warning senza campo UI associato
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public void AddWarn(int code, string message)
        {
            this.Add(code, message, ESeverity.Warn, string.Empty);
        }

        /// <summary>
        /// Aggiunge messaggio solo testuale
        /// </summary>
        /// <param name="message"></param>
        public void AddWarn(string message)
        {
            this.Add(-1, message, ESeverity.Warn, string.Empty);
        }


         /// <summary>
         /// Aggiunge messaggio con codice, sevarita' e messaggio in notazione String.Format
         /// </summary>
         /// <param name="code"></param>
         /// <param name="severity"></param>
         /// <param name="messageFmt"></param>
         /// <param name="args"></param>
        public void AddFormat(int code, ESeverity severity, string messageFmt, params object[] args)
        {
            this.Add(code, string.Format(messageFmt, args), severity, string.Empty);
        }
    }
}
