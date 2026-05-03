using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentUtcTime : IPropertyProcessor
    {
        public object Process(Model modelDeine, string propName, dynamic propValue)
        {
            return DateTime.UtcNow.ToString("HH:mm:ss");
        }
    }
}
