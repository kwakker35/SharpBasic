# calculator

A simple four-function calculator that reads two numbers and an operator from the user.

```
sharpbasic calculator.bas
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| `INPUT` + `VAL` | Convert user input string to a number |
| `STR$` | Convert a number back to a string for output |
| `IF`/`ELSE`/`END IF` nesting | Decision tree across four operators |
| Division by zero guard | Nested `IF b = 0` check |
| `FLOAT` arithmetic | Handles decimal inputs correctly |

## Pedagogical note

Notice that `INPUT` always reads a `STRING` — `VAL` is the conversion bridge to a numeric type. This mirrors a real-world constraint: raw input is always text. The nested `IF` chain shows how to handle multiple branches before SharpBASIC gains `ELSEIF` (a planned Phase 11 feature). It's deliberately verbose — a good refactoring exercise once you've added `ELSEIF`.
