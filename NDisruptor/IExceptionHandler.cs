using System;

namespace NDisruptor
{
public interface IExceptionHandler
{
    void handle(Exception ex, long sequence, Object @event);
}
}