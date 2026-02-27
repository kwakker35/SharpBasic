using SharpBasic.Ast;

namespace SharpBasic.Parsing;
public record ParseFailure(IReadOnlyList<ParseError> Errors): ParseResult;