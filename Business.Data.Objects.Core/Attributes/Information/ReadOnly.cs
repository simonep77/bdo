using System;

namespace Business.Data.Objects.Core.Attributes
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
