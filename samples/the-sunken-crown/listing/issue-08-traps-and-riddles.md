# The Sunken Crown
## Issue 8 — Traps and Riddles

*Not everything in the dungeon fights you.*

---

## The Story So Far

Issues 1 through 7 built a fully navigable dungeon with combat, inventory, atmosphere, and a wandering zombie. Every room has a description. Most things that move can kill you.

Two rooms have been sitting in the map since Issue 4, waiting for their mechanics. Room 5 traps you the moment you enter. Room 8 locks the door behind you. Neither fights you. Both can end the run.

This issue wires them up.

---

## What's New This Issue

- `FUNCTION TestLuck() AS INTEGER` — tests luck, decrements LUCK regardless of outcome, returns 1 or 0
- `SUB InitRiddle()` — selects riddle and correct door at startup, fixed for the run
- `SUB PrintRiddle()` — prints the active riddle text with door labels
- `SUB RiddleRoomSequence()` — inner loop for room 8, accepts LEFT or RIGHT only
- `SUB StillChamberSequence()` — teleport mechanic for room 5, advances 3 turns, tests luck, lands player in lucky or unlucky pool room
- `riddleSolved` flag — allows pass-through on revisit and when Still Chamber lands the player in room 8
- LUCK command added to the command loop
- `EnterRoom` updated to detect rooms 5 and 8 and route to their sequences

---

## Concept — IF / ELSE Chains and INPUT

The Riddle Room's inner loop accepts only two answers. Everything else holds the player in the room:

```
WHILE riddleSolved = 0
    INPUT cmd$
    LET cmd$ = UPPER$(cmd$)
    IF cmd$ = "LEFT" THEN
        REM resolve left door
    ELSE
        IF cmd$ = "RIGHT" THEN
            REM resolve right door
        ELSE
            PRINT "  The door does not move. The room waits."
        END IF
    END IF
WEND
```

Break this down:

- `UPPER$(cmd$)` — normalises input before comparison. `left`, `Left`, and `LEFT` all become `LEFT`. Always normalise before comparing — this is the standard throughout the game.
- The outer `IF cmd$ = "LEFT"` — checks the first valid answer
- The `ELSE IF cmd$ = "RIGHT"` — checks the second valid answer
- The final `ELSE` — catches everything else, prints the holding response, loops again
- `WHILE riddleSolved = 0` — the loop continues until a valid answer resolves it. There is no escape route except LEFT or RIGHT.

Note that SharpBASIC has no `ELSEIF` keyword. Multiple conditions are handled by nesting `IF` inside `ELSE`. The indentation makes the logic readable. This nesting pattern — check the first case, else check the second, else handle the remainder — appears throughout the game wherever more than two outcomes are possible.

This is the same INPUT-UPPER$-IF pattern used since Issue 2. The code is simple. The consequence — instant and unavoidable death on a wrong answer — is not.

---

## How It Fits

`EnterRoom` gains two new detection branches at the top of the SUB. If `roomId = 5`, `StillChamberSequence` runs immediately instead of the standard room entry. If `roomId = 8`, the code checks `riddleSolved` — if already solved the room is entered normally, if not `RiddleRoomSequence` runs.

`TestLuck` is introduced here because the Still Chamber needs it. It is also available from the LUCK command added this issue — the player can invoke it deliberately in situations where the outcome is uncertain.

The riddle text and correct door are selected at startup by `InitRiddle()` — fixed for this run, different across runs. Eight riddles in the pool, two possible correct doors each time.

---

## What You'll See

Navigate to room 5 via the northeast passage from the Crossroads. The Still Chamber sequence fires — narrative, three turns pass, a luck test, and you wake somewhere else. Your LUCK has decremented. Where you wake depends on luck.

Navigate to room 8 via the south from The Pit. The door vanishes behind you. The riddle appears on the wall. Type LEFT or RIGHT. Correct — the passage south opens. Wrong — the death sequence fires.

Type the wrong answer deliberately at least once. The death text earns its place because the information was always there. The dungeon does not lie.

---

## What to Try

Add a third input response — `EXAMINE` — that reprints the riddle text without advancing the loop or costing a turn. Does it feel fairer? Does being able to re-read the riddle change how you approach it?

There is no right answer. It is a design question. The published version does not include EXAMINE — the tension of a single reading is part of the design. But understanding that as a choice rather than an oversight is the point of the exercise.

---

## The Listing

```
REM Issue 8 listing — to be added once built and tested
```
