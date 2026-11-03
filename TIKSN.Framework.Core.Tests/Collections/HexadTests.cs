using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TIKSN.Collections;
using Xunit;

namespace TIKSN.Tests.Collections;

public class HexadTests
{
    [Fact]
    public void Accessors_ShouldReturnCorrectItems()
    {
        var hexad = new Hexad<int>(1, 2, 3, 4, 5, 6);

        Assert.Equal(1, hexad.First);
        Assert.Equal(2, hexad.Second);
        Assert.Equal(3, hexad.Third);
        Assert.Equal(4, hexad.Fourth);
        Assert.Equal(5, hexad.Fifth);
        Assert.Equal(6, hexad.Sixth);
    }

    [Fact]
    public async Task AsyncOperations_ShouldExecuteProperly()
    {
        var hexad = new Hexad<int>(1, 2, 3, 4, 5, 6);

        var mapped1 = await hexad.MapAsync(async x =>
        {
            await Task.Yield();
            return x.ToString(CultureInfo.InvariantCulture);
        });

        var mapped2 = await hexad.MapAsync(async (x, _) =>
        {
            await Task.Yield();
            return x + 100;
        }, TestContext.Current.CancellationToken);

        Assert.Equal(("1", "2", "3", "4", "5", "6"), mapped1.ToValueTuple());
        Assert.Equal((101, 102, 103, 104, 105, 106), mapped2.ToValueTuple());

        var list = new List<int>();
        await hexad.ForEachAsync(async x =>
        {
            await Task.Yield();
            lock (list)
            {
                list.Add(x);
            }
        });

        Assert.Equal(6, list.Count);
    }

    [Fact]
    public void Deconstruct_ShouldExtractItems()
    {
        var hexad = new Hexad<string>("A", "B", "C", "D", "E", "F");

        var (first, second, third, fourth, fifth, sixth) = hexad;

        Assert.Equal("A", first);
        Assert.Equal("B", second);
        Assert.Equal("C", third);
        Assert.Equal("D", fourth);
        Assert.Equal("E", fifth);
        Assert.Equal("F", sixth);
    }

    [Fact]
    public void Fold_ShouldReduceValues()
    {
        var hexad = new Hexad<int>(1, 2, 3, 4, 5, 6);

        var folded = hexad.Fold((a, b, c, d, e, f) => a + b + c + d + e + f);
        Assert.Equal(21, folded);

        var acc = hexad.Fold(0, (curr, next) => curr + next);
        Assert.Equal(21, acc);
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnAll()
    {
        var hexad = new Hexad<int>(1, 2, 3, 4, 5, 6);
        var list = new List<int>();

        hexad.ForEach(list.Add);

        Assert.Equal([1, 2, 3, 4, 5, 6], list);
    }

    [Fact]
    public void MapAndZip_ShouldPreserveHexadShape()
    {
        var hexad = new Hexad<int>(1, 2, 3, 4, 5, 6);
        var mapped = hexad.Map(x => x * 10);

        Assert.Equal((10, 20, 30, 40, 50, 60), mapped.ToValueTuple());

        var other = new Hexad<int>(6, 5, 4, 3, 2, 1);
        var zipped = hexad.Zip(other, (a, b) => a + b);

        Assert.Equal((7, 7, 7, 7, 7, 7), zipped.ToValueTuple());
    }

    [Fact]
    public void PredicatesAndComparisons_ShouldEvaluateCorrectly()
    {
        var distinct = new Hexad<int>(1, 2, 3, 4, 5, 6);
        var duplicate = new Hexad<int>(1, 2, 3, 4, 5, 1);
        var equal = new Hexad<int>(3, 3, 3, 3, 3, 3);

        Assert.True(distinct.AreDistinct());
        Assert.False(distinct.AreEqual());

        Assert.False(duplicate.AreDistinct());
        Assert.False(duplicate.AreEqual());

        Assert.False(equal.AreDistinct());
        Assert.True(equal.AreEqual());

        Assert.True(distinct.All(x => x > 0));
        Assert.False(distinct.All(x => x > 1));
        Assert.True(distinct.Any(x => x == 6));
        Assert.False(distinct.Any(x => x == 99));
    }

    [Fact]
    public void Symmetry_ReverseAndRotate_ShouldWorkAsExpected()
    {
        var hexad = new Hexad<int>(1, 2, 3, 4, 5, 6);

        var reversed = hexad.Reverse();
        Assert.Equal((6, 5, 4, 3, 2, 1), reversed.ToValueTuple());

        var left = hexad.RotateLeft();
        Assert.Equal((2, 3, 4, 5, 6, 1), left.ToValueTuple());

        var right = hexad.RotateRight();
        Assert.Equal((6, 1, 2, 3, 4, 5), right.ToValueTuple());
    }

    [Fact]
    public void ValueTupleConversion_NullHexad_ShouldThrow()
    {
        Hexad<int>? nullHexad = null;
        _ = Assert.Throws<ArgumentNullException>(() =>
        {
            var tuple = ((int, int, int, int, int, int))nullHexad!;
            Assert.Equal(0, tuple.Item1);
        });
    }

    [Fact]
    public void ValueTupleConversion_ShouldWorkBothWays()
    {
        var hexad = new Hexad<int>(1, 2, 3, 4, 5, 6);
        (int, int, int, int, int, int) tuple = hexad;

        Assert.Equal((1, 2, 3, 4, 5, 6), tuple);

        Hexad<int> fromTuple = (10, 20, 30, 40, 50, 60);
        Assert.Equal(10, fromTuple.First);
        Assert.Equal(60, fromTuple.Sixth);
    }
}
