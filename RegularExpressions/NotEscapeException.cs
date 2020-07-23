using System;
using System.Runtime.Serialization;

namespace RegularExpressions
{
    [Serializable]
    internal class NotEscapeException : Exception
    {
        public NotEscapeException()
        {
            Console.WriteLine("The string escapes a character that is not an escapable regex character.");
        }

        public NotEscapeException(string message) : base(message)
        {
        }

        public NotEscapeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotEscapeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}