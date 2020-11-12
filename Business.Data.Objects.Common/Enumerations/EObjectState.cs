using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common
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
