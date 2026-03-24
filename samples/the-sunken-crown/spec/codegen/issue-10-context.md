# The Sunken Crown — Issue 10 Generation Context
## The Gate

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

Exists from Issues 1–9. The complete game exists minus the Gate sequence,
end screen, and play again loop. Room 12 renders but accepts no meaningful
input. `endState` global exists. All other systems are fully working.

---

## Scope — What This Issue Adds

**Constants:**
```
CONST DOOR_LEFT = 1
CONST DOOR_RIGHT = 2
```

**Global variables:**
```
LET gateCorrectDoor = 0    ' assigned at startup: 1=LEFT, 2=RIGHT
```

**SUBs:**
```
SUB InitGate()
SUB GateSequence()
SUB PrintEndScreen()
SUB ResetGame()
```

**Updated:**
- `EnterRoom` — detect room 12: route to `GateSequence()`
- Top-level code — wrap entire game in play again outer WHILE loop
- Top-level code — call `ResetGame()` at start of each run
- `InitGate()` called at startup (in ResetGame)
- Command loop — add STATS and HELP commands
- Command loop — LEFT and RIGHT in room 12 route through GateSequence
  (handled inside GateSequence's own inner loop, not the main command loop)

**`InitGate` logic:**
`SET GLOBAL gateCorrectDoor = INT(RND() * 2) + 1`

**`GateSequence` logic:**
Print Gate description (first-visit or revisit — visited[12] flag).
Set `LET visited[12] = 1`.
Inner WHILE loop:
  INPUT cmd$. UPPER$.
  LEFT or RIGHT: resolve against gateCorrectDoor.
    Correct: print win sequence, SET GLOBAL endState = 1, exit loop.
    Wrong: print death sequence, SET GLOBAL endState = 4,
           SET GLOBAL stamina = 0, exit loop.
  Other: print unknown command response.
After loop: CALL PrintEndScreen().
SET GLOBAL gameOver = 1.

**Win sequence text contains double-quoted speech:**
`"You survived."` and `"Go."` — use `CHR$(34)` for the double-quote characters.
Example: `PRINT CHR$(34) & "You survived." & CHR$(34)`

**Win sequence paging:**
The win sequence is 29 lines and exceeds the 20 content row limit. Insert
`CALL Pause()` after printing *"Then he steps forward. Alone. The guards don't move."*
and before *"You survived."* — this is the `[PAUSE]` marker in the content asset file.
This is the only place in the game that requires a pause.

**`PrintEndScreen` logic:**
Print header. SELECT CASE on endState for the outcome message.
Count gold bags in inventory: loop `FOR i = 0 TO MAX_INVENTORY - 1`, count slots where `inventory[i] = ITEM_GOLD_BAG`. Use the constant, not a hardcoded 3.
Print run stats: startSkill, minStamina, luckTestCount, turns.
Print separator. Play again prompt: INPUT again$. UPPER$.
IF again$ = "YES" → leave gameOver=1 but signal play again (see ResetGame note).

**Play again loop structure:**
```
LET playAgain = 1
WHILE playAgain = 1
    CALL ResetGame()
    ' game runs here — inner WHILE gameOver = 0
    ' ...
    ' after gameOver = 1, PrintEndScreen asks play again
    ' if YES: playAgain stays 1, loop repeats
    ' if NO: SET GLOBAL playAgain = 0
WEND
```

`PrintEndScreen` must set `playAgain` based on the player's response.
`playAgain` is a global — writes from inside `PrintEndScreen` use SET GLOBAL.

**`ResetGame` logic:**
Reset all globals to their initial values:
- All LET-declared scalars reset to 0 or starting values
- All DIM-declared arrays reset via FOR loops
- Call InitExits(), InitMonsters(), ShuffleLoot(), InitRiddle(), InitGate()
- Do NOT call OpeningSequence or RollStartingStats — these are called separately
  after ResetGame returns. ResetGame only resets state and re-initialises systems.
  The play again loop calls: ResetGame() then OpeningSequence() then EnterRoom().

**Full reset list (every global that must be reset):**
skill, startSkill, stamina, startStamina, luck, minStamina, luckTestCount,
currentRoom (=1), gameOver (=0), turns, invCount, overburdened, poisoned,
medalTaken, armouryLocked (=1), zombieSpawned, zombieAlive, zombieRoom,
riddleIndex, riddleCorrectDoor, riddleSolved, goldBags, secondFight,
crownAvailable, terrorActive, endState, luckDrainZeroFired, playAgain (DO NOT reset),
gateCorrectDoor (reset via InitGate).

Array resets:
- visited[0..11] = 0
- searched[0..11] = 0
- monsterAlive[0..11] = 0 (then InitMonsters sets correct rooms to 1)
- inventory[0..3] = 0
- slotContents[0..5] = 0 (then ShuffleLoot sets values)
- slotTaken[0..5] = 0
- roomItems[0..11][0..11] = 0
- roomItemCount[0..11] = 0
- exitHidden — reset to initial values (call InitExits which re-sets all)

**STATS command:**
Print the header (calls PrintHeader). Zero turns cost.

**HELP command:**
Print list of available commands. Zero turns cost.

---

## Exit State

Complete game. All end states work. Play again resets cleanly. Multiple
consecutive runs produce fresh games with different attributes, loot,
riddles, and gate assignments.

---

## SET GLOBAL Audit

`GateSequence` writes to: `visited[12]` (LET — array element), `endState` (SET GLOBAL),
`stamina` (SET GLOBAL), `gameOver` (SET GLOBAL).

`PrintEndScreen` writes to: `playAgain` (SET GLOBAL).

`InitGate` writes to: `gateCorrectDoor` (SET GLOBAL).

`ResetGame` writes to: every scalar global listed above (all SET GLOBAL).
Array element resets use LET inside FOR loops.

**Note on ResetGame:**
`ResetGame` is a SUB that sets a large number of globals. Every single one
requires SET GLOBAL. This is the most SET GLOBAL-intensive SUB in the game.
Be methodical — work through the complete reset list above in order.

---

## Text Strings Required

**Gate first-visit:** Deliverable 1, Room [12], First visit
**Win sequence — full text:** Deliverable 7, Gate Win Text — Correct Door
**Death sequence — wrong door:** Deliverable 7, Gate Death Text — Wrong Door
**Win end screen:** Deliverable 7, Win End Screen (template with X placeholders)
**All endState messages:** Deliverable 7 for endState=1; Deliverable 6 crown
end screen for endState=2; remaining states use short inline strings.

**endState messages:**
- 1: `"  You survived The Sunken Crown."` + gold count
- 2: `"  You are the Bound King now."` (from Deliverable 6 Crown End Screen)
- 3: `"  The ceiling lowered. The room did not care."`
- 4: `"  The torch went out. The dungeon kept you."`
- 5: `"  Your STAMINA reached zero."`

States 3, 4, 5 text confirmed and locked in content asset file Deliverable 8,
End State Messages section. Pull from there.

**Double-quote characters in win text:**
`"You survived."` → `CHR$(34) & "You survived." & CHR$(34)`
`"Go."` → `CHR$(34) & "Go." & CHR$(34)`

---

## Known Gotchas

**Gate correct door — end screen gold count:**
The end screen must count `ITEM_GOLD_BAG` items in inventory, not read
`goldBags`. Loop `FOR i = 0 TO MAX_INVENTORY - 1`, count entries equal to ITEM_GOLD_BAG. Use the constant.
This is Decision 10 — the two values may differ if bags were dropped after
taking them.

**ResetGame and array bounds:**
roomItems reset via nested FOR loop:
```
FOR r = 0 TO 11
    FOR i = 0 TO 11
        LET roomItems[r][i] = 0
    NEXT i
    LET roomItemCount[r] = 0
NEXT r
```

**Play again loop placement:**
The outer `WHILE playAgain = 1` loop wraps everything — ResetGame,
OpeningSequence, the main game WHILE loop, and PrintEndScreen. The
structure is:
```
LET playAgain = 1
WHILE playAgain = 1
    CALL ResetGame()
    CALL OpeningSequence()         ' rolls stats, narrative
    CALL EnterRoom(currentRoom)    ' Entry Hall
    WHILE gameOver = 0
        ' main game loop
    WEND
    ' gameOver = 1 here — PrintEndScreen called from GateSequence
    ' OR called here if death happened outside GateSequence
WEND
```
Death outside GateSequence (stamina=0, crown, riddle) sets gameOver=1
in the main loop. PrintEndScreen must be called in those cases too.
The cleanest approach: deaths outside GateSequence (stamina=0, crown, riddle)
set gameOver=1 in the main loop. The main loop checks gameOver=1 after each
action and calls PrintEndScreen if gameOver was just set. GateSequence calls
PrintEndScreen itself before setting gameOver=1. No endScreenShown flag needed
— structure the control flow so PrintEndScreen is called exactly once per run.

---

## Testing Checklist

- [ ] Navigate to room 12 — Gate description renders
- [ ] LEFT accepted as command
- [ ] RIGHT accepted as command
- [ ] Other input — unknown command response, loop continues
- [ ] Correct door — win sequence prints with correct quoted speech
- [ ] Wrong door — death sequence prints
- [ ] End screen shows correct endState message for all 5 states
- [ ] End screen shows gold bags from inventory (not goldBags counter)
- [ ] End screen shows startSkill (not current skill)
- [ ] End screen shows minStamina
- [ ] End screen shows luckTestCount
- [ ] End screen shows turns
- [ ] Play again YES — full reset, new run begins
- [ ] Play again NO — program exits cleanly
- [ ] New run has different attributes, loot, riddle, gate
- [ ] Entry Hall shows first-visit text on new run
- [ ] STATS command prints header
- [ ] HELP command lists commands
- [ ] Final acceptance test plan executed in full

---

## Issue File Reference

`listing/issue-10-the-gate.md`

This is the final issue. After sign-off, execute the final acceptance test
plan (`final-acceptance-test.md`) across a minimum of three full playthroughs.
