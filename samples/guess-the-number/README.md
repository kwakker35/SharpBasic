# guess-the-number

A classic number-guessing game. The interpreter picks a random number between 1 and 100; the player keeps guessing until they find it.

```
sharpbasic guess-the-number.bas
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| `WHILE`/`WEND` game loop | Keeps running until `guess = secret` |
| `RND()` + `INT` | Generate a random integer in a range |
| Attempt counter | `LET attempts = attempts + 1` |
| `<>` (not-equal) | Loop condition `guess <> secret` |

## Pedagogical note

`WHILE`/`WEND` is the right loop here — we don't know how many guesses the player will need, so `FOR`/`NEXT` (which requires a fixed count) doesn't fit. The condition `guess <> secret` is checked **before** each guess, which means declaring `guess = 0` as the initial value is necessary to enter the loop at all. Consider what would happen if you forgot that initialisation.

`RND()` returns a `FLOAT` in [0, 1). `INT(RND() * 100) + 1` maps that to an integer in [1, 100]. This is the standard random-integer-in-range idiom in BASIC.
