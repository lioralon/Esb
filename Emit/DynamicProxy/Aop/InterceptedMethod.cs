using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ESBasic.Emit.DynamicProxy.Aop
{
    /// <summary>
    /// InterceptedMethod 封装被截获的方法的基本信息。
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
        /// Method 被截获的目标方法
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
        /// Target 被截获的方法需要在哪个对象上调用。
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
        /// Arguments 调用被截获的方法的参数
        /// </summary>
        public object[] Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        } 
        #endregion       

        #region Invoke
        /// <summary>
        /// Invoke 执行目标方法
        /// </summary>        
        public object Invoke()
        {           
            return this.method.Invoke(this.target, this.arguments); //发射调用，性能损失严重
        } 
        #endregion
    }
}
