using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common
{
    /// <summary>
    /// Definisce un livello di protezione testabile all'interno degli oggetti
    /// </summary>
    public enum EProtectionLevel
    {
        Lowest = 1,
        Low = 2,
        BelowNormal = 3,
        Normal = 4,
        AboveNormal = 5,
        High = 6,
        Highest = 7
    }
   
}
