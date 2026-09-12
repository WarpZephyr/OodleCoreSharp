using System;

namespace OodleCoreSharp.Exceptions
{
    /// <summary>
    /// An exception for when an oodle library to interop with is not found.
    /// </summary>
    public class OodleNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="OodleNotFoundException"/>.
        /// </summary>
        public OodleNotFoundException() { }

        /// <summary>
        /// Creates a new <see cref="OodleNotFoundException"/> with the specified message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public OodleNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="OodleNotFoundException"/> with the specified message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public OodleNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
