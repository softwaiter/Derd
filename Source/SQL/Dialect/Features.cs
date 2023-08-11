namespace CodeM.Common.Orm.SQL.Dialect
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

        /// <summary>
        /// 获取注释的个性化写法
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <param name="colDesc"></param>
        /// <returns></returns>
        internal static string GetCommentExtCommand(Model model, string tableName, string colName, string colDesc)
        {
            string extStr = Config.GetConfigValue<string>("feature", model, "comment_ext_format");
            return string.Format(extStr, tableName, colName, colDesc);
        }

        /// <summary>
        /// 判断创建表时是否在命令尾部需要增加补充命令信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static bool IsNeedCreateSuffix(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "create_suffix");
        }

        /// <summary>
        /// 获取创建表命令的补充命令信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        internal static string GetCreateSuffixExtCommand(Model model, string databaseName)
        {
            string extStr = Config.GetConfigValue<string>("feature", model, "create_suffix_ext_format");
            return string.Format(extStr, databaseName);
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

        /// <summary>
        /// 查询数据库用于支持自增序列的特殊字段类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static string GetAutoIncrementReplaceType(Model model)
        {
            return Config.GetConfigValue<string>("feature", model, "autoincrement_type_replace");
        }

        /// <summary>
        /// 获取AutoIncrement的个性化写法
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <param name="colDesc"></param>
        /// <returns></returns>
        internal static string[] GetAutoIncrementExtCommand(Model model, string tableName, string colName)
        {
            string[] extCmds = Config.GetConfigValue<string[]>("feature", model, "autoincrement_ext_format");
            string[] result = new string[extCmds.Length];
            if (extCmds != null && extCmds.Length > 0)
            {
                for (int i = 0; i < extCmds.Length; i++)
                {
                    result[i] = string.Format(extCmds[i], tableName, colName);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取RemoveTable时需要清理的Autoincrement相关命令信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        internal static string[] GetAutoIncrementGCExtCommand(Model model, string tableName, string colName)
        {
            string[] extCmds = Config.GetConfigValue<string[]>("feature", model, "autoincrement_gc_ext_format");
            string[] result = new string[extCmds.Length];
            if (extCmds != null && extCmds.Length > 0)
            {
                for (int i = 0; i < extCmds.Length; i++)
                {
                    result[i] = string.Format(extCmds[i], tableName, colName);
                }
            }
            return result;
        }

        internal static string GetTableExistsSql(Model model, string database, string table)
        {
            string existsSql = Config.GetConfigValue<string>("feature", model, "exists_sql_format");
            return string.Format(existsSql, database, table);
        }

        internal static bool IsSupportSelectForUpdate(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "select_forupdate");
        }

        /// <summary>
        /// 获取命令行中Table对象和字段对象的包围符
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static string[] GetObjectQuotes(Model model)
        {
            return Config.GetConfigValue<string[]>("feature", model, "object_quote");
        }

        /// <summary>
        /// 获取查询语句中字段别名的个性化包围符（默认同object_quote包围符）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static string[] GetFieldAliasQuotes(Model model)
        {
            return Config.GetConfigValue<string[]>("feature", model, "field_alias_quote");
        }


        internal static string GetCommandParamName(Model model, string name)
        {
            string paramStr = Config.GetConfigValue<string>("feature", model, "command_param_format");
            return string.Format(paramStr, name);
        }

        internal static string GetPagingCommand(Model model, string sql, int pagesize, int pageindex)
        {
            string pagingStr = Config.GetConfigValue<string>("feature", model, "paging_command_format");
            return string.Format(pagingStr, sql, pagesize, pageindex, (pageindex - 1) * pagesize, pageindex * pagesize);
        }

        internal static string GetFunctionCommand(Model model, string funcName, params object[] args)
        {
            string funcCmd = Config.GetConfigValue<string>("function", model, funcName.Trim().ToUpper());
            return string.Format(funcCmd, args);
        }

        internal static string GetFunctionReturnType(Model model, string funcName)
        {
            string funcType = Config.GetConfigValue<string>("functiontype", model, funcName.Trim().ToUpper());
            return funcType;
        }

        internal static bool IsSupportMultiCommand(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "exec_multi_command");
        }

        /// <summary>
        /// 是否构建参数时使用整型代替布尔型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static bool IsUseIntegerInsteadOfBool(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "boolean_is_int");
        }
    }
}
