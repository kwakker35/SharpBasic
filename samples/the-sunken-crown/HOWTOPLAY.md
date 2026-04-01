# How to Play — The Sunken Crown

The Sunken Crown is a text adventure in the tradition of the Fighting Fantasy gamebook series. You type commands. The dungeon responds. You will probably die on your first run. That is expected. The dungeon is learned, not solved.

---

## Starting the Game

Run the game from the command line:

```
sharpbasic run the-sunken-crown.sbx
```

Before you enter the dungeon you will roll three attributes. These are generated randomly — you have no control over them. They represent what you bring in. What you do with them is up to you.

---

## Your Attributes

**SKILL**
Your combat effectiveness. A higher SKILL means your attacks land harder and more often. It is modified temporarily by certain encounters and items. It does not recover during a run.

**STAMINA**
Your hit points. Every time something hurts you, STAMINA falls. If it reaches zero, you are dead. A Healing Potion can restore it — but only up to your starting value, not beyond it. Manage it carefully.

**LUCK**
A resource, not a stat. Every time you test your luck, it decrements by one regardless of the outcome. The more you lean on it, the less of it you have. Use it when it matters.

---

## Commands

Type commands in plain English. The game understands a fixed vocabulary — if it doesn't recognise what you typed, it will tell you. Commands are not case sensitive.

**Moving**
```
GO NORTH
GO SOUTH
GO EAST
GO WEST
GO NE
```
Movement takes one turn. The dungeon does not pause while you move.

**Looking and searching**
```
LOOK
```
Redescribes the current room. Takes one turn.

```
SEARCH
```
Physically goes through the room looking for anything hidden or left behind. Takes two turns. More thorough than LOOK — but the dungeon keeps moving while your back is turned.

**Combat**
```
FIGHT
```
Engage whatever is in the room with you. Combat runs automatically round by round until one side falls. Each round takes one turn.

```
SNEAK NORTH
SNEAK SOUTH
SNEAK EAST
SNEAK WEST
SNEAK NE
```
Attempt to slip past a creature without fighting. Success depends on your SKILL. If you fail, you fight regardless. Sneaking past an enemy means you will not find anything they were carrying. You must SNEAK in a specific direction.

**Items**
```
TAKE [item]
```
Pick up a specific item. Free — takes no turns.

```
TAKE ALL
```
Pick up everything in the room up to your carry limit. Free — takes no turns.

```
DROP [item]
```
Leave an item in the current room. It will still be there if you return. Free — takes no turns.

```
USE [item]
```
Use an item from your inventory. Takes one turn.

**Information**
```
INVENTORY
```
Shows everything you are currently carrying and your carry status. Takes no turns.

```
STATS
```
Shows your current SKILL, STAMINA, and LUCK. Takes no turns.

```
LUCK
```
Tests your luck in situations where the outcome is uncertain. Takes one turn. Decrements your LUCK by one regardless of the result.

```
HELP
```
Lists available commands. Takes no turns.

```
QUIT
```
Quits the current run through without confirmation.

---

## Inventory

You can carry up to three items comfortably. You can carry a fourth, but you will feel it.

**Overburdened** — carrying four items — has consequences:

- Movement costs 1 STAMINA per room
- Damage taken in combat increases

All penalties lift the moment you drop back to three items. There is no penalty for dropping items — only for carrying too many.

You cannot carry a fifth item. If you try, the game will tell you what you could not take. Use `TAKE [item]` to choose selectively.

---

## Combat

When you fight, both you and your opponent roll for attack strength each round. The higher roll wins the round. The loser takes damage. Ties result in no damage to either side.

Your attack strength is your SKILL plus two dice. Your opponent's is their SKILL plus two dice. Higher SKILL wins more often — but the dice always have a say.

Some enemies have special properties. You will discover what these are when you encounter them.

Combat ends when either side reaches zero STAMINA. If yours reaches zero, the run is over.

---

## Luck

Testing your luck costs one point of LUCK whether you succeed or fail. A lucky outcome is better than an unlucky one. What constitutes better or worse depends on the situation.

Your LUCK starts between 7 and 12. Every test brings it closer to zero. At zero, luck tests will always fail. The dungeon will still ask you to make them.

Use luck deliberately. Do not waste it.

---

## Turns

Most actions advance time by one or more turns. The dungeon is not static — things move and happen while you are standing still, searching rooms, or fighting. Long fights and thorough searches give the dungeon more time to act.

The turn counter is not displayed. You will feel its effects before you understand its mechanism.

---

## Death

There is no save function. One life, one run. If you die, you start again from the beginning with a fresh set of attribute rolls.

This is intentional. The dungeon is learned across runs, not within them. A death is information.

---

## A Few Things Worth Knowing

The dungeon is not fair. It was not designed to be. It was designed to be survivable — by someone paying attention.

SEARCH costs time. Everything that costs time has consequences you may not see immediately.

Items have weight in every sense. What you carry in determines what you can carry out.

The dungeon's exits do not always behave the way you expect. This is not a bug.

When in doubt, read the room description carefully. The dungeon does not lie. It simply does not volunteer everything at once.

---

*Good luck. You will need some of it.*
