using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Network.Tcp
{
    /// <summary>
    /// ITcpConnectionPool Tcp连接池。
    /// </summary>
    public interface ITcpConnectionPool :IDisposable
    {
        int MaxCount { get;set; }
        int ConnectionCount { get; }
        AgileIPEndPoint ServerIPE { set; }

        /// <summary>
        /// RentTcpStream 如果没有可用连接，则返回null。
        /// </summary>       
        NetworkStream RentTcpStream();

        /// <summary>
        /// RentTcpStreamToSucceed 如果没有可用连接，则阻塞一直等待到有空闲的连接为止。如果服务器不可用，则返回null 。
        /// </summary>       
        NetworkStream RentTcpStreamToSucceed();

        void GiveBackTcpStream(NetworkStream stream);
        void SetStreamDamaged(NetworkStream stream);
        void Clear();

        event CbSimpleInt ConnectionCountChanged; 	
    }
}
