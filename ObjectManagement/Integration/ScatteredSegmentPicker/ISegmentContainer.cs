using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.ObjectManagement.Integration
{
    /// <summary>
    /// ISegmentContainer 用于存放片段ISegment的容器。
    /// </summary>
    /// <typeparam name="TSegmentID">片段标志的类型</typeparam>
    /// <typeparam name="TVal">构成片段的元素的类型</typeparam>
    public interface ISegmentContainer<TSegmentID, TVal>
    {
        ISegment<TSegmentID, TVal> GetSmallestSegment();
        ISegment<TSegmentID, TVal> GetBiggestSegment();

        /// <summary>
        /// GetNextSegment 按照fromSmallToBig指定的顺序返回下一个Segment。
        /// 如果返回null，则表示不再有后续的Segment了。
        /// </summary>      
        ISegment<TSegmentID, TVal> GetNextSegment(TSegmentID curSegmentID, bool fromSmallToBig);
    }
}
