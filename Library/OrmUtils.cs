using CodeM.Common.Tools.Xml;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CodeM.Common.Orm
{
    public class OrmUtils
    {
        private static ConcurrentDictionary<string, Model> sModels = new ConcurrentDictionary<string, Model>();
        private static ConcurrentDictionary<string, DateTime> sModelUpdateTimes = new ConcurrentDictionary<string, DateTime>();

        private static ConcurrentDictionary<string, ConnectionSetting> sConnectionSettings = new ConcurrentDictionary<string, ConnectionSetting>();

        public static string ModelPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "models");

        public static void ParseConnectionFile(FileInfo connectionFile, string parent)
        {
            string connectionFilePath = connectionFile.FullName;

            XmlUtils.Iterate(connectionFilePath, (nodeInfo) => {
                return true;
            });
        }

        private static void ParseModelFile(FileInfo modelFile, string parent)
        {
            string modelFilePath = modelFile.FullName;

            Regex reInt = new Regex("^[1-9][0-9]*$");
            Regex reBool = new Regex("^(true|false)$", RegexOptions.IgnoreCase);

            Model model = new Model();
            XmlUtils.Iterate(modelFilePath, (nodeInfo) =>
            {
                if (nodeInfo.Path == "/model")
                {
                    if (!nodeInfo.IsEndNode)
                    {
                        string name = nodeInfo.GetAttribute("name");
                        if (name == null)
                        {
                            throw new Exception("缺少name属性。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }
                        else if (string.IsNullOrWhiteSpace(name))
                        {
                            throw new Exception("name属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }
                        model.Name = name.Trim();

                        string table = nodeInfo.GetAttribute("table");
                        if (table != null)
                        {
                            if (string.IsNullOrWhiteSpace(table))
                            {
                                throw new Exception("table属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                            model.Table = table.Trim();
                        }
                        else
                        {
                            model.Table = model.Name;
                        }
                    }
                    else
                    {
                        string key = parent;
                        if (!key.EndsWith("/"))
                        {
                            key = string.Concat(key, "/");
                        }
                        key = string.Concat(key, model.Name);
                        sModels.TryAdd(key.ToLower(), model);
                    }
                }
                else if (nodeInfo.Path == "/model/property")
                {
                    Property p = new Property();

                    string name = nodeInfo.GetAttribute("name");
                    if (name == null)
                    {
                        throw new Exception("缺少name属性。 " + modelFilePath + " - Line " + nodeInfo.Line);
                    }
                    else if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new Exception("name属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                    }
                    p.Name = name.Trim();

                    string field = nodeInfo.GetAttribute("field");
                    if (field != null)
                    {
                        if (string.IsNullOrWhiteSpace(field))
                        {
                            throw new Exception("field属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }
                        p.Field = field.Trim();
                    }
                    else
                    {
                        p.Field = p.Name;
                    }

                    string length = nodeInfo.GetAttribute("length");
                    if (length != null)
                    {
                        if (string.IsNullOrWhiteSpace(length))
                        {
                            throw new Exception("length属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        if (!reInt.IsMatch(length))
                        {
                            throw new Exception("length属性必须是有效正整数。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        p.Length = int.Parse(length);
                    }

                    string desc = nodeInfo.GetAttribute("desc");
                    if (!string.IsNullOrEmpty(desc))
                    {
                        p.Description = desc;
                    }

                    string notNull = nodeInfo.GetAttribute("notNull");
                    if (notNull != null)
                    {
                        if (string.IsNullOrWhiteSpace(notNull))
                        {
                            throw new Exception("notNull属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        if (!reBool.IsMatch(notNull))
                        {
                            throw new Exception("notNull属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        p.IsNotNull = bool.Parse(notNull);
                    }

                    string unique = nodeInfo.GetAttribute("unique");
                    if (unique != null)
                    {
                        if (string.IsNullOrWhiteSpace(unique))
                        {
                            throw new Exception("unique属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        if (!reBool.IsMatch(unique))
                        {
                            throw new Exception("unique属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        p.IsUnique = bool.Parse(unique);
                    }

                    string primary = nodeInfo.GetAttribute("primary");
                    if (primary != null)
                    {
                        if (string.IsNullOrWhiteSpace(primary))
                        {
                            throw new Exception("primary属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        if (!reBool.IsMatch(primary))
                        {
                            throw new Exception("primary属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        p.IsPrimaryKey = bool.Parse(primary);
                    }

                    string joinInsert = nodeInfo.GetAttribute("joinInsert");
                    if (joinInsert != null)
                    {
                        if (string.IsNullOrWhiteSpace(joinInsert))
                        {
                            throw new Exception("joinInsert属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        if (!reBool.IsMatch(joinInsert))
                        {
                            throw new Exception("joinInsert属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        p.JoinInsert = bool.Parse(joinInsert);
                    }

                    string joinUpdate = nodeInfo.GetAttribute("joinUpdate");
                    if (joinUpdate != null)
                    {
                        if (string.IsNullOrWhiteSpace(joinUpdate))
                        {
                            throw new Exception("joinUpdate属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        if (!reBool.IsMatch(joinUpdate))
                        {
                            throw new Exception("joinUpdate属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                        }

                        p.JoinUpdate = bool.Parse(joinUpdate);
                    }

                    model.AddProperty(p);
                }

                return true;
            });
        }

        private static bool ModelFileIsModified(FileInfo modelFile)
        {
            string filename = modelFile.FullName.ToLower();
            if (sModelUpdateTimes.ContainsKey(filename))
            {
                DateTime prevUpdateTime;
                if (sModelUpdateTimes.TryGetValue(filename, out prevUpdateTime))
                {
                    return modelFile.LastWriteTime > prevUpdateTime;
                }
            }
            return true;
        }

        private static void EnumDirectory(DirectoryInfo di, string parent, bool increment)
        {
            FileInfo fiConn = new FileInfo(Path.Combine(di.FullName, ".connection.xml"));
            if (fiConn.Exists)
            {
                //就一个文件，无论是否increment都重新解析一遍
                ParseConnectionFile(fiConn, parent);
            }

            IEnumerable<FileInfo> modelFiles = di.EnumerateFiles("*.model.xml", SearchOption.TopDirectoryOnly);
            IEnumerator<FileInfo> modelFilesEnumerator = modelFiles.GetEnumerator();
            while (modelFilesEnumerator.MoveNext())
            {
                if (!increment || ModelFileIsModified(modelFilesEnumerator.Current))
                {
                    ParseModelFile(modelFilesEnumerator.Current, parent);
                }
            }

            IEnumerable<DirectoryInfo> subDirs = di.EnumerateDirectories();
            IEnumerator<DirectoryInfo> subDirsEnumerator = subDirs.GetEnumerator();
            while (subDirsEnumerator.MoveNext())
            {
                EnumDirectory(subDirsEnumerator.Current,
                    string.Concat(parent, subDirsEnumerator.Current.Name, "/"),
                    increment);
            }
        }

        private static void LoadModels(bool increment = false)
        {
            if (!increment)
            {
                sModels.Clear();
                sModelUpdateTimes.Clear();
            }

            DirectoryInfo di = new DirectoryInfo(ModelPath);
            EnumDirectory(di, "/", increment);
        }

        /// <summary>
        /// 加载模型定义
        /// </summary>
        public static void Load()
        {
            //LoadConnections();
            LoadModels();
        }

        /// <summary>
        /// 运行时，如果模型定义有变化，可调用此方法进行刷新，属于增量加载
        /// </summary>
        public static void Refresh()
        {
            LoadModels(true);
        }

        private static string GetFullModelName(string name)
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

        public static bool IsDefind(string modelName)
        {
            string fullModelName = GetFullModelName(modelName);
            return sModels.ContainsKey(fullModelName.ToLower());
        }

        public static object Model(string modelName)
        {
            return false;
        }

        public static object ExecSql(string sql, string path = "/")
        {
            return false;
        }

        public static bool CreateTables(string path, bool recursion)
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
