using System;
using System.Threading ;
using System.Net ;
using System.Net.Sockets ;
using ESBasic;

namespace ESBasic.Network.Tcp
{
	/// <summary>
	/// IXTcpListener 用于封装TCP监听者及监听线程。
	/// 作者：朱伟 sky.zhuwei@163.com 
	/// 2005.05.23
	/// </summary>
	public interface IAgileTcpListener
	{
        /// <summary>
        /// Start 开始监听
        /// </summary>
		void Start() ; 

        /// <summary>
        /// Stop 停止监听
        /// </summary>
		void Stop() ;

        bool IsListening { get; }

        /// <summary>
        /// TcpConnectionEstablished 当新的Tcp连接成功建立时，会触发此事件	
        /// </summary>
        event CbStrongNetworkStream TcpConnectionEstablished;

        /// <summary>
        /// IdleSpanInMSec 连接检测时间间隔，单位为ms。指示当没有连接请求时，间隔多久再次检测。
        /// </summary>
        int IdleSpanInMSec { set; }

        /// <summary>
        /// MaxConnectionCount 允许的最大连接数
        /// </summary>
        int MaxConnectionCount { get;set; }

        IConnectionCountGetter ConnectionCountGetter { set; }
	}

    public delegate void CbStrongNetworkStream(NetworkStream stream, EndPoint romoteEP);   
	
}
