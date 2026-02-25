using SharpBasic.Ast;

namespace SharpBasic.Parser;

public record ParseSuccess(Program Program): ParseResult;