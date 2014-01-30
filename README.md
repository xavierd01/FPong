FPong
=====

Classic Pong using F#

In my attempt to learn Functional Programming in general (F# in particular) and start a new hobby of game programming, I have created a Pong clone using F#. I have tried to stay as "functional" as possible; limiting my use of mutable variables, etc.; using the following from http://fsharpforfunandprofit.com/learning-fsharp/ as a guide.

Dos and Don'ts
Here is a list of dos and don'ts that will encourage you to think functionally. These will be hard at first, but just like learning a foreign language, you have to dive in and force yourself to speak like the locals.

Don't use the mutable keyword at all as a beginner. Coding complex functions without the crutch of mutable state will really force you to understand the functional paradigm.
Don't use for loops or if-then-else. Use pattern matching for testing booleans and recursing through lists.
Don't use "dot notation". Instead of "dotting into" objects, try to use functions for everything. That is, write String.length "hello" rather than "hello".Length. It might seem like extra work, but this way of working is essential when using pipes and higher order functions like List.map. And don't write your own methods either! See this post for details.
As a corollary, don't create classes. Use only the pure F# types such as tuples, records and unions.
Don't use the debugger. If you have relied on the debugger to find and fix incorrect code, you will get a nasty shock. In F#, you will probably not get that far, because the compiler is so much stricter in many ways. And of course, there is no tool to “debug” the compiler and step through its processing. The best tool for debugging compiler errors is your brain, and F# forces you to use it!
On the other hand:

Do create lots of "little types", especially union types. They are lightweight and easy, and their use will help document your domain model and ensure correctness.
Do understand the list and seq types and their associated library modules. Functions like List.fold and List.map are very powerful. Once you understand how to use them, you will be well on your way to understanding higher order functions in general.
Once you understand the collection modules, try to avoid recursion. Recursion can be error prone, and it can be hard to make sure that it is properly tail-recursive. When you use List.fold, you can never have that problem.
Do use pipe (|>) and composition (>>) as much as you can. This style is much more idiomatic than nested function calls like f(g(x))
Do understand how partial application works, and try to become comfortable with point-free (tacit) style.
Do develop code incrementally, using the interactive window to test code fragments. If you blindly create lots of code and then try to compile it all at once, you may end up with many painful and hard-to-debug compilation errors.

