using System;
using System.Collections.Concurrent;

namespace CodeM.Common.Orm
{
    internal class ModelUtils
    {
        internal static ConcurrentDictionary<string, ConnectionSetting> sConnectionSettings = new ConcurrentDictionary<string, ConnectionSetting>();

        internal static ConcurrentDictionary<string, Model> sModels = new ConcurrentDictionary<string, Model>();
        internal static ConcurrentDictionary<string, ConnectionSetting> sModelConnections = new ConcurrentDictionary<string, ConnectionSetting>();

        internal static void ClearConnections()
        {
            sConnectionSettings.Clear();
            sModelConnections.Clear();
        }

        internal static bool AddConnection(string path, ConnectionSetting conn)
        {
            return sConnectionSettings.TryAdd(path.ToLower(), conn);
        }

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
                return sModels[fullModelName.ToLower()];
            }
            return null;
        }

        internal static ConnectionSetting GetConnectionByModel(Model model)
        {
            string fullModelName = model.Path;
            if (fullModelName.EndsWith("/"))
            {
                fullModelName += model.Name;
            }
            else
            {
                fullModelName += ("/" + model.Name);
            }
            return GetConnectionByModelName(fullModelName);
        }

        internal static ConnectionSetting GetConnectionByModelName(string modelName)
        {
            string fullModelName = GetFullModelName(modelName).ToLower();
            if (sModelConnections.ContainsKey(fullModelName))
            {
                ConnectionSetting result;
                if (sModelConnections.TryGetValue(fullModelName, out result))
                {
                    return result;
                }
            }

            string modelPath = fullModelName;
            int pos = modelPath.LastIndexOf("/");
            while (pos >= 0)
            {
                modelPath = modelPath.Substring(0, pos);
                if (pos == 0)
                {
                    modelPath = "/";
                }

                if (sConnectionSettings.ContainsKey(modelPath))
                {
                    ConnectionSetting conns = sConnectionSettings[modelPath];
                    sModelConnections.AddOrUpdate(fullModelName, conns, (key, value) =>
                    {
                        return conns;
                    });
                    return conns;
                }

                if (pos == 0)
                {
                    break;
                }
            }

            throw new Exception("未定义数据源：" + modelName);
        }

    }
}
