namespace TIKSN.Collections;

public class Octad<T, TRest> : Tuple<T, T, T, T, T, T, T, TRest> where TRest : notnull
{
    public Octad(T item1, T item2, T item3, T item4, T item5, T item6, T item7, TRest rest) : base(item1, item2, item3, item4, item5, item6, item7, rest)
    {
    }
}

public class Octad<T> : Tuple<T, T, T, T, T, T, T, T> where T : notnull
{
    public Octad(T item1, T item2, T item3, T item4, T item5, T item6, T item7, T rest) : base(item1, item2, item3, item4, item5, item6, item7, rest)
    {
    }
}
