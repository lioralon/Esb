using System;

namespace ESBasic.Network
{
	/// <summary>
	/// IBufferPool 内存缓冲池。
	/// </summary>
	public interface IBufferPool
	{
        /// <summary>
        /// RentBuffer 从缓冲池中租借一块大小至少为minSize的缓冲区
        /// </summary>       
        Buffer RentBuffer(int minSize);

        /// <summary>
        /// GivebackBuffer 将使用完毕的缓冲区归还给缓冲池
        /// </summary>      
		void GivebackBuffer(int buffID) ;
	}

    public class Buffer
    {
        #region Ctor
        public Buffer() { }
        public Buffer(int id, byte[] buff)
        {
            this.iD = id;
            this.data = buff;
        } 
        #endregion

        #region ID
        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        } 
        #endregion

        #region Data
        private byte[] data;
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        } 
        #endregion

        #region InUsing
        private bool inUsing = false;
        public bool InUsing
        {
            get { return inUsing; }
            set { inUsing = value; }
        } 
        #endregion
    }

	public sealed class SimpleBufferPool :IBufferPool
	{
		#region IBufferPool 成员

        public Buffer RentBuffer(int minSize)
		{
			byte[] data = new byte[minSize] ;
            return new Buffer(data.GetHashCode(), data);
		}

        public void GivebackBuffer(int buffID)
		{			
            //直接由GC回收
		}
		#endregion
	}
}
