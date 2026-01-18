\ Implement a need struct and functions.

#19717 constant need-id
    #4 constant need-struct-number-cells

\ Struct fields
0                           constant need-header-disp   \ 16 bits' [0] struct id, [1] use count, [2] Type (8 bits).
need-header-disp    cell+   constant need-domain-disp   \ A Domain addr.
need-domain-disp    cell+   constant need-action-disp   \ An Action addr.
need-action-disp    cell+   constant need-target-disp   \ A state.

0 value need-mma \ Storage for need mma instance.

\ Init need mma, return the addr of allocated memory.
: need-mma-init ( num-items -- ) \ sets need-mma.
    dup 1 <
    abort" need-mma-init: Invalid number of items."

    cr ." Initializing Need store."
    need-struct-number-cells swap mma-new to need-mma
;

\ Check need mma usage.
: assert-need-mma-none-in-use ( -- )
    need-mma mma-in-use 0<>
    abort" need-mma use GT 0"
;

\ Check instance type.
: is-allocated-need ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup need-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        need-id =
    else
        drop false
    then
;

\ Check TOS for need, unconventional, leaves stack unchanged.
: assert-tos-is-need ( tos -- tos )
    dup is-allocated-need
    is-false if
        s" TOS is not an allocated need"
       .abort-xt execute
    then
;

\ Check NOS for need, unconventional, leaves stack unchanged.
: assert-nos-is-need ( nos tos -- nos tos )
    over is-allocated-need
    is-false if
        s" NOS is not an allocated need"
       .abort-xt execute
    then
;

\ Check tos for valid need number.
: assert-tos-is-need-number ( tos -- tos )
    dup 1 < over #5 > or
    if
        s" tos invalid need number?"
       .abort-xt execute
    then
;

\ Check nos for valid need number.
: assert-nos-is-need-number ( nos tos -- nos tos )
    over dup                     \ u1 arg0 u1 u1
    1 < swap                     \ u1 arg0 b1 u1
    #5 >                         \ u1 arg0 b1 b2
    or
    if
        s" nos invalid need number?"
       .abort-xt execute
    then
;

\ Check 3os for valid need number.
: assert-3os-is-need-number ( 3os nos tos -- 3os nos tos )
    #2 pick dup                  \ u2 arg1 arg0 u2 u2
    1 < swap                     \ u2 arg1 arg0 b1 u2
    #5 >                         \ u2 arg1 arg0 b1 b2
    or
    if
        s" 3os invalid need number?"
       .abort-xt execute
    then
;

\ Start accessors.

\ Return the domain field from a need instance.
: need-get-domain ( ned0 -- dom )
    \ Check arg.
    assert-tos-is-need

    need-domain-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Set the domain field from a need instance, use only in this file.
: _need-set-domain ( dom1 ned0 -- )
    \ Check args.
    assert-tos-is-need
    assert-nos-is-domain-xt execute

    need-domain-disp +  \ Add offset.
    !                   \ Set the field.
;

\ Return the action field from a need instance.
: need-get-action ( ned0 -- act )
    \ Check arg.
    assert-tos-is-need

    need-action-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Set the action field from a need instance, use only in this file.
: _need-set-action ( act1 ned0 -- )
    \ Check args.
    assert-tos-is-need
    assert-nos-is-action-xt execute

    need-action-disp +  \ Add offset.
    !                   \ Set the field.
;

\ Return the target field from a need instance.
: need-get-target ( ned0 -- sta )
    \ Check arg.
    assert-tos-is-need

    need-target-disp +  \ Add offset.
    @                   \ Fetch the field.
;

\ Set the target field from a need instance, use only in this file.
: _need-set-target ( ned0 -- )
    \ Check arg.
    assert-tos-is-need

    need-target-disp +  \ Add offset.
    !                   \ Set the field.
;

: need-get-type ( ned0 -- type )
    \ Check arg.
    assert-tos-is-need

    4c@
;

: _need-set-type ( typ1 ned0 -- )
    assert-tos-is-need
    assert-nos-is-need-number

    4c!
;

\ End accessors.

\ Create a need given a need number, target, act and domain.
: need-new ( typ3 u2 act1 dom0 -- addr) \ For non-test code, call action-make-need instead of this.
    \ Check args.
    assert-tos-is-domain-xt execute
    assert-nos-is-action-xt execute
    assert-3os-is-value

    \ Allocate space.
    need-mma mma-allocate           \ typ3 u2 act1 dom0 ned

    \ Store id.
    need-id over struct-set-id      \ typ3 u2 act1 dom0 ned

    \ Init use count.
    0 over struct-set-use-count     \ typ3 u2 act1 dom0 ned

    \ Store domain
    tuck _need-set-domain           \ typ3 u2 act1 ned

    \ Store action
    tuck _need-set-action           \ typ3 u2 ned

    \ Store target
    tuck _need-set-target           \ typ3 ned

    \ Store type.
    tuck _need-set-type             \ ned
;

\ Print a need.
: .need ( ned0 -- )
    \ Check arg.
    assert-tos-is-need

    ." Dom: "
    dup need-get-domain domain-get-inst-id-xt execute #3 dec.r space
    ." Act: "
    dup need-get-action action-get-inst-id-xt execute #3 dec.r space

    \ Set up for value print.
    dup need-get-domain
    current-session session-set-current-domain-xt execute

    ." Target: "
    dup need-get-target .value

    need-get-type
    case
        1 of space ." State not in group" endof
       #2 of space ." Confirm logical structure" endof
       #3 of space ." Improve logical structure" endof
       #4 of space ." Fill group" endof
       #5 of space ." Confirm group" endof
        ." Unrecognized type value" abort
    endcase
;

\ Deallocate a need.
: need-deallocate ( ned0 -- )
    \ Check arg.
    assert-tos-is-need

    dup struct-get-use-count      \ ned0 count

    #2 <
    if
        \ Deallocate instance.
        need-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if two needs are equal.
: need-eq ( ned1 ned0 -- flag )
    \ Check args.
    assert-tos-is-need
    assert-nos-is-need

    over need-get-domain
    over need-get-domain <>
    if
        2drop false exit
    then

    over need-get-action
    over need-get-action <>
    if
        2drop false exit
    then

    over need-get-target
    over need-get-target =
;
