using System.Data.Common;

namespace CodeM.Common.Orm
{
    public interface IModelProcessor
    {
        public bool Process(Model modelDefine, dynamic modelObj, DbTransaction trans = null);
    }
}
