using CodeM.Common.DbHelper;
using CodeM.Common.Orm.Dialect;
using System;
using System.Data;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    internal class CommandUtils
    {
        public static DbType GetDbParamType(Property p)
        {
            if (p.FieldType == DbType.Boolean)
            {
                return DbType.Int16;
            }
            return p.FieldType;
        }

        public static string GenParamName(Property p) 
        {
            string suffix = DateTime.Now.ToString("FFFFFF");
            return string.Concat(p.Name, "$", suffix);
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
