\ RLC intersection pair struct and functions.
\
\ Two rlcs, from a rlc-list in session-rlclist-by-rate.

#47317 constant rlcintpair-id
    #4 constant rlcintpair-struct-number-cells

\ Struct fields.
0                             constant rlcintpair-header-disp       \ 16 -bits [0] struct id, [1] use count.
rlcintpair-header-disp  cell+ constant rlcintpair-rlc-0-disp        \ First RLC.
rlcintpair-rlc-0-disp   cell+ constant rlcintpair-rlc-1-disp        \ Second RLC,
rlcintpair-rlc-1-disp   cell+ constant rlcintpair-intersection-disp \ Intersection RLC.

0 value rlcintpair-mma \ Storage for rlcintpair mma instance.

\ Init rlcintpair mma, return the addr of allocated memory.
: rlcintpair-mma-init ( num-items -- ) \ sets rlcintpair-mma.
    dup 1 <
    abort" rlcintpair-mma-init: Invalid number of items."

    cr ." Initializing Rlcintpair store."
    rlcintpair-struct-number-cells swap mma-new to rlcintpair-mma
;

\ Check rlcintpair mma usage.
: assert-rlcintpair-mma-none-in-use ( -- )
    rlcintpair-mma mma-in-use 0<>
    abort" rlcintpair-mma use GT 0"
;

\ Check instance type.
: is-allocated-rlcintpair ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup rlcintpair-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    rlcintpair-id =
;

\ Check TOS for rlcintpair, unconventional, leaves stack unchanged.
: assert-tos-is-rlcintpair ( arg0 -- arg0 )
    dup is-allocated-rlcintpair
    is-false if
        s" TOS is not an allocated rlcintpair"
        .abort-xt execute
    then
;

\ Check NOS for rlcintpair, unconventional, leaves stack unchanged.
: assert-nos-is-rlcintpair ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-rlcintpair
    is-false if
        s" NOS is not an allocated rlcintpair"
        .abort-xt execute
    then
;

\ Check 3OS for rlcintpair, unconventional, leaves stack unchanged.
: assert-3os-is-rlcintpair ( arg2 arg1 arg0 -- arg1 arg0 )
    #2 pick is-allocated-rlcintpair
    is-false if
        s" 3OS is not an allocated rlcintpair"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the rlc-0 field from a rlcintpair instance.
: rlcintpair-get-rlc-0 ( addr -- u)
    \ Check arg.
    assert-tos-is-rlcintpair

    rlcintpair-rlc-0-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Return the rlc-1 field from a rlcintpair instance.
: rlcintpair-get-rlc-1 ( addr -- u)
    \ Check arg.
    assert-tos-is-rlcintpair

    \ Get second state.
    rlcintpair-rlc-1-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Return the intersection field from a rlcintpair instance.
: rlcintpair-get-intersection ( addr -- )
    \ Check arg.
    assert-tos-is-rlcintpair

    rlcintpair-intersection-disp +  \ Add offset.
    @                               \ Get the field.
;

\ Set the rlc-0 field from a rlcintpair instance, use only in this file.
: _rlcintpair-set-rlc-0 ( rlc1 addr -- )
    \ Check args.
    assert-tos-is-rlcintpair
    assert-nos-is-list

    rlcintpair-rlc-0-disp + \ Add offset.
    !                       \ Set field.
;

\ Set the rlc-1 field from a rlcintpair instance, use only in this file.
: _rlcintpair-set-rlc-1 ( rlc1 addr -- )
    \ Check args.
    assert-tos-is-rlcintpair
    assert-nos-is-list

    rlcintpair-rlc-1-disp + \ Add offset.
    !                       \ Set field.
;

\ Set the intersection field from a rlcintpair instance, use only in this file.
: _rlcintpair-set-intersection ( rlc1 addr -- )
    \ Check args.
    assert-tos-is-rlcintpair
    assert-nos-is-list

    rlcintpair-intersection-disp +  \ Add offset.
    !                               \ Set the field.
;

\ End accessors.

\ Return a new rlcintpair instatnce, given two non-equal, intersecting rlcs.
: rlcintpair-new ( rlc1 rlc0 -- rip )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    over list-get-length
    over list-get-length
    <> abort" rlcintpair-new: rlc1 len ne rlc0 len?"

    2dup region-list-eq abort" rlcintpair-new: rlc equal?"

    2dup region-list-corr-intersection  \ rlc1 rlc0, rlc-int t | f
    0= abort" rlcintpair-new: rlc1 does not intersect rlc0?"

                                        \ rlc1 rlc0 rlc-int
    rlcintpair-mma mma-allocate         \ rlc1 rlc0 rlc-int ripx

    \ Store id.
    rlcintpair-id over                  \ rlc1 rlc0 rlc-int ripx id ripx
    struct-set-id                       \ rlc1 rlc0 rlc-int ripx

    \ Init use count.
    0 over struct-set-use-count         \ rlc1 rlc0 rlc-int ripx

    \ Store fields.
    tuck _rlcintpair-set-intersection   \ rlc1 rlc0 ripx
    tuck _rlcintpair-set-rlc-0          \ rlc1 ripx
    tuck _rlcintpair-set-rlc-1          \ ripx
;

\ Print a rlcint pair.
: .rlcintpair ( rip0 -- )
    \ Check arg.
    assert-tos-is-rlcintpair

    ." [ "
    dup rlcintpair-get-rlc-0 .region-list-corr
    space ." <-> "
    dup rlcintpair-get-intersection .region-list-corr
    space ." <-> "
    dup rlcintpair-get-rlc-1 .region-list-corr
    ." ]"
;

\ Deallocate a rlcintpair.
: rlcintpair-deallocate ( rs0 -- )
    \ Check args.
    assert-tos-is-rlcintpair

    dup struct-get-use-count      \ reg0 count

    #2 <
    if
        \ Deallocate/clear fields.
        dup rlcintpair-get-rlc-0
        dup
        if
            region-list-deallocate
        else
            drop
        then

        dup rlcintpair-get-intersection
        dup
        if
            region-list-deallocate
        else
            drop
        then

        dup rlcintpair-get-rlc-1
        dup
        if
            region-list-deallocate
        else
            drop
        then

        \ Deallocate instance.
        rlcintpair-mma mma-deallocate
    else
        struct-dec-use-count
    then
;
