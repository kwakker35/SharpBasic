# The Sunken Crown

![The Bound King on his throne](IMG_2957.png)

*A text adventure for one. Written in SharpBASIC v1.*

---

You have sold everything. Your home, your tools, what little remained after the debts. Malachar's men took it all — converted to coin, dropped down a chute into the dark beneath the keep. You watched it go.

If you don't walk out of that dungeon, there is nothing to walk back to.

---

## Running the Game

```
sharpbasic the-sunken-crown.sbx
```

No installation. No dependencies. One file.

---

## What This Is

The Sunken Crown is a Fighting Fantasy-inspired text adventure written entirely in SharpBASIC v1 — a BASIC tree-walking interpreter built from scratch as a learning project.

It is a dungeon crawl with twelve rooms, six monsters, a wandering zombie, a loot system, and a final boss who cannot die. It has attributes, combat, luck tests, traps, riddles, and one choice at the end that the dungeon has been building toward since the gates closed.

It was written to demonstrate that SharpBASIC v1 is a real, usable language capable of expressing something genuinely fun.

---

## Files

| File | Contents |
|------|----------|
| `the-sunken-crown.sbx` | The complete game — run this |
| [HOWTOPLAY.md](HOWTOPLAY.md) | Player guide — commands, combat, inventory, rules |
| [LORE.md](LORE.md) | Background on the world and the trial. No spoilers. |
| [LORE-SPOILERS.md](LORE-SPOILERS.md) | The full story. Finish the game first. |
| [listing/](listing/) | The game in 10 issues — build it yourself, step by step |

---

## The 10-Issue Listing Series

The `listing/` folder contains the game broken into 10 issues in the tradition of the INPUT magazine partworks of the 1980s. Each issue explains one SharpBASIC feature, presents a typeable listing, and leaves you with something that runs.

| Issue | Title | Feature |
|-------|-------|---------|
| 1 | [The Gates Open](listing/issue-01-the-gates-open.md) | PRINT and string concatenation |
| 2 | [The First Room](listing/issue-02-the-first-room.md) | SUBs |
| 3 | [The Dice and the Dark](listing/issue-03-the-dice-and-the-dark.md) | FUNCTIONs and RND |
| 4 | [Moving Through Stone](listing/issue-04-moving-through-stone.md) | Arrays |
| 5 | [Something in the Dark](listing/issue-05-something-in-the-dark.md) | WHILE loops |
| 6 | [What You Carry](listing/issue-06-what-you-carry.md) | Parallel arrays and item tracking |
| 7 | [The Dungeon Breathes](listing/issue-07-the-dungeon-breathes.md) | Turn counter and RND-driven events |
| 8 | [Traps and Riddles](listing/issue-08-traps-and-riddles.md) | IF / ELSE chains and INPUT |
| 9 | [The Bound King](listing/issue-09-the-bound-king.md) | Nested loops and state flags |
| 10 | [The Gate](listing/issue-10-the-gate.md) | Putting it together |

---

## About SharpBASIC

SharpBASIC is a modern BASIC-inspired tree-walking interpreter built in C# .NET 10. It was written from scratch as a deliberate learning project — ten phases, fixed scope, TDD throughout. The Sunken Crown was written against the completed v1 language.

The source code for SharpBASIC is available in the root of this repository.

---

*Few who enter the Sunken Crown return. None speak of what waits inside. You will have one chance.*