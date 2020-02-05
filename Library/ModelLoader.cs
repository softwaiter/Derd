using CodeM.Common.Tools.Xml;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace CodeM.Common.Orm
{
    internal class ModelLoader
    {

        #region 遍历Model目录，逐个解析Model定义
        internal static ConcurrentDictionary<string, DateTime> sModelFileUpdateTimes = new ConcurrentDictionary<string, DateTime>();

        internal static void Load(string modelPath, bool increment = false)
        {
            ModelUtils.ClearConnections();

            if (!increment)
            {
                ModelUtils.ClearModels();
                sModelFileUpdateTimes.Clear();
            }

            DirectoryInfo di = new DirectoryInfo(modelPath);
            EnumDirectory(di, "/", increment);

            //TODO 注册数据源
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

        private static void ParseConnectionFile(FileInfo connectionFile, string parent)
        {
            string connectionFilePath = connectionFile.FullName;
            ConnectionSetting conn = ParseConnectionSetting(connectionFilePath);
            conn.ModelPath = parent.ToLower();
            ModelUtils.AddConnection(parent, conn);
        }

        private static bool ModelFileIsModified(FileInfo modelFile)
        {
            string filename = modelFile.FullName.ToLower();
            if (sModelFileUpdateTimes.ContainsKey(filename))
            {
                DateTime prevUpdateTime;
                if (sModelFileUpdateTimes.TryGetValue(filename, out prevUpdateTime))
                {
                    return modelFile.LastWriteTime > prevUpdateTime;
                }
            }
            return true;
        }

        private static void ParseModelFile(FileInfo modelFile, string parent)
        {
            string modelFilePath = modelFile.FullName;
            Model model = ParseModel(modelFilePath);
            ModelUtils.AddModel(parent, model);
        }

        #endregion

        private static Regex reNaturalInt = new Regex("^[0-9]+$");
        private static Regex rePositiveInt = new Regex("^[1-9][0-9]*$");
        private static Regex reBool = new Regex("^(true|false)$", RegexOptions.IgnoreCase);

        #region 解析Connection定义文件

        private static ConnectionSetting ParseConnectionSetting(string connectionFilePath)
        {
            ConnectionSetting result = new ConnectionSetting();
            XmlUtils.Iterate(connectionFilePath, (nodeInfo) => {
                if (!nodeInfo.IsEndNode)
                {
                    if (nodeInfo.Path == "/connection/dialect/@text")
                    {
                        result.Dialect = nodeInfo.Text.Trim().ToLower();
                    }
                    else if (nodeInfo.Path == "/connection/host/@text")
                    {
                        result.Host = nodeInfo.Text.Trim();
                    }
                    else if (nodeInfo.Path == "/connection/port/@text")
                    {
                        int port;
                        if (int.TryParse(nodeInfo.Text, out port))
                        {
                            result.Port = port;
                        }
                        else
                        {
                            throw new Exception("name属性不能为空。 " + connectionFilePath + " - Line " + nodeInfo.Line);
                        }
                    }
                    else if (nodeInfo.Path == "/connection/user/@text")
                    {
                        result.User = nodeInfo.Text.Trim();
                    }
                    else if (nodeInfo.Path == "/connection/password/@text")
                    {
                        result.Password = nodeInfo.Text.Trim();
                    }
                    else if (nodeInfo.Path == "/connection/database/@text")
                    {
                        result.Database = nodeInfo.Text.Trim();
                    }
                    else if (nodeInfo.Path == "/connection/pool")
                    {
                        string maxStr = nodeInfo.GetAttribute("max");
                        if (!rePositiveInt.IsMatch(maxStr))
                        {
                            throw new Exception("max属性必须是有效正整数。 " + connectionFilePath + " - Line " + nodeInfo.Line);
                        }
                        result.MaxPoolSize = int.Parse(maxStr);

                        string minStr = nodeInfo.GetAttribute("min");
                        if (!reNaturalInt.IsMatch(minStr))
                        {
                            throw new Exception("min属性必须是有效自然数。 " + connectionFilePath + " - Line " + nodeInfo.Line);
                        }
                        result.MinPoolSize = int.Parse(minStr);
                    }
                    else if (nodeInfo.Path == "/connection/pool/@text")
                    {
                        if (!reBool.IsMatch(nodeInfo.Text.Trim()))
                        {
                            throw new Exception("pool节点值必须是布尔型。 " + connectionFilePath + " - Line " + nodeInfo.Line);
                        }
                        result.Pooling = bool.Parse(nodeInfo.Text.Trim());
                    }
                }
                return true;
            });
            return result;
        }
        #endregion

        #region 解析Model定义文件

        /// <summary>
        /// 根据字符串定义得到属性值类型
        /// </summary>
        /// <param name="typeStr"></param>
        /// <returns></returns>
        private static Type GetPropertyType(string typeStr)
        {
            if (Enum.IsDefined(typeof(TypeCode), typeStr))
            {
                try
                {
                    return Type.GetType(string.Concat("System.", typeStr), true, true);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return typeof(Model);
            }
        }

        /// <summary>
        /// 将数据类型转换为数据库对应类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static DbType Type2DbType(Type type)
        {
            if (type == typeof(string))
            {
                return DbType.String;
            }
            else if (type == typeof(bool))
            {
                return DbType.Boolean;
            }
            else if (type == typeof(byte))
            {
                return DbType.Byte;
            }
            else if (type == typeof(sbyte))
            {
                return DbType.SByte;
            }
            else if (type == typeof(decimal))
            {
                return DbType.Decimal;
            }
            else if (type == typeof(double))
            {
                return DbType.Double;
            }
            else if (type == typeof(Int16))
            {
                return DbType.Int16;
            }
            else if (type == typeof(Int32))
            {
                return DbType.Int32;
            }
            else if (type == typeof(Int64))
            {
                return DbType.Int64;
            }
            else if (type == typeof(Single))
            {
                return DbType.Single;
            }
            else if (type == typeof(UInt16))
            {
                return DbType.UInt16;
            }
            else if (type == typeof(UInt32))
            {
                return DbType.UInt32;
            }
            else if (type == typeof(UInt64))
            {
                return DbType.UInt64;
            }
            else if (type == typeof(DateTime))
            {
                return DbType.DateTime;
            }
            return DbType.Object;
        }

        internal static Model ParseModel(string modelFilePath)
        {
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
                }
                else if (nodeInfo.Path == "/model/property")
                {
                    if (!nodeInfo.IsEndNode)
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

                        Type type = typeof(string);
                        string typeStr = nodeInfo.GetAttribute("type");
                        if (typeStr != null)
                        {
                            type = GetPropertyType(typeStr);
                            if (type == null)
                            {
                                throw new Exception("type属性非法。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }
                        p.Type = type;

                        p.FieldType = Type2DbType(type);
                        string fieldTypeStr = nodeInfo.GetAttribute("fieldType");
                        if (fieldTypeStr != null)
                        {
                            try
                            {
                                p.FieldType = Enum.Parse<DbType>(fieldTypeStr, true);
                            }
                            catch
                            {
                                throw new Exception("fieldType属性非法。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string lengthStr = nodeInfo.GetAttribute("length");
                        if (lengthStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(lengthStr))
                            {
                                throw new Exception("length属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            if (!rePositiveInt.IsMatch(lengthStr))
                            {
                                throw new Exception("length属性必须是有效正整数。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            p.Length = int.Parse(lengthStr);
                        }

                        string descStr = nodeInfo.GetAttribute("desc");
                        if (!string.IsNullOrEmpty(descStr))
                        {
                            p.Description = descStr;
                        }

                        string notNullStr = nodeInfo.GetAttribute("notNull");
                        if (notNullStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(notNullStr))
                            {
                                throw new Exception("notNull属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            if (!reBool.IsMatch(notNullStr))
                            {
                                throw new Exception("notNull属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            p.IsNotNull = bool.Parse(notNullStr);
                        }

                        string uniqueStr = nodeInfo.GetAttribute("unique");
                        if (uniqueStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(uniqueStr))
                            {
                                throw new Exception("unique属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            if (!reBool.IsMatch(uniqueStr))
                            {
                                throw new Exception("unique属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            p.IsUnique = bool.Parse(uniqueStr);
                        }

                        string primaryStr = nodeInfo.GetAttribute("primary");
                        if (primaryStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(primaryStr))
                            {
                                throw new Exception("primary属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            if (!reBool.IsMatch(primaryStr))
                            {
                                throw new Exception("primary属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            p.IsPrimaryKey = bool.Parse(primaryStr);
                            if (p.IsPrimaryKey)
                            {
                                p.IsUnique = true;
                            }
                        }

                        string joinInsertStr = nodeInfo.GetAttribute("joinInsert");
                        if (joinInsertStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(joinInsertStr))
                            {
                                throw new Exception("joinInsert属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            if (!reBool.IsMatch(joinInsertStr))
                            {
                                throw new Exception("joinInsert属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            p.JoinInsert = bool.Parse(joinInsertStr);
                        }

                        string joinUpdateStr = nodeInfo.GetAttribute("joinUpdate");
                        if (joinUpdateStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(joinUpdateStr))
                            {
                                throw new Exception("joinUpdate属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            if (!reBool.IsMatch(joinUpdateStr))
                            {
                                throw new Exception("joinUpdate属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            p.JoinUpdate = bool.Parse(joinUpdateStr);
                        }

                        model.AddProperty(p);
                    }
                }

                return true;
            });
            return model;
        }

        #endregion
    }
}
