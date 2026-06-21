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

Some operations, like depth-first plan searches, and getting needs for each action, can be run in parallel, with no mutex,
although UES-Forth is not doing that. The UES Rust project does.

Showing the power of roll-your-own Memory Management, I added an allocation counter, an hour after having the idea.
It turns out UES-Forth does literally Millions of struct Allocation/Deallocation operations, with no apparent slowness.
At end, everything is accounted for, no memory leaks, nothing left on the Forth stack.

<pre>
Understanding = ~A + ~B

A is different from B in some way.
  
~A + ~B covers everything, with regions containing A, regions containing B,
regions containing neither, NO regions containing A and B.
Avoiding regions that contain dissimilar things, forms regions that contain similar things.

Your initial thought may be that this just creates sets of similar things.
But a set of similar things may be divided into more than one region, depending on what is
to be avoided.

Similar calculations of other pairs, exhibiting the same difference, can be intersected for
an improved understanding.

Your initial thought may be that multiple intersections produces smaller, and smaller, results.
But everything intersecting everything, is still everything.
The arrangement of the underlying regions changes.

Commonly, there are a lot of overlaps between regions containing dissimilar things,
where there is no data.
Overlaps of dissimilar regions are a measure of uncertainty.
You could seek data in the dissimilar overlapped areas, producing smaller, more numerous, overlaps.
Thats ineffective.

The results are better with adjacent pairs, points on a logical <i>Edge</i>.
The results are even better with multiple adjacencies, like (~A + ~B) & (~A + ~C), a logical <i>Corner</i>.
The results are even better with a cluster of corners, like (~A + ~B) & (~A + ~C) & (~C + ~D).
"Better" can be defined as fewer dissimilar overlaps, less uncertainty.

The corners, above, are ~A + (~B & ~C), ~C + (~A & ~D).  I call A, and C, <i>Anchors</i>.
Note that I added a corner by adding a single item, ~D. Corner clusters share data.
The number of adjacent, dissimilar, terms used for a corner, equals the number of edges of
the region the anchor is in.  

In the results, regions not completely overlapped by other regions are <i>Defining</i> regions,
others are <i>Placeholders</i>.
Placeholder regions are caused by adjacent, similar, regions, not to be confused with the
overlaps mentioned above.

When a corner is well developed, all adjacent dissimilar samples tested, the anchor
will be in only one region, a defining region, else it will not be a valid corner.
Each defining region will have a corner.  Having more than one corner is likely, but not needed.

One of a region's corners may be better to keep than another, to be part of a corner cluster.
Placeholder regions suppress anchors in overlapped parts of defining regions, since an anchor
cannot be in more than one region.

After corners are established and tested, accumulated data, not part of the final corners,
can be deleted.
Corner clusters are the most data-efficient way to describe the regions, based on my experience.
There may be an opportunity for a mathematical proof here, but I don't have that talent.
Corner clusters, sharing data, can go as low as one term per region, depending on how regularly the
regions are found to be arranged.

Understanding is tenuous at first, then improves with more data, selected to find/test corners,
like IRL.
Poor selection of data (like me, doing this), or inability to select (like a baby, watching),
can still result in improved understanding. It requires more time, data input, and data storage,
to haphazardly discover corners.

The program starts in the inability-to-select mode, taking samples as available.
After 75, or so, samples (you can lean on the Enter key), a few short, single-step,
plans become possible to start to control selection.
Early plans have a tendency to fail, but failure provides data, understanding improves.

With more actively selected samples, corners are found.
Plans become easier to find, more steps per plan, more likely to succeed.
Corner clusters are calculated.
The corner cluster algorithm can be slowed by too much data, maybe there's a better way?
Excess data is then deleted.
</pre>
