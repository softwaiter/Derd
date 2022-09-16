using CodeM.Common.Ioc;
using System;
using System.Collections.Concurrent;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    internal class Processor
    {
        private static bool sInited = false;
        private static ConcurrentDictionary<string, object> sProcessorImpls = new ConcurrentDictionary<string, object>();

        public static void Init()
        {
            if (!sInited)
            {
                Register("CurrentDateTime", "CodeM.Common.Orm.Processors.CurrentDateTime");
                Register("CurrentDate", "CodeM.Common.Orm.Processors.CurrentDate");
                Register("CurrentTime", "CodeM.Common.Orm.Processors.CurrentTime");
                Register("Upper", "CodeM.Common.Orm.Processors.Upper");
                Register("Lower", "CodeM.Common.Orm.Processors.Lower");

                sInited = true;
            }
        }

        public static void Register(string name, string classname)
        {
            object inst = Wukong.GetSingleObject(classname);
            if (inst != null)
            {
                if (inst is IPropertyProcessor || inst is IModelProcessor)
                {
                    sProcessorImpls.AddOrUpdate(name.ToLower(), inst, (key, value) =>
                    {
                        return inst;
                    });
                    return;
                }
                throw new Exception(string.Concat("无效的Processor：", classname));
            }
            throw new Exception(string.Concat("Processor实现未找到：", classname));
        }

        public static object CallPropertyProcessor(string processorName, 
            Model modelDefine, string propName, dynamic propValue)
        {
            dynamic inst;
            if (sProcessorImpls.TryGetValue(processorName.ToLower(), out inst))
            {
                if (inst is IPropertyProcessor)
                {
                    return ((IPropertyProcessor)inst).Process(modelDefine, propName, propValue);
                }
                throw new Exception(string.Concat("无效的PropertyProcessor：", processorName));
            }
            throw new Exception(string.Concat("Processor不存在：", processorName));
        }

        public static bool CallModelProcessor(string processorName,
            Model modelDefine, dynamic modelObj, DbTransaction trans = null)
        {
            dynamic inst;
            if (sProcessorImpls.TryGetValue(processorName.ToLower(), out inst))
            {
                if (inst is IModelProcessor)
                {
                    return ((IModelProcessor)inst).Process(modelDefine, modelObj, trans);
                }
                throw new Exception(string.Concat("无效的ModelProcessor：", processorName));
            }
            throw new Exception(string.Concat("Processor不存在：", processorName));
        }
    }
}
