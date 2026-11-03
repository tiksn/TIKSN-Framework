using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TIKSN.Collections;
using Xunit;

namespace TIKSN.Tests.Collections;

public class PentadTests
{
    [Fact]
    public void Accessors_ShouldReturnCorrectItems()
    {
        var pentad = new Pentad<int>(1, 2, 3, 4, 5);

        Assert.Equal(1, pentad.First);
        Assert.Equal(2, pentad.Second);
        Assert.Equal(3, pentad.Third);
        Assert.Equal(4, pentad.Fourth);
        Assert.Equal(5, pentad.Fifth);
    }

    [Fact]
    public async Task AsyncOperations_ShouldExecuteProperly()
    {
        var pentad = new Pentad<int>(1, 2, 3, 4, 5);

        var mapped1 = await pentad.MapAsync(async x =>
        {
            await Task.Yield();
            return x.ToString(CultureInfo.InvariantCulture);
        });

        var mapped2 = await pentad.MapAsync(async (x, _) =>
        {
            await Task.Yield();
            return x + 100;
        }, TestContext.Current.CancellationToken);

        Assert.Equal(("1", "2", "3", "4", "5"), mapped1.ToValueTuple());
        Assert.Equal((101, 102, 103, 104, 105), mapped2.ToValueTuple());

        var list = new List<int>();
        await pentad.ForEachAsync(async x =>
        {
            await Task.Yield();
            lock (list)
            {
                list.Add(x);
            }
        });

        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Deconstruct_ShouldExtractItems()
    {
        var pentad = new Pentad<string>("A", "B", "C", "D", "E");

        var (first, second, third, fourth, fifth) = pentad;

        Assert.Equal("A", first);
        Assert.Equal("B", second);
        Assert.Equal("C", third);
        Assert.Equal("D", fourth);
        Assert.Equal("E", fifth);
    }

    [Fact]
    public void Fold_ShouldReduceValues()
    {
        var pentad = new Pentad<int>(1, 2, 3, 4, 5);

        var folded = pentad.Fold((a, b, c, d, e) => a + b + c + d + e);
        Assert.Equal(15, folded);

        var acc = pentad.Fold(0, (curr, next) => curr + next);
        Assert.Equal(15, acc);
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnAll()
    {
        var pentad = new Pentad<int>(1, 2, 3, 4, 5);
        var list = new List<int>();

        pentad.ForEach(list.Add);

        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void MapAndZip_ShouldPreservePentadShape()
    {
        var pentad = new Pentad<int>(1, 2, 3, 4, 5);
        var mapped = pentad.Map(x => x * 10);

        Assert.Equal((10, 20, 30, 40, 50), mapped.ToValueTuple());

        var other = new Pentad<int>(5, 4, 3, 2, 1);
        var zipped = pentad.Zip(other, (a, b) => a + b);

        Assert.Equal((6, 6, 6, 6, 6), zipped.ToValueTuple());
    }

    [Fact]
    public void PredicatesAndComparisons_ShouldEvaluateCorrectly()
    {
        var distinct = new Pentad<int>(1, 2, 3, 4, 5);
        var duplicate = new Pentad<int>(1, 2, 3, 4, 1);
        var equal = new Pentad<int>(2, 2, 2, 2, 2);

        Assert.True(distinct.AreDistinct());
        Assert.False(distinct.AreEqual());

        Assert.False(duplicate.AreDistinct());
        Assert.False(duplicate.AreEqual());

        Assert.False(equal.AreDistinct());
        Assert.True(equal.AreEqual());

        Assert.True(distinct.All(x => x > 0));
        Assert.False(distinct.All(x => x > 1));
        Assert.True(distinct.Any(x => x == 5));
        Assert.False(distinct.Any(x => x == 99));
    }

    [Fact]
    public void Symmetry_ReverseAndRotate_ShouldWorkAsExpected()
    {
        var pentad = new Pentad<int>(1, 2, 3, 4, 5);

        var reversed = pentad.Reverse();
        Assert.Equal((5, 4, 3, 2, 1), reversed.ToValueTuple());

        var left = pentad.RotateLeft();
        Assert.Equal((2, 3, 4, 5, 1), left.ToValueTuple());

        var right = pentad.RotateRight();
        Assert.Equal((5, 1, 2, 3, 4), right.ToValueTuple());
    }

    [Fact]
    public void ValueTupleConversion_NullPentad_ShouldThrow()
    {
        Pentad<int>? nullPentad = null;
        _ = Assert.Throws<ArgumentNullException>(() =>
        {
            var tuple = ((int, int, int, int, int))nullPentad!;
            Assert.Equal(0, tuple.Item1);
        });
    }

    [Fact]
    public void ValueTupleConversion_ShouldWorkBothWays()
    {
        var pentad = new Pentad<int>(1, 2, 3, 4, 5);
        (int, int, int, int, int) tuple = pentad;

        Assert.Equal((1, 2, 3, 4, 5), tuple);

        Pentad<int> fromTuple = (6, 7, 8, 9, 10);
        Assert.Equal(6, fromTuple.First);
        Assert.Equal(10, fromTuple.Fifth);
    }
}
