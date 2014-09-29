using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Emit.DynamicProxy.AopSimple
{
    /// <summary>
    /// ExceptionInterceptor 用于截获并记录异常详细信息。
    /// </summary>
    public class ExceptionInterceptor : IAopInterceptor, IArounder
    {
        private IExceptionFilter exceptionFilter;

        public ExceptionInterceptor() { }
        public ExceptionInterceptor(IExceptionFilter filter)
        {
            this.exceptionFilter = filter;
        }

        #region IAopInterceptor 成员

        public void PreProcess(InterceptedMethod method)
        {
          
        }

        public void PostProcess(InterceptedMethod method, object returnVal)
        {
            
        }

        public IArounder NewArounder()
        {
            return this;
        }

        #endregion

        #region IArounder 成员

        public void BeginAround(InterceptedMethod method)
        {
           
        }

        public void EndAround(object returnVal)
        {
           
        }

        public void OnException(InterceptedMethod method ,Exception ee)
        {
            string methodPath = method.Target.GetType().ToString() + "." + method.MethodName;

            int index = 0;
            string methodParaInfo = "<Parameters>";
            foreach (object para in method.Arguments)
            {
                if (para != null)
                {
                     methodParaInfo += string.Format("<Para{0}>{1}</Para{0}>" ,index++ ,para);
                }
             }

            methodParaInfo += "</Parameters>";

            this.exceptionFilter.Filter(ee, methodPath, methodParaInfo);
        }

        #endregion
    }
}
