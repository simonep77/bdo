using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Imposta una stringa di formattazione con cui verrà formattato il valore nell'output Xml
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class XmlFormatString: BaseAttribute 
    {

        public string Format { get; set; }

        public XmlFormatString(string format)
        {
            this.Format = format;
        }
    }
}
