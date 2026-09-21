using System;
using System.Threading.Tasks;
using LanguageExt;
using NSubstitute;
using ReactiveUI;
using Shouldly;
using TIKSN.Concurrency;
using TIKSN.UI.Events;
using TIKSN.UI.ViewModels;
using Xunit;

namespace TIKSN.Tests.UI.ViewModels;

public class ViewModelBaseTests
{
    private static readonly bool[] ExpectedBusyStates =
    [
        false,
        true,
        false
    ];

    private readonly IReactiveEventAggregator _eventAggregator;

    private readonly IScreen _hostScreen;

    private readonly ISequencers _sequencers;

    public ViewModelBaseTests()
    {
        this._sequencers = Substitute.For<ISequencers>();
        this._eventAggregator = Substitute.For<IReactiveEventAggregator>();
        this._hostScreen = Substitute.For<IScreen>();
    }

    [Fact]
    public void Constructor_EmptyUrlPathSegments_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var emptySegments = Seq<string>.Empty;

        // Act & Assert
        _ = Should.Throw<ArgumentOutOfRangeException>(() =>
            new TestViewModel(this._sequencers, this._eventAggregator, this._hostScreen, emptySegments));
    }

    [Fact]
    public void Constructor_ValidUrlPathSegments_SetsUrlPathSegment()
    {
        // Arrange
        var segments = Seq.create("home", "dashboard");

        // Act
        var viewModel = new TestViewModel(this._sequencers, this._eventAggregator, this._hostScreen, segments);

        // Assert
        viewModel.UrlPathSegment.ShouldBe("home/dashboard");
    }

    [Fact]
    public async Task RunWithBusyAsync_TracksBusyState()
    {
        // Arrange
        var segments = Seq.create("home");
        var viewModel = new TestViewModel(this._sequencers, this._eventAggregator, this._hostScreen, segments);
        var isBusyChanges = new System.Collections.Generic.List<bool>();

        _ = viewModel.WhenAnyValue(x => x.IsBusy)
            .Subscribe(isBusyChanges.Add);

        // Act
        await viewModel.RunWithBusyAsync(async () =>
        {
            viewModel.IsBusy.ShouldBeTrue();
            await Task.Yield();
        });

        // Assert
        viewModel.IsBusy.ShouldBeFalse();
        isBusyChanges.ShouldBe(ExpectedBusyStates);
    }

    private sealed class TestViewModel : ViewModelBase
    {
        public TestViewModel(
            ISequencers sequencers,
            IReactiveEventAggregator eventAggregator,
            IScreen hostScreen,
            Seq<string> urlPathSegments)
            : base(sequencers, eventAggregator, hostScreen, urlPathSegments)
        {
        }
    }
}
