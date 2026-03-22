# Call Frames and the Call Stack

> Read this before starting Phase 7.

---

## What the problem is — why a flat symbol table breaks

After Phase 3, variables live in a `SymbolTable` — a dictionary of name-to-value pairs. That works for global variables. It breaks the moment you add functions.

Consider:

```
FUNCTION Add(a AS INTEGER, b AS INTEGER) AS INTEGER
    RETURN a + b
END FUNCTION

LET a = 99
PRINT Add(3, 4)
PRINT a
```

If `a` and `b` are written directly into the global symbol table, the call to `Add(3, 4)` overwrites the global `a`. After the call, `PRINT a` outputs `3`, not `99`.

The fix is a separate scope for each function call — a scope that ceases to exist when the call returns, and that does not interfere with variables in the caller.

---

## What a call frame is

A call frame is the data that exists for the duration of one function call:
- Which function is executing
- The local variables and parameters for this call
- Where to return to when the call finishes

In SharpBASIC, a call frame is represented by a `SymbolTable` instance created at call time, with the caller's `SymbolTable` as its parent.

```csharp
// When calling a function:
var localScope = new SymbolTable(parent: callerScope);
// Define parameters in localScope
localScope.Define("a", new IntValue(3));
localScope.Define("b", new IntValue(4));
// Execute the function body against localScope
var childEvaluator = new Evaluator(functionBody, localScope, _subs, _functions);
```

When the call returns, `localScope` goes out of scope. The caller's scope is unchanged.

---

## The call stack as a concrete data structure

The call stack is the chain of active call frames — one frame per function call that has not yet returned.

In SharpBASIC's tree-walking interpreter, the call stack is implicit: the C# call stack tracks active recursive calls to `Evaluate`, and each call frame corresponds to one stack frame of the C# runtime.

This is one of the meaningful differences between a tree-walking interpreter and a bytecode VM. In a VM, the call stack is explicit — an array of `CallFrame` structs managed by the VM itself. In a tree-walker, C# does the work for you.

What this means in practice: the recursion depth of your SharpBASIC programs is bounded by the C# stack depth. For the interpreter's purposes, this is acceptable. It is one of the trade-offs accepted in `decisions/architecture-decisions.md`.

---

## How local variables work — scoped to a frame, not global names

Parameters become the first local variables of a call frame. Any `LET` statement inside the function writes to the local `SymbolTable`, not to the caller's.

```csharp
// SymbolTable.Set always writes to the current scope
public void Set(string name, IValue value)
{
    _values[name] = value; // always local
}

// SymbolTable.Get walks up the parent chain
public IValue? Get(string name)
{
    if (_values.TryGetValue(name, out var value)) return value;
    return Parent?.Get(name);
}
```

This means:
- A function can **read** a variable from the caller's scope (the parent chain allows it).
- A function **cannot write to** a variable in the caller's scope — `Set` always writes locally.

This is the expected and correct behaviour. See `docs/language-spec-v1.md` §4.2 for the formal statement.

---

## What a stack trace actually is

A stack trace is a printout of every active call frame, from the innermost (most recent) outward. Each line is: the name of the function and where in it execution is currently paused.

When you see a stack trace from a C# exception, it is the C# runtime printing its own call stack. When SharpBASIC throws a `RuntimeError` inside a deeply recursive call, the C# stack trace includes your evaluator frames — which is one reason the tree-walking approach makes Phase 9 diagnostics simpler to implement.

---

## Scope and the parent chain — why SymbolTable needs a Parent pointer

The parent chain was introduced in Phase 3. By Phase 7, it is load-bearing.

When a function is called:
1. Create a new `SymbolTable` with `parent = callerScope`.
2. Define parameters in the new scope.
3. Execute the function body in the new scope.
4. When `RETURN` is encountered, unwind back to the caller.

Without the parent pointer, the function body cannot read global variables or caller-scope variables. With it, the function body has full read access to everything in any enclosing scope.

The asymmetry — read access up the chain, write access only to the current scope — is deliberate. It prevents functions from mutating caller state, which is almost always the wrong behaviour.

---

## Common mistakes

**Passing fresh, empty declaration dictionaries to the child evaluator.** This is the exact bug that caused Phase 7's most serious failure (see `theory/pitfalls.md` — Bug 6). When spawning a child evaluator for a function call, pass the parent's `_subs` and `_functions` dictionaries by reference. A fresh empty dictionary means recursive calls — and any call to a function defined at the top level — will fail silently.

```csharp
// Wrong — child evaluator cannot see any declared functions or subs
new Evaluator(body, localScope)

// Correct — child shares the same declaration tables
new Evaluator(body, localScope, _subs, _functions)
```

**Calling Set on the wrong scope.** If `Set` walks the parent chain and writes to the first scope that contains the name (like Python's `nonlocal`), functions mutate caller variables. SharpBASIC's `Set` always writes to the current scope — confirm this explicitly in your implementation.

**Not hoisting declarations before execution.** All `SUB` and `FUNCTION` declarations must be registered before any top-level statement executes. Otherwise, a function declared after the call that invokes it will not be found. Implement `HoistDeclarations()` as a separate pass over the program before `Evaluate` runs.

**Parent chain depth and scope leaks.** If the parent chain is set incorrectly — for example, pointing to a scope that was already discarded — reads from that scope will either fail silently or return stale values. Test recursive functions (Fibonacci is the canonical case) and verify that each recursive call sees its own parameters, not those of an earlier call.
