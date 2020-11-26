using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentDateTime: IProcessor
    {
        public object Execute(Model model, string prop, dynamic obj)
        {
            return DateTime.Now;
        }
    }
}
