using System.Data;

namespace CodeM.Common.Orm.SQL.Dialect
{
    internal class FieldUtils
    {
        /// <summary>
        /// 根据数据库字段类型返回对应数据库的字符串表示
        /// </summary>
        /// <param name="model"></param>
        /// <param name="_typ"></param>
        /// <returns></returns>
        internal static string GetFieldType(Model model, DbType _typ)
        {
            return Config.GetConfigValue<string>("fieldtype", model, _typ);
        }

        /// <summary>
        /// 根据数据库字段类型返回对应数据库的默认长度
        /// </summary>
        /// <param name="model"></param>
        /// <param name="_typ"></param>
        /// <returns></returns>
        internal static int GetFieldLength(Model model, DbType _typ)
        {
            return Config.GetConfigValue<int>("fieldlength", model, _typ);
        }

        /// <summary>
        /// 根据数据库类型返回自增字段的标记
        /// </summary>
        /// <param name="model"></param>
        /// <param name="_typ"></param>
        /// <returns></returns>
        internal static string GetFieldAutoIncrementTag(Model model)
        {
            return Config.GetConfigValue<string>("autoincrtag", model, "tag");
        }

        /// <summary>
        /// 判断字段类型是否为整数类型
        /// </summary>
        /// <param name="_typ"></param>
        /// <returns></returns>
        internal static bool IsInteger(DbType _typ)
        {
            return _typ == DbType.Byte || _typ == DbType.SByte ||
                _typ == DbType.Int16 || _typ == DbType.Int32 ||
                _typ == DbType.Int64 || _typ == DbType.UInt16 ||
                _typ == DbType.UInt32 || _typ == DbType.UInt64;
        }

        /// <summary>
        /// 判断字段类型是否为浮点数类型
        /// </summary>
        /// <param name="_typ"></param>
        /// <returns></returns>
        internal static bool IsFloat(DbType _typ)
        {
            return _typ == DbType.Single ||
                _typ == DbType.Double ||
                _typ == DbType.Decimal;
        }

        /// <summary>
        /// 判断字段类型是否为数值型（即整数或浮点数）
        /// </summary>
        /// <param name="_typ"></param>
        /// <returns></returns>
        internal static bool IsNumeric(DbType _typ)
        {
            return IsInteger(_typ) || IsFloat(_typ);
        }
    }
}
