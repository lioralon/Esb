using System;
using System.Collections.Generic;
using System.Text;
using ESBasic.Logger;

namespace ESBasic.Emit.DynamicProxy
{
    /// <summary>
    /// IExceptionFilter ���ڼ�¼�쳣����ϸ��Ϣ
    /// </summary>
    public interface IExceptionFilter
    {
        //methodParaInfo���������˵��÷����ĸ�Parameters��ֵ,��"<Parameters><age>21</age><name>Leo</name></Parameters>"       
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

        #region IExceptionFilter ��Ա

        public virtual void Filter(Exception ee, string methodPath, string methodParaInfo)
        {
            this.agileLogger.Log(ee, methodParaInfo + "@" + methodPath, this.errorLevel);
        }
        #endregion
    } 
    #endregion
}
