# The Sunken Crown — Issue 3 Generation Context
## The Dice and the Dark

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

Exists from Issues 1–2:

**Constants:** `FRAME_WIDTH = 80`

**Globals:** `skill = 0`, `stamina = 0`, `luck = 0`, `currentRoom = 1`,
`visited[12]`, `gameOver = 0`

**SUBs:** `PrintHeader()`, `PrintSeparator()`, `PrintRoom(roomId)`, `EnterRoom(roomId)`

**Top-level:** Opens with `EnterRoom(1)` then command loop

---

## Scope — What This Issue Adds

**Global variables:**
```
LET startSkill = 0
LET startStamina = 0
```

**FUNCTIONs:**
```
FUNCTION RollDice(n AS INTEGER) AS INTEGER
```

**SUBs:**
```
SUB RollStartingStats()
SUB OpeningSequence()
```

**Updated top-level:**
Replace `EnterRoom(1)` at the start with:
```
CALL OpeningSequence()
CALL EnterRoom(currentRoom)
```

The command loop from Issue 2 is unchanged.

**`RollDice` logic:**
Loops `n` times. Each iteration: `INT(RND() * 6) + 1` added to total.
Returns total as INTEGER. Uses `VAL(STR$(INT(RND() * 6) + 1))` is NOT needed —
`INT(RND() * 6) + 1` returns a Float. The addition `total + Float` promotes
total to Float. `RETURN total` returns a Float from a declared INTEGER function.
**This is a known issue with INT() — see gotchas below.**

**`RollStartingStats` logic:**
```
SET GLOBAL skill = RollDice(1) + 6
SET GLOBAL startSkill = skill
SET GLOBAL stamina = RollDice(2) + 12
SET GLOBAL startStamina = stamina
SET GLOBAL luck = RollDice(1) + 6
```

**`OpeningSequence` logic:**
Print narrative text from Deliverable 8. Call `Pause()` for pacing. Call RollStartingStats.
Print rolled values. Call `Pause()` for pacing. The sequence ends — no return value needed.
`Pause()` was introduced in Issue 2 and is available here.

---

## Exit State

Running the program:
1. Opening narrative text appears
2. Player presses ENTER
3. Attributes roll and display (SKILL 7–12, STAMINA 14–24, LUCK 7–12)
4. Player presses ENTER
5. Entry Hall renders with real stats in header
6. Command loop runs as before

Three consecutive runs produce different attribute values.

---

## SET GLOBAL Audit

`RollStartingStats` writes to five globals: `skill`, `startSkill`, `stamina`,
`startStamina`, `luck`. All five must use `SET GLOBAL`.

`OpeningSequence` does not write to globals directly — it calls
`RollStartingStats` which does the writing.

---

## Text Strings Required

**Opening narrative:** Deliverable 8, Opening Sequence — Narrative Text

UI chrome (inline):
- `"  Rolling your attributes..."` 
- `"  SKILL:    "` & skill
- `"  STAMINA:  "` & stamina
- `"  LUCK:     "` & luck
- `"  Press ENTER to continue."` — INPUT prompt

---

## Known Gotchas

**INT() returns Float when given Float:**
`INT(RND() * 6)` returns a Float (e.g. `3.0`), not an Integer.
Adding 1 gives a Float. Accumulating in `total` promotes `total` to Float.
`RETURN total` from a declared `AS INTEGER` function — the spec says return
type is advisory, not enforced. The value `3.0` will be returned and used
in arithmetic correctly. However, if strict Integer is needed:
`VAL(STR$(INT(RND() * 6) + 1))` converts back to Integer via string round-trip.
For dice rolls, the Float values work correctly in practice. Document the
choice made in a comment.

**RollDice parameter is INTEGER but receives 1 or 2:**
Both are integer literals. No issue.

**INPUT pacing — variable name:**
`INPUT "Press ENTER to continue."; pause$` — the variable `pause$` is declared
implicitly by INPUT as a STRING. It is never used. This is correct and intentional.
The `$` suffix is not required in SharpBASIC — `pause` works equally well.

**startStamina must be set before stamina could be changed:**
`RollStartingStats` sets `startStamina = stamina` immediately after rolling stamina.
This value is the healing cap — it must never change after this point. Verify
it is set correctly and not overwritten anywhere.

---

## Testing Checklist

- [ ] Opening narrative text appears correctly
- [ ] First ENTER advances to attribute display
- [ ] SKILL rolls between 7 and 12 on every run
- [ ] STAMINA rolls between 14 and 24 on every run
- [ ] LUCK rolls between 7 and 12 on every run
- [ ] Three consecutive runs produce different values
- [ ] Header shows rolled stats after opening sequence
- [ ] Entry Hall renders with correct first-visit text
- [ ] Command loop works as before (GO SOUTH, unknown command)
- [ ] startStamina equals stamina at game start (verify via STATS or debug print)
- [ ] startSkill equals skill at game start

---

## Issue File Reference

`listing/issue-03-the-dice-and-the-dark.md`

The listing contains only new code — `RollDice`, `RollStartingStats`,
`OpeningSequence`, two new global declarations, and the updated top-level
call sequence.
