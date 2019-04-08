using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{

    /// <summary>
    /// Definizione verso ordinamento
    /// </summary>
    public enum OrderVersus
    {
        Asc = 0,
        Desc = 1
    }

    /// <summary>
    /// Classe per la gestione dell'ordinamento
    /// </summary>
    public class OrderBy: IEnumerable<Bdo.Objects.OrderBy.OrderByItem>
    {
        #region INTERNAL CLASS

        /// <summary>
        /// Classe interna di gestione orderBy
        /// </summary>
        public class OrderByItem
        {
            private string mField;
            private OrderVersus mVersus;

            public string Field
            {
                get { return this.mField; }
            }

            public OrderVersus Versus
            {
                get { return this.mVersus; }
            }


            internal OrderByItem(string field, OrderVersus versus)
            {
                this.mField = field;
                this.mVersus = versus;
            }
        }

        #endregion

        private List<OrderByItem> mList = new List<OrderByItem>();

        #region PROPERTIES

        public OrderByItem this[int index]
        {
            get
            {
                return this.mList[index];
            }
        }

        public int Count
        { 
            get 
            {
                return this.mList.Count;
            } 
        }

        #endregion


        #region PUBLIC METHODS

        /// <summary>
        /// Costruttore vuoto
        /// </summary>
        public OrderBy()
        { }

        /// <summary>
        /// Costruttore standard
        /// </summary>
        /// <param name="field"></param>
        /// <param name="versus"></param>
        public OrderBy(string field, OrderVersus versus)
        {
            this.Add(field, versus);
        }

        /// <summary>
        /// Crea orderby con verso default ascendente
        /// </summary>
        /// <param name="field"></param>
        public OrderBy(string field)
        {
            this.Add(field);
        }

        /// <summary>
        /// Aggiunge ulteriore clausola
        /// </summary>
        /// <param name="field"></param>
        /// <param name="versus"></param>
        /// <returns></returns>
        public OrderBy Add(string field, OrderVersus versus)
        {
            if (!string.IsNullOrEmpty(field))
            {
                this.mList.Add(new OrderByItem(field, versus));
            }
            return this;
        }

        /// <summary>
        /// Aggiunge clausola con verso default Ascendente
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public OrderBy Add(string field)
        {
            return this.Add(field, OrderVersus.Asc);
        }

        /// <summary>
        /// Ritorna lo statement di order by completo
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.mList.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(30);

            sb.Append(" ORDER BY ");
            foreach (OrderByItem item in this.mList)
            {
                sb.Append(item.Field);
                if (item.Versus == OrderVersus.Asc)
                {
                    sb.Append(" ASC, ");
                }
                else
                {
                    sb.Append(" DESC, ");
                }
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        /// <summary>
        /// Svuota clausola
        /// </summary>
        public void Clear()
        {
            this.mList.Clear();
        }

        #endregion


        #region IEnumerable<OrderByItem> Membri di

        public IEnumerator<OrderBy.OrderByItem> GetEnumerator()
        {
            return this.mList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Membri di

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.mList.GetEnumerator();
        }

        #endregion
    }
}
