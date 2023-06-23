using CodeM.Common.Orm.SQL.Dialect;
using System;

namespace CodeM.Common.Orm.SQL
{
    internal class SelectFieldPart
    {
        private Function mFunction;
        private int mIndex;
        private string mAlias;

        public SelectFieldPart(Function function, int index, string alias = null)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }

            mFunction = function;
            mIndex = index;
            mAlias = alias;
        }

        public Function Function
        {
            get { return mFunction; }
        }

        public string PropertyName
        {
            get
            {
                if (mFunction.IsProperty())
                {
                    return ((PROPERTY)mFunction).Value;
                }
                return null;
            }
        }

        public string FieldName
        {
            get
            {
                return this.OutputName.Replace(".", "_");
            }
        }

        public string OutputName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(mAlias))
                {
                    return mAlias.Trim();
                }

                if (mFunction is PROPERTY)
                {
                    return ((PROPERTY)mFunction).Value;
                }

                return string.Concat("Custom", mIndex);
            }
        }

        public string Convert2SQL(Model m)
        {
            string[] quotes = Features.GetObjectQuotes(m);
            string[] aliasQuotes = new string[] { quotes[0], quotes[1] };
            string[] specAliasQuotes = Features.GetFieldAliasQuotes(m);
            if (specAliasQuotes.Length > 0)
            {
                aliasQuotes = specAliasQuotes;
            }
            return string.Concat(mFunction.Convert2SQL(m), " AS ",
                aliasQuotes[0], this.FieldName, aliasQuotes[1]);
        }
    }
}
