using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Emit.DynamicProxy.Aop
{
    /// <summary>
    /// IMethodInterceptor �Է������нػ񲢼���Ԥ����ͺ���
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
        #region IMethodInterceptor ��Ա

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
