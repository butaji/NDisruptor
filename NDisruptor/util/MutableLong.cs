namespace NDisruptor.util
{
  public class MutableLong
{
    private long value = 0L;

    public MutableLong()
    {
    }

    public MutableLong(long initialValue)
    {
        this.value = initialValue;
    }

    public long get()
    {
        return value;
    }

    public void set(long value)
    {
        this.value = value;
    }
}

}