namespace SharpBasic.Ast;

/// <summary>
/// SET GLOBAL name = value           (scalar)
/// SET GLOBAL name[i] = value        (1D array element)
/// SET GLOBAL name[r][c] = value     (2D array element)
/// Index and ColIndex are null for the scalar form.
/// </summary>
public record SetGlobalStatement(
    string Identifier,
    Expression? Index,
    Expression? ColIndex,
    Expression Value,
    SourceLocation Location)
    : Statement(Location);
