namespace TIKSN.Collections;

public class Tetrad<T> : Tuple<T, T, T, T>
{
    public Tetrad(T item1, T item2, T item3, T item4) : base(item1, item2, item3, item4)
    {
    }

    public T First => this.Item1;

    public T Fourth => this.Item4;

    public T Second => this.Item2;

    public T Third => this.Item3;

    public bool All(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return predicate(this.Item1)
            && predicate(this.Item2)
            && predicate(this.Item3)
            && predicate(this.Item4);
    }

    public bool Any(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return predicate(this.Item1)
            || predicate(this.Item2)
            || predicate(this.Item3)
            || predicate(this.Item4);
    }

    public bool AreDistinct(IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        return !comparer.Equals(this.Item1, this.Item2)
            && !comparer.Equals(this.Item1, this.Item3)
            && !comparer.Equals(this.Item1, this.Item4)
            && !comparer.Equals(this.Item2, this.Item3)
            && !comparer.Equals(this.Item2, this.Item4)
            && !comparer.Equals(this.Item3, this.Item4);
    }

    public bool AreEqual(IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        return comparer.Equals(this.Item1, this.Item2)
            && comparer.Equals(this.Item2, this.Item3)
            && comparer.Equals(this.Item3, this.Item4);
    }

    public void Deconstruct(out T item1, out T item2, out T item3, out T item4)
    {
        item1 = this.Item1;
        item2 = this.Item2;
        item3 = this.Item3;
        item4 = this.Item4;
    }

    public TResult Fold<TResult>(Func<T, T, T, T, TResult> folder)
    {
        ArgumentNullException.ThrowIfNull(folder);

        return folder(this.Item1, this.Item2, this.Item3, this.Item4);
    }

    public TAccumulate Fold<TAccumulate>(TAccumulate initial, Func<TAccumulate, T, TAccumulate> folder)
    {
        ArgumentNullException.ThrowIfNull(folder);

        return folder(folder(folder(folder(initial, this.Item1), this.Item2), this.Item3), this.Item4);
    }

    public void ForEach(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        action(this.Item1);
        action(this.Item2);
        action(this.Item3);
        action(this.Item4);
    }

    public Task ForEachAsync(
        Func<T, CancellationToken, Task> asyncAction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(asyncAction);

        var task1 = asyncAction(this.Item1, cancellationToken);
        var task2 = asyncAction(this.Item2, cancellationToken);
        var task3 = asyncAction(this.Item3, cancellationToken);
        var task4 = asyncAction(this.Item4, cancellationToken);

        return Task.WhenAll(task1, task2, task3, task4);
    }

    public Task ForEachAsync(Func<T, Task> asyncAction)
    {
        ArgumentNullException.ThrowIfNull(asyncAction);

        return this.ForEachAsync((item, _) => asyncAction(item), CancellationToken.None);
    }

    public Tetrad<TResult> Map<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return new(selector(this.Item1), selector(this.Item2), selector(this.Item3), selector(this.Item4));
    }

    public async Task<Tetrad<TResult>> MapAsync<TResult>(
        Func<T, CancellationToken, Task<TResult>> asyncSelector,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(asyncSelector);

        var task1 = asyncSelector(this.Item1, cancellationToken);
        var task2 = asyncSelector(this.Item2, cancellationToken);
        var task3 = asyncSelector(this.Item3, cancellationToken);
        var task4 = asyncSelector(this.Item4, cancellationToken);

        _ = await Task.WhenAll(task1, task2, task3, task4).ConfigureAwait(false);

        return new(
            await task1.ConfigureAwait(false),
            await task2.ConfigureAwait(false),
            await task3.ConfigureAwait(false),
            await task4.ConfigureAwait(false));
    }

    public Task<Tetrad<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> asyncSelector)
    {
        ArgumentNullException.ThrowIfNull(asyncSelector);

        return this.MapAsync((item, _) => asyncSelector(item), CancellationToken.None);
    }

    public Tetrad<T> Reverse() => new(this.Item4, this.Item3, this.Item2, this.Item1);

    public Tetrad<T> RotateLeft() => new(this.Item2, this.Item3, this.Item4, this.Item1);

    public Tetrad<T> RotateRight() => new(this.Item4, this.Item1, this.Item2, this.Item3);

    public (T, T, T, T) ToValueTuple() => (this.Item1, this.Item2, this.Item3, this.Item4);

    public Tetrad<TResult> Zip<TOther, TResult>(Tetrad<TOther> other, Func<T, TOther, TResult> zipper)
    {
        ArgumentNullException.ThrowIfNull(other);
        ArgumentNullException.ThrowIfNull(zipper);

        return new(
            zipper(this.Item1, other.Item1),
            zipper(this.Item2, other.Item2),
            zipper(this.Item3, other.Item3),
            zipper(this.Item4, other.Item4));
    }

#pragma warning disable CA2225 // Operator overloads have named alternates
    public static implicit operator (T, T, T, T)(Tetrad<T> tetrad)
    {
        ArgumentNullException.ThrowIfNull(tetrad);

        return tetrad.ToValueTuple();
    }

    public static implicit operator Tetrad<T>((T Item1, T Item2, T Item3, T Item4) tuple) =>
        new(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
#pragma warning restore CA2225 // Operator overloads have named alternates
}
