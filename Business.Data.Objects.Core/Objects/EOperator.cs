using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core
{

    /// <summary>
    /// Indica il tipo di confronto da effettuare
    /// </summary>
    public enum EOperator
    { 
        /// <summary>
        /// Operatore =
        /// </summary>
        Equal,

        /// <summary>
        /// Operatore <>
        /// </summary>
        Differs,

        /// <summary>
        /// Operatore >
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Operatore >=
        /// </summary>
        GreaterEquals,

        /// <summary>
        /// Operatore &lt;
        /// </summary>
        LessThan,

        /// <summary>
        /// Operatore &lt;=
        /// </summary>
        LessEquals,

        /// <summary>
        /// Operatore LIKE
        /// </summary>
        Like,

        /// <summary>
        /// Operatore NOT LIKE
        /// </summary>
        NotLike,

        /// <summary>
        /// Operatore IS NULL
        /// </summary>
        IsNull,

        /// <summary>
        /// Operatore IS NOT NULL
        /// </summary>
        IsNotNull,

        /// <summary>
        /// Operatore BETWEEN
        /// </summary>
        Between,

        /// <summary>
        /// Operatore IN
        /// </summary>
        In
    }
}
