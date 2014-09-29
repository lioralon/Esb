using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Logger
{
    /// <summary>
    /// FileLogger ����־��¼���ı��ļ���FileLogger���̰߳�ȫ�ġ�
    /// </summary>
    public class FileLogger :ILogger
    {
        private StreamWriter writer;

        #region Ctor
        public FileLogger(string filePath)
        {
            if (!File.Exists(filePath))
            {
                FileStream fs = File.Create(filePath);
                fs.Close();
            }

            this.writer = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write, FileShare.Read));
        }

        ~FileLogger()
        {
            this.Close();
        }
        #endregion

        #region ILogger ��Ա

        #region Log
        public void Log(string msg)
        {
            if (!this.enabled)
            {
                return;
            }

            lock (this.writer)
            {
                this.writer.WriteLine(msg + "\n");
                this.writer.Flush();
            }
        } 
        #endregion

        #region LogWithTime
        public void LogWithTime(string msg)
        {
            string formatMsg = string.Format("{0}:{1}", DateTime.Now.ToString(), msg);
            this.Log(formatMsg);
        } 
        #endregion

        #region Close
        private void Close()
        {            
            if (this.writer != null)
            {
                try
                {
                    this.writer.Close();
                    this.writer = null;
                }
                catch { }
            }
        } 
        #endregion

        #region Enabled
        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        } 
        #endregion       

        #endregion

        #region IDisposable ��Ա

        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
