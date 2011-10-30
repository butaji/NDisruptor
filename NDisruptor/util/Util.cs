namespace NDisruptor
{
   public class Util
{
    public static int ceilingNextPowerOfTwo(int x)
    {
        return 1 << (32 - (x - 1).NumberOfLeadingZeros());
    }

    public static long getMinimumSequence(Sequence[] sequences)
    {
        long minimum = long.MaxValue;

        foreach (Sequence sequence in sequences)
        {
            long value = sequence.get();
            minimum = minimum < value ? minimum : value;
        }

        return minimum;
    }

    public static Sequence[] getSequencesFor(params EventProcessor[] processors)
    {
        Sequence[] sequences = new Sequence[processors.Length];
        for (int i = 0; i < sequences.Length; i++)
        {
            sequences[i] = processors[i].getSequence();
        }

        return sequences;
    }
}

}