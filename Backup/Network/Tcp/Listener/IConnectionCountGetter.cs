using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Network.Tcp
{
    /// <summary>
    /// IConnectionCountGetter ��ȡ��ǰ�Ѿ����ڵ�TCP������
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
