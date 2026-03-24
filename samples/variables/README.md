# variables

Introduces the four built-in types in SharpBASIC: `INTEGER`, `FLOAT`, `STRING`, and `BOOLEAN`.

```
sharpbasic variables.sbx
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| Typed variable declaration | `LET name AS STRING = "..."` |
| String concatenation | `"Language: " & name` |
| `STR$` conversion | `STR$(version)` |
| `TYPENAME` builtin | Reports the runtime type of any value |
| `CONST` | `CONST PI = 3.14159` — named constant, scoped like a variable |
| `SET GLOBAL` | `SET GLOBAL counter = counter + 1` — writes to an enclosing scope |

## Pedagogical note

SharpBASIC is **statically typed at declaration** — every variable has a type from the moment it is created. Compare this to classic BASIC, where type was inferred from a `$` suffix. The `TYPENAME` builtin is useful for exploring and debugging — it's the runtime equivalent of asking "what is this?".
