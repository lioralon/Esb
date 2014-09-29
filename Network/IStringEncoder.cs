using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Network
{
    /// <summary>
    /// StringEncoder 指定字符串转换成流时的编码格式
    /// </summary>
    public interface IStringEncoder
    {
        string GetStrFromStream(byte[] stream, int offset, int len);
        byte[] GetBytesFromStr(string ss);
    }

    public class UTF8StringEncoder : IStringEncoder
    {
        public string GetStrFromStream(byte[] stream, int offset, int len)
        {
            return System.Text.Encoding.UTF8.GetString(stream, offset, len);
        }

        public byte[] GetBytesFromStr(string ss)
        {
            return System.Text.Encoding.UTF8.GetBytes(ss);
        }
    }
}
