using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects.Base;

namespace Bdo.Objects
{
    /// <summary>
    /// Classe risultato di operazione group by su lista
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupByResult<TL>: Dictionary<object, TL> where TL : DataListBase 
    {
        
    }
}
