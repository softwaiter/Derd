using CodeM.Common.Orm;
using CodeM.Common.Tools;

namespace UnitTest.Processors
{
    public class EncryptDeposit : IProcessor
    {
        public object Execute(Model model, string prop, dynamic propValue)
        {
            if (propValue != null)
            {
                return propValue + 100000;
            }
            return Undefined.Value;
        }
    }
}
