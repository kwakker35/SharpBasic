using SharpBasic.Ast;

namespace SharpBasic.Parsing;

public record ParseError(Exception Exception, int Line, int Col);