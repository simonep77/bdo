using System.Collections.Generic;
using System.Linq;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Classe di tipo elenco messaggi
    /// </summary>
    public class MessageList : List<Message>
    {

        /// <summary>
        /// Indica se presenti messaggi di errore
        /// </summary>
        public bool HasErrors
        {
            get => this.Any(x => x.Severity == ESeverity.Error);
            set => value = false;
        }

        /// <summary>
        /// Indica se presenti warning
        /// </summary>
        public bool HasWarnings
        {
            get => this.Any(x => x.Severity == ESeverity.Warn);
            set => value = false;
        }


        /// <summary>
        /// Aggiunge messaggio di tipo impostabile
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        /// <param name="uifield"></param>
        public void Add(int code, string message, ESeverity severity, string uifield)
        {
            this.Add(new Message
            {
                Code = code,
                Severity = severity,
                Text = message,
                UiField = uifield,
            });
        }

        /// <summary>
        /// Aggiunge messaggio di tipo impostabile senza nome campo UI
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        public void Add(int code, string message, ESeverity severity)
        {
            this.Add(new Message
            {
                Code = code,
                Severity = severity,
                Text = message,
            });
        }

        /// <summary>
        /// Aggiunge messaggio errore
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="uifield"></param>
        public void AddError(int code, string message, string uifield)
        {
            this.Add(new Message
            {
                Code = code,
                Severity = ESeverity.Error,
                Text = message,
                UiField = uifield,
            });
        }

        /// <summary>
        /// Aggiunge messaggio errore senza campo UI assosciato
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public void AddError(int code, string message)
        {
            this.Add(new Message
            {
                Code = code,
                Severity = ESeverity.Error,
                Text = message,
            });
        }

        /// <summary>
        /// Aggiunge messaggio errore solo testuale
        /// </summary>
        /// <param name="message"></param>
        public void AddError(string message)
        {
            this.Add(new Message
            {
                Code = -1,
                Severity = ESeverity.Error,
                Text = message,
            });
        }

        /// <summary>
        /// Aggiunge messaggio info
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="uifield"></param>
        public void AddInfo(int code, string message, string uifield)
        {
            this.Add(new Message
            {
                Code = code,
                Severity = ESeverity.Info,
                Text = message,
                UiField = uifield,
            });
        }

        /// <summary>
        /// Aggiunge messaggio info senza campo UI associato
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public void AddInfo(int code, string message)
        {
            this.Add(new Message
            {
                Code = code,
                Severity = ESeverity.Info,
                Text = message,
            });
        }

        /// <summary>
        /// Aggiunge messaggio info solo testuale
        /// </summary>
        /// <param name="message"></param>
        public void AddInfo(string message)
        {
            this.Add(new Message
            {
                Code = 0,
                Severity = ESeverity.Info,
                Text = message
            });
        }

        /// <summary>
        /// Aggiunge messaggio warning
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="uifield"></param>
        public void AddWarn(int code, string message, string uifield)
        {
            this.Add(new Message
            {
                Code = code,
                Severity = ESeverity.Warn,
                Text = message,
                UiField = uifield,
            });
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
            this.Add(new Message
            {
                Code = -1,
                Severity = ESeverity.Warn,
                Text = message,
            });
        }


        public override string ToString()
        {
            return string.Join(" | ", this.Select(x => $"[{x.Severity}] {x.Text}"));
        }
    }
}
