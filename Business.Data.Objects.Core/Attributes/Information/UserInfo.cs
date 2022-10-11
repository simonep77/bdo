using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Indica che la proprieta' deve essere gestita in automatico attribuendo il valore prelevato da
    ///  - evento slot.OnUserInfoRequired
    ///  - in alternativa da slot.Username
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UserInfo : BaseAttribute 
    {
    }
}
