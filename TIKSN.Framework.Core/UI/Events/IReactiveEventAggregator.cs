namespace TIKSN.UI.Events;

public interface IReactiveEventAggregator
{
    public IObservable<TEvent> GetEvent<TEvent>();

    public void Publish<TEvent>(TEvent message);
}
