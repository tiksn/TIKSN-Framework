using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TIKSN.Collections;
using Xunit;

namespace TIKSN.Tests.Collections;

public class HeptadTests
{
    [Fact]
    public void Accessors_ShouldReturnCorrectItems()
    {
        var heptad = new Heptad<int>(1, 2, 3, 4, 5, 6, 7);

        Assert.Equal(1, heptad.First);
        Assert.Equal(2, heptad.Second);
        Assert.Equal(3, heptad.Third);
        Assert.Equal(4, heptad.Fourth);
        Assert.Equal(5, heptad.Fifth);
        Assert.Equal(6, heptad.Sixth);
        Assert.Equal(7, heptad.Seventh);
    }

    [Fact]
    public async Task AsyncOperations_ShouldExecuteProperly()
    {
        var heptad = new Heptad<int>(1, 2, 3, 4, 5, 6, 7);

        var mapped1 = await heptad.MapAsync(async x =>
        {
            await Task.Yield();
            return x.ToString(CultureInfo.InvariantCulture);
        });

        var mapped2 = await heptad.MapAsync(async (x, _) =>
        {
            await Task.Yield();
            return x + 100;
        }, TestContext.Current.CancellationToken);

        Assert.Equal(("1", "2", "3", "4", "5", "6", "7"), mapped1.ToValueTuple());
        Assert.Equal((101, 102, 103, 104, 105, 106, 107), mapped2.ToValueTuple());

        var list = new List<int>();
        await heptad.ForEachAsync(async x =>
        {
            await Task.Yield();
            lock (list)
            {
                list.Add(x);
            }
        });

        Assert.Equal(7, list.Count);
    }

    [Fact]
    public void Deconstruct_ShouldExtractItems()
    {
        var heptad = new Heptad<string>("A", "B", "C", "D", "E", "F", "G");

        var (first, second, third, fourth, fifth, sixth, seventh) = heptad;

        Assert.Equal("A", first);
        Assert.Equal("B", second);
        Assert.Equal("C", third);
        Assert.Equal("D", fourth);
        Assert.Equal("E", fifth);
        Assert.Equal("F", sixth);
        Assert.Equal("G", seventh);
    }

    [Fact]
    public void Fold_ShouldReduceValues()
    {
        var heptad = new Heptad<int>(1, 2, 3, 4, 5, 6, 7);

        var folded = heptad.Fold((a, b, c, d, e, f, g) => a + b + c + d + e + f + g);
        Assert.Equal(28, folded);

        var acc = heptad.Fold(0, (curr, next) => curr + next);
        Assert.Equal(28, acc);
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnAll()
    {
        var heptad = new Heptad<int>(1, 2, 3, 4, 5, 6, 7);
        var list = new List<int>();

        heptad.ForEach(list.Add);

        Assert.Equal([1, 2, 3, 4, 5, 6, 7], list);
    }

    [Fact]
    public void MapAndZip_ShouldPreserveHeptadShape()
    {
        var heptad = new Heptad<int>(1, 2, 3, 4, 5, 6, 7);
        var mapped = heptad.Map(x => x * 10);

        Assert.Equal((10, 20, 30, 40, 50, 60, 70), mapped.ToValueTuple());

        var other = new Heptad<int>(7, 6, 5, 4, 3, 2, 1);
        var zipped = heptad.Zip(other, (a, b) => a + b);

        Assert.Equal((8, 8, 8, 8, 8, 8, 8), zipped.ToValueTuple());
    }

    [Fact]
    public void PredicatesAndComparisons_ShouldEvaluateCorrectly()
    {
        var distinct = new Heptad<int>(1, 2, 3, 4, 5, 6, 7);
        var duplicate = new Heptad<int>(1, 2, 3, 4, 5, 6, 1);
        var equal = new Heptad<int>(9, 9, 9, 9, 9, 9, 9);

        Assert.True(distinct.AreDistinct());
        Assert.False(distinct.AreEqual());

        Assert.False(duplicate.AreDistinct());
        Assert.False(duplicate.AreEqual());

        Assert.False(equal.AreDistinct());
        Assert.True(equal.AreEqual());

        Assert.True(distinct.All(x => x > 0));
        Assert.False(distinct.All(x => x > 1));
        Assert.True(distinct.Any(x => x == 7));
        Assert.False(distinct.Any(x => x == 99));
    }

    [Fact]
    public void Symmetry_ReverseAndRotate_ShouldWorkAsExpected()
    {
        var heptad = new Heptad<int>(1, 2, 3, 4, 5, 6, 7);

        var reversed = heptad.Reverse();
        Assert.Equal((7, 6, 5, 4, 3, 2, 1), reversed.ToValueTuple());

        var left = heptad.RotateLeft();
        Assert.Equal((2, 3, 4, 5, 6, 7, 1), left.ToValueTuple());

        var right = heptad.RotateRight();
        Assert.Equal((7, 1, 2, 3, 4, 5, 6), right.ToValueTuple());
    }

    [Fact]
    public void ValueTupleConversion_NullHeptad_ShouldThrow()
    {
        Heptad<int>? nullHeptad = null;
        _ = Assert.Throws<ArgumentNullException>(() =>
        {
            var tuple = ((int, int, int, int, int, int, int))nullHeptad!;
            Assert.Equal(0, tuple.Item1);
        });
    }

    [Fact]
    public void ValueTupleConversion_ShouldWorkBothWays()
    {
        var heptad = new Heptad<int>(1, 2, 3, 4, 5, 6, 7);
        (int, int, int, int, int, int, int) tuple = heptad;

        Assert.Equal((1, 2, 3, 4, 5, 6, 7), tuple);

        Heptad<int> fromTuple = (10, 20, 30, 40, 50, 60, 70);
        Assert.Equal(10, fromTuple.First);
        Assert.Equal(70, fromTuple.Seventh);
    }
}
