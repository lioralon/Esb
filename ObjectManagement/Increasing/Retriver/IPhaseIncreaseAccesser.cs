using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.ObjectManagement.Increasing
{
    /// <summary>
    /// IPhaseIncreaseAccesser 用于从各个源访问每一阶段的增量数据。
    /// </summary>   
    public interface IPhaseIncreaseAccesser<TSourceToken, TRoundID, TKey, TObject>
    {
        /// <summary>
        /// GetMaxKeyOfPreviousRound 获取上一轮各个源中的数据的最大标志。
        /// </summary>       
        IDictionary<TSourceToken, TKey> GetMaxKeyOfPreviousRound();

        /// <summary>
        /// NextIsLastPhaseOfRound 下一增量是否为当前Round的最后一个Phase。如果是，则out出每个源的最后Phase的最大标志。
        /// </summary>               
        bool NextIsLastPhaseOfRound(out TRoundID currentRoundID, out IDictionary<TSourceToken, TKey> lastKeyOfRoundDic);

        /// <summary>
        /// GetMaxKey 获取指定源中的最大标志。
        /// </summary> 
        TKey GetMaxKey(TSourceToken token);      

        /// <summary>
        /// Retrieve 获取某一阶段的增量。maxKeyOfPrePhase 《 本阶段增量 《=  maxKeyOfThisPhase
        /// </summary>   
        IList<TObject> Retrieve(TSourceToken token, TKey maxKeyOfPrePhase, TKey maxKeyOfThisPhase);
    }
}
