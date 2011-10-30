namespace NDisruptor
{
    public static class Extensions
    {
        public static int BitCount(this int n)
        {
            int test = n;
            int count = 0;

            while (test != 0)
            {
                if ((test & 1) == 1)
                {
                    count++;
                }
                test >>= 1;
            }
            return count;
        }
    }
}