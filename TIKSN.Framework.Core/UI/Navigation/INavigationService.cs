using System.Reactive;
using ReactiveUI;

namespace TIKSN.UI.Navigation;

public interface INavigationService
{
    public RoutingState Router { get; }

    public IObservable<Unit> NavigateTo<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel;

    public IObservable<Unit> NavigateBack();
}
