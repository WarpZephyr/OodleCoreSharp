using System;

namespace OodleCoreSharp.Exceptions
{
    public class OodleNotFoundException : Exception
    {
        public OodleNotFoundException(string message) : base(message) { }
    }
}
