using System;

namespace NDisruptor
{
public class Sequence
{
    private PaddedAtomicLong value = new PaddedAtomicLong(Sequencer.INITIAL_CURSOR_VALUE);

    public Sequence()
    {
    }

    public Sequence(long initialValue)
    {
        set(initialValue);
    }

    public virtual long get()
    {
        return value.get();
    }

    public virtual void set(long value)
    {
        this.value.lazySet(value);
    }

    public String toString()
    {
        return value.get().ToString();
    }
} }