using System;
using System.Collections.Generic;

namespace ESBasic.ObjectManagement.Managers
{
    /// <summary>
    /// ISamePriorityObjectManager 同一优先级对象管理器，用于管理同一优先级的所有对象，这些对象将按照先来后到的顺序确定其优先顺序（“第二优先级”）。
    /// 该接口的实现必须保证是线程安全的。
    /// zhuweisky 2007.04.16 /2008.08.13修订
    /// </summary>
    /// <typeparam name="T">要被管理的对象的类型</typeparam>
    public interface ISamePriorityObjectManager<T>
    {   
        /// <summary>
        /// AddWaiter 添加一个等待者。如果等待者在管理器中已经存在，则直接返回。
        /// </summary>       
        void AddWaiter(T waiter);

        /// <summary>
        /// Count 当前管理器中等待者的数量。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// GetNextWaiter 返回等待时间最长的waiter。
        /// 注意，返回时并不会从等待列表中删除waiter。如果要删除某个等待者，请调用RemoveWaiter。
        /// </summary>       
        T GetNextWaiter();

        /// <summary>
        /// GetWaitersByPriority 按照等待者加入的先后顺序返回等待者数组，数组中index越小的等待者其等待时间越长，其优先级也越高。
        /// </summary>       
        T[] GetWaitersByPriority();

        /// <summary>
        /// RemoveWaiter 从管理器中移除指定的等待者。
        /// </summary>        
        void RemoveWaiter(T waiter);

        /// <summary>
        /// Clear 清空管理器中的所有等待者。
        /// </summary>
        void Clear();

        /// <summary>
        /// Contains 管理器中是否存在指定的等待者。
        /// </summary>       
        bool Contains(T waiter);
    }
}
