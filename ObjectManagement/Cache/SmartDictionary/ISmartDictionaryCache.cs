using System;
using System.Collections.Generic;

namespace ESBasic.ObjectManagement.Cache
{
    /// <summary>
    /// ISmartDictionaryCache 能自动提取缓存中不存在的object的智能缓存。
    /// 该接口的实现必须是线程安全的。
    /// </summary>   
    public interface ISmartDictionaryCache<Tkey ,TVal>
    {
        IObjectRetriever<Tkey, TVal> ObjectRetriever { set; }
        int Count { get; }


        void Initialize();        

        /// <summary>
        /// Get 如果缓存中不存在id对应的object，则采用ObjectRetriever提取一次，如果仍然提取不到则返回null。
        /// </summary>       
        TVal Get(Tkey id);

        void Clear();

        /// <summary>
        /// HaveContained 当前容器是否已经存在目标对象。
        /// </summary>       
        bool HaveContained(Tkey id);                 

        IList<TVal> GetAllValListCopy();

        IList<Tkey> GetAllKeyListCopy();
    }
}
