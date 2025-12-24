\ Implement a Sample struct and functions.
\
\ A initial/result pair of taking an action.
\
\ A initial/result problem, that may be solved with one, or many, actions, all within
\ a single domain.

#23719 constant sample-id
    #3 constant sample-struct-number-cells

\ Struct fields
0                           constant sample-header-disp     \ 16-bits, [0] struct id, [1] use count.
sample-header-disp  cell+   constant sample-initial-disp    \ Initial state.
sample-initial-disp cell+   constant sample-result-disp     \ Result state.

0 value sample-mma \ Storage for sample mma instance.

\ Init sample mma, return the addr of allocated memory.
: sample-mma-init ( num-items -- ) \ sets sample-mma.
    dup 1 <
    abort" sample-mma-init: Invalid number of items."

    cr ." Initializing Sample store."
    sample-struct-number-cells swap mma-new to sample-mma
;

\ Check sample mma usage.
: assert-sample-mma-none-in-use ( -- )
    sample-mma mma-in-use 0<>
    abort" sample-mma use GT 0"
;

\ Check instance type.
: is-allocated-sample ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup sample-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        sample-id =
    else
        drop false
    then
;

\ Check TOS for sample, unconventional, leaves stack unchanged.
: assert-tos-is-sample ( tos -- tos )
    dup is-allocated-sample
    is-false if
        s" TOS is not an allocated sample"
        .abort-xt execute
    then
;

\ Check NOS for sample, unconventional, leaves stack unchanged.
: assert-nos-is-sample ( nos tos -- nos tos )
    over is-allocated-sample
    is-false if
        s" NOS is not an allocated sample"
        .abort-xt execute
    then
;

\ Check 3OS for sample, unconventional, leaves stack unchanged.
: assert-3os-is-sample ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-sample
    is-false if
        s" 3OS is not an allocated sample"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the initial field from a sample instance.
: sample-get-initial ( addr -- u)
    \ Check arg.
    assert-tos-is-sample

    sample-initial-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the result field from a sample instance.
: sample-get-result ( addr -- u)
    \ Check arg.
    assert-tos-is-sample

    sample-result-disp +    \ Add offset.
    @                       \ Fetch the field.
;
\ Set the initial field from a sample instance, use only in this file.
: _sample-set-initial ( u1 addr -- )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-value

    sample-initial-disp +   \ Add offset.
    !                       \ Set initial field.
;

\ Set the result field from a sample instance, use only in this file.
: _sample-set-result ( u1 addr -- )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-value

    sample-result-disp +    \ Add offset.
    !                       \ Set result field.
;

: sample-get-states ( smpl -- u-r u-i )
    \ Check arg.
    assert-tos-is-sample

    dup sample-get-result swap  \ r smpl
    sample-get-initial          \ r i
;

\ End accessors.

\ Create a sample from two numbers on the stack.
\ The numbers may be the same.
\ If you want to keep the sample on the stack, or in a value, or variable,
\ run dup struct-inc-use-count, then deallocate it from there when done using it.
\ If you want to push the sample onto a list, sample-list-push will increment the use count.
: sample-new ( r1 i0 -- addr)
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    \ Allocate space.
    sample-mma mma-allocate     \ u1 u2 addr

    \ Store id.
    sample-id over              \ u1 u2 addr id addr
    struct-set-id               \ u1 u2 addr

    \ Store states
    tuck _sample-set-initial   \ u1  addr
    tuck _sample-set-result    \ addr
;

: sample-copy ( smpl0 - smpl )
    \ Check arg.
    assert-tos-is-sample

    dup sample-get-result       \ smpl0 s-r
    swap sample-get-initial     \ s-r s-i
    sample-new
;

\ Print a sample.
: .sample ( smp0 -- )
    \ Check arg.
    assert-tos-is-sample

    ." ("
    dup sample-get-initial .value
   ." ->"
   sample-get-result .value
   ." )"
;

\ Deallocate a sample.
: sample-deallocate ( smp0 -- )
    \ Check arg.
    assert-tos-is-sample

    dup struct-get-use-count      \ smp0 count

    #2 <
    if
        \ Clear fields.
        0 over _sample-set-initial
        0 over _sample-set-result

        \ Deallocate instance.
        sample-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return a changes instance from a sample.
: sample-calc-changes ( smpl0 -- cngs )
    \ Check arg.
    assert-tos-is-sample

    dup sample-get-initial      \ smpl0 i-sta
    over sample-get-result      \ smpl0 i-sta r-sta
    2dup invert and             \ smpl0 i-sta r-sta m10
    -rot                        \ smpl0 m10 i-sta r-sta
    swap invert and             \ smpl0 m10 m01
    changes-new                 \ smpl0 cngs
    nip
;

\ Return true if a sample result state is NE the initial state.
: sample-any-change ( smpl -- flag )
    \ Check arg.
    assert-tos-is-sample

    dup sample-get-initial      \ smpl0 i-sta
    swap sample-get-result      \ i-sta r-sta
    <>
;

\ Return true if a state is between a sample initial and result states,
\ inclusive.
: sample-state-between ( sta1 smpl0 -- flag )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-value

    dup sample-get-initial      \ sta1 smpl0 s-i
    swap sample-get-result      \ sta1 s-i s-r
    value-between
;

\ Return true if to samples are equal.
: sample-eq ( smpl1 smpl2 -- flag )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-sample

    over sample-get-initial     \ smpl1 smpl2 1-i
    over sample-get-initial     \ smpl1 smpl2 1-i 2-i
    <> if
        2drop false exit
    then
                                \ smpl1 smpl2
    sample-get-result           \ smpl1 2-r
    swap sample-get-result      \ 2-r 1-r
    =
;

\ Return true if a sample's changes intersect a given changes instance.
: sample-intersects-changes ( cngs1 smpl0 -- flag )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-changes

    sample-calc-changes     \ cngs1 s-cngs
    tuck                    \ s-cngs cngs1 s-cngs
    changes-intersect       \ s-cngs flag
    swap changes-deallocate \ flag
;

\ Return true if a sample contains a given state.
: sample-contains-state ( sta1 smpl0 -- flag )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-value

    sample-get-states       \ sta1 s1-r s1-i
    rot                     \ s1-r s1-i sta1
    tuck                    \ s1-r sta1 s1-i sta1
    =                       \ s1-r sta1 flag
    if
        2drop
        true
        exit
    then

    =                       \ flag
;

\ Return true if two samples share at least one state.
: sample-intersects ( smpl1 smpl0 -- flag )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-sample

    sample-get-states       \ smpl1 s0-r s0-i
    rot                     \ s0-r s0-i smpl1
    tuck                    \ s0-r smpl1 s0-i smpl1
    sample-contains-state   \ s0-r smpl1 flag
    if
        2drop
        true
        exit
    then
                            \ s0-r smpl1
    sample-contains-state   \ flag
;

\ Return a region for a sample.
: sample-to-region ( smpl0 -- reg )
    \ Check arg.
    assert-tos-is-sample

    sample-get-states
    region-new
;

