using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Network.Tcp
{
    public class TcpConnectionPool : ITcpConnectionPool
    {
        private bool serverAvailable = true;
        public event CbSimpleInt ConnectionCountChanged;
        private IDictionary<NetworkStream, ConnectionState> dicConnections = new Dictionary<NetworkStream, ConnectionState>();

        public TcpConnectionPool()
        {
            this.ConnectionCountChanged += delegate { };
        }

        #region MaxCount
        private int maxCount = 100;
        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        } 
        #endregion

        #region ConnectionCount
        public int ConnectionCount
        {
            get
            {
                return this.dicConnections.Count;
            }
        } 
        #endregion

        #region ServerIPE
        private AgileIPEndPoint serverIPE;
        public AgileIPEndPoint ServerIPE
        {
            set { serverIPE = value; }
        } 
        #endregion

        #region RentTcpStream
        public NetworkStream RentTcpStream()
        {
            lock (this.dicConnections)
            {
                foreach (NetworkStream stream in this.dicConnections.Keys)
                {
                    if (this.dicConnections[stream] == ConnectionState.Idle)
                    {
                        this.dicConnections[stream] = ConnectionState.Busy;
                        return stream;
                    }
                }

                if (this.dicConnections.Count < this.maxCount)
                {
                    TcpClient client = new TcpClient();
                    client.Connect(this.serverIPE.IPEndPoint);
                    NetworkStream stream = null ;
                    try
                    {
                        stream = client.GetStream();
                        this.serverAvailable = true;
                    }
                    catch
                    {
                        this.serverAvailable = false;
                    }

                    if (stream != null)
                    {
                        this.dicConnections.Add(stream, ConnectionState.Busy);
                        this.ConnectionCountChanged(this.dicConnections.Count);
                    }
                    return stream;
                }
            }

            return null;
        } 
        #endregion

        #region RentTcpStreamToSucceed
        public NetworkStream RentTcpStreamToSucceed()
        {
            NetworkStream stream = this.RentTcpStream(); ;
            while ((stream == null) && (this.serverAvailable))
            {
                System.Threading.Thread.Sleep(20);
                stream = this.RentTcpStream();                
            }

            return stream;
        }  
        #endregion          

        #region GiveBackTcpStream
        public void GiveBackTcpStream(NetworkStream stream)
        {
            this.dicConnections[stream] = ConnectionState.Idle;
        } 
        #endregion

        #region SetStreamDamaged
        public void SetStreamDamaged(NetworkStream stream)
        {
            lock (this.dicConnections)
            {
                this.dicConnections.Remove(stream);
            }

            try
            {
                this.ConnectionCountChanged(this.dicConnections.Count);
                stream.Dispose();
            }
            catch { }
        } 
        #endregion

        #region Clear
        public void Clear()
        {
            lock (this.dicConnections)
            {
                foreach (NetworkStream stream in this.dicConnections.Keys)
                {
                    try
                    {
                        stream.Dispose();
                    }
                    catch { }
                }

                this.dicConnections.Clear();                
            }

            this.ConnectionCountChanged(this.dicConnections.Count);
        } 
        #endregion

        #region Dispose
        public void Dispose()
        {
            this.Clear();
        } 
        #endregion
    }

    public enum ConnectionState
    {
        Idle ,Busy
    }
}
