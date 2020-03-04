using CodeM.Common.Orm.Dialect;
using System;
using System.Data;
using System.Text;

namespace CodeM.Common.Orm
{
    public class Property: ICloneable
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
        /// 属性是否参与插入操作
        /// </summary>
        public bool JoinInsert { get; set; } = true;

        /// <summary>
        /// 属性是否参与更新操作
        /// </summary>
        public bool JoinUpdate { get; set; } = true;

        private object mValue = null;
        /// <summary>
        /// 
        /// </summary>
        public object Value {
            get
            {
                return mValue;
            }
            set
            {
                SetValue(value);
            }
        }

        private void SetValue(object value)
        {
            SetValue(value, true);
        }

        private void SetValue(object value, bool throwError)
        {
            if (value != mValue)
            {
                if (value == null && IsNotNull)
                {
                    throw new NoNullAllowedException(Name);
                }

                if (value != null && Length > 0 && Type == typeof(String))
                {
                    if (value.ToString().Length > Length)
                    {
                        throw new Exception(string.Concat("属性“", Name, "”允许的最大长度 ", Length));
                    }
                }

                mValue = value;

                IsModified = true;
            }
        }

        public object DefaultValue { get; internal set; }

        public bool IsModified { get; set; } = false;

        public string AsString
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToString(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public Int16? AsInt16
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToInt16(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public Int32? AsInt32
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToInt32(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public Int64? AsInt64
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToInt64(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public UInt16? AsUInt16
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToUInt16(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public UInt32? AsUInt32
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToUInt32(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public UInt64? AsUInt64
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToUInt64(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public float? AsFloat
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToSingle(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public decimal? AsDecimal
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToDecimal(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public double? AsDouble
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToDouble(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public bool? AsBoolean
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToBoolean(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public DateTime? AsDateTime
        {
            get
            {
                if (mValue != null)
                {
                    return Convert.ToDateTime(mValue);
                }
                return null;
            }
            set
            {
                SetValue(value);
            }
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
            cloneObj.IsNotNull = this.IsNotNull;
            cloneObj.IsPrimaryKey = this.IsPrimaryKey;
            cloneObj.DefaultValue = this.DefaultValue;
            cloneObj.JoinInsert = this.JoinInsert;
            cloneObj.JoinUpdate = this.JoinUpdate;
            cloneObj.IsModified = this.IsModified;
            return cloneObj;
        }

    }
}
