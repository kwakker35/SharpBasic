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
- `SUB StillChamberSequence()` — teleport mechanic for room 5, shows shorter revisit text on return visits, advances 3 turns, tests luck, lands player in lucky or unlucky pool room
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

`EnterRoom` gains two new detection branches at the top of the SUB. If `roomId = ROOM_STILL`, `StillChamberSequence` runs immediately instead of the standard room entry — every visit triggers the teleport, but revisits show shorter recognition text instead of the full narrative. If `roomId = ROOM_RIDDLE`, the code checks `riddleSolved` — if already solved the room is entered normally, if not `RiddleRoomSequence` runs.

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
REM === ADD TO: file header comment block, after Issue 7 line ===

REM  Issue 8: Riddle, Still Chamber, Underhall lock
REM  Still Chamber teleport, riddle room, Underhall east door lock.


REM === ADD TO: globals block, after LET zombieRoom = 0 ===

REM ----------------------------------------------------------------
REM  Riddle state
REM  riddleIndex: 1-8, which riddle is active this run.
REM  riddleCorrectDoor: 1=LEFT, 2=RIGHT -- the safe door.
REM  riddleSolved: 0 = unsolved, 1 = solved (pass-through on revisit).
REM ----------------------------------------------------------------
LET riddleIndex = 0
LET riddleCorrectDoor = 0
LET riddleSolved = 0

REM  Lucky and unlucky teleport pools for the Still Chamber
DIM luckyPool[2] AS INTEGER
DIM unluckyPool[6] AS INTEGER


REM === ADD TO: reset block, after LET zombieRoom = 0 ===

    LET riddleIndex = 0
    LET riddleCorrectDoor = 0
    LET riddleSolved = 0
    LET pendingFlavourCount = 0


REM === ADD BEFORE: SUB CombatLoop (or wherever InitRiddle fits logically) ===

REM =================================================================
REM  FUNCTION TestLuck -> INTEGER
REM  Rolls 2d6 against current LUCK. Returns 1 if lucky, 0 if not.
REM  Decrements LUCK by 1 regardless. Increments luckTestCount.
REM =================================================================
FUNCTION TestLuck() AS INTEGER
    LET roll = RollDice(2)
    SET GLOBAL luckTestCount = luckTestCount + 1
    IF roll <= luck THEN
        SET GLOBAL luck = MAX(luck - 1, 0)
        RETURN 1
    ELSE
        SET GLOBAL luck = MAX(luck - 1, 0)
        RETURN 0
    END IF
END FUNCTION

REM =================================================================
REM  SUB InitRiddle
REM  Selects one of eight riddles at random and assigns the correct
REM  door (1=LEFT, 2=RIGHT) independently. Populates lucky/unlucky
REM  teleport pools for the Still Chamber. Called once per run.
REM =================================================================
SUB InitRiddle()
    LET pick = CINT(RND() * 8) + 1
    SET GLOBAL riddleIndex = pick
    SET GLOBAL riddleCorrectDoor = CINT(RND() * 2) + 1
    SET GLOBAL riddleSolved = 0

    SET GLOBAL luckyPool[0] = ROOM_ENTRY
    SET GLOBAL luckyPool[1] = ROOM_ARMOURY

    SET GLOBAL unluckyPool[0] = ROOM_GUARDROOM
    SET GLOBAL unluckyPool[1] = ROOM_COLLAPSED
    SET GLOBAL unluckyPool[2] = ROOM_PIT
    SET GLOBAL unluckyPool[3] = ROOM_RIDDLE
    SET GLOBAL unluckyPool[4] = ROOM_CISTERN
    SET GLOBAL unluckyPool[5] = ROOM_UNDERHALL
END SUB

REM =================================================================
REM  SUB PrintRiddle
REM  Prints the active riddle text based on riddleIndex, then labels
REM  each door with a candidate answer. The correct answer is placed
REM  on the door matching riddleCorrectDoor (1=LEFT, 2=RIGHT).
REM  Player must solve the riddle to know which door is safe.
REM =================================================================
SUB PrintRiddle()
    LET answer$ = ""
    LET decoy$ = ""
    SELECT CASE riddleIndex
        CASE 1
            PRINT "  I have cities, but no houses."
            PRINT "  Mountains, but no trees."
            PRINT "  Water, but no fish."
            PRINT "  Roads, but no carriages."
            PRINT ""
            PRINT "  What am I?"
            LET answer$ = "A Map"
            LET decoy$ = "A Dream"
        CASE 2
            PRINT "  The more you take, the more you leave behind."
            PRINT ""
            PRINT "  What am I?"
            LET answer$ = "Footsteps"
            LET decoy$ = "Shadows"
        CASE 3
            PRINT "  I speak without a mouth and hear without ears."
            PRINT "  I have no body, but I come alive with wind."
            PRINT ""
            PRINT "  What am I?"
            LET answer$ = "An Echo"
            LET decoy$ = "The Wind"
        CASE 4
            PRINT "  This man's father is my father's son."
            PRINT "  I have no brothers."
            PRINT ""
            PRINT "  Who is this man?"
            LET answer$ = "My Son"
            LET decoy$ = "Myself"
        CASE 5
            PRINT "  Two guards. Two doors. One guard always lies."
            PRINT "  One always tells the truth."
            PRINT "  You may ask one question of one guard."
            PRINT ""
            PRINT "  Which door is safe?"
            LET answer$ = "The opposite"
            LET decoy$ = "Their answer"
        CASE 6
            PRINT "  The more you have of it, the less you see."
            PRINT ""
            PRINT "  What is it?"
            LET answer$ = "Darkness"
            LET decoy$ = "Silence"
        CASE 7
            PRINT "  I am not alive, but I grow."
            PRINT "  I have no lungs, but I need air."
            PRINT "  I have no mouth, but water kills me."
            PRINT ""
            PRINT "  What am I?"
            LET answer$ = "Fire"
            LET decoy$ = "Time"
        CASE 8
            PRINT "  I can run but never walk."
            PRINT "  I have a mouth but never talk."
            PRINT "  I have a head but never weep."
            PRINT "  I have a bed but never sleep."
            PRINT ""
            PRINT "  What am I?"
            LET answer$ = "A River"
            LET decoy$ = "A Road"
    END SELECT
    PRINT ""
    IF riddleCorrectDoor = 1 THEN
        PRINT "  LEFT: " & answer$ & "          RIGHT: " & decoy$
    ELSE
        PRINT "  LEFT: " & decoy$ & "          RIGHT: " & answer$
    END IF
END SUB

REM =================================================================
REM  SUB StillChamberSequence
REM  Fires when the player enters Room 5 (The Still Chamber).
REM  On revisit, prints shorter recognition text. First visit prints
REM  full narrative. Both paths advance 3 turns, test luck, teleport
REM  to lucky or unlucky pool. Calls EnterRoom at the destination.
REM =================================================================
SUB StillChamberSequence()
    CALL PrintHeader()
    CALL FlushFlavour()
    PRINT "  LOCATION: " & RoomName(ROOM_STILL)
    PRINT ""
    IF visited[ROOM_STILL - 1] = 1 THEN
        PRINT "  You recognise it immediately. The circular walls. The sourceless light."
        PRINT "  The silence with weight in it."
        PRINT ""
        PRINT "  You know what this room does."
        PRINT ""
        PRINT "  Knowing does not help."
        PRINT ""
        PRINT "  The floor comes up to meet you."
    ELSE
        PRINT "  The passage ends in a room that should not be here."
        PRINT ""
        PRINT "  The walls are smooth -- not hewn, not carved, smooth in a way that"
        PRINT "  stone has no right to be. The air is still. Not quiet -- still."
        PRINT "  There is a difference. Quiet is the absence of sound. Still is the"
        PRINT "  absence of everything else."
        PRINT ""
        PRINT "  The floor comes up to meet you."
    END IF
    PRINT ""
    CALL Pause()
    PRINT ""
    PRINT "  You do not remember falling. You do not remember how long you were"
    PRINT "  down. When the world reassembles itself, you are somewhere else."
    PRINT ""
    CALL AdvanceTurns(3)
    LET result = TestLuck()
    IF result = 1 THEN
        LET pick = CINT(RND() * 2)
        SET GLOBAL currentRoom = luckyPool[pick]
        PRINT "  Fortune held. You wake in familiar ground."
        PRINT ""
    ELSE
        LET pick = CINT(RND() * 6)
        SET GLOBAL currentRoom = unluckyPool[pick]
        PRINT "  The dice betray you. You wake somewhere worse."
        PRINT ""
    END IF
    SET GLOBAL visited[ROOM_STILL - 1] = 1
    CALL EnterRoom(currentRoom, 0)
END SUB

REM =================================================================
REM  SUB RiddleRoomSequence
REM  Fires when the player enters Room 8 and riddleSolved = 0.
REM  Inner loop accepts LEFT or RIGHT only. Correct: riddleSolved=1,
REM  unhide exits 14 and 15. Wrong: instant death (endState=3).
REM =================================================================
SUB RiddleRoomSequence()
    CALL PrintHeader()
    CALL FlushFlavour()
    PRINT "  LOCATION: " & RoomName(ROOM_RIDDLE)
    PRINT ""
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
    PRINT ""
    CALL PrintSeparator()
    PRINT ""
    CALL PrintRiddle()
    PRINT ""
    WHILE riddleSolved = 0
        INPUT "  Choose a door (LEFT / RIGHT): "; cmd$
        LET cmd$ = UPPER$(cmd$)
        LET chosenDoor = 0
        IF cmd$ = "LEFT" THEN
            LET chosenDoor = 1
        END IF
        IF cmd$ = "RIGHT" THEN
            LET chosenDoor = 2
        END IF
        IF chosenDoor > 0 THEN
            IF chosenDoor = riddleCorrectDoor THEN
                SET GLOBAL riddleSolved = 1
                PRINT ""
                PRINT "  The door yields. Beyond it, a passage -- real, open, leading somewhere."
                PRINT "  The wall behind you is still solid. But you are through."
                PRINT ""
                SET GLOBAL exitHidden[14] = 0
                SET GLOBAL exitHidden[15] = 0
                SET GLOBAL visited[ROOM_RIDDLE - 1] = 1
                CALL EnterRoom(ROOM_RIDDLE, 0)
            ELSE
                PRINT ""
                PRINT "  You pass through the door into a room straight from your worst nightmare."
                PRINT "  The mouldy bones of previous adventurers litter the floor. Realising"
                PRINT "  your mistake you turn to leave -- only to find the door has no handle."
                PRINT "  On closer examination you notice bloody scratches, and is that a fingernail?"
                PRINT ""
                PRINT "  A deep rumble shakes your body and turns your bowels to water. You look"
                PRINT "  up to see the ceiling, slow but inexorably, lowering."
                PRINT ""
                PRINT "  You frantically try to open the door, adding to the bloody legacy, as"
                PRINT "  the room crushes the life out of you."
                PRINT ""
                SET GLOBAL gameOver = 1
                SET GLOBAL endState = 3
                RETURN
            END IF
        ELSE
            PRINT "  The doors do not move. The room waits."
            PRINT ""
        END IF
    WEND
END SUB


REM === MODIFY: SUB EnterRoom -- add special-sequence intercepts at top ===

REM  Add before CALL PrintHeader(), at the very top of EnterRoom:

    IF roomId = ROOM_STILL THEN
        CALL StillChamberSequence()
        RETURN
    END IF
    IF roomId = ROOM_RIDDLE AND riddleSolved = 0 THEN
        CALL RiddleRoomSequence()
        RETURN
    END IF


REM === MODIFY: SUB HandleGo -- add sealed Underhall door ===

REM  Add after the rubble-clearing check, before LET start = ...:

    REM --- Issue 8: Underhall east door locked until riddle solved ---
    IF currentRoom = ROOM_UNDERHALL AND dir = DIR_E AND riddleSolved = 0 THEN
        CALL QueueFlavour("  The eastern passage ends at a door. It is sealed -- not locked,")
        CALL QueueFlavour("  sealed, as though the stone grew shut around it. Whatever opens")
        CALL QueueFlavour("  this, it is not a key.")
        CALL QueueFlavour("")
        RETURN
    END IF



REM === ADD TO: game loop SELECT CASE, before CASE ELSE ===

            CASE "LUCK"
                LET result = TestLuck()
                IF result = 1 THEN
                    PRINT "  Fortune holds -- for now. Your LUCK is tested and found sufficient."
                    PRINT ""
                ELSE
                    PRINT "  The dice betray you. Whatever you were hoping for, this is not it."
                    PRINT ""
                END IF
                CALL AdvanceTurns(1)


REM === MODIFY: SUB PrintEndScreen -- add endState 3 branch ===

    IF endState = 3 THEN
        PRINT "  You pass through the door into a room straight from your worst nightmare."
        PRINT "  The mouldy bones of previous adventurers litter the floor. Realising"
        PRINT "  your mistake you turn to leave -- only to find the door has no handle."
        PRINT "  On closer examination you notice bloody scratches, and is that a fingernail?"
        PRINT ""
        PRINT "  A deep rumble shakes your body and turns your bowels to water. You look"
        PRINT "  up to see the ceiling, slow but inexorably, lowering."
        PRINT ""
        PRINT "  You frantically try to open the door, adding to the bloody legacy, as"
        PRINT "  the room crushes the life out of you."
    ELSE
        PRINT "  Your STAMINA reached zero."
    END IF


REM === ADD TO: startup block, after CALL InitMonsters() ===

    CALL InitRiddle()
```
