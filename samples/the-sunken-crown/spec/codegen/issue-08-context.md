# The Sunken Crown — Issue 8 Generation Context
## Traps and Riddles

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

Exists from Issues 1–7. Full navigation, combat, inventory, atmosphere,
zombie, and all fixed monsters except the Bound King are working.
Room 5 still traps the player (no special handling).
Room 8 is navigable but the riddle loop is not yet implemented.

---

## Scope — What This Issue Adds

**Global variables:**
```
LET riddleIndex = 0           ' which riddle is active (1–8)
LET riddleCorrectDoor = 0     ' correct door: 1=LEFT, 2=RIGHT
LET riddleSolved = 0          ' 0 = not solved, 1 = solved
LET luckTestCount = 0         ' number of LUCK tests performed this run
```

**FUNCTIONs:**
```
FUNCTION TestLuck() AS INTEGER
```

**SUBs:**
```
SUB InitRiddle()
SUB PrintRiddle()
SUB RiddleRoomSequence()
SUB StillChamberSequence()
```

**Updated:**
- `EnterRoom` — detect room 5: route to `StillChamberSequence()`
- `EnterRoom` — detect room 8: if riddleSolved=0 route to `RiddleRoomSequence()`,
  else enter normally with revisit description
- `InitRiddle()` called at startup
- Command loop — add LUCK command (calls `TestLuck()` and prints result)

**`TestLuck` logic:**
Roll `RollDice(2)`. If roll <= luck: result = 1 (lucky). Else: result = 0.
Decrement luck by 1 regardless (if luck > 0). Increment `luckTestCount`.
SET GLOBAL for luck and luckTestCount writes.
Returns INTEGER (1 or 0).

**`InitRiddle` logic:**
`SET GLOBAL riddleIndex = INT(RND() * 8) + 1`
`SET GLOBAL riddleCorrectDoor = INT(RND() * 2) + 1`

**`PrintRiddle` logic:**
SELECT CASE on riddleIndex. Print riddle text. Print door labels (LEFT/RIGHT)
with correct door indicated by riddleCorrectDoor.
All 8 riddle texts inline (not from content asset — riddle text is not in
the content asset file).

**Riddles (8 total):**
1. Cities but no houses → map (LEFT if correct=1, RIGHT if correct=2)
2. More you take → footsteps
3. No mouth but come alive with wind → echo
4. This man's father → my son
5. More you have the less you see → darkness
6. Not alive but grows → fire
7. Run but never walks, mouth but never talks → river
8. No wings but fly, no mouth but cry → cloud

**`RiddleRoomSequence` logic:**
Print first-visit or revisit description (based on visited[8]).
Do NOT set visited[8] inside RiddleRoomSequence. EnterRoom sets the visited
flag before routing to the sequence — it has already been set to 1 by the time
RiddleRoomSequence runs. Setting it again is harmless but misleading.
Print riddle via PrintRiddle().
Inner WHILE loop: INPUT cmd$, UPPER$, check LEFT/RIGHT.
Correct answer: set riddleSolved=1, reveal exit (LET exitHidden[15] = 0), print door opens.
Wrong answer: print death text, set stamina=0, set endState=3, exit loop.
Invalid input: print holding response, loop again.

**`StillChamberSequence` logic:**
Print first-visit or revisit description (based on visited[5]).
Set visited[5] = 1.
Call AdvanceTurns(3).
Call TestLuck(). 
If lucky (result=1): pick from lucky pool [1, 3]. Print lucky wake text.
If unlucky (result=0): pick from unlucky pool [6, 7, 8, 10].
  Note: Cistern [9] removed from lucky pool (Mage is there). Underhall [10] added to unlucky pool.
  If destination = 8 and riddleSolved = 1: replace with room 10 (pass-through).
  Else use destination as-is.
Print unlucky wake text.
SET GLOBAL currentRoom = destination.
CALL EnterRoom(currentRoom) directly before returning from StillChamberSequence.

Do NOT rely on the main loop to call EnterRoom — StillChamberSequence is called
FROM EnterRoom (via the room 5 detection), so the main loop has already called
EnterRoom for room 5. The new room entry must happen inside the sequence itself.
After EnterRoom(currentRoom) returns, StillChamberSequence returns, and the main
loop resumes normally from wherever it was.

---

## Exit State

Room 5 teleports player on entry. Room 8 presents riddle on first entry.
Wrong answer in room 8 ends the run. Correct answer opens south exit.
Riddle solved — revisiting room 8 enters normally.
LUCK command works. TestLuck() used by Still Chamber.

---

## SET GLOBAL Audit

`TestLuck` writes to `luck`, `luckTestCount` — SET GLOBAL.
`InitRiddle` writes to `riddleIndex`, `riddleCorrectDoor` — SET GLOBAL.
`RiddleRoomSequence` writes to `visited[8]` (LET — array element),
`riddleSolved` (SET GLOBAL), `exitHidden[15]` (LET — array element),
`stamina` (SET GLOBAL), `endState` (SET GLOBAL).
`StillChamberSequence` writes to `visited[5]` (LET — array element),
`currentRoom` (SET GLOBAL), `luck` via TestLuck (SET GLOBAL in TestLuck).

---

## Text Strings Required

**Still Chamber first-visit:** Deliverable 1, Room [5], First visit
**Still Chamber revisit:** Deliverable 1, Room [5], Revisit
**Riddle Room first-visit:** Deliverable 1, Room [8], First visit
**Riddle Room revisit (solved):** Deliverable 1, Room [8], Revisit — already solved
**Still Chamber lucky wake text:** Deliverable 8, Still Chamber — Lucky Wake Text
**Still Chamber unlucky wake text:** Deliverable 8, Still Chamber — Unlucky Wake Text
**Riddle Room wrong door death:** Deliverable 8, Riddle Room — Wrong Door Death Text
**Holding response (invalid riddle input):** Deliverable 8, Riddle Room — Holding Response
**LUCK command refused:** Deliverable 8, Riddle Room — LUCK Command Refused

---

## Known Gotchas

**Still Chamber and unlucky pool room 8:**
If unlucky pool selects room 8 and riddleSolved = 0, the riddle fires
when EnterRoom(8) is called after the sequence. This is correct — but it
means a player can be trapped in the riddle loop immediately after waking
from the Still Chamber. This is intentional per the design. Do not add
any special handling beyond the riddleSolved pass-through.

**EXIT SLOT 15 — Riddle Room south exit:**
This was set to `exitHidden[15] = 1` in `InitExits()`. Confirming:
Slot 15 is the Riddle Room's south exit to room 10.
Revealed by: `LET exitHidden[15] = 0` inside `RiddleRoomSequence` on correct answer.

**Still Chamber pool selection:**
Lucky pool has 2 rooms. `INT(RND() * 2)` gives 0 or 1.
`DIM luckyPool[2] AS INTEGER` — luckyPool[0]=1, luckyPool[1]=3.
Unlucky pool has 4 rooms. `INT(RND() * 4)` gives 0–3.
`DIM unluckyPool[4] AS INTEGER` — [0]=6, [1]=7, [2]=8, [3]=10.
Note: room 9 (Cistern) is NOT in the unlucky pool — the Mage being there
made it punishing enough to remove. Room 10 (Underhall/Troll) IS unlucky.

**TestLuck — LUCK command:**
When the player types LUCK from the command loop, TestLuck() is called
and the result prints a flavour line. This consumes 1 turn and decrements
LUCK. The player should understand this is consequential.

---

## Testing Checklist

- [ ] Navigate to room 5 — Still Chamber sequence fires
- [ ] 3 turns advance during Still Chamber
- [ ] LUCK decrements by 1
- [ ] Player wakes in lucky room (1 or 3) on lucky result
- [ ] Player wakes in unlucky room (6–10) on unlucky result
- [ ] If waking in room 8 and already solved, pass-through to room 10
- [ ] Room 5 revisit still fires the trap
- [ ] Navigate to room 8 — riddle appears
- [ ] Correct answer opens south exit to room 10
- [ ] Wrong answer ends run (endState=3 on end screen)
- [ ] Invalid input — holding response, loop continues
- [ ] Room 8 revisit after solving — enters normally
- [ ] LUCK command works, decrements LUCK
- [ ] luckTestCount increments each test
- [ ] Nothing from Issues 1–7 broken

---

## Issue File Reference

`listing/issue-08-traps-and-riddles.md`
