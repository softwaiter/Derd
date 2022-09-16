using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentDate : IPropertyProcessor
    {
        public object Process(Model modelDefine, string propName, dynamic propValue)
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
