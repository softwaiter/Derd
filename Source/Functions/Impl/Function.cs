using CodeM.Common.Orm.SQL.Dialect;
using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public class Function
    {
        protected object[] mArguments = null;

        public Function(params object[] args)
        {
            mArguments = args;
        }

        internal object[] Arguments
        {
            get { return mArguments; }
        }

        internal virtual bool IsUndefined()
        {
            return false;
        }

        internal virtual bool IsProperty()
        {
            return false;
        }

        internal virtual bool IsValue()
        {
            return false;
        }

        internal virtual bool IsDistinct()
        {
            return false;
        }

        internal bool IsIncludeDistinct()
        {
            bool bRet = IsDistinct();
            if (!bRet)
            {
                if (mArguments != null)
                {
                    for (int i = 0; i < mArguments.Length; i++)
                    {
                        if (mArguments[i] is Function &&
                            ((Function)mArguments[i]).IsIncludeDistinct())
                        {
                            return true;
                        }
                    }
                }
            }
            return bRet;
        }

        private object[] TranslateArguments(Model m)
        {
            List<object> result = new List<object>();
            if (mArguments != null)
            {
                foreach (object arg in mArguments)
                {
                    if (arg is Function)
                    {
                        result.Add(((Function)arg).Convert2SQL(m));
                    }
                    else if (arg is string)
                    {
                        if (CommandUtils.IsProperty(m, arg.ToString(), out Property p))
                        {
                            string[] quotes = Features.GetObjectQuotes(p.Owner);
                            result.Add(string.Concat(quotes[0], p.Owner.Table,
                                quotes[1], ".", quotes[0], p.Field, quotes[1]));
                        }
                        else
                        {
                            result.Add(string.Concat("'", arg, "'"));
                        }
                    }
                    else
                    {
                        result.Add(arg);
                    }
                }
            }
            return result.ToArray();
        }

        internal virtual string Convert2SQL(Model m)
        {
            string funcName = this.GetType().Name.ToUpper();
            object[] args = TranslateArguments(m);
            return Features.GetFunctionCommand(m, funcName, args);
        }
    }
}
