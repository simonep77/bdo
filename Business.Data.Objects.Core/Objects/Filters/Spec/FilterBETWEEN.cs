namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro BETWEEN
    /// </summary>
    public class FilterBETWEEN: FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterBETWEEN(string fieldName, object value1, object value2)
            : base(fieldName, EOperator.Between, new object[]{ value1, value2})
        {
            
        }


    }
}
