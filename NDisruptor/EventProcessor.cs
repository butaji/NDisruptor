using System;

namespace NDisruptor
{
    public interface EventProcessor : Runnable
    {
        Sequence getSequence();

        void halt();
    }

}