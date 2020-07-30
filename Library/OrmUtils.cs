using CodeM.Common.DbHelper;
using System;
using System.IO;

namespace CodeM.Common.Orm
{
    public class OrmUtils
    {
        public static string ModelPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models");

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

        public static int ExecSql(string sql, string path = "/")
        {
            return DbUtils.ExecuteNonQuery(path.ToLower(), sql);
        }

        public static bool CreateTables(bool force = false)
        {
            try
            {
                int count = ModelUtils.ModelCount;
                for (int i = 0; i < count; i++)
                {
                    Model m = ModelUtils.Get(i);
                    m.CreateTable(force);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool RemoveTables()
        {
            try
            {
                int count = ModelUtils.ModelCount;
                for (int i = 0; i < count; i++)
                {
                    Model m = ModelUtils.Get(i);
                    m.RemoveTable();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool TruncateTables()
        {
            try
            {
                int count = ModelUtils.ModelCount;
                for (int i = 0; i < count; i++)
                {
                    Model m = ModelUtils.Get(i);
                    m.TruncateTable();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
