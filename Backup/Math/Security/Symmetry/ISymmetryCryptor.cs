using System;
using System.Text;
using System.Security.Cryptography;

namespace ESBasic.Security
{
    public interface ISymmetryCryptor
    {
        Encoding Encoding { get; set; }

        /// <summary>
        /// SymmetricAlgorithmType 采用的加密算法类型。
        /// 如果是DES加密，则要求64位密匙。
        /// 如果是Rijndael加密，则支持 128、192 或 256 位的密钥长度。
        /// 如果是RC2加密，则支持的密钥长度为从 40 位到 128 位，以 8 位递增。
        /// 如果是TripleDES加密，则支持从 128 位到 192 位（以 64 位递增）的密钥长度。
        /// </summary>
        SymmetricAlgorithmType SymmetricAlgorithmType { get; set; }

        void Initialize(CipherMode mode);

        byte[] EncryptStream(byte[] source, string key);
        byte[] EncryptStream(byte[] source, int offset, int toEncrpyLen, string key);
        byte[] DecryptStream(byte[] bytesEncoded, string key);
        byte[] DecryptStream(byte[] bytesEncoded, int offset, int toDecrpyLen, string key);   
       
        string EncryptString(string source, string key);
        string DecryptString(string strEncoded, string key);   
    }

    public enum SymmetricAlgorithmType
    {
        DES, RC2, Rijndael, TripleDES
    }
}