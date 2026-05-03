using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentUtcDate : IPropertyProcessor
    {
        public object Process(Model modelDefine, string propName, dynamic propValue)
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd");
        }
    }
}
