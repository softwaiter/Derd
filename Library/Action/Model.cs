using CodeM.Common.DbHelper;
using CodeM.Common.Orm.Action;
using CodeM.Common.Orm.Dialect;
using CodeM.Common.Tools;
using CodeM.Common.Tools.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CodeM.Common.Orm
{
    public enum AggregateType
    {
        NONE = 0,
        COUNT = 1,
        SUM = 2,
        DISTINCT = 3,
        MAX = 4,
        MIN = 5,
        AVG = 6
    }

    public enum FunctionType
    {
        NONE = 0,
        DATE = 1
    }

    public partial class Model : ISetValue, IGetValue, IPaging, ISort, ICommand, IAssist
    {
        internal class GetValueSetting
        {
            public GetValueSetting(string name)
            {
                this.Name = name;
            }

            public GetValueSetting(string name, AggregateType type)
                : this(name)
            {
                mOperations.Add(type);
            }

            public GetValueSetting(string name, AggregateType type, string alias)
                : this(name, type)
            {
                if (!string.IsNullOrWhiteSpace(alias))
                {
                    this.Alias = alias;
                }
            }

            public GetValueSetting(string name, AggregateType aggType,
                FunctionType funcType) : this(name, aggType)
            {
                mOperations.Add(funcType);
            }

            public GetValueSetting(string name, AggregateType aggType,
                FunctionType funcType, string alias) : this(name, aggType, alias)
            {
                mOperations.Add(funcType);
            }

            public GetValueSetting(string name, AggregateType aggType,
                AggregateType aggType2) : this(name, aggType)
            {
                mOperations.Add(aggType2);
            }

            public GetValueSetting(string name, AggregateType aggType,
                AggregateType aggType2, string alias) : this(name, aggType, alias)
            {
                mOperations.Add(aggType2);
            }

            public GetValueSetting(string name, FunctionType type)
                : this(name)
            {
                mOperations.Add(type);
            }

            public GetValueSetting(string name, FunctionType type, string alias)
                : this(name, type)
            {
                if (!string.IsNullOrWhiteSpace(alias))
                {
                    this.Alias = alias;
                }
            }

            public GetValueSetting(string name, FunctionType funcType, 
                AggregateType aggType) : this(name, funcType)
            {
                mOperations.Add(aggType);
            }

            public GetValueSetting(string name, FunctionType funcType,
                AggregateType aggType, string alias) : this(name, funcType, alias)
            {
                mOperations.Add(aggType);
            }

            public GetValueSetting(string name, FunctionType funcType,
                FunctionType funcType2) : this(name, funcType)
            {
                mOperations.Add(funcType2);
            }

            public GetValueSetting(string name, FunctionType funcType,
                FunctionType funcType2, string alias) : this(name, funcType, alias)
            {
                mOperations.Add(funcType2);
            }

            private List<dynamic> mOperations = new List<dynamic>();
            public List<dynamic> Operations
            {
                get
                {
                    return mOperations;
                }
            }

            public string Name { get; set; }

            public string Alias { get; set; } = null;

            /// <summary>
            /// 拼接SQL使用的名称
            /// </summary>
            public string FieldName
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(this.Alias))
                    {
                        return this.Alias;
                    }

                    if (this.Name.Contains("."))
                    {
                        return this.Name.Replace(".", "_");
                    }

                    return this.Name;
                }
            }

            /// <summary>
            /// 输出JSON对象使用的名称
            /// </summary>
            public string OutputName
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(this.Alias))
                    {
                        return this.Alias;
                    }
                    return this.Name;
                }
            }
        }

        internal class GroupBySetting
        {
            public GroupBySetting(string name)
            {
                this.Name = name;
            }

            public GroupBySetting(string name, FunctionType type)
                : this(name)
            {
                this.FunctionType = type;
            }

            public string Name { get; set; }

            public FunctionType FunctionType { get; set; } = FunctionType.NONE;
        }

        #region ISetValue
        dynamic mSetValues;

        internal bool TryGetValue(Property p, out object value, bool useDefaultValue = true)
        {
            if (mSetValues.Has(p.Name))
            {
                value = mSetValues[p.Name];
                return true;
            }
            else if (useDefaultValue)
            {
                try
                {
                    if (p.DefaultValue != null)
                    {
                        value = p.CalcDefaultValue(mSetValues);
                        return true;
                    }
                }
                catch
                {
                    ;
                }
            }

            value = null;
            return false;
        }

        private void _CheckPropertyType(Property p, object value)
        {
            if (value != null)
            {
                Type type = p.RealType;
                if (type == typeof(bool))
                {
                    bool result;
                    if (!bool.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：Boolean，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(byte))
                {
                    byte result;
                    if (!byte.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：Byte，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(sbyte))
                {
                    sbyte result;
                    if (!sbyte.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：SByte，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(decimal))
                {
                    decimal result;
                    if (!decimal.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：Decimal，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(double))
                {
                    double result;
                    if (!double.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：Double，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(Int16))
                {
                    Int16 result;
                    if (!Int16.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：Int16，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(Int32))
                {
                    Int32 result;
                    if (!Int32.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：Int32，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(Int64))
                {
                    Int64 result;
                    if (!Int64.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：Int64，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(Single))
                {
                    Single result;
                    if (!Single.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：Single，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(UInt16))
                {
                    UInt16 result;
                    if (!UInt16.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：UInt16，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(UInt32))
                {
                    UInt32 result;
                    if (!UInt32.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：UInt32，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(UInt64))
                {
                    UInt64 result;
                    if (!UInt64.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：UInt64，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(DateTime))
                {
                    DateTime result;
                    if (!DateTime.TryParse("" + value, out result))
                    {
                        throw new Exception(string.Concat(p.Name, "属性值期待类型：DateTime，实际类型为：", value.GetType().Name));
                    }
                }
            }
        }

        private void _CheckPropertyValue(Property p, object value)
        {
            if (value == null)
            {
                if (p.IsNotNull && !p.AutoIncrement &&
                    string.IsNullOrWhiteSpace(p.DefaultValue) &&
                    !p.NeedCalcBeforeSave)
                {
                    throw new Exception("属性值不允许为空：" + p.Name);
                }
            }
            else
            {
                if (p.RealType == typeof(string) && p.Length > 0)
                {
                    if (value.ToString().Length > p.Length)
                    {
                        throw new Exception(string.Concat(p.Name, "属性值最大长度不能超过", p.Length));
                    }
                }
                else if (FieldUtils.IsNumeric(p.FieldType))
                {
                    double dValue = double.Parse(value.ToString());
                    if (p.MinValue != null)
                    {
                        if (dValue < p.MinValue)
                        {
                            throw new Exception(string.Concat(p.Name, "属性值取值限制范围：", p.MinValue, "-", p.MaxValue));
                        }
                    }
                    if (p.MaxValue != null)
                    {
                        if (dValue > p.MaxValue)
                        {
                            throw new Exception(string.Concat(p.Name, "属性值取值限制范围：", p.MinValue, "-", p.MaxValue));
                        }
                    }
                }
            }
        }

        private void _CheckModelConstraint(string name, object value)
        {
            Property p = GetProperty(name);
            _CheckPropertyType(p, value);
            _CheckPropertyValue(p, value);
        }

        public Model SetValue(string name, object value, bool validate = false)
        {
            if (mSetValues == null)
            {
                mSetValues = new DynamicObjectExt();
            }

            if (validate)
            {
                _CheckModelConstraint(name, value);
            }
            mSetValues.SetValue(name, value);

            return this;
        }

        public Model SetValues(dynamic obj, bool validate = false)
        {
            if (obj != null)
            {
                for (int i = 0; i < PropertyCount; i++)
                {
                    Property p = GetProperty(i);
                    if (obj.Has(p.Name))
                    {
                        SetValue(p.Name, obj[p.Name], validate);
                    }
                }
            }
            return this;
        }
        #endregion

        #region IGetValue
        List<GetValueSetting> mGetValues = new List<GetValueSetting>();

        internal List<GetValueSetting> ReturnValues
        {
            get
            {
                return mGetValues;
            }
        }

        private string CheckGetValuePropName(string name)
        {
            string compactName = name.Trim();
            if (!compactName.Contains("."))
            {
                Property p = GetProperty(compactName);
                if (p == null)
                {
                    throw new Exception(string.Concat("未找到属性：", compactName));
                }
            }
            else
            {
                string[] typeItems = compactName.Split(".");
                Model currM = this;
                for (int i = 0; i < typeItems.Length; i++)
                {
                    Property p = currM.GetProperty(typeItems[i]);
                    if (p == null)
                    {
                        throw new Exception(string.Concat("未找到属性：", compactName));
                    }

                    if (i < typeItems.Length - 1)
                    {
                        currM = ModelUtils.GetModel(p.TypeValue);
                        if (currM == null)
                        {
                            throw new Exception(string.Concat("非法的Model引用：", p.TypeValue));
                        }
                    }
                }
            }
            return compactName;
        }

        public Model GetValue(AggregateType aggType, string name, string alias = null)
        {
            string compactName = CheckGetValuePropName(name);
            mGetValues.Add(new GetValueSetting(compactName, aggType, alias));
            return this;
        }

        public Model GetValue(AggregateType aggType, FunctionType funcType, string name, string alias = null)
        {
            string compactName = CheckGetValuePropName(name);
            mGetValues.Add(new GetValueSetting(compactName, aggType, funcType, alias));
            return this;
        }

        public Model GetValue(AggregateType aggType, AggregateType aggType2, string name, string alias = null)
        {
            string compactName = CheckGetValuePropName(name);
            mGetValues.Add(new GetValueSetting(compactName, aggType, aggType2, alias));
            return this;
        }

        public Model GetValue(FunctionType funcType, string name, string alias = null)
        {
            string compactName = CheckGetValuePropName(name);
            mGetValues.Add(new GetValueSetting(compactName, funcType, alias));
            return this;
        }

        public Model GetValue(FunctionType funcType, AggregateType aggType, string name, string alias = null)
        {
            string compactName = CheckGetValuePropName(name);
            mGetValues.Add(new GetValueSetting(compactName, funcType, aggType, alias));
            return this;
        }

        public Model GetValue(FunctionType funcType, FunctionType funcType2, string name, string alias = null)
        {
            string compactName = CheckGetValuePropName(name);
            mGetValues.Add(new GetValueSetting(compactName, funcType, funcType2, alias));
            return this;
        }

        public Model GetValue(params string[] names)
        {
            foreach (string name in names)
            {
                GetValue(AggregateType.NONE, name);
            }
            return this;
        }
        #endregion

        #region IFilter

        private SubFilter mFilter = new SubFilter();

        internal SubFilter Where
        {
            get
            {
                return mFilter;
            }
        }

        public Model And(IFilter subCondition)
        {
            mFilter.And(subCondition);
            return this;
        }

        public Model Or(IFilter subCondition)
        {
            mFilter.Or(subCondition);
            return this;
        }

        public Model Equals(string name, object value)
        {
            mFilter.Equals(name, value);
            return this;
        }

        public Model NotEquals(string name, object value)
        {
            mFilter.NotEquals(name, value);
            return this;
        }

        public Model Gt(string name, object value)
        {
            mFilter.Gt(name, value);
            return this;
        }

        public Model Gte(string name, object value)
        {
            mFilter.Gte(name, value);
            return this;
        }

        public Model Lt(string name, object value)
        {
            mFilter.Lt(name, value);
            return this;
        }

        public Model Lte(string name, object value)
        {
            mFilter.Lte(name, value);
            return this;
        }

        public Model Like(string name, string value)
        {
            mFilter.Like(name, value);
            return this;
        }

        public Model NotLike(string name, string value)
        {
            mFilter.NotLike(name, value);
            return this;
        }

        public Model IsNull(string name)
        {
            mFilter.IsNull(name);
            return this;
        }

        public Model IsNotNull(string name)
        {
            mFilter.IsNotNull(name);
            return this;
        }

        public Model Between(string name, object value, object value2)
        {
            mFilter.Between(name, value, value2);
            return this;
        }

        public Model In(string name, params object[] values)
        {
            mFilter.In(name, values);
            return this;
        }

        public Model NotIn(string name, params object[] values)
        {
            mFilter.NotIn(name, values);
            return this;
        }

        #endregion

        #region IPaging

        private bool mUsePaging = false;
        private int mPageSize = 100;
        private int mPageIndex = 1;

        internal bool IsUsePaging 
        {
            get
            {
                return mUsePaging;
            }
        }

        internal int CurrPageSize 
        {
            get 
            {
                return mPageSize;
            }
        }

        internal int CurrPageIndex 
        {
            get
            {
                return mPageIndex;
            }
        }

        public Model PageSize(int size)
        {
            if (size < 1)
            {
                throw new Exception("分页大小至少为1。");
            }

            mPageSize = size;
            mUsePaging = true;
            return this;
        }

        public Model PageIndex(int index)
        {
            if (index < 1)
            {
                throw new Exception("最小页码从1开始。");
            }

            mPageIndex = index;
            mUsePaging = true;
            return this;
        }

        public Model Top(int num)
        {
            if (num < 1)
            {
                throw new Exception("最小返回数据数量至少为1。");
            }

            mPageSize = num;
            mPageIndex = 1;
            mUsePaging = true;
            return this;
        }

        #endregion

        #region ISort
        private List<string> mSorts = new List<string>();
        private List<string> mForeignSortNames = new List<string>();

        public Model AscendingSort(string name)
        {
            string[] quotes = Features.GetObjectQuotes(this);

            if (mGetValues.Exists(item => item.Alias == name))
            {
                mSorts.Add(string.Concat(quotes[0], name, quotes[1], " ASC"));
            }
            else
            {
                if (!name.Contains("."))
                {
                    Property p = GetProperty(name);
                    if (p == null)
                    {
                        throw new Exception(string.Concat("未找到属性：", name));
                    }
                    mSorts.Add(string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " ASC"));
                }
                else
                {
                    Model currM = this;
                    string[] subNames = name.Split(".");
                    for (int i = 0; i < subNames.Length; i++)
                    {
                        Property subProp = currM.GetProperty(subNames[0]);
                        Model subM = ModelUtils.GetModel(subProp.TypeValue);
                        currM = subM;

                        if (i == subNames.Length - 2)
                        {
                            Property lastProp = subM.GetProperty(subNames[i + 1]);
                            mSorts.Add(string.Concat(quotes[0], subM.Table, quotes[1], ".", quotes[0], lastProp.Field, quotes[1], " ASC"));
                            break;
                        }
                    }

                    mForeignSortNames.Add(name);
                }
            }
            return this;
        }

        public Model DescendingSort(string name)
        {
            string[] quotes = Features.GetObjectQuotes(this);

            if (mGetValues.Exists(item => item.Alias == name))
            {
                mSorts.Add(string.Concat(quotes[0], name, quotes[1], " DESC"));
            }
            else
            {

                if (!name.Contains("."))
                {
                    Property p = GetProperty(name);
                    if (p == null)
                    {
                        throw new Exception(string.Concat("未找到属性：", name));
                    }
                    mSorts.Add(string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " DESC"));
                }
                else
                {
                    Model currM = this;
                    string[] subNames = name.Split(".");
                    for (int i = 0; i < subNames.Length; i++)
                    {
                        Property subProp = currM.GetProperty(subNames[0]);
                        Model subM = ModelUtils.GetModel(subProp.TypeValue);
                        currM = subM;

                        if (i == subNames.Length - 2)
                        {
                            Property lastProp = subM.GetProperty(subNames[i + 1]);
                            mSorts.Add(string.Concat(quotes[0], subM.Table, quotes[1], ".", quotes[0], lastProp.Field, quotes[1], " DESC"));
                            break;
                        }
                    }

                    mForeignSortNames.Add(name);
                }
            }
            return this;
        }

        internal List<string> ForeignSortNames 
        {
            get
            {
                return mForeignSortNames;
            }
        }

        internal string Sort
        {
            get
            {
                return string.Join(",", mSorts);
            }
        }
        #endregion

        private void Reset()
        {
            mSetValues = null;
            mGetValues.Clear();
            mFilter.Reset();
            mSorts.Clear();
            mGroupByNames.Clear();
            mForeignSortNames.Clear();

            mUsePaging = false;
            mPageSize = 100;
            mPageIndex = 1;

            mIsSelectForUpdate = false;
        }

        #region IAssist
        bool mIsSelectForUpdate = false;
        public Model SelectForUpdate()
        {
            if (Features.IsSupportSelectForUpdate(this))
            {
                mIsSelectForUpdate = true;
            }
            return this;
        }

        public bool IsSelectForUpdate
        {
            get
            {
                return mIsSelectForUpdate;
            }
        }

        bool mIsNoWait = false;
        public Model NoWait()
        {
            mIsNoWait = true;
            return this;
        }

        public bool IsNoWait
        {
            get
            {
                return mIsNoWait;
            }
        }

        List<GroupBySetting> mGroupByNames = new List<GroupBySetting>();

        internal List<GroupBySetting> GroupByNames
        {
            get
            {
                return mGroupByNames;
            }
        }

        public Model GroupBy(params string[] names)
        {
            foreach (string name in names)
            {
                string compactName = name.Trim();
                if (!mGroupByNames.Exists(item => item.Name == compactName && item.FunctionType == FunctionType.NONE))
                {
                    if (!compactName.Contains("."))
                    {
                        Property p = GetProperty(compactName);
                        if (p == null)
                        {
                            throw new Exception(string.Concat("未找到属性：", compactName));
                        }
                    }
                    else
                    {
                        string[] typeItems = compactName.Split(".");
                        Model currM = this;
                        for (int i = 0; i < typeItems.Length; i++)
                        {
                            Property p = currM.GetProperty(typeItems[i]);
                            if (p == null)
                            {
                                throw new Exception(string.Concat("未找到属性：", compactName));
                            }

                            if (i < typeItems.Length - 1)
                            {
                                currM = ModelUtils.GetModel(p.TypeValue);
                                if (currM == null)
                                {
                                    throw new Exception(string.Concat("非法的Model引用：", p.TypeValue));
                                }
                            }
                        }
                    }
                    mGroupByNames.Add(new GroupBySetting(compactName, FunctionType.NONE));
                }
            }
            return this;
        }

        public Model GroupBy(FunctionType funcType, string name)
        {
            string compactName = name.Trim();
            if (!mGroupByNames.Exists(item => item.Name == compactName && item.FunctionType == funcType))
            {
                if (!compactName.Contains("."))
                {
                    Property p = GetProperty(compactName);
                    if (p == null)
                    {
                        throw new Exception(string.Concat("未找到属性：", compactName));
                    }
                }
                else
                {
                    string[] typeItems = compactName.Split(".");
                    Model currM = this;
                    for (int i = 0; i < typeItems.Length; i++)
                    {
                        Property p = currM.GetProperty(typeItems[i]);
                        if (p == null)
                        {
                            throw new Exception(string.Concat("未找到属性：", compactName));
                        }

                        if (i < typeItems.Length - 1)
                        {
                            currM = ModelUtils.GetModel(p.TypeValue);
                            if (currM == null)
                            {
                                throw new Exception(string.Concat("非法的Model引用：", p.TypeValue));
                            }
                        }
                    }
                }
                mGroupByNames.Add(new GroupBySetting(compactName, funcType));
            }
            return this;
        }
        #endregion

        #region ICommand
        public void CreateTable(bool replace = false)
        {
            StringBuilder sb = new StringBuilder(ToString());
            if (replace)
            {
                RemoveTable();
            }

            OrmUtils.PrintSQL(sb.ToString());
            CommandUtils.ExecuteNonQuery(this, Path.ToLower(), sb.ToString());

            string tableIndexSQL = ToString(true);
            if (!string.IsNullOrWhiteSpace(tableIndexSQL))
            {
                OrmUtils.PrintSQL(tableIndexSQL);
                CommandUtils.ExecuteNonQuery(this, Path.ToLower(), tableIndexSQL);
            }

            if (Features.IsSupportAutoIncrement(this))
            {
                for (int i = 0; i < PropertyCount; i++)
                {
                    Property p = GetProperty(i);
                    if (p.AutoIncrement)
                    {
                        string[] aiCmds = Features.GetAutoIncrementExtCommand(this, Table, p.Field);
                        foreach (string cmd in aiCmds)
                        {
                            if (!string.IsNullOrEmpty(cmd))
                            {
                                OrmUtils.PrintSQL(cmd);
                                DbUtils.ExecuteNonQuery(Path.ToLower(), cmd);
                            }
                        }
                    }
                }
            }
        }

        public bool TryCreateTable(bool replace = false)
        {
            try
            {
                CreateTable(replace);
                return true;
            }
            catch
            {
                ;
            }
            return false;
        }

        public void RemoveTable(bool throwError = false)
        {
            try
            {
                string[] quotes = Features.GetObjectQuotes(this);

                string sql = string.Concat("DROP TABLE IF EXISTS ", quotes[0], Table, quotes[1]);
                if (!Features.IsSupportIfExists(this))
                {
                    sql = string.Concat("DROP TABLE ", quotes[0], Table, quotes[1]);
                }
                OrmUtils.PrintSQL(sql);
                CommandUtils.ExecuteNonQuery(this, Path.ToLower(), sql);
            }
            catch (Exception exp)
            {
                if (throwError)
                {
                    throw exp;
                }
            }

            try
            {
                for (int i = 0; i < PropertyCount; i++)
                {
                    Property p = GetProperty(i);
                    if (p.AutoIncrement)
                    {
                        string[] aiCmds = Features.GetAutoIncrementGCExtCommand(this, Table, p.Field);
                        foreach (string cmd in aiCmds)
                        {
                            if (!string.IsNullOrEmpty(cmd))
                            {
                                OrmUtils.PrintSQL(cmd);
                                DbUtils.ExecuteNonQuery(Path.ToLower(), cmd);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                if (throwError)
                {
                    throw exp;
                }
            }
        }

        public bool TryRemoveTable()
        {
            try
            {
                RemoveTable(true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void TruncateTable(bool throwError = false)
        {
            try
            {
                string[] quotes = Features.GetObjectQuotes(this);

                string sql = string.Concat("TRUNCATE TABLE ", quotes[0], Table, quotes[1]);
                if (!Features.IsSupportTruncate(this))
                {
                    sql = string.Concat("DELETE FROM ", quotes[0], Table, quotes[1]);
                }
                OrmUtils.PrintSQL(sql);
                CommandUtils.ExecuteNonQuery(this, Path.ToLower(), sql);
            }
            catch (Exception exp)
            {
                if (throwError)
                {
                    throw exp;
                }
            }
        }

        public bool TryTruncateTable()
        {
            try
            {
                TruncateTable(true);
                return true;
            }
            catch
            {
                ;
            }
            return false;
        }

        public bool TableExists()
        {
            string database = ConnectionUtils.GetConnectionByModel(this).Database;
            string sql = Features.GetTableExistsSql(this, database, Table);
            if (sql != null)
            {
                OrmUtils.PrintSQL(sql);

                object result = DbUtils.ExecuteScalar(Path.ToLower(), sql);
                long count = Convert.ToInt64(result);
                return count > 0;
            }
            return false;
        }

        public int GetTransaction()
        {
            return OrmUtils.GetTransaction(this.Path);
        }

        private void _CalcBeforeSaveProperties()
        {
            foreach (Property p in mBeforeSavePropeties)
            {
                object value = p.DoBeforeSaveProcessor(mSetValues);
                if (!Undefined.IsUndefinedValue(value))
                {
                    SetValue(p.Name, value);
                }
            }
        }

        private void _CalcBeforeSaveProcessor(string type, dynamic obj)
        {
            if (!string.IsNullOrWhiteSpace(BeforeSaveProcessor))
            {
                Processor.Call(BeforeSaveProcessor, this, type, obj);
            }
        }

        private void _CalcAfterSaveProcessor(string type, dynamic obj)
        {
            if (!string.IsNullOrWhiteSpace(AfterSaveProcessor))
            {
                Processor.Call(AfterSaveProcessor, this, type, obj);
            }
        }

        public bool Save(int? transCode = null)
        {
            DbTransaction trans = null;
            if (transCode != null)
            {
                trans = OrmUtils.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            try
            {
                if (mSetValues == null)
                {
                    throw new Exception("没有任何要保存的内容，请通过SetValue设置内容。");
                }

                _CalcBeforeSaveProperties();

                CommandSQL cmd = SQLBuilder.BuildInsertSQL(this);
                OrmUtils.PrintSQL(cmd.SQL, cmd.Params.ToArray());

                bool bRet = false;
                _CalcBeforeSaveProcessor("beforeCreate", mSetValues);
                if (trans == null)
                {
                    bRet = CommandUtils.ExecuteNonQuery(this, Path, cmd.SQL, cmd.Params.ToArray()) == 1;
                }
                else
                {
                    bRet = CommandUtils.ExecuteNonQuery(this, trans, cmd.SQL, cmd.Params.ToArray()) == 1;
                }
                _CalcAfterSaveProcessor("afterCreate", mSetValues);
                return bRet;
            }
            finally
            {
                Reset();
            }
        }

        /// <summary>
        /// 将过滤条件的属性值混合到用户赋值对象上
        /// </summary>
        /// <param name="filterProperties"></param>
        /// <returns></returns>
        private dynamic MixActionValues(Dictionary<string, object> filterProperties)
        {
            dynamic result = mSetValues != null ? mSetValues.Clone() : new DynamicObjectExt();
            Dictionary<string, object>.Enumerator e = filterProperties.GetEnumerator();
            while (e.MoveNext())
            {
                if (!result.ContainsKey(e.Current.Key))
                { 
                    result.SetValue(e.Current.Key, e.Current.Value);
                }
            }
            return result;
        }

        public bool Update(bool updateAll = false)
        {
            return Update(null, updateAll);
        }

        public bool Update(int? transCode, bool updateAll = false)
        {
            DbTransaction trans = null;
            if (transCode != null)
            {
                trans = OrmUtils.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            try
            {
                if (mSetValues == null)
                {
                    throw new Exception("没有任何要更新的内容，请通过SetValue设置内容。");
                }

                if (mFilter.IsEmpty() && !updateAll)
                {
                    throw new Exception("未设置更新的条件范围。");
                }

                _CalcBeforeSaveProperties();

                CommandSQL cmd = SQLBuilder.BuildUpdateSQL(this);
                OrmUtils.PrintSQL(cmd.SQL, cmd.Params.ToArray());

                bool bRet = false;
                dynamic mixedValues = MixActionValues(cmd.FilterProperties);
                _CalcBeforeSaveProcessor("beforeUpdate", mixedValues);
                if (trans == null)
                {
                    bRet = CommandUtils.ExecuteNonQuery(this, Path, cmd.SQL, cmd.Params.ToArray()) > 0;
                }
                else
                {
                    bRet = CommandUtils.ExecuteNonQuery(this, trans, cmd.SQL, cmd.Params.ToArray()) > 0;
                }
                _CalcAfterSaveProcessor("afterUpdate", mixedValues);
                return bRet;
            }
            finally
            {
                Reset();
            }
        }

        private void _CalcBeforeDeleteProcessor(string type, dynamic obj)
        {
            if (!string.IsNullOrWhiteSpace(BeforeDeleteProcessor))
            {
                Processor.Call(BeforeDeleteProcessor, this, type, obj);
            }
        }

        private void _CalcAfterDeleteProcessor(string type, dynamic obj)
        {
            if (!string.IsNullOrWhiteSpace(AfterDeleteProcessor))
            {
                Processor.Call(AfterDeleteProcessor, this, type, obj);
            }
        }

        public bool Delete(bool deleteAll = false)
        {
            return Delete(null, deleteAll);
        }

        public bool Delete(int? transCode, bool deleteAll = false)
        {
            DbTransaction trans = null;
            if (transCode != null)
            {
                trans = OrmUtils.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            try
            {
                if (mFilter.IsEmpty() && !deleteAll)
                {
                    throw new Exception("未设置删除的条件范围。");
                }

                CommandSQL where = mFilter.Build(this);

                string sql = string.Concat("DElETE FROM ", this.Table);
                if (!string.IsNullOrWhiteSpace(where.SQL))
                {
                    sql += string.Concat(" WHERE ", where.SQL);
                }

                OrmUtils.PrintSQL(sql, where.Params.ToArray());

                bool bRet = false;
                dynamic mixedValues = MixActionValues(where.FilterProperties);
                _CalcBeforeDeleteProcessor("beforeDelete", mixedValues);
                if (trans == null)
                {
                    bRet = CommandUtils.ExecuteNonQuery(this, Path, sql, where.Params.ToArray()) > 0;
                }
                else
                {
                    bRet = CommandUtils.ExecuteNonQuery(this, trans, sql, where.Params.ToArray()) > 0;
                }
                _CalcAfterDeleteProcessor("afterDelete", mixedValues);
                return bRet;
            }
            finally
            {
                Reset();
            }
        }

        private void SetPropertyValueFromDB(dynamic obj, Property prop, string propName, DbDataReader dr, string fieldName = null)
        {
            if (fieldName == null)
            {
                fieldName = propName;
            }

            if (prop.RealType == typeof(string))
            {
                obj.SetValueByPath(propName, dr.GetString(fieldName));
            }
            else if (prop.RealType == typeof(Int16))
            {
                obj.SetValueByPath(propName, dr.GetInt16(fieldName));
            }
            else if (prop.RealType == typeof(Int32))
            {
                obj.SetValueByPath(propName, dr.GetInt32(fieldName));
            }
            else if (prop.RealType == typeof(Int64))
            {
                obj.SetValueByPath(propName, dr.GetInt64(fieldName));
            }
            else if (prop.RealType == typeof(float))
            {
                obj.SetValueByPath(propName, dr.GetFloat(fieldName));
            }
            else if (prop.RealType == typeof(decimal))
            {
                obj.SetValueByPath(propName, dr.GetDecimal(fieldName));
            }
            else if (prop.RealType == typeof(double))
            {
                obj.SetValueByPath(propName, dr.GetDouble(fieldName));
            }
            else if (prop.RealType == typeof(bool))
            {
                obj.SetValueByPath(propName, dr.GetBoolean(fieldName));
            }
            else if (prop.RealType == typeof(DateTime))
            {
                obj.SetValueByPath(propName, dr.GetDateTime(fieldName));
            }
            else if (prop.RealType == typeof(DynamicObjectExt))
            {
                string fieldValue = dr.GetString(fieldName);
                dynamic jsonObj = Xmtool.Json.ConfigParser().Parse(fieldValue);
                obj.SetValueByPath(propName, jsonObj);
            }
            else
            {
                obj.SetValueByPath(propName, Convert.ChangeType(dr.GetValue(fieldName), prop.RealType));
            }
        }

        public dynamic QueryFirst(int? transCode = null)
        {
            if (!IsUsePaging)
            {
                PageIndex(1);
                PageSize(1);
            }
            List<dynamic> result = Query(transCode);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        /// 返回字段是否存在聚合操作或者函数计算
        /// </summary>
        /// <param name="gvs"></param>
        /// <returns></returns>
        private bool HaveOperation(GetValueSetting gvs)
        {
            if (gvs.Operations.Count > 0)
            {
                if (gvs.Operations.Count == 1)
                {
                    if ((gvs.Operations[0] is AggregateType &&
                        gvs.Operations[0] == AggregateType.NONE) ||
                        (gvs.Operations[0] is FunctionType &&
                        gvs.Operations[0] == FunctionType.NONE))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public List<dynamic> Query(int? transCode = null)
        {
            DbTransaction trans = null;
            if (transCode != null)
            {
                trans = OrmUtils.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            DbDataReader dr = null;
            try
            {
                if (mGetValues.Count == 0)
                {
                    for (int i = 0; i < PropertyCount; i++)
                    {
                        Property p = GetProperty(i);
                        mGetValues.Add(new GetValueSetting(p.Name));
                    }
                }

                List<dynamic> result = new List<dynamic>();

                CommandSQL cmd = SQLBuilder.BuildQuerySQL(this);

                OrmUtils.PrintSQL(cmd.SQL, cmd.Params.ToArray());

                if (trans == null)
                {
                    dr = DbUtils.ExecuteDataReader(Path, cmd.SQL, cmd.Params.ToArray());
                }
                else
                {
                    dr = DbUtils.ExecuteDataReader(trans, cmd.SQL, cmd.Params.ToArray());
                }

                if (dr != null)
                {
                    while (dr.Read())
                    {
                        dynamic obj = new DynamicObjectExt();
                        foreach (GetValueSetting gvs in mGetValues)
                        {
                            if (!gvs.Name.Contains("."))
                            {
                                Property p = GetProperty(gvs.Name);
                                if (dr.IsDBNull(gvs.FieldName))
                                {
                                    obj.SetValueByPath(gvs.OutputName, null);
                                }
                                else
                                {
                                    if (HaveOperation(gvs))
                                    {
                                        object processedValue = dr.GetValue(gvs.FieldName);
                                        obj.SetValueByPath(gvs.OutputName, processedValue);
                                    }
                                    else
                                    {
                                        SetPropertyValueFromDB(obj, p, gvs.OutputName, dr, gvs.FieldName);
                                    }
                                }

                                if (!HaveOperation(gvs) && p.NeedCalcAfterQuery)
                                {
                                    object input = obj.HasPath(gvs.OutputName) ? obj.GetValueByPath(gvs.OutputName) : null;
                                    dynamic value = Processor.Call(p.AfterQueryProcessor, this, gvs.Name, input);
                                    if (!Undefined.IsUndefinedValue(value))
                                    {
                                        if (value != null)
                                        {
                                            obj.SetValueByPath(gvs.OutputName, value);
                                        }
                                        else
                                        {
                                            obj.SetValueByPath(gvs.OutputName, null);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Model currM = this;
                                string[] subNames = gvs.Name.Split(".");
                                for (int i = 0; i < subNames.Length; i++)
                                {
                                    string subName = subNames[i];
                                    Property subProp = currM.GetProperty(subName);
                                    Model subM = ModelUtils.GetModel(subProp.TypeValue);
                                    currM = subM;

                                    if (i == subNames.Length - 2)
                                    {
                                        string lastName = subNames[subNames.Length - 1];
                                        Property lastProp = currM.GetProperty(lastName);
                                        if (dr.IsDBNull(gvs.FieldName))
                                        {
                                            obj.SetValueByPath(gvs.OutputName, null);
                                        }
                                        else
                                        {
                                            if (HaveOperation(gvs))
                                            {
                                                object processedValue = dr.GetValue(gvs.FieldName);
                                                obj.SetValueByPath(gvs.OutputName, processedValue);
                                            }
                                            else
                                            {
                                                SetPropertyValueFromDB(obj, lastProp, gvs.OutputName, dr, gvs.FieldName);
                                            }
                                        }

                                        if (!HaveOperation(gvs) && lastProp.NeedCalcAfterQuery)
                                        {
                                            object input = obj.HasPath(gvs.OutputName) ? obj.GetValueByPath(gvs.OutputName) : null;
                                            object value = Processor.Call(lastProp.AfterQueryProcessor, currM, lastName, input);
                                            if (!Undefined.IsUndefinedValue(value))
                                            {
                                                if (value != null)
                                                {
                                                    obj.SetValueByPath(gvs.OutputName, value);
                                                }
                                                else
                                                {
                                                    obj.SetValueByPath(gvs.OutputName, null);
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                        result.Add(obj);
                    }
                }

                return result;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }

                Reset();
            }
        }

        public long Count(int? transCode = null)
        {
            DbTransaction trans = null;
            if (transCode != null)
            {
                trans = OrmUtils.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            try
            {
                mSorts.Clear();
                CommandSQL cmd = SQLBuilder.BuildQuerySQL(this);
                string sql = string.Concat("SELECT COUNT(1) FROM (", cmd.SQL, ") ", this.Table);

                OrmUtils.PrintSQL(sql, cmd.Params.ToArray());

                object count;
                if (trans == null)
                {
                    count = DbUtils.ExecuteScalar(Path, sql, cmd.Params.ToArray());
                }
                else
                {
                    count = DbUtils.ExecuteScalar(trans, sql, cmd.Params.ToArray());
                }

                return Convert.ToInt64(count);
            }
            finally
            {
                Reset();
            }
        }

        public bool Exists(int? transCode = null)
        {
            DbTransaction trans = null;
            if (transCode != null)
            {
                trans = OrmUtils.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            DbDataReader dr = null;
            try
            {
                if (mFilter.IsEmpty())
                {
                    throw new Exception("未设置判断的条件范围。");
                }

                CommandSQL where = mFilter.Build(this);

                string joinSql = SQLBuilder.BuildJoinTableSQL(this, where.ForeignTables);
                string[] quotes = Features.GetObjectQuotes(this);
                string sql = string.Concat("* FROM ", quotes[0], this.Table, quotes[1], joinSql);
                if (!string.IsNullOrWhiteSpace(where.SQL))
                {
                    sql += string.Concat(" WHERE ", where.SQL);
                }
                sql = Features.GetPagingCommand(this, sql, 1, 1);

                OrmUtils.PrintSQL(sql, where.Params.ToArray());

                if (trans == null)
                {
                    dr = DbUtils.ExecuteDataReader(this.Path, sql, where.Params.ToArray());
                }
                else
                {
                    dr = DbUtils.ExecuteDataReader(trans, sql, where.Params.ToArray());
                }
                return dr.HasRows;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                Reset();
            }
        }
        #endregion
    }
}
