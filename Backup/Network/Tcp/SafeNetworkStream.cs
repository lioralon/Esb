using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Network.Tcp
{
    public sealed class SafeNetworkStream : ISafeNetworkStream
    {
        private NetworkStream stream = null;
        private object lockForRead = new object();
        private object lockForWrite = new object();

        public SafeNetworkStream(NetworkStream netStream)
        {
            this.stream = netStream;
        }

        #region ISafeNetworkStream 成员

        #region Write ,Read
        public void Write(byte[] buffer, int offset, int size)
        {
            lock (this.lockForWrite)
            {
                this.stream.Write(buffer, offset, size);
            }
        }

        public int Read(byte[] buffer, int offset, int size)
        {
            lock (this.lockForRead)
            {
                return this.stream.Read(buffer, offset, size);
            }
        }

        public void BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback ,object state)
        {
            this.stream.BeginWrite(buffer, offset, size, callback ,state);
        }
        #endregion

        #region Flush ,Close
        public void Flush()
        {
            this.stream.Flush();
        }

        public void Close()
        {
            this.stream.Close();
        }
        #endregion

        #region property
        public NetworkStream NetworkStream
        {
            get
            {
                return this.stream;
            }
        }

        public bool DataAvailable
        {
            get
            {
                return this.stream.DataAvailable;
            }
        }
        #endregion

        #endregion

        public override int GetHashCode()
        {
            return this.stream.GetHashCode();
        }

        #region IDisposable 成员

        public void Dispose()
        {
            this.stream.Dispose();
        }

        #endregion
    }    
}
