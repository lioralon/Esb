using System;

namespace ESBasic.Network
{
	/// <summary>
	/// IBufferPool �ڴ滺��ء�
	/// </summary>
	public interface IBufferPool
	{
        /// <summary>
        /// RentBuffer �ӻ���������һ���С����ΪminSize�Ļ�����
        /// </summary>       
        Buffer RentBuffer(int minSize);

        /// <summary>
        /// GivebackBuffer ��ʹ����ϵĻ������黹�������
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
		#region IBufferPool ��Ա

        public Buffer RentBuffer(int minSize)
		{
			byte[] data = new byte[minSize] ;
            return new Buffer(data.GetHashCode(), data);
		}

        public void GivebackBuffer(int buffID)
		{			
            //ֱ����GC����
		}
		#endregion
	}
}
