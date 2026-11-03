using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TIKSN.Collections;
using Xunit;

namespace TIKSN.Tests.Collections;

public class TetradTests
{
    [Fact]
    public void Accessors_ShouldReturnCorrectItems()
    {
        var tetrad = new Tetrad<int>(1, 2, 3, 4);

        Assert.Equal(1, tetrad.First);
        Assert.Equal(2, tetrad.Second);
        Assert.Equal(3, tetrad.Third);
        Assert.Equal(4, tetrad.Fourth);
    }

    [Fact]
    public async Task AsyncOperations_ShouldExecuteProperly()
    {
        var tetrad = new Tetrad<int>(1, 2, 3, 4);

        var mapped1 = await tetrad.MapAsync(async x =>
        {
            await Task.Yield();
            return x.ToString(CultureInfo.InvariantCulture);
        });

        var mapped2 = await tetrad.MapAsync(async (x, _) =>
        {
            await Task.Yield();
            return x + 100;
        }, TestContext.Current.CancellationToken);

        Assert.Equal(("1", "2", "3", "4"), mapped1.ToValueTuple());
        Assert.Equal((101, 102, 103, 104), mapped2.ToValueTuple());

        var list = new List<int>();
        await tetrad.ForEachAsync(async x =>
        {
            await Task.Yield();
            lock (list)
            {
                list.Add(x);
            }
        });

        Assert.Equal(4, list.Count);
        Assert.Contains(1, list);
        Assert.Contains(4, list);
    }

    [Fact]
    public void Deconstruct_ShouldExtractItems()
    {
        var tetrad = new Tetrad<string>("A", "B", "C", "D");

        var (first, second, third, fourth) = tetrad;

        Assert.Equal("A", first);
        Assert.Equal("B", second);
        Assert.Equal("C", third);
        Assert.Equal("D", fourth);
    }

    [Fact]
    public void Fold_ShouldReduceValues()
    {
        var tetrad = new Tetrad<int>(1, 2, 3, 4);

        var folded = tetrad.Fold((a, b, c, d) => a + b + c + d);
        Assert.Equal(10, folded);

        var acc = tetrad.Fold("Init", (curr, next) => $"{curr}:{next}");
        Assert.Equal("Init:1:2:3:4", acc);
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnAll()
    {
        var tetrad = new Tetrad<int>(1, 2, 3, 4);
        var list = new List<int>();

        tetrad.ForEach(list.Add);

        Assert.Equal([1, 2, 3, 4], list);
    }

    [Fact]
    public void MapAndZip_NullArguments_ShouldThrow()
    {
        var tetrad = new Tetrad<int>(1, 2, 3, 4);

        _ = Assert.Throws<ArgumentNullException>(() => tetrad.Map<string>(null!));
        _ = Assert.Throws<ArgumentNullException>(() => tetrad.Zip<int, int>(null!, (a, b) => a + b));
        _ = Assert.Throws<ArgumentNullException>(() => tetrad.Zip<int, int>(tetrad, null!));
    }

    [Fact]
    public void MapAndZip_ShouldPreserveTetradShape()
    {
        var tetrad = new Tetrad<int>(1, 2, 3, 4);
        var mapped = tetrad.Map(x => x * 10);

        Assert.Equal((10, 20, 30, 40), mapped.ToValueTuple());

        var labels = new Tetrad<string>("w", "x", "y", "z");
        var zipped = tetrad.Zip(labels, (num, str) => $"{str}{num}");

        Assert.Equal(("w1", "x2", "y3", "z4"), zipped.ToValueTuple());
    }

    [Fact]
    public void PredicatesAndComparisons_ShouldEvaluateCorrectly()
    {
        var distinct = new Tetrad<int>(1, 2, 3, 4);
        var duplicate = new Tetrad<int>(1, 2, 3, 1);
        var equal = new Tetrad<int>(7, 7, 7, 7);

        Assert.True(distinct.AreDistinct());
        Assert.False(distinct.AreEqual());

        Assert.False(duplicate.AreDistinct());
        Assert.False(duplicate.AreEqual());

        Assert.False(equal.AreDistinct());
        Assert.True(equal.AreEqual());

        Assert.True(distinct.All(x => x > 0));
        Assert.False(distinct.All(x => x > 1));
        Assert.True(distinct.Any(x => x == 4));
        Assert.False(distinct.Any(x => x == 99));
    }

    [Fact]
    public void Symmetry_ReverseAndRotate_ShouldWorkAsExpected()
    {
        var tetrad = new Tetrad<int>(1, 2, 3, 4);

        var reversed = tetrad.Reverse();
        Assert.Equal((4, 3, 2, 1), reversed.ToValueTuple());

        var left = tetrad.RotateLeft();
        Assert.Equal((2, 3, 4, 1), left.ToValueTuple());

        var right = tetrad.RotateRight();
        Assert.Equal((4, 1, 2, 3), right.ToValueTuple());
    }

    [Fact]
    public void ValueTupleConversion_NullTetrad_ShouldThrow()
    {
        Tetrad<int>? nullTetrad = null;
        _ = Assert.Throws<ArgumentNullException>(() =>
        {
            var tuple = ((int, int, int, int))nullTetrad!;
            Assert.Equal(0, tuple.Item1);
        });
    }

    [Fact]
    public void ValueTupleConversion_ShouldWorkBothWays()
    {
        var tetrad = new Tetrad<int>(1, 2, 3, 4);
        (int, int, int, int) tuple = tetrad;

        Assert.Equal((1, 2, 3, 4), tuple);

        Tetrad<int> fromTuple = (5, 6, 7, 8);
        Assert.Equal(5, fromTuple.First);
        Assert.Equal(6, fromTuple.Second);
        Assert.Equal(7, fromTuple.Third);
        Assert.Equal(8, fromTuple.Fourth);
    }
}
