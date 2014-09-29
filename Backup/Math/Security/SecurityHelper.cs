using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ESBasic.Security
{
    public static class SecurityHelper
    {
        #region MD5Password
        /// <summary>
        /// MD5Password ���ַ�������MD5ժҪ���㡣
        /// </summary>      
        public static string MD5String(string pwd)
        {
            return SecurityHelper.MD5String(pwd, Encoding.UTF8);
        }

        public static string MD5String(string pwd, Encoding encoding)
        {
            byte[] origin = encoding.GetBytes(pwd);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(origin);

            StringBuilder strBuilder = new StringBuilder("");
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x")) ;
            }

            return strBuilder.ToString();                
        } 
        #endregion
    }
}
