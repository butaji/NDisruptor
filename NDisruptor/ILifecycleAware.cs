namespace NDisruptor
{
    public interface ILifecycleAware
    {
        void onStart();

        void onShutdown(); 
    }
}