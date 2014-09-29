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
    /// DynamicProxyFactory ���ڴ����������Ͷ�̬����Ĺ������̰߳�ȫ��
    /// zhuweisky 2008.03.01
    /// </summary>
    public static class DynamicProxyFactory
    {
        private static SimpleProxyEmitter SimpleProxyEmitter = new SimpleProxyEmitter();
        private static ExceptionFilterProxyEmitter ExceptionFilterProxyEmitter = new ExceptionFilterProxyEmitter();
        private static AopProxyEmitter AopProxyEmitter = new AopProxyEmitter();
        private static SimpleAopProxyEmitter SimpleAopProxyEmitter = new SimpleAopProxyEmitter();

        #region CreateSimpleProxy ��Ŀ��������䵽TInterface�ӿڣ���ת�����ã������κζ��⴦��
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

        #region CreateEFProxy ������Ч�Ľػ��쳣����¼����������ϸ��Ϣ�Ķ�̬������ֱ�ӵ��ñ��ػ�Ŀ�귽����������ͨ�����䣩
        /// <summary>
        /// CreateEFProxy ���originΪ͸��������ԭʼ��������Ϣ�ᶪʧ���÷������׳��쳣�����������ʹ�ô�ǿ���͵�origin���������ط�����        
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

        #region CreateAopProxy ����ͨ�õ�AOP����ͨ��������ñ��ػ��Ŀ�귽��������������Ӱ��
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

        #region CreateSimpleAopProxy ����ͨ�õ�AOP����ֱ�ӵ��ñ��ػ��Ŀ�귽�������ܽϺá�
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
