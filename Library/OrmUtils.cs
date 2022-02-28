using CodeM.Common.DbHelper;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

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
            Processor.Register(name, classname);
        }

        /// <summary>
        /// 加载模型定义
        /// </summary>
        public static void Load()
        {
            ModelLoader.Load(ModelPath);
            ModelUtils.AddVersionControlModel();
        }

        /// <summary>
        /// 运行时，如果模型定义有变化，可调用此方法进行刷新，属于增量加载
        /// </summary>
        public static void Refresh()
        {
            ModelLoader.Load(ModelPath, true);
        }

        #region Debug
        public static bool sAllowDebug = false;
        public static void EnableDebug(bool enable)
        {
            sAllowDebug = enable;
        }

        internal static void PrintSQL(string sql, params DbParameter[] sqlParams)
        {
            if (sAllowDebug)
            {
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine(sql);
                if (sqlParams.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (DbParameter dp in sqlParams)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", ");
                        }
                        sb.Append(dp.Value.ToString());
                    }
                    Console.WriteLine(sb);
                }
                Console.WriteLine("================================================");
                Console.WriteLine();
            }
        }
        #endregion

        #region transaction
        private static ConcurrentDictionary<int, DbTransaction> sTransactions = new ConcurrentDictionary<int, DbTransaction>();

        internal static DbTransaction GetTransaction(int code)
        {
            DbTransaction trans = null;
            sTransactions.TryGetValue(code, out trans);
            return trans;
        }

        public static int GetTransaction()
        {
            DbTransaction trans = DbUtils.GetTransaction("/");
            sTransactions.AddOrUpdate(trans.GetHashCode(), trans, (key, value) =>
            {
                return trans;
            });
            return trans.GetHashCode();
        }

        public static int GetTransaction(string path, 
            IsolationLevel level=IsolationLevel.Unspecified)
        {
            DbTransaction trans = DbUtils.GetTransaction(path.ToLower(), level);
            sTransactions.AddOrUpdate(trans.GetHashCode(), trans, (key, value) =>
            {
                return trans;
            });
            return trans.GetHashCode();
        }

        public static bool CommitTransaction(int code)
        {
            DbTransaction trans;
            if (sTransactions.TryGetValue(code, out trans))
            {
                DbUtils.CommitTransaction(trans);
                return true;
            }
            return false;
        }

        public static bool RollbackTransaction(int code)
        {
            DbTransaction trans;
            if (sTransactions.TryGetValue(code, out trans))
            {
                DbUtils.RollbackTransaction(trans);
                return true;
            }
            return false;
        }
        #endregion

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

        public static int ExecSql(string sql, int transCode)
        {
            DbTransaction trans;
            if (sTransactions.TryGetValue(transCode, out trans))
            {
                return DbUtils.ExecuteNonQuery(trans, sql);
            }
            return -1;
        }

        public static void CreateTables(bool force = false)
        {
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                m.CreateTable(force);
            }
        }

        public static bool TryCreateTables(bool force = false)
        {
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                if (!m.TryCreateTable(force))
                {
                    return false;
                }
            }
            return true;
        }

        public static void RemoveTables()
        {
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                m.RemoveTable();
            }
        }

        public static bool TryRemoveTables()
        {
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                if (!m.TryRemoveTable())
                {
                    return false;
                }
            }
            return true;
        }

        public static void TruncateTables()
        {
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                m.TruncateTable();
            }
        }

        public static bool TryTruncateTables()
        {
            int count = ModelUtils.ModelCount;
            for (int i = 0; i < count; i++)
            {
                Model m = ModelUtils.Get(i);
                if (!m.TryTruncateTable())
                {
                    return false;
                }
            }
            return true;
        }

        public static bool EnableVersionControl()
        {
            if (!ModelUtils.VersionControlTableExists())
            {
                if (ModelUtils.CreateVersionControlTable())
                {
                    return ModelUtils.SetVersion(0);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (ModelUtils.GetVersion() == -1)
                {
                    return ModelUtils.SetVersion(0);
                }
            }
            return true;
        }

        public static int GetVersion()
        {
            return ModelUtils.GetVersion();
        }

        public static bool SetVersion(int newVer, int? transCode = null)
        {
            int currVer = GetVersion();
            if (currVer >= 0)
            {
                if (newVer > currVer)
                {
                    return ModelUtils.SetVersion(newVer, transCode);
                }
            }
            return false;
        }

    }
}
