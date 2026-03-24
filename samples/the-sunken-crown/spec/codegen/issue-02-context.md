# The Sunken Crown — Issue 2 Generation Context
## The First Room

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

The `.sbx` file exists from Issue 1 and contains:

**Constants:** `FRAME_WIDTH = 80`

**Global variables:** `skill = 0`, `stamina = 0`, `luck = 0`

**SUBs:** `PrintHeader()`, `PrintSeparator()`

**Top-level code:** calls PrintHeader, prints blank line, calls PrintSeparator, prints prompt

---

## Scope — What This Issue Adds

**Global variables:**
```
LET currentRoom = 1
DIM visited[12] AS INTEGER      ' 0 = not yet visited, 1 = visited before
```

**SUBs:**
```
SUB PrintRoom(roomId AS INTEGER)
SUB EnterRoom(roomId AS INTEGER)
SUB Pause()
```

**`Pause` logic:**
```
SUB Pause()
    INPUT "  Press ENTER to continue."; p$
END SUB
```
Prints the continue prompt and waits. Does not clear the screen — the terminal
scrolls naturally. Called wherever the content asset file marks `[PAUSE]`.
Currently used in the win sequence (Issue 10). Introduced here so it is available
to all subsequent issues.

**Updated top-level code:**
Replace the existing top-level code with a command loop:
- Call `EnterRoom(1)` once before the loop
- WHILE loop reading INPUT, normalising with UPPER$
- Recognise `GO SOUTH` — print no-exit response
- All other input — print unknown command response
- `gameOver` flag controls the loop (set to 0, never set to 1 in this issue — loop runs indefinitely until Ctrl+C)

**`PrintRoom` scope:**
Room 1 only. First-visit text from Deliverable 1. Revisit text from Deliverable 1.
Selects variant based on `visited[1]` flag.
Prints exits line: `"  Exits: SOUTH"`

**`EnterRoom` scope:**
Calls `PrintHeader()`, prints location line, blank line, calls `PrintRoom(roomId)`,
blank line, calls `PrintSeparator()`. Sets `visited[roomId] = 1`.

---

## Exit State

Running the program produces the Entry Hall render inside the frame.
`Pause()` is defined and available for use in subsequent issues.
GO SOUTH prints the no-exit response. Any other command prints the unknown
command response. The loop continues until interrupted.

On second entry to room 1 (not yet possible — navigation not wired — but
the visited flag is set on first entry and the revisit text would show
if the room were re-entered).

---

## SET GLOBAL Audit

`EnterRoom` writes to `visited[roomId]`. Array element assignment uses `LET`
syntax (`LET visited[roomId] = 1`). This is correct — array element writes
use `LET`, not `SET GLOBAL`. `SET GLOBAL` is for scalar globals only.

No scalar globals are written inside SUBs in this issue.

---

## Text Strings Required

**Room 1 first-visit:** Deliverable 1, Room [1] — Entry Hall, First visit
**Room 1 revisit:** Deliverable 1, Room [1] — Entry Hall, Revisit
**No exit that way:** Deliverable 4, string 2
**Unknown command:** Deliverable 4, string 1

UI chrome (inline — not from content asset):
- `"  LOCATION: Entry Hall"` — location line
- `"  Exits: SOUTH"` — exits line
- `" > "` — prompt

---

## Known Gotchas

**Array element writes use LET, not SET GLOBAL:**
`LET visited[roomId] = 1` is correct. `SET GLOBAL visited[roomId] = 1` is wrong.
Array elements are slots within a global array — they are written with LET.
Only scalar globals (LET x = ...) need SET GLOBAL when written from inside a SUB.

**visited array is DIM'd globally:**
`DIM visited[12] AS INTEGER` must be at the top level. DIM inside a SUB is
permitted but would create a local array that disappears when the SUB returns.

**No navigation yet:**
GO SOUTH prints the no-exit response — it does not move the player. Full
navigation comes in Issue 4. The command loop only needs to handle one
recognised command (GO SOUTH) and the default.

**gameOver loop variable:**
Declare `LET gameOver = 0` at top level before the WHILE loop. The loop
condition is `WHILE gameOver = 0`. In this issue gameOver is never set to 1 —
the loop runs until interrupted. This is intentional and correct for Issue 2.

---

## Testing Checklist

- [ ] Program runs without error
- [ ] Entry Hall first-visit text appears on startup
- [ ] Header shows SKILL: 0, STAMINA: 0, LUCK: 0
- [ ] LOCATION line shows `Entry Hall`
- [ ] Exits line shows `SOUTH`
- [ ] `GO SOUTH` prints the no-exit response
- [ ] Any other input prints the unknown command response
- [ ] Loop continues after each command
- [ ] Prompt ` > ` appears after each response
- [ ] Separators appear correctly around the content
- [ ] `Pause()` SUB exists and compiles without error (call it manually to verify)

---

## Issue File Reference

`listing/issue-02-the-first-room.md`

The listing contains only new code — additions to the file created in Issue 1.
Navigation comments indicate exactly where each block is added.
