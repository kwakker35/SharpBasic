# calculator

A simple four-function calculator that reads two numbers and an operator from the user.

```
sharpbasic calculator.sbx
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| `INPUT` + `VAL` | Convert user input string to a number |
| `STR$` | Convert a number back to a string for output |
| `SELECT CASE`/`CASE`/`END SELECT` | Multi-way branch across four operators |
| Division by zero guard | Nested `IF b = 0` check inside the division `CASE` |
| `FLOAT` arithmetic | Handles decimal inputs correctly |

## Pedagogical note

Notice that `INPUT` always reads a `STRING` — `VAL` is the conversion bridge to a numeric type. This mirrors a real-world constraint: raw input is always text. `SELECT CASE` dispatches cleanly on the operator string, keeping each branch flat and readable. The division-by-zero guard sits inside the division `CASE` as a nested `IF` — the one place where a condition is genuinely nested rather than a separate branch.
