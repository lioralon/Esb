using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.ObjectManagement.Managers
{
    /// <summary>
    /// IPriorityManager 具有优先级的对象的管理器。
    /// </summary>
    /// <typeparam name="T">被管理的对象的类型，必须从IPriorityObject继承。</typeparam>
    public interface IPriorityManager<T> : ISamePriorityObjectManager<T> where T : class, IPriorityObject 
    {
        int PriorityLevelCount { get; set; }
    }
}
