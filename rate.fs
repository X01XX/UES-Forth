\ Implement a rate struct and functions.
\
\ The rate represents a rating for a given RLC.

#41719 constant rate-id
    #1 constant rate-struct-number-cells

\ Struct fields
0 constant rate-header        \ 16-bits [0] struct id [1] use count, [2] Positive quantity, [3] (ABS) negative quantity.

0 value rate-mma \ Storage for rate mma instance.

\ Init rate mma, return the addr of allocated memory.
: rate-mma-init ( num-items -- ) \ sets rate-mma.
    dup 1 <
    abort" rate-mma-init: Invalid number of items."

    cr ." Initializing Rate store."
    rate-struct-number-cells swap mma-new to rate-mma
;

\ Check rate mma usage.
: assert-rate-mma-none-in-use ( -- )
    rate-mma mma-in-use 0<>
    abort" rate-mma use GT 0"
;

\ Check instance type.
: is-allocated-rate ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup rate-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    rate-id =     
;

\ Check TOS for rate, unconventional, leaves stack unchanged. 
: assert-tos-is-rate ( arg0 -- arg0 )
    dup is-allocated-rate
    is-false if
        s" TOS is not an allocated rate"
        .abort-xt execute
    then
;

\ Check NOS for rate, unconventional, leaves stack unchanged. 
: assert-nos-is-rate ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-rate
    is-false if
        s" NOS is not an allocated rate"
        .abort-xt execute
    then
;

\ Check 3OS for rate, unconventional, leaves stack unchanged. 
: assert-3os-is-rate ( arg2 arg1 arg0 -- arg1 arg0 )
    #2 pick is-allocated-rate
    is-false if
        s" NOS is not an allocated rate"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the first field from a rate instance.
: rate-get-positive ( rt0 -- u)
    \ Check arg.
    assert-tos-is-rate

    2w@                     \ Fetch the field.
;
 
\ Return the second field from a rate instance.
: rate-get-negative ( rt0 -- n )
    \ Check arg.
    assert-tos-is-rate

    3w@                     \ Fetch the field.

    \ Change value to negative.
    dup 0 > if
        -1 *
    then
;
 
\ Set the first field from a rate instance, use only in this file.
: _rate-set-positive ( u1 rt0 -- )
    \ Check args.
    assert-tos-is-rate
    swap abs swap

    2w!                     \ Set third header field.
;
 
\ Set the second field from a rate instance, use only in this file.
: _rate-set-negative ( u1 addr -- )
    \ Check args.
    assert-tos-is-rate

    \ Change value to abs.
    swap
    abs
    swap

    3w!                   \ Set fourth header field.
;

\ End accessors.

\ Create a rate from two numbers on the stack.
: rate-new ( neg-u1 pos-u0 -- addr)

    \ Allocate space.
    rate-mma mma-allocate   \ nu1 pu0 addr

    \ Store id.
    rate-id over            \ nu1 pu0 addr id addr
    struct-set-id           \ nu1 pu0 addr

    \ Init use count.
    0 over struct-set-use-count

    \ Store values.
    swap over _rate-set-positive    \ nu1 addr
    swap over _rate-set-negative    \ nu1 addr
;

\ Print a rate.
: .rate ( rt0 -- )
    \ Check arg.
    assert-tos-is-rate

    ." ("
    dup rate-get-positive   \ rt0 u
    .                       \ rt0

    rate-get-negative       \ n
    dup abs 0               \ n u 0
    <# #S rot sign #> type  \
    ." )"
;


\ Deallocate a rate.
: rate-deallocate ( reg0 -- )
    \ Check arg.
    assert-tos-is-rate

    dup struct-get-use-count      \ reg0 count

    #2 <
    if
        \ Deallocate instance.
        rate-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a rate positive and negative is zero.
: rate-all-zero ( rt0 -- bool )
    \ Check arg.
    assert-tos-is-rate

    dup rate-get-positive 0=
    swap rate-get-negative 0=
    and
;

\ Add an nos rate to a tos rate.
: rate-add ( rt1 rt0 -- rt )
    \ Check args.
    assert-tos-is-rate
    assert-nos-is-rate

    over rate-get-negative  \ rt1 rt0 neg1
    over rate-get-negative  \ rt1 rt0 neg1 neg0
    +                       \ rt1 rt0 neg
    over _rate-set-negative \ rt1 rt0

    swap rate-get-positive  \ rt0 pos1
    over rate-get-positive  \ rt0 pos1 pos0
    +                       \ rt0 pos
    swap _rate-set-positive \
;
