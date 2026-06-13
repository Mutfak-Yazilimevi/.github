---
name: low-level-design
description: "OOP, SOLID, DRY/KISS/YAGNI ilkeleriyle temiz nesne yönelimli bileşenler, sınıf diyagramları ve tasarım desenleri için kullan."
---

# Low-Level Design (LLD)

## Overview

Turn a requirement into a clean class-level design: the right objects, responsibilities,
relationships, and patterns — testable, extensible, and free of code smells. LLD sits below
system design (services/scaling) and above raw coding.

Work in this order; stop as soon as the design is clear — do not over-engineer.

## Workflow

1. **Clarify scope & constraints.** Inputs/outputs, core use cases, edge cases, expected
   read/write patterns, concurrency, and what is explicitly out of scope.
2. **Model the objects.** Identify entities (nouns), their attributes, and behaviors (verbs).
   Assign **one clear responsibility per class** (SRP). Prefer small, deep classes.
3. **Define relationships.** Composition vs inheritance (favor composition), interfaces for
   contracts, and dependency direction (depend on abstractions). Map interactions explicitly.
4. **Apply principles, then patterns.** Principles first; reach for a pattern only when it
   removes real duplication or change-friction — never to look clever.
5. **Handle errors & edges deliberately.** Decide failure modes up front (see Error handling).
6. **Refactor toward clarity.** Remove smells; verify each class is independently testable.

## OOP fundamentals

- **Encapsulation** — hide state behind behavior; expose intent, not fields.
- **Abstraction** — model the concept, not the implementation detail.
- **Inheritance** — only for genuine "is-a"; otherwise compose.
- **Polymorphism** — program to an interface so behavior varies without `if/switch` on type.

## SOLID

| | Principle | Litmus test |
| :- | :--- | :--- |
| **S** | Single Responsibility | "Does this class have more than one reason to change?" |
| **O** | Open/Closed | Add behavior by extension, not by editing existing tested code. |
| **L** | Liskov Substitution | Any subtype must be usable wherever its base is, without surprises. |
| **I** | Interface Segregation | Many small role-interfaces beat one fat interface. |
| **D** | Dependency Inversion | High-level policy depends on abstractions, not concretions. |

## Core principles

- **DRY** — one authoritative place for each piece of knowledge (not blind code de-dup).
- **KISS** — simplest design that satisfies the use cases; defer complexity.
- **YAGNI** — don't build for imagined future requirements.
- **Composition over inheritance** · **Separation of concerns** · **Law of Demeter**.

## Design patterns (select, don't collect)

- **Creational** — Factory / Abstract Factory, Builder, Singleton (sparingly), Prototype.
  *Use when* object construction is complex or must be decoupled from use.
- **Structural** — Adapter, Decorator, Facade, Composite, Proxy.
  *Use when* you need to compose or adapt structure without changing clients.
- **Behavioral** — Strategy, Observer, State, Command, Template Method, Iterator.
  *Use when* behavior varies or must be swapped/extended at runtime.

> Pattern selection: name the *force* (what changes, what must stay stable). If no force, no
> pattern. Strategy/State replace type-switching; Observer decouples producers from consumers;
> Decorator adds behavior without subclass explosion; Factory hides construction choices.

## Object modeling checklist

- Each class: one responsibility, a clear name, minimal public surface.
- Relationships explicit (composition/aggregation/association); no hidden coupling.
- Invariants enforced in the type, not by callers.
- Edge cases handled in the model (null/empty, limits, concurrent access).

## Error handling

- Use **custom exceptions** that carry meaning; don't leak primitives or generic errors.
- **Fail fast** on invalid state (guard clauses) — don't continue with bad data.
- **Graceful degradation** at boundaries; never swallow exceptions silently.
- Define a **logging strategy** (what, where, at which level) as part of the design.

## Refactoring (improve structure, not behavior)

Remove smells: long methods/classes, duplicated logic, poor names, deep nesting, feature
envy, primitive obsession. Improve readability, then optimize structure, then apply patterns —
in that order. Pair with the `code-refactorer-agent` agent for larger refactors.

## Practice problems (apply the workflow end-to-end)

Parking lot · Elevator system · ATM · Online booking / reservation system. For each: list use
cases → model objects & responsibilities → choose patterns (e.g., Strategy for pricing/State
for elevator) → handle concurrency and edge cases → sketch the class diagram.

## Gotchas

- **Pattern for its own sake** — applying a pattern with no real force adds indirection, not value.
- **Inheritance overuse** — deep hierarchies are fragile; prefer composition (LSP violations hide here).
- **God class / anemic model** — one class doing everything, or data bags with logic elsewhere.
- **Premature abstraction** — interfaces/factories before a second implementation exists (YAGNI).
- **Leaky encapsulation** — public setters/fields let callers break invariants.
- **Refactoring + behavior change at once** — keep them separate commits; tests must stay green.

## Related

- Diagram the result: `md-uml` (class/sequence diagrams).
- API/contract boundaries: `dev-api-and-interface-design`.
- Codebase-level structure: `dev-improve-codebase-architecture`.
- System/scaling level (above LLD): `dev-system-design-scaling`.
