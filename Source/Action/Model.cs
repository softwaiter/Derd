using CodeM.Common.DbHelper;
using CodeM.Common.Orm.Action;
using CodeM.Common.Orm.SQL;
using CodeM.Common.Orm.SQL.Dialect;
using CodeM.Common.Tools;
using CodeM.Common.Tools.DynamicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CodeM.Common.Orm
{
    public partial class Model : ISetValue, IGetValue, IPaging, ISort, ICommand, IAssist
    {
        #region ISetValue
        dynamic mSetValues;
        List<dynamic> mBatchValues = new List<dynamic>();

        internal bool TryGetValue(dynamic modelValues, Property p, out object value, bool useDefaultValue = true)
        {
            if (modelValues.Has(p.Name))
            {
                value = modelValues[p.Name];
                return true;
            }
            else if (useDefaultValue)
            {
                try
                {
                    if (p.DefaultValue != null)
                    {
                        value = p.CalcDefaultValue(modelValues);
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
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：Boolean，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(byte))
                {
                    byte result;
                    if (!byte.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：Byte，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(sbyte))
                {
                    sbyte result;
                    if (!sbyte.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：SByte，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(decimal))
                {
                    decimal result;
                    if (!decimal.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：Decimal，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(double))
                {
                    double result;
                    if (!double.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：Double，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(Int16))
                {
                    Int16 result;
                    if (!Int16.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：Int16，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(Int32))
                {
                    Int32 result;
                    if (!Int32.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：Int32，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(Int64))
                {
                    Int64 result;
                    if (!Int64.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：Int64，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(Single))
                {
                    Single result;
                    if (!Single.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：Single，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(UInt16))
                {
                    UInt16 result;
                    if (!UInt16.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：UInt16，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(UInt32))
                {
                    UInt32 result;
                    if (!UInt32.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：UInt32，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(UInt64))
                {
                    UInt64 result;
                    if (!UInt64.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：UInt64，实际类型为：", value.GetType().Name));
                    }
                }
                else if (type == typeof(DateTime))
                {
                    DateTime result;
                    if (!DateTime.TryParse("" + value, out result))
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "期待类型：DateTime，实际类型为：", value.GetType().Name));
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
                    !p.NeedCalcPreSaveProcessor)
                {
                    throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "不允许为空。"));
                }
            }
            else
            {
                if (p.RealType == typeof(string))
                {
                    if (p.MinLength > 0 && value.ToString().Length < p.MinLength)
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "内容长度不能小于", p.Length, "。"));
                    }

                    if (value.ToString().Length > p.Length)
                    {
                        throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "内容长度不能超过", p.Length, "。"));
                    }
                }
                else if (FieldUtils.IsNumeric(p.FieldType))
                {
                    string valueStr = value.ToString();

                    if (FieldUtils.IsFloat(p.FieldType) && p.Precision > 0)
                    {
                        int pos = valueStr.IndexOf(".");
                        if (pos > 0 && valueStr.Length - pos - 1 > p.Precision)
                        {
                            throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "小数位数不能超过", p.Precision, "位小数。"));
                        }
                    }

                    double dValue = double.Parse(valueStr);
                    if (p.MinValue != null)
                    {
                        if (dValue < p.MinValue)
                        {
                            throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "最小值不能小于", p.MinValue, "。"));
                        }
                    }
                    if (p.MaxValue != null)
                    {
                        if (dValue > p.MaxValue)
                        {
                            throw new PropertyValidationException(string.Concat(p.Label ?? p.Name, "最大值不能超过：", p.MaxValue, "。"));
                        }
                    }
                }
            }
        }

        private void _CheckPropertyRules(Property p, object value)
        {
            if (p.Rules.Count > 0 && value != null &&
                !string.IsNullOrWhiteSpace(value.ToString()))
            {
                string valueStr = value.ToString();
                foreach (PropertyRule rule in p.Rules)
                {
                    rule.Validate(p, value);
                }
            }
        }

        private void _CheckModelConstraint(string name, object value)
        {
            Property p = GetProperty(name);
            _CheckPropertyType(p, value);
            _CheckPropertyValue(p, value);
            _CheckPropertyRules(p, value);
        }

        private void _SetPropertyValue(dynamic modelValues,
            string name, object value, bool validate = false)
        {
            if (validate)
            {
                _CheckModelConstraint(name, value);
            }
            modelValues.SetValue(name, value);
        }

        public Model SetValue(string name, object value, bool validate = false)
        {
            if (mSetValues == null)
            {
                mSetValues = new DynamicObjectExt();
            }

            _SetPropertyValue(mSetValues, name, value, validate);

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

        public Model SetBatchInsertValues(dynamic obj, bool validate = false)
        {
            if (obj != null)
            {
                if (validate)
                {
                    for (int i = 0; i < PropertyCount; i++)
                    {
                        Property p = GetProperty(i);
                        if (obj.Has(p.Name))
                        {
                            _CheckModelConstraint(p.Name, obj[p.Name]);
                        }
                    }
                }
                mBatchValues.Add(obj);
            }

            return this;
        }
        #endregion

        #region IGetValue
        List<SelectFieldPart> mGetValues = new List<SelectFieldPart>();

        internal List<SelectFieldPart> ReturnValues
        {
            get
            {
                return mGetValues;
            }
        }

        public Model GetValue(string propName, string alias = null)
        {
            if (CommandUtils.IsProperty(this, propName, out Property p))
            {
                mGetValues.Add(new SelectFieldPart(new PROPERTY(propName, p), mGetValues.Count + 1, alias));
            }
            else
            {
                mGetValues.Add(new SelectFieldPart(new VALUE(propName), mGetValues.Count + 1, alias));
            }
            return this;
        }

        public Model GetValue(Function function, string alias = null)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }

            if (function.IsIncludeDistinct())    // 是否包含Distinct运算
            {
                mGetValues.Insert(0, new SelectFieldPart(function, mGetValues.Count + 1, alias));
            }
            else
            {
                mGetValues.Add(new SelectFieldPart(function, mGetValues.Count + 1, alias));
            }

            return this;
        }

        public Model GetValues(params string[] names)
        {
            foreach (string name in names)
            {
                GetValue(name);
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

        public Model And(string constCondition)
        {
            mFilter.And(constCondition);
            return this;
        }

        public Model And(IFilter subCondition)
        {
            mFilter.And(subCondition);
            return this;
        }

        public Model Or(string constCondition)
        {
            mFilter.Or(constCondition);
            return this;
        }

        public Model Or(IFilter subCondition)
        {
            mFilter.Or(subCondition);
            return this;
        }

        public new Model Equals(object key, object value)
        {
            mFilter.Equals(key, value);
            return this;
        }

        public Model Equals(Function function, object value)
        {
            mFilter.Equals(function, value);
            return this;
        }

        public Model NotEquals(object key, object value)
        {
            mFilter.NotEquals(key, value);
            return this;
        }

        public Model NotEquals(Function function, object value)
        {
            mFilter.NotEquals(function, value);
            return this;
        }

        public Model Gt(object key, object value)
        {
            mFilter.Gt(key, value);
            return this;
        }

        public Model Gt(Function function, object value)
        {
            mFilter.Gt(function, value);
            return this;
        }

        public Model Gte(object key, object value)
        {
            mFilter.Gte(key, value);
            return this;
        }

        public Model Gte(Function function, object value)
        {
            mFilter.Gte(function, value);
            return this;
        }

        public Model Lt(object key, object value)
        {
            mFilter.Lt(key, value);
            return this;
        }

        public Model Lt(Function function, object value)
        {
            mFilter.Lt(function, value);
            return this;
        }

        public Model Lte(object key, object value)
        {
            mFilter.Lte(key, value);
            return this;
        }

        public Model Lte(Function function, object value)
        {
            mFilter.Lte(function, value);
            return this;
        }

        public Model Like(object key, object value)
        {
            mFilter.Like(key, value);
            return this;
        }

        public Model Like(Function function, object value)
        {
            mFilter.Like(function, value);
            return this;
        }

        public Model NotLike(object key, object value)
        {
            mFilter.NotLike(key, value);
            return this;
        }

        public Model NotLike(Function function, object value)
        {
            mFilter.NotLike(function, value);
            return this;
        }

        public Model IsNull(object key)
        {
            mFilter.IsNull(key);
            return this;
        }

        public Model IsNull(Function function)
        {
            mFilter.IsNull(function);
            return this;
        }

        public Model IsNotNull(object key)
        {
            mFilter.IsNotNull(key);
            return this;
        }

        public Model IsNotNull(Function function)
        {
            mFilter.IsNotNull(function);
            return this;
        }

        public Model Between(object key, object value, object value2)
        {
            mFilter.Between(key, value, value2);
            return this;
        }

        public Model Between(Function function, object value, object value2)
        {
            mFilter.Between(function, value, value2);
            return this;
        }

        public Model In(object key, params object[] values)
        {
            mFilter.In(key, values);
            return this;
        }

        public Model In(Function function, params object[] values)
        {
            mFilter.In(function, values);
            return this;
        }

        public Model NotIn(object key, params object[] values)
        {
            mFilter.NotIn(key, values);
            return this;
        }

        public Model NotIn(Function function, params object[] values)
        {
            mFilter.NotIn(function, values);
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

            if (mGetValues.Exists(item => item.FieldName == name))
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

            if (mGetValues.Exists(item => item.FieldName == name))
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

        List<GroupByPart> mGroupByNames = new List<GroupByPart>();

        internal List<GroupByPart> GroupByNames
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
                if (CommandUtils.IsProperty(this, name, out Property p))
                {
                    mGroupByNames.Add(new GroupByPart(new PROPERTY(name, p)));
                }
                else
                {
                    mGroupByNames.Add(new GroupByPart(new VALUE(name)));
                }
            }
            return this;
        }

        public Model GroupBy(Function function)
        {
            mGroupByNames.Add(new GroupByPart(function));
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

            Derd.PrintSQL(sb.ToString());
            CommandUtils.ExecuteNonQuery(this, Path.ToLower(), sb.ToString());

            string tableIndexSQL = ToString(true);
            if (!string.IsNullOrWhiteSpace(tableIndexSQL))
            {
                Derd.PrintSQL(tableIndexSQL);
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
                                Derd.PrintSQL(cmd);
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
            bool tableRemoved = false;
            try
            {
                if (this.TableExists())
                {
                    string[] quotes = Features.GetObjectQuotes(this);
                    string sql = string.Concat("DROP TABLE ", quotes[0], Table, quotes[1]);
                    Derd.PrintSQL(sql);
                    CommandUtils.ExecuteNonQuery(this, Path.ToLower(), sql);
                    tableRemoved = true;
                }
            }
            catch (Exception exp)
            {
                if (throwError)
                {
                    throw exp;
                }
            }

            if (tableRemoved)
            {
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
                                    Derd.PrintSQL(cmd);
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
                Derd.PrintSQL(sql);
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
                Derd.PrintSQL(sql);

                object result = DbUtils.ExecuteScalar(Path.ToLower(), sql);
                long count = Convert.ToInt64(result);
                return count > 0;
            }
            return false;
        }

        public int GetTransaction()
        {
            return Derd.GetTransaction(this.Path);
        }

        private void _CalcPreSaveProperties(dynamic modelValues)
        {
            foreach (Property p in mPreSavePropeties)
            {
                object value = p.DoPreSaveProcessor(modelValues);
                if (!NotSet.IsNotSetValue(value))
                {
                    _SetPropertyValue(modelValues, p.Name, value);
                }
            }
        }

        private bool _HasBeforeNewProcessor()
        {
            return !string.IsNullOrWhiteSpace(BeforeNewProcessor);
        }

        private bool _CalcBeforeNewProcessor(dynamic input, dynamic output, int? transCode = null)
        {
            if (_HasBeforeNewProcessor())
            {
                string[] processors = BeforeNewProcessor.Split(",");
                for (int i = 0; i < processors.Length; i++)
                {
                    if (!Processor.CallModelProcessor(
                        processors[i].Trim(), this,
                        input, output, transCode))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool _HasAfterNewProcessor()
        {
            return !string.IsNullOrWhiteSpace(AfterNewProcessor);
        }

        private bool _CalcAfterNewProcessor(dynamic input, dynamic output, int? transCode = null)
        {
            if (_HasAfterNewProcessor())
            {
                string[] processors = AfterNewProcessor.Split(",");
                for (int i = 0; i < processors.Length; i++)
                {
                    if (!Processor.CallModelProcessor(
                        processors[i].Trim(), this,
                        input, output, transCode))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool _SaveOne(dynamic modelValues, int? transCode = null)
        {
            DbTransaction trans = null;

            bool haveUserTransCode = !(transCode == null);
            if ((_HasBeforeNewProcessor() || _HasAfterNewProcessor()) && !haveUserTransCode)
            {
                transCode = Derd.GetTransaction(Path);
            }

            if (transCode != null)
            {
                trans = Derd.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            bool bRet = false;
            try
            {
                _CalcPreSaveProperties(modelValues);

                CommandSQL cmd = SQLBuilder.BuildInsertSQL(this, modelValues);
                Derd.PrintSQL(cmd.SQL, cmd.Params.ToArray());

                bRet = _CalcBeforeNewProcessor(modelValues, null, transCode);
                if (bRet)
                {
                    if (trans == null)
                    {
                        bRet = CommandUtils.ExecuteNonQuery(this, Path, cmd.SQL, cmd.Params.ToArray()) == 1;
                    }
                    else
                    {
                        bRet = CommandUtils.ExecuteNonQuery(this, trans, cmd.SQL, cmd.Params.ToArray()) == 1;
                    }

                    if (bRet)
                    {
                        bRet = _CalcAfterNewProcessor(modelValues, null, transCode);
                    }
                }

                if (trans != null && !haveUserTransCode)
                {
                    if (bRet)
                    {
                        Derd.CommitTransaction(transCode.Value);
                    }
                    else
                    {
                        Derd.RollbackTransaction(transCode.Value);
                    }
                }

                return bRet;
            }
            catch (Exception exp)
            {
                if (trans != null && !haveUserTransCode)
                {
                    Derd.RollbackTransaction(transCode.Value);
                }
                throw exp;
            }
            finally
            {
                Reset();
            }
        }

        private bool _CommonSaveBatch(int? transCode)
        {
            DbTransaction trans = null;
            if (transCode != null)
            {
                trans = Derd.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            for (int i = 0; i < mBatchValues.Count; i++)
            {
                _CalcPreSaveProperties(mBatchValues[i]);
            }

            CommandSQL cmd = SQLBuilder.BuildBatchInsertSQL(this, mBatchValues);
            Derd.PrintSQL(cmd.SQL, cmd.Params.ToArray());

            bool bRet = false;
            if (trans == null)
            {
                bRet = CommandUtils.ExecuteNonQuery(this, Path, cmd.SQL, cmd.Params.ToArray()) == mBatchValues.Count;
            }
            else
            {
                bRet = CommandUtils.ExecuteNonQuery(this, trans, cmd.SQL, cmd.Params.ToArray()) == mBatchValues.Count;
            }

            return bRet;
        }

        private bool _SqliteSaveBatch(int? transCode)
        {
            bool isInnerTransaction = false;
            if (transCode == null)
            {
                transCode = Derd.GetTransaction();
                isInnerTransaction = true;
            }

            bool bRet = true;
            try
            {
                for (int i = 0; i < mBatchValues.Count; i++)
                {
                    bRet = _SaveOne(mBatchValues[i], transCode);
                    if (!bRet)
                    {
                        break;
                    }
                }

                if (isInnerTransaction)
                {
                    if (bRet)
                    {
                        Derd.CommitTransaction(transCode.Value);
                    }
                    else
                    {
                        Derd.RollbackTransaction(transCode.Value);
                    }
                }

                return bRet;
            }
            catch (Exception exp)
            {
                if (isInnerTransaction)
                {
                    Derd.RollbackTransaction(transCode.Value);
                }
                throw exp;
            }
        }

        /// <summary>
        /// 批量保存模型不会触发beforeNew、afterNew、beforeUpdate、afterUpdate、beforeDelete、afterDelete等模型事件
        /// </summary>
        /// <param name="transCode"></param>
        /// <returns></returns>
        private bool _SaveBatch(int? transCode = null)
        {
            bool bRet = false;
            try
            {
                ConnectionSetting cs = ConnectionUtils.GetConnectionByModel(this);
                switch (cs.Dialect.ToLower())
                {
                    case "sqlite":
                        bRet = _SqliteSaveBatch(transCode);
                        break;
                    default:
                        bRet = _CommonSaveBatch(transCode);
                        break;
                }
                return bRet;
            }
            finally
            {
                Reset();
                mBatchValues.Clear();
            }
        }

        public bool Save(int? transCode = null)
        {
            if (mSetValues != null)
            {
                return _SaveOne(mSetValues, transCode);
            }
            else if (mBatchValues.Count > 0)
            {
                return _SaveBatch(transCode);
            }
            else
            {
                throw new Exception("未找到要保存的内容，请通过SetValue、SetValues或SetBatchInsertValues方法设置内容后再进行保存操作。");
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
                    object value = e.Current.Value;
                    if (value != null)
                    {
                        Property p;
                        if (mProperties.TryGetValue(e.Current.Key.ToLower(), out p))
                        {
                            value = Convert.ChangeType(value, p.RealType);
                        }
                    }
                    result.SetValue(e.Current.Key, value);
                }
            }
            return result;
        }

        private bool _HasBeforeUpdateProcessor()
        {
            return !string.IsNullOrWhiteSpace(BeforeUpdateProcessor);
        }

        private bool _CalcBeforeUpdateProcessor(dynamic input, dynamic output, int? transCode = null)
        {
            if (_HasBeforeUpdateProcessor())
            {
                string[] processors = BeforeUpdateProcessor.Split(",");
                for (int i = 0; i < processors.Length; i++)
                {
                    if (!Processor.CallModelProcessor(
                        processors[i].Trim(), this,
                        input, output, transCode))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool _HasAfterUpdateProcessor()
        {
            return !string.IsNullOrWhiteSpace(AfterUpdateProcessor);
        }

        private bool _CalcAfterUpdateProcessor(dynamic input, dynamic output, int? transCode = null)
        {
            if (_HasAfterUpdateProcessor())
            {
                string[] processors = AfterUpdateProcessor.Split(",");
                for (int i = 0; i < processors.Length; i++)
                {
                    if (!Processor.CallModelProcessor(
                        processors[i].Trim(), this,
                        input, output, transCode))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Update(bool updateAll = false)
        {
            return Update(null, updateAll);
        }

        public bool Update(int? transCode, bool updateAll = false)
        {
            DbTransaction trans = null;

            bool haveUserTransCode = !(transCode == null);
            if ((_HasBeforeUpdateProcessor() || _HasAfterUpdateProcessor()) && !haveUserTransCode)
            {
                transCode = Derd.GetTransaction(Path);
            }

            if (transCode != null)
            {
                trans = Derd.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            bool bRet = false;
            try
            {
                if (mSetValues == null)
                {
                    throw new Exception("未找到要更新的内容，请通过SetValue、SetValues或SetBatchInsertValues方法设置内容后再进行更新操作。");
                }

                if (mFilter.IsEmpty() && !updateAll)
                {
                    throw new Exception("未设置更新的条件范围。");
                }

                _CalcPreSaveProperties(mSetValues);

                CommandSQL cmd = SQLBuilder.BuildUpdateSQL(this, mSetValues);
                Derd.PrintSQL(cmd.SQL, cmd.Params.ToArray());

                dynamic mixedValues = MixActionValues(cmd.FilterProperties);
                bRet = _CalcBeforeUpdateProcessor(mixedValues, null, transCode);
                if (bRet)
                {
                    if (trans == null)
                    {
                        bRet = CommandUtils.ExecuteNonQuery(this, Path, cmd.SQL, cmd.Params.ToArray()) > 0;
                    }
                    else
                    {
                        bRet = CommandUtils.ExecuteNonQuery(this, trans, cmd.SQL, cmd.Params.ToArray()) > 0;
                    }

                    if (bRet)
                    {
                        bRet = _CalcAfterUpdateProcessor(mixedValues, null, transCode);
                    }
                }

                if (trans != null && !haveUserTransCode)
                {
                    if (bRet)
                    {
                        Derd.CommitTransaction(transCode.Value);
                    }
                    else
                    {
                        Derd.RollbackTransaction(transCode.Value);
                    }
                }

                return bRet;
            }
            catch (Exception exp)
            {
                if (trans != null && !haveUserTransCode)
                {
                    Derd.RollbackTransaction(transCode.Value);
                }
                throw exp;
            }
            finally
            {
                Reset();
            }
        }

        private bool _HasBeforeDeleteProcessor()
        {
            return !string.IsNullOrWhiteSpace(BeforeDeleteProcessor);
        }

        private bool _CalcBeforeDeleteProcessor(dynamic input, dynamic output, int? transCode = null)
        {
            if (_HasBeforeDeleteProcessor())
            {
                string[] processors = BeforeDeleteProcessor.Split(",");
                for (int i = 0; i < processors.Length; i++)
                {
                    if (!Processor.CallModelProcessor(
                        processors[i].Trim(), this,
                        input, output, transCode))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool _HasAfterDeleteProcessor()
        {
            return !string.IsNullOrWhiteSpace(AfterDeleteProcessor);
        }

        private bool _CalcAfterDeleteProcessor(dynamic input, dynamic output, int? transCode = null)
        {
            if (_HasAfterDeleteProcessor())
            {
                string[] processors = AfterDeleteProcessor.Split(",");
                for (int i = 0; i < processors.Length; i++)
                {
                    if (!Processor.CallModelProcessor(
                        processors[i].Trim(), this,
                        input, output, transCode))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Delete(bool deleteAll = false)
        {
            return Delete(null, deleteAll);
        }

        public bool Delete(int? transCode, bool deleteAll = false)
        {
            DbTransaction trans = null;

            bool haveUserTransCode = !(transCode == null);
            if ((_HasBeforeDeleteProcessor() || _HasAfterDeleteProcessor()) && !haveUserTransCode)
            {
                transCode = Derd.GetTransaction(Path);
            }

            if (transCode != null)
            {
                trans = Derd.GetTransaction(transCode.Value);
                if (trans == null)
                {
                    throw new Exception(string.Format("未找到指定事务：{0}", transCode));
                }
            }

            bool bRet = false;
            try
            {
                if (mFilter.IsEmpty() && !deleteAll)
                {
                    throw new Exception("未设置删除的条件范围。");
                }

                CommandSQL where = mFilter.Build(this);

                string[] quotes = Features.GetObjectQuotes(this);
                string sql = string.Concat("DELETE FROM ", quotes[0], this.Table, quotes[1]);

                if (where.ForeignTables.Count > 0)
                {
                    if (this.PrimaryKeyCount > 1)
                    {
                        throw new NotSupportedException("暂不支持组合主键的关联条件删除。");
                    }

                    Property pp = GetPrimaryKey(0);
                    string joinSql = SQLBuilder.BuildJoinTableSQL(this, where.ForeignTables);
                    string tempTable = string.Concat(quotes[0], "tmp", DateTime.Now.Millisecond, quotes[1]);
                    string subSql = string.Concat("SELECT ", tempTable, ".", quotes[0], pp.Field, quotes[1], 
                        " FROM (SELECT ", quotes[0], this.Table, quotes[1], ".", quotes[0], pp.Field, quotes[1], 
                        " FROM ", quotes[0], this.Table, quotes[1], joinSql, " WHERE ", where.SQL, ") ", tempTable);

                    //string subSql = string.Concat("SELECT ", quotes[0], this.Table, quotes[1], ".", quotes[0], pp.Field, quotes[1], " FROM ", quotes[0], this.Table, quotes[1], joinSql, " WHERE ", where.SQL);

                    sql += String.Concat(" WHERE ", quotes[0], this.Table, quotes[1], ".", quotes[0], pp.Field, quotes[1], " IN (", subSql, ")");
                }
                else if (!string.IsNullOrWhiteSpace(where.SQL))
                {
                    sql += string.Concat(" WHERE ", where.SQL);
                }

                Derd.PrintSQL(sql, where.Params.ToArray());

                dynamic mixedValues = MixActionValues(where.FilterProperties);
                bRet = _CalcBeforeDeleteProcessor(mixedValues, null, transCode);
                if (bRet)
                {
                    if (trans == null)
                    {
                        bRet = CommandUtils.ExecuteNonQuery(this, Path, sql, where.Params.ToArray()) > 0;
                    }
                    else
                    {
                        bRet = CommandUtils.ExecuteNonQuery(this, trans, sql, where.Params.ToArray()) > 0;
                    }

                    if (bRet)
                    {
                        bRet = _CalcAfterDeleteProcessor(mixedValues, null, transCode);
                    }
                }

                if (trans != null && !haveUserTransCode)
                {
                    if (bRet)
                    {
                        Derd.CommitTransaction(transCode.Value);
                    }
                    else
                    {
                        Derd.RollbackTransaction(transCode.Value);
                    }
                }

                return bRet;
            }
            catch (Exception exp)
            {
                if (trans != null && !haveUserTransCode)
                {
                    Derd.RollbackTransaction(transCode.Value);
                }
                throw exp;
            }
            finally
            {
                Reset();
            }
        }

        private bool _HasBeforeQueryProcessor()
        {
            return !string.IsNullOrWhiteSpace(BeforeQueryProcessor);
        }

        private bool _CalcBeforeQueryProcessor(dynamic input, dynamic output, int? transCode = null)
        {
            if (_HasBeforeQueryProcessor())
            {
                string[] processors = BeforeQueryProcessor.Split(",");
                for (int i = 0; i < processors.Length; i++)
                {
                    if (!Processor.CallModelProcessor(
                        processors[i].Trim(), this,
                        input, output, transCode))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool _HasAfterQueryProcessor()
        {
            return !string.IsNullOrWhiteSpace(AfterQueryProcessor);
        }

        private bool _CalcAfterQueryProcessor(dynamic input, dynamic output, int? transCode)
        {
            if (_HasAfterQueryProcessor())
            {
                string[] processors = AfterQueryProcessor.Split(",");
                for (int i = 0; i < processors.Length; i++)
                {
                    if (!Processor.CallModelProcessor(
                        processors[i].Trim(), this,
                        input, output, transCode))
                    {
                        return false;
                    }
                }
            }
            return true;
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

        public List<dynamic> Query(int? transCode = null)
        {
            DbTransaction trans = null;
            if (transCode != null)
            {
                trans = Derd.GetTransaction(transCode.Value);
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
                        mGetValues.Add(new SelectFieldPart(new PROPERTY(p.Name, p), i + 1));
                    }
                }

                List<dynamic> result = new List<dynamic>();

                CommandSQL cmd = SQLBuilder.BuildQuerySQL(this);

                Derd.PrintSQL(cmd.SQL, cmd.Params.ToArray());

                dynamic mixedValues = MixActionValues(cmd.FilterProperties);
                bool bRet = _CalcBeforeQueryProcessor(mixedValues, result, transCode);
                if (bRet)
                {
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
                            foreach (SelectFieldPart sfp in mGetValues)
                            {
                                if (dr.IsDBNull(sfp.FieldName))
                                {
                                    obj.SetValueByPath(sfp.OutputName, null);
                                }
                                else
                                {
                                    if (CommandUtils.IsProperty(this,
                                        sfp.PropertyName, out Property p))
                                    {
                                        SetPropertyValueFromDB(obj, p, sfp.OutputName, dr, sfp.FieldName);

                                        if (p.NeedCalcPostQueryProcessor)
                                        {
                                            object input = obj.HasPath(sfp.OutputName) ? obj.GetValueByPath(sfp.OutputName) : null;
                                            dynamic value = p.DoPostQueryProcessor(input);
                                            if (!NotSet.IsNotSetValue(value))
                                            {
                                                if (value != null)
                                                {
                                                    obj.SetValueByPath(sfp.OutputName, value);
                                                }
                                                else
                                                {
                                                    obj.SetValueByPath(sfp.OutputName, null);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        object processedValue = dr.GetValue(sfp.FieldName);
                                        obj.SetValueByPath(sfp.OutputName, processedValue);
                                    }
                                }
                            }
                            result.Add(obj);
                        }
                    }

                    _CalcAfterQueryProcessor(mixedValues, result, transCode);
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
                trans = Derd.GetTransaction(transCode.Value);
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

                Derd.PrintSQL(sql, cmd.Params.ToArray());

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
                trans = Derd.GetTransaction(transCode.Value);
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

                Derd.PrintSQL(sql, where.Params.ToArray());

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
