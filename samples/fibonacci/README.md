# fibonacci

Computes Fibonacci numbers recursively, demonstrating `FUNCTION` declarations and recursive calls.

```
sharpbasic fibonacci.bas
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| `FUNCTION` declaration | `FUNCTION Fibonacci(n AS INTEGER) AS INTEGER` |
| `RETURN` statement | Returns a value up the call stack |
| Recursion | `Fibonacci(n-1) + Fibonacci(n-2)` |
| Base case | `IF n <= 1 THEN RETURN n END IF` |

## Pedagogical note

The recursive Fibonacci function is the canonical test for a working call stack. Each call creates a new **stack frame** — a separate set of local variables — then returns its result to the caller. When `n = 5`, the interpreter builds a tree of 15 calls before producing the answer.

`F(10) = 55` is the Phase 7 completion milestone for this interpreter. If that prints correctly, the evaluator's call stack, frame management, and `RETURN` mechanism are all working.

This naive recursive implementation is intentionally **not optimised** — it's exponential time. For `n = 30` you'll notice it slowing. A good follow-up exercise: convert it to an iterative version using `FOR`/`NEXT` and see how the runtime changes.
