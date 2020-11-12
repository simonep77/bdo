using System.Collections.Generic;
using Bdo.Utils;
using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Utils;

namespace Bdo.Objects
{
    /// <summary>
    /// Identifica un messaggio di esecuzione
    /// </summary>
    public class Message
    {
        private Dictionary<string, string> mParams = new Dictionary<string,string>();

        #region PROPERTY

        public int Code {get; set;}


        public string Text {get; set;}


        public string UiField {get; set;}


        public ESeverity Severity {get; set;}


        public Dictionary<string, string> Params
        {
            get
            {
                return this.mParams;
            }
        }

        #endregion

        public Message(string text)
            : this(1, text, ESeverity.Error, string.Empty, null)
        {
        }

        public Message(int code, string text)
            : this(code, text, ESeverity.Error, string.Empty, null)
        {
        }

        public Message(int code, string text, ESeverity severity)
            :this(code,text,severity, string.Empty, null)
        {
        }

        public Message(int code, string text, ESeverity severity, string uifield)
            : this(code, text, severity, uifield, null)
        {
        }

        public Message(int code, string text, ESeverity severity, string uifield, Dictionary<string, string> paramsIn)
        {
            this.Code = code;
            this.Text = text;
            this.Severity = severity;
            this.UiField = uifield;

            if (paramsIn != null)
            {
                foreach (var item in paramsIn)
                {
                    this.Params[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// Rappresentazione XML del messaggio
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        { 
            using (XmlWrite xw = new XmlWrite())
            {
                xw.WriteStartElement("MESSAGE");
                try
                {
                    xw.WriteElementString("CODE", this.Code.ToString());
                    xw.WriteElementString("TEXT", this.Text);
                    xw.WriteElementString("SEVERITY", this.Severity.ToString().ToUpper());
                    xw.WriteElementString("UIFIELD", this.UiField);

                    //Se presenti parametri li inserisce
                    if (this.Params.Count > 0)
                    {
                        foreach (var item in this.Params)
                        {
                            xw.WriteStartElement("PARAMS");
                            try
                            {
                                xw.WriteElementString("KEY", item.Key);
                                xw.WriteElementString("VALUE", item.Value);

                            }
                            finally
                            {
                                xw.WriteEndElement();
                            }
                        }
                    }
                }
                finally
                {
                    xw.WriteEndElement();
                }

                return xw.ToString();
            }
        }

    }
}
