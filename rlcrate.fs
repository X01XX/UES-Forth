\ Implement a rlcrate struct and functions.
\
\ The rlcrate represents a Region List Corresponding (to domains) (RLC) and a value.

#41737 constant rlcrate-id
    #3 constant rlcrate-struct-number-cells

\ Struct fields
0 constant rlcrate-header        \ 16-bits [0] struct id, [1] use count.
rlcrate-header      cell+ constant rlcrate-rate-disp
rlcrate-rate-disp   cell+ constant rlcrate-rlc-disp

0 value rlcrate-mma \ Storage for rlcrate mma instance.

\ Init rlcrate mma, return the addr of allocated memory.
: rlcrate-mma-init ( num-items -- ) \ sets rlcrate-mma.
    dup 1 <
    abort" rlcrate-mma-init: Invalid number of items."

    cr ." Initializing RlcRate store."
    rlcrate-struct-number-cells swap mma-new to rlcrate-mma
;

\ Check rlcrate mma usage.
: assert-rlcrate-mma-none-in-use ( -- )
    rlcrate-mma mma-in-use 0<>
    abort" rlcrate-mma use GT 0"
;

\ Check instance type.
: is-allocated-rlcrate ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup rlcrate-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    rlcrate-id =     
;

\ Check TOS for rlcrate, unconventional, leaves stack unchanged. 
: assert-tos-is-rlcrate ( arg0 -- arg0 )
    dup is-allocated-rlcrate
    is-false if
        s" TOS is not an allocated rlcrate"
        .abort-xt execute
    then
;

\ Check NOS for rlcrate, unconventional, leaves stack unchanged. 
: assert-nos-is-rlcrate ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-rlcrate
    is-false if
        s" NOS is not an allocated rlcrate"
        .abort-xt execute
    then
;

\ Check 3OS for rlcrate, unconventional, leaves stack unchanged. 
: assert-3os-is-rlcrate ( arg2 arg1 arg0 -- arg1 arg0 )
    #2 pick is-allocated-rlcrate
    is-false if
        s" NOS is not an allocated rlcrate"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the first field from a rlcrate instance.
: rlcrate-get-rate ( addr -- u)
    \ Check arg.
    assert-tos-is-rlcrate

    rlcrate-rate-disp + \ Add offset.
    @                   \ Fetch the field.
;

\ Return the second field from a rlcrate instance.
: rlcrate-get-rlc ( addr -- u)
    \ Check arg.
    assert-tos-is-rlcrate

    \ Get second state.
    rlcrate-rlc-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Set the first field from a rlcrate instance, use only in this file.
: _rlcrate-set-rate ( rate1 rlcrate0 -- )
    \ Check args.
    assert-tos-is-rlcrate
    assert-nos-is-rate

    rlcrate-rate-disp + \ Add offset.
    !                   \ Set first field.
;
 
\ Set the second field from a rlcrate instance, use only in this file.
: _rlcrate-set-rlc ( rlc1 rlcrate0 -- )
    \ Check args.
    assert-tos-is-rlcrate
    assert-nos-is-list

    rlcrate-rlc-disp +  \ Add offset.
    !                   \ Set second field.
;

\ End accessors.

\ Create a rlcrate from a n rlc and rate on the stack.
: rlcrate-new ( rlc rate -- addr )
    \ Check args.
    assert-tos-is-rate
    assert-nos-is-list

    \ Allocate space.
    rlcrate-mma mma-allocate    \ rlc rate rlcrate

    \ Store id.
    rlcrate-id over             \ rlc rate rlcrate id rlcrate
    struct-set-id               \ rlc rate rlcrate

    \ Init use count.
    0 over struct-set-use-count \ rlc rate rlcrate

    \ Store rate.
    swap                        \ rlc rlcrate rate
    dup struct-inc-use-count    \ rlc rlcrate rate
    over _rlcrate-set-rate      \ rlc rlcrate

    \ Store rlc.
    swap                        \ rlcrate rlc
    dup struct-inc-use-count    \ rlcrate rlc
    over _rlcrate-set-rlc       \ rlcrate
;

\ ' rlcrate-new to rlcrate-new-xt

\ Print a rlcrate.
: .rlcrate ( reg0 -- )
    \ Check arg.
    assert-tos-is-rlcrate

    ." ["
    dup rlcrate-get-rate .rate
    ." , "
    rlcrate-get-rlc
    .region-list-corr
    ." ]"
;

' .rlcrate to .rlcrate-xt

\ Deallocate a rlcrate.
: rlcrate-deallocate ( rlcrt0 -- )
    \ Check arg.
    assert-tos-is-rlcrate

    dup struct-get-use-count      \ rlcrt0 count

    #2 <
    if                              \ rlcrt
        \ Deallocate rate.
        dup rlcrate-get-rate        \ rlcrt rt
        rate-deallocate             \ rlcrt

        \ Deallocate rlc
        dup rlcrate-get-rlc         \ rlcrt rlc
        region-list-deallocate      \ rlcrt
        
        \ Deallocate instance.
        rlcrate-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: rlcrate-rate-all-zero ( rlcrt0 -- bool )
    \ Check arg.
    assert-tos-is-rlcrate

    rlcrate-get-rate    \ rate
    rate-all-zero       \ bool
;

