# hangman

A fully playable Hangman game. The interpreter picks a random word from a built-in word list; the player guesses one letter at a time with 6 lives.

```
sharpbasic hangman.bas
```

## What it demonstrates

| Feature | Example |
|---------|---------|
| `DIM` + array of strings | Word list stored in `words[8]` |
| `DIM` + array of booleans | `guessed[26]` tracks which letters have been tried |
| `FUNCTION` returning a value | `BuildDisplay` builds the `_ _ _ _` view |
| `FUNCTION` with complex logic | `CountHidden` counts remaining letters |
| `SUB` for side effects | `DrawHangman` renders the ASCII scaffold |
| `CALL` | Invoking SUBs from the main loop |
| `UPPER$`, `LEFT$`, `TRIM$`, `MID$` | Normalising and parsing user input |
| `WHILE`/`WEND` game loop | Runs until solved or out of lives |

## How it works

The game uses a 26-element boolean array (`guessed[0]`=A, `guessed[1]`=B, …) to track which letters have been tried. When displaying the word, `BuildDisplay` iterates each character and checks this array — if the letter has been guessed, it's shown; otherwise an underscore appears.

Because SharpBASIC doesn't have a built-in `ASC()` function yet, letter-to-index conversion is done by scanning the string `"ABCDEFGHIJKLMNOPQRSTUVWXYZ"` with `MID$`. It's verbose but demonstrates how to work around missing primitives with what the language provides.

## Pedagogical note

Hangman is a genuine test of language maturity. It requires:
- **Data structures** (arrays for word list and guess tracking)
- **Decomposition** (SUBs and FUNCTIONs separate concerns cleanly)
- **String manipulation** (parsing input, building display strings)
- **State management** (wrong count, solved flag, guessed array)

If all of these work together cleanly, your interpreter is doing its job.
