using CodeM.Common.DbHelper;
using CodeM.Common.Orm.Processor;
using System;
using System.IO;

namespace CodeM.Common.Orm
{
    public class OrmUtils
    {
        public static string ModelPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models");

        /// <summary>
        /// 注册属性Processor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classname"></param>
        public static void RegisterProcessor(string name, string classname)
        {
            Executor.Register(name, classname);
        }

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
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                m.CreateTable(force);
            }
            return true;
        }

        public static bool RemoveTables()
        {
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                m.RemoveTable();
            }
            return true;
        }

        public static bool TruncateTables()
        {
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                m.TruncateTable();
            }
            return true;
        }

    }
}
