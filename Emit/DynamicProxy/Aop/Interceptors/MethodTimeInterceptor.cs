using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using ESBasic.ObjectManagement.Pool;

namespace ESBasic.Emit.DynamicProxy.Aop
{
    /// <summary>
    /// MethodTimeInterceptor ��AroundInterceptor���ڼ��Ŀ�귽��ִ�е�ʱ�䡣
    /// zhuweisky 2008.06.13
    /// </summary>
    public sealed class MethodTimeInterceptor : IAroundInterceptor, IPooledObjectCreator<Stopwatch>
    {
        private IObjectPool<Stopwatch> stopwatchPool = new ObjectPool<Stopwatch>();

        #region MethodTimeLogger
        private IMethodTimeLogger methodTimeLogger;
        public IMethodTimeLogger MethodTimeLogger
        {
            set { methodTimeLogger = value; }
        } 
        #endregion

        #region Ctor
        public MethodTimeInterceptor() { }
        public MethodTimeInterceptor(IMethodTimeLogger logger)
        {
            this.methodTimeLogger = logger;
            this.stopwatchPool.MinObjectCount = 1;
            this.stopwatchPool.PooledObjectCreator = this;
            this.stopwatchPool.Initialize();
        } 
        #endregion

        #region IAroundInterceptor ��Ա
        public object AroundCall(InterceptedMethod method)
        {
            object result = null;

            Stopwatch stopwatch = this.stopwatchPool.Rent();
            try
            {
                stopwatch.Start();
                result = method.Invoke();

                string methodPath = method.Method.DeclaringType.ToString() + "." + method.Method.Name;
                this.methodTimeLogger.Log(methodPath, stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
            }
            finally
            {
                this.stopwatchPool.GiveBack(stopwatch);
            }

            return result;
        }

        #endregion

        #region IPooledObjectCreator<Stopwatch> ��Ա

        public Stopwatch Create()
        {
            return new Stopwatch();
        }

        public void Reset(Stopwatch obj)
        {
            obj.Reset();
        }

        #endregion
    }
}
