using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ESBasic.Threading.Synchronize
{
    /// <summary>
    /// LockingObject SmartRWLocker的Lock方法返回的锁对象。仅仅通过using来使用该对象，如：using(this.smartLocker.Lock(AccessMode.Read)){...}
    /// </summary>
    public class LockingObject : IDisposable
    {
        private ReaderWriterLock readerWriterLock;
        private AccessMode accessMode = AccessMode.Read;

        #region Ctor
        public LockingObject(ReaderWriterLock _lock, AccessMode _lockMode)
        {
            this.readerWriterLock = _lock;
            this.accessMode = _lockMode;

            if (this.accessMode == AccessMode.Read)
            {
                this.readerWriterLock.AcquireReaderLock(-1);
            }
            else
            {
                this.readerWriterLock.AcquireWriterLock(-1);
            }
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (this.accessMode == AccessMode.Read)
            {
                this.readerWriterLock.ReleaseReaderLock();
            }
            else
            {
                this.readerWriterLock.ReleaseWriterLock();
            }
        }

        #endregion
    }
}
