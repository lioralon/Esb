using System;
using System.Threading ;
using System.Net;
using System.Net.Sockets ;

namespace ESBasic.Network.Tcp
{
	/// <summary>
	/// INetworkStreamSafe �̰߳�ȫ�������� ����֤��һʱ�����ֻ��һ����/д����������
    /// NetworkStream �Լ����ܱ�֤ͬ���� ��
	/// ���ߣ���ΰ sky.zhuwei@163.com 
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
