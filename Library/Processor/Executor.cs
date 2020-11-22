using CodeM.Common.Ioc;
using System;
using System.Collections.Generic;

namespace CodeM.Common.Orm.Processor
{
    internal class Executor
    {
        private static bool sInited = false;
        private static Dictionary<string, IExecute> sImpls = new Dictionary<string, IExecute>();

        public static void Init()
        {
            if (!sInited)
            {
                Register("CurrentDateTime", "CodeM.Common.Orm.Processor.Impl.CurrentDateTime");

                sInited = true;
            }
        }

        public static void Register(string name, string classname)
        {
            IExecute inst = IocUtils.GetSingleObject<IExecute>(classname);
            if (inst != null)
            {
                sImpls.Add(name.ToLower(), inst);
                return;
            }
            throw new Exception(string.Concat("Processor实现未找到：", classname));
        }

        public static object Call(string method, Model model, dynamic obj)
        {
            IExecute inst;
            if (sImpls.TryGetValue(method.ToLower(), out inst))
            {
                return inst.Execute(model, obj);
            }
            throw new Exception(string.Concat("属性Processor不存在：", method));
        }
    }
}
