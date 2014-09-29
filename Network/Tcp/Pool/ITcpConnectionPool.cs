using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Network.Tcp
{
    /// <summary>
    /// ITcpConnectionPool Tcp���ӳء�
    /// </summary>
    public interface ITcpConnectionPool :IDisposable
    {
        int MaxCount { get;set; }
        int ConnectionCount { get; }
        AgileIPEndPoint ServerIPE { set; }

        /// <summary>
        /// RentTcpStream ���û�п������ӣ��򷵻�null��
        /// </summary>       
        NetworkStream RentTcpStream();

        /// <summary>
        /// RentTcpStreamToSucceed ���û�п������ӣ�������һֱ�ȴ����п��е�����Ϊֹ����������������ã��򷵻�null ��
        /// </summary>       
        NetworkStream RentTcpStreamToSucceed();

        void GiveBackTcpStream(NetworkStream stream);
        void SetStreamDamaged(NetworkStream stream);
        void Clear();

        event CbSimpleInt ConnectionCountChanged; 	
    }
}
