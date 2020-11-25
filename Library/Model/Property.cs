using CodeM.Common.Orm.Dialect;
using CodeM.Common.Orm.Processor;
using System;
using System.Data;
using System.Text;

namespace CodeM.Common.Orm
{
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
        /// Type属性值为Model时，Model的具体类型名称。如：/User
        /// </summary>
        public string TypeValue { get; set; }

        /// <summary>
        /// 属性对应的数据库字段名称
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 属性对应的数据库字段类型
        /// </summary>
        public DbType FieldType { get; set; }

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
        /// 属性值的最大长度，默认为0，不限制
        /// </summary>
        public long Length { get; set; } = 0;

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

        /// <summary>
        /// 存储前先处理Processor，在模型保存前会先调用该处理器对属性值进行处理
        /// </summary>
        public string BeforeSaveProcessor { get; internal set; } = null;

        public object DoBeforeSaveProcessor(dynamic obj)
        {
            if (!string.IsNullOrWhiteSpace(BeforeSaveProcessor))
            {
                return Executor.Call(BeforeSaveProcessor, Owner, Name, obj);
            }
            return null;
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

                    if (DefaultValue != null)
                    {
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
                    return Executor.Call(processorName, Owner, Name, obj);
                }
                return Convert.ChangeType(DefaultValue, Type);
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            string fieldType = FieldUtils.GetFieldType(Owner, FieldType);
            sb.Append(string.Concat(Field, " ", fieldType));
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
            if (IsPrimaryKey && Owner.PrimaryKeyCount == 1)
            {
                sb.Append(" PRIMARY KEY");
            }
            if (AutoIncrement && Features.IsSupportAutoIncrement(Owner))
            {
                sb.Append(string.Concat(" ", FieldUtils.GetFieldAutoIncrementTag(Owner)));
            }
            if (IsNotNull)
            {
                sb.Append(" NOT NULL");
            }
            if (Features.IsSupportUnsigned(Owner) && 
                !string.IsNullOrWhiteSpace(Description))
            {
                sb.Append(string.Concat(" COMMENT '", Description, "'"));
            }
            return sb.ToString();
        }

        public object Clone()
        {
            Property cloneObj = new Property();
            cloneObj.Name = this.Name;
            cloneObj.Type = this.Type;
            cloneObj.TypeValue = this.TypeValue;
            cloneObj.Field = this.Field;
            cloneObj.FieldType = this.FieldType;
            cloneObj.Description = this.Description;
            cloneObj.Length = this.Length;
            cloneObj.Precision = this.Precision;
            cloneObj.AutoIncrement = this.AutoIncrement;
            cloneObj.Unsigned = this.Unsigned;
            cloneObj.UniqueGroup = this.UniqueGroup;
            cloneObj.IndexGroup = this.IndexGroup;
            cloneObj.IsNotNull = this.IsNotNull;
            cloneObj.IsPrimaryKey = this.IsPrimaryKey;
            cloneObj.BeforeSaveProcessor = this.BeforeSaveProcessor;
            cloneObj.DefaultValue = this.DefaultValue;
            cloneObj.DefaultValueIsProcessor = this.DefaultValueIsProcessor;
            cloneObj.JoinInsert = this.JoinInsert;
            cloneObj.JoinUpdate = this.JoinUpdate;
            return cloneObj;
        }

    }
}
