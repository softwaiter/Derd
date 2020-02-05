using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public class Model : ICloneable
    {
        private ConcurrentDictionary<string, Property> mProperties = new ConcurrentDictionary<string, Property>();
        private ConcurrentDictionary<int, string> mPropertyIndexes = new ConcurrentDictionary<int, string>();

        private ConcurrentDictionary<string, Property> mPrimaryKeys = new ConcurrentDictionary<string, Property>();
        private ConcurrentDictionary<int, string> mPrimaryKeyIndexes = new ConcurrentDictionary<int, string>();

        public string Path { get; set; }

        public string Name { get; set; }

        public string Table { get; set; }

        internal bool AddProperty(Property p)
        {
            if (mProperties.TryAdd(p.Name.ToLower(), p))
            {
                mPropertyIndexes.AddOrUpdate(mPropertyIndexes.Count, p.Name, (key, value) => {
                    return p.Name;
                });

                if (p.IsPrimaryKey)
                {
                    mPrimaryKeys.AddOrUpdate(p.Name.ToLower(), p, (key, value) =>
                    {
                        return p;
                    });

                    mPrimaryKeyIndexes.AddOrUpdate(mPrimaryKeyIndexes.Count, p.Name, (key, value) =>
                    {
                        return p.Name;
                    });
                }
                return true;
            }
            return false;
        }

        public Property GetProperty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException(name);
            }

            Property result;
            if (mProperties.TryGetValue(name.ToLower(), out result))
            {
                return result;
            }

            throw new Exception(string.Concat("未找到Property：", name));
        }

        public Property GetProperty(int index)
        {
            if (mPropertyIndexes.ContainsKey(index))
            {
                string name = mPropertyIndexes[index];
                return GetProperty(name);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public int PropertyCount
        {
            get
            {
                return mProperties.Count;
            }
        }

        public Property GetPrimaryKey(int index)
        {
            if (mPrimaryKeyIndexes.ContainsKey(index))
            {
                string name = mPrimaryKeyIndexes[index];
                return GetProperty(name);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public int PrimaryKeyCount
        {
            get
            {
                return mPrimaryKeys.Count;
            }
        }

        public bool CreateTable(bool force = false)
        {
            return true;
        }

        public bool RemoveTable()
        {
            return true;
        }

        public bool TruncateTable()
        {
            return true;
        }

        public object Clone()
        {
            Model m = new Model();
            m.Name = this.Name;
            m.Table = this.Table;

            IEnumerator<KeyValuePair<string, Property>> e = this.mProperties.GetEnumerator();
            while (e.MoveNext())
            {
                Property newP = (Property)e.Current.Value.Clone();
                newP.Owner = m;
                m.AddProperty(newP);
            }

            return m;
        }
    }
}
