using System;
using System.Net;

namespace ESBasic.Network
{
	/// <summary>
	/// AgileIPEndPoint ø…xml≈‰÷√µƒIPEndPoint°£
	/// </summary>
	public sealed class AgileIPEndPoint
	{
        public AgileIPEndPoint()
		{			
		}

        public AgileIPEndPoint(string ip ,int thePort)
        {
            this.iPAddress = ip;
            this.port = thePort;
        }

		#region IPAddress
		private string iPAddress = "" ; 
		public string IPAddress
		{
			get
			{
				return this.iPAddress ;
			}
			set
			{
				this.iPAddress = value ;
			}
		}
		#endregion
		
		#region Port
		private int port = 0 ; 
		public int Port
		{
			get
			{
				return this.port ;
			}
			set
			{
				this.port = value ;
			}
		}
		#endregion

        #region IPEndPoint
        public IPEndPoint IPEndPoint
        {
            get
            {
                return new IPEndPoint(System.Net.IPAddress.Parse(this.iPAddress), this.port);
            }
        } 
        #endregion
	}
}
