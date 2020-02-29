using CodeM.Common.DbHelper;
using System;
using System.IO;

namespace CodeM.Common.Orm
{
    public class OrmUtils
    {
        public static string ModelPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "models");

        /// <summary>
        /// 加载模型定义
        /// </summary>
        public static void Load()
        {
            ModelLoader.Load(ModelPath);
        }

        /// <summary>
        /// 运行时，如果模型定义有变化，可调用此方法进行刷新，属于增量加载
        /// </summary>
        public static void Refresh()
        {
            ModelLoader.Load(ModelPath, true);
        }

        public static bool IsDefind(string modelName)
        {
            return ModelUtils.IsDefined(modelName);
        }

        public static Model Model(string modelName)
        {
            return ModelUtils.GetModel(modelName);
        }

        public static bool ExecSql(string sql, string path = "/")
        {
            return DbUtils.ExecuteNonQuery(path.ToLower(), sql) == 0;
        }

        public static bool CreateTables(string path, bool force = false)
        {
            return false;
        }

        public static bool RemoveTables()
        {
            return false;
        }

        public static bool TruncateTables()
        {
            return false;
        }

    }
}
