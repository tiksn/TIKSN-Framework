namespace TIKSN.Collections;

public class Dyad<T> : Tuple<T, T>
{
    public Dyad(T item1, T item2) : base(item1, item2)
    {
    }

    public T First => this.Item1;

    public T Second => this.Item2;

    public bool All(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return predicate(this.Item1) && predicate(this.Item2);
    }

    public bool Any(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return predicate(this.Item1) || predicate(this.Item2);
    }

    public bool AreDistinct(IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        return !comparer.Equals(this.Item1, this.Item2);
    }

    public bool AreEqual(IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        return comparer.Equals(this.Item1, this.Item2);
    }

    public void Deconstruct(out T item1, out T item2)
    {
        item1 = this.Item1;
        item2 = this.Item2;
    }

    public TResult Fold<TResult>(Func<T, T, TResult> folder)
    {
        ArgumentNullException.ThrowIfNull(folder);

        return folder(this.Item1, this.Item2);
    }

    public TAccumulate Fold<TAccumulate>(TAccumulate initial, Func<TAccumulate, T, TAccumulate> folder)
    {
        ArgumentNullException.ThrowIfNull(folder);

        return folder(folder(initial, this.Item1), this.Item2);
    }

    public void ForEach(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        action(this.Item1);
        action(this.Item2);
    }

    public Task ForEachAsync(
        Func<T, CancellationToken, Task> asyncAction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(asyncAction);

        var task1 = asyncAction(this.Item1, cancellationToken);
        var task2 = asyncAction(this.Item2, cancellationToken);

        return Task.WhenAll(task1, task2);
    }

    public Task ForEachAsync(Func<T, Task> asyncAction)
    {
        ArgumentNullException.ThrowIfNull(asyncAction);

        return this.ForEachAsync((item, _) => asyncAction(item), CancellationToken.None);
    }

    public Dyad<TResult> Map<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return new(selector(this.Item1), selector(this.Item2));
    }

    public async Task<Dyad<TResult>> MapAsync<TResult>(
        Func<T, CancellationToken, Task<TResult>> asyncSelector,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(asyncSelector);

        var task1 = asyncSelector(this.Item1, cancellationToken);
        var task2 = asyncSelector(this.Item2, cancellationToken);

        _ = await Task.WhenAll(task1, task2).ConfigureAwait(false);

        return new(await task1.ConfigureAwait(false), await task2.ConfigureAwait(false));
    }

    public Task<Dyad<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> asyncSelector)
    {
        ArgumentNullException.ThrowIfNull(asyncSelector);

        return this.MapAsync((item, _) => asyncSelector(item), CancellationToken.None);
    }

    public Dyad<T> Reverse() => this.Swap();

    public Dyad<T> Swap() => new(this.Item2, this.Item1);

    public (T, T) ToValueTuple() => (this.Item1, this.Item2);

    public Dyad<TResult> Zip<TOther, TResult>(Dyad<TOther> other, Func<T, TOther, TResult> zipper)
    {
        ArgumentNullException.ThrowIfNull(other);
        ArgumentNullException.ThrowIfNull(zipper);

        return new(zipper(this.Item1, other.Item1), zipper(this.Item2, other.Item2));
    }

#pragma warning disable CA2225 // Operator overloads have named alternates
    public static implicit operator (T, T)(Dyad<T> dyad)
    {
        ArgumentNullException.ThrowIfNull(dyad);

        return dyad.ToValueTuple();
    }

    public static implicit operator Dyad<T>((T Item1, T Item2) tuple) =>
        new(tuple.Item1, tuple.Item2);
#pragma warning restore CA2225 // Operator overloads have named alternates
}
