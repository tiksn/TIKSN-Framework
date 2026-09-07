using System;
using System.Threading.Tasks;
using LanguageExt;
using NSubstitute;
using ReactiveUI;
using Shouldly;
using TIKSN.Concurrency;
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

    private readonly IScreen _hostScreen;
    private readonly IMessageBus _messageBus;

    private readonly ISequencers _sequencers;

    public ViewModelBaseTests()
    {
        _sequencers = Substitute.For<ISequencers>();
        _messageBus = Substitute.For<IMessageBus>();
        _hostScreen = Substitute.For<IScreen>();
    }

    [Fact]
    public void Constructor_EmptyUrlPathSegments_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var emptySegments = Seq<string>.Empty;

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() =>
            new TestViewModel(_sequencers, _messageBus, _hostScreen, emptySegments));
    }

    [Fact]
    public void Constructor_ValidUrlPathSegments_SetsUrlPathSegment()
    {
        // Arrange
        var segments = Seq.create("home", "dashboard");

        // Act
        var viewModel = new TestViewModel(_sequencers, _messageBus, _hostScreen, segments);

        // Assert
        viewModel.UrlPathSegment.ShouldBe("home/dashboard");
    }

    [Fact]
    public async Task RunWithBusyAsync_TracksBusyState()
    {
        // Arrange
        var segments = Seq.create("home");
        var viewModel = new TestViewModel(_sequencers, _messageBus, _hostScreen, segments);
        var isBusyChanges = new System.Collections.Generic.List<bool>();

        viewModel.WhenAnyValue(x => x.IsBusy)
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
            IMessageBus messageBus,
            IScreen hostScreen,
            Seq<string> urlPathSegments)
            : base(sequencers, messageBus, hostScreen, urlPathSegments)
        {
        }
    }
}
