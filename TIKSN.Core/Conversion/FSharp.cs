using System;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.FSharp.Core;

namespace TIKSN.Conversion
{
    public static class FSharp
    {
        public static Fin<T> ToFin<T>(this FSharpResult<T, Error> result)
            => result.IsOk
                ? Fin<T>.Succ(result.ResultValue)
                : Fin<T>.Fail(result.ErrorValue);

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
}
