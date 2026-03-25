# The Sunken Crown
## Issue 7 — The Dungeon Breathes

*The dungeon is not waiting for you.*

---

## The Story So Far

Issues 1 through 6 built a navigable dungeon with combat and inventory. The player has attributes, can fight, can pick up items, and can search rooms.

But the dungeon is still static between actions. Nothing happens unless the player makes it happen. The atmosphere exists only in the room descriptions. Between commands, the world holds its breath.

This issue changes that. The dungeon gets its own rhythms. Things happen whether the player is ready or not. And something starts moving through the passages.

---

## What's New This Issue

- `SUB AdvanceTurns(n AS INTEGER)` — central time management, fires per-turn events
- `SUB AtmosphericEvent()` — selects and prints one of 12 atmospheric events at random
- Wandering zombie — `zombieSpawned`, `zombieAlive`, `zombieRoom` state variables
- `SUB WanderZombie()` — moves the zombie one step via the exit arrays each turn
- Zombie encounter check added to `EnterRoom`
- Poison damage on room entry — fires before room description
- LOOK and SNEAK commands added to the command loop
- All remaining monsters wired to their rooms in the combat dispatcher
- All time-costing commands updated to call `AdvanceTurns` instead of incrementing directly

---

## Concept — The Turn Counter and RND-Driven Events

Every time-consuming action increments a global counter through `AdvanceTurns`:

```
LET turns = turns + 1
```

After each increment, the event system runs a probability check:

```
IF RND() * 10 > 8.5 THEN
    CALL AtmosphericEvent()
END IF
```

Break this down:

- `RND()` returns a value from 0.0 to just under 1.0
- Multiply by 10 to get a range from 0.0 to just under 10.0
- Compare to 8.5 — the result exceeds 8.5 roughly 15% of the time
- When it does, `AtmosphericEvent()` fires

15% is not every turn. It is not never. It is often enough to feel alive, irregular enough to feel unscripted. The dungeon breathes on its own schedule.

The zombie is a second system running on the same counter. Every turn after spawning, `WanderZombie()` runs. The zombie picks a random valid exit from its current room and moves there. It uses the same exit arrays the player uses — no separate pathfinding, no special logic. It wanders.

The encounter happens when the numbers align. Your position and the zombie's position happen to be the same room on the same turn. Neither system knows about the other. The emergent behaviour — the feeling that the dungeon is alive and something is moving through it — comes from two simple systems sharing one counter.

---

## How It Fits

`AdvanceTurns` replaces every bare `turns = turns + 1` from Issue 5 onward. Every command that costs turns — GO, FIGHT, LOOK, SEARCH, USE — now calls `AdvanceTurns` with the appropriate count. This is the single place where the dungeon's time system lives.

`EnterRoom` gains two new checks: poison damage fires at the top before the room description, and the zombie encounter check fires at the bottom after it. Both are additions to the existing SUB — its core structure does not change.

The atmospheric event text for all 12 events comes from the content asset file, Deliverable 3.

---

## What You'll See

Move through the dungeon. Atmospheric events start appearing between actions — the torch gutters, something scrapes in the dark below, the wall is warm when you touch it. They fire irregularly. No two runs feel quite the same.

After a few turns the zombie may spawn at the Crossroads. Move fast and you may never see it. Linger — searching rooms, fighting long battles — and eventually your paths cross. When they do, FIGHT and SNEAK both work against it.

Navigate to room 6 and fight the Skittering Horror. Win. Move to the next room. The poison message fires before the room description. Navigate to room 9 to find the Antidote Vial.

---

## What to Try

Change the atmospheric event threshold from `8.5` to `5`. Events fire roughly half the time now. Play through the first few rooms. Does the dungeon feel more alive or just noisier?

The threshold is a design decision as much as a code decision. 8.5 is right for this game — but understanding why requires knowing what the wrong numbers feel like. Change it back to 8.5 before Issue 8.

---

## The Listing

```
REM Issue 7 listing — to be added once built and tested
```
