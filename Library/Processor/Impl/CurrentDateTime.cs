using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentDateTime: IProcessor
    {
        public dynamic Execute(Model model, string key, dynamic value)
        {
            return DateTime.Now;
        }
    }
}
