using Business.Data.Objects.Core.Base;
using System.Collections.Generic;

namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Classe risultato di operazione group by su lista
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupByResult<TL>: Dictionary<object, TL> where TL : DataListBase 
    {
        
    }
}
