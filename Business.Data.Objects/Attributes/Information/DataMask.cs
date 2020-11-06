using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Bdo.Schema.Definition;
using Bdo.Objects;
using Bdo.Objects.Base;
using Bdo.Utils;
using Bdo.Utils.Encryption;

namespace Bdo.Attributes
{
    /// <summary>
    /// Identifica la proprieta' come criptata su db andando a codificare/decodificare il contenuto del campo DB.
    /// Il parametro keyProperty del costruttore identifica il nome di proprieta' dello slot che DEVE contenere la chiave
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataMask : BaseAttribute
    {
        private char mMaskChar = '*';

        /// <summary>
        /// Viene richiesto il nome della Property dello slot che conterra' la chiave e la lunghezza del campo da verificare (per evtare che il dato piu' lungo possa essere troncato)
        /// </summary>
        /// <param name="slotEncDefinitionKey"></param>
        public DataMask(char maskChar)
            : base()
        {
            this.mMaskChar = maskChar;
        }

        internal string GetMasked(string text)
        {
            return string.Concat(text[0], string.Empty.PadRight(text.Length-2, this.mMaskChar), text.Length > 2 ? text[text.Length - 2].ToString() : string.Empty);
        }

        /// <summary>
        /// Verifica se il testo criptato e' conforme allo standard
        /// </summary>
        /// <param name="encText"></param>
        /// <returns></returns>
        internal bool IsMasked(string text)
        {
            return text.Equals(this.GetMasked(text));
        }

    }
}
