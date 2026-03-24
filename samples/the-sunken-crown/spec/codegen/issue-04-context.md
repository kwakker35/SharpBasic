# The Sunken Crown — Issue 4 Generation Context
## Moving Through Stone

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

Exists from Issues 1–3:

**Constants:** `FRAME_WIDTH = 80`

**Globals:** `skill`, `startSkill`, `stamina`, `startStamina`, `luck`,
`currentRoom = 1`, `visited[12]`, `gameOver = 0`

**FUNCTIONs:** `RollDice(n)`

**SUBs:** `PrintHeader()`, `PrintSeparator()`, `PrintRoom(roomId)`,
`EnterRoom(roomId)`, `RollStartingStats()`, `OpeningSequence()`

**Top-level:** `OpeningSequence()` then `EnterRoom(currentRoom)` then command loop

---

## Scope — What This Issue Adds

**Constants:**
```
CONST DIR_N = 1
CONST DIR_S = 2
CONST DIR_E = 3
CONST DIR_W = 4
CONST DIR_NE = 5
CONST MAX_EXITS = 30
CONST MAX_ROOMS = 12
```

**Global arrays:**
```
DIM roomExitStart[12] AS INTEGER  ' index of first exit slot for each room
DIM roomExitCount[12] AS INTEGER  ' number of exits for each room
DIM exitDir[30] AS INTEGER        ' direction code per exit slot
DIM exitDest[30] AS INTEGER       ' destination room per exit slot
DIM exitHidden[30] AS INTEGER     ' 0 = visible, 1 = hidden until searched
DIM searched[12] AS INTEGER       ' 0 = unsearched, 1 = exhausted
```

**FUNCTIONs:**
```
FUNCTION DirName(dir AS INTEGER) AS STRING
FUNCTION RoomName(roomId AS INTEGER) AS STRING
```

**SUBs:**
```
SUB InitExits()
SUB PrintExits(roomId AS INTEGER)
SUB HandleGo(dir AS INTEGER)
```

**Updated SUBs:**
- `PrintRoom` — add all 12 room descriptions (first-visit and revisit variants)
- `EnterRoom` — use `RoomName(roomId)` for location line; call `PrintExits(roomId)`
  inside `PrintRoom` rather than hardcoded exits string
- Command loop — handle all 5 directions, call `HandleGo` then `EnterRoom`

**`InitExits` — full exit map:**
See architecture document ADR-001 for exit slot assignments.
Room 6's south exit to room 9 is assigned `exitHidden[slot] = 1`.
Riddle Room's south exit to room 10 is assigned `exitHidden[slot] = 1`.
All other exits are `exitHidden[slot] = 0`.

**`HandleGo` logic:**
Walk `roomExitStart[currentRoom]` to `roomExitStart[currentRoom] + roomExitCount[currentRoom] - 1`.
For each slot, if `exitDir[slot] = dir AND exitHidden[slot] = 0`, set
`SET GLOBAL currentRoom = exitDest[slot]` and return.
If no match found, print no-exit response.

**`InitExits` must be called at startup** before `OpeningSequence`.

---

## Exit State

Full dungeon navigable. All 12 rooms render with correct first-visit and
revisit descriptions. Exits display correctly per room. Room 6's south exit
does not appear. Riddle Room's south exit does not appear.
Room 5 (Still Chamber) has no exits — player is trapped if they navigate there
in this issue. This is expected and correct for Issue 4.

---

## SET GLOBAL Audit

`HandleGo` writes to `currentRoom`. Must use `SET GLOBAL currentRoom = exitDest[slot]`.

`InitExits` writes to all exit arrays — these are array element writes (`LET exitDir[i] = ...`),
not scalar global writes. LET is correct for array elements.

---

## Full Exit Map — Issue 4

```
Room 1 — Entry Hall: S→2
  slot 1: dir=DIR_S, dest=2, hidden=0

Room 2 — Guardroom: N→1, E→3, S→4
  slot 2: dir=DIR_N, dest=1, hidden=0
  slot 3: dir=DIR_E, dest=3, hidden=0
  slot 4: dir=DIR_S, dest=4, hidden=0

Room 3 — Armoury: W→2
  slot 5: dir=DIR_W, dest=2, hidden=0

Room 4 — Crossroads: N→2, W→6, E→7, NE→5
  slot 6: dir=DIR_N, dest=2, hidden=0
  slot 7: dir=DIR_W, dest=6, hidden=0
  slot 8: dir=DIR_E, dest=7, hidden=0
  slot 9: dir=DIR_NE, dest=5, hidden=0

Room 5 — Still Chamber: no exits
  roomExitStart[5] = 10, roomExitCount[5] = 0

Room 6 — Collapsed Passage: E→4, S→9 (hidden)
  slot 10: dir=DIR_E, dest=4, hidden=0
  slot 11: dir=DIR_S, dest=9, hidden=1

Room 7 — The Pit: W→4, S→8
  slot 12: dir=DIR_W, dest=4, hidden=0
  slot 13: dir=DIR_S, dest=8, hidden=0

Room 8 — Riddle Room: N→7, S→10 (hidden until solved)
  slot 14: dir=DIR_N, dest=7, hidden=0
  slot 15: dir=DIR_S, dest=10, hidden=1

Room 9 — Cistern: N→6, S→10
  slot 16: dir=DIR_N, dest=6, hidden=0
  slot 17: dir=DIR_S, dest=10, hidden=0

Room 10 — Underhall: N→4 (via routes), S→11
  slot 18: dir=DIR_N, dest=4, hidden=0
  slot 19: dir=DIR_S, dest=11, hidden=0

Room 11 — Throne Room: N→10, S→12
  slot 20: dir=DIR_N, dest=10, hidden=0
  slot 21: dir=DIR_S, dest=12, hidden=0

Room 12 — The Gate: N→11
  slot 22: dir=DIR_N, dest=11, hidden=0
```

Total exit slots used: 22. DIM exitDir[30] is sufficient.

**Slot 0 is deliberately unused.** Slots are 1-indexed in this implementation —
the first room's first exit is at slot 1, not slot 0. This avoids ambiguity
between "no slot assigned" (0) and "first slot" (0). All roomExitStart values
are >= 1. The generator must initialise slots starting from 1.

roomExitStart values: [1]=1, [2]=2, [3]=5, [4]=6, [5]=10, [6]=10, [7]=12, [8]=14, [9]=16, [10]=18, [11]=20, [12]=22
roomExitCount values: [1]=1, [2]=3, [3]=1, [4]=4, [5]=0, [6]=2, [7]=2, [8]=2, [9]=2, [10]=2, [11]=2, [12]=1

---

## Text Strings Required

**All 12 room descriptions:** Deliverable 1, all rooms, first-visit and revisit variants.
**Room 6 — all variants:** Deliverable 1, Room [6] — Collapsed Passage, all six variants
plus SEARCH find text. (SEARCH is not wired yet — include the text in PrepRoom
but it will only be used in Issue 6.)

**DirName returns:** "NORTH", "SOUTH", "EAST", "WEST", "NE" — UI chrome, inline.
**RoomName returns:** Room names — UI chrome, inline.

---

## Known Gotchas

**Room 5 traps the player in Issue 4:**
Room 5 has no exits. If the player navigates there, they are stuck. This is
expected. Add a comment in the code noting that the Still Chamber sequence
is implemented in Issue 8. Do not add any special handling in Issue 4.

**roomExitStart for room 5:**
Room 5 has no exits (`roomExitCount[5] = 0`). `roomExitStart[5]` must still
be set to a valid value — use 10 (the next slot after room 4's exits end).
The count being 0 means the loop in HandleGo and PrintExits never iterates.

**Underhall north exit:**
Room 10 connects north to room 4. This is the convergence of both routes.
Both the easy route (6→9→10) and the hard route (7→8→10) end here.

**SELECT CASE for DirName and RoomName:**
Both functions use SELECT CASE — this is the correct pattern for multi-branch
string lookup. Not IF/ELSE chains.

**PrintRoom visited flag — update from Issue 2:**
Issue 2 hardcoded `IF visited[1] = 0 THEN` in PrintRoom. Issue 4 adds all 12 rooms.
Update every room branch to use `IF visited[roomId] = 0 THEN` — the parameter,
not a hardcoded room number. This is a required fix as part of Issue 4's
expansion of PrintRoom.

---

## Testing Checklist

- [ ] `InitExits()` called at startup (before OpeningSequence)
- [ ] All 12 rooms reachable via navigation
- [ ] All room descriptions correct (first-visit and revisit)
- [ ] Location line shows correct room name in each room
- [ ] Exits list accurate for each room
- [ ] Room 6 south exit does not appear
- [ ] Room 8 south exit does not appear
- [ ] Room 5 traps player (no exits shown, GO commands all print no-exit response)
- [ ] GO NORTH from room 2 returns to room 1 (revisit text shows)
- [ ] Direction constants used — no magic numbers in navigation code
- [ ] No-exit response for invalid directions

---

## Issue File Reference

`listing/issue-04-moving-through-stone.md`
