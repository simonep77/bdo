namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro EQUAL
    /// </summary>
    public class FilterEQUAL: FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterEQUAL(string fieldName, object fieldValue)
            :base(fieldName, EOperator.Equal, fieldValue)
        {
        }
    }
}
