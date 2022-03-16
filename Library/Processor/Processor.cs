using CodeM.Common.Ioc;
using System;
using System.Collections.Concurrent;

namespace CodeM.Common.Orm
{
    internal class Processor
    {
        private static bool sInited = false;
        private static ConcurrentDictionary<string, IProcessor> sImpls = new ConcurrentDictionary<string, IProcessor>();

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
            IProcessor inst = IocUtils.GetSingleObject<IProcessor>(classname);
            if (inst != null)
            {
                sImpls.AddOrUpdate(name.ToLower(), inst, (key, value) =>
                {
                    return inst;
                });
                return;
            }
            throw new Exception(string.Concat("Processor实现未找到：", classname));
        }

        public static object Call(string method, Model model, string prop, dynamic propValue)
        {
            IProcessor inst;
            if (sImpls.TryGetValue(method.ToLower(), out inst))
            {
                return inst.Execute(model, prop, propValue);
            }
            throw new Exception(string.Concat("Processor不存在：", method));
        }
    }
}
