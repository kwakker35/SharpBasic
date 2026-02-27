using SharpBasic.Ast;

namespace SharpBasic.Parsing;

public record ParseSuccess(Program Program): ParseResult;