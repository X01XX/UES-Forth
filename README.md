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

Showing the power of roll-your-own Memory Management, I added an allocation counter, an hour after having the idea.
It turns out UES-Forth does literally Millions of struct Allocations/Deallocations, with no apparent slowness.
At end, everything is accounted for, no memory leaks, nothing left on the Forth stack.

<pre>
Understanding = ~A + ~B

A is different from B in some way.
  
~A + ~B covers everything, with regions containing A, regions containing B, regions containing neither,
NO regions containing A and B.
Similar calculations of other pairs, exhibiting the same difference, can be intersected for a finer understanding.
Everything intersecting everything, is still everything. The arrangement of the underlying regions changes.

The results are better with adjacent pairs, points on a logical <i>Edge</i>.
The results are even better with multiple adjacencies, like (~A + ~B) & (~A + ~C), a logical <i>Corner</i>.
The results are even better with a cluster of corners, like (~A + ~B) & (~A + ~C) & (~C + ~D).
The corners, above, are ~A + (~B & ~C), ~C + (~A & ~D).  I call A, and C, <i>Anchors</i>.
The number of adjacent, dissimilar, terms used for a corner, equals the number of edges of the region the anchor is in.  

In the results, regions not completely overlapped by other regions are <i>Defining</i> regions, others are <i>Placeholders</i>.
When a corner is well developed, all adjacent dissimilar samples tested, the anchor
will be in only one region, a defining region, else it will not be a valid corner.
Each defining region will have a corner.  Having more than one corner is possible, but not needed.

One of a region's corners may be better to keep than another, to be part of a corner cluster.
Placeholder regions suppress anchors in overlapped parts of defining regions, since an anchor cannot be in more than one region.

After corners are established and tested, some data accumulated in the process can be deleted.
The corners are the most data-efficent way to describe the regions, based on my experience.
There may be an opportunity for a mathematical proof here, but I don't have that talent.

Understanding is tenuous at first, then improves with more data, selected judiciously to find/test corners, like IRL. 
</pre>
