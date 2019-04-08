using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Utils;
using Bdo.Schema.Definition;
using Bdo.Schema.Usage;
using Bdo.ObjFactory;

namespace Bdo.Objects.Base
{
    /// <summary>
    /// Inner list item 
    /// </summary>
    internal class InnerDataListItem
    {
        /// <summary>
        /// Values of PK
        /// </summary>
        internal object[] PkValues;

        /// <summary>
        /// Eventual Hash of PK values
        /// </summary>
        internal string PkHashCode;

        /// <summary>
        /// Other query values 
        /// </summary>
        internal Dictionary<string, object> ExtraData;

        /// <summary>
        /// Loaded inner object
        /// </summary>
        internal DataObjectBase Object;

        /// <summary>
        /// Serializza item
        /// </summary>
        /// <returns></returns>
       

    }
}
