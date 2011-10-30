using System;

namespace NDisruptor
{
    internal class FatalExceptionHandler : IExceptionHandler
    {
        public void handle(Exception ex, long sequence, object @event)
        {
            throw new NotImplementedException();
        }
    }
}