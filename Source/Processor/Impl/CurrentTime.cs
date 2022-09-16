using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentTime : IPropertyProcessor
    {
        public object Process(Model modelDeine, string propName, dynamic propValue)
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
