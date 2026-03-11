namespace SharpBasic.Ast;

public enum TokenType
{
    //Keywords
    Print, Let, If, Then, Else, End, While, Wend, For, To, Step, Next, Sub, Function,
    Return, Call, As,

    //Identifiers and Literals
    Identifier, StringLiteral, IntLiteral, FloatLiteral, True, False,
    Plus, Minus, Multiply, Divide, LParen, RParen,

    //Type Names
    Integer, Float, String, Boolean,

    //Operators and Punctuation
    Eq, NotEq, Lt, Gt, LtEq, GtEq, Ampersand, And, Or, Not, Comma,

    //structural
    NewLine, Eof, Unknown
}