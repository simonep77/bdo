using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Schema.Definition;
using Bdo.Utils;

namespace Bdo.Attributes
{
    /// <summary>
    /// Attributo che non necessita di valore
    /// </summary>
    public abstract class BaseStringModifierAttribute: BaseModifierAttribute
    {
        /// <summary>
        /// Indica se applicabile alla proprieta' fornita
        /// </summary>
        /// <param name="propIn"></param>
        /// <returns></returns>
        internal override bool CanApplyToProperty(Property propIn)
        {
            return TypeHelper.IsString(propIn.Type);
        }


    }
}
