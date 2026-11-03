using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TIKSN.Collections;
using Xunit;

namespace TIKSN.Tests.Collections;

public class OctadTests
{
    [Fact]
    public void Accessors_ShouldReturnCorrectItems()
    {
        var octad = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 8);

        Assert.Equal(1, octad.First);
        Assert.Equal(2, octad.Second);
        Assert.Equal(3, octad.Third);
        Assert.Equal(4, octad.Fourth);
        Assert.Equal(5, octad.Fifth);
        Assert.Equal(6, octad.Sixth);
        Assert.Equal(7, octad.Seventh);
        Assert.Equal(8, octad.Eighth);
    }

    [Fact]
    public async Task AsyncOperations_ShouldExecuteProperly()
    {
        var octad = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 8);

        var mapped1 = await octad.MapAsync(async x =>
        {
            await Task.Yield();
            return x.ToString(CultureInfo.InvariantCulture);
        });

        var mapped2 = await octad.MapAsync(async (x, _) =>
        {
            await Task.Yield();
            return x + 100;
        }, TestContext.Current.CancellationToken);

        Assert.Equal(("1", "2", "3", "4", "5", "6", "7", "8"), mapped1.ToValueTuple());
        Assert.Equal((101, 102, 103, 104, 105, 106, 107, 108), mapped2.ToValueTuple());

        var list = new List<int>();
        await octad.ForEachAsync(async x =>
        {
            await Task.Yield();
            lock (list)
            {
                list.Add(x);
            }
        });

        Assert.Equal(8, list.Count);
    }

    [Fact]
    public void Deconstruct_ShouldExtractItems()
    {
        var octad = new Octad<string>("A", "B", "C", "D", "E", "F", "G", "H");

        var (first, second, third, fourth, fifth, sixth, seventh, eighth) = octad;

        Assert.Equal("A", first);
        Assert.Equal("B", second);
        Assert.Equal("C", third);
        Assert.Equal("D", fourth);
        Assert.Equal("E", fifth);
        Assert.Equal("F", sixth);
        Assert.Equal("G", seventh);
        Assert.Equal("H", eighth);
    }

    [Fact]
    public void Fold_ShouldReduceValues()
    {
        var octad = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 8);

        var folded = octad.Fold((a, b, c, d, e, f, g, h) => a + b + c + d + e + f + g + h);
        Assert.Equal(36, folded);

        var acc = octad.Fold(0, (curr, next) => curr + next);
        Assert.Equal(36, acc);
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnAll()
    {
        var octad = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 8);
        var list = new List<int>();

        octad.ForEach(list.Add);

        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8], list);
    }

    [Fact]
    public void MapAndZip_ShouldPreserveOctadShape()
    {
        var octad = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 8);
        var mapped = octad.Map(x => x * 10);

        Assert.Equal((10, 20, 30, 40, 50, 60, 70, 80), mapped.ToValueTuple());

        var other = new Octad<int>(8, 7, 6, 5, 4, 3, 2, 1);
        var zipped = octad.Zip(other, (a, b) => a + b);

        Assert.Equal((9, 9, 9, 9, 9, 9, 9, 9), zipped.ToValueTuple());
    }

    [Fact]
    public void OctadRest_AccessorsAndDeconstruction_ShouldWorkCorrectly()
    {
        var octad = new Octad<int, Tuple<string>>(1, 2, 3, 4, 5, 6, 7, Tuple.Create("rest"));

        Assert.Equal(1, octad.First);
        Assert.Equal(2, octad.Second);
        Assert.Equal(3, octad.Third);
        Assert.Equal(4, octad.Fourth);
        Assert.Equal(5, octad.Fifth);
        Assert.Equal(6, octad.Sixth);
        Assert.Equal(7, octad.Seventh);
        Assert.Equal(Tuple.Create("rest"), octad.Rest);

        var (item1, item2, item3, item4, item5, item6, item7, rest) = octad;
        Assert.Equal(1, item1);
        Assert.Equal(Tuple.Create("rest"), rest);

        (int, int, int, int, int, int, int, Tuple<string>) tuple = octad;
        Assert.Equal((1, 2, 3, 4, 5, 6, 7, Tuple.Create("rest")), tuple);

        Octad<int, Tuple<string>> fromTuple = (10, 20, 30, 40, 50, 60, 70, Tuple.Create("rest2"));
        Assert.Equal(10, fromTuple.First);
        Assert.Equal(Tuple.Create("rest2"), fromTuple.Rest);

        Octad<int, Tuple<string>>? nullOctad = null;
        _ = Assert.Throws<ArgumentNullException>(() =>
        {
            var tuple = ((int, int, int, int, int, int, int, Tuple<string>))nullOctad!;
            Assert.Equal(0, tuple.Item1);
        });
    }

    [Fact]
    public void PredicatesAndComparisons_ShouldEvaluateCorrectly()
    {
        var distinct = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 8);
        var duplicate = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 1);
        var equal = new Octad<int>(4, 4, 4, 4, 4, 4, 4, 4);

        Assert.True(distinct.AreDistinct());
        Assert.False(distinct.AreEqual());

        Assert.False(duplicate.AreDistinct());
        Assert.False(duplicate.AreEqual());

        Assert.False(equal.AreDistinct());
        Assert.True(equal.AreEqual());

        Assert.True(distinct.All(x => x > 0));
        Assert.False(distinct.All(x => x > 1));
        Assert.True(distinct.Any(x => x == 8));
        Assert.False(distinct.Any(x => x == 99));
    }

    [Fact]
    public void Symmetry_ReverseAndRotate_ShouldWorkAsExpected()
    {
        var octad = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 8);

        var reversed = octad.Reverse();
        Assert.Equal((8, 7, 6, 5, 4, 3, 2, 1), reversed.ToValueTuple());

        var left = octad.RotateLeft();
        Assert.Equal((2, 3, 4, 5, 6, 7, 8, 1), left.ToValueTuple());

        var right = octad.RotateRight();
        Assert.Equal((8, 1, 2, 3, 4, 5, 6, 7), right.ToValueTuple());
    }

    [Fact]
    public void ValueTupleConversion_NullOctad_ShouldThrow()
    {
        Octad<int>? nullOctad = null;
        _ = Assert.Throws<ArgumentNullException>(() =>
        {
            var tuple = ((int, int, int, int, int, int, int, int))nullOctad!;
            Assert.Equal(0, tuple.Item1);
        });
    }

    [Fact]
    public void ValueTupleConversion_ShouldWorkBothWays()
    {
        var octad = new Octad<int>(1, 2, 3, 4, 5, 6, 7, 8);
        (int, int, int, int, int, int, int, int) tuple = octad;

        Assert.Equal((1, 2, 3, 4, 5, 6, 7, 8), tuple);

        Octad<int> fromTuple = (10, 20, 30, 40, 50, 60, 70, 80);
        Assert.Equal(10, fromTuple.First);
        Assert.Equal(80, fromTuple.Eighth);
    }
}
