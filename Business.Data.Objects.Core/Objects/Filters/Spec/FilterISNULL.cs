namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro ISNULL
    /// </summary>
    public class FilterISNULL : FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterISNULL(string fieldName)
            :base(fieldName, EOperator.IsNull, null)
        {
        }


    }
}
