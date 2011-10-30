namespace NDisruptor
{
    public interface ISequenceReportingEventHandler<T>
        : IEventHandler<T>
    {
        void setSequenceCallback(Sequence sequenceCallback);
    }
}