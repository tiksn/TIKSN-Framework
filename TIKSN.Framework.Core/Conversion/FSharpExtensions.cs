using LanguageExt;
using LanguageExt.Common;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace TIKSN.Conversion;

public static class FSharpExtensions
{
    public static Fin<T> ToFin<T>(this FSharpResult<T, Error> result)
        => result.IsOk
            ? Fin<T>.Succ(result.ResultValue)
            : Fin<T>.Fail(result.ErrorValue);

    public static Fin<T> ToFin<T>(this FSharpResult<T, Exception> result)
        => result.IsOk
            ? Fin<T>.Succ(result.ResultValue)
            : Fin<T>.Fail(result.ErrorValue);

    public static Fin<T> ToFin<T>(this FSharpResult<T, string> result)
        => result.IsOk
            ? Fin<T>.Succ(result.ResultValue)
            : Fin<T>.Fail(Error.New(result.ErrorValue));

    public static Fin<T> ToFin<T>(this FSharpResult<T, Tuple<int, string>> result)
        => result.IsOk
            ? Fin<T>.Succ(result.ResultValue)
            : Fin<T>.Fail(Error.New(result.ErrorValue.Item1, result.ErrorValue.Item2));

    public static Fin<T> ToFin<T>(this FSharpResult<T, ValueTuple<int, string>> result)
        => result.IsOk
            ? Fin<T>.Succ(result.ResultValue)
            : Fin<T>.Fail(Error.New(result.ErrorValue.Item1, result.ErrorValue.Item2));

    public static Fin<T> ToFin<T>(this FSharpResult<T, FSharpList<Tuple<int, string>>> result)
        => result.IsOk
            ? Fin<T>.Succ(result.ResultValue)
            : Fin<T>.Fail(Error.Many(result.ErrorValue.Map(x => Error.New(x.Item1, x.Item2)).ToSeq()));

    public static Fin<T> ToFin<T>(this FSharpResult<T, FSharpList<ValueTuple<int, string>>> result)
        => result.IsOk
            ? Fin<T>.Succ(result.ResultValue)
            : Fin<T>.Fail(Error.Many(result.ErrorValue.Map(x => Error.New(x.Item1, x.Item2)).ToSeq()));

    public static Fin<T> ToFin<T>(this FSharpResult<T, FSharpList<string>> result)
        => result.IsOk
            ? Fin<T>.Succ(result.ResultValue)
            : Fin<T>.Fail(Error.Many(result.ErrorValue.Map(x => Error.New(x)).ToSeq()));

    public static FSharpResult<T, Error> ToFSharp<T>(this Fin<T> fin)
        => fin.Match(
            FSharpResult<T, Error>.NewOk,
            FSharpResult<T, Error>.NewError);

    public static FSharpResult<T, Exception> ToFSharp<T>(this Result<T> result)
        => result.Match(
            FSharpResult<T, Exception>.NewOk,
            FSharpResult<T, Exception>.NewError);

    public static Result<T> ToResult<T>(this FSharpResult<T, Exception> result)
        => result.IsOk
            ? new Result<T>(result.ResultValue)
            : new Result<T>(result.ErrorValue);
}
