using System;
using System.Threading ;
using System.Net ;
using System.Net.Sockets ;
using ESBasic;

namespace ESBasic.Network.Tcp
{
	/// <summary>
	/// IXTcpListener ���ڷ�װTCP�����߼������̡߳�
	/// ���ߣ���ΰ sky.zhuwei@163.com 
	/// 2005.05.23
	/// </summary>
	public interface IAgileTcpListener
	{
        /// <summary>
        /// Start ��ʼ����
        /// </summary>
		void Start() ; 

        /// <summary>
        /// Stop ֹͣ����
        /// </summary>
		void Stop() ;

        bool IsListening { get; }

        /// <summary>
        /// TcpConnectionEstablished ���µ�Tcp���ӳɹ�����ʱ���ᴥ�����¼�	
        /// </summary>
        event CbStrongNetworkStream TcpConnectionEstablished;

        /// <summary>
        /// IdleSpanInMSec ���Ӽ��ʱ��������λΪms��ָʾ��û����������ʱ���������ٴμ�⡣
        /// </summary>
        int IdleSpanInMSec { set; }

        /// <summary>
        /// MaxConnectionCount ��������������
        /// </summary>
        int MaxConnectionCount { get;set; }

        IConnectionCountGetter ConnectionCountGetter { set; }
	}

    public delegate void CbStrongNetworkStream(NetworkStream stream, EndPoint romoteEP);   
	
}
