using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Collections;
using Xunit;

namespace TIKSN.Tests.Collections;

public class DyadTests
{
    [Fact]
    public void Accessors_ShouldReturnCorrectItems()
    {
        var dyad = new Dyad<int>(10, 20);

        Assert.Equal(10, dyad.First);
        Assert.Equal(20, dyad.Second);
    }

    [Fact]
    public void Comparisons_AreDistinctAndAreEqual_ShouldEvaluateCorrectly()
    {
        var distinct = new Dyad<string>("A", "B");
        var identical = new Dyad<string>("A", "A");

        Assert.True(distinct.AreDistinct());
        Assert.False(distinct.AreEqual());

        Assert.False(identical.AreDistinct());
        Assert.True(identical.AreEqual());

        var caseDiff = new Dyad<string>("a", "A");
        Assert.True(caseDiff.AreDistinct(StringComparer.Ordinal));
        Assert.False(caseDiff.AreDistinct(StringComparer.OrdinalIgnoreCase));
        Assert.False(caseDiff.AreEqual(StringComparer.Ordinal));
        Assert.True(caseDiff.AreEqual(StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void Deconstruct_ShouldExtractItems()
    {
        var dyad = new Dyad<int>(1, 2);

        var (first, second) = dyad;

        Assert.Equal(1, first);
        Assert.Equal(2, second);
    }

    [Fact]
    public void Fold_NullFolder_ShouldThrow()
    {
        var dyad = new Dyad<int>(1, 2);

        _ = Assert.Throws<ArgumentNullException>(() => dyad.Fold<int>(null!));
        _ = Assert.Throws<ArgumentNullException>(() => dyad.Fold(0, null!));
    }

    [Fact]
    public void Fold_ShouldReduceValues()
    {
        var dyad = new Dyad<int>(10, 20);

        var sum = dyad.Fold((a, b) => a + b);
        var acc = dyad.Fold("Start:", (current, item) => $"{current}{item}-");

        Assert.Equal(30, sum);
        Assert.Equal("Start:10-20-", acc);
    }

    [Fact]
    public async Task ForEachAsync_NullAction_ShouldThrow()
    {
        var dyad = new Dyad<int>(1, 2);

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            dyad.ForEachAsync(null!));

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            dyad.ForEachAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ForEachAsync_ShouldExecuteConcurrently()
    {
        var dyad = new Dyad<int>(1, 2);
        var list1 = new List<int>();
        var list2 = new List<int>();

        await dyad.ForEachAsync(async item =>
        {
            await Task.Yield();
            lock (list1)
            {
                list1.Add(item);
            }
        });

        await dyad.ForEachAsync(async (item, _) =>
        {
            await Task.Yield();
            lock (list2)
            {
                list2.Add(item * 10);
            }
        }, TestContext.Current.CancellationToken);

        Assert.Equal(2, list1.Count);
        Assert.Contains(1, list1);
        Assert.Contains(2, list1);

        Assert.Equal(2, list2.Count);
        Assert.Contains(10, list2);
        Assert.Contains(20, list2);
    }

    [Fact]
    public void ForEach_NullAction_ShouldThrow()
    {
        var dyad = new Dyad<int>(1, 2);
        _ = Assert.Throws<ArgumentNullException>(() => dyad.ForEach(null!));
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnAll()
    {
        var dyad = new Dyad<int>(1, 2);
        var list = new List<int>();

        dyad.ForEach(list.Add);

        Assert.Equal([1, 2], list);
    }

    [Fact]
    public async Task MapAsync_NullSelector_ShouldThrow()
    {
        var dyad = new Dyad<int>(1, 2);

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            dyad.MapAsync<string>(null!));

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            dyad.MapAsync((Func<int, CancellationToken, Task<string>>)null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task MapAsync_ShouldTransformConcurrently()
    {
        var dyad = new Dyad<int>(1, 2);

        var result1 = await dyad.MapAsync(async x =>
        {
            await Task.Yield();
            return x.ToString(CultureInfo.InvariantCulture);
        });

        var result2 = await dyad.MapAsync(async (x, _) =>
        {
            await Task.Yield();
            return x * 100;
        }, TestContext.Current.CancellationToken);

        Assert.Equal("1", result1.First);
        Assert.Equal("2", result1.Second);
        Assert.Equal(100, result2.First);
        Assert.Equal(200, result2.Second);
    }

    [Fact]
    public void Map_NullSelector_ShouldThrow()
    {
        var dyad = new Dyad<int>(1, 2);
        _ = Assert.Throws<ArgumentNullException>(() => dyad.Map<string>(null!));
    }

    [Fact]
    public void Map_ShouldTransformAllItems()
    {
        var dyad = new Dyad<int>(1, 2);

        var result = dyad.Map(x => x * 10);

        Assert.Equal(10, result.First);
        Assert.Equal(20, result.Second);
    }

    [Fact]
    public void Predicates_AllAndAny_ShouldEvaluateCorrectly()
    {
        var dyad = new Dyad<int>(2, 4);

        Assert.True(dyad.All(x => x % 2 == 0));
        Assert.False(dyad.All(x => x > 2));
        Assert.True(dyad.Any(x => x > 2));
        Assert.False(dyad.Any(x => x < 0));
    }

    [Fact]
    public void Predicates_NullArguments_ShouldThrow()
    {
        var dyad = new Dyad<int>(1, 2);

        _ = Assert.Throws<ArgumentNullException>(() => dyad.All(null!));
        _ = Assert.Throws<ArgumentNullException>(() => dyad.Any(null!));
    }

    [Fact]
    public void SwapAndReverse_ShouldSwapItems()
    {
        var dyad = new Dyad<string>("A", "B");

        var swapped = dyad.Swap();
        var reversed = dyad.Reverse();

        Assert.Equal("B", swapped.First);
        Assert.Equal("A", swapped.Second);
        Assert.Equal("B", reversed.First);
        Assert.Equal("A", reversed.Second);
    }

    [Fact]
    public void ValueTupleConversion_NullDyad_ShouldThrow()
    {
        Dyad<int>? nullDyad = null;
        _ = Assert.Throws<ArgumentNullException>(() =>
        {
            var tuple = ((int, int))nullDyad!;
            Assert.Equal(0, tuple.Item1);
        });
    }

    [Fact]
    public void ValueTupleConversion_ShouldWorkBothWays()
    {
        var dyad = new Dyad<int>(1, 2);
        (int first, int second) tuple = dyad;

        Assert.Equal((1, 2), tuple);

        Dyad<int> fromTuple = (3, 4);
        Assert.Equal(3, fromTuple.First);
        Assert.Equal(4, fromTuple.Second);
    }

    [Fact]
    public void Zip_NullArguments_ShouldThrow()
    {
        var dyad = new Dyad<int>(1, 2);

        _ = Assert.Throws<ArgumentNullException>(() => dyad.Zip<int, int>(null!, (a, b) => a + b));
        _ = Assert.Throws<ArgumentNullException>(() => dyad.Zip<int, int>(dyad, null!));
    }

    [Fact]
    public void Zip_ShouldCombineTwoDyads()
    {
        var dyad1 = new Dyad<string>("A", "B");
        var dyad2 = new Dyad<int>(1, 2);

        var zipped = dyad1.Zip(dyad2, (s, i) => $"{s}{i}");

        Assert.Equal("A1", zipped.First);
        Assert.Equal("B2", zipped.Second);
    }
}
