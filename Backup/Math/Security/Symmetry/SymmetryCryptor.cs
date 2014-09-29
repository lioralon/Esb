using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Security
{  
    public class SymmetryCryptor : ISymmetryCryptor 
    {
        private SymmetricAlgorithm symmetricAlgorithm;
        private const string strInitialVector = "I'm sky!";
        private byte[] initialVector;

        #region Encoding
        private Encoding encoding = Encoding.Default;
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        } 
        #endregion

        #region SymmetricAlgorithmType
        private SymmetricAlgorithmType symmetricAlgorithmType = SymmetricAlgorithmType.DES;        
        public SymmetricAlgorithmType SymmetricAlgorithmType
        {
            get { return symmetricAlgorithmType; }
            set { symmetricAlgorithmType = value; }
        } 
        #endregion

        #region Initialize
        public void Initialize(CipherMode mode)
        {
            this.initialVector = this.GetExactBytes(SymmetryCryptor.strInitialVector, 16);
            switch (this.symmetricAlgorithmType)
            {
                case SymmetricAlgorithmType.DES:
                    {
                        this.symmetricAlgorithm = new DESCryptoServiceProvider();
                        break;
                    }
                case SymmetricAlgorithmType.Rijndael:
                    {
                        this.symmetricAlgorithm = new RijndaelManaged();
                        break;                       
                    }
                case SymmetricAlgorithmType.RC2:
                    {
                        this.symmetricAlgorithm = new RC2CryptoServiceProvider();
                        break;                       
                    }
                case SymmetricAlgorithmType.TripleDES:
                    {
                        this.symmetricAlgorithm = new TripleDESCryptoServiceProvider();
                        break;                       
                    }
                default:
                    {
                        this.symmetricAlgorithm = new DESCryptoServiceProvider();
                        break;                        
                    }
            }

            this.symmetricAlgorithm.Mode = mode;
            //this.symmetricAlgorithm..Padding = PaddingMode.PKCS7;
           
        } 
        #endregion

        #region EncryptStream
        public byte[] EncryptStream(byte[] source, string key)
        {
            if (source == null)
            {
                return null;
            }

            byte[] bytesKey = this.encoding.GetBytes(key);

            MemoryStream memStream = new MemoryStream();
            CryptoStream crytoStream = new CryptoStream(memStream, this.symmetricAlgorithm.CreateEncryptor(bytesKey, this.initialVector), CryptoStreamMode.Write);

            try
            {
                crytoStream.Write(source, 0, source.Length);//将原始字符串加密后写到memStream
                crytoStream.FlushFinalBlock();
                byte[] bytesEncoded = memStream.ToArray();

                return bytesEncoded;

            }
            finally
            {
                memStream.Close();
                crytoStream.Close();
            }
        } 
        #endregion

        #region EncryptStream
        public byte[] EncryptStream(byte[] source, int offset, int toEncrpyLen, string key)
        {
            if (toEncrpyLen == 0)
            {
                return source;
            }

            byte[] temp = this.GetPartOfStream(source, offset, toEncrpyLen);

            return this.EncryptStream(temp, key);
        }

        private byte[] GetPartOfStream(byte[] source, int offset, int length)
        {
            if ((source == null) || offset >= source.Length)
            {
                return null;
            }

            if (length + offset > source.Length)
            {
                length = source.Length - offset;
            }

            byte[] temp = new byte[length];
            for (int i = 0; i < length; i++)
            {
                temp[i] = source[offset + i];
            }

            return temp;
        }
        
        #endregion
    
        #region DecryptStream
        public byte[] DecryptStream(byte[] bytesEncoded, string key)
        {
            if (bytesEncoded == null)
            {
                return null;
            }

            byte[] bytesKey = this.encoding.GetBytes(key);
            MemoryStream memStream = new MemoryStream(bytesEncoded);
            CryptoStream crytoStream = new CryptoStream(memStream, this.symmetricAlgorithm.CreateDecryptor(bytesKey, this.initialVector), CryptoStreamMode.Read);
                                
            try
            {
                StreamReader streamReader = new StreamReader(crytoStream ,this.encoding );
                string ss = streamReader.ReadToEnd();
                return this.encoding.GetBytes(ss);                
            }          
            finally
            {
                memStream.Close();
                crytoStream.Close();
            }           
        }

        public byte[] DecryptStream(byte[] bytesEncoded, int offset, int toDecrpyLen, string key)
        {
            byte[] temp = this.GetPartOfStream(bytesEncoded, offset, toDecrpyLen);
            return this.DecryptStream(temp, key);
        } 
        #endregion       

        #region EncryptString ,DecryptString       
        public string EncryptString(string source, string key)
        {
            byte[] bytes_source = this.encoding.GetBytes(source);
            byte[] bytesEncoded = this.EncryptStream(bytes_source, key);
            return Convert.ToBase64String(bytesEncoded);            
        }
        
        public string DecryptString(string strEncoded, string key)
        {
            byte[] bytesEncoded = Convert.FromBase64String(strEncoded); //不能使用this.encoding.GetBytes(str_encoded);
            byte[] bytesDecoded = this.DecryptStream(bytesEncoded, key);
           
            return this.encoding.GetString(bytesDecoded, 0, bytesDecoded.Length);
        }
        #endregion

        #region private
        private byte[] GetExactBytes(string source, int length)
        {
            byte[] result = new byte[length];
            byte[] buff = this.encoding.GetBytes(source);


            int buff_len = buff.Length;

            if (buff_len >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    result[i] = buff[i];
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    result[i] = buff[i % buff_len];
                }
            }

            return result;
        }

        #endregion
    }  
}
