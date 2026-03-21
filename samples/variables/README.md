# variables

Introduces the four built-in types in SharpBASIC: `INTEGER`, `FLOAT`, `STRING`, and `BOOLEAN`.

```
sharpbasic variables.bas
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| Typed variable declaration | `LET name AS STRING = "..."` |
| String concatenation | `"Language: " & name` |
| `STR$` conversion | `STR$(version)` |
| `TYPENAME` builtin | Reports the runtime type of any value |

## Pedagogical note

SharpBASIC is **statically typed at declaration** — every variable has a type from the moment it is created. Compare this to classic BASIC, where type was inferred from a `$` suffix. The `TYPENAME` builtin is useful for exploring and debugging — it's the runtime equivalent of asking "what is this?".
