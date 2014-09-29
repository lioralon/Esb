using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ESBasic.Emit.DynamicProxy.Aop
{
    /// <summary>
    /// InterceptedMethod ��װ���ػ�ķ����Ļ�����Ϣ��
    /// zhuweisky 2008.05.20
    /// </summary>
    public sealed class InterceptedMethod
    {
        #region Ctor
        public InterceptedMethod() { }
        public InterceptedMethod(object _target, MethodInfo _method, object[] paras)
        {
            this.target = _target;
            this.method = _method;
            this.arguments = paras;
        } 
        #endregion

        #region Method
        private MethodInfo method;
        /// <summary>
        /// Method ���ػ��Ŀ�귽��
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
            set { method = value; }
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

        #region Invoke
        /// <summary>
        /// Invoke ִ��Ŀ�귽��
        /// </summary>        
        public object Invoke()
        {           
            return this.method.Invoke(this.target, this.arguments); //������ã�������ʧ����
        } 
        #endregion
    }
}
