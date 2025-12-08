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
    dup is-allocated-plan
    is-false if
        s" TOS is not an allocated plan"
       .abort-xt execute
    then
;

\ Check NOS for plan, unconventional, leaves stack unchanged. 
: assert-nos-is-plan ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-plan
    is-false if
        s" NOS is not an allocated plan"
       .abort-xt execute
    then
;

\ Check 3OS for plan, unconventional, leaves stack unchanged. 
: assert-3os-is-plan ( pln2 arg1 arg0 -- arg1 arg0 )
    #2 pick is-allocated-plan
    is-false if
        s" 3OS is not an allocated plan"
       .abort-xt execute
    then
;

\ Check 4OS for plan, unconventional, leaves stack unchanged. 
: assert-4os-is-plan ( pln3 arg2 arg1 arg0 -- arg1 arg0 )
    #3 pick is-allocated-plan
    is-false if
        s" 4OS is not an allocated plan"
       .abort-xt execute
    then
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

\ Return the result region of a non-empty plan.
: plan-get-result-region ( pln - reg )
    \ Check arg.
    assert-tos-is-plan

    \ Check for empty list.
    plan-get-step-list      \ stp-lst
    dup list-is-empty
    abort" plan-get-result-region: Empty plan?"

    \ Scan the steps.
    list-get-links          \ link
    begin
        dup link-get-data   \ link step
        swap                \ step link

        link-get-next       \ step link
        dup 0=
        if
            drop
            step-get-result-region
            exit
        then
        nip                 \ link
    again
;

\ Check plan for any initial region intersecting a step's result region.
: plan-check-step-result ( stp1 pln0 -- flag )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-step

    swap step-get-result-region
    swap                            \ reg-r pln0

    plan-get-step-list              \ reg-r stp-lst
    list-get-links                  \ reg-r link
    begin
        ?dup
    while
        \ Get step
        dup link-get-data           \ reg-r link step
        step-get-initial-region     \ reg-r link s-r
        #2 pick                     \ reg-r link s-r reg-r
        region-intersects           \ reg-r link flag
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                    \ reg-r
    drop
    false
;

\ Check plan for any result region intersecting a step's initial region.
: plan-check-step-initial ( stp1 pln0 -- flag )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-step

    swap step-get-initial-region
    swap                            \ reg-i pln0

    plan-get-step-list              \ reg-i stp-lst
    list-get-links                  \ reg-i link
    begin
        ?dup
    while
        \ Get step
        dup link-get-data           \ reg-i link step
        step-get-result-region      \ reg-i link s-r
        #2 pick                     \ reg-i link s-r reg-i
        region-intersects           \ reg-i link flag
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                    \ reg-i
    drop
    false
;

\ Push a step to the end of a plan, forward chaining.
: plan-push-end ( stp1 pln0 -- )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-step
    \ cr ." plan-push-end: " dup .plan space over .step cr

    2dup plan-check-step-result     \ stp1 pln0 flag
    if
        cr ." plan: " .plan space ." contains result region intersection of step: " .step cr
        abort
    then

    over                            \ stp1 pln0 | stp1
    over plan-get-step-list         \ stp1 pln0 | stp1 stp-lst

    \ Check step linkage.
    dup list-is-empty               \ stp1 pln0 | stp1 stp-lst flag
    0= if                           \ stp1 pln0 | stp1 stp-lst
        over                        \ stp1 pln0 | stp1 stp-lst stp1
        step-get-initial-region     \ stp1 pln0 | stp1 stp-lst stp-i
        #3 pick                     \ stp1 pln0 | stp1 stp-lst stp-i pln
        plan-get-result-region      \ stp1 pln0 | stp1 stp-lst stp-i pln-r
        region-neq abort" steps do not link directly, use plan-link"
    then

    step-list-push-end              \ stp1 pln0
    2drop
;

\ Return the initial region of a non-empty plan.
: plan-get-initial-region ( pln - reg )
    \ Check arg.
    assert-tos-is-plan

    plan-get-step-list      \ stp-lst
    dup list-is-empty
    abort" plan-get-initial-region: Empty plan?"

    list-get-links          \ link
    link-get-data           \ step
    step-get-initial-region \ reg
;

\ Push a step to the beginning of a plan, backward chaining.
: plan-push ( stp1 pln0 -- )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-step
    \ cr ." plan-push: " over .step space dup .plan cr

    2dup plan-check-step-initial    \ stp1 pln0 flag
    if
        cr ." plan: " .plan space ." contains initial region intersection of step: " .step cr
        abort
    then

    over                            \ stp1 pln0 | stp1
    over plan-get-step-list         \ stp1 pln0 | stp1 stp-lst

    \ Check step linkage.
    dup list-is-empty               \ stp1 pln0 | stp1 stp-lst flag
    0= if                           \ stp1 pln0 | stp1 stp-lst
        over                        \ stp1 pln0 | stp1 stp-lst stp1
        step-get-result-region      \ stp1 pln0 | stp1 stp-lst stp-i
        #3 pick                     \ stp1 pln0 | stp1 stp-lst stp-i pln
        plan-get-initial-region     \ stp1 pln0 | stp1 stp-lst stp-i pln-r
        region-eq is-false abort" steps do not link directly, maybe use plan-link?"
    then

    step-list-push                  \ stp1 pln0
    2drop
;

\ Run a plan.  Return true if it works.
: plan-run ( pln0 -- flag )
 cr ." plan-run: " dup .plan
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

    #2 pick plan-get-initial-region \ pln0 dom cur-sta pln-reg
    region-superset-of-state is-false abort" Plan initial region does not match the domain current state"

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
        dup                         \ pln0 dom link step d-smpl d-smpl
        sample-get-result           \ pln0 dom link step d-smpl d-r
        #2 pick                     \ pln0 dom link step d-smpl d-r step
        step-get-result-region      \ pln0 dom link step d-smpl d-r s-r
        region-superset-of-state    \ pln0 dom link step d-smpl bool
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

\ Return a plan after restricting its initial region.
: plan-restrict-initial-region ( reg1 pln0 -- pln t | f )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-region

    2dup plan-get-initial-region    \ reg1 pln0 reg1 pln-i-reg
    region-intersects               \ reg1 pln0 bool
    is-false abort" plan initial region does not intersect?"

    \ Init return plan.
    dup plan-get-domain             \ reg1 pln0 dom-id
    plan-new -rot                   \ pln reg1 pln0

    \ Prep for loop.
    plan-get-step-list              \ pln reg1 list
    list-get-links                  \ pln reg1 link

    begin
        ?dup
    while
        dup link-get-data               \ pln reg1 link stpx
        #2 pick                         \ pln reg1 link stpx reg1
        over step-get-initial-region    \ pln reg1 link stpx reg1 stp-i-reg
        2dup region-intersects          \ pln reg1 link stpx reg1 stp-i-reg bool
        is-false if
            3drop 2drop                 \ pln
            plan-deallocate
            false
            exit
        then

                                            \ pln reg1 link stpx reg1 stp-i-reg
        region-subset-of                    \ pln reg1 link stpx bool ( includes region-eq )
        is-false if                         \ pln reg1 link stpx
            \ Restrict step initial region.
            #2 pick                         \ pln reg1 link stpx reg1
            swap                            \ pln reg1 link reg1 stpx
            step-restrict-initial-region    \ pln reg1 link stpx'
        then
        
        \ Set new reg1.
        rot drop                    \ pln link stpx
        dup step-get-result-region  \ pln link stpx s-rslt
        -rot                        \ pln reg1-new link stpx

        \ Add step to plan.
        #3 pick                 \ pln reg1 link stpx pln
        plan-push-end           \ pln reg1 link

        link-get-next
    repeat
                                \ pln reg1
    drop
    true
;

\ Return a plan after restricting its result region.
: plan-restrict-result-region ( reg1 pln0 -- pln t | f )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-region

    2dup plan-get-result-region     \ reg1 pln0 reg1 pln-r-reg
    region-intersects               \ reg1 pln0 bool
    is-false abort" plan result region does not intersect?"

    \ Init return plan.
    dup plan-get-domain             \ reg1 pln0 dom-id
    plan-new -rot                   \ pln reg1 pln0

    \ Prep for loop.

    \ Get step list from plan, reverse list, save ref for later deallocation.
    plan-get-step-list              \ pln reg1 stp-list
    step-list-reverse               \ pln reg1 stp-list'
    -rot                            \ stp-lst' pln reg1
    #2 pick                         \ stp-lst' pln reg1 stp-list'
    
    list-get-links                  \ stp-lst' pln reg1 link

    begin
        ?dup
    while
        dup link-get-data               \ stp-lst' pln reg1 link stpx
        #2 pick                         \ stp-lst' pln reg1 link stpx reg1
        over step-get-result-region     \ stp-lst' pln reg1 link stpx reg1 stp-r-reg
        2dup region-intersects          \ stp-lst' pln reg1 link stpx reg1 stp-r-reg bool
        is-false if
            3drop 2drop                 \ stp-lst' pln
            plan-deallocate
            step-list-deallocate
            false
            exit
        then

                                            \ stp-lst' pln reg1 link stpx reg1 stp-r-reg
        region-subset-of                    \ stp-lst' pln reg1 link stpx bool ( includes region-eq )
        is-false if                         \ stp-lst' pln reg1 link stpx
            \ Restrict step result region.
            #2 pick                         \ stp-lst' pln reg1 link stpx reg1
            swap                            \ stp-lst' pln reg1 link reg1 stpx
            step-restrict-result-region     \ stp-lst' pln reg1 link stpx'
        then
        
        \ Set new reg1.             \ stp-lst' pln reg1 link stpx
        rot drop                    \ stp-lst' pln link stpx'
        dup step-get-initial-region \ stp-lst' pln link stpx' s-rslt
        -rot                        \ stp-lst' pln reg1-new link stpx

        \ Add step to plan.
        #3 pick                 \ stp-lst' pln reg1 link stpx pln
        plan-push               \ stp-lst' pln reg1 link

        link-get-next
    repeat
                                \ stp-lst' pln reg1
    drop                        \ stp-lst' pln
    swap step-list-deallocate   \ pln
    true
;

\ Link two plans, where tos plan result intersects nos plan initial.
: plan-link ( pln-to pln-from -- pln t | f )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-plan
    \ cr ." plan-link start: from " dup .plan space ." to " over .plan cr

    \ Check intersection.
    over plan-get-initial-region
    over plan-get-result-region

    region-intersection                 \ pln-to pln-from, reg-int' t | f
    if
        \ Adjust each plan to be compatible with the other.
        tuck                            \ pln-to reg-int' pln-from reg-int'
        swap                            \ pln-to reg-int' reg-int' pln-from
        plan-restrict-result-region     \ pln-to reg-int', pln-from' t | f
        is-false if
            cr ." restrict result region failed" cr
            region-deallocate
            drop
            false
            exit
        then
    else
        over plan-get-initial-region    
        over plan-get-result-region
        cr ." plan-link: no intersection, plan from result-region " .region space ." to plan-to initial region " .region cr
        2drop
        false
        exit
    then
                                    \ pln-to reg-int' pln-from'
    -rot                            \ pln-from' pln-to reg-int'
    tuck                            \ pln-from' reg-int' pln-to reg-int'
    swap                            \ pln-from' reg-int' reg-int' pln-to'
    plan-restrict-initial-region    \ pln-from' reg-int', pln-to' t | f
    is-false if
        region-deallocate
        plan-deallocate
        false
        exit
    then
                                    \ pln-from' reg-int' pln-to'
    swap region-deallocate          \ pln-from' pln-to'

    \ Build return plan.
    tuck                            \ pln-to' pln-from' pln-to'
    plan-get-step-list              \ pln-to' pln-from' stp-lst
    list-get-links                  \ pln-to' pln-from' link

    begin
        ?dup
    while
        dup link-get-data           \ pln-to' pln-from' link stpx
        #2 pick                     \ pln-to' pln-from' link stpx pln-from'
        plan-push-end               \ pln-to' pln-from' link

        link-get-next               \ pln-to' pln-from' link
    repeat
                                    \ pln-to' pln-from'
    swap plan-deallocate            \ pln-from'
    true
    \ cr ." plan link end: " .s cr
;

: plan-get-length ( pln0 -- u )
    \ Check args.
    assert-tos-is-plan

    plan-get-step-list
    list-get-length
;

\ Pop the first step from a plan.
: plan-pop ( pln0 -- stp t | f )
    \ Check arg.
    assert-tos-is-plan

    plan-get-step-list  \ stp-lst
    step-list-pop       \ stp t | f
;

\ Add a step to the end of a plan, returning a new plan.
: plan-link-step-to-result-region ( stp-to pln-from -- pln t | f )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-step
    \ cr ." plan-link-step-to-result-region start: from " dup .plan space ." to " over .step cr

    \ Check step for plan.
    2dup plan-check-step-result     \ stp-to pln-from bool
    if
        2drop
        false
        exit
    then

    \ Make plan from step.
    swap                        \ pln-from stp-to
    over plan-get-domain        \ pln-from stp-to dom
    plan-new                    \ pln-from stp-to pln-to
    tuck plan-push              \ pln-from pln-to

    \ Link plans.
    tuck swap                   \ pln-to pln-to pln-from
    plan-link                   \ pln-to, pln-rslt t | f

    \ Check result.
    if                          \ pln-to pln-rslt
        swap                    \ pln-rslt pln-to
        dup plan-pop if drop then   \ Protect step from deallocation.
        plan-deallocate         \ pln-rslt
        true
    else                        \ pln-to
        dup plan-pop if drop then   \ Protect step from deallocation.
        plan-deallocate         \
        false
    then
;

\ Add a step to the beginning of a plan, returning a new plan.
: plan-link-step-to-initial-region ( stp-from pln-to -- pln t | f )
    \ Check args.
    assert-tos-is-plan
    assert-nos-is-step
    \ cr ." plan-link-step-to-initial-region: start: " .s cr

    \ Check step for plan.
    2dup plan-check-step-initial    \ stp-from pln-to bool
    if
        2drop
        false
        exit
    then

    \ Make plan from step.
    swap                        \ pln-to stp-from
    over plan-get-domain        \ pln-to stp-from' dom
    plan-new                    \ pln-to stp pln-from
    tuck plan-push              \ pln-to pln-from

    \ Link plans.
    tuck plan-link                  \ pln-from, pln-rslt t | f
    if                              \ pln-from pln-rslt
        swap                        \ pln-rslt pln-from
        dup plan-pop if drop then   \ Protect step from deallocation.
        plan-deallocate             \ pln-rslt
        true
    else                            \ pln-from
        dup plan-pop if drop then   \ Protect step from deallocation.
        plan-deallocate             \ stp-from pln-to
        drop
        false
    then
    \ cr ." plan-link-step-to-initial-region: end: " .s cr
;

\ Return true if a plan is empty.
: plan-is-empty ( pln -- bool )
    \ Check arg.
    assert-tos-is-plan

    plan-get-step-list
    list-is-empty
;
