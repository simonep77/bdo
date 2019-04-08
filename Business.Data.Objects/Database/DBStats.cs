using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Database 
{
    /// <summary>
    /// Classe per statistiche di accesso al DB
    /// </summary>
    public class DBStats
    {

        /// <summary>
        /// Identifica lo statement eseguito
        /// </summary>
        public enum EStatement: Int32
        {
            Select = 1,
            Insert = 2,
            Update = 3,
            Delete = 4,
            Begin = 5,
            Commit = 6,
            Rollback = 7,
            Other = 99
        }

        /// <summary>
        /// Identifica un contatore di singolo statement
        /// </summary>
        internal class StatementRecord
        {
            public EStatement Statement;
            public string Name;
            public long Counter;
        }

        private Dictionary<EStatement, StatementRecord> _Data = new Dictionary<EStatement, StatementRecord>(15);
        private static Array _ENUM_ARR;

        #region PROPRIETA



        #endregion

        #region PRIVATI

        internal void Increment(EStatement statement)
        {
            this._Data[statement].Counter++;
        }
       
        /// <summary>
        /// Azzera contatori
        /// </summary>
        internal void Reset()
        {
            foreach (var item in this._Data.Values)
            {
                item.Counter = 0;
            }
        }

        #endregion

        #region "PUBBLICI"

        static DBStats()
        {
            //Appoggia l'elenco degli enum per non ripetere le chiamate
            _ENUM_ARR = Enum.GetValues(typeof(EStatement));
        }

        public DBStats()
        {
            //Genera i contatori
            foreach (var item in _ENUM_ARR)
            {
                //Popola il counter
                this._Data.Add((EStatement)item, 
                    new StatementRecord() {
                        Statement = (EStatement)item,
                        Name = string.Concat(@"Num", Enum.GetName(typeof(EStatement), item))
                    });
            }
        }


        /// <summary>
        /// Ritorna conteggio per statement
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public long GetCounter(EStatement statement)
        {
            return this._Data[statement].Counter;
        }


        /// <summary>
        /// Aggrega statistiche
        /// </summary>
        /// <param name="other"></param>
        public void Sum(DBStats other)
        {
            foreach (var item in this._Data.Values)
            {
                item.Counter += other._Data[item.Statement].Counter;
            }
        }

        /// <summary>
        /// Ritorna Xml che rappresenta le statistiche
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            using (System.IO.StringWriter tw = new System.IO.StringWriter())
            {
                using (System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(tw))
                {
                    xw.WriteStartElement("DBStats");
                    foreach (var item in this._Data.Values)
                    {
                        xw.WriteElementString(item.Name, item.Counter.ToString());
                    }
                    xw.WriteEndElement();
                }

                return tw.ToString();
            }
        }

        /// <summary>
        /// Ritorna rappresentazione in stringa
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in this._Data.Values)
            {
                sb.Append(item.Name);
                sb.Append(@": ");
                sb.AppendLine(item.Counter.ToString());
            }

            return sb.ToString();
        }

        #endregion
    }
}
