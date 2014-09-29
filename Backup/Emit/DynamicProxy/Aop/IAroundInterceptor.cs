using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Emit.DynamicProxy.Aop
{
    /// <summary>
    /// IAroundInterceptor 对方法进行Around截获处理。注意，必须要触发目标方法的调用。
    /// zhuweisky 2008.05.20
    /// </summary>
    public interface IAroundInterceptor
    {
        object AroundCall(InterceptedMethod method);
    }

    #region EmptyAroundInterceptor
    public sealed class EmptyAroundInterceptor : IAroundInterceptor
    {
        #region IAroundInterceptor 成员

        public object AroundCall(InterceptedMethod method)
        {
            return method.Invoke();
        }

        #endregion
    } 
    #endregion
}
