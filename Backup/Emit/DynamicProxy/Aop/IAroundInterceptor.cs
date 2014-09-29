using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Emit.DynamicProxy.Aop
{
    /// <summary>
    /// IAroundInterceptor �Է�������Around�ػ���ע�⣬����Ҫ����Ŀ�귽���ĵ��á�
    /// zhuweisky 2008.05.20
    /// </summary>
    public interface IAroundInterceptor
    {
        object AroundCall(InterceptedMethod method);
    }

    #region EmptyAroundInterceptor
    public sealed class EmptyAroundInterceptor : IAroundInterceptor
    {
        #region IAroundInterceptor ��Ա

        public object AroundCall(InterceptedMethod method)
        {
            return method.Invoke();
        }

        #endregion
    } 
    #endregion
}
