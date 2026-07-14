\ Implement a region struct and functions.
\
\ The region is a series of trits, representing a span of 2^N power squares,
\ in a K-Map of a number of bits, less the number of bits in a unsigned forth cell.
\
\ The bit limitation is for one Domain, but there can be a number of domains.
\
\ A number of trits equal to the number of bits in a whole forth cell may be possible,
\ but the sign bit may be a problem.
\
\ The region can do this with any two states/numbers. The states may be the same for a single-state
\ "region" in a K-Map.
\
\ In the action-incompatible-pairs list, regions are used as a two-state store, the states being not-equal.
\
\ Order of the states does not matter, although it can be seen in a printed region.
\ XxXx is state-0: 1010 and state-1: 0101.

#19317 constant region-struct-id
    #3 constant region-struct-number-cells

\ Struct fields
0                           constant region-header-disp   \ 16-bits [0] struct id, [1] use count, [2] Number bits ( 8 bits )
region-header-disp  cell+   constant region-state-0-disp  \ First state.
region-state-0-disp cell+   constant region-state-1-disp  \ Second state.

0 value region-mma \ Storage for region mma instance.

\ Init region mma, return the addr of allocated memory.
: region-mma-init ( num-items -- ) \ sets region-mma.
    dup 1 <
    abort" region-mma-init: Invalid number of items."

    cr ." Initializing Region store."
    region-struct-number-cells swap mma-new to region-mma
;

\ Check if tos is an allocated region.
: is-region? ( tos -- bool )
    dup region-mma mma-is-item? \ tos bool
    if
        struct-get-id           \ id
        region-struct-id =      \ bool
    else
        drop
        false                   \ f
    then
;

\ Start accessors.

\ Return the state-0 field from a region instance.
: region-get-state-0 ( reg0 -- sta0 )
    \ Check arg.
    assert( tos is-region? )

    region-state-0-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the state-1 field from a region instance.
: region-get-state-1 ( reg0 -- sta1 )
    \ Check arg.
    assert( tos is-region? )

    \ Get second state.
    region-state-1-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the state-0 field from a region instance, use only in this file.
: _region-set-state-0 ( sta1 reg0 -- )
    \ Check arg.
    assert( tos is-region? )

    region-state-0-disp +   \ Add offset.
    !                       \ Set the field.
;

\ Set the state-1 field from a region instance, use only in this file.
: _region-set-state-1 ( sta1 reg0 -- )
    \ Check arg.
    assert( tos is-region? )

    region-state-1-disp +   \ Add offset.
    !                       \ Set the field.
;

\ Get the number of bits.
: region-get-num-bits ( reg0 -- nb )
    \ Check arg.
    assert( tos is-region? )

    4c@
;

\ Set the number of bits.
: _region-set-num-bits ( nb reg0 -- )
    4c!
;

\ End accessors.

\ Create a region from two numbers on the stack, without checking their validity.
\ Split from region-new to allow adding an act0 in domain-new.
\ The order of the values will determine how an X bit is displayed, 1 0 = x, 0 1 = X.
: region-new2 ( u1 u0 nb -- reg )
    depth #3 < abort" Too few argements"

    \ Allocate space.
    region-struct-id region-mma
    struct-allocate             \ u1 u0 nb reg

    \ Set num bits.
    tuck _region-set-num-bits   \ u1 u0 reg

    \ Set state 0.
    tuck _region-set-state-0    \ u1 reg

    \ Set state 1.
    tuck _region-set-state-1    \ reg
;

\ Create a region from two numbers on the stack.
\ The numbers may be the same.
: region-new ( u1 u0 -- reg )
    \ Check args.
    assert( tos is-value? )
    assert( nos is-value? )

    \ Get current number bits, from current domain.
    current-num-bits-gbl    \ u1 u0 nb

    \ Make a new region without checking the values.
    region-new2
;

\ Return the two states that make a region.
: region-get-states ( reg0 -- sta1 sta0 )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    dup region-get-state-1  \ reg0 sta1
    swap                    \ sta1 reg0
    region-get-state-0      \ sta1 sta0
;

\ Print a region.
: .region ( reg0 -- )
    \ Check arg.
    assert( tos is-region? )

    \ Setup for trit-position loop.
    dup region-get-states           \ reg0 sta1 sta0
    rot region-get-num-bits         \ sta1 sta0 nb
    ms-bit                          \ sta1 sta0 ms-bit

    \ Print prefix.
    [char] r emit

    \ Process each trit.
    begin
      dup
    while
        \ Apply msb to state 1.
        #2 pick                     \ sta1 sta0 ms-bit sta1
        over                        \ sta1 sta0 ms-bit sta1 ms-bit
        and                         \ sta1 sta0 ms-bit sta1-bit

        \ Apply msb to state 0.
        #2 pick                     \ sta1 sta0 ms-bit sta1-bit sta0
        #2 pick                     \ sta1 sta0 ms-bit sta1-bit sta0 ms-bit
        and                         \ sta1 sta0 ms-bit sta1-bit sta0-bit


        if                          \ sta1 sta0 ms-bit sta1-bit
            if                      \ sta1 sta0 ms-bit
                ." 1"
            else
                ." X"
            then
        else                        \ sta1 sta0 ms-bit sta1-bit
            if                      \ sta1 sta0 ms-bit
                ." x"
            else
                ." 0"
            then
        then

        1 rshift                    \ sta1 sta0 ms-bit\2
    repeat
                                    \ st2 st1 ms-bit
    3drop
;

\ Return the highest state in a region.
: region-high-state ( reg0 -- n )
    \ Check arg.
    assert( tos is-region? )

    dup  region-get-state-0    \ reg0 sta0
    swap region-get-state-1    \ sta0 sta1
    or                         \ sta-high
;

\ Return the lowest state in a region.
: region-low-state ( reg0 -- n )
    \ Check arg.
    assert( tos is-region? )

    dup  region-get-state-0    \ reg0 sta0
    swap region-get-state-1    \ sta0 sta1
    and                        \ sta-low
;

\ Deallocate a region.
: region-deallocate ( reg0 -- )
    \ Check arg.
    assert( tos is-region? )

    dup struct-get-use-count      \ reg0 count
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Deallocate instance.
        region-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return a regions edge mask,
\ trits that are 0, or 1.
: region-edge-mask ( reg0 -- msk )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    region-get-states       \ s1 s0
    !nxor
;

\ Return true if two regions have a different number of bits.
: regions-dif-num-bits? ( reg1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )

    region-get-num-bits  \ reg1 nb0
    swap                 \ nb0 reg1
    region-get-num-bits  \ nb0 nb1
    <>
;

\ Return true if two regions intersect, no corresponding
\ trits are 0 and 1.
: region-intersects? ( reg1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"
    \ cr ." region-intersects: reg1: " over .region space ." reg0: " dup .region cr

    \ Get different bits mask of any pair states from reg1 and reg0.
    over region-get-state-0     \ reg1 reg0 reg1-sta0
    over region-get-state-0     \ reg1 reg0 reg1-sta0 reg0-sta0
    xor                         \ reg1 reg0 dif
    -rot                        \ sta-dif-msk reg1 reg2

    \ Get reg0 edge bits.
    region-edge-mask            \ sta-dif-msk reg1 reg0-edg

    \ Get reg1 edge bits.
    swap region-edge-mask       \ sta-dif-msk reg0-edg reg1-edg

    \ Get mask of same edge bits in both regions.
    and                         \ sta-dif-msk edge-msk

    \ Get different edge bit mask.
    and                         \ edge-dif-msk

    \ Return result
    0=
;

\ Return the region high state and low state.
: region-high-low ( reg0 -- high low )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    dup region-high-state   \ reg0 high
    swap region-low-state   \ high low
;

\ Return the intersection of two regions, or false if they do not intersect.
\ Since this must check for intersection first, there may be no need to check
\ for intersection before calling this.
: region-intersection ( reg1 reg0 -- reg t | f )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"

    \ Check that the two regions intersect.
    2dup region-intersects? \ reg1 reg0 bool
    if
        \ Save number bits.
        dup region-get-num-bits -rot    \ nb reg1 reg2

        \ Get high and low state of reg0
        region-high-low     \ nb reg1 reg0high reg0low

        \ Get high and low state of reg1
        rot                 \ nb reg0high reg0low reg1
        region-high-low     \ nb reg0high reg0low reg1high reg1low

        \ Group high/low states.
        rot                 \ nb reg0high reg1ghigh reg1low reg0low

        \ Calc lowest state.
        or                  \ nb reg0high reg1ghigh reg-low

        \ Calc highest state.
        -rot                \ nb reg-low reg0high reg1ghigh
        and                 \ nb reg-low reg-high

        \ Make new region, return.
        rot                 \ reg-low reg-high nb
        region-new2
        true
    else                    \ nb reg1 reg0
        2drop
        false
    then
;

\ Return the union of two regions.
: ?region-union ( reg1 reg0 -- reg3 )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"

    dup region-get-num-bits -rot    \ nb reg1 reg2

    \ Get high and low state of reg0
    region-high-low     \ nb reg1 reg0high reg0low

    \ Get high and low state of reg1
    rot                 \ nb reg0high reg0low reg1
    region-high-low     \ nb reg0high reg0low reg1high reg1low

    \ Group high/low states.
    rot                 \ nb reg0high reg1high reg1low reg0low

    \ Calc low state.
    and                 \ nb reg0high reg1high low2

    \ Calc high state.
    -rot                \ nb low2 reg0high reg1high
    or                  \ nb low2 high2
    rot                 \ low2 high2 nb

    region-new2
;

\ Return the union of a region and a state.
: region-union-state ( sta1 reg0 -- reg )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-value? )
    dup region-get-num-bits -rot \ nb sta1 reg0

    region-high-low         \ nb sta1 high low
    rot                     \ nb high low sta1
    tuck                    \ nb high sta1 low sta1

    \ Get new low state.
    and                     \ nb high sta1 low2

    \ Get new high state.
    -rot                    \ nb low2 high sta1
    or                      \ nb low2 high2
    rot                     \ low2 high2 nb

    region-new2
;

\ Return a regions X mask.
: region-x-mask ( reg0 -- mask )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    region-get-states   \ sta1 sta0
    xor
;

\ Return a region's edge 1 mask.
: region-edge-1-mask ( reg0 -- mask )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    region-get-states   \ sta1 sta0
    and
;

\ Return a region's edge 0 mask.
: region-edge-0-mask ( reg0 -- mask )
    \ Check arg.
    assert( tos is-region? )

    \ Calc result.
    region-get-states   \ sta1 sta0
    !nor
;

\ Return a new region with some X positions set to zero.
\ Change 1-0 or 0-1 to 0-0.
\ Mask will usually have a single bit, called from region-subtract.
: region-x-to-0 ( to-0-msk reg0 -- reg )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-value? )

    \ Save number bits.
    dup region-get-num-bits -rot    \ nb to-0-msk reg0

    region-get-states       \ nb to-0-msk sta1 sta0
    rot !not                \ nb sta1 sta0 keep-mask
    tuck                    \ nb sta1 keep sta0 keep
    and                     \ nb sta1 keep sta0-new
    -rot                    \ nb sta0-new sta1 keep
    and                     \ nb sta0-new sta1-new
    swap                    \ nb sta1-new sta0-new
    rot                     \ sta1-new sta0-new nb
    region-new2             \ reg
;

\ Return a new region with some X positions set to one.
\ Change 1-0 or 0-1 to 1-1.
\ Mask will usually have a single bit, called from region-subtract.
: region-x-to-1 ( to-1-msk reg0 -- reg )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-value? )

    \ Save number bits.
    dup region-get-num-bits -rot    \ nb to-1-msk reg0

    region-get-states       \ nb to-1-msk sta1 sta0
    rot                     \ nb sta1 sta0 to-1-msk
    tuck                    \ nb sta1 to-1-msk sta0 to-1-msk
    or                      \ nb sta1 to-1-msk sta0-new
    -rot                    \ nb sta0-new sta1 to-1-msk
    or                      \ nb sta0-new sta1-new
    swap                    \ nb sta1-new sta0-new
    rot                     \ sta1-new sta0-new nb
    region-new2             \ reg
;

\ Return true if two regions are equal.
: regions-eq? ( reg1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"

    \ Check address.
    2dup =                  \ reg1 reg0 bool
    if
        2drop
        true
        exit
    then

    over region-high-state  \ reg1 reg0 reg1-h
    over region-high-state  \ reg1 reg0 reg1-h reg0-h
    <>                      \ reg1 reg0 bool
    if
        2drop
        false
        exit
    then

    region-low-state        \ reg1 reg0-low
    swap region-low-state   \ reg0-low reg1-low
    =                       \ bool
;

\ Return true if regions are not equal.
: region-neq? ( reg1 rge0 -- bool )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"

    regions-eq?
    false?
;

\ Return true if a TOS region is a superset of the NOS region.
: region-superset? ( reg1 reg-sup -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"

    2dup region-intersects?         \ reg1 reg-sup flag
    if
        \ Regions intersect.
        over region-intersection    \ reg1 reg-int flag
        0= abort" region-superset-of: reg-sup and reg1 should intersect"
                                    \ reg1 reg-int
        tuck regions-eq?            \ reg-int flag
        swap region-deallocate      \ flag
    else
        \ Regions do not intersect, return false.
        2drop
        false
    then
;

\ Return true if a TOS region is a subset of the NOS region.
: region-subset? ( reg1 reg-sub -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"

    2dup region-intersects?         \ reg1 reg-sub flag
    if
        \ Regions intersect.
        tuck                        \ reg-sub reg1 reg-sub
        region-intersection         \ reg-sub reg-int flag
        0= abort" region-subset-of: reg-sub and reg1 should intersect"
                                    \ reg-sub reg-int'
        tuck regions-eq?            \ reg-int' flag
        swap region-deallocate      \ flag
    else
        \ Regions do not intersect, return false.
        2drop
        false
    then
;

\ Return true if a TOS region is a superset of the NOS state.
: region-superset-of-state? ( sta1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-value? )

    region-get-states           \ sta1 reg-sta1 reg-sta0
    rot                         \ reg-sta1 reg-sta0 sta1
    tuck                        \ reg-sta1 sta1 reg-sta0 sta1
    xor                         \ reg-sta1 sta1 diff0
    -rot                        \ diff0 reg-sta1 sta1
    xor                         \ diff0 diff1
    and                         \ both-diff
    0=                          \ flag
;

\ Return true if a region uses a given state.
: region-uses-state? ( sta1 reg0 -- flag )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-value? )

    region-get-states           \ sta1 reg-sta1 reg-sta0
    #2 pick                     \ sta1 reg-sta1 reg-sta0 sta1
    =                           \ sta1 reg-sta1 flag
    if                          \ sta1 reg-sta1
        2drop
        true
        exit
    then

                                \ sta1 reg-sta1
    =                           \ flag
;

\ Get a region from a string.
\ Valid chars are 0, 1, X, x, and underscore as separator.
\ All bit positions must be specified.
: region-from-string ( c-addr u --  reg t | f)
    \ cr ." region-from-string: " 2dup type cr

    \ Check for prefix.
    over c@ [char] r <>
    if
        2drop
        false
        exit
    then

    \ Inc address.
    swap 1+ swap

    \ Dec len.
    1-

    \ Init character counter.
    0 swap              \ c-addr cnt u

    \ Init state 1, state 0, and do initial value.
    0 swap              \ c-addr cnt sta1 u
    0 swap              \ c-addr cnt sta1 sta0 u
    0                   \ c-addr cnt sta1 sta0 u 0

    \ For each character...
    do                  \ c-addr cnt sta1 sta0
        \ Get a character.
        #3 pick         \ c-addr cnt sta1 sta0 c-addr
        i +             \ c-addr cnt sta1 sta0 c-addr+
        c@              \ c-addr cnt sta1 sta0 chr

        \ Process character.
        case
            [char] 0 of
                        \ Leave bit positions as 0/0.
                        \ Update sta1
                        swap 1 lshift
                        \ Update sta0
                        swap 1 lshift
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] 1 of
                        \ Set bit positions to 1/1.
                        \ Update sta1
                        swap 1 lshift 1+
                        \ Update sta0
                        swap 1 lshift 1+
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] X of
                        \ Set bit positions to 1/0.
                        \ Update sta1
                        swap 1 lshift
                        \ Update sta0
                        swap 1 lshift 1+
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            [char] x of
                        \ Set bit positions to 0/1.
                        \ Update sta1
                        swap 1 lshift 1+
                        \ Update sta0
                        swap 1 lshift
                        \ Update char counter.
                        rot 1+ -rot
                    endof
            \ Ignore unrecognized characters.
        endcase
    loop
                            \ c-addr cnt sta1 sta0

    \ Make new region, return.
                            \ c-addr cnt sta1 sta0
    rot                     \ c-addr sta1 sta0 cnt
    region-new2             \ c-addr reg
    nip                     \ reg
    true
;

\ Get region from a string, abort if the attempt failed.
: region-from-string-a ( addr n --  reg )
    region-from-string      \ reg t | f
    0= abort" region-from-string failed?"
;

\ Return states that are in a region.
: region-states-in ( sta-lst1 reg1 -- sta-lst )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-list? )

    \ Change order to scan state list.
    swap                            \ reg0 sta-lst1

    \ Init return list.
    list-new swap                   \ reg0 ret-lst sta-lst

    \ Scan through the state-list.
    list-get-links                  \ reg0 ret-lst link

    begin
        ?dup
    while
        dup link-get-data           \ reg0 ret-lst link data
        #3 pick                     \ reg0 ret-lst link data reg0
        region-superset-of-state?   \ reg0 ret-lst link flag
        if
            \ Store the state into the return list.
            dup link-get-data       \ reg0 ret-lst link data
            #2 pick                 \ reg0 ret-lst link data ret-lst
            list-push               \ reg0 ret-lst link
        then

        link-get-next               \ reg0 ret-lst link
    repeat
                                    \ reg0 ret-lst
    nip                             \ ret-lst
;

\ Return a state in a region, requiring the least changes from a state
\ outside of the region.
: region-translate-state    ( sta1 reg0 -- sta )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-value? )
    2dup region-superset-of-state?
    if
        drop
        exit
    then

    dup region-get-state-0      \ sta1 reg0 rsta0
    #2 pick xor                 \ sta1 reg0 diff
    swap region-edge-mask       \ sta1 diff edge-msk
    and                         \ sta1 diff'
    xor                         \ sta' (in region)
;

\ Return a mask of different non-X trit positions between two regions.
: region-diff-mask ( reg1 reg0 -- msk )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )

    \ Get mask of trit positions that are not X in either region.
    over region-edge-mask           \ reg1 reg0 e1-msk
    over region-edge-mask           \ reg1 reg0 e1-msk e0-msk
    and                             \ reg1 reg0 e-msk
    -rot                            \ e-msk reg1 reg0

    \ Get region state dif-mask, which may include X bit positions.
    region-get-state-0              \ e-msk reg1 sta-0
    swap region-get-state-0         \ e-msk sta-0 sta-1
    xor                             \ e-msk sta0-dif-msk

    \ Remove X trit positions from the mask.
    and                             \ dif-msk
;

\ Return the number of corresponding trits that are 0 and 1, between two regions.
: region-distance ( reg1 reg0 -- u )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )

    region-diff-mask        \ msk
    value-num-bits          \ nb
;

: region-adjacent? ( reg1 reg0 -- bool )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )

    region-distance     \ num
    1 =
;

\ Return the state far from a given state, within a region.
: region-far-from-state ( sta1 reg0 - sta-far )
    \ Check args.
    assert( tos is-region? )
    assert( nos is-value? )
    2dup region-superset-of-state?
    0= abort" State is not in region?"

    region-x-mask       \ sta1 x-msk
    xor                 \ sta-far
;

\ Return true if a region's two states are adjacent.
: region-states-adjacent ( reg0 -- flag )
    \ Check arg.
    assert( tos is-region? )

    region-get-states       \ sta1 sta0

    \ Check if X mask contains exactly one bit.
    xor                     \ msk
    value-1-bit-set         \ flag
;

\ Translate a region to another, with fewest changes.
\ From:  0 0 0 | 1 1 1 | X X X
\ To:    0 1 X | 0 1 X | 0 1 X
\ =:     0 1 0 | 0 1 1 | 0 1 X
: region-translate-to-region ( reg-to reg-from -- reg )
    \ cr ." region-translate-to-region: from: " dup .region space ." to: " over .region
    \ Check args.
    assert( tos is-region? )
    assert( nos is-region? )
    2dup regions-dif-num-bits? abort" regions do not have the same number bits?"

    \ Change selected reg-from X positions to zero.
    over region-x-mask          \ reg-to reg-from tx
    over region-edge-0-mask     \ reg-to reg-from tx f0
    and                         \ reg-to reg-from 0x

    #2 pick                     \ reg-to reg-from 0x reg-to
    region-x-to-0               \ reg-to reg-from reg-to'

    \ Change selected reg-from' X positions to one.
    #2 pick region-x-mask       \ reg-to reg-from reg-to' tx
    #2 pick region-edge-1-mask  \ reg-to reg-from reg-to' tx f1
    and                         \ reg-to reg-from reg-to' 1x

    over region-x-to-1          \ reg-to reg-from reg-to' reg-to''

    \ Clean up, return.
    swap region-deallocate      \ reg-to reg-from reg-to''
    nip nip                     \ reg-to''
;

\ Return a copy of a region.
: region-copy ( reg0 -- reg )
    \ Check arg.
    assert( tos is-region? )
    dup region-get-num-bits swap    \ nb reg0

    region-get-states               \ nb sta1 sta0
    rot                             \ sta1 sta0 nb
    region-new2
;

\ Return the number of edges in a region.
: region-get-number-edges ( reg0 -- u )
    \ Check arg.
    assert( tos is-region? )

    region-edge-mask    \ msk
    value-num-bits      \ u
;

