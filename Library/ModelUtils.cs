using System.Collections.Concurrent;

namespace CodeM.Common.Orm
{
    internal class ModelUtils
    {
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
                return sModels[fullModelName.ToLower()];
            }
            return null;
        }

    }
}
