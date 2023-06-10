using System;

namespace CodeM.Common.Orm
{
    public class Function
    {
        private string mPropertyName;
        private Function mChildFunction;

        public Function(string propName)
        {
            if (string.IsNullOrWhiteSpace(propName))
            {
                throw new ArgumentNullException("propName");
            }

            mPropertyName = propName;
        }

        public Function(Function function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }

            mChildFunction = function;
        }

        public string PropertyName
        {
            get 
            { 
                return mPropertyName; 
            }
        }

        public Function ChildFunction
        { 
            get 
            { 
                return mChildFunction; 
            } 
        }
    }
}
