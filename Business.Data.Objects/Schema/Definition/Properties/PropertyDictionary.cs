using System;
using System.Collections.Generic;

namespace Bdo.Schema.Definition
{
    /// <summary>
    /// Elenco Proprietà
    /// </summary>
    internal class PropertyDictionary : List<Property>
    {
        private ClassSchema mSchema;
        private Dictionary<string, Property> mDictionary;

        internal PropertyDictionary(ClassSchema schema, int capacity)
            :base(capacity)
        {
            this.mSchema = schema;
            this.mDictionary = new Dictionary<string, Property>(capacity);
        }

        /// <summary>
        /// Aggiunge proprietà e crea indive
        /// </summary>
        /// <param name="prop"></param>
        public new void Add(Property prop)
        {
            base.Add(prop);
            this.mDictionary.Add(prop.Name, prop);
        }

        /// <summary>
        /// Cerca proprietà per nome
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public Property GetPropertyByName(string propName)
        {
            Property oProp = null;

            if (!this.mDictionary.TryGetValue(propName, out oProp))
                throw new Bdo.Objects.ObjectException(Resources.ObjectMessages.Base_PropertyNotExists, this.mSchema.ClassName, propName);

            return oProp;
        }

        /// <summary>
        /// Verifica esistenza proprieta per nome
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public bool ContainsProperty(string propName)
        {
            return this.mDictionary.ContainsKey(propName);
        }

        /// <summary>
        /// Verifica esistenza proprieta e ritorna valore
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetProperty(string propName, out Property value)
        {
            return this.mDictionary.TryGetValue(propName, out value);
        }
    }
}
