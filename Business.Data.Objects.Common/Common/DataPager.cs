using System;
using System.Collections.Generic;
using System.Text;
using Business.Data.Objects.Common.Utils;

namespace Business.Data.Objects.Common
{
    /// <summary>
    /// Dati di paginazione
    /// </summary>
    public class DataPager
    {

        /// <summary>
        /// Posizione (se lista paginata)
        /// </summary>
        public int Position
        {
            get
            {
                return ((this.Page - 1) * this.Offset);
            }
        }

        /// <summary>
        /// Numero max elementi
        /// </summary>
        public int Offset { get; set; }


        /// <summary>
        /// Pagina corrente
        /// </summary>
        public int Page { get; set; }


        /// <summary>
        /// Totale record
        /// </summary>
        public int TotRecords { get; set; }


        /// <summary>
        /// Totale pagine
        /// </summary>
        public int TotPages
        {
            get
            {
                return (int)Math.Ceiling(Convert.ToDouble(this.TotRecords) / Convert.ToDouble(this.Offset));
            }
        }

        /// <summary>
        /// Costruttore base
        /// </summary>
        public DataPager()
        {
            this.Offset = 20;
        }


        /// <summary>
        /// Ritorna rappresentazione Xml del pager
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            using (XmlWrite xw = new XmlWrite())
            {
                //Formattazione standard
                xw.WriteStartElement("Pager");
                try
                {
                    xw.WriteElementString("Position", this.Position.ToString());
                    xw.WriteElementString("Offset", this.Offset.ToString());
                    xw.WriteElementString("Page", this.Page.ToString());
                    xw.WriteElementString("TotRecords", this.TotRecords.ToString());
                    xw.WriteElementString("TotPages", this.TotPages.ToString());

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
