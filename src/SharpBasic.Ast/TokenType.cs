namespace SharpBasic.Ast;

public enum TokenType
{
    //Keywords
    Print, Let, If, Then, Else, End, While, Wend, For, To, Step, Next, NextVar,

    //Identifiers and Literals
    Identifier, StringLiteral, IntLiteral, FloatLiteral,
    Plus, Minus, Multiply, Divide, LParen, RParen,

    //Operators and Punctuation
    Eq, NotEq, Lt, Gt, LtEq, GtEq,

    //structural
    NewLine, Eof, Unknown
}