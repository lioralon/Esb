using System;
using System.Net;
using System.Net.Sockets;

namespace ESBasic.Network.Tcp
{
	/// <summary>
    /// AgileTcpClient �����õ�TcpClient��
	/// </summary>
	public sealed class AgileTcpClient :IDisposable
	{
        TcpClient client = null;       

		public AgileTcpClient()
		{			
		}

        public AgileTcpClient(string _serverIp, int _serverPort) :this(_serverIp ,_serverPort ,0 ,null)
		{            
		}

        public AgileTcpClient(string _serverIp, int _serverPort, int iniLocalPort)
            : this(_serverIp, _serverPort, iniLocalPort ,null)
        {          
        }

        public AgileTcpClient(string _serverIp, int _serverPort, int iniLocalPort ,string localIP)
        {
            this.serverIP = _serverIp;
            this.serverPort = _serverPort;
            this.port = iniLocalPort;
            this.localIPAddress = localIP;
        }

		#region ServerIP
		private string serverIP = "" ; 
		public string ServerIP
		{
			get
			{
				return this.serverIP ;
			}
			set
			{
				this.serverIP = value ;
			}
		}
		#endregion		
	
        #region ServerPort
        private int serverPort = 0; 
		public int ServerPort
		{
			get
			{
                return this.serverPort;
			}
			set
			{
                this.serverPort = value;
			}
		}
		#endregion

        #region Port
        private int port = 0;
        /// <summary>
        /// Port ͨ�Ų��õı��ض˿ڣ���ֵ������GetNetworkStream�����б��޸ġ�
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }
        #endregion        

        #region LocalIPAddress
        private string localIPAddress = null;
        /// <summary>
        /// LocalIPAddress ���ĸ�IP����TCP���ӡ���������ã���ѡ�õ�һ�������ĵ�ַ(��ʱ����ֵ��GetNetworkStream�����б��޸�)
        /// </summary>
        public string LocalIPAddress
        {
            get { return localIPAddress; }
            set { localIPAddress = value; }
        } 
        #endregion

        #region GetNetworkStream   
        public NetworkStream GetNetworkStream()
        {
            try
            {                
                if (this.port <= 0)
                {
                    this.client = new TcpClient();
                }
                else
                {
                    IPAddress address = NetHelper.GetFirstLocalIp();
                    if (this.localIPAddress != null)
                    {
                        address = IPAddress.Parse(this.localIPAddress);
                    }
                    this.localIPAddress = address.ToString();

                    this.client = new TcpClient(new IPEndPoint(address, this.port));
                    this.client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                }
               
                this.client.Connect(IPAddress.Parse(this.serverIP), this.serverPort);
                this.port = ((IPEndPoint)this.client.Client.LocalEndPoint).Port;
                //object rev = this.client.Client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer); //8k
                //object snd = this.client.Client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer); //8k
                //this.client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer ,0);
                NetworkStream stream = this.client.GetStream();

                return stream;
            }
            catch(Exception ee)
            {
                throw ee;
            }
        }
        #endregion

        #region IDisposable ��Ա

        public void Dispose()
        {
            if (this.client != null)
            {
                this.client.Close();
            }
        }

        #endregion
    }
}
