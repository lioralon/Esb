using System;
using System.Collections.Generic;
using System.Text;
using ESBasic.Helpers;

namespace ESBasic.ObjectManagement.Managers
{
    public class ObjectManager<TPKey, TObject> : IObjectManager<TPKey, TObject> 
    {
        protected IDictionary<TPKey, TObject> objectDictionary = new Dictionary<TPKey, TObject>();
        protected object locker = new object();

        public event CbGeneric<TObject> ObjectRegistered;
        public event CbGeneric<TObject> ObjectUnregistered;
       
        public ObjectManager()
        {
            this.ObjectRegistered += delegate { };
            this.ObjectUnregistered += delegate { };
        }

        #region IObjectManager<TObject,TPKey> ≥…‘±

        public int Count
        {
            get
            {
                return this.objectDictionary.Count;
            }
        }

        public virtual void Add(TPKey key, TObject obj)
        {
            lock (this.locker)
            {
                if (this.objectDictionary.ContainsKey(key))
                {
                    this.objectDictionary.Remove(key);
                }

                this.objectDictionary.Add(key, obj);
            }

            this.ObjectRegistered(obj);
        }

        public virtual void Remove(TPKey id)
        {
            TObject target = default(TObject);
            lock (this.locker)
            {
                if (this.objectDictionary.ContainsKey(id))
                {
                    target = this.objectDictionary[id];
                    this.objectDictionary.Remove(id);
                }
            }

            if (target != null)
            {
                this.ObjectUnregistered(target);
            }
        }

        public virtual void Clear()
        {
            lock (this.locker)
            {
                this.objectDictionary.Clear();
            }
        }

        public TObject Get(TPKey id)
        {
            lock (this.locker)
            {
                if (this.objectDictionary.ContainsKey(id))
                {
                    return this.objectDictionary[id];
                }
            }

            return default(TObject);
        }

        public bool Contains(TPKey id)
        {
            lock (this.locker)
            {
                return this.objectDictionary.ContainsKey(id);               
            }
        }

        public IList<TObject> GetAll()
        {
            lock (this.locker)
            {
                return ESBasic.Collections.CollectionConverter.CopyAllToList<TObject>(this.objectDictionary.Values);
            }
        }

        public IList<TPKey> GetKeyList()
        {
            lock (this.locker)
            {
                return ESBasic.Collections.CollectionConverter.CopyAllToList<TPKey>(this.objectDictionary.Keys);
            }
        }

        public IList<TPKey> GetKeyListByObj(TObject obj)
        {
            lock (this.locker)
            {
                IList<TPKey> list = new List<TPKey>();
                foreach (TPKey key in this.GetKeyList())
                {
                    if (this.objectDictionary[key].Equals(obj))
                    {
                        list.Add(key);
                    }
                }

                return list;
            }
        }

        #endregion
    }
}
