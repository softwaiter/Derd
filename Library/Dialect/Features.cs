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

        /// <summary>
        /// 判断数据库类型是否支持整数自增功能
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static bool IsSupportAutoIncrement(Model model)
        {
            return Config.GetConfigValue<bool>("feature", model, "autoincrement");
        }
    }
}
