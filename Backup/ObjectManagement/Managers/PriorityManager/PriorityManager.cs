using System;
using System.Collections.Generic;
using System.Text;
using ESBasic.Threading.Synchronize;

namespace ESBasic.ObjectManagement.Managers
{
    /// <summary>
    /// PriorityManager 具有优先级的对象的管理器。该实现是线程安全的。
    /// </summary>
    /// <typeparam name="T">被管理的对象的类型，必须从IPriorityObject继承。</typeparam>
    /// 注意：在PriorityManager中，优先等级是用int表示的，是从0开始连续的一串整数，整数值越小，表明优先级越高。
    /// 当Initialize方法被执行后，优先等级的范围就被固定下来。比如PriorityLevelCount值设为4，则PriorityManager所支持的优先等级即为：0，1，2，3
    public class PriorityManager<T> where T : class ,IPriorityObject 
    {
        private ISamePriorityObjectManager<T>[] spObjectManagerAry = null;
        private SmartRWLocker smartRWLocker = new SmartRWLocker();

        #region Ctor
        public PriorityManager() { }
        public PriorityManager(int _priorityLevelCount)
        {
            this.PriorityLevelCount = _priorityLevelCount;
        } 
        #endregion

        #region PriorityLevelCount
        private int priorityLevelCount = 1;
        /// <summary>
        /// PriorityLevelCount 优先级分为几个等级。一旦Initialize执行完毕，该属性便不可以被修改。或者说，即使被修改，也不会产生任何效果。
        /// </summary>
        public int PriorityLevelCount
        {
            get { return priorityLevelCount; }
            set 
            {
                if (value < 1)
                {
                    throw new Exception("The value of PriorityLevelCount must be greater than 1.");
                }
                priorityLevelCount = value; 
            }
        } 
        #endregion

        #region Initialize
        public void Initialize()
        {
            this.spObjectManagerAry = new ISamePriorityObjectManager<T>[this.priorityLevelCount];
            for (int i = 0; i < this.spObjectManagerAry.Length; i++)
            {
                this.spObjectManagerAry[i] = new SamePriorityObjectManager<T>();
            }
        } 
        #endregion

        #region AddWaiter
        public void AddWaiter(T waiter)
        {
            if ((waiter.PriorityLevel < 0) || (waiter.PriorityLevel >= this.priorityLevelCount))
            {
                throw new Exception("Current PriorityManager instance don't support the PriorityLevel of the target.");
            }

            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                this.spObjectManagerAry[waiter.PriorityLevel].AddWaiter(waiter);
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
                    int count = 0;
                    for (int i = 0; i < this.spObjectManagerAry.Length; i++)
                    {
                        count += this.spObjectManagerAry[i].Count;
                    }

                    return count;
                }
            }
        } 
        #endregion

        #region GetNextWaiter
        /// <summary>
        /// GetNextWaiter 返回优先级别最高且等待时间最长的waiter。
        /// 注意，返回时并不会从等待列表中删除waiter。如果要删除某个等待者，请调用RemoveWaiter。
        /// </summary>       
        public T GetNextWaiter()
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                for (int i = 0; i < this.spObjectManagerAry.Length; i++)
                {
                    if (this.spObjectManagerAry[i].Count > 0)
                    {
                        return this.spObjectManagerAry[i].GetNextWaiter();
                    }
                }

                return null;
            }
        }
        #endregion

        #region GetWaitersByPriority
        public T[] GetWaitersByPriority()
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                if (this.Count == 0)
                {
                    return null;
                }

                T[] resultAry = new T[this.Count];
                int startIndex = 0;
                for (int i = 0; i < this.spObjectManagerAry.Length; i++)
                {
                    if (this.spObjectManagerAry[i].Count > 0)
                    {
                        T[] temp = this.spObjectManagerAry[i].GetWaitersByPriority();
                        for (int index = 0; index < temp.Length; index++)
                        {
                            resultAry[startIndex++] = temp[index];
                        }
                    }
                }

                return resultAry;
            }
        } 
        #endregion

        #region RemoveWaiter
        public void RemoveWaiter(T waiter)
        {
            if ((waiter.PriorityLevel < 0) || (waiter.PriorityLevel >= this.priorityLevelCount))
            {
                return;
            }

            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                this.spObjectManagerAry[waiter.PriorityLevel].RemoveWaiter(waiter);
            }
        } 
        #endregion

        #region Clear
        public void Clear()
        {
            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                for (int i = 0; i < this.spObjectManagerAry.Length; i++)
                {
                    this.spObjectManagerAry[i].Clear();
                }
            }
        }
        
        #endregion

        #region Contains
        public bool Contains(T waiter)
        {
            if ((waiter.PriorityLevel < 0) || (waiter.PriorityLevel >= this.priorityLevelCount))
            {
                return false;
            }

            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                return this.spObjectManagerAry[waiter.PriorityLevel].Contains(waiter);
            }
        } 
        #endregion
    }
}
