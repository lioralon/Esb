using System;
using System.Collections.Generic;
using System.Text;
using ESBasic.Emit.DynamicProxy;
using System.Reflection;
using ESBasic.Emit.DynamicProxy.AopSimple;
using ESBasic.Emit.DynamicProxy.Aop;

namespace ESBasic.Emit.Application
{
    /// <summary>
    /// DynamicProxyFactory 用于创建各种类型动态代理的工厂。线程安全。
    /// zhuweisky 2008.03.01
    /// </summary>
    public static class DynamicProxyFactory
    {
        private static SimpleProxyEmitter SimpleProxyEmitter = new SimpleProxyEmitter();
        private static ExceptionFilterProxyEmitter ExceptionFilterProxyEmitter = new ExceptionFilterProxyEmitter();
        private static AopProxyEmitter AopProxyEmitter = new AopProxyEmitter();
        private static SimpleAopProxyEmitter SimpleAopProxyEmitter = new SimpleAopProxyEmitter();

        #region CreateSimpleProxy 让目标对象适配到TInterface接口，仅转发调用，不做任何额外处理
        public static TInterface CreateSimpleProxy<TInterface>(object origin)
        {
            lock (DynamicProxyFactory.SimpleProxyEmitter)
            {
                Type orignType = origin.GetType();
                Type dynamicType = DynamicProxyFactory.SimpleProxyEmitter.EmitProxyType<TInterface>(orignType);
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType });
                return (TInterface)ctor.Invoke(new object[] { origin });
            }
        }

        public static TInterface CreateSimpleProxy<TInterface>(TInterface origin)
        {
            lock (DynamicProxyFactory.SimpleProxyEmitter)
            {
                Type orignType = typeof(TInterface);
                Type dynamicType = DynamicProxyFactory.SimpleProxyEmitter.EmitProxyType<TInterface>(orignType);
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType });
                return (TInterface)ctor.Invoke(new object[] { origin });
            }
        }

        public static object CreateSimpleProxy(Type proxyIntfaceType, object origin)
        {
            lock (DynamicProxyFactory.SimpleProxyEmitter)
            {
                Type orignType = origin.GetType();
                Type dynamicType = DynamicProxyFactory.SimpleProxyEmitter.EmitProxyType(proxyIntfaceType ,orignType);
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType });
                return ctor.Invoke(new object[] { origin });
            }
        }
        #endregion

        #region CreateEFProxy 创建高效的截获异常并记录方法参数详细信息的动态代理。（直接调用被截获目标方法，而不是通过反射）
        /// <summary>
        /// CreateEFProxy 如果origin为透明代理，则原始的类型信息会丢失，该方法将抛出异常。该情况下请使用带强类型的origin参数的重载方法。        
        /// </summary>  
        public static TInterface CreateEFProxy<TInterface>(object origin, IExceptionFilter filter)
        {
            lock (DynamicProxyFactory.ExceptionFilterProxyEmitter)
            {
                Type orignType = origin.GetType();
                Type dynamicType = DynamicProxyFactory.ExceptionFilterProxyEmitter.EmitProxyType<TInterface>(orignType);
                //DynamicProxyFactory.ExceptionFilterProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IExceptionFilter) });
                return (TInterface)ctor.Invoke(new object[] { origin, filter });
            }
        }

        public static TInterface CreateEFProxy<TInterface>(TInterface origin, IExceptionFilter filter)
        {
            lock (DynamicProxyFactory.ExceptionFilterProxyEmitter)
            {
                Type orignType = typeof(TInterface);
                Type dynamicType = DynamicProxyFactory.ExceptionFilterProxyEmitter.EmitProxyType<TInterface>(orignType);
                //DynamicProxyFactory.ExceptionFilterProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IExceptionFilter) });
                return (TInterface)ctor.Invoke(new object[] { origin, filter });
            }
        }

        public static object CreateEFProxy(Type proxyIntfaceType , object origin, IExceptionFilter filter)
        {
            lock (DynamicProxyFactory.ExceptionFilterProxyEmitter)
            {
                Type orignType = origin.GetType();
                Type dynamicType = DynamicProxyFactory.ExceptionFilterProxyEmitter.EmitProxyType(proxyIntfaceType ,orignType);
                //DynamicProxyFactory.ExceptionFilterProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IExceptionFilter) });
                return ctor.Invoke(new object[] { origin, filter });
            }
        }
        #endregion

        #region CreateAopProxy 创建通用的AOP代理，通过反射调用被截获的目标方法，对性能有所影响
        public static TInterface CreateAopProxy<TInterface>(object origin, IMethodInterceptor methodInterceptor, IAroundInterceptor aroundInterceptor)
        {
            lock (DynamicProxyFactory.AopProxyEmitter)
            {
                Type orignType = origin.GetType();
                Type dynamicType = DynamicProxyFactory.AopProxyEmitter.EmitProxyType<TInterface>(orignType);
                //DynamicProxyFactory.AopProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IMethodInterceptor), typeof(IAroundInterceptor) });
                return (TInterface)ctor.Invoke(new object[] { origin, methodInterceptor, aroundInterceptor });
            }
        }

        public static TInterface CreateAopProxy<TInterface>(TInterface origin, IMethodInterceptor methodInterceptor, IAroundInterceptor aroundInterceptor)
        {
            lock (DynamicProxyFactory.AopProxyEmitter)
            {

                Type orignType = typeof(TInterface);
                Type dynamicType = DynamicProxyFactory.AopProxyEmitter.EmitProxyType<TInterface>(orignType);
                //DynamicProxyFactory.AopProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IMethodInterceptor), typeof(IAroundInterceptor) });
                return (TInterface)ctor.Invoke(new object[] { origin, methodInterceptor, aroundInterceptor });
            }
        }

        public static object CreateAopProxy(Type proxyIntfaceType, object origin, IMethodInterceptor methodInterceptor, IAroundInterceptor aroundInterceptor)
        {
            lock (DynamicProxyFactory.AopProxyEmitter)
            {
                Type orignType = origin.GetType();
                Type dynamicType = DynamicProxyFactory.AopProxyEmitter.EmitProxyType(proxyIntfaceType ,orignType);
                //DynamicProxyFactory.AopProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IMethodInterceptor), typeof(IAroundInterceptor) });
                return ctor.Invoke(new object[] { origin, methodInterceptor, aroundInterceptor });
            }
        }
        #endregion

        #region CreateSimpleAopProxy 创建通用的AOP代理，直接调用被截获的目标方法，性能较好。
        public static TInterface CreateSimpleAopProxy<TInterface>(object origin, IAopInterceptor aopInterceptor)
        {
            lock (DynamicProxyFactory.SimpleAopProxyEmitter)
            {
                Type orignType = origin.GetType();
                Type dynamicType = DynamicProxyFactory.SimpleAopProxyEmitter.EmitProxyType<TInterface>(orignType);
                //DynamicProxyFactory.SimpleAopProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IAopInterceptor) });
                return (TInterface)ctor.Invoke(new object[] { origin, aopInterceptor});
            }
        }

        public static TInterface CreateSimpleAopProxy<TInterface>(TInterface origin, IAopInterceptor aopInterceptor)
        {
            lock (DynamicProxyFactory.SimpleAopProxyEmitter)
            {
                Type orignType = typeof(TInterface);
                Type dynamicType = DynamicProxyFactory.SimpleAopProxyEmitter.EmitProxyType<TInterface>(orignType);
                //DynamicProxyFactory.SimpleAopProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IAopInterceptor) });
                return (TInterface)ctor.Invoke(new object[] { origin, aopInterceptor });
            }
        }

        public static object CreateSimpleAopProxy(Type proxyIntfaceType, object origin, IAopInterceptor aopInterceptor)
        {
            lock (DynamicProxyFactory.SimpleAopProxyEmitter)
            {
                Type orignType = origin.GetType();
                Type dynamicType = DynamicProxyFactory.SimpleAopProxyEmitter.EmitProxyType(proxyIntfaceType ,orignType);
                //DynamicProxyFactory.SimpleAopProxyEmitter.Save();
                ConstructorInfo ctor = dynamicType.GetConstructor(new Type[] { orignType, typeof(IAopInterceptor) });
                return ctor.Invoke(new object[] { origin, aopInterceptor });
            }
        }
        #endregion
    }
}
