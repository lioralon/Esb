using System;
using System.Runtime.InteropServices ;
using System.Collections.Generic ;
using System.Net ;
using System.Net.Sockets ;
using System.IO ;
using ESBasic.Helpers ;
using ESBasic.Network.Tcp;

namespace ESBasic.Network
{
	/// <summary>
	/// NetHelper ��
	/// </summary>
	public static class NetHelper
	{
		#region IsPublicIPAddress
		public static bool IsPublicIPAddress(string ip)
		{        
			if(ip.StartsWith("10.")) //A�� 10.0.0.0��10.255.255.255.255 
			{
				return false ;
			}

			if(ip.StartsWith("172."))//B�� 172.16.0.0��172.31.255.255 
			{
				if(ip.Substring(6 ,1) == ".")
				{
					int secPart = int.Parse(ip.Substring(4 ,2)) ;
					if((16 <= secPart) && (secPart <= 31) )
					{
						return false ;
					}
				}
			}

			if(ip.StartsWith("192.168."))//C�� 192.168.0.0��192.168.255.255 
			{
				return false ;
			}

			return true ;
		}
		#endregion		

		#region ReceiveData
		/// <summary>
        /// ReceiveData �������ȡָ�����ȵ�����
		/// </summary>	
		public static byte[] ReceiveData(NetworkStream stream ,int size)
		{
			byte[] result = new byte[size] ;

			NetHelper.ReceiveData(stream ,result ,0 ,size) ;

			return result ;
		}

		/// <summary>
        /// ReceiveData �������ȡָ�����ȵ����� �������buff��offset��
		/// </summary>	
		public static void ReceiveData(NetworkStream stream ,byte[] buff ,int offset ,int size)
		{			
			int readCount  = 0 ;
			int totalCount = 0 ;
			int curOffset = offset ;

			while(totalCount < size)
			{
				int exceptSize = size - totalCount ;	
				readCount = stream.Read(buff ,curOffset ,exceptSize) ;
				if(readCount == 0)
				{
					throw new IOException("NetworkStream Interruptted !") ;
				}
				curOffset  += readCount ;
				totalCount += readCount ;
			}			
		}

        /// <summary>
        /// ReceiveData �������ȡָ�����ȵ�����
        /// </summary>	
		public static byte[] ReceiveData(ISafeNetworkStream stream ,int size)
		{
			byte[] result = new byte[size] ;

			NetHelper.ReceiveData(stream ,result ,0 ,size) ;

			return result ;
		}

		/// <summary>
        /// ReceiveData �������ȡָ�����ȵ����� �������buff��offset��
		/// </summary>		
        public static void ReceiveData(ISafeNetworkStream stream, byte[] buff, int offset, int size)
		{			
			int readCount  = 0 ;
			int totalCount = 0 ;
			int curOffset = offset ;

			while(totalCount < size)
			{
				int exceptSize = size - totalCount ;	
				readCount = stream.Read(buff ,curOffset ,exceptSize) ;
				if(readCount == 0)
				{
					throw new IOException("NetworkStream Interruptted !") ;
				}
				curOffset += readCount ;
				totalCount += readCount ;
			}			
		}
		#endregion			

		#region GetRemotingHanler
		//ǰ�����Ѿ�ע����remotingͨ��
		public static object GetRemotingHanler(string channelTypeStr ,string ip ,int port ,string remotingServiceName ,Type destInterfaceType)
		{
			try
			{
				string remoteObjUri = string.Format("{0}://{1}:{2}/{3}" ,channelTypeStr ,ip ,port ,remotingServiceName) ;
				return Activator.GetObject(destInterfaceType ,remoteObjUri);
			}
			catch
			{
				return null ;
			}
		}
		#endregion

		#region GetLocalIp
        /// <summary>
        /// GetLocalIp ��ȡ������IP��ַ
        /// </summary>       
		public static IPAddress[] GetLocalIp()
		{
			string hostName = Dns.GetHostName() ;
			IPHostEntry hEntry = Dns.Resolve(hostName) ;

			return hEntry.AddressList ;
		}

        public static IPAddress GetFirstLocalIp()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hEntry = Dns.Resolve(hostName);

            return hEntry.AddressList[0];
        }

        /// <summary>
        /// GetLocalPublicIp ��ȡ�����Ĺ���IP��ַ
        /// </summary>       
		public static string GetLocalPublicIp()
		{
			IPAddress[] list = NetHelper.GetLocalIp();
			foreach(IPAddress ip in list)
			{
				if(NetHelper.IsPublicIPAddress(ip.ToString()))
				{
					return ip.ToString() ;
				}
			}

			return null ;
		}
		#endregion

		#region IsConnectedToInternet
        /// <summary>
        /// IsConnectedToInternet �����Ƿ�����
        /// </summary>       
		public static bool IsConnectedToInternet() 
		{ 
			int Desc=0; 
			return InternetGetConnectedState(Desc,0); 
		}

		[DllImport("wininet.dll")] 
		private extern static bool InternetGetConnectedState(int Description,int ReservedValue); 
		#endregion

		#region GetMacAddress ��ȡ����mac��ַ
        /// <summary>
        /// GetMacAddress ��ȡ��������������Mac��ַ
        /// </summary>       
		public static IList<string> GetMacAddress() 
		{ 
			return MachineHelper.GetMacAddress();
		}
		#endregion

		#region DownLoadFileFromUrl
        /// <summary>
        /// DownLoadFileFromUrl ��url�����ļ����ص�����
        /// </summary>       
		public static void DownLoadFileFromUrl(string url ,string saveFilePath)
		{
			FileStream fstream  = new FileStream(saveFilePath ,FileMode.Create ,FileAccess.Write);
			WebRequest wRequest =  WebRequest.Create(url);

			try
			{				
				WebResponse wResponse = wRequest.GetResponse();	
				int contentLength =(int)wResponse.ContentLength;				
												
				byte[] buffer = new byte[1024];
				int read_count = 0 ;
				int total_read_count = 0 ;
				bool complete = false;							
				
				while (!complete )
				{
					read_count = wResponse.GetResponseStream().Read(buffer,0,buffer.Length);
					
					if(read_count > 0)
					{						
						fstream.Write(buffer ,0 ,read_count) ;
						total_read_count += read_count ;								
					}	
					else
					{
						complete = true ;
					}
				}
				
				fstream.Flush() ;				
			}			
			finally
			{				
				fstream.Close() ;				
				wRequest = null;
			}
		}
		#endregion
	}	
}
