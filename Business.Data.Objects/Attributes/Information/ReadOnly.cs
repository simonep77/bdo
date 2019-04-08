using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Imposta una classe come sola lettura impossibilitando accessi di tipo
    /// Insert, Update, Delete
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReadOnly : BaseAttribute
    {
    }
}
