using System;
using System.Collections.Generic;
using System.Text;
using ESBasic.Logger;

namespace ESBasic.Emit.DynamicProxy
{
    /// <summary>
    /// IExceptionFilter 用于记录异常的详细信息
    /// </summary>
    public interface IExceptionFilter
    {
        //methodParaInfo参数给出了调用方法的各Parameters的值,如"<Parameters><age>21</age><name>Leo</name></Parameters>"       
        void Filter(Exception ee, string methodPath, string methodParaInfo);      
    }

    #region ExceptionLoggerFilter
    public class ExceptionLoggerFilter : IExceptionFilter
    {
        public ExceptionLoggerFilter() { }
        public ExceptionLoggerFilter(IAgileLogger logger)
            : this(logger, ErrorLevel.Standard)
        {

        }

        public ExceptionLoggerFilter(IAgileLogger logger, ErrorLevel _errorLevel)
        {
            this.agileLogger = logger;
            this.errorLevel = _errorLevel;
        }

        #region AgileLogger
        protected IAgileLogger agileLogger;
        public IAgileLogger AgileLogger
        {
            set { agileLogger = value; }
        }
        #endregion

        #region ErrorLevel
        protected ErrorLevel errorLevel = ErrorLevel.Standard;
        public ErrorLevel ErrorLevel
        {
            set { errorLevel = value; }
        }
        #endregion

        #region IExceptionFilter 成员

        public virtual void Filter(Exception ee, string methodPath, string methodParaInfo)
        {
            this.agileLogger.Log(ee, methodParaInfo + "@" + methodPath, this.errorLevel);
        }
        #endregion
    } 
    #endregion
}
