public interface IEventPublisher
{
    void Publish<TEvent>(TEvent eventData)
        where TEvent : class;
}
