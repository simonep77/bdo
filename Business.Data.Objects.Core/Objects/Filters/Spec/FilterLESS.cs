namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro LESS
    /// </summary>
    public class FilterLESS : FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterLESS(string fieldName, object fieldValue)
            :base(fieldName, EOperator.LessThan, fieldValue)
        {
        }


    }
}
