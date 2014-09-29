using System;
using System.Collections.Generic;
using System.Text;
using ESBasic.Threading.Synchronize;

namespace ESBasic.Collections
{
    /// <summary>
    /// SortedArray 有序的数组，SortedArray 中的元素是不允许重复的。如果添加数组中已经存在的元素，将会被忽略。
    /// </summary>
    [Serializable]
    public class SortedArray<T> where T :IComparable
    {
        private int minCapacityForShrink = 32;
        private T[] array = new T[32] ;
        private int validCount = 0;
        private SmartRWLocker smartRWLocker = new SmartRWLocker();

        #region Ctor
        public SortedArray() { }
        public SortedArray(int capacity)
        {
            if (capacity > 0)
            {
                this.array = new T[capacity];               
            }
        }

        public SortedArray(ICollection<T> collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return;
            }

            this.array = new T[collection.Count];

            collection.CopyTo(this.array, 0);

            Array.Sort(this.array);
        }
        #endregion

        #region Property     

        #region Count
        public int Count
        {
            get
            {
                return this.validCount;
            }
        }
        #endregion

        #region Capacity
        private int Capacity
        {
            get
            {
                return this.array.Length;
            }
        }
        #endregion

        #region LastReadTime
        public DateTime LastReadTime
        {
            get { return this.smartRWLocker.LastRequireReadTime; }
        }
        #endregion

        #region LastWriteTime
        public DateTime LastWriteTime
        {
            get { return this.smartRWLocker.LastRequireWriteTime; }
        }
        #endregion 
        #endregion

        #region Contains
        public bool Contains(T t)
        {
            return this.IndexOf(t) >= 0;
        } 
        #endregion

        #region Index
        public T this[int index]
        {
            get
            {
                using (this.smartRWLocker.Lock(AccessMode.Read))
                {
                    if (index < 0 || (index >= this.validCount))
                    {
                        throw new Exception("Index out of the range !");
                    }

                    return this.array[index];
                }
            }
        } 
        #endregion

        #region IndexOf
        public int IndexOf(T t)
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                if (this.validCount == 0)
                {
                    return -1;
                }

                int index = Array.BinarySearch<T>(this.array, 0, this.validCount, t);

                return (index < 0) ? -1 : index;
            }
        } 
        #endregion

        #region Add
        public void Add(T t)
        {
            int posIndex = 0;
            this.Add(t, out posIndex);
        }

        /// <summary>
        /// Add 将一个元素添加到数组中。如果数组中不存在目标元素，则返回true。如果已存在，则返回false。无论哪种情况，posIndex都会被赋予正确的值。
        /// </summary>        
        public void Add(T t, out int posIndex)
        {
            if (t == null)
            {
                throw new Exception("Target can't be null !");
            }

            using (this.smartRWLocker.Lock(AccessMode.Write))
            {                
                int index = Array.BinarySearch<T>(this.array, 0, this.validCount, t);
                if (index >= 0)
                {
                    posIndex = index;
                    return ;
                }

                this.AdjustCapacity(1);
                posIndex = ~index;
                Array.Copy(this.array, posIndex, this.array, posIndex + 1, this.validCount - posIndex);
                this.array[posIndex] = t;

                ++this.validCount;               
            }
        }
     
        public void Add(ICollection<T> collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return;
            }

            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                ICollection<T> resultCollection = collection;

                #region checkRepeat
                Dictionary<T, T> dic = new Dictionary<T, T>();
                foreach (T t in collection)
                {
                    if (dic.ContainsKey(t) || this.Contains(t))
                    {
                        continue;
                    }

                    dic.Add(t, t);
                }

                resultCollection = dic.Keys;
                #endregion

                if (resultCollection.Count == 0)
                {
                    return;
                }

                this.AdjustCapacity(resultCollection.Count);

                foreach (T t in resultCollection)
                {                    
                    this.array[this.validCount] = t;
                    ++this.validCount;
                }

                Array.Sort<T>(this.array, 0, this.validCount);
            }
        }
        #endregion

        #region Remove
        #region Remove
        /// <summary>
        /// Remove 删除数组中所有值为t的元素。
        /// </summary>      
        public void Remove(T t)
        {
            if (t == null)
            {
                return;
            }

            int index = -1;
            do
            {
                index = this.IndexOf(t);
                if (index >= 0)
                {
                    this.RemoveAt(index);
                }

            } while (index >= 0);

        } 
        #endregion

        #region RemoveAt
        public void RemoveAt(int index)
        {
            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                if (index < 0 || (index >= this.validCount))
                {
                    return;
                }

                Array.Copy(this.array, index + 1, this.array, index, this.validCount - index);
                --this.validCount;
            }
        } 
        #endregion

        #region RemoveBetween
        public void RemoveBetween(int minIndex, int maxIndex)
        {
            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                minIndex = minIndex < 0 ? 0 : minIndex;
                maxIndex = maxIndex >= this.validCount ? this.validCount - 1 : maxIndex;

                if (maxIndex < minIndex)
                {
                    return;
                }

                Array.Copy(this.array, maxIndex + 1, this.array, minIndex, this.validCount - maxIndex - 1);

                this.validCount -= (maxIndex - minIndex + 1);
            }
        } 
        #endregion

        #endregion

        #region GetBetween
        public T[] GetBetween(int minIndex, int maxIndex)
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                minIndex = minIndex < 0 ? 0 : minIndex;
                maxIndex = maxIndex >= this.validCount ? this.validCount - 1 : maxIndex;

                if (maxIndex < minIndex)
                {
                    return new T[0];
                }

                int count = maxIndex - minIndex - 1;
                T[] result = new T[count];

                Array.Copy(this.array, minIndex, result, 0, count);
                return result;
            }
        } 
        #endregion

        #region Shrink
        /// <summary>
        /// Shrink 将内部数组收缩到最小，释放内存。
        /// </summary>
        public void Shrink()
        {
            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                if (this.array.Length == this.validCount)
                {
                    return;
                }


                int len = this.validCount >= this.minCapacityForShrink ? this.validCount : this.minCapacityForShrink;

                T[] newAry = new T[len];              

                Array.Copy(this.array,0, newAry,0, this.validCount);
                this.array = newAry;
            }
        } 
        #endregion

        #region AdjustCapacity
        private void AdjustCapacity(int newCount)
        {
            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                int totalCount = this.validCount + newCount;
                if (this.array.Length >= totalCount)
                {
                    return;
                }

                int newCapacity = this.array.Length;
                while (newCapacity < totalCount)
                {
                    newCapacity *= 2;
                }

                T[] newAry = new T[newCapacity];
                Array.Copy(this.array, 0, newAry, 0, this.validCount);
                this.array = newAry;
            }
        } 
        #endregion

        #region GetMax
        public T GetMax()
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                if (this.validCount == 0)
                {
                    throw new Exception("SortedArray is Empty !");
                }

                return this.array[this.validCount - 1];
            }
        }
        #endregion

        #region GetMin
        public T GetMin()
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                if (this.validCount == 0)
                {
                    throw new Exception("SortedArray is Empty !");
                }

                return this.array[0];
            }
        }
        #endregion

        #region GetAll
        public List<T> GetAll()
        {
            using (this.smartRWLocker.Lock(AccessMode.Read))
            {
                List<T> list = new List<T>();
                for (int i = 0; i < this.validCount; i++)
                {
                    list.Add(this.array[i]);
                }

                return list;
            }
        } 
        #endregion

        #region Clear
        public void Clear()
        {
            using (this.smartRWLocker.Lock(AccessMode.Write))
            {
                this.array = new T[this.minCapacityForShrink];              
                this.validCount = 0;
            }
        }
        #endregion
    }
}
