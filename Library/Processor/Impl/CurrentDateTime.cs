using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentDateTime: IProcessor
    {
        public dynamic Execute(Model model, string prop, dynamic propValue)
        {
            return DateTime.Now;
        }
    }
}
