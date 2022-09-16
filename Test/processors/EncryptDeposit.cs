using CodeM.Common.Orm;

namespace UnitTest.Processors
{
    public class EncryptDeposit : IPropertyProcessor
    {
        public object Process(Model modelDefine, string propName, dynamic propValue)
        {
            if (propValue != null)
            {
                return propValue + 100000;
            }
            return NotSet.Value;
        }
    }
}
