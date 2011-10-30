using System;

namespace NDisruptor
{
public interface ExceptionHandler
{
    void handle(Exception ex, long sequence, Object @event);
}
}