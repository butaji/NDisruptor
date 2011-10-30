using System.Threading;
using NDisruptor.util;

namespace NDisruptor
{
    internal class OverridedThreadLocal <T> : ThreadLocal<T>
    {
        public MutableLong initialValue()
        {
            return new MutableLong(Sequencer.INITIAL_CURSOR_VALUE);
        }

        public MutableLong get()
        {
            throw new System.NotImplementedException();
        }
    }
}