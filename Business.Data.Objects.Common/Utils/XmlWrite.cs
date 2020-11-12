using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Business.Data.Objects.Common.Utils
{

    /// <summary>
    /// Classe utility per scrivere xml con un solo oggetto
    /// </summary>
    /// <remarks></remarks>
    public class XmlWrite: IDisposable
    {

        private XmlTextWriter _xw;
        private StringWriter _sw;

        private void init()
        {
            this._sw = new StringWriter();
            this._xw = new XmlTextWriter(_sw);
        }

        private void end()
        {
            this._xw.Close();
        }

        public XmlWrite()
        {
            this.init();
        }

        /// <summary>
        /// Resetta contenuto
        /// </summary>
        public void Reset()
        {
            this.end();
            this.init();
        }

        /// <summary>
        /// Scrive elemento iniziale
        /// </summary>
        /// <param name="nomeElemento"></param>
        public void WriteStartElement(string nomeElemento)
        {
            this._xw.WriteStartElement(nomeElemento);
        }


        /// <summary>
        /// Scrive elemento finale
        /// </summary>
        public void WriteEndElement()
        {
            this._xw.WriteEndElement();

        }

        /// <summary>
        /// Scrive elemento con valore
        /// </summary>
        /// <param name="nomeElemento"></param>
        /// <param name="valore"></param>
        public void WriteElementString(string nomeElemento, string valore)
        {
            this._xw.WriteElementString(nomeElemento, valore);
        }

        /// <summary>
        /// Scrive XML senza controllo
        /// </summary>
        /// <param name="rawXml"></param>
        public void WriteRaw(string rawXml)
        {
            this._xw.WriteRaw(rawXml);
        }

        /// <summary>
        /// Scrive valore all'interno del tag
        /// </summary>
        /// <param name="value"></param>
        public void WriteValue(object value)
        {
            this._xw.WriteValue(value);
        }

        /// <summary>
        /// Scrive attributo
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="valore"></param>
        public void WriteAttributeString(string nome, string valore)
        {
            this._xw.WriteAttributeString(nome, valore);
        }

        /// <summary>
        /// Scrive dati in base64
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteBase64(byte[] buffer)
        {
            this._xw.WriteBase64(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Scrive CDATA
        /// </summary>
        /// <param name="value"></param>
        public void WriteCData(string value)
        {
            this._xw.WriteCData(value);
        }

        /// <summary>
        /// Scrive commento
        /// </summary>
        /// <param name="value"></param>
        public void WriteComment(string value)
        {
            this._xw.WriteComment(value);
        }


        /// <summary>
        /// Ritorna Output Xml
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            this._xw.Flush();
            return this._sw.ToString();
        }


        #region IDisposable Membri di

        void IDisposable.Dispose()
        {
            this.end();
        }

        #endregion

    }

}