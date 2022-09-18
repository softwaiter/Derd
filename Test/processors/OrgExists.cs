using CodeM.Common.Orm;
using System;

namespace Test.Processors
{
    public class OrgExists : IRuleProcessor
    {
        public void Validate(Property prop, dynamic value)
        {
            dynamic orgObj = Derd.Model("Org").Equals("Code", value).QueryFirst();
            if (orgObj == null)
            {
                throw new Exception(String.Concat("指定Org未找到：", value));
            }
        }
    }
}
