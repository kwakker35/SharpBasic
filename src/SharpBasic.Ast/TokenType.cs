namespace SharpBasic.Ast;

public enum TokenType
{
    //Keywords
    Print, Let, If, Then, Else, End, While, Wend, For, To, Step, Next, Sub, Function, Dim, Const,
    Return, Call, As, Input, Select, Case, Set, Global,

    //Identifiers and Literals
    Identifier, StringLiteral, IntLiteral, FloatLiteral, True, False,
    Plus, Minus, Multiply, Divide, Mod, LParen, RParen,

    //Type Names
    Integer, Float, String, Boolean,

    //Operators and Punctuation
    Eq, NotEq, Lt, Gt, LtEq, GtEq, Ampersand, And, Or, Not, Comma,
    LBracket, RBracket, Semicolon,

    //structural
    NewLine, Eof, Unknown
}