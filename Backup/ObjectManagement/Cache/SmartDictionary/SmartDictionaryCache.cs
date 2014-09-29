using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.ObjectManagement.Cache
{
    public class SmartDictionaryCache<Tkey, TVal> : ISmartDictionaryCache<Tkey ,TVal>
    {
        private IDictionary<Tkey, TVal> ditionary;
        private object locker = new object();

        #region ObjectRetriever
        private IObjectRetriever<Tkey, TVal> objectRetriever;
        public IObjectRetriever<Tkey, TVal> ObjectRetriever
        {
            set { objectRetriever = value; }
        } 
        #endregion

        #region Initialize
        public void Initialize()
        {
            this.ditionary = this.objectRetriever.RetrieveAll();
        } 
        #endregion

        #region Get
        public TVal Get(Tkey id)
        {
            if (id == null)
            {
                return default(TVal);
            }

            lock (this.locker)
            {
                if (this.ditionary.ContainsKey(id))
                {
                    return this.ditionary[id];
                }

                TVal val = this.objectRetriever.Retrieve(id);
                if (val != null)
                {
                    this.ditionary.Add(id, val);
                }

                return val;
            }
        } 
        #endregion    

        #region Clear
        public void Clear()
        {
            lock (this.locker)
            {
                this.ditionary.Clear();
            }
        } 
        #endregion

        #region GetAllValListCopy
        public IList<TVal> GetAllValListCopy()
        {
            lock (this.locker)
            {
                return ESBasic.Collections.CollectionConverter.CopyAllToList<TVal>(this.ditionary.Values);
            }
        } 
        #endregion

        #region GetAllKeyListCopy
        public IList<Tkey> GetAllKeyListCopy()
        {
            lock (this.locker)
            {
                return ESBasic.Collections.CollectionConverter.CopyAllToList<Tkey>(this.ditionary.Keys);
            }
        } 
        #endregion

        #region HaveContained
        public bool HaveContained(Tkey id)
        {
            if (id == null)
            {
                return false;
            }

            return this.ditionary.ContainsKey(id);//不会与写线程冲突。
        } 
        #endregion

        #region Count
        public int Count
        {
            get
            {
                return this.ditionary.Count;
            }
        } 
        #endregion
    }
}
