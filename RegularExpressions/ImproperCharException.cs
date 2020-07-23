using System;
using System.Runtime.Serialization;

namespace RegularExpressions
{
    [Serializable]
    internal class ImproperCharException : Exception
    {
        public ImproperCharException()
        {
        }

        public ImproperCharException(string message) : base(message)
        {
        }

        public ImproperCharException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImproperCharException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}