using CodeM.Common.Orm.Functions.Impl;
using System;

namespace CodeM.Common.Orm
{
    internal class PropertyChecker
    {
        /// <summary>
        /// 检查属性在指定模型定义中是否存在
        /// </summary>
        /// <param name="m"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string CheckValueProperty(Model m, string name)
        {
            string compactName = name.Trim();
            if (!compactName.Contains("."))
            {
                Property p = m.GetProperty(compactName);
                if (p == null)
                {
                    throw new Exception(string.Concat("未找到属性：", compactName));
                }
            }
            else
            {
                string[] typeItems = compactName.Split(".");
                Model currM = m;
                for (int i = 0; i < typeItems.Length; i++)
                {
                    Property p = currM.GetProperty(typeItems[i]);
                    if (p == null)
                    {
                        throw new Exception(string.Concat("未找到属性：", compactName));
                    }

                    if (i < typeItems.Length - 1)
                    {
                        currM = ModelUtils.GetModel(p.TypeValue);
                        if (currM == null)
                        {
                            throw new Exception(string.Concat("非法的Model引用：", p.TypeValue));
                        }
                    }
                }
            }
            return compactName;
        }

        /// <summary>
        /// 检查函数属性在指定模型定义中是否存在且合法
        /// </summary>
        /// <param name="m"></param>
        /// <param name="function"></param>
        /// <returns>是否包含DISTINCT聚合函数</returns>
        public static bool CheckFunctionProperty(Model m, Function function)
        {
            bool includeDistinct = false;

            Function currFunction = function;
            while (string.IsNullOrWhiteSpace(currFunction.PropertyName))
            {
                if (!includeDistinct && currFunction is DISTINCT)
                {
                    includeDistinct = true;
                }
                currFunction = currFunction.ChildFunction;
            }
            CheckValueProperty(m, currFunction.PropertyName);

            return includeDistinct;
        }
    }
}
