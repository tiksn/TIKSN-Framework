using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using TIKSN.Concurrency;
using Xunit;

namespace TIKSN.Tests.Concurrency;

public class AsyncConcurrencyTests
{
    [Fact]
    public async Task AsyncReaderWriterLock_AllowsConcurrentReadersAndExcludesWriter()
    {
        await using var asyncLock = new AsyncReaderWriterLock();
        var firstReader = await asyncLock.AcquireReadAsync(TestContext.Current.CancellationToken);
        var secondReader = await asyncLock.AcquireReadAsync(TestContext.Current.CancellationToken);

        var writerStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var writerTask = AcquireWriteAsync();
        await writerStarted.Task;
        await Task.Yield();

        writerTask.IsCompleted.ShouldBeFalse();

        await firstReader.DisposeAsync();
        await secondReader.DisposeAsync();
        await writerTask;

        async Task AcquireWriteAsync()
        {
            writerStarted.SetResult();
            await using var lease = await asyncLock.AcquireWriteAsync(TestContext.Current.CancellationToken);
        }
    }

    [Fact]
    [SuppressMessage("xUnit.Analyzers", "xUnit1051",
        Justification = "A linked cancellation token is required to verify a pending acquisition can be canceled.")]
    public async Task AsyncReaderWriterLock_CanceledWriterDoesNotBlockNextWriter()
    {
        await using var asyncLock = new AsyncReaderWriterLock();
        var reader = await asyncLock.AcquireReadAsync(TestContext.Current.CancellationToken);

        using var cancellationSource =
            CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var pendingWriter = asyncLock.AcquireWriteAsync(cancellationSource.Token).AsTask();
        cancellationSource.Cancel();

        _ = await Should.ThrowAsync<OperationCanceledException>(() => pendingWriter);
        await reader.DisposeAsync();
        await using var nextWriter = await asyncLock.AcquireWriteAsync(TestContext.Current.CancellationToken);
    }
}
