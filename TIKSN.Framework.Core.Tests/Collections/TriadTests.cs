using System.Collections.Generic;
using System.Threading.Tasks;
using TIKSN.Collections;
using Xunit;

namespace TIKSN.Tests.Collections;

public class TriadTests
{
    [Fact]
    public void Accessors_ShouldReturnCorrectItems()
    {
        var triad = new Triad<int>(1, 2, 3);

        Assert.Equal(1, triad.First);
        Assert.Equal(2, triad.Second);
        Assert.Equal(3, triad.Third);
    }

    [Fact]
    public async Task AsyncOperations_ShouldExecuteProperly()
    {
        var triad = new Triad<int>(1, 2, 3);

        var mapped = await triad.MapAsync(async x =>
        {
            await Task.Yield();
            return x + 10;
        });

        Assert.Equal((11, 12, 13), mapped.ToValueTuple());

        var list = new List<int>();
        await triad.ForEachAsync(async x =>
        {
            await Task.Yield();
            lock (list)
            {
                list.Add(x);
            }
        });

        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Deconstruct_ShouldExtractItems()
    {
        var triad = new Triad<string>("A", "B", "C");

        var (first, second, third) = triad;

        Assert.Equal("A", first);
        Assert.Equal("B", second);
        Assert.Equal("C", third);
    }

    [Fact]
    public void Fold_ShouldReduceValues()
    {
        var triad = new Triad<int>(1, 2, 3);

        var folded = triad.Fold((a, b, c) => a + b + c);
        Assert.Equal(6, folded);

        var acc = triad.Fold("Init", (curr, next) => $"{curr}:{next}");
        Assert.Equal("Init:1:2:3", acc);
    }

    [Fact]
    public void MapAndZip_ShouldPreserveTriadShape()
    {
        var triad = new Triad<int>(1, 2, 3);
        var mapped = triad.Map(x => x * 2);

        Assert.Equal((2, 4, 6), mapped.ToValueTuple());

        var labels = new Triad<string>("one", "two", "three");
        var zipped = triad.Zip(labels, (num, str) => $"{str}={num}");

        Assert.Equal(("one=1", "two=2", "three=3"), zipped.ToValueTuple());
    }

    [Fact]
    public void PredicatesAndComparisons_ShouldEvaluateCorrectly()
    {
        var distinct = new Triad<int>(1, 2, 3);
        var duplicate = new Triad<int>(1, 2, 1);
        var equal = new Triad<int>(5, 5, 5);

        Assert.True(distinct.AreDistinct());
        Assert.False(distinct.AreEqual());

        Assert.False(duplicate.AreDistinct());
        Assert.False(duplicate.AreEqual());

        Assert.False(equal.AreDistinct());
        Assert.True(equal.AreEqual());

        Assert.True(distinct.All(x => x > 0));
        Assert.False(distinct.All(x => x > 1));
        Assert.True(distinct.Any(x => x == 3));
        Assert.False(distinct.Any(x => x == 99));
    }

    [Fact]
    public void Symmetry_ReverseAndRotate_ShouldWorkAsExpected()
    {
        var triad = new Triad<int>(1, 2, 3);

        var reversed = triad.Reverse();
        Assert.Equal((3, 2, 1), reversed.ToValueTuple());

        var left = triad.RotateLeft();
        Assert.Equal((2, 3, 1), left.ToValueTuple());

        var right = triad.RotateRight();
        Assert.Equal((3, 1, 2), right.ToValueTuple());
    }

    [Fact]
    public void ValueTupleConversion_ShouldWorkBothWays()
    {
        var triad = new Triad<int>(1, 2, 3);
        (int, int, int) tuple = triad;

        Assert.Equal((1, 2, 3), tuple);

        Triad<int> fromTuple = (4, 5, 6);
        Assert.Equal(4, fromTuple.First);
        Assert.Equal(5, fromTuple.Second);
        Assert.Equal(6, fromTuple.Third);
    }
}
