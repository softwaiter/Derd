using CodeM.Common.DbHelper;
using CodeM.Common.Orm.SQL.Dialect;
using CodeM.Common.Tools.DynamicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    internal class CommandUtils
    {
        #region property
        public static bool IsProperty(Model m, object value, out Property p)
        {
            p = null;

            if (!(value is Function))
            {
                string compactName = ("" + value).Trim();
                if (!string.IsNullOrWhiteSpace(compactName))
                {
                    if (!compactName.Contains("."))
                    {
                        if (m.HasProperty(compactName))
                        {
                            p = m.GetProperty(compactName);
                            return true;
                        }
                    }
                    else
                    {
                        string[] typeItems = compactName.Split(".");
                        Model currM = m;
                        for (int i = 0; i < typeItems.Length - 1; i++)
                        {
                            if (currM.HasProperty(typeItems[i]))
                            {
                                Property subProp = currM.GetProperty(typeItems[i]);
                                currM = ModelUtils.GetModel(subProp.TypeValue);
                                if (currM == null)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                currM = null;
                                break;
                            }
                        }

                        if (currM != null)
                        {
                            if (currM.HasProperty(typeItems[typeItems.Length - 1]))
                            {
                                p = currM.GetProperty(typeItems[typeItems.Length - 1]);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsProperty(Model m, object value)
        {
            return IsProperty(m, value, out Property p);
        }
        #endregion

        #region Type DbType

        public static Type DbType2Type(DbType type)
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
        public static DbType Type2DbType(Type type)
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
        #endregion


        /// <summary>
        /// 将所有跨模型引用加入关联列表中，最终将转换为join语句
        /// </summary>
        /// <param name="function"></param>
        /// <param name="foreignTables"></param>
        public static void CheckFunctionForeignProperty(Model m, Function function, List<string> foreignTables)
        {
            if (!function.IsValue())
            {
                foreach (object arg in function.Arguments)
                {
                    if (arg is string)
                    {
                        if (("" + arg).Contains(".") &&
                            CommandUtils.IsProperty(m, arg))
                        {
                            foreignTables.Add("" + arg);
                        }
                    }
                    else if (arg is PROPERTY)
                    {
                        PROPERTY propFunc = (PROPERTY)arg;
                        if (propFunc.Value.Contains("."))
                        {
                            foreignTables.Add(propFunc.Value);
                        }
                    }
                    else if (arg is Function)
                    {
                        CheckFunctionForeignProperty(m, (Function)arg, foreignTables);
                    }
                }
            }
        }

        public static DbType GetDbParamType(Property p)
        {
            if (p.FieldType == DbType.Boolean &&
                Features.IsUseIntegerInsteadOfBool(p.Owner))
            {
                return DbType.Int16;
            }
            return p.FieldType;
        }

        public static string GenParamName(Property p) 
        {
            string suffix = DateTime.Now.ToString("FFFFFF");
            return string.Concat(p.Name, suffix);
        }

        public static string GenParamName()
        {
            string suffix = DateTime.Now.ToString("FFFFFF");
            return string.Concat("constant", suffix);
        }

        public static int ExecuteNonQuery(Model m, string datasourceName, string commandText, params DbParameter[] commandParams)
        {
            if (Features.IsSupportMultiCommand(m))
            {
                return DbUtils.ExecuteNonQuery(datasourceName, commandText, commandParams);
            }
            else
            {
                int iRet = 0;

                string[] cmdItems = commandText.Split(';');
                foreach (string cmdItem in cmdItems)
                {
                    if (!string.IsNullOrWhiteSpace(cmdItem))
                    {
                        iRet = DbUtils.ExecuteNonQuery(datasourceName, cmdItem, commandParams);
                    }
                }

                return iRet;
            }
        }

        public static int ExecuteNonQuery(Model m, DbTransaction transaction, string commandText, params DbParameter[] commandParams)
        {
            if (Features.IsSupportMultiCommand(m))
            {
                return DbUtils.ExecuteNonQuery(transaction, commandText, commandParams);
            }
            else
            {
                int iRet = 0;

                string[] cmdItems = commandText.Split(';');
                foreach (string cmdItem in cmdItems)
                {
                    if (!string.IsNullOrWhiteSpace(cmdItem))
                    {
                        iRet = DbUtils.ExecuteNonQuery(transaction, cmdItem, commandParams);
                    }
                }

                return iRet;
            }
        }
    }
}
