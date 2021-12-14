# Command Design Pattern


## Summary
Games have many commands:
> Jump, Move, Throw_Ball

We can wrap these commands in a Command object.

When used, the Command object doesn't need to care about __HOW__ the command is executed


## When To Use
- Easier to `Rebind Keys`
- Easier to create a `Replay System`
- Easier to create an `Undo / Redo System`


## Implementation
Create a base class:
> Name it `Command`

> Use the `abstract` keyword since it's a base class

> Create an `abstract` method called `Execute`

> Create an `abstract` method called `Undo` if implementing `Undo / Redo System`

When creating child classes:
> Derive from the `Command` class

> `override` the `Execute` method, and define what actually happens when it's called

> `override` the `Undo` method, and perform the opposite of `Execute` if implementing `Undo / Redo System`

If implementing `Undo / Redo / Replay System`:
> Create 2 `Stack` data structures. These will act as our Undo / Redo Command storages

> Whenever a new key is pressed clear the Redo Stack and add to the Undo stack
