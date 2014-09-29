using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Network.Tcp
{
    /// <summary>
    /// IConnectionCountGetter 获取当前已经存在的TCP连接数
    /// </summary>
    public interface IConnectionCountGetter
    {
        int GetConnectionCount();
    }

    public class EmptyConnectionCountGetter : IConnectionCountGetter
    {
        public int GetConnectionCount()
        {
            return 0;
        }
    }
}
