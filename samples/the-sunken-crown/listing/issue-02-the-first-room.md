# The Sunken Crown
## Issue 2 — The First Room

*The frame exists. Now something has to go inside it.*

---

## The Story So Far

Issue 1 built the frame — the header, the separator, the prompt. The dungeon has a face. But it is an empty face. There is nothing inside it yet.

This issue puts something inside it. The Entry Hall is the simplest room in the dungeon — no monster, no trap, no decision beyond which way to go. That simplicity is deliberate. The first room is where the player learns to read the game before the game starts asking things of them.

Writing the room description and putting it on screen is the moment SharpBASIC stops being a program and starts being a place.

---

## What's New This Issue

- `SUB PrintRoom(roomId AS INTEGER)` — selects and prints the room description for the given room
- Entry Hall description wired up as room 1, with first-visit and revisit variants
- A basic command loop — reads input, recognises GO SOUTH, prints a placeholder response
- The `visited` flag array introduced — room 1 tracks whether you have been there before
- `SUB Pause()` — prints a continue prompt and waits for ENTER. Used wherever long text sequences exceed the screen. Introduced here, used throughout.

This issue does not wire navigation to a second room. GO SOUTH is recognised but goes nowhere yet. That comes in Issue 4.

---

## Concept — SUBs

The room description is printed by a SUB — a named block of code you can call by name from anywhere in the program.

```
SUB PrintRoom(roomId AS INTEGER)
    IF roomId = ROOM_ENTRY THEN
        PRINT "  The hall is wide and low..."
    END IF
END SUB
```

Break this down:

- `SUB PrintRoom(roomId AS INTEGER)` — declares a subroutine named `PrintRoom` that takes one parameter, `roomId`, which must be an integer
- `IF roomId = ROOM_ENTRY THEN` — checks which room we want
- `PRINT "  The hall is wide and low..."` — prints the description for that room
- `END IF` — closes the conditional
- `END SUB` — closes the subroutine

Call it with `CALL PrintRoom(ROOM_ENTRY)` and the Entry Hall appears. Call it with `CALL PrintRoom(ROOM_GUARDROOM)` and the Guardroom will appear — once we add that room in Issue 4.

The parameter `roomId` is typed as `INTEGER`. SharpBASIC requires typed parameters in every SUB and FUNCTION declaration. This is not optional. If you try to pass a string where an integer is expected, the interpreter will tell you.

SUBs are the architecture the entire game is built on. Every room, every combat frame, every inventory screen goes through a SUB. The pattern you learn here — declare it, give it a typed parameter, call it by name — is the same pattern used everywhere.

One important note: SharpBASIC hoists all SUB and FUNCTION declarations before running any top-level code. This means you can declare `PrintRoom` anywhere in the file and call it anywhere else, regardless of order. Convention is to put all SUBs together near the top of the file, above the main game loop. That is the pattern this game follows throughout.

A second note on scope that matters for every SUB in this game: `LET` inside a SUB writes to local scope only. If a SUB needs to update a global variable — the player's STAMINA, the current room, the visited flag — it must use `SET GLOBAL name = expression` instead of `LET`. This is the pattern used throughout the game for all global state mutations.

---

## How It Fits

Issue 1 created `PrintHeader` and `PrintSeparator`. This issue calls them both — `PrintRoom` renders inside the frame that Issue 1 built.

The command loop introduced here is the seed of the main game loop that will run the entire game by Issue 10. It is simplified now — it only handles one command — but its shape is permanent. Every issue from here adds new commands to this loop without changing its structure.

---

## What You'll See

When you run the program after adding this issue's code, the Entry Hall renders inside the frame. The room description appears, the exits are listed, and the prompt waits. Type `GO SOUTH` and the program responds — it doesn't take you anywhere yet, but it recognises the command. Type anything else and the unknown command response appears.

If the room description does not appear, check that `CALL PrintRoom(ROOM_ENTRY)` is being called and that `PrintRoom` has a branch for `roomId = ROOM_ENTRY`. If the visited text appears on first entry instead of the first-visit text, check the `visited[ROOM_ENTRY - 1]` flag — it should be 0 before `EnterRoom` sets it to 1.

---

## What to Try

Add a second room description — write the Guardroom text and add it as `IF roomId = ROOM_GUARDROOM THEN` inside `PrintRoom`. To test it, temporarily change `CALL EnterRoom(ROOM_ENTRY)` to `CALL EnterRoom(ROOM_GUARDROOM)` in the top-level code. Run the program. The Guardroom renders instead of the Entry Hall.

Change it back to `CALL EnterRoom(ROOM_ENTRY)` when you are done.

This is exactly how every room in the game gets added. The pattern is always the same: a new `IF` branch, a new room number, the text from the content file. Twelve rooms means twelve branches. Understanding the pattern now means Issue 4 holds no surprises.

---

## The Listing

> This is Issue 2. You already have `the-sunken-crown.sbx` from Issue 1.
> Add the code below in the sections indicated. Navigation markers (lines
> starting with `'`) tell you where each block goes — do not type those
> lines into your program.

```
' === MODIFY: file header comment block ===

REM  Replace "Everything that follows in subsequent issues goes inside it."
REM  with:

REM  Issue 2: The First Room
REM  Room description system, PrintRoom, EnterRoom, command loop.


' === ADD TO: constants block, after CONST COMBAT_DELAY ===

CONST MAX_ROOMS = 12
CONST ROOM_ENTRY = 1

' === ADD TO: global declarations, after LET luck = 0 ===

REM ----------------------------------------------------------------
REM  Navigation state
REM  currentRoom: the room the player is currently in (1-12)
REM  visited[MAX_ROOMS]: 0 = first visit, 1 = revisited. Index = roomId - 1.
REM ----------------------------------------------------------------
LET currentRoom = ROOM_ENTRY
DIM visited[MAX_ROOMS] AS INTEGER

REM ----------------------------------------------------------------
REM  Game loop control
REM  gameOver: 0 = game running, 1 = game ended
REM  Not set to 1 in this issue -- loop runs until interrupted.
REM ----------------------------------------------------------------
LET gameOver = 0

' === NEW SUB: add after SUB PrintSeparator() ===

REM =================================================================
REM  SUB Pause
REM  Prints a continue prompt and waits for the player to press
REM  ENTER. Used wherever long text sequences exceed the screen.
REM  The terminal scrolls naturally -- no clearing is performed.
REM =================================================================
SUB Pause()
    INPUT "  Press ENTER to continue."; p$
END SUB

' === NEW SUB: add after SUB Pause() ===

REM =================================================================
REM  SUB PrintRoom -- roomId AS INTEGER
REM  Selects and prints the room description for the given room.
REM  Chooses first-visit or revisit text based on the visited flag.
REM  ROOM_ENTRY: Entry Hall. Additional rooms added in Issue 4.
REM  Note: visited flag is read here, set by EnterRoom after this call.
REM =================================================================
SUB PrintRoom(roomId AS INTEGER)
    IF roomId = ROOM_ENTRY THEN
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
        PRINT "  Exits: SOUTH"
    END IF
END SUB

' === NEW SUB: add after SUB PrintRoom() ===

REM =================================================================
REM  SUB EnterRoom -- roomId AS INTEGER
REM  Renders a full room screen: header, location line, description,
REM  separator. Sets the visited flag for the room after rendering.
REM  Room name lookup: ROOM_ENTRY = Entry Hall. Extended in Issue 4.
REM =================================================================
SUB EnterRoom(roomId AS INTEGER)
    CALL PrintHeader()
    CALL FlushFlavour()
    IF roomId = ROOM_ENTRY THEN
        PRINT "  LOCATION: Entry Hall"
    END IF
    PRINT ""
    CALL PrintRoom(roomId)
    PRINT ""
    CALL PrintSeparator()
    SET GLOBAL visited[roomId - 1] = 1
END SUB

' === REPLACE: the existing main program block at the bottom of the file ===
' Replace the four lines:
'   CALL PrintHeader()
'   PRINT ""
'   CALL PrintSeparator()
'   PRINT " > "
' With the block below:

REM =================================================================
REM  Main program
REM  EnterRoom renders the first room before the command loop starts.
REM  The loop reads and normalises input, then dispatches commands.
REM  GO SOUTH: prints the no-exit response (navigation not yet wired).
REM  All other input: prints the unknown command response.
REM =================================================================
CALL EnterRoom(ROOM_ENTRY)

WHILE gameOver = 0
    INPUT " > "; cmd$
    LET cmd$ = UPPER$(cmd$)
    IF cmd$ = "GO SOUTH" THEN
        PRINT "  There is no way through in that direction."
    ELSE
        PRINT "  The dungeon does not respond to that."
    END IF
    PRINT ""
WEND
```
