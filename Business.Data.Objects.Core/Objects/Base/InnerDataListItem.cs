using System.Collections.Generic;

namespace Business.Data.Objects.Core.Base
{
    /// <summary>
    /// Inner list item 
    /// </summary>
    class InnerDataListItem
    {
        /// <summary>
        /// Values of PK
        /// </summary>
        internal object[] PkValues;

        /// <summary>
        /// Other query values 
        /// </summary>
        internal Dictionary<string, object> ExtraData;

        /// <summary>
        /// Loaded inner object
        /// </summary>
        internal DataObjectBase Object;

    }
}
