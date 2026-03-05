namespace SharpBasic.Ast;

public enum TokenType
{
    //Keywords
    Print, Let,

    //Identifiers and Literals
    Identifier, StringLiteral, IntLiteral, FloatLiteral,
    Plus, Minus, Multiply, Divide, LParen, RParen,

    //Operators and Punctuation
    Eq,

    //structural
    NewLine, Eof, Unknown
}