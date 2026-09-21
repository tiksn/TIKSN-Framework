using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace TIKSN.UI.Navigation;

public class NavigationService : INavigationService
{
    public NavigationService() => this.Router = new RoutingState();

    public RoutingState Router { get; }

    public IObservable<Unit> NavigateBack() => this.Router.NavigateBack.Execute().Select(_ => Unit.Default);

    public IObservable<Unit> NavigateTo<TViewModel>(TViewModel viewModel)
        where TViewModel : IRoutableViewModel
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        return this.Router.Navigate.Execute(viewModel).Select(_ => Unit.Default);
    }
}
