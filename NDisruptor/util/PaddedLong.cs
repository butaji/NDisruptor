namespace NDisruptor
{
   public class PaddedLong : MutableLong
{
    public long p1, p2, p3, p4, p5, p6 = 7L;

    public PaddedLong(long initialValue) 
        : base(initialValue)
    {
    }

    public long sumPaddingToPreventOptimisation()
    {
        return p1 + p2 + p3 + p4 + p5 + p6;
    }
}
}