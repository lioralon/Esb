using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.ObjectManagement.Increasing
{
    /// <summary>
    /// BasePhaseIncreaseAccesser 当一个Round是由N天构成时，可以借助BasePhaseIncreaseAccesser简化IPhaseIncreaseAccesser的实现。
    /// </summary>    
    public abstract class BasePhaseIncreaseAccesser<TSourceToken, TKey, TObject> : IPhaseIncreaseAccesser<TSourceToken, int, TKey, TObject>
    {
        #region CurrentRoundStartTime
        private DateTime currentRoundStartTime = DateTime.Now;
        public DateTime CurrentRoundStartTime
        {
            get { return currentRoundStartTime; }
        } 
        #endregion

        #region DayOriginTime
        private ShortTime dayOriginTime = new ShortTime(0, 0, 0);
        public ShortTime DayOriginTime
        {
            get { return dayOriginTime; }
            set { dayOriginTime = value; }
        }
        #endregion        

        #region ManyDaysInOneRound
        private int manyDaysInOneRound = 1;
        public int ManyDaysInOneRound
        {
            get { return manyDaysInOneRound; }
            set
            {
                if (value < 1)
                {
                    throw new Exception("The value of Property [ManyDaysInOneRound] must be greater than 1.");
                }
                manyDaysInOneRound = value;
            }
        }
        #endregion 

        #region TodayIsFirstDay
        private bool todayIsFirstDay = false;
        /// <summary>
        /// TodayIsFirstDay 是将启动时刻作为当前Round的第一天还是最后一天？
        /// </summary>
        public bool TodayIsFirstDay
        {
            get { return todayIsFirstDay; }
            set { todayIsFirstDay = value; }
        } 
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize 如果派生类override该方法，则在实现时必须先调用base.Initialize()方法。
        /// </summary>
        public virtual void Initialize()
        {
            DateTime startTime = DateTime.Now;
            DateTime now = DateTime.Now;
            if (this.dayOriginTime.CompareTo(new ShortTime(now)) <= 0)
            {
                startTime = this.dayOriginTime.GetDateTime(now.Year, now.Month, now.Day);
            }
            else
            {
                DateTime yesterday = now.AddDays(-1);
                startTime = this.dayOriginTime.GetDateTime(yesterday.Year, yesterday.Month, yesterday.Day);
            }

            this.currentRoundStartTime = this.todayIsFirstDay ? startTime : startTime.AddDays(1 - this.manyDaysInOneRound);
        } 
        #endregion

        /// <summary>
        /// GetMaxKeyBefore 获取endTime之前的最大的Key。注意TimeColumn要小于endTime，不能等于。
        /// </summary>      
        protected abstract IDictionary<TSourceToken, TKey> GetMaxKeyBefore(DateTime endTime);

        public abstract TKey GetMaxKey(TSourceToken token);
        public abstract IList<TObject> Retrieve(TSourceToken token, TKey maxKeyOfPrePhase, TKey maxKeyOfThisPhase);

        #region IPhaseIncreaseAccesser<TSourceToken,int,TKey,TObject> 成员

        public IDictionary<TSourceToken, TKey> GetMaxKeyOfPreviousRound()
        {
            return this.GetMaxKeyBefore(this.currentRoundStartTime);
        }

        public bool NextIsLastPhaseOfRound(out int currentRoundID, out IDictionary<TSourceToken, TKey> lastKeyOfRoundDic)
        {
            currentRoundID = new Date(this.currentRoundStartTime).ToDateInteger();
            lastKeyOfRoundDic = null;
            DateTime now = DateTime.Now;

            TimeSpan span = now - this.currentRoundStartTime;
            bool isLastPhaseOfRound = span.TotalDays >= this.manyDaysInOneRound;

            if (isLastPhaseOfRound)
            {
                DateTime roundEndTime = this.dayOriginTime.GetDateTime(now.Year, now.Month, now.Day);
                lastKeyOfRoundDic = this.GetMaxKeyBefore(roundEndTime);

                this.currentRoundStartTime = roundEndTime; 
            }

            return isLastPhaseOfRound;
        }             

        #endregion
    }
}
