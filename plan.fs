\ Implement a plan struct and functions.

37379 constant plan-id                                                                                  
    3 constant plan-struct-number-cells

\ Struct fields
0 constant plan-header                          \ id (16) use count (16)
plan-header   cell+ constant plan-domain        \ A domain addr.
plan-domain   cell+ constant plan-step-list     \ A step-list.

0 value plan-mma \ Storage for plan mma instance.

\ Init plan mma, return the addr of allocated memory.
: plan-mma-init ( num-items -- ) \ sets plan-mma.
    dup 1 < 
    abort" plan-mma-init: Invalid number of items."

    cr ." Initializing Plan store."
    plan-struct-number-cells swap mma-new to plan-mma
;

\ Check plan mma usage.
: assert-plan-mma-none-in-use ( -- )
    plan-mma mma-in-use 0<>
    abort" plan-mma use GT 0"
;

\ Check instance type.
: is-allocated-plan ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup plan-mma mma-within-array 0=
    if  
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    plan-id =    
;

: is-not-allocated-plan ( addr -- flag )
    is-allocated-plan 0=
;

\ Check TOS for plan, unconventional, leaves stack unchanged. 
: assert-tos-is-plan ( arg0 -- arg0 )
    dup is-allocated-plan 0=
    abort" TOS is not an allocated plan"
;

\ Check NOS for plan, unconventional, leaves stack unchanged. 
: assert-nos-is-plan ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-plan 0=
    abort" NOS is not an allocated plan"
;

\ Start accessors.

\ Return the plan domain. 
: plan-get-domain ( addr -- act )
    \ Check arg.
    assert-tos-is-plan

    plan-domain +       \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the domain of a plan instance, use only in this file.
: _plan-set-domain ( u1 addr -- )
    plan-domain +       \ Add offset.
    !                   \ Set field.
;

\ Return the plan step-list. 
: plan-get-step-list ( addr -- act )
    \ Check arg.
    assert-tos-is-plan

    plan-step-list +       \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the step-list of a plan instance, use only in this file.
: _plan-set-step-list ( u1 addr -- )
    plan-step-list +       \ Add offset.
    !                   \ Set field.
;

\ End accessors.

\ Return a new, empty, plan, given a domain.
: plan-new    ( dom0 -- plan )
    \ Check args.
    assert-tos-is-domain

   \ Allocate space.
    plan-mma mma-allocate           \  d0 addr

    \ Store id.
    plan-id over                    \  d0 addr id addr
    struct-set-id                   \  d0 addr
        
    \ Init use count.
    0 over struct-set-use-count     \  d0 addr

    \ Set domain.
    tuck                            \  addr d0 addr
    _plan-set-domain                \  addr

    \ Set step-list.
    list-new
    dup struct-inc-use-count        \ addr stp-lst
    over _plan-set-step-list        \ addr
;

: .plan ( stp0 -- )
    \ Check arg.
    assert-tos-is-plan

    dup plan-get-domain domain-get-inst-id
    ." Dom: " . space
    dup plan-get-step-list .step-list
;

: plan-deallocate ( stp0 -- )
    \ Check arg.
    assert-tos-is-plan

    dup struct-get-use-count      \ stp0 count

    2 <
    if
        \ Deallocate instance.
        dup plan-get-step-list
        step-list-deallocate
        plan-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

