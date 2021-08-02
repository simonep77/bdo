using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Utils.Encryption
{
    public class CryptoSimpleAES : ICryptoAlgo
    {
        private SimpleAes mSimpleAES;
        public CryptoSimpleAES(string key)
        {
            this.mSimpleAES = new SimpleAes(key);
        }

        public byte[] DecryptData(string encdata)
        {
            return this.mSimpleAES.Decrypt(Convert.FromBase64String(encdata));
        }

        public string DecryptString(string encdata)
        {
            return this.mSimpleAES.Decrypt(encdata);
        }

        public string EncryptData(byte[] cleardata)
        {
            return Convert.ToBase64String(this.mSimpleAES.Encrypt(cleardata));
        }

        public string EncryptString(string cleardata)
        {
            return this.mSimpleAES.Encrypt(cleardata);
        }
    }
}
