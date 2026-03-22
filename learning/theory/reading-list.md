# Reading List

Ordered by when to read it, not by importance. All of these are worth reading. The primary reference is non-negotiable.

---

## Primary reference

**Crafting Interpreters — Robert Nystrom**

The book that this project orbits. Part I and Part II (jlox) cover a tree-walking interpreter for a language called Lox — structurally similar to SharpBASIC but in Java. Part III (clox) implements a bytecode VM in C.

Chapters 1–4 give the big picture. Reading them before Phase 1 is worth the time even if you don't read another word until Phase 4.

The physical book is the recommended format — the diagrams reproduce well in print and it is the kind of book you will return to. The full text is available free online at craftinginterpreters.com.

---

## Phases 1–3

**"Let's Build a Simple Interpreter" — Ruslan Spivak**  
ruslanspivak.com — a step-by-step series in Python. The concepts translate directly to C#. Good supplementary reading for the lexer and early parser phases if the book's approach doesn't immediately click.

**The Super Tiny Compiler — James Kyle**  
github.com/jamiebuilds/the-super-tiny-compiler  
A minimal compiler in ~200 lines of JavaScript. Useful for seeing the full pipeline in a single file, which helps when the pipeline is spread across five C# projects.

**Computerphile — Parsing videos**  
YouTube. Search "Computerphile compiler" or "Computerphile parsing". Good visual intuition for what the parser is doing before the code makes it concrete.

---

## Phase 4 — Pratt parsing

**Crafting Interpreters — Chapter 6**  
Nystrom's treatment of Pratt parsing. Read `theory/pratt-parsing.md` first, then Chapter 6 for the full account.

**"Pratt Parsers: Expression Parsing Made Easy" — Bob Nystrom**  
journal.stuffwithstuff.com  
A shorter, standalone article by the same author. More directly applicable than the book chapter for a first implementation. Recommended.

---

## General compiler theory

**Roslyn source — github.com/dotnet/roslyn**  
The C# compiler itself, written in C#. Too large to study directly, but useful for error handling and AST design inspiration in the later phases. The diagnostic model in particular is worth looking at before Phase 9.

---

## What comes after SharpBASIC

The natural next step is Part III of *Crafting Interpreters* — implementing clox, a bytecode VM for Lox, in C. This is a different discipline from the tree-walking interpreter: manual memory management, an explicit value stack, call frames as data structures, and a garbage collector.

clox is worth implementing fully — not reading and skimming, but typing every line. The concepts it builds (stack-based VM, bytecode compilation, closures, GC) are the foundation of the Grob project. Working through clox before designing Grob means every key architectural decision in Grob's VM will be informed by having built one already.

After clox, if systems programming appeals: a bare-metal x86 kernel in C and NASM assembly is a coherent next project. The mental models — stack, instruction pointer, memory addresses — transfer directly from the VM work.
