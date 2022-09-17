using CodeM.Common.Orm.Dialect;
using CodeM.Common.Tools;
using CodeM.Common.Tools.DynamicObject;
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
        internal class DelayCheckPropertySetting
        {
            public string Position { get; set; }

            public bool FieldTypeNotSet { get; set; } = false;

            public bool LengthNotSet { get; set; } = false;
        }

        #region 遍历Model目录，逐个解析Model定义
        internal static ConcurrentDictionary<string, DateTime> sModelFileUpdateTimes = new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        /// 存储模型解析过程中需要延后校验的属性定义
        /// </summary>
        internal static ConcurrentDictionary<Property, DelayCheckPropertySetting> sDelayCheckProperties = new ConcurrentDictionary<Property, DelayCheckPropertySetting>();

        internal static void Load(string modelPath, bool increment = false)
        {
            //初始化属性Processor
            Processor.Init();

            ConnectionUtils.ClearConnections();

            if (!increment)
            {
                ModelUtils.ClearModels();
                sModelFileUpdateTimes.Clear();
            }

            //遍历加载解析模型定义
            DirectoryInfo di = new DirectoryInfo(modelPath);
            EnumDirectory(di, "/", increment);

            //处理模型解析过程中需要延后校验的属性定义
            ProcessDelayCheckProperties();

            //注册数据源
            ConnectionUtils.RegisterAllConnections();
        }

        private static void ProcessDelayCheckProperties()
        {
            IEnumerator<KeyValuePair<Property, DelayCheckPropertySetting>> e = sDelayCheckProperties.GetEnumerator();
            while (e.MoveNext())
            {
                Property p = e.Current.Key;
                DelayCheckPropertySetting dcps = e.Current.Value;
                if (p.Type == typeof(Model))
                {
                    Model m = ModelUtils.GetModel(p.TypeValue);
                    if (m == null)
                    {
                        throw new Exception(string.Concat("Model定义“", p.TypeValue,"”不存在。", dcps.Position));
                    }

                    Property joinP = null;
                    if (!string.IsNullOrWhiteSpace(p.JoinProp))
                    {
                        try
                        {
                            joinP = m.GetProperty(p.JoinProp);
                        }
                        catch
                        {
                            throw new Exception(string.Concat("关联模型“", p.TypeValue, "”中不存在属性“", p.JoinProp, "”的定义。", dcps.Position));
                        }
                    }

                    if (joinP == null)
                    {
                        joinP = m.GetPrimaryKey(0);
                    }

                    //TODO 多级关联属性这里会有问题，如果是多级关联，此时joinP的类型可能还是Model
                    p.RealType = joinP.Type;

                    if (dcps.FieldTypeNotSet)
                    {
                        p.FieldType = joinP.FieldType;
                    }
                    if (dcps.LengthNotSet)
                    {
                        p.Length = joinP.Length;
                    }
                }
            }
        }

        private static void EnumDirectory(DirectoryInfo di, string parent, bool increment)
        {
            FileInfo fiConn = new FileInfo(Path.Combine(di.FullName, ".connection.xml"));
            if (fiConn.Exists)
            {
                //就一个文件，无论是否increment都重新解析一遍
                ParseConnectionFile(fiConn, parent);
            }
            else if ("/".Equals(parent))
            {
                throw new Exception("模型定义根目录必须配置数据库连接.connection.xml文件。");
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
            conn.DataSource = parent.ToLower();
            ConnectionUtils.AddConnection(parent, conn);
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
            Model model = ParseModel(modelFilePath, parent);
            if (model.PrimaryKeyCount == 0)
            {
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
                model.AddProperty(p);
            }
            ModelUtils.AddModel(parent, model);
        }

        #endregion

        private static Regex reBool = new Regex("^(true|false)$", RegexOptions.IgnoreCase);

        #region 解析Connection定义文件

        private static ConnectionSetting ParseConnectionSetting(string connectionFilePath)
        {
            ConnectionSetting result = new ConnectionSetting();
            Xmtool.Xml().Iterate(connectionFilePath, (nodeInfo) => {
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
                    else if (nodeInfo.Path == "/connection/charset/@text")
                    {
                        result.Charset = nodeInfo.Text.Trim();
                    }
                    else if (nodeInfo.Path == "/connection/encrypt/@text")
                    {
                        if (!reBool.IsMatch(nodeInfo.Text.Trim()))
                        {
                            throw new Exception("encrypt节点值必须是布尔型。 " + connectionFilePath + " - Line " + nodeInfo.Line);
                        }
                        result.Encrypt = bool.Parse(nodeInfo.Text.Trim());
                    }
                    else if (nodeInfo.Path == "/connection/pool")
                    {
                        string maxStr = nodeInfo.GetAttribute("max");
                        if (!Xmtool.Regex().IsPositiveInteger(maxStr))
                        {
                            throw new Exception("max属性必须是有效正整数。 " + connectionFilePath + " - Line " + nodeInfo.Line);
                        }
                        result.MaxPoolSize = int.Parse(maxStr);

                        string minStr = nodeInfo.GetAttribute("min");
                        if (!Xmtool.Regex().IsNaturalInteger(minStr))
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
            else if ("json".Equals(typeStr, StringComparison.OrdinalIgnoreCase))
            {
                return typeof(DynamicObjectExt);
            }
            else
            {
                return typeof(Model);
            }
        }

        private static Type DbType2Type(DbType type)
        {
            if (type == DbType.String ||
                type == DbType.AnsiString ||
                type == DbType.AnsiStringFixedLength ||
                type == DbType.StringFixedLength)
            {
                return typeof(string);
            }
            else if (type == DbType.Boolean)
            {
                return typeof(bool);
            }
            else if (type == DbType.Byte)
            {
                return typeof(byte);
            }
            else if (type == DbType.SByte)
            {
                return typeof(sbyte);
            }
            else if (type == DbType.Decimal)
            {
                return typeof(decimal);
            }
            else if (type == DbType.Double)
            {
                return typeof(double);
            }
            else if (type == DbType.Int16)
            {
                return typeof(Int16);
            }
            else if (type == DbType.Int32)
            {
                return typeof(Int32);
            }
            else if (type == DbType.Int64)
            {
                return typeof(Int64);
            }
            else if (type == DbType.Single)
            {
                return typeof(Single);
            }
            else if (type == DbType.UInt16)
            {
                return typeof(UInt16);
            }
            else if (type == DbType.UInt32)
            {
                return typeof(UInt32);
            }
            else if (type == DbType.UInt64)
            {
                return typeof(UInt64);
            }
            else if (type == DbType.Date ||
                type == DbType.DateTime ||
                type == DbType.DateTime2 ||
                type == DbType.DateTimeOffset)
            {
                return typeof(DateTime);
            }
            return typeof(Object);
        }

        /// <summary>
        /// 将数据类型转换为数据库对应类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static DbType Type2DbType(Type type)
        {
            if (type == typeof(string) ||
                type == typeof(DynamicObjectExt))
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
                return DbType.Int32;
            }
            else if (type == typeof(UInt32))
            {
                return DbType.Int64;
            }
            else if (type == typeof(UInt64))
            {
                return DbType.Int64;
            }
            else if (type == typeof(DateTime))
            {
                return DbType.DateTime;
            }
            return DbType.Object;
        }

        internal static Model ParseModel(string modelFilePath, string parent)
        {
            Model model = new Model();
            model.Path = parent.ToLower();

            Property currProp = null;

            Xmtool.Xml().Iterate(modelFilePath, (nodeInfo) =>
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

                        string beforeNewProcStr = nodeInfo.GetAttribute("beforeNew");
                        if (beforeNewProcStr != null)
                        {
                            beforeNewProcStr = beforeNewProcStr.Trim();
                            if (beforeNewProcStr.Length > 4 &&
                                beforeNewProcStr.StartsWith("{{") &&
                                beforeNewProcStr.EndsWith("}}"))
                            {
                                model.BeforeNewProcessor = beforeNewProcStr.Substring(2, beforeNewProcStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("beforeNew属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string afterNewProcStr = nodeInfo.GetAttribute("afterNew");
                        if (afterNewProcStr != null)
                        {
                            afterNewProcStr = afterNewProcStr.Trim();
                            if (afterNewProcStr.Length > 4 &&
                                afterNewProcStr.StartsWith("{{") &&
                                afterNewProcStr.EndsWith("}}"))
                            {
                                model.AfterNewProcessor = afterNewProcStr.Substring(2, afterNewProcStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("afterNew属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string beforeUpdateProcStr = nodeInfo.GetAttribute("beforeUpdate");
                        if (beforeUpdateProcStr != null)
                        {
                            beforeUpdateProcStr = beforeUpdateProcStr.Trim();
                            if (beforeUpdateProcStr.Length > 4 &&
                                beforeUpdateProcStr.StartsWith("{{") &&
                                beforeUpdateProcStr.EndsWith("}}"))
                            {
                                model.BeforeUpdateProcessor = beforeUpdateProcStr.Substring(2, beforeUpdateProcStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("beforeUpdate属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string afterUpdateProcStr = nodeInfo.GetAttribute("afterUpdate");
                        if (afterUpdateProcStr != null)
                        {
                            afterUpdateProcStr = afterUpdateProcStr.Trim();
                            if (afterUpdateProcStr.Length > 4 &&
                                afterUpdateProcStr.StartsWith("{{") &&
                                afterUpdateProcStr.EndsWith("}}"))
                            {
                                model.AfterUpdateProcessor = afterUpdateProcStr.Substring(2, afterUpdateProcStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("afterUpdate属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string beforeDeleteProcStr = nodeInfo.GetAttribute("beforeDelete");
                        if (beforeDeleteProcStr != null)
                        {
                            beforeDeleteProcStr = beforeDeleteProcStr.Trim();
                            if (beforeDeleteProcStr.Length > 4 &&
                                beforeDeleteProcStr.StartsWith("{{") &&
                                beforeDeleteProcStr.EndsWith("}}"))
                            {
                                model.BeforeDeleteProcessor = beforeDeleteProcStr.Substring(2, beforeDeleteProcStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("beforeDelete属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string afterDeleteProcStr = nodeInfo.GetAttribute("afterDelete");
                        if (afterDeleteProcStr != null)
                        {
                            afterDeleteProcStr = afterDeleteProcStr.Trim();
                            if (afterDeleteProcStr.Length > 4 &&
                                afterDeleteProcStr.StartsWith("{{") &&
                                afterDeleteProcStr.EndsWith("}}"))
                            {
                                model.AfterDeleteProcessor = afterDeleteProcStr.Substring(2, afterDeleteProcStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("afterDelete属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }
                    }
                }
                else if (nodeInfo.Path == "/model/property")
                {
                    if (!nodeInfo.IsEndNode)
                    {
                        Property p = new Property();
                        DelayCheckPropertySetting dcps = null;

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
                            p.Field = string.Concat("f_", p.Name.ToLower());
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
                            else if (type == typeof(Model) ||
                                type == typeof(DynamicObjectExt))
                            {
                                p.TypeValue = typeStr;

                                dcps = new DelayCheckPropertySetting();
                                dcps.Position = modelFilePath + " - Line " + nodeInfo.Line;
                            }
                        }
                        else
                        {
                            string aiStr = nodeInfo.GetAttribute("autoIncrement");
                            if (aiStr != null)
                            {
                                if (!string.IsNullOrWhiteSpace(aiStr) &&
                                    reBool.IsMatch(aiStr))
                                {
                                    type = typeof(Int32);
                                }
                            }
                        }
                        p.Type = type;
                        p.RealType = type;

                        string joinPropStr = nodeInfo.GetAttribute("joinProp");
                        if (joinPropStr != null)
                        {
                            if (p.Type == typeof(Model))
                            {
                                if (string.IsNullOrWhiteSpace(joinPropStr))
                                {
                                    throw new Exception("joinProp属性不能为空。" + modelFilePath + " - Line " + nodeInfo.Line);
                                }
                                p.JoinProp = joinPropStr.Trim();
                            }
                            else
                            {
                                throw new Exception("joinProp属性只在type属性为Model类型时有效。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string fieldTypeStr = nodeInfo.GetAttribute("fieldType");
                        if (fieldTypeStr != null)
                        {
                            if (type == typeof(DynamicObjectExt))
                            {
                                throw new Exception(typeStr + "类型的Property禁止设置fieldType。" + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            try
                            {
                                p.FieldType = Enum.Parse<DbType>(fieldTypeStr, true);
                                p.RealType = DbType2Type(p.FieldType);
                            }
                            catch
                            {
                                throw new Exception("fieldType属性非法。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }
                        else
                        {
                            p.FieldType = Type2DbType(type);
                            if (p.Type == typeof(Model))
                            {
                                dcps.FieldTypeNotSet = true;
                            }
                        }

                        string lengthStr = nodeInfo.GetAttribute("length");
                        if (lengthStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(lengthStr))
                            {
                                throw new Exception("length属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            if (!Xmtool.Regex().IsPositiveInteger(lengthStr))
                            {
                                throw new Exception("length属性必须是有效正整数。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            p.Length = int.Parse(lengthStr);
                        }
                        else
                        {
                            p.Length = FieldUtils.GetFieldLength(model, p.FieldType);
                            if (p.Type == typeof(DynamicObjectExt))
                            {
                                p.Length = 512;
                            }
                            else if (p.Type == typeof(Model))
                            {
                                dcps.LengthNotSet = true;
                            }
                        }

                        string minStr = nodeInfo.GetAttribute("min");
                        if (minStr != null)
                        {
                            if (FieldUtils.IsNumeric(p.FieldType))
                            {
                                if (string.IsNullOrWhiteSpace(minStr))
                                {
                                    throw new Exception("min属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                if (!Xmtool.Regex().IsNumber(minStr))
                                {
                                    throw new Exception("min属性必须是有效数值。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                p.MinValue = double.Parse(minStr);
                            }
                            else
                            {
                                throw new NotSupportedException(p.FieldType.ToString() + "类型不支持min设置。" + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string maxStr = nodeInfo.GetAttribute("max");
                        if (maxStr != null)
                        {
                            if (FieldUtils.IsNumeric(p.FieldType))
                            {
                                if (string.IsNullOrWhiteSpace(maxStr))
                                {
                                    throw new Exception("max属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                if (!Xmtool.Regex().IsNumber(maxStr))
                                {
                                    throw new Exception("max属性必须是有效数值。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                p.MaxValue = double.Parse(maxStr);
                            }
                            else
                            {
                                throw new NotSupportedException(p.FieldType.ToString() + "类型不支持max设置。" + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string precisionStr = nodeInfo.GetAttribute("precision");
                        if (precisionStr != null)
                        {
                            if (FieldUtils.IsFloat(p.FieldType))
                            {
                                if (string.IsNullOrWhiteSpace(precisionStr))
                                {
                                    throw new Exception("precision属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                if (!Xmtool.Regex().IsNaturalInteger(precisionStr))
                                {
                                    throw new Exception("precision属性必须是有效自然数。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                p.Precision = int.Parse(precisionStr);
                            }
                            else
                            {
                                throw new NotSupportedException(p.FieldType.ToString() + "类型不支持precision设置。" + modelFilePath + " - Line " + nodeInfo.Line);
                            }
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

                        string descStr = nodeInfo.GetAttribute("desc");
                        if (!string.IsNullOrEmpty(descStr))
                        {
                            p.Description = descStr;
                        }

                        string autoIncrStr = nodeInfo.GetAttribute("autoIncrement");
                        if (autoIncrStr != null)
                        {
                            if (FieldUtils.IsInteger(p.FieldType))
                            {
                                if (string.IsNullOrWhiteSpace(autoIncrStr))
                                {
                                    throw new Exception("autoIncrement属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                if (!reBool.IsMatch(autoIncrStr))
                                {
                                    throw new Exception("autoIncrement属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                p.AutoIncrement = bool.Parse(autoIncrStr);
                            }
                            else
                            {
                                throw new NotSupportedException(p.FieldType.ToString() + "类型不支持autoIncrement设置。" + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string unsignedStr = nodeInfo.GetAttribute("unsigned");
                        if (unsignedStr != null)
                        {
                            if (FieldUtils.IsNumeric(p.FieldType))
                            {
                                if (string.IsNullOrWhiteSpace(unsignedStr))
                                {
                                    throw new Exception("unsigned属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                if (!reBool.IsMatch(unsignedStr))
                                {
                                    throw new Exception("unsigned属性必须是布尔型。 " + modelFilePath + " - Line " + nodeInfo.Line);
                                }

                                p.Unsigned = bool.Parse(unsignedStr);
                            }
                            else
                            {
                                throw new NotSupportedException(p.FieldType.ToString() + "类型不支持unsigned设置。" + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string uniqueGroupStr = nodeInfo.GetAttribute("uniqueGroup");
                        if (uniqueGroupStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(uniqueGroupStr))
                            {
                                throw new Exception("uniqueGroup属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                            p.UniqueGroup = uniqueGroupStr.Trim();
                        }

                        string indexGroupStr = nodeInfo.GetAttribute("indexGroup");
                        if (indexGroupStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(indexGroupStr))
                            {
                                throw new Exception("indexGroup属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                            p.IndexGroup = indexGroupStr.Trim();
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
                        }

                        string defaultStr = nodeInfo.GetAttribute("defaultValue");
                        if (defaultStr != null)
                        {
                            p.DefaultValue = defaultStr.Trim();
                        }

                        string beforeProcStr = nodeInfo.GetAttribute("beforeSave");
                        if (beforeProcStr != null)
                        {
                            beforeProcStr = beforeProcStr.Trim();
                            if (beforeProcStr.Length > 4 &&
                                beforeProcStr.StartsWith("{{") &&
                                beforeProcStr.EndsWith("}}"))
                            {
                                p.BeforeSaveProcessor = beforeProcStr.Substring(2, beforeProcStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("beforeSave属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string afterProcStr = nodeInfo.GetAttribute("afterQuery");
                        if (afterProcStr != null)
                        {
                            afterProcStr = afterProcStr.Trim();
                            if (afterProcStr.Length > 4 &&
                                afterProcStr.StartsWith("{{") &&
                                afterProcStr.EndsWith("}}"))
                            {
                                p.AfterQueryProcessor = afterProcStr.Substring(2, afterProcStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("afterQuery属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
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

                        if (dcps != null)
                        {
                            sDelayCheckProperties.TryAdd(p, dcps);
                        }

                        currProp = p;
                    }
                    else
                    {
                        currProp = null;
                    }
                }
                else if (nodeInfo.Path == "/model/property/rule")
                {
                    if (!nodeInfo.IsEndNode)
                    {
                        PropertyRule rule = new PropertyRule();

                        string patternStr = nodeInfo.GetAttribute("pattern");
                        if (patternStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(patternStr))
                            {
                                throw new Exception("pattern属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            RulePattern pattern;
                            if (Enum.TryParse<RulePattern>(patternStr, true, out pattern))
                            {
                                rule.Pattern = pattern;
                            }
                        }

                        string regexStr = nodeInfo.GetAttribute("regex");
                        if (regexStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(regexStr))
                            {
                                throw new Exception("regex属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            rule.Regex = new Regex(regexStr, RegexOptions.Compiled);
                        }

                        string validationStr = nodeInfo.GetAttribute("validation");
                        if (validationStr != null)
                        {
                            validationStr = validationStr.Trim();
                            if (validationStr.Length > 4 &&
                                validationStr.StartsWith("{{") &&
                                validationStr.EndsWith("}}"))
                            {
                                rule.ValidationProcessor = validationStr.Substring(2, validationStr.Length - 4);
                            }
                            else
                            {
                                throw new Exception("validation属性必须是Processor。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }
                        }

                        string messageStr = nodeInfo.GetAttribute("message");
                        if (messageStr != null)
                        {
                            if (string.IsNullOrWhiteSpace(messageStr))
                            {
                                throw new Exception("message属性不能为空。 " + modelFilePath + " - Line " + nodeInfo.Line);
                            }

                            rule.Message = messageStr;
                        }

                        currProp.Rules.Add(rule);
                    }
                }
                
                return true;
            });
            return model;
        }

        #endregion
    }
}
