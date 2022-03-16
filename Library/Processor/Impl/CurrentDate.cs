using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentDate : IProcessor
    {
        public dynamic Execute(Model model, string key, dynamic value)
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
