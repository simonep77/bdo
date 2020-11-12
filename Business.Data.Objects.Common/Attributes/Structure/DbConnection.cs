using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica la connessione database all'interno del businessslot da utilizzare 
    /// (Necessario in caso di mappature di oggetti su database differenti)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class DbConnection: BaseAttribute 
    {
        public string Name{get; set;}

        /// <summary>
        /// Imposta nome standard di connessione
        /// </summary>
        /// <param name="name"></param>
        public DbConnection(string name)
        {
            this.Name = name;
        }

    }
}
