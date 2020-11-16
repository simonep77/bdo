namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro NOTLIKE
    /// </summary>
    public class FilterNOTLIKE : FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterNOTLIKE(string fieldName, object fieldValue)
            :base(fieldName, EOperator.NotLike, fieldValue)
        {
        }
        
    }
}
