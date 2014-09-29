using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Emit.DynamicProxy
{
    /// <summary>
    /// IProxyEmitter 用于发射指定类型的动态代理类型。
    /// TInterface 不能是泛型接口，但是TInterface可以包括泛型方法(泛型参数可以有约束)。支持ref/out参数。 
    /// </summary>
    public interface IProxyEmitter
    {
        /// <summary>
        /// EmitProxyType 用于originType没有实现TInterface的情况。
        /// </summary>       
        Type EmitProxyType<TInterface>(Type originType);

        void Save();
    }
}
