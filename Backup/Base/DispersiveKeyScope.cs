using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic
{  
    /// <summary>
    /// DispersiveKeyScope ���ڱ�ʾ���������Χ��һ����ɢ������ֵ
    /// </summary>
    public class DispersiveKeyScope
    {
        #region DispersiveKeyList
        private IList<int> dispersiveKeyList = new List<int>();
        public IList<int> DispersiveKeyList
        {
            set
            {
                this.dispersiveKeyList = value ?? new List<int>();
            }
        }
        #endregion

        #region DispersiveKeyScopeList Ԫ��ΪKeyScope
        private IList<KeyScope> dispersiveKeyScopeList = new List<KeyScope>();
        public IList<KeyScope> DispersiveKeyScopeList
        {
            set
            {
                if (value == null)
                {
                    this.dispersiveKeyScopeList = new List<KeyScope>();
                }
                else
                {
                    this.dispersiveKeyScopeList = value;
                }
            }
        }
        #endregion

        #region Contains
        public bool Contains(int val)
        {
            bool found = ESBasic.Collections.CollectionHelper.ContainsSpecification<int>(this.dispersiveKeyList, delegate(int key) { return key == val; });

            if (!found)
            {
                found = ESBasic.Collections.CollectionHelper.ContainsSpecification<KeyScope>(this.dispersiveKeyScopeList, delegate(KeyScope scope) { return scope.Contains(val); });               
            }

            return found;
        }
        #endregion
    }   
}
