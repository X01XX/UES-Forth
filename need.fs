\ Implement a need struct and functions.

19717 constant need-id
    4 constant need-struct-number-cells

\ Struct fields
0 constant need-header        \ 16-bits [0] struct id [1] use count.
need-header  cell+ constant need-domain     \ A Domain addr.
need-domain  cell+ constant need-action     \ An Action addr.
need-action  cell+ constant need-target     \ A state.

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
    dup need-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    need-id =     
;

: is-not-allocated-need ( addr -- flag )
    is-allocated-need 0=
;

\ Check TOS for need, unconventional, leaves stack unchanged. 
: assert-tos-is-need ( arg0 -- arg0 )
    dup is-allocated-need 0=
    abort" TOS is not an allocated need"
;

\ Check NOS for need, unconventional, leaves stack unchanged. 
: assert-nos-is-need ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-need 0=
    abort" NOS is not an allocated need"
;

\ Start accessors.

\ Return the domain field from a need instance.
: need-get-domain ( ned0 -- dom )
    \ Check arg.
    assert-tos-is-need

    need-domain +       \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the domain field from a need instance, use only in this file.
: _need-set-domain ( dom1 ned0 -- )
    \ Check args.
    assert-tos-is-need

    need-action +       \ Add offset.
    !                   \ Set first field.
;
 
\ Return the action field from a need instance.
: need-get-action ( ned0 -- dom )
    \ Check arg.
    assert-tos-is-need

    need-action +       \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the action field from a need instance, use only in this file.
: _need-set-action ( dom1 ned0 -- )
    \ Check args.
    assert-tos-is-need

    need-action +       \ Add offset.
    !                   \ Set first field.
;

\ Return the target field from a need instance.
: need-get-target ( ned0 -- dom )
    \ Check arg.
    assert-tos-is-need

    need-target +       \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the target field from a need instance, use only in this file.
: _need-set-target ( dom1 ned0 -- )
    \ Check args.
    assert-tos-is-need

    need-target +       \ Add offset.
    !                   \ Set first field.
;

\ End accessors.

\ Create a need given a target.
: need-new ( u0 -- addr)

    \ Allocate space.
    need-mma mma-allocate           \ u0 addr

    \ Store id.
    need-id over                    \ u0 addr id addr
    struct-set-id                   \ u0 addr

    \ Init use count.
    0 over struct-set-use-count     \ u0 addr

    \ Store target
    swap over _need-set-target      \ addr

    \ Store domain.
    cur-domain-xt execute           \ addr dom
    2dup swap                       \ addr dom dom addr
    _need-set-domain                \ addr dom

    \ Store acction.
    cur-action-xt execute           \ addr act
    over _need-set-action           \ addr
;

\ Print a need.
: .need ( ned0 -- )
    \ Check arg.
    assert-tos-is-need

    ." Need: Dom: "
    dup need-get-domain domain-get-inst-id-xt execute . space
    ." Act: "
    dup need-get-action action-get-inst-id-xt execute . space
    ." Target: "
    need-get-target .value
;

\ Deallocate a need.
: need-deallocate ( ned0 -- )
    \ Check arg.
    assert-tos-is-need

    dup struct-get-use-count      \ ned0 count

    2 <
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
