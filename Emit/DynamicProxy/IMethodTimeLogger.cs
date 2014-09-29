using System;
using System.Collections.Generic;
using System.Text;
using ESBasic.Logger;

namespace ESBasic.Emit.DynamicProxy
{
    /// <summary>
    /// IMethodTimeLogger 该接口用于记录MethodTimeInterceptor捕捉到的方法的执行时间。
    /// zhuweisky 2008.06.13
    /// </summary>
    public interface IMethodTimeLogger
    {
        void Log(string detailMethodPath, double millisecondsConsumed);
    }

    #region MethodTimeLogger
    public class MethodTimeLogger : IMethodTimeLogger
    {
        #region AgileLogger
        private IAgileLogger agileLogger;
        public IAgileLogger AgileLogger
        {
            set { agileLogger = value; }
        }
        #endregion

        #region MinSpanInMSecsToLog
        private int minSpanInMSecsToLog = 0;
        public int MinSpanInMSecsToLog
        {
            set { minSpanInMSecsToLog = value; }
        }
        #endregion

        #region ctor
        public MethodTimeLogger() { }
        public MethodTimeLogger(IAgileLogger logger, int _minSpanInMSecsToLog)
        {
            this.agileLogger = logger;
            this.minSpanInMSecsToLog = _minSpanInMSecsToLog;
        }
        #endregion

        #region IMethodTimeLogger 成员

        public void Log(string detailMethodPath, double millisecondsConsumed)
        {
            if (millisecondsConsumed >= this.minSpanInMSecsToLog)
            {
                string msg = string.Format("{0}方法耗时:{1}ms", detailMethodPath, millisecondsConsumed);
                this.agileLogger.LogWithTime(msg);
            }
        }

        #endregion
    } 
    #endregion
}
