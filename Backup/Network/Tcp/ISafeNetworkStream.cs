using System;
using System.Threading ;
using System.Net;
using System.Net.Sockets ;

namespace ESBasic.Network.Tcp
{
	/// <summary>
	/// INetworkStreamSafe 线程安全的网络流 。保证任一时刻最多只有一个读/写动作发生。
    /// NetworkStream 自己就能保证同步！ ？
	/// 作者：朱伟 sky.zhuwei@163.com 
	/// 2005.04.22
	/// </summary>	
	public interface ISafeNetworkStream :IDisposable
	{		
		void Flush();
		void Close() ;

        void Write(byte[] buffer, int offset, int size);
        void BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state);
        int  Read (byte[] buffer, int offset, int size);

		bool DataAvailable{get ;} 	
		NetworkStream NetworkStream{get ;}
	}	
}
