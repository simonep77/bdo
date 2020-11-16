namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro LESS EQUAL
    /// </summary>
    public class FilterLESSEQ : FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterLESSEQ(string fieldName, object fieldValue)
            :base(fieldName, EOperator.LessEquals, fieldValue)
        {
        }


    }
}
