\ Implement a region struct and functions.
\
\ The region represents a span of 2^N power squares in a K-Map of any number of bits.
\
\ The region can do this with any two states. The states may be the same for a single-state
\ "region" in a K-Map.
\
\ The region is used as a two-state store, the states being not-equal, in
\ action-incompatible-pairs list.
\
\ Order of the states does not matter.

#19317 constant region-id
    #3 constant region-struct-number-cells

\ Struct fields
0                           constant region-header-disp   \ 16-bits [0] struct id [1] use count.
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

\ Check region mma usage.
: assert-region-mma-none-in-use ( -- )
    region-mma mma-in-use 0<>
    abort" region-mma use GT 0"
;

\ Check instance type.
: is-allocated-region ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup region-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        region-id =
    else
        drop false
    then
;

\ Check TOS for region, unconventional, leaves stack unchanged.
: assert-tos-is-region ( tos -- tos )
    dup is-allocated-region
    is-false if
        s" TOS is not an allocated region"
        .abort-xt execute
    then
;

\ Check NOS for region, unconventional, leaves stack unchanged.
: assert-nos-is-region ( nos tos -- nos tos )
    over is-allocated-region
    is-false if
        s" NOS is not an allocated region"
        .abort-xt execute
    then
;

\ Check 3OS for region, unconventional, leaves stack unchanged.
: assert-3os-is-region ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-region
    is-false if
        s" 3OS is not an allocated region"
        .abort-xt execute
    then
;

\ Check 4OS for region, unconventional, leaves stack unchanged.
: assert-4os-is-region ( 4os 3os nos tos -- 4os 3os nos tos )
    #3 pick is-allocated-region
    is-false if
        s" 4OS is not an allocated region"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the state-0 field from a region instance.
: region-get-state-0 ( addr -- u)
    \ Check arg.
    assert-tos-is-region

    region-state-0-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the state-1 field from a region instance.
: region-get-state-1 ( addr -- u)
    \ Check arg.
    assert-tos-is-region

    \ Get second state.
    region-state-1-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the state-0 field from a region instance, use only in this file.
: _region-set-state-0 ( u1 addr -- )
    \ Check args.
    assert-tos-is-region

    region-state-0-disp +   \ Add offset.
    !                       \ Set the field.
;

\ Set the state-1 field from a region instance, use only in this file.
: _region-set-state-1 ( u1 addr -- )
    \ Check args.
    assert-tos-is-region

    region-state-1-disp +   \ Add offset.
    !                       \ Set the field.
;

\ End accessors.

\ Create a region from two numbers on the stack, without checking their validity.
\ Split from region-new to allow adding an act0 in domain-new.
\ The order of the values will determine how an X bit is displayed, 1 0 = x, 0 1 = X.
: region-new2 ( u1 u0 -- addr)

    \ Allocate space.
    region-mma mma-allocate     \ u1 u0 addr

    \ Store id.
    region-id over              \ u1 u0 addr id addr
    struct-set-id               \ u1 u0 addr

    \ Init use count.
    0 over struct-set-use-count

    \ Prepare to store states.
    -rot            \ addr u1 u0
    #2 pick         \ addr u1 u0 addr
    tuck            \ addr u1 addr u0 addr

    \ Store states
    _region-set-state-0     \ addr u1 addr
    _region-set-state-1     \ addr
;

\ Create a region from two numbers on the stack.
\ The numbers may be the same.
: region-new ( u1 u0 -- addr )
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    \ Make a new region without checking the values.
    region-new2
;

\ Print a region.
: .region ( reg0 -- )
    \ Check arg.
    assert-tos-is-region

    \ Setup for bit-position loop.
    dup  region-get-state-1
    swap region-get-state-0
    current-ms-bit-mask     \ st2 st1 ms-bit

    \ Process each bit.
    begin
      dup
    while
      \ Apply msb to state 1.
      over      \ Get state 1.
      over      \ Get msb and isolate state 1 bit.
      and       \ Isolate state 1 bit corresponding to the msb.

      \ Apply msb to state 2.
      over      \ Get msb.
      #4 pick   \ Get state2 and isolate 1 bit.
      and       \ Isolate state 2 bit corresponding to the msb.

      if
          if
            ." 1"
          else
            ." x"
          then
      else
          if
            ." X"
          else
            ." 0"
          then
      then

      1 rshift
    repeat
    3drop   \ Drop state1 state2 msb.
;

\ Return the highest state in a region.
: region-high-state ( reg0 -- n )
    \ Check arg.
    assert-tos-is-region

    dup  region-get-state-0    \ reg0 state1.
    swap region-get-state-1    \ state1 state2.
    or                         \ High state.
;

\ Return the lowest state in a region.
: region-low-state ( reg0 -- n )
    \ Check arg.
    assert-tos-is-region

    dup  region-get-state-0    \ addr state1.
    swap region-get-state-1    \ state1 state2.
    and                        \ Low state.
;

\ Deallocate a region.
: region-deallocate ( reg0 -- )
    \ Check arg.
    assert-tos-is-region

    dup struct-get-use-count      \ reg0 count

    #2 <
    if
        \ Deallocate instance.
        region-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ ' region-deallocate to region-deallocate-xt

\ Return the two states that make a region.
: region-get-states ( reg0 -- s1 s0 )
    \ Check arg.
    assert-tos-is-region

    \ Calc result.
    dup region-get-state-1
    swap
    region-get-state-0
;

\ Return a regions edge mask.
: region-edge-mask ( reg0 -- mask )
    \ Check arg.
    assert-tos-is-region

    \ Calc result.
    region-get-states       \ s1 s0
    !nxor
;

\ Return true if two regions intersect.
\ And diff-bits in a state from each region.
\     same bits mask from reg1
\     same bits mask from reg2
\ Return 0=
: region-intersects ( reg1 reg0 -- flag )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    \ Get different bits, a superset of such bits.
    over region-get-state-0
    over region-get-state-0
    xor
    -rot     \ dif reg1 reg2

    \ Get reg2 edge bits.
    region-edge-mask

    \ Get reg1 edge bits.
    swap region-edge-mask

    \ And the three
    and and

    \ Return result
    0=
;

\ Return the region high state and low state.
: region-high-low ( reg0 -- high low )
    \ Check arg.
    assert-tos-is-region

    \ Calc result.
    dup region-high-state
    swap region-low-state
;

\ Return the intersection of two regions, or false if they do not intersect.
\ Since this must check for intersection first, there may be no need to check
\ for intersection before calling this.
: region-intersection ( reg1 reg0 -- reg true | false )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    \ Check that the two regions intersect.
    2dup region-intersects
    if
        \ reg1 reg0
        \ Get high and low state of reg0
        region-high-low     \ reg1 reg0high reg0low

        \ Get high and low state of reg1
        rot region-high-low \ reg0high reg0low reg1high reg1low

        \ Group high/low states.
        rot                 \ reg0high reg1ghigh reg1low reg0low

        \ Calc result
        or -rot and
        region-new
        true
    else
        2drop
        false
    then
;

\ Return the union of two regions.
: region-union ( reg1 reg0 -- reg3 )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    \ reg1 reg0
    \ Get high and low state of reg0
    region-high-low     \ reg1 reg0high reg0low

    \ Get high and low state of reg1
    rot region-high-low \ reg0high reg0low reg1high reg1low

    \ Group high/low states.
    rot                 \ reg0high reg1high reg1low reg0low

    \ Calc result
    and -rot or
    region-new
;

\ Return the union of a region and a state.
: region-union-state ( sta1 reg0 -- reg )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-value

    region-high-low         \ sta1 high low
    rot                     \ high low sta1
    tuck                    \ high sta1 low sta1
    and                     \ high sta1 low2
    -rot                    \ low2 high sta1
    or                      \ low2 high2
    region-new
;

\ Return a regions X mask.
: region-x-mask ( reg0 -- mask )
    \ Check arg.
    assert-tos-is-region

    \ Calc result.
    region-get-states   \ s1 s0
    xor
;

\ Return a region's edge 1 mask.
: region-edge-1-mask ( reg0 -- mask )
    \ Check arg.
    assert-tos-is-region

    \ Calc result.
    region-get-states   \ s1 s0
    and
;

\ Return a region's edge 0 mask.
: region-edge-0-mask ( reg0 -- mask )
    \ Check arg.
    assert-tos-is-region

    \ Calc result.
    region-get-states   \ s1 s0
    !nor
;

\ Return a new region with some X positions set to zero.
\ Change 1-0 or 0-1 to 0-0.
\ Mask will usually have a single bit, called from region-subtract.
: region-x-to-0 ( to-0-mask reg0 -- reg )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-value

    region-get-states       \ to-0-mask s1 s0
    rot !not                \ state1 state2 keep-mask
    tuck                    \ state1 keep state2 keep
    and                     \ state1 keep state2'
    -rot                    \ state2' state1 keep
    and                     \ state2' state1'
    region-new              \ reg
;

\ Return a new region with some X positions set to one.
\ Change 1-0 or 0-1 to 1-1.
\ Mask will usually have a single bit, called from region-subtract.
: region-x-to-1 ( to-1-mask reg0 -- reg )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-value

    region-get-states       \ to-1 s1 s0
    rot                     \ s1 s0 to-1
    tuck                    \ s1 to-1 s0 to-1
    or                      \ s1 to-1 s0'
    -rot                    \ s0' s1 to-1
    or                      \ s0' s1'
    region-new              \ reg
;

\ Return true if two regions are equal.
: region-eq ( reg1 reg0 -- flag )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    \ Check address.
    2dup =                  \ reg1 reg0 bool
    if
        2drop
        true
        exit
    then

    over region-high-state over region-high-state <>
    if
        2drop
        false
        exit
    then

    region-low-state swap region-low-state <>
    if
        false
    else
        true
    then
;

\ Return true if regions are not equal.
: region-neq ( reg1 rge0 -- bool )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    region-eq
    is-false
;

\ Return true if a TOS region is a superset of the NOS region.
: region-superset-of ( reg1 reg-sup -- flag )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    2dup region-intersects          \ reg1 reg-sup flag
    if
        \ Regions intersect.
        over region-intersection    \ reg1 reg-int flag
        0= abort" region-superset-of: reg-sup and reg1 should intersect"
                                    \ reg1 reg-int
        tuck region-eq              \ reg-int flag
        swap region-deallocate      \ flag
    else
        \ Regions do not intersect, return false.
        2drop
        false
    then
;

\ Return true if a TOS region is a subset of the NOS region.
: region-subset-of ( reg1 reg-sub -- flag )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    2dup region-intersects          \ reg1 reg-sub flag
    if
        \ Regions intersect.
        tuck                        \ reg-sub reg1 reg-sub
        region-intersection         \ reg-sub reg-int flag
        0= abort" region-subset-of: reg-sub and reg1 should intersect"
                                    \ reg-sub reg-int
        tuck region-eq              \ reg-int flag
        swap region-deallocate      \ flag
    else
        \ Regions do not intersect, return false.
        2drop
        false
    then
;

\ Return true if a TOS region is a superset of the NOS state.
: region-superset-of-state ( sta1 reg0 -- flag )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-value

    region-get-states           \ sta1 s1 s0
    rot                         \ s1 s0 sta1
    tuck                        \ s1 sta1 s0 sta1
    xor                         \ rs1 sta1 diff2
    -rot                        \ diff2 s1 sta1
    xor                         \ diff2 diff1
    and                         \ both-diff
    0=                          \ flag
;

\ Return true if a region uses a given state.
: region-uses-state ( sta1 reg0 -- flag )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-value

    region-get-states           \ sta1 s1 s0
    #2 pick                     \ sta1 s1 s0 sta1
    =                           \ sta1 s0 flag
    if                          \ sta1 s0
        2drop
        true
        exit
    then

    \ sta1 s0
    =                           \ flag
;

\ Get a region from a string.
\ Valid chars are 0, 1, X, x, and underscore as separator.
\ The number of valid characters must equal the number of bits in the current domain.
: region-from-string ( addr n --  reg t | f)
    \ Init character counter.
    0 -rot              \ cnt addr n

    \ Init state 0, state 1, and do initial value. All bit positions are 0/0.
    0 swap 0 swap 0     \ cnt addr 0 0 n 0
    do                  \ cnt addr 0 0
        #2 pick i +
        c@

        \ Process character.
        case
            [char] 0 of
                        \ Left shift state 0, state 1.
                        swap 1 lshift swap 1 lshift
                        \ Leave bit positions as 0/0.
                        \ Update char counter.
                        2swap swap 1+ swap 2swap
                    endof
            [char] 1 of
                        \ Left shift state 0, state 1.
                        swap 1 lshift swap 1 lshift
                        \ Set bit positions to 1/1.
                        swap 1+ swap 1+
                        \ Update char counter.
                        2swap swap 1+ swap 2swap
                    endof
            [char] X of
                        \ Left shift state 0, state 1.
                        swap 1 lshift swap 1 lshift
                        \ Set bit positions to 1/0.
                        swap 1+ swap
                        \ Update char counter.
                        2swap swap 1+ swap 2swap
                    endof
            [char] x of
                        \ Left shift state 0, state 1.
                        swap 1 lshift swap 1 lshift
                        \ Set bit positions to 0/1.
                        1+
                        \ Update char counter.
                        2swap swap 1+ swap 2swap
                    endof
                \ Ignore unrecognized characters.
        endcase
    loop
                            \ cnt addr s1 s0
    \ Check counter.
    2swap                   \ s1 s0 cnt addr
    drop                    \ s1 s0 cnt
    current-num-bits        \ s1 s0 cnt nb
    <> if                   \ s1 s0
        2drop
        false
        exit
    then
                            \ s1 s0
    region-new
    true
;

\ Get region from a string, abort if the attemp failed.
: region-from-string-a ( addr n --  reg )
    region-from-string      \ reg t | f
    0= abort" region-from-string failed?"
;

\ Return states that are in a region.
: region-states-in ( sta-lst1 reg1 -- sta-lst )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-list

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
        region-superset-of-state    \ reg0 ret-lst link flag
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
    assert-tos-is-region
    assert-nos-is-value
    2dup region-superset-of-state
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

\ Return a mask of different non-X bit positions between two regions.
: region-diff-mask ( reg1 reg0 -- msk )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    \ Get mask of bit positions that are not X in either region.
    over region-edge-mask           \ reg1 reg0 e1-msk
    over region-edge-mask           \ reg1 reg0 e1-msk e0-msk
    and                             \ reg1 reg0 e-msk
    -rot                            \ e-msk reg1 reg0

    \ Get region state dif-mask, which may include X bit positions.
    region-get-state-0              \ e-msk reg1 sta-0
    swap region-get-state-0         \ e-msk sta-0 sta-1
    xor                             \ e-msk sta0-dif-msk

    \ Remove X bit positions from the mask.
    and                             \ dif-msk
;

\ Return the number of bits different two regions are.
: region-distance ( reg1 reg0 -- u )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    region-diff-mask        \ msk
    value-num-bits          \ nb
;

\ Return the state far from a given state, within a region.
: region-far-from-state ( sta1 reg0 - sta-far )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-value
    2dup region-superset-of-state
    0= abort" State is not in region?"

    region-x-mask       \ sta1 x-msk
    xor                 \ sta-far
;

\ Return true if a region's two states are adjacent.
: region-states-adjacent ( reg0 -- flag )
    \ Check arg.
    assert-tos-is-region

    region-get-states       \ s1 s0

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
    assert-tos-is-region
    assert-nos-is-region

    \ Change selected reg-from X positions to zero.
    over region-x-mask      \ reg-to reg-from tx
    over region-edge-0-mask \ reg-to reg-from tx f0
    and                     \ reg-to reg-from 0x

    #2 pick                 \ reg-to reg-from 0x reg-to
    region-x-to-0           \ reg-to reg-from reg-to'

    \ Change selected reg-from' X positions to one.
    #2 pick region-x-mask       \ reg-to reg-from reg-to' tx
    #2 pick region-edge-1-mask  \ reg-to reg-from reg-to' tx f1
    and                         \ reg-to reg-from reg-to' 1x

    over region-x-to-1      \ reg-to reg-from reg-to' reg-to''

    \ Clean up, return.
    swap region-deallocate  \ reg-to reg-from reg-to''
    nip nip                 \ reg-to''
    \ space ." = " dup .region cr
;

\ Return a copy of a region.
: region-copy ( reg0 -- reg )
    \ Check arg.
    assert-tos-is-region

    region-get-states   \ s1 s0
    region-new
;
