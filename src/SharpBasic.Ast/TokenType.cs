namespace SharpBasic.Ast;

public enum TokenType
{
    //Keywords
    Print, Let,

    //Identifiers and Literals
    Identifier, StringLiteral,

    //Operators and Punctuation
    Eq,

    //structural
    NewLine, Eof, Unknown
}