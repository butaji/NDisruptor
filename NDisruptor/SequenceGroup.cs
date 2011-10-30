using System;

namespace NDisruptor
{
public class SequenceGroup : Sequence
{
    private AtomicReference<Sequence[]> sequencesRef = new AtomicReference<Sequence[]>(new Sequence[0]);

    public SequenceGroup()
    {
    }

    public override long get()
    {
        return Util.getMinimumSequence(sequencesRef.get());
    }

    public override void set(long value)
    {
         Sequence[] sequences = sequencesRef.get();
        for (int i = 0, size = sequences.Length; i < size; i++)
        {
            sequences[i].set(value);
        }
    }

    public void add(Sequence sequence)
    {
        Sequence[] oldSequences;
        Sequence[] newSequences;
        do
        {
            oldSequences = sequencesRef.get();
            int oldSize = oldSequences.Length;
            newSequences = new Sequence[oldSize + 1];
            Array.Copy(oldSequences, 0, newSequences, 0, oldSize);
            newSequences[oldSize] = sequence;
        }
        while (!sequencesRef.compareAndSet(oldSequences, newSequences));
    }

    public bool remove(Sequence sequence)
    {
        bool found = false;
        Sequence[] oldSequences;
        Sequence[] newSequences;
        do
        {
            oldSequences = sequencesRef.get();
            int oldSize = oldSequences.Length;
            newSequences = new Sequence[oldSize - 1];

            int pos = 0;
            for (int i = 0; i < oldSize; i++)
            {
                Sequence testSequence = oldSequences[i];
                if (sequence == testSequence && !found)
                {
                    found = true;
                }
                else
                {
                    newSequences[pos++] = testSequence;
                }
            }

            if (!found)
            {
                break;
            }
        }
        while (!sequencesRef.compareAndSet(oldSequences, newSequences));

        return found;
    }

    public int size()
    {
        return sequencesRef.get().Length;
    }
}
}