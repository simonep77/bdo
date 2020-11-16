namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro ISNOTNULL
    /// </summary>
    public class FilterISNOTNULL : FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterISNOTNULL(string fieldName)
            :base(fieldName, EOperator.IsNotNull, null)
        {
        }


    }
}
