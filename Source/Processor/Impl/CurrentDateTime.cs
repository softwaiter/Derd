using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentDateTime: IPropertyProcessor
    {
        public object Process(Model modelDeine, string propName, dynamic propValue)
        {
            return DateTime.Now;
        }
    }
}
