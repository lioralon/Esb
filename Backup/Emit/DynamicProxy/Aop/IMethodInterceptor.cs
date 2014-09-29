using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Emit.DynamicProxy.Aop
{
    /// <summary>
    /// IMethodInterceptor 对方法进行截获并加入预处理和后处理。
    /// zhuweisky 2008.05.20
    /// </summary>
    public interface IMethodInterceptor
    {
        void PreProcess(InterceptedMethod method);

        void PostProcess(InterceptedMethod method ,object returnVal);
    }

    #region EmptyMethodInterceptor
    public sealed class EmptyMethodInterceptor : IMethodInterceptor
    {
        #region IMethodInterceptor 成员

        public void PreProcess(InterceptedMethod method)
        {

        }

        public void PostProcess(InterceptedMethod method, object returnVal)
        {

        }

        #endregion
    } 
    #endregion
}
