using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Schema.Definition;

namespace Bdo.Attributes
{
    /// <summary>
    /// Attributo che non necessita di valore
    /// </summary>
    public abstract class BaseModifierAttribute: BaseAttribute
    {
        /// <summary>
        /// Indica se applicabile alla proprieta' fornita
        /// </summary>
        /// <param name="propIn"></param>
        /// <returns></returns>
        internal abstract bool CanApplyToProperty(Property propIn);


        /// <summary>
        /// Esegue la modifica
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract object Modify(object value);

    }
}
