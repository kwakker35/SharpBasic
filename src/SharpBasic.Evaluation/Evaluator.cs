using SharpBasic.Ast;

namespace SharpBasic.Evaluation;

public class Evaluator(
    Program _program,
    SymbolTable? table = null,
    Dictionary<string, SubDeclaration>? subs = null,
    Dictionary<string, FunctionDeclaration>? functions = null)
{
    private SymbolTable _table = table ?? new();
    private Dictionary<string, SubDeclaration> _subs = subs ?? new();
    private Dictionary<string, FunctionDeclaration> _functions = functions ?? new();
    private List<Diagnostic> _diagnostics = [];

    private static readonly Dictionary<string, Func<List<Value>, Value?>> _builtins = CreateBuiltins();

    private static Dictionary<string, Func<List<Value>, Value?>> CreateBuiltins() => new()
    {
        ["LEN"] = args => args[0] is StringValue sv ?
                            new IntValue(sv.V.Length)
                            : null,
        ["MID$"] = args => args[0] is StringValue sv &&
                            args[1] is IntValue iv1 &&
                            args[2] is IntValue iv2 ? new StringValue(
                                sv.V.Substring(iv1.V - 1, iv2.V)
                            ) : null,
        ["LEFT$"] = args => args[0] is StringValue sv &&
                            args[1] is IntValue iv1 ? new StringValue(
                                sv.V[..iv1.V]
                            ) : null,
        ["RIGHT$"] = args => args[0] is StringValue sv &&
                            args[1] is IntValue iv1 ? new StringValue(
                                sv.V[^iv1.V..]
                            ) : null,
        ["TRIM$"] = args => args[0] is StringValue sv ? new StringValue(
                                sv.V.Trim()
                            ) : null,
        ["UPPER$"] = args => args[0] is StringValue sv ? new StringValue(
                            sv.V.ToUpperInvariant()
                            ) : null,
        ["LOWER$"] = args => args[0] is StringValue sv ? new StringValue(
                            sv.V.ToLowerInvariant()
                            ) : null,
        ["INT"] = args => args[0] is IntValue iv ?
                            new IntValue(iv.V) :
                            args[0] is FloatValue fv ?
                            new FloatValue(Math.Floor(fv.V))
                            : null,
        ["STR$"] = args => args[0] is IntValue iv ?
                            new StringValue(iv.V.ToString()) :
                            args[0] is FloatValue fv ?
                            new StringValue(fv.V.ToString())
                            : null,
        ["VAL"] = args => args[0] is StringValue sv ?
                            int.TryParse(sv.V, out int ir) ? new IntValue(ir) :
                            double.TryParse(sv.V,
                                System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out double dr) ? new FloatValue(dr)
                            : null
                            : null,
        ["ABS"] = args => args[0] is IntValue iv ?
                            new IntValue(Math.Abs(iv.V)) :
                            args[0] is FloatValue fv ?
                            new FloatValue(Math.Abs(fv.V))
                            : null,
        ["SQR"] = args => args[0] is IntValue iv ?
                            new FloatValue(Math.Sqrt(iv.V)) :
                            args[0] is FloatValue fv ?
                            new FloatValue(Math.Sqrt(fv.V))
                            : null,
        ["RND"] = args => new FloatValue(Random.Shared.NextDouble()),
        ["TYPENAME"] = args => new StringValue(args[0].TypeName),
        ["CHR$"] = args => args[0] is IntValue iv ? new StringValue(((char)iv.V).ToString()) : null
    };

    public EvalResult Evaluate()
    {
        var hoistResult = HoistDeclarations();
        if (hoistResult is EvalFailure) return hoistResult;

        foreach (Statement stmt in _program.Statements)
        {
            var result = EvaluateStatement(stmt);

            if (result is EvalFailure failure)
            {
                _diagnostics.AddRange(failure.Diagnostics);
            }
        }


        return _diagnostics.Count > 0 ? new EvalFailure(_diagnostics) : new EvalSuccess(new VoidValue());
    }

    private EvalResult HoistDeclarations()
    {
        foreach (Statement stmt in _program.Statements)
        {
            if (stmt is SubDeclaration s)
            {
                if (_builtins.ContainsKey(s.Name.ToUpperInvariant()))
                {
                    return new EvalFailure(
                        [
                            new Diagnostic(
                                stmt.Location?.Line ?? 0,
                                stmt.Location?.Col ?? 0,
                                $"SUB {s.Name} is the name of a built in function and cannot be reused.",
                                DiagnosticSeverity.Error
                            )
                        ]
                    );
                }

                _subs.Add(s.Name, s);
            }
            else if (stmt is FunctionDeclaration f)
            {
                if (_builtins.ContainsKey(f.Name.ToUpperInvariant()))
                {
                    return new EvalFailure(
                        [
                            new Diagnostic(
                                stmt.Location?.Line ?? 0,
                                stmt.Location?.Col ?? 0,
                                $"FUNCTION {f.Name} is the name of a built in function and cannot be reused.",
                                DiagnosticSeverity.Error
                            )
                        ]
                    );
                }

                _functions.Add(f.Name, f);
            }
        }

        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateStatement(Statement stmt)
    {
        return stmt switch
        {
            PrintStatement p => EvaluatePrintStatement(p),
            LetStatement l => EvaluateLetStatement(l),
            IfStatement i => EvaluateIfStatement(i),
            WhileStatement w => EvaluateWhileStatement(w),
            ForStatement f => EvaluateForStatement(f),
            SubDeclaration => new EvalSuccess(new VoidValue()),
            FunctionDeclaration => new EvalSuccess(new VoidValue()),
            CallStatement cs => EvaluateCallStatement(cs),
            ReturnStatement rs => EvaluateReturnStatement(rs),
            DimStatement ds => EvaluateDimStatement(ds),
            Dim2dStatement ds2 => EvaluateDim2dStatement(ds2),
            ArrayAssignStatement aas => EvaluateArrayAssignStatement(aas),
            Array2dAssignStatement aas2 => EvaluateArray2dAssignStatement(aas2),
            InputStatement ins => EvaluateInputStatement(ins),
            ConstStatement cs => EvaluateConstStatement(cs),
            SelectCaseStatement scs => EvaluateSelectCaseStatement(scs),
            SetGlobalStatement sgs => EvaluateSetGlobalStatement(sgs),
            _
                => new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0,
                            $"Unknown statement type: {stmt.GetType().Name}",
                            DiagnosticSeverity.Error
                        )
                    ]
                )
        };
    }

    private EvalResult EvaluateSetGlobalStatement(SetGlobalStatement stmt)
    {
        if (_table.IsGlobal)
            return new EvalFailure(
                [new Diagnostic(
                    stmt.Location.Line,
                    stmt.Location.Col,
                    "'SET GLOBAL' is not valid outside a SUB or FUNCTION.",
                    DiagnosticSeverity.Error
                )]
            );

        var root = _table.Root;

        if (root.Get(stmt.Identifier) is null)
            return new EvalFailure(
                [new Diagnostic(
                    stmt.Location.Line,
                    stmt.Location.Col,
                    $"'{stmt.Identifier}' does not exist in global scope.",
                    DiagnosticSeverity.Error
                )]
            );

        var valueResult = EvaluateExpression(stmt.Value);
        if (valueResult is EvalFailure) return valueResult;

        root.Set(stmt.Identifier, ((EvalSuccess)valueResult).Value!);
        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateSelectCaseStatement(SelectCaseStatement stmt)
    {
        var subjectResult = EvaluateExpression(stmt.Subject);
        if (subjectResult is EvalFailure) return subjectResult;
        var subject = ((EvalSuccess)subjectResult).Value;

        foreach (var clause in stmt.Cases)
        {
            foreach (var valueExpr in clause.Values)
            {
                var valResult = EvaluateExpression(valueExpr);
                if (valResult is EvalFailure) return valResult;
                var val = ((EvalSuccess)valResult).Value;

                if (subject == val)
                {
                    List<Diagnostic> errors = [];
                    foreach (var s in clause.Body)
                    {
                        var r = EvaluateStatement(s);
                        if (r is EvalFailure f) errors.AddRange(f.Diagnostics);
                    }
                    return errors.Count > 0 ? new EvalFailure(errors) : new EvalSuccess(new VoidValue());
                }
            }
        }

        if (stmt.Else is not null)
        {
            List<Diagnostic> errors = [];
            foreach (var s in stmt.Else.Body)
            {
                var r = EvaluateStatement(s);
                if (r is EvalFailure f) errors.AddRange(f.Diagnostics);
            }
            return errors.Count > 0 ? new EvalFailure(errors) : new EvalSuccess(new VoidValue());
        }

        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateInputStatement(InputStatement stmt)
    {
        var target = stmt.Target.Name;
        string inputVal;

        if (stmt.Prompt is not null)
        {
            Console.Write(stmt.Prompt + "? ");
        }

        inputVal = Console.ReadLine() ?? string.Empty;

        _table.Set(target, new StringValue(inputVal));

        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateDimStatement(DimStatement stmt)
    {
        var name = stmt.Name;
        var exists = _table.Get(name);

        if (exists is not null)
        {
            return new EvalFailure(
                [
                    new Diagnostic(
                        stmt.Location?.Line ?? 0,
                        stmt.Location?.Col ?? 0,
                        $"The variable {name} already exists and cannot be redefined.",
                        DiagnosticSeverity.Error
                    )
                ]
            );
        }

        var arrVal = new Value[stmt.Size];

        for (var i = 0; i < stmt.Size; i++)
        {
            arrVal[i] = stmt.TypeName.ToUpperInvariant() switch
            {
                "INTEGER" => new IntValue(0),
                "FLOAT" => new FloatValue(0),
                "BOOLEAN" => new BoolValue(false),
                "STRING" => new StringValue(string.Empty),
                _ => new VoidValue()
            };
        }

        var val = new ArrayValue(arrVal, stmt.TypeName);
        _table.Set(name, val);
        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateDim2dStatement(Dim2dStatement stmt)
    {
        var name = stmt.Name;
        var exists = _table.Get(name);

        if (exists is not null)
        {
            return new EvalFailure(
                [
                    new Diagnostic(
                        stmt.Location?.Line ?? 0,
                        stmt.Location?.Col ?? 0,
                        $"The variable {name} already exists and cannot be redefined.",
                        DiagnosticSeverity.Error
                    )
                ]
            );
        }

        var total = stmt.Rows * stmt.Cols;
        var arrVal = new Value[total];
        var defaultVal = stmt.TypeName.ToUpperInvariant() switch
        {
            "INTEGER" => (Value)new IntValue(0),
            "FLOAT" => new FloatValue(0),
            "BOOLEAN" => new BoolValue(false),
            "STRING" => new StringValue(string.Empty),
            _ => new VoidValue()
        };

        for (var i = 0; i < total; i++)
            arrVal[i] = defaultVal;

        var val = new ArrayValue(arrVal, stmt.TypeName, stmt.Cols);
        _table.Set(name, val);
        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateReturnStatement(ReturnStatement stmt)
    {
        if (stmt.Value is null)
            throw new ReturnException(null);

        var result = EvaluateExpression(stmt.Value);
        if (result is EvalFailure) return result;

        throw new ReturnException(((EvalSuccess)result).Value);
    }

    private EvalResult EvaluatePrintStatement(PrintStatement p)
    {
        var result = EvaluateExpression(p.Value);
        if (result is EvalFailure)
            return result;

        if (result is EvalSuccess es)
            Console.WriteLine(es.Value?.ToString() ?? string.Empty);

        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateLetStatement(LetStatement l)
    {
        var ident = l.Identifier.Value;

        if (_table.IsConst(ident))
            return new EvalFailure(
                [
                    new Diagnostic(
                        l.Location?.Line ?? 0,
                        l.Location?.Col ?? 0,
                        $"Cannot assign to CONST '{ident}'.",
                        DiagnosticSeverity.Error
                    )
                ]
            );

        var valueRes = EvaluateExpression(l.Value);

        if (valueRes is EvalFailure)
            return valueRes;

        if (valueRes is EvalSuccess es && es.Value is not null)
        {
            _table.Set(ident, es.Value);
            return new EvalSuccess(new VoidValue());
        }

        return new EvalFailure(
            [
                new Diagnostic(
                    l.Location?.Line ?? 0,
                    l.Location?.Col ?? 0,
                    $"Value assigned to '{l.Identifier.Value}' was null.",
                    DiagnosticSeverity.Error
                )
            ]
        );
    }

    private EvalResult EvaluateConstStatement(ConstStatement cs)
    {
        var name = cs.Identifier.Value;

        if (_table.IsConst(name))
            return new EvalFailure(
                [
                    new Diagnostic(
                        cs.Location?.Line ?? 0,
                        cs.Location?.Col ?? 0,
                        $"Cannot redefine CONST '{name}'.",
                        DiagnosticSeverity.Error
                    )
                ]
            );

        var valueRes = EvaluateExpression(cs.Value);

        if (valueRes is EvalFailure) return valueRes;

        if (valueRes is EvalSuccess es && es.Value is not null)
        {
            _table.SetConst(name, es.Value);
            return new EvalSuccess(new VoidValue());
        }

        return new EvalFailure(
            [
                new Diagnostic(
                    cs.Location?.Line ?? 0,
                    cs.Location?.Col ?? 0,
                    $"Value for CONST '{name}' was null.",
                    DiagnosticSeverity.Error
                )
            ]
        );
    }

    private EvalResult EvaluateIfStatement(IfStatement stmt)
    {
        var result = EvaluateExpression(stmt.Condition);

        if (result is EvalFailure) return result;

        bool value = false;
        List<Diagnostic> localErrors = [];

        if (result is EvalSuccess es && es.Value is BoolValue bv)
        {
            value = bv.V;
        }
        else
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Condition.Location?.Line ?? 0,
                            stmt.Condition.Location?.Col ?? 0,
                            "IF condition must evaluate to a boolean value.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }


        if (value)
        {
            foreach (Statement thenStmt in stmt.ThenBlock)
            {
                var thenResult = EvaluateStatement(thenStmt);

                if (thenResult is EvalFailure failure)
                {
                    localErrors.AddRange(failure.Diagnostics);
                }
            }
        }
        else if (stmt.ElseBlock is not null)
        {
            foreach (Statement elseStmt in stmt.ElseBlock)
            {
                var elseResult = EvaluateStatement(elseStmt);

                if (elseResult is EvalFailure failure)
                {
                    localErrors.AddRange(failure.Diagnostics);
                }
            }
        }

        return localErrors.Count > 0 ?
                new EvalFailure(localErrors) :
                new EvalSuccess(new VoidValue());

    }

    private EvalResult EvaluateForStatement(ForStatement stmt)
    {
        List<Diagnostic> localErrors = [];
        var startEx = EvaluateExpression(stmt.Start);
        var limitEx = EvaluateExpression(stmt.Limit);
        var stepEx = stmt.Step is null ?
                        new EvalSuccess(new FloatValue(1)) :
                        EvaluateExpression(stmt.Step);


        if (startEx is EvalFailure)
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Start.Location?.Line ?? 0,
                            stmt.Start.Location?.Col ?? 0,
                            "Invalid START expression or value",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }
        else if (limitEx is EvalFailure)
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Limit.Location?.Line ?? 0,
                            stmt.Limit.Location?.Col ?? 0,
                            "Invalid LIMIT expression or value",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }
        else if (stepEx is EvalFailure)
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Step?.Location?.Line ?? 0,
                            stmt.Step?.Location?.Col ?? 0,
                            "Invalid STEP expression or value",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }


        if (startEx is EvalSuccess startS &&
            limitEx is EvalSuccess limitS &&
            stepEx is EvalSuccess stepS)
        {
            // evaluated once before the loop
            double i = ToFloat(startS.Value);
            double limit = ToFloat(limitS.Value);
            double step = ToFloat(stepS.Value);

            _table.Set(stmt.LoopVar.Value, new IntValue((int)i));

            while (true)
            {
                // condition depends on step direction
                bool shouldRun = step > 0 ? i <= limit : i >= limit;
                if (!shouldRun) break;

                _table.Set(stmt.LoopVar.Value, new IntValue((int)i));

                foreach (Statement bodyStmt in stmt.Body)
                {
                    var bodyResult = EvaluateStatement(bodyStmt);

                    if (bodyResult is EvalFailure failure)
                    {
                        localErrors.AddRange(failure.Diagnostics);
                    }
                }

                // increment
                i += step;
            }
        }

        return localErrors.Count > 0 ?
               new EvalFailure(localErrors) :
               new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateWhileStatement(WhileStatement stmt)
    {
        List<Diagnostic> localErrors = [];

        while (true)
        {
            bool loop = false;
            var result = EvaluateExpression(stmt.Condition);

            if (result is EvalFailure) return result;

            if (result is EvalSuccess es && es.Value is BoolValue bv)
            {
                loop = bv.V;
            }
            else
            {
                return new EvalFailure(
                        [
                            new Diagnostic(
                            stmt.Condition.Location?.Line ?? 0,
                            stmt.Condition.Location?.Col ?? 0,
                            "WHILE condition must evaluate to a boolean value.",
                            DiagnosticSeverity.Error
                        )
                        ]
                    );
            }

            if (!loop) break;

            foreach (Statement bodyStmt in stmt.Body)
            {
                var bodyResult = EvaluateStatement(bodyStmt);

                if (bodyResult is EvalFailure failure)
                {
                    localErrors.AddRange(failure.Diagnostics);
                }
            }
        }

        return localErrors.Count > 0 ?
                new EvalFailure(localErrors) :
                new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateCallStatement(CallStatement stmt)
    {
        var subExists = _subs.TryGetValue(stmt.Name, out var sub);

        if (!subExists)
        {
            return new EvalFailure(
                        [
                            new Diagnostic(
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0,
                            $"Sub: {stmt.Name} not found.",
                            DiagnosticSeverity.Error
                        )
                        ]
                    );
        }

        var localSymbols = new SymbolTable(_table);

        for (int i = 0; i < sub!.Parameters.Count; i++)
        {
            var argResult = EvaluateExpression(stmt.Arguments[i]);  // evaluate in caller's scope
            if (argResult is EvalFailure) return argResult;
            localSymbols.Set(sub.Parameters[i].Name, ((EvalSuccess)argResult).Value!);
        }

        try
        {
            var funcEval = new Evaluator(
                                new Program(sub.Body),
                                localSymbols,
                                _subs,
                                _functions
                            ).Evaluate();
            if (funcEval is EvalFailure) return funcEval;
        }
        catch (ReturnException re)
        {
            //sucess hit return statement and swallow the return value
            return new EvalSuccess(new VoidValue());
        }
        catch (Exception e)
        {
            //unkown exception
            return new EvalFailure(
                        [
                            new Diagnostic(
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0,
                            e.Message,
                            DiagnosticSeverity.Error
                        )
                        ]
                    );
        }

        //no return statment - no problem!
        return new EvalSuccess(new VoidValue());
    }

    private EvalResult EvaluateCallExpression(CallExpression expr)
    {
        var builtInExists = _builtins.TryGetValue(expr.Name.ToUpperInvariant(), out var builtIn);

        if (builtInExists)
        {
            List<Value> localArgs = [];

            foreach (var arg in expr.Arguments)
            {
                var argResult = EvaluateExpression(arg);  // evaluate in caller's scope
                if (argResult is EvalFailure) return argResult;

                localArgs.Add(((EvalSuccess)argResult).Value!);
            }

            return new EvalSuccess(builtIn!(localArgs));
        }

        var funcExists = _functions.TryGetValue(expr.Name, out var func);

        if (!funcExists)
        {
            return new EvalFailure(
                        [
                            new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Function: {expr.Name} not found.",
                            DiagnosticSeverity.Error
                        )
                        ]
                    );
        }

        var localSymbols = new SymbolTable(_table);

        for (int i = 0; i < func!.Parameters.Count; i++)
        {
            var argResult = EvaluateExpression(expr.Arguments[i]);  // evaluate in caller's scope
            if (argResult is EvalFailure) return argResult;
            localSymbols.Set(func.Parameters[i].Name, ((EvalSuccess)argResult).Value!);
        }

        try
        {
            var funcEval = new Evaluator(
                                new Program(func.Body),
                                localSymbols,
                                _subs,
                                _functions
                            ).Evaluate();
            if (funcEval is EvalFailure) return funcEval;
        }
        catch (ReturnException re)
        {
            //sucess hit return statement 
            return new EvalSuccess(re.ReturnValue);
        }
        catch (Exception e)
        {
            //unkown exception
            return new EvalFailure(
                        [
                            new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            e.Message,
                            DiagnosticSeverity.Error
                        )
                        ]
                    );
        }

        //no return statement hit
        return new EvalFailure(
                        [
                            new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            "Missing RETURN statement in FUNCTION.",
                            DiagnosticSeverity.Error
                        )
                        ]
                    );

    }

    private static double ToFloat(Value v) => v switch
    {
        IntValue iv => iv.V,
        FloatValue fv => fv.V,
        _ => throw new InvalidOperationException($"Cannot convert {v.GetType().Name} to float")
    };

    private EvalResult EvaluateBinaryExpression(BinaryExpression expr)
    {
        if (expr.Operator.Type is not (TokenType.Plus or
                                        TokenType.Minus or
                                        TokenType.Multiply or
                                        TokenType.Divide or
                                        TokenType.Eq or
                                        TokenType.NotEq or
                                        TokenType.Lt or
                                        TokenType.Gt or
                                        TokenType.LtEq or
                                        TokenType.GtEq or
                                        TokenType.Ampersand or
                                        TokenType.Mod or
                                        TokenType.And or
                                        TokenType.Or))
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Unknown statement type: {expr.Operator.GetType().Name}",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }

        var leftRes = EvaluateExpression(expr.Left);
        if (leftRes is EvalFailure) return leftRes;
        var leftES = (EvalSuccess)leftRes;
        var rightRes = EvaluateExpression(expr.Right);
        if (rightRes is EvalFailure) return rightRes;
        var rightES = (EvalSuccess)rightRes;

        //boolean operations
        if (expr.Operator.Type is TokenType.And or TokenType.Or)
        {
            if (leftES.Value is BoolValue lbv && rightES.Value is BoolValue rbv)
            {
                bool result = expr.Operator.Type switch
                {
                    TokenType.And => lbv.V && rbv.V,
                    TokenType.Or => lbv.V || rbv.V,
                    _ => throw new InvalidOperationException("Unreachable")
                };

                return new EvalSuccess(new BoolValue(result));
            }
            else
            {
                return new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Invalid combination: {leftES.Value.GetType()} : {rightES.Value.GetType()}. Cannot perform AND or OR on non boolean values.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
            }
        }

        // string concatenation
        if (expr.Operator.Type is TokenType.Ampersand)
        {
            return new EvalSuccess(new StringValue(
                (leftES.Value?.ToString() ?? "") + (rightES.Value?.ToString() ?? "")
            ));
        }

        if (leftES.Value is StringValue lsv && rightES.Value is StringValue rsv)
        {
            //string comparison
            if (expr.Operator.Type is TokenType.Eq or
                                        TokenType.NotEq)
            {
                bool result = expr.Operator.Type switch
                {
                    TokenType.Eq => lsv == rsv,
                    TokenType.NotEq => lsv != rsv,
                    _ => throw new InvalidOperationException("Unreachable")
                };

                return new EvalSuccess(new BoolValue(result));
            }
            else
            {
                return new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Unknown Expression: {expr.GetType()}",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
            }
        }

        var isFloat = leftES.Value is FloatValue || rightES.Value is FloatValue;

        double left = ToFloat(leftES.Value);
        double right = ToFloat(rightES.Value);

        if (expr.Operator.Type is TokenType.Divide
                && right == 0)
        {
            return new EvalFailure(
                [
                    new Diagnostic(
                        expr.Location?.Line ?? 0,
                        expr.Location?.Col ?? 0,
                        "Attempted to divide by zero.",
                        DiagnosticSeverity.Error
                    )
                ]
            );
        }

        //Arithmetic
        if (expr.Operator.Type is TokenType.Plus or
                                        TokenType.Minus or
                                        TokenType.Multiply or
                                        TokenType.Divide or
                                        TokenType.Mod)
        {
            double result = expr.Operator.Type switch
            {
                TokenType.Plus => left + right,
                TokenType.Minus => left - right,
                TokenType.Multiply => left * right,
                TokenType.Divide => left / right,
                TokenType.Mod => (int)left % (int)right,
                _ => throw new InvalidOperationException("Unreachable")
            };

            Value output = isFloat ? new FloatValue(result) :
                    double.IsInteger(result) ?
                        new IntValue((int)result) :
                        new FloatValue(result);

            return new EvalSuccess(output);
        } //Boolean
        else if (expr.Operator.Type is TokenType.Eq or
                                        TokenType.NotEq or
                                        TokenType.Lt or
                                        TokenType.Gt or
                                        TokenType.LtEq or
                                        TokenType.GtEq)
        {
            bool result = expr.Operator.Type switch
            {
                TokenType.Eq => left == right,
                TokenType.NotEq => left != right,
                TokenType.Lt => left < right,
                TokenType.Gt => left > right,
                TokenType.LtEq => left <= right,
                TokenType.GtEq => left >= right,
                _ => throw new InvalidOperationException("Unreachable")
            };

            return new EvalSuccess(new BoolValue(result));
        }

        return new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Unknown Expression: {expr.GetType()}",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
    }

    private EvalResult EvaluateArrayAssignStatement(ArrayAssignStatement stmt)
    {
        var getVal = _table.Get(stmt.Name);

        if (getVal is not ArrayValue)
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0,
                            $"Identifier: {stmt.Name} is not an Array.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }

        var arrVal = (ArrayValue)getVal;

        var idxExpr = EvaluateExpression(stmt.Index);

        if (idxExpr is EvalSuccess es && es.Value is IntValue iv)
        {
            var targetType = arrVal.ElementTypeName.ToUpperInvariant();
            var idx = iv.V;
            if (idx < 0 || idx >= arrVal.Items.Length)
            {
                return new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0,
                            $"Supplied index {idx} is outside of the range 0-{arrVal.Items.Length} defined for the array: {stmt.Name}.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
            }

            var valueResult = EvaluateExpression(stmt.Value);
            if (valueResult is not EvalSuccess valueSuccess)
                return valueResult;

            var value = valueSuccess.Value;
            if (targetType == "INTEGER" && value is IntValue ival)
            {
                arrVal.Items[idx] = ival;
                _table.Set(stmt.Name, arrVal);
                return new EvalSuccess(new VoidValue());
            }
            else if (targetType == "FLOAT" && value is FloatValue fval)
            {
                arrVal.Items[idx] = fval;
                _table.Set(stmt.Name, arrVal);
                return new EvalSuccess(new VoidValue());
            }
            else if (targetType == "STRING" && value is StringValue sval)
            {
                arrVal.Items[idx] = sval;
                _table.Set(stmt.Name, arrVal);
                return new EvalSuccess(new VoidValue());
            }
            else if (targetType == "BOOLEAN" && value is BoolValue bval)
            {
                arrVal.Items[idx] = bval;
                _table.Set(stmt.Name, arrVal);
                return new EvalSuccess(new VoidValue());
            }
            else
            {
                return new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0,
                            $"Supplied value is not an {targetType} as defined for the array: {stmt.Name}.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
            }
        }
        else
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            stmt.Location?.Line ?? 0,
                            stmt.Location?.Col ?? 0,
                            "Invalid index suppled.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }
    }

    private EvalResult EvaluateArrayAccessExpression(ArrayAccessExpression expr)
    {
        var getVal = _table.Get(expr.Name);

        if (getVal is not ArrayValue)
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Identifier: {expr.Name} is not an Array.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }

        var arrVal = (ArrayValue)getVal;

        var idxExpr = EvaluateExpression(expr.Index);

        if (idxExpr is EvalSuccess es && es.Value is IntValue iv)
        {
            var idx = iv.V;
            if (idx < 0 || idx >= arrVal.Items.Length)
            {
                return new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Supplied index {idx} is outside of the range 0-{arrVal.Items.Length} defined for the array: {expr.Name}.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
            }

            return arrVal.ElementTypeName.ToUpperInvariant() switch
            {
                "INTEGER" => new EvalSuccess((IntValue)arrVal.Items[idx]),
                "FLOAT" => new EvalSuccess((FloatValue)arrVal.Items[idx]),
                "BOOLEAN" => new EvalSuccess((BoolValue)arrVal.Items[idx]),
                "STRING" => new EvalSuccess((StringValue)arrVal.Items[idx]),
                _ => new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Unknown Typename supplied: {arrVal.TypeName}",
                            DiagnosticSeverity.Error
                        )
                    ]
                )
            };
        }
        else
        {
            return new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            "Invalid index suppled.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
        }
    }

    private EvalResult EvaluateArray2dAssignStatement(Array2dAssignStatement stmt)
    {
        var getVal = _table.Get(stmt.Name);

        if (getVal is not ArrayValue arrVal || arrVal.Cols == 0)
        {
            return new EvalFailure([
                new Diagnostic(stmt.Location?.Line ?? 0, stmt.Location?.Col ?? 0,
                    $"Identifier: {stmt.Name} is not a 2D array.", DiagnosticSeverity.Error)
            ]);
        }

        var rowResult = EvaluateExpression(stmt.RowIndex);
        if (rowResult is not EvalSuccess rowSuccess || rowSuccess.Value is not IntValue rowIv)
        {
            return new EvalFailure([
                new Diagnostic(stmt.Location?.Line ?? 0, stmt.Location?.Col ?? 0,
                    "Row index must be an integer.", DiagnosticSeverity.Error)
            ]);
        }

        var colResult = EvaluateExpression(stmt.ColIndex);
        if (colResult is not EvalSuccess colSuccess || colSuccess.Value is not IntValue colIv)
        {
            return new EvalFailure([
                new Diagnostic(stmt.Location?.Line ?? 0, stmt.Location?.Col ?? 0,
                    "Column index must be an integer.", DiagnosticSeverity.Error)
            ]);
        }

        var row = rowIv.V;
        var col = colIv.V;
        var rows = arrVal.Items.Length / arrVal.Cols;

        if (row < 0 || row >= rows)
        {
            return new EvalFailure([
                new Diagnostic(stmt.Location?.Line ?? 0, stmt.Location?.Col ?? 0,
                    $"Row index {row} is outside of the range 0-{rows - 1} for array: {stmt.Name}.",
                    DiagnosticSeverity.Error)
            ]);
        }

        if (col < 0 || col >= arrVal.Cols)
        {
            return new EvalFailure([
                new Diagnostic(stmt.Location?.Line ?? 0, stmt.Location?.Col ?? 0,
                    $"Column index {col} is outside of the range 0-{arrVal.Cols - 1} for array: {stmt.Name}.",
                    DiagnosticSeverity.Error)
            ]);
        }

        var valueResult = EvaluateExpression(stmt.Value);
        if (valueResult is not EvalSuccess valueSuccess)
            return valueResult;

        var value = valueSuccess.Value;
        var targetType = arrVal.ElementTypeName.ToUpperInvariant();
        var flatIdx = row * arrVal.Cols + col;

        if ((targetType == "INTEGER" && value is IntValue) ||
            (targetType == "FLOAT" && value is FloatValue) ||
            (targetType == "STRING" && value is StringValue) ||
            (targetType == "BOOLEAN" && value is BoolValue))
        {
            arrVal.Items[flatIdx] = value;
            _table.Set(stmt.Name, arrVal);
            return new EvalSuccess(new VoidValue());
        }

        return new EvalFailure([
            new Diagnostic(stmt.Location?.Line ?? 0, stmt.Location?.Col ?? 0,
                $"Supplied value is not a {targetType} as defined for the array: {stmt.Name}.",
                DiagnosticSeverity.Error)
        ]);
    }

    private EvalResult EvaluateArray2dAccessExpression(Array2dAccessExpression expr)
    {
        var getVal = _table.Get(expr.Name);

        if (getVal is not ArrayValue arrVal || arrVal.Cols == 0)
        {
            return new EvalFailure([
                new Diagnostic(expr.Location?.Line ?? 0, expr.Location?.Col ?? 0,
                    $"Identifier: {expr.Name} is not a 2D array.", DiagnosticSeverity.Error)
            ]);
        }

        var rowResult = EvaluateExpression(expr.RowIndex);
        if (rowResult is not EvalSuccess rowSuccess || rowSuccess.Value is not IntValue rowIv)
        {
            return new EvalFailure([
                new Diagnostic(expr.Location?.Line ?? 0, expr.Location?.Col ?? 0,
                    "Row index must be an integer.", DiagnosticSeverity.Error)
            ]);
        }

        var colResult = EvaluateExpression(expr.ColIndex);
        if (colResult is not EvalSuccess colSuccess || colSuccess.Value is not IntValue colIv)
        {
            return new EvalFailure([
                new Diagnostic(expr.Location?.Line ?? 0, expr.Location?.Col ?? 0,
                    "Column index must be an integer.", DiagnosticSeverity.Error)
            ]);
        }

        var row = rowIv.V;
        var col = colIv.V;
        var rows = arrVal.Items.Length / arrVal.Cols;

        if (row < 0 || row >= rows)
        {
            return new EvalFailure([
                new Diagnostic(expr.Location?.Line ?? 0, expr.Location?.Col ?? 0,
                    $"Row index {row} is outside of the range 0-{rows - 1} for array: {expr.Name}.",
                    DiagnosticSeverity.Error)
            ]);
        }

        if (col < 0 || col >= arrVal.Cols)
        {
            return new EvalFailure([
                new Diagnostic(expr.Location?.Line ?? 0, expr.Location?.Col ?? 0,
                    $"Column index {col} is outside of the range 0-{arrVal.Cols - 1} for array: {expr.Name}.",
                    DiagnosticSeverity.Error)
            ]);
        }

        var flatIdx = row * arrVal.Cols + col;

        return arrVal.ElementTypeName.ToUpperInvariant() switch
        {
            "INTEGER" => new EvalSuccess((IntValue)arrVal.Items[flatIdx]),
            "FLOAT" => new EvalSuccess((FloatValue)arrVal.Items[flatIdx]),
            "BOOLEAN" => new EvalSuccess((BoolValue)arrVal.Items[flatIdx]),
            "STRING" => new EvalSuccess((StringValue)arrVal.Items[flatIdx]),
            _ => new EvalFailure([
                new Diagnostic(expr.Location?.Line ?? 0, expr.Location?.Col ?? 0,
                    $"Unknown type name: {arrVal.TypeName}", DiagnosticSeverity.Error)
            ])
        };
    }

    private EvalResult EvaluateUnaryExpression(UnaryExpression expr)
    {
        var evalResult = EvaluateExpression(expr.Operand); if (evalResult is EvalFailure) return evalResult;

        var resValue = ((EvalSuccess)evalResult).Value;

        if (expr.Operator.Type is TokenType.Minus && resValue is IntValue iv)
        {
            return new EvalSuccess(new IntValue(-iv.V));
        }
        else if (expr.Operator.Type is TokenType.Minus && resValue is FloatValue fv)
        {
            return new EvalSuccess(new FloatValue(-fv.V));
        }
        else if (expr.Operator.Type is TokenType.Not && resValue is BoolValue bv)
        {
            return new EvalSuccess(new BoolValue(!bv.V));
        }

        return new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Invalid Operator: {expr.Operator} for type {expr.Operand.GetType()}.",
                            DiagnosticSeverity.Error
                        )
                    ]
                );
    }

    private EvalResult EvaluateExpression(Expression expr)
    {
        return expr switch
        {
            StringLiteralExpression sl => new EvalSuccess(new StringValue(sl.Value)),
            IntLiteralExpression il => new EvalSuccess(new IntValue(il.Value)),
            FloatLiteralExpression fl => new EvalSuccess(new FloatValue(fl.Value)),
            BinaryExpression be => EvaluateBinaryExpression(be),
            CallExpression ce => EvaluateCallExpression(ce),
            ArrayAccessExpression aae => EvaluateArrayAccessExpression(aae),
            Array2dAccessExpression aae2 => EvaluateArray2dAccessExpression(aae2),
            BoolLiteralExpression ble => new EvalSuccess(new BoolValue(ble.Value)),
            UnaryExpression ue => EvaluateUnaryExpression(ue),
            IdentifierExpression id when _table.Get(id.Name) is { } val => new EvalSuccess(val),
            IdentifierExpression id
                => new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Unknown Identifier: {id.Name}",
                            DiagnosticSeverity.Error
                        )
                    ]
                ),
            _
                => new EvalFailure(
                    [
                        new Diagnostic(
                            expr.Location?.Line ?? 0,
                            expr.Location?.Col ?? 0,
                            $"Unknown Expression: {expr.GetType()}",
                            DiagnosticSeverity.Error
                        )
                    ]
                )
        };
    }
}
