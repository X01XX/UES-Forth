Eating my own cooking, using my Memory Management code to implement an Unorthodox-Expert-System in a language that has limited support for memory management.
And I like Forth.

See the README.md for the forth-memory-management and Unorthodox-Expert-System projects, and theory.html.

As usual, rethinking everything.

To run:

> gforth

> include main.fs

> all-tests | main

A program that can learn (at a low level), avoid negative states, seek positive states, and run
on a Raspberry Pi class computer.

Understanding = ~A + ~B.

A is different from B in some way.
  
~A + ~B covers everything, with regions containing A, regions containing B, regions containing neither, NO regions containing A and B.
Similar calculations of other pairs, exhibiting the same difference, can be intersected for a finer understanding.
Everything intersecting everything, is still everything. The arrangement of the underlying regions changes.

The results are better with adjacent pairs, points on a logical <i>edge</i>.
The results are even better with multiple adjacencies, like (~A + ~B) & (~A + ~C), a logical <i>corner</i>.
The results are even better with a cluster of corners, like (~A + ~B) & (~A + ~C) & (~C + ~D).
The corners, above, are ~A + (~B & ~C), ~C + (~A & ~D).  I call A, and C, <i>anchors</i>.

In the results, regions not completely overlapped by other regions are <i>defining</i> regions, others are placeholders.
When well developed, all adjacent disimmilar samples tested, the anchor of a corner,
will be in only one region, a defining region, else it will not be a valid corner.
Each defining region will have a corner.  Having more than one corner is possible, but not needed.

Understanding is tenuous at first, then improves with more data, selected judiciously to find/test corners, like IRL.
