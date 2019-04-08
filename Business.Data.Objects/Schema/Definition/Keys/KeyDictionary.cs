using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Schema.Definition
{
    internal class KeyDictionary : Dictionary<string, Key>
    {
        public new void Add(string key, Key value)
        {
            base.Add(key, value);


            value.KeyIndex = Convert.ToInt16(this.Count - 1);
        }
    }
}
