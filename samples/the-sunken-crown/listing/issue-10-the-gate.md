# The Sunken Crown
## Issue 10 — The Gate

*Two doors. No label. No instruction. The dungeon's last move.*

---

## The Story So Far

Issues 1 through 9 built everything. The frame, the map, the attributes, the combat, the inventory, the atmosphere, the zombie, the trap rooms, and the final boss. Every system is in place. Every room has its logic.

One room is not yet wired. ROOM_GATE (Room 12). The Gate. You can navigate there, but nothing happens. Two doors sit in the far wall and the program does not know what to do with them.

This issue finishes the game.

---

## What's New This Issue

- `SUB InitGate()` — assigns the correct door at startup, 1=LEFT or 2=RIGHT
- `SUB GateSequence(choice AS INTEGER)` — resolves the final door choice: win (endState 1) or death (endState 4)
- `SUB PrintEndScreen()` — final version, replaces all prior iterations; handles all six endStates, counts gold bags on win, skips stats on crown
- `SUB PrintHelp()` — prints the full command list
- `PrintRoom` ROOM_GATE updated with door-wear hint based on `gateCorrectDoor`
- `CASE "LEFT"` / `CASE "RIGHT"` added to game loop — calls `GateSequence` when in ROOM_GATE
- `CASE "STATS"` / `CASE "HELP"` added to game loop
- `gateCorrectDoor` global state variable and reset
- `CALL InitGate()` added to startup and reset sequence
- Exit re-hide block consolidated in reset (covers Collapsed Passage + Riddle Room exits)
- CHEAT command removed (header lines and game loop block)
- Main block comment updated to final version

---

## Concept — Putting It Together

There is no new SharpBASIC feature in this issue. The Gate uses INPUT, IF, and PRINT — the same tools as Issue 1. What has changed is everything around them.

The complete game is a single `.sbx` file. No modules, no imports, no external dependencies. SharpBASIC's scope — ten phases, one file, a real runnable language — was always pointed at this moment. The constraint was the point.

Here is what happens when the player types LEFT or RIGHT at The Gate:

```
INPUT cmd$
LET cmd$ = UPPER$(cmd$)
IF cmd$ = "LEFT" THEN
    IF gateCorrectDoor = 1 THEN
        REM win sequence
    ELSE
        REM death sequence
    END IF
ELSE
    IF cmd$ = "RIGHT" THEN
        IF gateCorrectDoor = 2 THEN
            REM win sequence
        ELSE
            REM death sequence
        END IF
    ELSE
        PRINT "  The dungeon does not respond to that."
    END IF
END IF
```

The same INPUT-UPPER$-IF pattern used since Issue 2. The same nested IF structure used since Issue 8. The code is simple. What surrounds it — ten issues of building, every system connected, the entire dungeon held in one file — is not.

What you have built is a tree-walking interpreter executing a program that tells a story. Every PRINT statement passes through a lexer, a parser, an AST, and an evaluator before a character appears on screen. You built all of those too. The dungeon runs on an engine you made by hand.

That is not a small thing.

---

## How It Fits

`GateSequence` is called from the game loop's LEFT and RIGHT commands when the player is in ROOM_GATE. After this issue, all twelve rooms are fully wired — standard entry for most rooms, dedicated sequences for rooms 5 (Still Chamber), 8 (Riddle Room), and 11 (Throne Room via HandleFight).

The play again loop is the outermost structure of the entire MAIN block. On YES, every state variable resets to its starting value and every array is cleared. `InitExits`, `InitMonsters`, `ShuffleLoot`, `InitRiddle`, and `InitGate` all run again. The opening sequence runs. A new run begins. Every run is a fresh game with fresh dice.

The end screen reads `endState` to select its message. The state was set wherever the run ended — STAMINA zero, Riddle Room wrong door, Gate wrong door, Gate correct door, or TAKE CROWN. The end screen is the same SUB regardless of outcome.

---

## What You'll See

Navigate to ROOM_GATE (The Gate). The Gate description renders. The prompt accepts only LEFT or RIGHT. Choose the less worn door. If correct, the win sequence runs — Malachar in the courtyard, his single word, freedom. If wrong, the dungeon keeps you quietly.

The end screen shows your stats. Play again and everything resets. Different attributes, different loot, different riddle, different gate. The dungeon is learned across runs, not within them.

Play it through at least twice. On the second run you know things you didn't on the first. That is the game working exactly as designed.

---

## What to Try

Change the ending. Not the mechanics — the text. The win text, the death text, the end screen message. Make it yours. The published version uses text from the content asset file. Your copy does not have to.

A game you have modified, even slightly, is a game you understand differently than one you only played. The text is in the content asset file. The code just calls PRINT. Change what gets printed and the ending changes. That is all a text adventure is, at its foundation.

---

## Congratulations

You have built The Sunken Crown — a complete, playable text adventure — across ten issues, each one adding one layer to a single `.sbx` file.

More than that: you built it in SharpBASIC v1 — a language you also built. The lexer, the parser, the AST, the tree-walking evaluator. The dungeon runs on an engine you made by hand.

That was always the point.

---

## The Listing

> **Before finalising Issue 10:** remove the CHEAT command added in Issue 5 for testing.
> In the game loop SELECT CASE, remove the `CASE "CHEAT"` block.

```
REM === ADD TO: file header comment block, after Issue 9 line ===

REM  Issue 10: The Gate
REM  Gate room door choice, win/death paths, PrintEndScreen, HELP, STATS.


REM === ADD TO: globals block, after LET terrorActive = 0 ===

REM ----------------------------------------------------------------
REM  Gate state (Issue 10)
REM  gateCorrectDoor: correct door (1=LEFT, 2=RIGHT), randomised
REM ----------------------------------------------------------------
LET gateCorrectDoor = 0


REM === ADD TO: reset block, after Issue 9 reset lines ===

    LET gateCorrectDoor = 0
    REM  Re-hide exits that may have been unhidden by previous run
    LET exitHidden[11] = 1
    LET exitHidden[14] = 1
    LET exitHidden[15] = 1


REM === ADD TO: reset block -- add CALL InitGate() after CALL InitRiddle() ===

    CALL InitGate()


REM === NEW SUB: add after SUB InitRiddle ===

REM =================================================================
REM  SUB InitGate
REM  Randomises the correct Gate door (1=LEFT, 2=RIGHT).
REM  Called once per run alongside InitRiddle.
REM =================================================================
SUB InitGate()
    SET GLOBAL gateCorrectDoor = CINT(RND() * 2) + 1
END SUB


REM === NEW SUB: add after SUB HandleQuit ===

REM =================================================================
REM  SUB GateSequence -- choice AS INTEGER
REM  Resolves the Gate room door choice. choice = 1 (LEFT) or
REM  2 (RIGHT). Correct door = win (endState 1), wrong = death
REM  (endState 4). Gold count at win reads inventory slots holding
REM  ITEM_GOLD (Decision 10). Win narrative includes a PAUSE break.
REM =================================================================
SUB GateSequence(choice AS INTEGER)
    IF choice = gateCorrectDoor THEN
        REM  === WIN PATH ===
        SET GLOBAL endState = 1
        SET GLOBAL gameOver = 1
        PRINT ""
        PRINT "  The door opens onto grey sky."
        PRINT ""
        PRINT "  You don't move at first. You stand in the doorway and breathe. Cold air."
        PRINT "  Real air. The smell of mud and grass and distance. Things that have"
        PRINT "  nothing to do with stone and dark and death."
        PRINT ""
        PRINT "  Your legs carry you forward because they have forgotten how to stop."
        PRINT ""
        PRINT "  There is no crowd. There never is. The competition is not a spectacle"
        PRINT "  -- it is a disposal."
        PRINT ""
        PRINT "  Lord Malachar is waiting in the courtyard. Six guards flank him. You"
        PRINT "  notice, with the detached clarity of someone who has spent the last"
        PRINT "  hours keeping themselves alive by noticing things, that any one of them"
        PRINT "  could kill you where you stand. You have nothing left. Your hands are"
        PRINT "  shaking. You hadn't noticed until now."
        PRINT ""
        PRINT "  He looks at you for a long moment. His expression does not change."
        CALL Pause()
        PRINT ""
        PRINT "  " & CHR$(34) & "You survived." & CHR$(34)
        PRINT ""
        PRINT "  It is not a question. It is not a compliment. It is the pronouncement"
        PRINT "  of something that was not supposed to happen, delivered by a man who has"
        PRINT "  made his peace with occasionally being wrong."
        PRINT ""
        PRINT "  His eyes drop to the gold in your hands. The gold that was never his to"
        PRINT "  give -- it belonged to the dungeon, and the dungeon let you take it."
        PRINT ""
        PRINT "  He steps aside."
        PRINT ""
        PRINT "  " & CHR$(34) & "Go." & CHR$(34)
        PRINT ""
        PRINT "  You go."
        PRINT ""
        PRINT "  You are alive. You are free. You are one of almost none."
        PRINT ""
    ELSE
        REM  === DEATH PATH ===
        SET GLOBAL endState = 4
        SET GLOBAL gameOver = 1
        PRINT ""
        PRINT "  You walk through the door into yet another damp stone-lined corridor."
        PRINT "  You walk on, exploring the ever more twisting path. Your torch finally"
        PRINT "  stutters and dies, leaving you to roam the pitch black halls and"
        PRINT "  corridors until you eventually succumb to starvation, or something that"
        PRINT "  was already down here finds you first."
        PRINT ""
    END IF
END SUB


REM === REPLACE: SUB PrintEndScreen -- final version (replaces all prior iterations) ===

REM =================================================================
REM  SUB PrintEndScreen
REM  Final end-of-game handler. Branches on endState to print the
REM  appropriate narrative and stats. Win path (endState 1) counts
REM  gold bags from inventory slots holding ITEM_GOLD (Decision 10).
REM  Crown path (endState 2) shows no stats. All others show stats.
REM =================================================================
SUB PrintEndScreen()
    PRINT ""
    CALL PrintSeparator()
    PRINT ""
    IF endState = 1 THEN
        REM  Count gold bags currently in inventory
        LET bagCount = 0
        FOR i = 1 TO 4
            IF inventory[i] = ITEM_GOLD THEN
                LET bagCount = bagCount + 1
            END IF
        NEXT i
        PRINT "  You survived The Sunken Crown."
        IF bagCount = 1 THEN
            PRINT "  You escaped with 1 bag of gold."
        ELSE
            PRINT "  You escaped with " & bagCount & " bags of gold."
        END IF
    END IF
    IF endState = 2 THEN
        PRINT "  You are the Bound King now."
    END IF
    IF endState = 3 THEN
        PRINT "  The ceiling lowered. The room did not care."
    END IF
    IF endState = 4 THEN
        PRINT "  The torch went out. The dungeon kept you."
    END IF
    IF endState = 5 THEN
        PRINT "  Your STAMINA reached zero."
    END IF
    IF endState = 6 THEN
        PRINT "  The dungeon has been here longer than anyone you have ever met has been"
        PRINT "  alive. It will be here after everyone you know is dust."
        PRINT "  It did not notice you arrive."
        PRINT "  It did not notice you leave."
    END IF
    IF endState <> 2 THEN
        PRINT ""
        PRINT "  Your SKILL was " & startSkill & ". Your STAMINA reached as low as " & minStamina & "."
        PRINT "  You tested your LUCK " & luckTestCount & " times."
    END IF
    PRINT ""
END SUB


REM === NEW SUB: add after SUB PrintEndScreen ===

REM =================================================================
REM  SUB PrintHelp
REM  Prints the list of available commands.
REM =================================================================
SUB PrintHelp()
    PRINT ""
    PRINT "  COMMANDS:"
    PRINT "  GO NORTH / SOUTH / EAST / WEST / NE   Move to adjacent room"
    PRINT "  SNEAK NORTH / SOUTH / EAST / WEST / NE Attempt to pass unseen"
    PRINT "  FIGHT                                   Engage monster in room"
    PRINT "  SEARCH                                  Search room for items"
    PRINT "  TAKE <item>                             Pick up an item"
    PRINT "  DROP <item>                             Drop an item"
    PRINT "  USE <item>                              Use an item"
    PRINT "  INVENTORY                               Show carried items"
    PRINT "  LOOK                                    Redisplay room"
    PRINT "  LUCK                                    Test your luck"
    PRINT "  STATS                                   Show current stats"
    PRINT "  LEFT / RIGHT                            Choose a door (Gate only)"
    PRINT "  HELP                                    Show this list"
    PRINT "  QUIT                                    End the game"
    PRINT ""
END SUB


REM === MODIFY: SUB PrintRoom -- ROOM_GATE block ===

REM  Replace the section between "looks like a room." and
REM  "You step forward and make your choice." with:

        IF gateCorrectDoor = 1 THEN
            PRINT "  The right door is worn smooth around the handle. Thousands of hands have"
            PRINT "  reached for it. The left door is stiffer -- the wood darker, the hinges"
            PRINT "  tighter. It has not been opened nearly as often."
        ELSE
            PRINT "  The left door is worn smooth around the handle. Thousands of hands have"
            PRINT "  reached for it. The right door is stiffer -- the wood darker, the hinges"
            PRINT "  tighter. It has not been opened nearly as often."
        END IF
        PRINT ""
        PRINT "  You step forward and make your choice."
        PRINT ""
        PRINT "  Type LEFT or RIGHT to choose a door."


REM === REPLACE: main block comment ===

REM  Replace the existing main block comment with:

REM =================================================================
REM  Main program
REM  InitExits populates the full map. InitMonsters sets all fixed
REM  monsters to alive. The opening sequence runs: narrative,
REM  attribute roll, pacing. EnterRoom renders the first room.
REM  The WHILE loop reads and normalises input, then dispatches
REM  all commands via SELECT CASE. Navigation goes through HandleGo;
REM  combat goes through HandleFight. The loop exits when gameOver = 1.
REM  PrintEndScreen shows the run summary based on endState.
REM =================================================================


REM === REMOVE: game loop SELECT CASE -- CHEAT block ===

REM  Remove these lines from the game loop:
REM            CASE "CHEAT"
REM                REM  DEV TOOL -- strip before release (Issue 10)
REM                LET skill = 12
REM                LET stamina = 24
REM                PRINT "  [CHEAT] SKILL: " & skill & "  STAMINA: " & stamina
REM                PRINT ""


REM === ADD TO: game loop SELECT CASE, after CASE "QUIT" and before CASE ELSE ===

            CASE "LEFT"
                IF currentRoom = ROOM_GATE THEN
                    CALL GateSequence(1)
                ELSE
                    PRINT "  The dungeon does not respond to that."
                    PRINT ""
                END IF
            CASE "RIGHT"
                IF currentRoom = ROOM_GATE THEN
                    CALL GateSequence(2)
                ELSE
                    PRINT "  The dungeon does not respond to that."
                    PRINT ""
                END IF
            CASE "STATS"
                PRINT "  SKILL: " & skill & "  STAMINA: " & stamina & "  LUCK: " & luck
                PRINT ""
            CASE "HELP"
                CALL PrintHelp()
```
