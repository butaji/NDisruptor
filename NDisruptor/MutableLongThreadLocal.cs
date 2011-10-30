using System.Threading;
using NDisruptor.util;

namespace NDisruptor
{
    internal class MutableLongThreadLocal : ThreadLocal<MutableLong>
    {
        public MutableLongThreadLocal()
        {
            Value = new MutableLong(Sequencer.INITIAL_CURSOR_VALUE);
        }

        public MutableLong get()
        {
            return this.Value;
        }
    }
}