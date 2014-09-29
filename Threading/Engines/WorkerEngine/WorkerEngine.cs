using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ESBasic.Threading.Engines
{
    public class WorkerEngine<T> : IEngineActor, IWorkerEngine<T>
    {
        private AgileCycleEngine[] agileCycleEngines;
        private Queue<T> queueOfWork = new Queue<T>() ;
        private object lockerOfQueue = new object();

        #region Property
        #region MaxWaitWorkCount
        private int maxWaitWorkCount = 0;
        public int MaxWaitWorkCount
        {
            get { return maxWaitWorkCount; }
        }
        #endregion

        #region WorkProcesser
        protected IWorkProcesser<T> workProcesser;
        public IWorkProcesser<T> WorkProcesser
        {
            set { workProcesser = value; }
        }
        #endregion

        #region WorkerThreadCount
        private int workerThreadCount = 1;
        public int WorkerThreadCount
        {
            get { return workerThreadCount; }
            set
            {
                if (workerThreadCount < 1)
                {
                    throw new Exception("The number of worker must be > 0 !");
                }
                workerThreadCount = value;
            }
        }
        #endregion

        #region WorkCount
        public int WorkCount
        {
            get
            {
                return this.queueOfWork.Count;
            }
        }
        #endregion

        #region IdleSpanInMSecs
        private int idleSpanInMSecs = 10;
        public int IdleSpanInMSecs
        {
            get { return idleSpanInMSecs; }
            set { idleSpanInMSecs = value; }
        }
        #endregion 
        #endregion

        #region Initialize
        public void Initialize()
        {
            this.agileCycleEngines = new AgileCycleEngine[this.workerThreadCount];
            for (int i = 0; i < this.agileCycleEngines.Length; i++)
            {
                this.agileCycleEngines[i] = new AgileCycleEngine(this);
                this.agileCycleEngines[i].DetectSpanInSecs = 0;
            }
        } 
        #endregion

        #region Start
        public void Start()
        {
            foreach (AgileCycleEngine engine in this.agileCycleEngines)
            {
                engine.Start();
            }
        } 
        #endregion

        #region Stop
        public void Stop()
        {
            foreach (AgileCycleEngine engine in this.agileCycleEngines)
            {
                engine.Stop();
            }
        } 
        #endregion

        #region AddWork
        public void AddWork(T work)
        {
            lock (this.lockerOfQueue)
            {
                this.queueOfWork.Enqueue(work);
                if (this.queueOfWork.Count > this.maxWaitWorkCount)
                {
                    this.maxWaitWorkCount = this.queueOfWork.Count;
                }
            }
        } 
        #endregion       

        #region DoWork
        private void DoWork()
        {
            #region Get Work
            T work = default(T);
            bool haveWork = false;
            lock (this.lockerOfQueue)
            {
                if (this.queueOfWork.Count > 0)
                {
                    work = this.queueOfWork.Dequeue();
                    haveWork = true;
                }
            }
            #endregion

            if (haveWork)
            {
                this.workProcesser.Process(work);
            }
            else
            {
                Thread.Sleep(this.idleSpanInMSecs);
            }
        } 
        #endregion

        #region IEngineActor ≥…‘±
        public bool EngineAction()
        {
            this.DoWork();
            return true;
        }
        #endregion
    }
}
