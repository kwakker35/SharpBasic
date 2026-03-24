# The Sunken Crown — Issue 1 Generation Context
## The Gates Open

> Operational file. Not part of the repo. Not part of the learning path.
> Used by the code generator alongside the code generation prompt.
> Read the full code generation prompt before reading this file.

---

## Entry State

The `.sbx` file does not exist. Issue 1 creates it from scratch.

No prior variables, SUBs, or FUNCTIONs exist.

---

## Scope — What This Issue Adds

**Constants:**
```
CONST FRAME_WIDTH = 80
CONST SCREEN_HEIGHT = 30    ' total terminal rows
CONST CONTENT_ROWS = 20     ' rows available for content after chrome
```

**Global variables:**
```
LET skill = 0
LET stamina = 0
LET luck = 0
```

**SUBs:**
```
SUB PrintHeader()
SUB PrintSeparator()
```

**Top-level code:**
- Call `PrintHeader()`
- Print a blank line
- Call `PrintSeparator()`
- Print the prompt `" > "`

Nothing else. No INPUT loop. No commands. The program runs, prints the frame, and exits.

---

## Exit State

At the end of Issue 1 the complete `.sbx` file contains:
- One CONST declaration
- Three global LET declarations (skill, stamina, luck — all zero)
- Two SUB declarations
- Four lines of top-level code

Running the program produces exactly this output and exits:

```
================================================================================
  THE SUNKEN CROWN                           SKILL: 0  STAMINA: 0  LUCK: 0
================================================================================
--------------------------------------------------------------------------------
 >
```

The `=` separator is 80 characters. The `-` separator is 80 characters.
Title and stats are on one line between the two `=` separators.

---

## SET GLOBAL Audit

No SUBs in this issue write to global state. `PrintHeader` and `PrintSeparator`
are read-only display SUBs. No `SET GLOBAL` required in Issue 1.

Note: `PrintHeader` reads `skill`, `stamina`, and `luck` from the enclosing
scope via the parent chain — this is a read, not a write, and is correct.

---

## Text Strings Required

No game text from the content asset file is used in Issue 1. The header
and separator are UI chrome, not game text.

The title string `"THE SUNKEN CROWN"` and the stat labels `"SKILL: "`,
`"STAMINA: "`, `"LUCK: "` are functional UI strings, not content asset strings.
These may be written inline.

---

## Known Gotchas

**Building the separator string:**
`PrintSeparator` must build a string of 80 characters using a FOR loop.
SharpBASIC has no string repeat function. The pattern is:
```
LET sep = ""
FOR i = 1 TO FRAME_WIDTH
    LET sep = sep & "="
NEXT i
PRINT sep
```
`FRAME_WIDTH` is a CONST — do not use the literal `80` anywhere in the code.

**Header alignment:**
The stats line must be constructed with `&` concatenation. The spacing
between title and stats must produce a line that fits within 80 characters.
Verify the output alignment visually.

**CONST at top level only:**
`CONST FRAME_WIDTH = 80` must be declared at the top of the file, not
inside a SUB. CONST inside a SUB is a runtime error.

---

## Testing Checklist

- [ ] Program runs without error
- [ ] Output matches expected layout exactly
- [ ] Top separator is exactly 80 `=` characters
- [ ] Bottom separator is exactly 80 `-` characters
- [ ] Stats line shows `SKILL: 0  STAMINA: 0  LUCK: 0`
- [ ] Title `THE SUNKEN CROWN` appears on the stats line
- [ ] Prompt ` > ` appears on the last line
- [ ] Program exits cleanly after printing
- [ ] SCREEN_HEIGHT and CONTENT_ROWS constants declared and correct values

---

## Issue File Reference

`listing/issue-01-the-gates-open.md`

The listing in this file must be complete — it creates the `.sbx` file from
nothing. A reader who types this listing into a new file and runs it must
see the output described above.
