using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ESBasic.Emit.DynamicProxy.AopSimple
{
    /// <summary>
    /// InterceptedMethod ��װ���ػ�ķ����Ļ�����Ϣ��
    /// zhuweisky 2008.05.20
    /// </summary>
    public sealed class InterceptedMethod
    {
        #region Ctor
        public InterceptedMethod() { }
        public InterceptedMethod(object _target, string _method, object[] paras)
        {
            this.target = _target;
            this.methodName = _method;
            this.arguments = paras;
        } 
        #endregion

        #region MethodName
        private string methodName;
        /// <summary>
        /// MethodName ���ػ��Ŀ�귽��
        /// </summary>
        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }
        #endregion

        #region Target
        private object target;
        /// <summary>
        /// Target ���ػ�ķ�����Ҫ���ĸ������ϵ��á�
        /// </summary>
        public object Target
        {
            get { return target; }
            set { target = value; }
        } 
        #endregion

        #region Arguments
        private object[] arguments;
        /// <summary>
        /// Arguments ���ñ��ػ�ķ����Ĳ���
        /// </summary>
        public object[] Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        } 
        #endregion              
    }
}
