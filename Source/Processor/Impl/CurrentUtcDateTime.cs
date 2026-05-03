using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentUtcDateTime : IPropertyProcessor
    {
        public object Process(Model modelDefine, string propName, dynamic propValue)
        {
            return DateTime.UtcNow;
        }
    }
}
