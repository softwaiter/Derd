using System;

namespace CodeM.Common.Orm.Processor.Impl
{
    public class CurrentDateTime: IExecute
    {
        public object Execute(Model model, dynamic obj)
        {
            return DateTime.Now;
        }
    }
}
