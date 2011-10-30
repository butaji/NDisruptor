namespace NDisruptor
{
    public interface LifecycleAware
    {
        void onStart();

        void onShutdown(); 
    }
}