using Business.Data.Objects.Core.Schema.Definition;

namespace Business.Data.Objects.Core.Attributes
{
    public abstract class BaseValidatorAttribute : BaseAttribute 
    {
        public readonly string CustomMessage;

        public BaseValidatorAttribute(string customMessage)
        {
            this.CustomMessage = string.Intern(customMessage);
        }

        /// <summary>
        /// Indica se applicabile a proprieta' specificata
        /// </summary>
        /// <param name="propIn"></param>
        /// <returns></returns>
        internal abstract bool CanApplyToProperty(Property propIn);

        /// <summary>
        /// Esegue la validazione
        /// </summary>
        /// <param name="propIn"></param>
        /// <param name="value"></param>
        internal abstract void Validate(Property propIn, object value);


    }
}
