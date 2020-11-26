using CodeM.Common.Orm;

namespace UnitTest.Processors
{
    public class EncryptDeposit : IProcessor
    {
        public object Execute(Model model, string prop, dynamic obj)
        {
            if (obj.Has(prop))
            {
                if (obj[prop] != null)
                {
                    return obj[prop] + 100000;
                }
            }
            return Undefined.Value;
        }
    }
}
