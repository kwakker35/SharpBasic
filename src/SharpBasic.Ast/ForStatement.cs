namespace SharpBasic.Ast;

public record ForStatement(Token LoopVar,
                            Expression Start,
                            Expression Limit,
                            Expression? Step,
                            Token? NextVar,
                            List<Statement> Body,
                            SourceLocation Location)
    : Statement(Location);