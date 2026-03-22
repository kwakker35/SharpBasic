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
- `SUB PrintHeader()` — draws the title bar and stats line
- `SUB PrintSeparator()` — draws the dividing line
- Placeholder stats so the frame has something to display
- The `>` prompt

This issue introduces the frame that every subsequent issue will use. Get it right here and you won't need to touch it again.

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
LET sep = ""
FOR i = 1 TO 80
    LET sep = sep & "="
NEXT i
PRINT sep
```

SharpBASIC has no string repeat function. So you build one. Start with an empty string. Loop eighty times. Add one `=` on each pass. At the end of the loop, `sep` holds eighty equals signs. Print it.

This loop pattern — building a string character by character — is used throughout the game wherever a fixed-width line is needed. Learn its shape here.

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

```
' Issue 1 listing — to be added once built and tested
```
