using FluentValidation;
using LanguageExt.Parsec;

namespace TIKSN.Parsing;

public static class ParserResultExtensions
{
    public static T GetOrThrow<T>(this ParserResult<T> parserResult) => parserResult.Match(
          e => throw CreateValidationException(e),
          c => throw CreateValidationException(c),
          o => o.Reply.Result);

    private static ValidationException CreateValidationException(ParserError error) => new(error.Msg);
}

