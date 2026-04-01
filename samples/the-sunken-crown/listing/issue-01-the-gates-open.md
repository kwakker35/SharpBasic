# The Sunken Crown
## Issue 1 — The Gates Open

*Before the dungeon has rooms or monsters or choices, it needs a face.*

---

## The Story So Far

There is no story yet. That is the point of this issue.

Before a single room is described, before a monster is named, before a choice is offered — the dungeon needs to look like something. A header. A separator. Numbers arranged on a screen in a way that says: this is real, this means something, pay attention.

The frame comes first. Everything else goes inside it.

This issue builds the frame. A handful of PRINT statements, carefully arranged, and suddenly there is a dungeon on the screen. It doesn't do anything yet. It doesn't need to. The feeling of it is enough to start.

---

## What's New This Issue

- The `.sbx` file is created for the first time
- `CONST FRAME_WIDTH = 80` — the terminal width, used throughout
- `CONST COMBAT_DELAY = 1500` — milliseconds between combat roll and outcome display
- `SUB PrintHeader()` — draws the title bar and stats line
- `SUB PrintSeparator()` — draws the dividing line
- `SUB QueueFlavour(line$)` — buffers a line of narrative text for display after the next header
- `SUB FlushFlavour()` — prints and clears the buffer
- `pendingFlavour[30]` array and `pendingFlavourCount` — the buffer backing store
- Placeholder stats so the frame has something to display
- The `>` prompt

This issue introduces the frame and the constants that every subsequent issue will use. Get them right here and you won't need to touch them again.

---

## Concept — PRINT and String Concatenation

SharpBASIC's `PRINT` statement is the language's only way to speak to the player. Everything the dungeon says — every room description, every combat result, every atmospheric whisper — comes through `PRINT`. There is no other output mechanism.

Here is the stats line that appears in the header:

```
PRINT "SKILL: " & skill & "  STAMINA: " & stamina & "  LUCK: " & luck
```

Break this down piece by piece:

- `"SKILL: "` — a fixed string literal, the label
- `& skill` — joins the current value of the `skill` variable to it
- `& "  STAMINA: "` — joins the next label
- `& stamina` — joins the current STAMINA value
- and so on

The `&` operator is SharpBASIC's string concatenation operator. It calls `.ToString()` on whatever it touches — so a number like `9` becomes the string `"9"` automatically. You never need to convert numbers to strings before printing them. The language handles it.

Now the separator line — eighty `=` characters across the full width of the terminal:

```
PRINT STRING$("=", FRAME_WIDTH)
```

`STRING$(char, count)` is SharpBASIC's string repeat function. `STRING$("=", 80)` produces eighty equals signs in a single call. No loop required. The same function is used later with `STRING$(" ", padLen)` to build right-aligned padding for the stats line.

Learn its shape here — `STRING$` appears throughout the game wherever a fixed-width line or dynamic padding is needed.

---

## How It Fits

This is Issue 1. There is nothing before it. The `.sbx` file does not exist yet — this issue creates it.

Every subsequent issue adds to what this issue establishes. The frame built here — `PrintHeader`, `PrintSeparator`, the `>` prompt — appears on screen in every subsequent issue unchanged. Build it once. Build it right.

---

## What You'll See

When you run the program after typing in this issue's listing, you should see exactly this on screen:

```
================================================================================
  THE SUNKEN CROWN                           SKILL: 0  STAMINA: 0  LUCK: 0
================================================================================

  >
```

The stats show zero because no attributes have been rolled yet — that comes in Issue 3. The frame is the point. If your output matches this layout, the issue is complete.

If the separator line is shorter or longer than the title bar, check your loop counter. If the stats line is misaligned, check your spacing inside the string literals.

---

## What to Try

Change the width of the frame. The listing assumes 80 characters — the classic terminal width. What happens if you make it 60? What breaks, and what do you need to fix to make it hold together at a different size?

Width is a design decision that affects every issue that follows. 80 is the right choice for this game. But understanding *why* it is the right choice — what breaks at 60, what looks wrong at 100 — is more valuable than just accepting the number.

---

## The Listing

> This is Issue 1. There is no prior code. Create a new file called
> `the-sunken-crown.sbx` and type in this listing in full.

```
REM =================================================================
REM  THE SUNKEN CROWN -- A SharpBASIC v1 Adventure
REM =================================================================
REM  Issue 1: The Gates Open
REM  Establishes the display frame -- header, separator, prompt.
REM  Everything that follows in subsequent issues goes inside it.
REM =================================================================

REM ----------------------------------------------------------------
REM  Constants
REM  FRAME_WIDTH governs every separator and alignment calculation.
REM ----------------------------------------------------------------
CONST FRAME_WIDTH = 80
CONST COMBAT_DELAY = 1500   REM milliseconds between attack roll and outcome

REM ----------------------------------------------------------------
REM  Player stats
REM  Declared here at zero. RollStartingStats (Issue 3) sets the
REM  real values when the opening sequence runs. PrintHeader reads
REM  these via the parent scope chain -- no SET GLOBAL required.
REM ----------------------------------------------------------------
LET skill = 0
LET stamina = 0
LET luck = 0

REM ----------------------------------------------------------------
REM  Pending flavour buffer
REM  Lines queued during movement and atmospheric events are
REM  flushed by EnterRoom after PrintHeader, keeping them visible
REM  between the header and the room description.
REM  pendingFlavourCount: number of lines currently buffered (0-29).
REM ----------------------------------------------------------------
DIM pendingFlavour[30] AS STRING
LET pendingFlavourCount = 0

REM =================================================================
REM  SUB PrintHeader
REM  Prints the full header frame: an = separator, then the title
REM  and current stats on one line, then another = separator.
REM  Reads skill, stamina, luck from global scope -- read-only.
REM =================================================================
SUB PrintHeader()
    CALL PrintSeparator()
    LET statsLine$ = "SKILL: " & skill & "  STAMINA: " & stamina & "  LUCK: " & luck
    LET padLen = FRAME_WIDTH - 18 - LEN(statsLine$)
    PRINT "  THE SUNKEN CROWN" & STRING$(" ", padLen) & statsLine$
    CALL PrintSeparator()
END SUB

REM =================================================================
REM  SUB PrintSeparator
REM  Prints a line of 80 = characters. Divides the header chrome
REM  from the room content area below it.
REM =================================================================
SUB PrintSeparator()
    PRINT STRING$("=", FRAME_WIDTH)
END SUB

REM =================================================================
REM  SUB QueueFlavour -- line$ AS STRING
REM  Appends one line to the pending flavour buffer. Pass "" for a
REM  blank line. If inCombat = 1, prints immediately instead so that
REM  combat-time events remain visible inline.
REM =================================================================
SUB QueueFlavour(line$ AS STRING)
    IF pendingFlavourCount < 30 THEN
        SET GLOBAL pendingFlavour[pendingFlavourCount] = line$
        SET GLOBAL pendingFlavourCount = pendingFlavourCount + 1
    END IF
END SUB

REM =================================================================
REM  SUB FlushFlavour
REM  Prints all buffered flavour lines then resets the buffer.
REM  Called by EnterRoom immediately after PrintHeader, and as a
REM  safety net before the command prompt for SEARCH / USE / LUCK.
REM =================================================================
SUB FlushFlavour()
    IF pendingFlavourCount > 0 THEN
        FOR i = 0 TO pendingFlavourCount - 1
            PRINT pendingFlavour[i]
        NEXT i
        SET GLOBAL pendingFlavourCount = 0
    END IF
END SUB

REM =================================================================
REM  Main program
REM =================================================================
CALL PrintHeader()
PRINT ""
CALL PrintSeparator()
PRINT " > "
```
