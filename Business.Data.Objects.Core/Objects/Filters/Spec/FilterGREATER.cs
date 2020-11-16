namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Filtro GREATER
    /// </summary>
    public class FilterGREATER : FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterGREATER(string fieldName, object fieldValue)
            :base(fieldName, EOperator.GreaterThan, fieldValue)
        {
        }


    }
}
