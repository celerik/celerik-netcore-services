using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Exception related to injecting service instances.
    /// </summary>
    [Serializable]
    public class InjectException : Exception
    {
        /// <summary>
        /// Initializes a new InjectException.
        /// </summary>
        public InjectException()
        {
        }

        /// <summary>
        /// Initializes a new InjectException.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InjectException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new InjectException.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current
        /// exception, or a null reference if no inner exception is specified.</param>
        public InjectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new InjectException.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that
        /// holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual
        /// information about the source or destination.</param>
        [ExcludeFromCodeCoverage]
        protected InjectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
