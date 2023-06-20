using CodeM.Common.Orm.SQL.Dialect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CodeM.Common.Orm
{
    [Serializable]
    public class Property : ICloneable
    {

        /// <summary>
        /// 属性拥有者，即所属Model
        /// </summary>
        public Model Owner { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 真实属性类型，通常和Type属性相同，Type属性为Model时不同
        /// </summary>
        public Type RealType { get; set; }

        /// <summary>
        /// Type属性值为Model时，Model的具体类型名称。如：/User
        /// </summary>
        public string TypeValue { get; set; }

        /// <summary>
        /// Type属性值为Model时，和当前属性相关联的对应Model的属性，不设置的话为对应Model的第1个主键
        /// </summary>
        public string JoinProp { get; set; }

        /// <summary>
        /// 属性对应的数据库字段名称
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 属性对应的数据库字段类型
        /// </summary>
        public DbType FieldType { get; set; }

        /// <summary>
        /// 属性显示名称，用于页面标签显示
        /// </summary>
        public string Label { get; set; }
        
        /// <summary>
        /// 属性描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; } = false;

        /// <summary>
        /// 是否不能为空
        /// </summary>
        public bool IsNotNull { get; set; } = false;

        /// <summary>
        /// 属性值的最小长度，默认为0，不限制
        /// </summary>
        public long MinLength { get; set; } = 0;

        /// <summary>
        /// 属性值的最大长度，默认为0，不限制
        /// </summary>
        public long Length { get; set; } = 0;

        /// <summary>
        /// 属性类型为数值型时，所允许的最小值
        /// </summary>
        public double? MinValue { get; set; } = null;

        /// <summary>
        /// 属性类型为数值型时，所允许的最大值
        /// </summary>
        public double? MaxValue { get; set; } = null;

        /// <summary>
        /// 浮点数类型的精度，即保留小数位数
        /// </summary>
        public int Precision { get; set; } = 0;

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool AutoIncrement { get; set; } = false;

        /// <summary>
        /// 用来设置数值是否为无符号形式
        /// </summary>
        public bool Unsigned { get; set; } = false;

        /// <summary>
        /// 唯一约束设置，值相同的字段形成组合唯一约束
        /// </summary>
        public string UniqueGroup { get; set; } = null;

        /// <summary>
        /// 索引设置，值相同的字段形成联合索引
        /// </summary>
        public string IndexGroup { get; set; } = null;

        /// <summary>
        /// 属性是否参与插入操作
        /// </summary>
        public bool JoinInsert { get; set; } = true;

        /// <summary>
        /// 属性是否参与更新操作
        /// </summary>
        public bool JoinUpdate { get; set; } = true;

        public List<PropertyRule> Rules { get; } = new List<PropertyRule>();

        /// <summary>
        /// 存储前处理Processor，在模型保存前会先调用该处理器对属性值进行处理
        /// </summary>
        internal string PreSaveProcessor { get; set; } = null;

        public bool NeedCalcPreSaveProcessor
        {
            get
            {
                return !string.IsNullOrWhiteSpace(PreSaveProcessor);
            }
        }

        internal object DoPreSaveProcessor(dynamic obj)
        {
            if (NeedCalcPreSaveProcessor)
            {
                return Processor.CallPropertyProcessor(PreSaveProcessor, Owner, Name, 
                    obj.Has(Name) ? obj[Name] : null);
            }
            return NotSet.Value;
        }

        /// <summary>
        /// 查询后处理Processor，在数据从数据库查询返回后调用该处理器对属性值进行处理
        /// </summary>
        internal string PostQueryProcessor { get; set; } = null;

        public bool NeedCalcPostQueryProcessor
        {
            get
            {
                return !string.IsNullOrWhiteSpace(PostQueryProcessor);
            }
        }

        internal object DoPostQueryProcessor(dynamic propValue)
        {
            if (NeedCalcPostQueryProcessor)
            {
                return Processor.CallPropertyProcessor(
                    PostQueryProcessor, Owner, Name, propValue);
            }
            return NotSet.Value;
        }

        /// <summary>
        /// 属性默认值，新建模型时，若属性未赋值，将使用该值填充
        /// </summary>
        public string DefaultValue { get; internal set; } = null;

        private int mDefaultValueIsProcessor = 0;
        public bool DefaultValueIsProcessor
        {
            get
            {
                if (mDefaultValueIsProcessor == 0)
                {
                    mDefaultValueIsProcessor = -1;
                    if (!string.IsNullOrWhiteSpace(DefaultValue))
                    {
                        string propDefaultValue = DefaultValue.Trim();
                        if (propDefaultValue.Length > 4 &&
                            propDefaultValue.StartsWith("{{") &&
                            propDefaultValue.EndsWith("}}"))
                        {
                            mDefaultValueIsProcessor = 1;
                        }
                    }
                }
                return mDefaultValueIsProcessor > 0;
            }
            internal set
            {
                mDefaultValueIsProcessor = value ? 1 : -1;
            }
        }

        public object CalcDefaultValue(dynamic obj)
        {
            if (DefaultValue != null)
            {
                if (DefaultValueIsProcessor)
                {
                    string propDefaultValue = DefaultValue.Trim();
                    string processorName = propDefaultValue.Substring(
                        2, propDefaultValue.Length - 4).Trim();
                    dynamic value = Processor.CallPropertyProcessor(processorName, Owner, Name, 
                        obj.Has(Name) ? obj[Name] : null);
                    if (!NotSet.IsNotSetValue(value))
                    {
                        return value;
                    }
                    return null;
                }
                return Convert.ChangeType(DefaultValue, Type);
            }
            return null;
        }

        public override string ToString()
        {
            string[] quotes = Features.GetObjectQuotes(Owner);

            StringBuilder sb = new StringBuilder(64);
            sb.Append(string.Concat(quotes[0], Field, quotes[1]));

            string autoIncrReplaceType = Features.GetAutoIncrementReplaceType(Owner);
            if (AutoIncrement && !string.IsNullOrWhiteSpace(autoIncrReplaceType))
            {
                sb.Append(string.Concat(" ", autoIncrReplaceType));
            }
            else
            {
                string fieldType = FieldUtils.GetFieldType(Owner, FieldType);
                sb.Append(string.Concat(" ", fieldType));
                if (Length > 0)
                {
                    if (FieldUtils.IsFloat(FieldType))
                    {
                        sb.Append(string.Concat("(", Length, ",", Precision, ")"));
                    }
                    else
                    {
                        sb.Append(string.Concat("(", Length, ")"));
                    }
                }
                if (Features.IsSupportUnsigned(Owner) && Unsigned)
                {
                    sb.Append(" UNSIGNED");
                }
            }
            if (IsPrimaryKey && Owner.PrimaryKeyCount == 1)
            {
                sb.Append(" PRIMARY KEY");
            }
            if (AutoIncrement && 
                Features.IsSupportAutoIncrement(Owner) &&
                string.IsNullOrWhiteSpace(autoIncrReplaceType))
            {
                string[] extCmds = Features.GetAutoIncrementExtCommand(Owner, Owner.Table, Field);
                if (extCmds.Length == 0)
                {
                    sb.Append(string.Concat(" ", FieldUtils.GetFieldAutoIncrementTag(Owner)));
                }
            }
            if (IsNotNull)
            {
                sb.Append(" NOT NULL");
            }
            if (Features.IsSupportComment(Owner) && 
                !string.IsNullOrWhiteSpace(Description))
            {
                string extCmd = Features.GetCommentExtCommand(Owner, Owner.Table, Field, Description);
                if (string.IsNullOrWhiteSpace(extCmd))
                {
                    sb.Append(string.Concat(" COMMENT '", Description, "'"));
                }
            }
            return sb.ToString();
        }

        public object Clone()
        {
            Property cloneObj = new Property();
            cloneObj.Name = this.Name;
            cloneObj.Type = this.Type;
            cloneObj.RealType = this.RealType;
            cloneObj.TypeValue = this.TypeValue;
            cloneObj.JoinProp = this.JoinProp;
            cloneObj.Field = this.Field;
            cloneObj.FieldType = this.FieldType;
            cloneObj.Label = this.Label;
            cloneObj.Description = this.Description;
            cloneObj.MinLength = this.MinLength;
            cloneObj.Length = this.Length;
            cloneObj.MinValue = this.MinValue;
            cloneObj.MaxValue = this.MaxValue;
            cloneObj.Precision = this.Precision;
            cloneObj.AutoIncrement = this.AutoIncrement;
            cloneObj.Unsigned = this.Unsigned;
            cloneObj.UniqueGroup = this.UniqueGroup;
            cloneObj.IndexGroup = this.IndexGroup;
            cloneObj.IsNotNull = this.IsNotNull;
            cloneObj.IsPrimaryKey = this.IsPrimaryKey;
            cloneObj.PreSaveProcessor = this.PreSaveProcessor;
            cloneObj.PostQueryProcessor = this.PostQueryProcessor;
            cloneObj.DefaultValue = this.DefaultValue;
            cloneObj.DefaultValueIsProcessor = this.DefaultValueIsProcessor;
            cloneObj.JoinInsert = this.JoinInsert;
            cloneObj.JoinUpdate = this.JoinUpdate;
            cloneObj.Rules.AddRange(this.Rules);
            return cloneObj;
        }

    }
}
