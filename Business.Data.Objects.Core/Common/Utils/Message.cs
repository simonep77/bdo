using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Utils;
using System.Collections.Generic;

namespace Business.Data.Objects.Common.Utils
{
    /// <summary>
    /// Identifica un messaggio di esecuzione
    /// </summary>
    public class Message
    {

        #region PROPERTY

        public int Code { get; set; }


        public string Text { get; set; }


        public string UiField { get; set; }


        public ESeverity Severity { get; set; }


        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();

        #endregion




    }
}
