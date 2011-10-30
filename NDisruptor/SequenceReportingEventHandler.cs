namespace NDisruptor
{
    public interface SequenceReportingEventHandler<T>
        : EventHandler<T>
    {
        void setSequenceCallback(Sequence sequenceCallback);
    }
}