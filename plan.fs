\ Implement a plan struct and functions.

#37379 constant plan-id                                                                                  
    #3 constant plan-struct-number-cells

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

\ Check 3OS for plan, unconventional, leaves stack unchanged. 
: assert-3os-is-plan ( pln2 arg1 arg0 -- arg1 arg0 )
    #2 pick is-allocated-plan 0=
    abort" 3OS is not an allocated plan"
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
    assert-tos-is-domain-xt execute

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

    dup plan-get-domain domain-get-inst-id-xt execute
    ." Dom: " dec. space
    plan-get-step-list .step-list
;

: plan-deallocate ( pln0 -- )
    \ Check arg.
    assert-tos-is-plan

    dup struct-get-use-count      \ stp0 count

    #2 <
    if
        \ Deallocate instance.
        dup plan-get-step-list
        step-list-deallocate
        plan-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return the result state of a non-empty plan.
: plan-get-result-state ( pln - sta )
    \ Check arg.
    assert-tos-is-plan

    \ Check for empty list.
    plan-get-step-list      \ stp-lst
    dup list-is-empty
    abort" Empty step-list in plan?"

    \ Scan the steps.
    list-get-links          \ link
    begin
        dup link-get-data   \ link step
        swap                \ step link

        link-get-next       \ step link
        dup 0=
        if
            drop
            step-get-result
            exit
        then
        nip                 \ link
    again
;

\ Check plan for any result equal to a given result.
: plan-contains-result ( sta1 pln0 -- flag )
    dup plan-get-step-list          \ sta1 pln0 stp-lst
    list-get-links                  \ sta1 pln0 link
    begin
        ?dup
    while
        \ Get step
        dup link-get-data           \ sta1 pln0 link step
        step-get-sample             \ sta1 pln0 link smpl
        sample-get-result           \ sta1 pln0 link s-r
        #3 pick                     \ sta1 pln0 link s-r sta1
        =                           \ sta1 pln0 link flag
        if
            3drop
            true
            exit
        then

        link-get-next
    repeat
                                    \ sta1 pln0
    2drop
    false
;

\ Push a step to the end of a plan.
: plan-push-end ( stp1 pln0 -- )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-step

    over step-get-sample            \ stp1 pln0 smpl
    sample-get-result               \ stp1 pln0 s-r
    over                            \ stp1 pln0 s-r pln0
    plan-contains-result            \ stp1 pln0 flag
    if
        cr ." plan: " .plan space ." contains result of step: " .step cr
        abort
    then

    over                            \ stp1 pln0 | stp1
    over plan-get-step-list         \ stp1 pln0 | stp1 stp-lst

    \ Check step linkage.
    dup list-is-empty               \ stp1 pln0 | stp1 stp-lst flag
    0= if                           \ stp1 pln0 | stp1 stp-lst
        over step-get-initial       \ stp1 pln0 | stp1 stp-lst stp-i
        #3 pick                     \ stp1 pln0 | stp1 stp-lst stp-i pln
        plan-get-result-state       \ stp1 pln0 | stp1 stp-lst stp-i pln-r
        <> abort" steps do not link"
    then

    step-list-push-end              \ stp1 pln0
    2drop
;

\ Return the initial state of a non-empty plan.
: plan-get-initial-state ( pln - sta )
    \ Check arg.
    assert-tos-is-plan

    plan-get-step-list      \ stp-lst
    dup list-is-empty
    abort" Empty step-list in plan?"
    list-get-links          \ link
    link-get-data           \ step
    step-get-initial        \ sta
;

\ Push a step to the beginning of a plan.
: plan-push ( stp1 pln0 -- )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-step
    \ cr ." plan-push: " over .step space dup .plan cr

    over step-get-sample            \ stp1 pln0 smpl
    sample-get-result               \ stp1 pln0 s-r
    over                            \ stp1 pln0 s-r pln0
    plan-contains-result            \ stp1 pln0 flag
    if
        cr ." plan: " .plan space ." contains result of step: " .step cr
        abort
    then

    over                            \ stp1 pln0 | stp1
    over plan-get-step-list         \ stp1 pln0 | stp1 stp-lst

    \ Check step linkage.
    dup list-is-empty               \ stp1 pln0 | stp1 stp-lst flag
    0= if                           \ stp1 pln0 | stp1 stp-lst
        over step-get-result        \ stp1 pln0 | stp1 stp-lst stp-i
        #3 pick                     \ stp1 pln0 | stp1 stp-lst stp-i pln
        plan-get-initial-state      \ stp1 pln0 | stp1 stp-lst stp-i pln-r
        <> abort" steps do not link"
    then

    step-list-push                  \ stp1 pln0
    2drop
;

\ Run a plan.  Return true if it works.
: plan-run ( pln0 -- flag )
    cr ." plan-run" cr
    \ Check arg.
    assert-tos-is-plan

    \ Set current domain and action.
    dup plan-get-domain             \ pln0 dom
    dup                             \ pln0 dom dom
    current-session                 \ pln0 dom dom sess
    session-set-current-domain-xt
    execute                         \ pln0 dom

    dup domain-get-current-state-xt
    execute                         \ pln0 dom cur-sta
    #2 pick plan-get-initial-state  \ pln0 dom cur-sta pln-sta
    <> abort" Plan initial state does not match the domain current state"

                                    \ pln0 dom
    over plan-get-step-list         \ pln0 dom stp-lst
    list-get-links                  \ pln0 dom link
    begin
        ?dup
    while
        \ Get action sample.
        dup link-get-data           \ pln0 dom link step
        dup step-get-action         \ pln0 dom link step actx
        #3 pick                     \ pln0 dom link step actx dom
        domain-get-sample-xt
        execute                     \ pln0 dom link step d-smpl

        \ Check if action sample is as expected.
        over step-get-sample        \ pln0 dom link step d-smpl s-smpl
        over sample-eq              \ pln0 dom link step d-smpl flag
        swap sample-deallocate      \ pln0 dom link step flag
        0= if
            2drop 2drop false exit
        then
        drop                        \ pln0 dom link

        link-get-next
    repeat
                                    \ pln0 dom
    2drop
    true
;

\ Append a nos plan to a tos plan.
: plan-append ( pln1 pln0 -- )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-plan
    \ cr ." append: " over .plan space ." to: " dup .plan cr

    swap                    \ pln0 pln1
    plan-get-step-list      \ pln0 stp-lst1
    list-get-links          \ pln0 link

    begin
        ?dup
    while
        dup link-get-data   \ pln0 link stpx
        #2 pick             \ pln0 link stpx pln0
        plan-push-end       \ pln0 link

        link-get-next       \ pln0 link
    repeat
    drop
;
