using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace CodeM.Common.Orm
{
    internal class ModelUtils
    {
        #region global data
        internal static ConcurrentDictionary<string, Model> sModels = new ConcurrentDictionary<string, Model>();

        internal static void ClearModels()
        {
            sModels.Clear();
        }

        internal static bool AddModel(string parent, Model model)
        {
            string key = parent;
            if (!key.EndsWith("/"))
            {
                key = string.Concat(key, "/");
            }
            key = string.Concat(key, model.Name);

            return sModels.TryAdd(key.ToLower(), model);
        }

        internal static string GetFullModelName(string name)
        {
            if (!name.StartsWith("/"))
            {
                name = "/" + name;
            }
            if (name.EndsWith("/"))
            {
                name = name.Substring(0, name.Length - 1);
            }
            return name.Trim();
        }

        internal static bool IsDefined(string modelName)
        {
            string fullModelName = GetFullModelName(modelName);
            return sModels.ContainsKey(fullModelName.ToLower());
        }

        internal static Model GetModel(string modelName)
        {
            string fullModelName = GetFullModelName(modelName);
            if (sModels.ContainsKey(fullModelName.ToLower()))
            {
                Model m = sModels[fullModelName.ToLower()];
                return (Model)m.Clone();
            }
            return null;
        }

        internal static int ModelCount
        {
            get
            {
                return sModels.Count;
            }
        }

        internal static Model Get(int index)
        {
            KeyValuePair<string, Model>[] models = sModels.ToArray();
            if (index >= 0 && index < models.Length)
            {
                Model m = models[index].Value;
                return (Model)m.Clone();
            }
            return null;
        }
        #endregion

        #region Version Model
        private static string sVerModelName = "VersionControl";
        private static string sVerModelTableName = "t_version_control";

        internal static void AddVersionControlModel()
        {
            Model m = new Model();
            m.Path = "/";
            m.Name = sVerModelName;
            m.Table = sVerModelTableName;

            Property p = new Property();
            p.Name = "Id";
            p.Type = typeof(Int32);
            p.RealType = p.Type;
            p.Field = "f_id";
            p.FieldType = DbType.Int32;
            p.IsPrimaryKey = true;
            p.AutoIncrement = true;
            p.Unsigned = true;
            p.JoinInsert = false;
            p.JoinUpdate = false;
            p.Description = "自动生成主键";
            m.AddProperty(p);

            Property p2 = new Property();
            p2.Name = "Path";
            p2.Type = typeof(String);
            p2.RealType = p2.Type;
            p2.Field = "f_path";
            p2.FieldType = DbType.String;
            p2.Length = 255;
            p2.IsNotNull = true;
            p2.JoinInsert = true;
            p2.JoinUpdate = false;
            p2.DefaultValue = "{{CurrentDateTime}}";
            p2.Description = "模型路径";
            p2.UniqueGroup = "uc_path_versoin";
            m.AddProperty(p2);

            Property p3 = new Property();
            p3.Name = "Version";
            p3.Type = typeof(Int32);
            p3.RealType = p3.Type;
            p3.Field = "f_version";
            p3.FieldType = DbType.Int32;
            p3.Unsigned = true;
            p3.IsNotNull = true;
            p3.JoinInsert = true;
            p3.JoinUpdate = false;
            p3.Description = "版本号";
            p3.UniqueGroup = "uc_path_versoin";
            m.AddProperty(p3);

            Property p4 = new Property();
            p4.Name = "CreateTime";
            p4.Type = typeof(DateTime);
            p4.RealType = p4.Type;
            p4.Field = "f_createtime";
            p4.FieldType = DbType.DateTime;
            p4.JoinInsert = true;
            p4.JoinUpdate = false;
            p4.DefaultValue = "{{CurrentDateTime}}";
            p4.Description = "创建时间";
            m.AddProperty(p4);

            AddModel("/", m);
        }

        internal static bool VersionControlTableExists()
        {
            if (IsDefined(sVerModelName))
            {
                return GetModel(sVerModelName).TableExists();
            }
            return false;
        }

        internal static bool CreateVersionControlTable()
        {
            return GetModel(sVerModelName).TryCreateTable(false);
        }

        internal static int GetVersion(string path = "/")
        {
            return GetVersion(path, null);
        }

        internal static int GetVersion(string path, int? transCode = null)
        {
            Model m = ModelUtils.GetModel(sVerModelName);
            if (m.TableExists())
            {
                dynamic result = m.Equals("Path", path).DescendingSort("Id").QueryFirst(transCode);
                if (result != null)
                {
                    return result.Version;
                }
            }
            return 0;
        }

        internal static bool SetVersion(string path, int version, int? transCode = null)
        {
            Model m = ModelUtils.GetModel(sVerModelName);
            return m.SetValue("Path", path)
                .SetValue("Version", version)
                .Save(transCode);
        }
        #endregion
    }
}
