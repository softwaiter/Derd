using System;

namespace CodeM.Common.Orm
{
    public class PropertyValue
    {
        protected object[] mArguments = null;

        public PropertyValue(params object[] args)
        {
            mArguments = args;
        }

        internal object[] Arguments
        {
            get { return mArguments; }
        }

        internal virtual string Convert2SQL(Property p)
        {
            throw new NotSupportedException();
        }
    }
}
