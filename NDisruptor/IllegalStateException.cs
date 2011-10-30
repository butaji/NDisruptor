using System;

namespace NDisruptor
{
    public class IllegalStateException : Exception
    {
        public IllegalStateException(string threadIsAlreadyRunning)
        {
            throw new NotImplementedException();
        }
    }
}