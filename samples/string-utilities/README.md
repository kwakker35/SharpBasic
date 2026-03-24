# string-utilities

A tour of every built-in string function in SharpBASIC.

```
sharpbasic string-utilities.sbx
```

## What it demonstrates

| Builtin | What it does |
|---------|-------------|
| `TRIM$` | Remove leading and trailing whitespace |
| `UPPER$` | Convert to uppercase |
| `LOWER$` | Convert to lowercase |
| `LEN` | Character count |
| `LEFT$(s, n)` | First `n` characters |
| `RIGHT$(s, n)` | Last `n` characters |
| `MID$(s, start, len)` | Substring from `start` with length `len` |
| `CHR$(n)` | Returns the character for ASCII code `n` |

## Pedagogical note

String functions compose naturally — `UPPER$(TRIM$(s))` trims first and then uppercases. The title-casing example at the bottom chains `UPPER$`, `LEFT$`, `LOWER$`, `RIGHT$`, and `LEN` together to capitalise the first letter of a word without any dedicated `TITLECASE` function. This is a useful pattern: most string manipulations are achievable by composing simpler primitives.

Note that `MID$` is 1-indexed (position 1 is the first character) — the same convention used by classic BASIC and VBA.
