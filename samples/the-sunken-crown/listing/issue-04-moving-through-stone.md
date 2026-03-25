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
REM === ADD TO: file header comment block, replacing the Issue 3 line ===
REM  Issue 4: Moving Through Stone
REM  Exit arrays, full map, navigation, all 12 room descriptions.

REM === ADD TO: Constants block, after CONST CONTENT_ROWS = 20 ===
CONST DIR_N = 1
CONST DIR_S = 2
CONST DIR_E = 3
CONST DIR_W = 4
CONST DIR_NE = 5
CONST MAX_EXITS = 30
CONST MAX_ROOMS = 12

REM === ADD TO: Navigation state block, after DIM visited[12] AS INTEGER ===
DIM searched[12] AS INTEGER

REM === ADD TO: Navigation state block, after DIM searched[12] AS INTEGER ===
REM  Exit map -- parallel arrays indexed by exit slot number (1-based)
REM  Slot 0 is unused; valid slots are 1 to 22 for this 12-room map.
REM  roomExitStart[r-1]: index of first exit slot for room r
REM  roomExitCount[r-1]: number of exits for room r
REM  exitDir[i]:    direction code for exit slot i
REM  exitDest[i]:   destination room for exit slot i
REM  exitHidden[i]: 0 = visible to player, 1 = hidden until SEARCHed
DIM roomExitStart[12] AS INTEGER
DIM roomExitCount[12] AS INTEGER
DIM exitDir[30] AS INTEGER
DIM exitDest[30] AS INTEGER
DIM exitHidden[30] AS INTEGER

REM === NEW FUNCTION: add after FUNCTION RollDice ===
REM =================================================================
REM  FUNCTION DirName -- dir AS INTEGER -> STRING
REM  Returns the display name for a direction code.
REM  Used by PrintExits to build the exits line.
REM =================================================================
FUNCTION DirName(dir AS INTEGER) AS STRING
    SELECT CASE dir
        CASE DIR_N
            RETURN "NORTH"
        CASE DIR_S
            RETURN "SOUTH"
        CASE DIR_E
            RETURN "EAST"
        CASE DIR_W
            RETURN "WEST"
        CASE DIR_NE
            RETURN "NE"
        CASE ELSE
            RETURN "?"
    END SELECT
END FUNCTION

REM === NEW FUNCTION: add after FUNCTION DirName ===
REM =================================================================
REM  FUNCTION RoomName -- roomId AS INTEGER -> STRING
REM  Returns the display name for a room number.
REM  Used by EnterRoom to print the LOCATION line.
REM =================================================================
FUNCTION RoomName(roomId AS INTEGER) AS STRING
    SELECT CASE roomId
        CASE 1
            RETURN "Entry Hall"
        CASE 2
            RETURN "Guardroom"
        CASE 3
            RETURN "Armoury"
        CASE 4
            RETURN "The Crossroads"
        CASE 5
            RETURN "The Still Chamber"
        CASE 6
            RETURN "Collapsed Passage"
        CASE 7
            RETURN "The Pit"
        CASE 8
            RETURN "The Riddle Room"
        CASE 9
            RETURN "The Cistern"
        CASE 10
            RETURN "The Underhall"
        CASE 11
            RETURN "Throne Room"
        CASE 12
            RETURN "The Gate"
        CASE ELSE
            RETURN "Unknown"
    END SELECT
END FUNCTION

REM === NEW SUB: add after SUB RollStartingStats ===
REM =================================================================
REM  SUB InitExits
REM  Populates the exit map for all 12 rooms. Called once at startup
REM  before OpeningSequence. All arrays use 0-based room indexing
REM  (roomExitStart[roomId - 1]) and 1-based slot numbering
REM  (slot 0 is unused; first slot is 1). exitHidden = 1 marks exits
REM  that do not display until the room is SEARCHed (Issue 6).
REM =================================================================
SUB InitExits()
    REM --- Room 1: Entry Hall -- S->2 (slot 1) ---
    LET roomExitStart[0] = 1
    LET roomExitCount[0] = 1
    LET exitDir[1] = DIR_S
    LET exitDest[1] = 2
    LET exitHidden[1] = 0

    REM --- Room 2: Guardroom -- N->1, E->3, S->4 (slots 2-4) ---
    LET roomExitStart[1] = 2
    LET roomExitCount[1] = 3
    LET exitDir[2] = DIR_N
    LET exitDest[2] = 1
    LET exitHidden[2] = 0
    LET exitDir[3] = DIR_E
    LET exitDest[3] = 3
    LET exitHidden[3] = 0
    LET exitDir[4] = DIR_S
    LET exitDest[4] = 4
    LET exitHidden[4] = 0

    REM --- Room 3: Armoury -- W->2 (slot 5) ---
    LET roomExitStart[2] = 5
    LET roomExitCount[2] = 1
    LET exitDir[5] = DIR_W
    LET exitDest[5] = 2
    LET exitHidden[5] = 0

    REM --- Room 4: The Crossroads -- N->2, W->6, E->7, NE->5 (slots 6-9) ---
    LET roomExitStart[3] = 6
    LET roomExitCount[3] = 4
    LET exitDir[6] = DIR_N
    LET exitDest[6] = 2
    LET exitHidden[6] = 0
    LET exitDir[7] = DIR_W
    LET exitDest[7] = 6
    LET exitHidden[7] = 0
    LET exitDir[8] = DIR_E
    LET exitDest[8] = 7
    LET exitHidden[8] = 0
    LET exitDir[9] = DIR_NE
    LET exitDest[9] = 5
    LET exitHidden[9] = 0

    REM --- Room 5: The Still Chamber -- no exits (teleport handled in Issue 8) ---
    LET roomExitStart[4] = 10
    LET roomExitCount[4] = 0

    REM --- Room 6: Collapsed Passage -- E->4, S->9 (hidden) (slots 10-11) ---
    LET roomExitStart[5] = 10
    LET roomExitCount[5] = 2
    LET exitDir[10] = DIR_E
    LET exitDest[10] = 4
    LET exitHidden[10] = 0
    LET exitDir[11] = DIR_S
    LET exitDest[11] = 9
    LET exitHidden[11] = 1

    REM --- Room 7: The Pit -- W->4, S->8 (slots 12-13) ---
    LET roomExitStart[6] = 12
    LET roomExitCount[6] = 2
    LET exitDir[12] = DIR_W
    LET exitDest[12] = 4
    LET exitHidden[12] = 0
    LET exitDir[13] = DIR_S
    LET exitDest[13] = 8
    LET exitHidden[13] = 0

    REM --- Room 8: The Riddle Room -- N->7, S->10 (hidden until solved) (slots 14-15) ---
    LET roomExitStart[7] = 14
    LET roomExitCount[7] = 2
    LET exitDir[14] = DIR_N
    LET exitDest[14] = 7
    LET exitHidden[14] = 0
    LET exitDir[15] = DIR_S
    LET exitDest[15] = 10
    LET exitHidden[15] = 1

    REM --- Room 9: The Cistern -- N->6, S->10 (slots 16-17) ---
    LET roomExitStart[8] = 16
    LET roomExitCount[8] = 2
    LET exitDir[16] = DIR_N
    LET exitDest[16] = 6
    LET exitHidden[16] = 0
    LET exitDir[17] = DIR_S
    LET exitDest[17] = 10
    LET exitHidden[17] = 0

    REM --- Room 10: The Underhall -- N->4, S->11 (slots 18-19) ---
    LET roomExitStart[9] = 18
    LET roomExitCount[9] = 2
    LET exitDir[18] = DIR_N
    LET exitDest[18] = 4
    LET exitHidden[18] = 0
    LET exitDir[19] = DIR_S
    LET exitDest[19] = 11
    LET exitHidden[19] = 0

    REM --- Room 11: Throne Room -- N->10, S->12 (slots 20-21) ---
    LET roomExitStart[10] = 20
    LET roomExitCount[10] = 2
    LET exitDir[20] = DIR_N
    LET exitDest[20] = 10
    LET exitHidden[20] = 0
    LET exitDir[21] = DIR_S
    LET exitDest[21] = 12
    LET exitHidden[21] = 0

    REM --- Room 12: The Gate -- N->11 (slot 22) ---
    LET roomExitStart[11] = 22
    LET roomExitCount[11] = 1
    LET exitDir[22] = DIR_N
    LET exitDest[22] = 11
    LET exitHidden[22] = 0
END SUB

REM === NEW SUB: add after SUB InitExits ===
REM =================================================================
REM  SUB PrintExits -- roomId AS INTEGER
REM  Prints the visible exits for the given room. Hidden exits
REM  (exitHidden = 1) are not shown. Called from PrintRoom.
REM  Builds the exits line by concatenating direction names.
REM =================================================================
SUB PrintExits(roomId AS INTEGER)
    LET start = roomExitStart[roomId - 1]
    LET count = roomExitCount[roomId - 1]
    IF count = 0 THEN
        PRINT "  Exits: none"
        RETURN
    END IF
    LET exitLine = "  Exits:"
    FOR i = start TO start + count - 1
        IF exitHidden[i] = 0 THEN
            LET exitLine = exitLine & " " & DirName(exitDir[i])
        END IF
    NEXT i
    PRINT exitLine
END SUB

REM === NEW SUB: add after SUB PrintExits ===
REM =================================================================
REM  SUB HandleGo -- dir AS INTEGER
REM  Walks the exit slots for the current room. If a visible exit
REM  matching dir is found, moves the player to the destination and
REM  returns. If no match, prints the no-exit response.
REM  Writes currentRoom via SET GLOBAL -- the only global write here.
REM =================================================================
SUB HandleGo(dir AS INTEGER)
    LET start = roomExitStart[currentRoom - 1]
    LET count = roomExitCount[currentRoom - 1]
    FOR i = start TO start + count - 1
        IF exitDir[i] = dir AND exitHidden[i] = 0 THEN
            SET GLOBAL currentRoom = exitDest[i]
            RETURN
        END IF
    NEXT i
    PRINT "  There is no way through in that direction."
    PRINT ""
END SUB

REM === REPLACE: entire SUB PrintRoom -- replaces the Issue 2/3 single-room version ===
REM =================================================================
REM  SUB PrintRoom -- roomId AS INTEGER
REM  Selects and prints the room description for the given room.
REM  Chooses first-visit or revisit text based on the visited flag.
REM  For rooms with monsters (2, 6, 7, 9, 10): revisit text shows
REM  the monster-alive variant. Dead-variant text is added in Issue 5
REM  when monsterAlive[] is available.
REM  Calls PrintExits at the end of each room branch.
REM  Note: visited flag is read here, set by EnterRoom after this call.
REM =================================================================
SUB PrintRoom(roomId AS INTEGER)
    IF roomId = 1 THEN
        IF visited[roomId - 1] = 1 THEN
            PRINT "  You have been here before. The torches are still burning."
            PRINT "  Whatever tends them has been and gone since you passed through."
            PRINT ""
            PRINT "  The gates are still closed."
        ELSE
            PRINT "  The hall is wide and low, the ceiling barely clearing your head. The stone"
            PRINT "  is older here than anywhere you have ever stood -- worn smooth by centuries"
            PRINT "  of feet, most of which never came back. Torches burn in iron brackets on"
            PRINT "  either wall. Someone keeps them lit. You have not seen who."
            PRINT ""
            PRINT "  The air smells of cold and damp and something faintly animal. Behind you,"
            PRINT "  the gates are closed. You heard the bar drop. You did not look back."
            PRINT ""
            PRINT "  South is a doorway with no door. Beyond it, the sound of the dungeon"
            PRINT "  settling into itself."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 2 THEN
        IF visited[roomId - 1] = 1 THEN
            REM  Dead-variant text added in Issue 5 when monsterAlive[] is available.
            PRINT "  It is still here. It has turned to face the door this time."
        ELSE
            PRINT "  The room smells of old iron and something worse underneath it. Weapon"
            PRINT "  racks line the far wall -- empty, or nearly. Whatever was stored here"
            PRINT "  has been used, or lost, or taken by someone who came before you."
            PRINT ""
            PRINT "  A broad figure stands in the passage to the south, back to you. Big."
            PRINT "  The kind of big that moves slowly because it doesn't need to move fast."
            PRINT "  It has not noticed you yet."
            PRINT ""
            PRINT "  There are deep gouges in the floor near the centre of the room. Something"
            PRINT "  heavy was dragged here. You don't look too closely at the dark stain"
            PRINT "  beside them."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 3 THEN
        IF visited[roomId - 1] = 1 THEN
            PRINT "  The chest is as you left it -- locked, or open, depending on what you"
            PRINT "  did here."
            PRINT ""
            PRINT "  The brackets on the walls are still empty. The room has nothing more"
            PRINT "  to offer you."
        ELSE
            PRINT "  A dead end -- or what passes for one down here. The room is narrow, the"
            PRINT "  walls lined with empty brackets and bare hooks. Whatever armed the"
            PRINT "  dungeon's original occupants is long gone."
            PRINT ""
            PRINT "  Against the far wall sits a chest. Iron-banded, old, locked. The lock"
            PRINT "  is substantial. Someone thought what was inside was worth protecting."
            PRINT ""
            PRINT "  The only way out is west, back the way you came."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 4 THEN
        IF visited[roomId - 1] = 1 THEN
            PRINT "  You are back at the crossroads. The passages sit where they were. The"
            PRINT "  dungeon has not rearranged itself. Not here."
        ELSE
            PRINT "  Four passages meet here. The ceiling rises slightly -- not comfort, just"
            PRINT "  space. The floor is worn in crossing patterns where feet have moved"
            PRINT "  between directions over a very long time."
            PRINT ""
            PRINT "  North leads back toward the Guardroom. West opens into darkness that"
            PRINT "  smells of dust and something crushed. East carries a faint sound you"
            PRINT "  cannot quite identify. A passage angles away to the northeast."
            PRINT ""
            PRINT "  This is the centre of the dungeon. You feel it."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 5 THEN
        REM  The Still Chamber has no exits in Issue 4. The player is trapped here.
        REM  The teleport mechanic (luck roll, lucky/unlucky pool) is implemented in Issue 8.
        IF visited[roomId - 1] = 1 THEN
            PRINT "  You recognise it immediately. The circular walls. The sourceless light."
            PRINT "  The silence with weight in it."
            PRINT ""
            PRINT "  You know what this room does."
            PRINT ""
            PRINT "  Knowing does not help."
            PRINT ""
            PRINT "  The floor comes up to meet you."
        ELSE
            PRINT "  The passage ends in a room that shouldn't be here."
            PRINT ""
            PRINT "  It is perfectly circular. The walls are smooth in a way the rest of the"
            PRINT "  dungeon is not -- not worn smooth, shaped smooth, as though the stone was"
            PRINT "  never anything else. There are no torches. The light comes from nowhere"
            PRINT "  you can identify and illuminates nothing clearly."
            PRINT ""
            PRINT "  There is no door. There is no exit. There is only the room, and the"
            PRINT "  silence, and a heaviness behind your eyes that you notice too late to do"
            PRINT "  anything about."
            PRINT ""
            PRINT "  The floor comes up to meet you."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 6 THEN
        REM  Dead-variant and passage-found-variant text added in Issue 5/6
        REM  when monsterAlive[] and searched[] are used to branch here.
        IF visited[roomId - 1] = 1 THEN
            PRINT "  The skittering starts again the moment you step through the doorway."
            PRINT ""
            PRINT "  The rubble at the far end sits undisturbed. Whatever is buried under it,"
            PRINT "  you haven't looked yet."
        ELSE
            PRINT "  The ceiling has come down at the far end -- not recently. The rubble has"
            PRINT "  been here long enough to gather its own dust. Whatever this passage once"
            PRINT "  connected, it no longer does. Not obviously."
            PRINT ""
            PRINT "  The air is cold and close. Something has been moving through here. The"
            PRINT "  dust on the rubble is disturbed in places, and not by your feet."
            PRINT ""
            PRINT "  You hear it before you see it -- a dry, chitinous skittering from"
            PRINT "  somewhere in the dark beyond the fallen stone. Then the sound stops."
            PRINT "  Then it is very close."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 7 THEN
        REM  Dead-variant text added in Issue 5 when monsterAlive[] is available.
        IF visited[roomId - 1] = 1 THEN
            PRINT "  It is still there. Still waiting. It turns to face you again with the"
            PRINT "  same unhurried certainty."
        ELSE
            PRINT "  The floor drops away three feet below the entrance -- you step down into"
            PRINT "  the room rather than walking into it. The pit that gives the room its"
            PRINT "  name is in the centre: a square shaft, iron-railed, dropping into"
            PRINT "  darkness. You cannot hear the bottom."
            PRINT ""
            PRINT "  Standing between you and the far passage is a figure in armour. Old"
            PRINT "  armour -- dented, repaired, repaired again. It has the posture of"
            PRINT "  something that has been waiting for a long time and has no particular"
            PRINT "  feelings about waiting longer."
            PRINT ""
            PRINT "  It turns to face you with the unhurried certainty of something that has"
            PRINT "  done this before."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 8 THEN
        IF visited[roomId - 1] = 1 THEN
            PRINT "  The door you chose stands open. The passage beyond is exactly as you"
            PRINT "  left it. The riddle is still carved into the wall between them. The"
            PRINT "  other door is sealed. There is no handle, no gap, no way through."
            PRINT "  Whatever is on the other side stays there."
        ELSE
            PRINT "  The door behind you is gone."
            PRINT ""
            PRINT "  You did not hear it close. You did not feel it move. You turned and it"
            PRINT "  was simply not there -- solid stone where the entrance was, seamless,"
            PRINT "  cold, indifferent."
            PRINT ""
            PRINT "  Ahead of you are two doors, identical in every respect except one: a"
            PRINT "  riddle is carved into the stone wall between them, deep and deliberate,"
            PRINT "  cut by someone who knew exactly what they were doing."
            PRINT ""
            PRINT "  Below the riddle, two words. One above each door."
            PRINT ""
            PRINT "  LEFT. RIGHT."
            PRINT ""
            PRINT "  The room waits."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 9 THEN
        REM  Dead-variant text added in Issue 5 when monsterAlive[] is available.
        IF visited[roomId - 1] = 1 THEN
            PRINT "  The cold finds you again the moment you step through the doorway. The"
            PRINT "  luminescence flickers at the edge of vision."
            PRINT ""
            PRINT "  It remembers you too."
        ELSE
            PRINT "  Water, somewhere below the floor. You can hear it moving through stone"
            PRINT "  channels -- old infrastructure, still functioning, tending to a dungeon"
            PRINT "  that has long outlasted whoever built it."
            PRINT ""
            PRINT "  The room is larger than it looks. The ceiling is vaulted, the far corners"
            PRINT "  lost in shadow. Against the north wall, partially hidden by a fall of"
            PRINT "  loose stone, is a recess that doesn't quite look accidental."
            PRINT ""
            PRINT "  Something is in the room with you. You sense it before you see it -- a"
            PRINT "  chill that has nothing to do with the water, a faint luminescence at the"
            PRINT "  edge of vision that vanishes when you look directly at it."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 10 THEN
        REM  Dead-variant text added in Issue 5 when monsterAlive[] is available.
        IF visited[roomId - 1] = 1 THEN
            PRINT "  It remembers you. The small dark eyes find you immediately."
        ELSE
            PRINT "  Both passages meet here. This is where the dungeon funnels everything."
            PRINT ""
            PRINT "  The ceiling is high and the room is wide and it smells of age and"
            PRINT "  something organic and wrong. The walls are lined with what you first"
            PRINT "  take for discarded equipment -- bundles of cloth and leather, dull metal,"
            PRINT "  shapes that your eyes keep trying to resolve into something else."
            PRINT ""
            PRINT "  They are not equipment."
            PRINT ""
            PRINT "  They are what happens to people who made it this far and ran out of"
            PRINT "  something -- STAMINA, luck, time. Dry bones in old armour. The dungeon"
            PRINT "  keeps them here. A record."
            PRINT ""
            PRINT "  In the centre of the room, massive and still and watching you with small"
            PRINT "  dark eyes, is the Troll."
            PRINT ""
            PRINT "  It does not move. It is waiting to see what you do."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 11 THEN
        IF visited[roomId - 1] = 1 THEN
            PRINT "  He is already looking at the door when you enter."
            PRINT ""
            PRINT "  The gold is as it was. The throne is as it was. The figure on it is as"
            PRINT "  it was -- minus whatever STAMINA you took from him, already restored,"
            PRINT "  already waiting."
            PRINT ""
            PRINT "  He does not look surprised to see you again."
        ELSE
            PRINT "  The room is vast. After everything that came before it, the scale is"
            PRINT "  wrong -- ceilings that disappear into darkness above, walls lost in"
            PRINT "  shadow at the edges, a floor of cracked stone buried under three hundred"
            PRINT "  years of accumulated coin."
            PRINT ""
            PRINT "  Gold. More than you have ever seen or imagined. More than you will ever"
            PRINT "  be able to carry. It sits in drifts and piles around the base of the"
            PRINT "  throne like a tide that came in and never went back out."
            PRINT ""
            PRINT "  The throne is at the far end. On it, a figure. Armoured, still, head"
            PRINT "  bowed under the weight of something on its brow."
            PRINT ""
            PRINT "  Not dead. You can tell, even from here. Not dead, not sleeping. Waiting"
            PRINT "  -- the way only something that has been waiting for three hundred years"
            PRINT "  can wait, with a patience that has long since stopped being patience and"
            PRINT "  become simply the shape of its existence."
            PRINT ""
            PRINT "  It raises its head."
        END IF
        PRINT ""
        CALL PrintExits(roomId)
    END IF
    IF roomId = 12 THEN
        PRINT "  A plain room. Stone floor, stone walls, low ceiling. Two doors in the far"
        PRINT "  wall, side by side. No decoration, no inscription, no indication of what"
        PRINT "  lies beyond either of them."
        PRINT ""
        PRINT "  After everything that brought you here, the ordinariness of it is its own"
        PRINT "  kind of wrong. You stand in the doorway and wait for something to happen."
        PRINT "  Nothing does. The room offers nothing -- no sound from beyond the doors,"
        PRINT "  no draft, no sign. Just two doors in a plain room at the end of a dungeon"
        PRINT "  that has been trying to kill you since you stepped into it."
        PRINT ""
        PRINT "  You keep waiting. The silence holds."
        PRINT ""
        PRINT "  Eventually you realise the dungeon has already made its last move. It just"
        PRINT "  looks like a room."
        PRINT ""
        PRINT "  You step forward and make your choice."
        PRINT ""
        CALL PrintExits(roomId)
    END IF
END SUB

REM === REPLACE: entire SUB EnterRoom -- replaces the Issue 2/3 version ===
REM =================================================================
REM  SUB EnterRoom -- roomId AS INTEGER
REM  Renders a full room screen: header, location line, description,
REM  separator. Sets the visited flag for the room after rendering.
REM  Uses RoomName() for the location line.
REM =================================================================
SUB EnterRoom(roomId AS INTEGER)
    CALL PrintHeader()
    PRINT "  LOCATION: " & RoomName(roomId)
    PRINT ""
    CALL PrintRoom(roomId)
    PRINT ""
    CALL PrintSeparator()
    LET visited[roomId - 1] = 1
END SUB

REM === REPLACE: main program block -- replaces the Issue 2/3 command loop ===
REM =================================================================
REM  Main program
REM  InitExits populates the full map before the opening sequence.
REM  The opening sequence runs: narrative, attribute roll, pacing.
REM  EnterRoom renders the first room. The loop reads and normalises
REM  input, then dispatches all five direction commands to HandleGo.
REM  HandleGo moves the player; EnterRoom renders the new room.
REM  All other input prints the unknown command response.
REM =================================================================
CALL InitExits()
CALL OpeningSequence()
CALL EnterRoom(currentRoom)

WHILE gameOver = 0
    INPUT " > "; cmd$
    LET cmd$ = UPPER$(cmd$)
    IF cmd$ = "GO NORTH" THEN
        CALL HandleGo(DIR_N)
        IF currentRoom > 0 THEN
            CALL EnterRoom(currentRoom)
        END IF
    ELSE
        IF cmd$ = "GO SOUTH" THEN
            CALL HandleGo(DIR_S)
            IF currentRoom > 0 THEN
                CALL EnterRoom(currentRoom)
            END IF
        ELSE
            IF cmd$ = "GO EAST" THEN
                CALL HandleGo(DIR_E)
                IF currentRoom > 0 THEN
                    CALL EnterRoom(currentRoom)
                END IF
            ELSE
                IF cmd$ = "GO WEST" THEN
                    CALL HandleGo(DIR_W)
                    IF currentRoom > 0 THEN
                        CALL EnterRoom(currentRoom)
                    END IF
                ELSE
                    IF cmd$ = "GO NE" THEN
                        CALL HandleGo(DIR_NE)
                        IF currentRoom > 0 THEN
                            CALL EnterRoom(currentRoom)
                        END IF
                    ELSE
                        PRINT "  The dungeon does not respond to that."
                        PRINT ""
                    END IF
                END IF
            END IF
        END IF
    END IF
WEND
```
