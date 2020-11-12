using System;
using System.Collections.Generic;
using Bdo.Objects;
using Bdo.Schema.Definition;
using Business.Data.Objects.Common.Exceptions;

namespace Bdo.Schema
{
    /// <summary>
    /// Eccezione da errore schema
    /// </summary>
    [Serializable]
    public class SchemaReaderException: BdoBaseException
    {
        public SchemaReaderException(string msgFormat, params object[] args)
            : base(string.Format(msgFormat, args))
        {
        }

        #region For Schema

        internal SchemaReaderException(ClassSchema schema, string messageFmt, string infoText)
           : base(string.Format(messageFmt, schema.ClassName, infoText))
        {
        }

        internal SchemaReaderException(ClassSchema schema, string messageFmt)
            : base(string.Format(messageFmt, schema.ClassName))
        {
        }

        #endregion

        #region For Properies

        internal SchemaReaderException(Property propIn, string messageFmt, string infoText)
           : base(string.Format(messageFmt, propIn.Schema.ClassName, propIn.Name, infoText))
        {
        }

        internal SchemaReaderException(Property propIn, string messageFmt)
            : base(string.Format(messageFmt, propIn.Schema.ClassName, propIn.Name))
        {
        }

        #endregion
    }
}


