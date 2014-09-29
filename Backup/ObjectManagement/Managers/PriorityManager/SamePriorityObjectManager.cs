using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ESBasic.Threading.Synchronize;

namespace ESBasic.ObjectManagement.Managers
{
    /// <summary>
    /// SamePriorityObjectManager 同一优先级对象管理器的参考实现。
    /// </summary>    
    public class SamePriorityObjectManager<T> : ISamePriorityObjectManager<T>
    {
        private LinkedList<T> waiterList = new LinkedList<T>();
        private SmartRWLocker smartRWLocker = new SmartRWLocker();

        #region AddWaiter
        public void AddWaiter(T waiter)
        {
            if (this.waiterList.Contains(waiter))
            {
                return;
            }

            using (this.smartRWLocker.Lock(AccessMode.Write))            
            {
                this.waiterList.AddLast(waiter);
            }
        } 
        #endregion

        #region Contains
        public bool Contains(T waiter)
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                return this.waiterList.Contains(waiter);
            }
        } 
        #endregion

        #region RemoveWaiter
        public void RemoveWaiter(T waiter)
        {
            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                if (!this.waiterList.Contains(waiter))
                {
                    return;
                }

                this.waiterList.Remove(waiter);
            }
        } 
        #endregion        

        #region GetNextWaiter
        public T GetNextWaiter()
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))           
            {
                if (this.waiterList.Count == 0)
                {
                    return default(T);
                }

                return this.waiterList.First.Value;    
            }
        } 
        #endregion

        #region GetWaitersByPriority
        public T[] GetWaitersByPriority()
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))           
            {
                T[] waiters = new T[this.Count];
                if (waiters.Length == 0)
                {
                    return waiters;
                }

                LinkedListNode<T> firstNode = this.waiterList.First;
                waiters[0] = firstNode.Value;

                LinkedListNode<T> temp = firstNode;
                for (int i = 1; i < waiters.Length; i++)
                {
                    temp = temp.Next;
                    waiters[i] = temp.Value;
                }

                return waiters;
            }
        } 
        #endregion      

        #region Clear
        public void Clear()
        {
            using (this.smartRWLocker.Lock(AccessMode.Write))           
            {
                this.waiterList.Clear();
            }
        } 
        #endregion

        #region Count
        public int Count
        {
            get
            {
                using (this.smartRWLocker.Lock(AccessMode.Read))
                {
                    return this.waiterList.Count;
                }
            }
        } 
        #endregion       
    }
}
