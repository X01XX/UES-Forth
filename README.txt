
Start forth: > gforth

then: include main.fs

then: all-tests

Build one thing upon another:

1) A stand-alone stack (SAS), with an array of cells for stack items,
   and a cell for storing the stack Pointer.

2) A Memory Management array (MMA), with an array of potential struct instances,
   and a SAS to hold addresses of the array items.

3) A link struct, holding data and next-link cells, using a MMA.

4) A list struct, holding a link cell, using a MMA, and links.
   Enables an empty, no-link, list.

5) All sorts of lists, of various struct instances.

Features.

    A Region is an array of two cells, instead of a list\vector.

    States and masks are just an unsigned number, with an upper limit based on the number of bits in use.

    A RegionStore is an array of two cells, instead of a list/vector.
    The second cell may be zeroed out.

