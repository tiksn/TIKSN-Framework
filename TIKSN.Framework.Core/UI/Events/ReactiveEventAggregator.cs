using ReactiveUI;

namespace TIKSN.UI.Events;

public class ReactiveEventAggregator : IReactiveEventAggregator
{
    private readonly IMessageBus _messageBus;

    public ReactiveEventAggregator(IMessageBus messageBus) =>
        this._messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

    public IObservable<TEvent> GetEvent<TEvent>() => this._messageBus.Listen<TEvent>();

    public void Publish<TEvent>(TEvent message) => this._messageBus.SendMessage(message);
}
