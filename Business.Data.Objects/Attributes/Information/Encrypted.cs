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
using Business.Data.Objects.Common.Exceptions;

namespace Bdo.Attributes
{
    /// <summary>
    /// Identifica la proprieta' come criptata su db andando a codificare/decodificare il contenuto del campo DB.
    /// Il parametro keyProperty del costruttore identifica il nome di proprieta' dello slot che DEVE contenere la chiave
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Encrypted : BaseAttribute
    {
        private const string S_ENC_PREFIX = @"@ENC@";
        private string mSlotKeyProperty;
        private int mFieldLen;

        /// <summary>
        /// Viene richiesto il nome della Property dello slot che conterra' la chiave e la lunghezza del campo da verificare (per evtare che il dato piu' lungo possa essere troncato)
        /// </summary>
        /// <param name="slotEncDefinitionKey"></param>
        public Encrypted(string slotKeyProperty, int fieldLen)
            : base()
        {
            this.mSlotKeyProperty = slotKeyProperty;
            this.mFieldLen = fieldLen;
        }

        /// <summary>
        /// Verifica se il testo criptato e' conforme allo standard
        /// </summary>
        /// <param name="encText"></param>
        /// <returns></returns>
        internal bool IsEncrypted(string encText)
        {
            if (encText.StartsWith(S_ENC_PREFIX, StringComparison.Ordinal))
                return true;

            return false;
        }


        /// <summary>
        /// Cripta il testo fornito
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="clearText"></param>
        /// <returns></returns>
        internal string Encrypt(BusinessSlot slot, Property prop, string clearText)
        {
            var sEnc = string.Concat(S_ENC_PREFIX,RSA_AES_Encryptor.AES_Encrypt_B64_Text(clearText, slot.PropertyGet(this.mSlotKeyProperty).ToString(), Encoding.UTF8));

            if (sEnc.Length > this.mFieldLen)
                throw new ObjectException(Resources.ObjectMessages.Prop_Enc_Wrong_Length, prop.Name, this.mFieldLen);

            return sEnc;
        }

        /// <summary>
        /// Decripta il testo fornito
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encText"></param>
        /// <returns></returns>
        internal string Decrypt(BusinessSlot slot, Property prop, string encText)
        {
            if (!this.IsEncrypted(encText))
                return encText;

            try
            {
                return RSA_AES_Encryptor.AES_Decrypt_B64_Text(encText.Substring(S_ENC_PREFIX.Length), slot.PropertyGet(this.mSlotKeyProperty).ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new ObjectException(Resources.ObjectMessages.Enc_Decrypt_Failure, prop.Schema.ClassName, prop.Name, ex.Message);
            }


        }

    }
}
