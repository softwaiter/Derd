using CodeM.Common.Orm;
using CodeM.Common.Tools;

namespace Test.Processors
{
    public class DecryptMobile : IPropertyProcessor
    {
        public object Process(Model modelDefine, string propName, dynamic propValue)
        {
            if (propValue != null)
            {
                return Xmtool.Crypto().Base64Decode(propValue);
            }
            return NotSet.Value;
        }
    }
}
