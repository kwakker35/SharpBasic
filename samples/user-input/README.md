# user-input

Reads a name from the user and prints a personalised greeting.

```
sharpbasic user-input.bas
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| `INPUT` statement | `INPUT name` — reads a line from stdin |
| String concatenation | `"Hello, " & name & "!"` |

## Pedagogical note

`INPUT` is the bridge between your program and the person running it. Notice that the variable `name` has no explicit type annotation — SharpBASIC infers `STRING` from the value read. Running this program shows the full round-trip: output with `PRINT`, input with `INPUT`, then output again.
