using System;
using System.Runtime.Serialization;

namespace CodeM.Common.Orm
{
    public class PropertyValidationException : Exception
    {
        public PropertyValidationException()
            : base()
        { 
        }

        public PropertyValidationException(string message)
            : base(message)
        { 
        }

        public PropertyValidationException(string message, Exception innerException)
            : base(message, innerException)
        { 
        }

        public PropertyValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
    }
}
