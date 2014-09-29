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
	/// NetHelper 。
	/// </summary>
	public static class NetHelper
	{
		#region IsPublicIPAddress
		public static bool IsPublicIPAddress(string ip)
		{        
			if(ip.StartsWith("10.")) //A类 10.0.0.0到10.255.255.255.255 
			{
				return false ;
			}

			if(ip.StartsWith("172."))//B类 172.16.0.0到172.31.255.255 
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

			if(ip.StartsWith("192.168."))//C类 192.168.0.0到192.168.255.255 
			{
				return false ;
			}

			return true ;
		}
		#endregion		

		#region ReceiveData
		/// <summary>
        /// ReceiveData 从网络读取指定长度的数据
		/// </summary>	
		public static byte[] ReceiveData(NetworkStream stream ,int size)
		{
			byte[] result = new byte[size] ;

			NetHelper.ReceiveData(stream ,result ,0 ,size) ;

			return result ;
		}

		/// <summary>
        /// ReceiveData 从网络读取指定长度的数据 ，存放在buff中offset处
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
        /// ReceiveData 从网络读取指定长度的数据
        /// </summary>	
		public static byte[] ReceiveData(ISafeNetworkStream stream ,int size)
		{
			byte[] result = new byte[size] ;

			NetHelper.ReceiveData(stream ,result ,0 ,size) ;

			return result ;
		}

		/// <summary>
        /// ReceiveData 从网络读取指定长度的数据 ，存放在buff中offset处
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
		//前提是已经注册了remoting通道
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
        /// GetLocalIp 获取本机的IP地址
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
        /// GetLocalPublicIp 获取本机的公网IP地址
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
        /// IsConnectedToInternet 机器是否联网
        /// </summary>       
		public static bool IsConnectedToInternet() 
		{ 
			int Desc=0; 
			return InternetGetConnectedState(Desc,0); 
		}

		[DllImport("wininet.dll")] 
		private extern static bool InternetGetConnectedState(int Description,int ReservedValue); 
		#endregion

		#region GetMacAddress 获取网卡mac地址
        /// <summary>
        /// GetMacAddress 获取本机所有网卡的Mac地址
        /// </summary>       
		public static IList<string> GetMacAddress() 
		{ 
			return MachineHelper.GetMacAddress();
		}
		#endregion

		#region DownLoadFileFromUrl
        /// <summary>
        /// DownLoadFileFromUrl 将url处的文件下载到本地
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
