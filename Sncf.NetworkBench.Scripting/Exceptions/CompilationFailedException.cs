using System;

namespace Sncf.NetworkBench.Scripting.Exceptions
{
    [Serializable]
    public class CompilationFailedException : Exception
    {
        public CompilationFailedException(string message) : base(message)
        {
        }
    }
}
