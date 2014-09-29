using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.ObjectManagement.Integration
{
    /// <summary>
    /// ScatteredSegmentPicker 片段整合提取器。
    /// ScatteredSegmentPicker将多个片段（ISegment）整合为一个有序的整体，然后可以提取该整体中任意的一有序块（该有序块可能横跨多个ISegment）。
    /// zhuweisky 2009.04.02
    /// </summary>  
    /// <typeparam name="TSegmentID">片段标志的类型</typeparam>
    /// <typeparam name="TVal">构成片段的元素的类型</typeparam>
    public class ScatteredSegmentPicker<TSegmentID, TVal>
    {
        #region Ctor
        public ScatteredSegmentPicker() { }
        public ScatteredSegmentPicker(ISegmentContainer<TSegmentID, TVal> container, bool fromSmallToBig)
        {
            this.segmentContainer = container;
            this.pickFromSmallToBig = fromSmallToBig;
        } 
        #endregion
        
        #region PickFromSmallToBig
        private bool pickFromSmallToBig = true;
        /// <summary>
        /// PickFromSmallToBig 提取的顺序，从小到大或反之。
        /// </summary>
        public bool PickFromSmallToBig
        {
            get { return pickFromSmallToBig; }
            set { pickFromSmallToBig = value; }
        }
        #endregion

        #region SegmentContainer
        private ISegmentContainer<TSegmentID, TVal> segmentContainer;
        public ISegmentContainer<TSegmentID, TVal> SegmentContainer
        {
            set { segmentContainer = value; }
        } 
        #endregion

        #region Pick
        /// <summary>
        /// Pick 从整合后的整体中提取有序块。
        /// </summary>
        /// <param name="startIndex">目标块在整合后的整体中的起始位置</param>
        /// <param name="pickCount">提取元素的个数</param>
        /// <returns>有序的列表（从小到大或从大到小，与PickFromSmallToBig一致）</returns> 
        public IList<TVal> Pick(int startIndex, int pickCount)
        {
            if (startIndex < 0)
            {
                throw new Exception("startIndex must greater than 0 !");
            }

            if (pickCount <= 0)
            {
                return new List<TVal>();
            }

            return this.DoPick(startIndex, pickCount);
        } 
        #endregion

        #region DoPick
        private IList<TVal> DoPick(int startIndex, int pickCount)
        {
            IList<TVal> resultList = new List<TVal>();

            int accumulateIndex = 0;
            bool startPointFound = false;
            int havePickedCount = 0;
            ISegment<TSegmentID, TVal> curSegment = null;
            if (this.pickFromSmallToBig)
            {
                curSegment = this.segmentContainer.GetSmallestSegment();
            }
            else
            {
                curSegment = this.segmentContainer.GetBiggestSegment();
            }

            while (curSegment != null)
            {
                bool startPointInCurSegment = false;
                IList<TVal> curContent = curSegment.GetContent();

                #region Process
                if ((curContent != null) && (curContent.Count > 0))
                {
                    if (!startPointFound)
                    {
                        if (accumulateIndex + curContent.Count >= startIndex)
                        {
                            startPointFound = true;
                            startPointInCurSegment = true;
                        }
                        else
                        {
                            accumulateIndex += curContent.Count;
                        }
                    }

                    if (startPointFound)
                    {
                        if (this.pickFromSmallToBig)
                        {
                            int offset = startPointInCurSegment ? startIndex - accumulateIndex : 0;
                            for (int i = offset; i < curContent.Count; i++)
                            {
                                resultList.Add(curContent[i]);
                                ++havePickedCount;
                                if (havePickedCount >= pickCount)
                                {
                                    return resultList;
                                }
                            }
                        }
                        else
                        {
                            int offset = startPointInCurSegment ? (curContent.Count - 1) - (startIndex - accumulateIndex) : curContent.Count - 1;

                            for (int i = offset; i >= 0; i--)
                            {
                                resultList.Add(curContent[i]);
                                ++havePickedCount;
                                if (havePickedCount >= pickCount)
                                {
                                    return resultList;
                                }
                            }
                        }
                    }
                } 
                #endregion

                curSegment = this.segmentContainer.GetNextSegment(curSegment.ID, this.pickFromSmallToBig);
            }

            return resultList;
        }
        #endregion
    }
}
