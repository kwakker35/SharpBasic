using SharpBasic.Ast;

namespace SharpBasic.Parser;
public record ParseFailure(IReadOnlyList<ParseError> Errors): ParseResult;