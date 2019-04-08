using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Enumerazione stati interni oggetto
    /// </summary>
    public enum EObjectState: byte
    {
        New,
        Loaded,
        Deleted
    }
}
