# The Sunken Crown
## Issue 4 — Moving Through Stone

*The dungeon is not a description. It is a place you move through.*

---

## The Story So Far

Issues 1 through 3 built the frame, the opening sequence, and a character with real attributes. The dungeon looks like something and knows who you are.

But it is still just one room. GO SOUTH goes nowhere.

This issue changes that. The full map goes in — all 12 rooms, all exits, the connections between them. North means something. South means something different. The dungeon becomes a place with a shape, and movement through it becomes the central act of play.

---

## What's New This Issue

- Exit arrays — the full map encoded as parallel arrays
- `SUB InitExits()` — populates all room connections including the hidden south exit from room 6
- `FUNCTION DirName(dir AS INTEGER) AS STRING` — converts direction codes to display names
- `FUNCTION RoomName(roomId AS INTEGER) AS STRING` — returns the room name for the location line
- `SUB PrintExits(roomId AS INTEGER)` — prints visible exits for the current room
- `SUB HandleGo(dir AS INTEGER)` — resolves a direction to a destination and moves the player
- All 12 room descriptions wired into `PrintRoom`
- `SUB EnterRoom()` updated to use `RoomName` for the location line
- Direction constants — N, S, E, W, NE as named integers
- The hidden exit flag — room 6's south passage is in the array but not displayed until searched

---

## Concept — Arrays

The dungeon's rooms and exits live in arrays — fixed-size collections of values, each slot numbered from zero.

```
DIM roomExitCount[12] AS INTEGER   REM  how many exits each room has
DIM roomExitStart[12] AS INTEGER   REM  index of first exit slot for each room
DIM exitDir[30] AS INTEGER         REM  direction code for each exit slot
DIM exitDest[30] AS INTEGER        REM  destination room for each exit slot
DIM exitHidden[30] AS INTEGER      REM  1 = hidden until SEARCH finds it
```

This is the parallel array pattern. Five separate arrays, but they describe one thing: the map. They are kept in sync by the initialisation code that populates them.

Here is how to read a room's exits at runtime:

```
LET start = roomExitStart[currentRoom]
LET count = roomExitCount[currentRoom]
FOR i = start TO start + count - 1
    REM exitDir[i] is the direction code
    REM exitDest[i] is where it goes
    REM exitHidden[i] is whether it is visible
NEXT i
```

`roomExitStart[4]` holds the index of room 4's first exit in the exit arrays. `roomExitCount[4]` holds how many exits room 4 has. Together they define a slice of the exit arrays that belongs to room 4.

Why not a simpler approach — a 2D array of room × direction? Because the dungeon has asymmetric connections. Going NE from room 4 reaches room 5, but room 5 has no navigable exits. The Still Chamber teleports you — it does not let you walk back. A flat 2D grid cannot represent this without special-casing every asymmetric connection. The parallel array approach handles asymmetry naturally — every connection is simply stated, with no assumption about return paths.

`DIM` fixes the size at declaration. Thirty exit slots is enough for this map. SharpBASIC checks every array access against that boundary. Go outside it and the interpreter tells you immediately. This is the safety net that makes the map trustworthy.

---

## How It Fits

Issue 2 introduced `EnterRoom` and a command loop that handled GO SOUTH as a placeholder. Issue 4 replaces that placeholder with real navigation. The command loop expands to recognise all five directions. `HandleGo` does the work of finding the matching exit slot and moving the player.

`PrintRoom` gains eleven new room branches — all 12 rooms are now in the file, each with first-visit and revisit text pulled from the content asset file.

`InitExits()` is called once at game startup, before `OpeningSequence`. The map is fixed for the lifetime of the program — it does not change between runs.

---

## What You'll See

The full dungeon is navigable. Starting in the Entry Hall, go south to the Guardroom, east to the Armoury (a dead end with a locked chest), back west and south to the Crossroads. All four directions from the Crossroads lead somewhere. Room descriptions update correctly on entry. Revisit text appears on return.

The northeast passage from the Crossroads leads to the Still Chamber — which has no exit. Entering it will trap you for now. The teleport mechanic is Issue 8's job. For this issue, note the behaviour and move on.

Room 6's south exit to the Cistern is in the map but hidden. It does not appear in the exits list until it is found. SEARCH is not yet wired — that is Issue 6.

---

## What to Try

Add a new connection — make room 1 have a west exit that leads back to room 1. A room that loops back to itself. One new line in `InitExits()`, one incremented exit count.

Does the navigation handler cope? Does anything break? Understanding what the code assumes about the map is as valuable as understanding what it guarantees. Remove the test connection before Issue 5.

---

## The Listing

```
REM Issue 4 listing — to be added once built and tested
```
