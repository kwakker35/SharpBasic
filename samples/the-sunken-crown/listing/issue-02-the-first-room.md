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

This issue does not wire navigation to a second room. GO SOUTH is recognised but goes nowhere yet. That comes in Issue 4.

---

## Concept — SUBs

The room description is printed by a SUB — a named block of code you can call by name from anywhere in the program.

```
SUB PrintRoom(roomId AS INTEGER)
    IF roomId = 1 THEN
        PRINT "  The hall is wide and low..."
    END IF
END SUB
```

Break this down:

- `SUB PrintRoom(roomId AS INTEGER)` — declares a subroutine named `PrintRoom` that takes one parameter, `roomId`, which must be an integer
- `IF roomId = 1 THEN` — checks which room we want
- `PRINT "  The hall is wide and low..."` — prints the description for that room
- `END IF` — closes the conditional
- `END SUB` — closes the subroutine

Call it with `CALL PrintRoom(1)` and the Entry Hall appears. Call it with `CALL PrintRoom(2)` and the Guardroom will appear — once we add that room in Issue 4.

The parameter `roomId` is typed as `INTEGER`. SharpBASIC requires typed parameters in every SUB and FUNCTION declaration. This is not optional. If you try to pass a string where an integer is expected, the interpreter will tell you.

SUBs are the architecture the entire game is built on. Every room, every combat frame, every inventory screen goes through a SUB. The pattern you learn here — declare it, give it a typed parameter, call it by name — is the same pattern used everywhere.

One important note: SharpBASIC hoists all SUB and FUNCTION declarations before running any top-level code. This means you can declare `PrintRoom` anywhere in the file and call it anywhere else, regardless of order. Convention is to put all SUBs together near the top of the file, above the main game loop. That is the pattern this game follows throughout.

---

## How It Fits

Issue 1 created `PrintHeader` and `PrintSeparator`. This issue calls them both — `PrintRoom` renders inside the frame that Issue 1 built.

The command loop introduced here is the seed of the main game loop that will run the entire game by Issue 10. It is simplified now — it only handles one command — but its shape is permanent. Every issue from here adds new commands to this loop without changing its structure.

---

## What You'll See

When you run the program after adding this issue's code, the Entry Hall renders inside the frame. The room description appears, the exits are listed, and the prompt waits. Type `GO SOUTH` and the program responds — it doesn't take you anywhere yet, but it recognises the command. Type anything else and the unknown command response appears.

If the room description does not appear, check that `CALL PrintRoom(1)` is being called and that `PrintRoom` has a branch for `roomId = 1`. If the visited text appears on first entry instead of the first-visit text, check the `visited[1]` flag — it should be 0 before `EnterRoom` sets it to 1.

---

## What to Try

Add a second room description — write the Guardroom text and add it as `IF roomId = 2 THEN` inside `PrintRoom`. Then call `CALL PrintRoom(2)` directly at the bottom of the file to see it render. The dungeon grows by one room.

This is exactly how every room in the game gets added. The pattern is always the same: a new `IF` branch, a new room number, the text from the content file. Twelve rooms means twelve branches. Understanding the pattern now means Issue 4 holds no surprises.

---

## The Listing

```
' Issue 2 listing — to be added once built and tested
```
