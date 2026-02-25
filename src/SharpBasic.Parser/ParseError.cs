using SharpBasic.Ast;

namespace SharpBasic.Parser;

public record ParseError(Exception Exception, int Line, int Col);