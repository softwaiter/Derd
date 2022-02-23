namespace CodeM.Common.Orm.Dialect
{
    internal class Features
    {
        /// <summary>
        /// 判断数据库类型是否支持无符号数值
        /// </summary>
        /// <param name="dialect"></param>
        /// <returns></returns>
        internal static bool IsSupportUnsigned(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "unsigned");
        }

        /// <summary>
        /// 判断数据库类型是否支持Truncate操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static bool IsSupportTruncate(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "truncate");
        }

        /// <summary>
        /// 判断数据库类型是否支持注释功能
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static bool IsSupportComment(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "comment");
        }

        internal static string GetCommentExtCommand(Model model, string tableName, string colName, string colDesc)
        {
            string extStr = Config.GetConfigValue<string>("feature", model, "comment_ext_format");
            return string.Format(extStr, tableName, colName, colDesc);
        }

        /// <summary>
        /// 判断数据库类型是否支持整数自增功能
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static bool IsSupportAutoIncrement(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "autoincrement");
        }

        internal static string GetTableExistsSql(Model model)
        {
            return Config.GetConfigValue<string>("feature", model, "exists_sql_format");
        }

        internal static bool IsSupportSelectForUpdate(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "select_forupdate");
        }

        internal static string[] GetObjectQuotes(Model model)
        {
            return Config.GetConfigValue<string[]>("feature", model, "object_quote");
        }

        internal static string GetCommandParamName(Model model, string name)
        {
            string paramStr = Config.GetConfigValue<string>("feature", model, "command_param_format");
            return string.Format(paramStr, name);
        }
    }
}
