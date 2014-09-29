using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.ObjectManagement.Managers
{
    /// <summary>
    /// IObjectManager 用于管理具有唯一标志的对象，该接口实现必须保证线程安全。
    /// zhuweisky 2008.05.31
    /// </summary>   
    public interface IObjectManager<TPKey, TObject>
    {
        event CbGeneric<TObject> ObjectRegistered;
        event CbGeneric<TObject> ObjectUnregistered;

        int Count { get; }

        /// <summary>
        /// Add 如果已经存在同ID的对象，则用新对象替换旧对象。
        /// </summary>     
        void Add(TPKey key, TObject obj);

        void Remove(TPKey id);
        void Clear();
        bool Contains(TPKey id);

        /// <summary>
        /// Get 如果不存在，则返回default（TObject）。
        /// </summary>        
        TObject Get(TPKey id);

        IList<TObject> GetAll();
        IList<TPKey> GetKeyList();
        IList<TPKey> GetKeyListByObj(TObject obj);
    }   
}
