using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Schema.Definition;

namespace Business.Data.Objects.Core.Attributes
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
            return propIn.Type.IsString();
        }


    }
}
