using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentTime : IProcessor
    {
        public dynamic Execute(Model model, string key, dynamic value)
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
