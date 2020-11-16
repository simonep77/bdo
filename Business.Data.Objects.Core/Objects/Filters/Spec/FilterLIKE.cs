namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro LIKE
    /// </summary>
    public class FilterLIKE: FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterLIKE(string fieldName, object fieldValue)
            :base(fieldName, EOperator.Like, fieldValue)
        {
        }
        
    }
}
