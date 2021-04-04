using CodeM.Common.Orm;

namespace UnitTest.Processors
{
    public class DecryptDeposit : IProcessor
    {
        public object Execute(Model model, string prop, dynamic propValue)
        {
            if (propValue != null)
            {
                return propValue - 100000;
            }
            return Undefined.Value;
        }
    }
}
