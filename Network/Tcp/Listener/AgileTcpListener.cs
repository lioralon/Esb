using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ESBasic.Threading.Engines;

namespace ESBasic.Network.Tcp
{
    /// <summary>
    /// AgileTcpListener ��IAgileTcpListener��Ĭ��ʵ��
	/// </summary>
    public sealed class AgileTcpListener :BaseCycleEngine ,IAgileTcpListener
	{
		#region members
		private TcpListener tcpListener = null ;		
        public event CbStrongNetworkStream TcpConnectionEstablished;

        #region IsListening
        private bool isListening = false;
        public bool IsListening
        {
            get { return isListening; }
        } 
        #endregion

        #region MaxConnectionCount
        private int maxConnectionCount = 10000;
        public int MaxConnectionCount
        {
            get { return maxConnectionCount; }
            set { maxConnectionCount = value; }
        } 
        #endregion

        #region ConnectionCountGetter
        private IConnectionCountGetter connectionCountGetter = new EmptyConnectionCountGetter();
        public IConnectionCountGetter ConnectionCountGetter
        {
            set 
            {
                connectionCountGetter = value ?? new EmptyConnectionCountGetter();  
            }
        }        
        #endregion

        #region IdleSpanInMSec
        private int idleSpanInMSec = 10;
        public int IdleSpanInMSec
        {
            set { idleSpanInMSec = value; }
        } 
        #endregion        
		#endregion
		
		#region ctor
        public AgileTcpListener(int port) :this(port ,null)
		{
		}

        public AgileTcpListener(int port, string ip)
		{
            this.TcpConnectionEstablished += delegate { };

            if (ip != null)
            {
                this.tcpListener = new TcpListener(IPAddress.Parse(ip), port);//IPAddress.Any��ʾ�������е�������IP��ַ��
            }
            else
            {
                this.tcpListener = new TcpListener(IPAddress.Any ,port);
            }
		}
       
		#endregion

        #region IEsfTcpListener ��Ա
        #region Start , Stop
        public void Start()
		{	
			this.tcpListener.Start();
            this.isListening = true;
            base.Start();
		}

		public void Stop()
		{
            base.Stop();
			this.tcpListener.Stop();
            this.isListening = false;            
		}
		#endregion

        #region DoDetect
        protected override bool DoDetect()
        {
            #region maxConnectionCount
            bool accept = this.connectionCountGetter.GetConnectionCount() < this.maxConnectionCount;
            if (!accept)
            {
                if (this.isListening)
                {
                    this.tcpListener.Stop();
                    this.isListening = false;
                }

                return true;
            }
            else
            {
                if (!this.isListening)
                {
                    this.tcpListener.Start();
                    this.isListening = true;
                }
            }
            #endregion

            if (!this.tcpListener.Pending())
            {
                Thread.Sleep(this.idleSpanInMSec);
                return true;
            }

            #region old
            /*
            TcpClient tcp_client = this.tcpListener.AcceptTcpClient();
            EndPoint romoteEP = tcp_client.Client.RemoteEndPoint;
            this.TcpConnectionEstablished(tcp_client.GetStream(), romoteEP);
            return true;   
            */
            #endregion

            Socket sock = this.tcpListener.AcceptSocket();            
            // KeepAliveΪ20�룬�����Ϊ2�롣��������ͻ������ߣ�������Socket.Receive()����20����׳��쳣��
            int keepAlive = -1744830460; // SIO_KEEPALIVE_VALS
            byte[] inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 }; //True, 20 ��, 2 ��
            sock.IOControl(keepAlive, inValue, null);
            NetworkStream stream = new NetworkStream(sock) ;
            this.TcpConnectionEstablished(stream, sock.RemoteEndPoint);
           
            return true;           
        }	
		#endregion				

		#endregion

	}	
}
