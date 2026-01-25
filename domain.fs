\ Implement a Domain struct and functions.

#31379 constant domain-id
    #8 constant domain-struct-number-cells

\ Struct fields
0                                   constant domain-header-disp         \ 16-bits [0] struct id, [1] use count, [2] instance id (8 bits), num-bits (8 bits)
domain-header-disp          cell+   constant domain-parent-session-disp \ A session.
domain-parent-session-disp  cell+   constant domain-actions-disp        \ A action-list
domain-actions-disp         cell+   constant domain-current-state-disp  \ A state/value.
domain-current-state-disp   cell+   constant domain-current-action-disp \ An action addr.
domain-current-action-disp  cell+   constant domain-max-region-disp     \ A region with all valid bits set to X.
domain-max-region-disp      cell+   constant domain-all-bits-mask-disp  \ A mask of all bits set to 1.
domain-all-bits-mask-disp   cell+   constant domain-ms-bit-mask-disp    \ A mask with the most significant bit set to one.


0 value domain-mma \ Storage for domain mma instance.

\ Init domain mma, return the addr of allocated memory.
: domain-mma-init ( num-items -- ) \ sets domain-mma.
    dup 1 <
    abort" domain-mma-init: Invalid number of items."

    cr ." Initializing Domain store."
    domain-struct-number-cells swap mma-new to domain-mma
;

\ Check domain mma usage.
: assert-domain-mma-none-in-use ( -- )
    domain-mma mma-in-use 0<>
    abort" domain-mma use GT 0"
;

\ Check instance type.
: is-allocated-domain ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup domain-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        domain-id =
    else
        drop false
    then
;

\ Check TOS for domain, unconventional, leaves stack unchanged.
: assert-tos-is-domain ( tos -- tos )
    dup is-allocated-domain
    is-false if
        s" TOS is not an allocated domain"
       .abort-xt execute
    then
;

' assert-tos-is-domain to assert-tos-is-domain-xt

\ Check NOS for domain, unconventional, leaves stack unchanged.
: assert-nos-is-domain ( nos tos -- nos tos )
    over is-allocated-domain
    is-false if
        s" NOS is not an allocated domain"
       .abort-xt execute
    then
;

' assert-nos-is-domain to assert-nos-is-domain-xt

\ Start accessors.

\ Return the action-list from an domain instance.
: domain-get-actions ( dom0 -- lst )
    \ Check arg.
    assert-tos-is-domain

    domain-actions-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the action-list from an domain instance.
: _domain-set-actions ( lst dom0 -- )
    \ Check arg.
    assert-tos-is-domain
    assert-nos-is-action-list

    domain-actions-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return the instance ID from an domain instance.
: domain-get-inst-id ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    \ Get intst ID.
    4c@
;

' domain-get-inst-id to domain-get-inst-id-xt

\ Set the instance ID of an domain instance.
: domain-set-inst-id ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    over 0<
    abort" Invalid instance id"

    over #255 >
    abort" Invalid instance id"

    \ Set inst id.
    4c!
;

\ Return the number bits used by a domain instance.
: domain-get-num-bits ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    \ Get intst ID.
    5c@
;

' domain-get-num-bits to domain-get-num-bits-xt

\ Set the number bits used by a domain instance, use only in this file.
: _domain-set-num-bits ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    over 1 <
    abort" Invalid number of bits."

    over #64 >
    abort" Invalid number of bits."

    \ Set inst id.
    5c!
;

\ Return the current state from a domain instance.
: domain-get-current-state ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    domain-current-state-disp +
    @
;

' domain-get-current-state to domain-get-current-state-xt

\ Set the current state of a domain instance.
: domain-set-current-state ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-value

    \ Set inst id.
    domain-current-state-disp +
    !
;

\ Return the current actien from a domain instance.
: domain-get-current-action ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    domain-current-action-disp +
    @
;

' domain-get-current-action to domain-get-current-action-xt

\ Set the current action of a domain instance.
: domain-set-current-action ( act1 dom0 -- )
    \ Check args.
    assert-tos-is-domain
    over 0<> if
        assert-nos-is-action
    then

    \ Set inst id.
    domain-current-action-disp +
    !
;

\ Return the parent session of the domain.
: domain-get-parent-session ( act0 -- dom )
    \ Check arg.
    assert-tos-is-domain

    domain-parent-session-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Set the parent session of an domain.
: _domain-set-parent-session ( dom act0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-parent-session-disp +    \ Add offset.
    !                               \ Set the field.
;

\ Return the max-region of the domain.
: domain-get-max-region ( dom0 -- reg-max )
    \ Check arg.
    assert-tos-is-domain

    domain-max-region-disp +    \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-max-region to domain-get-max-region-xt

\ Set the max region of the domain.
: _domain-set-max-region ( reg-max dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-max-region-disp +    \ Add offset.
    !struct                     \ Set the field.
;

\ Return the all-bits-mask of the domain.
: domain-get-all-bits-mask ( dom0 -- mask )
    \ Check arg.
    assert-tos-is-domain

    domain-all-bits-mask-disp +    \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-all-bits-mask to domain-get-all-bits-mask-xt

\ Set the max region of the domain.
: _domain-set-all-bits-mask ( mask1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-all-bits-mask-disp +    \ Add offset.
    !                               \ Set the field.
;

\ Return the ms-bit-mask of the domain.
: domain-get-ms-bit-mask ( dom0 -- mask )
    \ Check arg.
    assert-tos-is-domain

    domain-ms-bit-mask-disp +   \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-ms-bit-mask to domain-get-ms-bit-mask-xt

\ Set the max region of the domain.
: _domain-set-ms-bit-mask ( mask1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-ms-bit-mask-disp +   \ Add offset.
    !                           \ Set the field.
;

\ End accessors.

\ Create a domain, given the number of bits to be used.
\
\ The domain instance ID defaults to zero.
\ The instance ID will likely be reset to match its position in a list,
\ using domain-set-inst-id, which avoids duplicates and may be useful as an index into the list.
\
\ The current state defaults to zero, but can be set with domain-set-current-state.
: domain-new ( nb1 ses0 -- dom )
    \ Check arg.
    assert-tos-is-session-xt execute

    \ Allocate space.
    domain-mma mma-allocate         \ nb1 ses0 dom

    \ Store struct id.
    domain-id over                  \ nb1 ses0 dom id dom
    struct-set-id                   \ nb1 ses0 dom

    \ Init use count
    0 over struct-set-use-count     \ nb1 ses0 dom

    \ Set instance ID.
    over                            \ nb1 ses0 dom ses0
    session-get-number-domains-xt   \ nb1 ses0 dom ses0 xt
    execute                         \ nb1 ses0 dom nd
    over                            \ nb1 ses0 dom nd dom
    domain-set-inst-id              \ nb1 ses0 dom

    \ Set num bits.
    rot over                        \ ses0 dom nb1 dom
    _domain-set-num-bits            \ ses0 dom

    \ Set parent session field.
    tuck                            \ dom ses0 dom
    _domain-set-parent-session      \ dom

    \ Set actions list.
    list-new                        \ dom lst
    2dup swap                       \ dom lst lst dom
    _domain-set-actions             \ dom lst

    \ Add action 0.
    [ ' act-0-get-sample ] literal  \ dom lst xt
    #2 pick                         \ dom lst xt dom
    action-new dup                  \ dom lst act act
    rot                             \ dom act act lst
    action-list-push-end            \ dom act
    over domain-set-current-action  \ dom

    \ Set all bits mask.
    dup domain-get-num-bits \ dom u
    1-                      \ dom u'    Don't just take 2^n, as it might be the maximum number of bits.
    1 swap lshift           \ dom u''   Get most-significant-bit.
    1-                      \ dom u'''  Get all bits 1 except the msb.
    1 lshift                \ dom u'''' Get all bits 1 except the least-significant-bit.
    1+                      \ dom mask  Make lsb 1.
    over _domain-set-all-bits-mask

    \ Set max region.
    dup domain-get-all-bits-mask    \ dom msk
    0 region-new2                   \ dom regx
    over _domain-set-max-region     \ dom

    \ Set the most significant bit mask.
    dup domain-get-num-bits \ dom u
    1-                      \ dom u'    Don't just take 2^n, as it might be the maximum number of bits.
    1 swap lshift           \ dom mask
    over _domain-set-ms-bit-mask    \ dom

    \ Set arbitrary current state.
    0 over domain-current-state-disp + ! \ dom
;

\ Print a domain.
: .domain ( dom0 -- )
    \ Check arg.
    assert-tos-is-domain

    dup domain-get-inst-id
    cr cr ." Dom: " dec.

    dup domain-get-num-bits ." num-bits: " . space
    dup domain-get-actions
    list-get-length
    ."  num actions: " .
    dup domain-get-current-state ." cur: " .value
    cr
    domain-get-actions .action-list
;

\ Deallocate a domain.
: domain-deallocate ( act0 -- )
    \ Check arg.
    assert-tos-is-domain

    dup struct-get-use-count      \ act0 count

    #2 <
    if
        \ Clear fields.
        dup domain-get-actions action-list-deallocate
        dup domain-get-max-region region-deallocate

        \ Deallocate instance.
        domain-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: domain-add-action ( xt1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    tuck                        \ dom0 xt1 dom0

    action-new                  \ dom0 actx

    swap                        \ actx dom0
    2dup                        \ actx dom0 actx dom0
    domain-get-actions          \ actx dom0 actx act-lst
    action-list-push-end        \ actx dom0

    domain-set-current-action   \
;

\ Return true if two domains are equal.
: domain-eq ( grp1 grp0 -- flag )
     \ Check args.
    assert-tos-is-group
    assert-nos-is-group

    domain-get-inst-id
    swap
    domain-get-inst-id
    =
;

\ Get a sample fram an action in a domain.
\ Call only from session-get-sample, since current-domain in set there.
: domain-get-sample ( act1 dom0 -- sample )
     \ Check args.
    assert-tos-is-domain
    assert-nos-is-action

    \ Set domain current action.
    2dup domain-set-current-action  \ act1 dom0

    \ Get action sample.
    dup domain-get-current-state    \ act1 dom0 | d-sta
    #2 pick                         \ act1 dom0 | d-sta act1
    action-get-sample               \ act1 dom0 | smpl

    \ Set domain current state.
    dup sample-get-result           \ act1 dom0 | smpl sta
    #2 pick                         \ act1 dom0 | smpl sta dom
    domain-set-current-state        \ act1 dom0 | smpl

    over domain-get-inst-id cr ." Dom: " dec.      \ act1 dom0 | smpl
    #2 pick action-get-inst-id ." Act: " dec.      \ smpl
    dup .sample cr
    nip nip
;

' domain-get-sample to domain-get-sample-xt

\ Return true if a domain id matches a number.
: domain-id-eq ( id1 sqr0 -- flag )
    \ Check args.
    assert-tos-is-domain

    domain-get-inst-id
    =
;

: domain-get-needs ( reg1 dom0 -- ned-lst )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region

    \ dup domain-get-inst-id cr ." domain-get-needs: Dom: " .
    \ space ." reachable region: " over .region cr

    dup domain-get-current-state    \ reg1 dom0 sta
    swap                            \ reg1 sta dom0

    dup domain-get-actions          \ reg1 sta dom0 act-lst

    \ Init list to start appending action need lists to.
    list-new swap                   \ reg1 sta dom0 ret-lst act-lst

    \ Scan action-list, getting needs from each action.
    list-get-links                  \ reg1 sta dom0 ret-lst link
    begin
        ?dup
    while
        \ Set current action.
        dup link-get-data           \ reg1 sta dom0 ret-lst link actx
        #3 pick                     \ reg1 sta dom0 ret-lst link actx dom
        domain-set-current-action   \ reg1 sta dom0 ret-lst link

        \ Get action needs.
        #4 pick                     \ reg1 sta dom0 ret-lst link reg1
        #4 pick                     \ reg1 sta dom0 ret-lst link reg1 sta
        #2 pick link-get-data       \ reg1 sta dom0 ret-lst link reg1 sta actx
        action-get-needs            \ reg1 sta dom0 ret-lst link act-neds

        \ Add needs to return list.
        dup #3 pick                 \ reg1 sta dom0 ret-lst link act-neds act-neds ret-lst
        need-list-append            \ reg1 sta dom0 link act-neds
        need-list-deallocate        \ reg1 sta dom0 link

        link-get-next
    repeat
                                    \ reg1 sta dom0 ret-lst
    2nip nip                        \ ret-lst
;

\ Return a maximum region that might be reached, given the
\ current state and the aggregate changes of all action group rules.
: domain-calc-reachable-region ( dom0 -- r-reg )
    \ Check args.
    assert-tos-is-domain

    dup domain-get-actions          \ dom0 act-lst

    \ Init changes to start appending action changes to.
    0 0 changes-new swap            \ dom0 cng-agg act-lst

    \ Scan action-list, getting changes from each action.
    list-get-links                  \ dom0 cng-agg link
    begin
        ?dup
    while
        dup link-get-data          \ dom0 cng-agg link actx

        \ Set current action.
        dup #4 pick                 \ dom0 cng-agg link actx actx dom
        domain-set-current-action   \ dom0 cng-agg link actx

        \ Get aggregate action changes.
        action-calc-changes         \ dom0 cng-agg link act-cngs

        \ Aggregate changes.
        rot                         \ dom0 link act-cngs cng-agg
        2dup changes-calc-union     \ dom0 link act-cngs cng-agg cng-agg'

        \ Clean up.
        swap changes-deallocate     \ dom0 link act-cngs cng-agg'
        swap changes-deallocate     \ dom0 link cng-agg'
        swap                        \ dom0 cng-agg' link

        link-get-next
    repeat
                                    \ dom0 cng-agg
    swap domain-get-current-state   \ cng-agg sta
    2dup swap                       \ cng-agg sta sta cng-agg
    changes-apply-to-state          \ cng-agg sta sta'
    region-new                      \ cng-agg reg
    swap changes-deallocate         \ reg
;

\ Return a step forward, from an initial region,
\ towards the goal result region.
\ If more than one step is found, randomly choose one,
\ to support a random depth-first strategy.
: domain-calc-step-fc ( reg-to reg-from dom0 -- step true | false )
    \ cr ." domain-calc-step-fc: start: from " over .region space ." to: " #2 pick .region cr
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick swap                        \ | reg-from reg-to
    region-superset-of                          \ | bool
    abort" domain-calc-step-fc: region subset?" \ |


\    2dup region-superset-of                     \ | reg-to reg-from bool
\    abort" domain-calc-step-fc: region subset?" \ | reg-to reg-from
\    swap region-superset-of                     \ | bool
\    abort" domain-calc-step-fc: region subset?" \ |

    \ Init aggregate step list.
    list-new                        \ reg-to reg-from dom0 | stp-lst

    \ Get steps from each action.
    over domain-get-actions         \ | stp-lst act-lst
    list-get-links                  \ | stp-lst link
    begin
        ?dup
    while
        dup link-get-data               \ | stp-lst link actx
        dup                             \ | stp-lst link actx actx
        #4 pick                         \ | stp-lst link actx actx dom
        domain-set-current-action       \ | stp-lst link actx
        #5 pick swap                    \ | stp-lst link reg-to actx
        #5 pick swap                    \ | stp-lst link reg-to reg-from actx
        action-calc-plansteps-fc        \ | stp-lst link act-stps
        dup                             \ | stp-lst link act-stps act-stps
        #3 pick planstep-list-append    \ | stp-lst link act-stps
        planstep-list-deallocate        \ | stp-lst link

        link-get-next
    repeat
                                    \ reg-to reg-from dm0 | stp-lst
    \ cr ." Dom: " over domain-get-inst-id dec.
    \ space ." for: " #2 pick .region
    \ space ." to " #3 pick .region
    \ space ." Possible steps: " dup .step-list cr

    \ Clean up.
    2nip nip                        \ stp-lst

    \ Check for no steps.
    dup list-is-empty               \ stp-lst flag
    if
        planstep-list-deallocate
        false
        \ cr ." domain-calc-step-fc: returning false"
        exit
    then

    \ Generate a list of each different number of unwanted changes in steps.
    list-new                        \ stp-lst lst-unw
    over list-get-links             \ stp-lst lst-unw link
    begin
        ?dup
    while
        dup link-get-data                       \ stp-lst lst-unw link stpx
        planstep-get-number-unwanted-changes    \ stp-lst lst-unw link u-unw

        \ Check if the number is already in the list.
        [ ' = ] literal                     \ stp-lst lst-unw link u-uw xt
        over                                \ stp-lst lst-unw link u-uw xt u-unw
        #4 pick                             \ stp-lst lst-unw link u-uw xt u-unw lst-unw
        list-member                         \ stp-lst lst-unw link u-uw bool
        if                                  \ stp-lst lst-unw link u-uw
            drop                            \ stp-lst lst-unw link
        else                                \ stp-lst lst-unw link u-uw
            #2 pick                         \ stp-lst lst-unw link u-uw  lst-unw
            list-push                       \ stp-lst lst-unw link
        then

        link-get-next                       \ stp-lst lst-unw link
    repeat

    \ Sort the list of numbers, ascending   \ stp-lst lst-unw
    [ ' > ] literal over list-sort          \ stp-lst lst-unw

    \ Get first, lowest, number.
    0 over list-get-item                    \ stp-lst lst-unw u-unw
    swap list-deallocate                    \ stp-lst u-unw

    \ Get steps with lowest number unwanted changes.
    over planstep-list-match-number-unwanted-changes    \ stp-lst stp-lst2
    swap planstep-list-deallocate                       \ stp-lst2

    \ Pick a step.
    dup list-get-length             \ stp-lst2 len
    random                          \ stp-lst2 inx
    over                            \ stp-lst2 inx stp-lst

    \ Extract step.
    planstep-list-remove-item       \ stp-lst2, stpx true | false
    0= abort" Step not found?"      \ stp-lst2 stpx

    \ Clean up.                     \ stp-lst2 stpx
    swap planstep-list-deallocate   \ stpx
    \ cr ." domain-calc-step-fc: returning: " dup .step cr

    \ Return.
    true
;

\ Return a action, given a action ID.
: domain-find-action ( u1 dom0 -- act t | f )
    \ cr ." domain-find-action: Dom: " dup domain-get-inst-id . space over . cr
    \ Check args.
    assert-tos-is-domain
    over 0 < if
        2drop
        false
        exit
    then

    tuck domain-get-actions \ dom0 u1 act-lst
    2dup list-get-length    \ dom0 u1 act-lst u1 len
    >= if                   \ dom0 u1 act-lst
        3drop
        false
        exit
    then

    list-get-item               \ dom0 act
    tuck swap                   \ act act dom0
    domain-set-current-action   \ act
    true
;

\ Form a plan by getting successive steps closer, between
\ a from-region (tos) and a to-region (nos).
: domain-get-plan2-fc ( depth reg-to reg-from dom0 -- plan true | false )
    \ cr ." domain-get-plan2-fc: start: depth: " #3 pick dec. space ." from: " over .region space ." to: " #2 pick .region space ." dom: " dup domain-get-inst-id dec. cr
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #3 pick 0 #5 within is-false abort" invalid depth?"
    #2 pick #2 pick                             \ | reg-to reg-from
    swap region-superset-of                     \ | bool
    if
        over dup rule-new-region-to-region      \ | rul
        0 #2 pick                               \ | rul 0 dom
        domain-find-action                      \ | rul, act0 t | f
        is-false abort" Action 0 not found?"
        planstep-new                            \ | plnstp
        over plan-new                           \ | plnstp pln
        tuck plan-push                          \ | pln
        2nip nip nip
        true
        \ cr ." domain-get-plan2-fc: true exit 1" cr
        exit
    then
    \ abort" domain-get-plan2-fc: 2 region subset?" \ |

    \ Put read-only arguments at the bottom of the function's logical stack frame.
    over                            \ depth reg-to reg-from dom0 | reg-from

    \ Init return plan.
    over plan-new                   \ depth reg-to reg-from dom0 | reg-from pln
    swap                            \ depth reg-to reg-from dom0 | pln reg-from

    begin
        #4 pick                     \ depth reg-to reg-from dom0 | pln reg-from | reg-to
        over                        \ depth reg-to reg-from dom0 | pln reg-from | reg-to reg-from
        #4 pick                     \ depth reg-to reg-from dom0 | pln reg-from | reg-to reg-from dom0
        domain-calc-step-fc         \ depth reg-to reg-from dom0 | pln reg-from | stpx true | false

        is-false if                 \ depth reg-to reg-from dom0 | pln reg-from |
            \ cr ." domain-get-plan-fc: 1 returning f depth: " #4 pick . cr
            \ No step found, done.
            drop
            plan-deallocate
            2drop 2drop
            false
            \ cr ." domain-get-plan2-fc: false exit 2" cr
            exit
        then

        \ Check if step backtracks.
        dup                                     \ depth reg-to reg-from dom0 | pln reg-from | stpx stpx
        #3 pick                                 \ depth reg-to reg-from dom0 | pln reg-from | stpx stpx pln
        plan-check-step-result                  \ depth reg-to reg-from dom0 | pln reg-from | stpx bool
        if
            \ cr ." domain-get-plan-fc 2: returning f depth: " #5 pick . cr
            planstep-deallocate
            drop
            plan-deallocate
            2drop 2drop
            false
            \ cr ." domain-get-plan2-fc: false exit 3" cr
            exit
        then

        \ Check if step intersects reg-from.
                                                \ depth reg-to reg-from dom0 | pln reg-from | stpx
        over                                    \ depth reg-to reg-from dom0 | pln reg-from | stpx reg-from
        over planstep-get-initial-region        \ depth reg-to reg-from dom0 | pln reg-from | stpx reg-from stp-i
        region-intersects                       \ depth reg-to reg-from dom0 | pln reg-from | stpx bool
        if
            \ cr ." plan: " #2 pick .plan space ." add intersecting step fc: " dup .step cr
            \ Add intersecting step to plan.
                                            \ depth reg-to reg-from dom0 | pln reg-from | stpx
            #2 pick                         \ depth reg-to reg-from dom0 | pln reg-from | stpx pln
            \ Check if this is the first step.
            dup plan-is-empty
            if
                plan-push                       \ depth reg-to reg-from dom0 | pln reg-from |
                \ cr ." next interation of plan: " over .plan cr
                drop
                dup plan-get-result-region      \ depth reg-to reg-from dom0 | pln reg-from |
            else                                \ depth reg-to reg-from dom0 | pln reg-from | stpx pln
                2dup                            \ depth reg-to reg-from dom0 | pln reg-from | stpx pln stpx pln
                plan-link-step-to-result-region \ depth reg-to reg-from dom0 | pln reg-from | stpx pln, pln' t | f
                if                              \ depth reg-to reg-from dom0 | pln reg-from | stpx pln pln'
                    \ cr ." next interation of plan: " dup .plan cr
                    \ Replace previous plan with current plan.
                    nip nip                        \ depth reg-to reg-from dom0 | pln reg-from | pln'
                    rot                         \ depth reg-to reg-from dom0 | reg-from | pln' pln
                    plan-deallocate             \ depth reg-to reg-from dom0 | reg-from | pln'
                    nip                         \ depth reg-to reg-from dom0 | pln'
                    dup plan-get-result-region  \ depth reg-to reg-from dom0 | pln' reg-from |
                else                            \ depth reg-to reg-from dom0 | pln reg-from | stpx pln
                    \ cr ." domain-get-plan-fc: 3 returning f depth: " #6 pick . cr
                    \ plan link failed, done.
                    drop                        \ depth reg-to reg-from dom0 | pln reg-from | stpx
                    planstep-deallocate
                    drop
                    plan-deallocate
                    2drop 2drop
                    false
                    \ cr ." domain-get-plan2-fc: false exit 4" cr
                    exit
                then
            then
        else                                    \ depth reg-to reg-from dom0 | pln reg-from | stpx
            \ Process non-intersecting step.
                                                \ depth reg-to reg-from dom0 | pln reg-from | stpx
            \ cr ." plan: " #2 pick .plan space ." add extending step fc: " dup .planstep cr
            \ Set up for recursion.
            #6 pick 1-                          \ depth reg-to reg-from dom0 | pln reg-from | stpx | depth   ( -1 to prevent infinite recursion )
            over planstep-get-initial-region    \ depth reg-to reg-from dom0 | pln reg-from | stpx | depth stp-i
            #3 pick                             \ depth reg-to reg-from dom0 | pln reg-from | stpx | depth stp-i reg-from
            #6 pick                             \ depth reg-to reg-from dom0 | pln reg-from | stpx | depth stp-i reg-from dom
            \ cr ." calling domain-get-plan-fc depth " #3 pick . cr
            domain-get-plan-fc-xt execute       \ depth reg-to reg-from dom0 | pln reg-from | stpx | pln2 t | f
            if                                  \ depth reg-to reg-from dom0 | pln reg-from | stpx | pln2
                \ cr ." returned from domain-get-plan-fc: t " dup .plan space ." depth: " #6 pick . space ." continuing" cr
                swap planstep-deallocate        \ depth reg-to reg-from dom0 | pln reg-from | pln2
                #2 pick                         \ depth reg-to reg-from dom0 | pln reg-from | pln2 pln
                dup plan-is-empty               \ depth reg-to reg-from dom0 | pln reg-from | pln2 pln bool
                if                              \ depth reg-to reg-from dom0 | pln reg-from | pln2 pln
                    \ pln2 replaces pln.
                    drop                        \ depth reg-to reg-from dom0 | pln reg-from | pln2
                    nip                         \ depth reg-to reg-from dom0 | pln pln2
                    swap                        \ depth reg-to reg-from dom0 | pln2 pln
                    plan-deallocate             \ depth reg-to reg-from dom0 | pln2
                    dup plan-get-result-region  \ depth reg-to reg-from dom0 | pln2 reg-from

                else                                \ depth reg-to reg-from dom0 | pln reg-from | pln2 pln
                    2dup                            \ depth reg-to reg-from dom0 | pln reg-from | pln2 pln pln2 pln
                    plan-link                       \ depth reg-to reg-from dom0 | pln reg-from | pln2 pln, pln3 t | f
                    if
                        nip                         \ depth reg-to reg-from dom0 | pln reg-from | pln2 pln3
                        swap plan-deallocate        \ depth reg-to reg-from dom0 | pln reg-from | pln3
                        rot plan-deallocate         \ depth reg-to reg-from dom0 | reg-from pln3
                        nip                         \ depth reg-to reg-from dom0 | pln3
                        dup plan-get-result-region  \ depth reg-to reg-from dom0 | pln3 reg-from
                    else                            \ depth reg-to reg-from dom0 | pln reg-from | pln2 pln
                        \ cr ." domain-get-plan-fc: 4 returning f depth: " #6 pick . cr
                        drop
                        plan-deallocate
                        drop
                        plan-deallocate
                        2drop 2drop
                        false
                        \ cr ." domain-get-plan2-fc: false exit 5" cr
                        exit
                    then
                then
            else                                \ depth reg-to reg-from dom0 | pln reg-from | stpx
                \ cr ." domain-get-plan-fc: 5 returning f depth: " #5 pick . cr
                planstep-deallocate
                drop
                plan-deallocate
                2drop 2drop
                false
                \ cr ." domain-get-plan2-fc: false exit 6" cr
                exit
            then
        then

        \ Check if the plan result, the current reg-from, is a subset of the goal region.
                                            \ depth reg-to reg-from dom0 | pln' reg-from |
        \ cr ." domain-get-plan2-fc: Checking end of plan: " over .plan space ." cur reg-from: " dup .region space ." reg-to: " #4 pick .region space ." depth: " #5 pick . cr

        #4 pick                             \ depth reg-to reg-from dom0 | pln' reg-from | reg-to
        over                                \ depth reg-to reg-from dom0 | pln' reg-from | reg-to reg-from
        swap                                \ depth reg-to reg-from dom0 | pln' reg-from | reg-from reg-to
        \ cr ." reg-from: " over .region space ." reg-to: " dup .region cr
        region-superset-of                  \ depth reg-to reg-from dom0 | pln' reg-from | bool
        if
            \ Plan finished.
            drop                            \ depth reg-to reg-from dom0 | pln'
            \ cr ." domain-get-plan-fc: 6 returning t " dup .plan space ." depth: " #3 pick . cr

            \ Check results.
            #2 pick                         \ depth reg-to reg-from dom0 | pln' reg-from
            over plan-get-initial-region    \ depth reg-to reg-from dom0 | pln' reg-from pln-i
            region-superset-of              \ depth reg-to reg-from dom0 | pln' bool
            if
            else
                ." domain-get-plan2-fc: invalid plan initial region."
                abort
            then

            dup plan-get-result-region      \ depth reg-to reg-from dom0 | pln' pln-r
            #4 pick                         \ depth reg-to reg-from dom0 | pln' pln-r reg-to
            region-superset-of              \ depth reg-to reg-from dom0 | pln' bool
            if
            else
                ." domain-get-plan2-fc: invalid plan result region."
                abort
            then

            \ Clean up.
            2nip nip nip                    \ pln

            \ Return.
            true
            \ cr ." domain-get-plan2-fc: true exit 7" cr
            exit
        then

    again
;

\ Using a random depth-first forward-chaining strategy. Try a number
\ of times to find a plan to accomplish a desired sample.
: domain-get-plan-fc ( depth reg-to reg-from dom0 -- plan true | false )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #3 pick 0 #5 within is-false abort" invalid depth?"
\    cr ." domain-get-plan-fc: " .stack-structs-xt execute cr

    \ Check depth.
    #3 pick 1 < if
        cr ." domain-get-plan-fc: Depth exceeded." cr
        2drop 2drop
        false
        exit
    then

    #3 0 do
        #3 pick #3 pick #3 pick #3 pick

        domain-get-plan2-fc         \ depth reg-to reg-from dom0 | pln t | f
        if                          \ depth reg-to reg-from dom0 | pln
            \ Clean up.
            2nip nip nip            \ pln
            \ Return.
            true                    \ pln t
            unloop
            exit
        then
    loop
                                    \ depth reg-to reg-from dom0
    \ Clean up.
    2drop 2drop
    \ Return.
    false
;

' domain-get-plan-fc to domain-get-plan-fc-xt

: domain-calc-step-bc ( reg-to reg-from dom0 -- step true | false )
    \ cr ." domain-calc-step-bc: start: reg-to: " #2 pick .region space ." reg-from: " over .region cr
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                             \ | reg-to reg-from
    swap region-superset-of                     \ | bool
    if
        over dup rule-new-region-to-region      \ | rul
        0 #2 pick                               \ | rul 0 dom
        domain-find-action                      \ | rul, act0 t | f
        is-false abort" Action 0 not found?"
        planstep-new                            \ | plnstp
        2nip nip                                \ plnstp
        true
        exit
    then
    \ abort" domain-calc-step-bc: 2 region subset?" \ |
    \ cr ." at 1: " .stack-structs-xt execute cr

    \ Init aggregate step list.
    list-new                                    \ | stp-lst

    \ Get steps from each action.
    over domain-get-actions         \ | stp-lst act-lst
    list-get-links                  \ | stp-lst link
    begin
        ?dup
    while                               \ | stp-lst link
        dup link-get-data               \ | stp-lst link actx
        dup                             \ | stp-lst link actx actx
        #4 pick                         \ | stp-lst link actx actx dom
        domain-set-current-action       \ | stp-lst link actx
        #5 pick swap                    \ | stp-lst link reg-to actx
        #5 pick swap                    \ | stp-lst link reg-to reg-from actx
        action-calc-plansteps-bc        \ | stp-lst link act-stps
        dup                             \ | stp-lst link act-stps act-stps
        #3 pick planstep-list-append    \ | stp-lst link act-stps
        planstep-list-deallocate        \ | stp-lst link

        link-get-next
    repeat
                                    \ reg-to reg-from stp-lst dom0
    \ cr ." Dom: " dup domain-get-inst-id .
    \ space ." for: " 2 pick .sample
    \ space ." Possible steps: " over .step-list cr

    \ Clean up.
    2nip nip                        \ stp-lst

    \ Check for no steps.
    dup list-is-empty               \ stp-lst flag
    if
        planstep-list-deallocate
        false
        exit
    then

    \ Generate a list of each different number of unwanted changes in steps.
    list-new                        \ stp-lst lst-unw
    over list-get-links             \ stp-lst lst-unw link
    begin
        ?dup
    while
        dup link-get-data                       \ stp-lst lst-unw link stpx
        planstep-get-number-unwanted-changes    \ stp-lst lst-unw link u-unw

        \ Check if the number is already in the list.
        [ ' = ] literal                     \ stp-lst lst-unw link u-uw xt
        over                                \ stp-lst lst-unw link u-uw xt u-unw
        #4 pick                             \ stp-lst lst-unw link u-uw xt u-unw lst-unw
        list-member                         \ stp-lst lst-unw link u-uw bool
        if                                  \ stp-lst lst-unw link u-uw
            drop                            \ stp-lst lst-unw link
        else                                \ stp-lst lst-unw link u-uw
            #2 pick                         \ stp-lst lst-unw link u-uw  lst-unw
            list-push                       \ stp-lst lst-unw link
        then

        link-get-next                       \ stp-lst lst-unw link
    repeat


    \ Sort the list of numbers, ascending   \ stp-lst lst-unw
    [ ' > ] literal over list-sort          \ stp-lst lst-unw

    \ Get first, lowest, number.
    0 over list-get-item                    \ stp-lst lst-unw u-unw
    swap list-deallocate                    \ stp-lst u-unw

    \ Get steps with lowest number unwanted changes.
    over planstep-list-match-number-unwanted-changes    \ stp-lst stp-lst2
    swap planstep-list-deallocate                       \ stp-lst2

    \ Pick a step.
    dup list-get-length             \ stp-lst2 len
    random                          \ stp-lst2 inx
    over                            \ stp-lst2 inx stp-lst

    \ Extract step.
    planstep-list-remove-item           \ stp-lst2, stpx true | false
    0= abort" domain-calc-step-bc: Step not found?"      \ stp-lst2 stpx

    \ Clean up.                     \ stp-lst2 stpx
    swap planstep-list-deallocate   \ stpx
    \ cr ." domain-calc-step-bc: returning: " dup .step cr

    \ Return.
    true
;

\ Form a plan by getting successive steps closer, between
\ a sample result state to a sample initial state.
: domain-get-plan2-bc ( depth reg-to reg-from dom0 -- plan true | false )
    \ cr ." domain-get-plan2-bc: start: depth: " #3 pick . space ." reg-to: " #2 pick .region space ." reg-from: " over .region cr
    \ Check args.
    assert-tos-is-domain
    assert-3os-is-region
    #3 pick 0 #5 within is-false abort" invalid depth?"
    #2 pick #2 pick                             \ | reg-to reg-from
    swap region-superset-of                     \ | bool
    abort" domain-get-plan2-bc: 2 region subset?" \ |

    \ Put read-only arguments at the bottom of the function's logical stack frame.
    #2 pick                         \ depth reg-to reg-from dom0 | reg-to

    \ Init return plan.
    over plan-new                   \ depth reg-to reg-from dom0 | reg-to pln
    swap                            \ depth reg-to reg-from dom0 | pln reg-to

    begin
        \ cr ." top begin: " .stack-structs-xt execute cr
        \ cr ." top begin: to " dup .region space ." from: " #3 pick .region cr

        dup                         \ depth reg-to reg-from dom0 | pln reg-to | reg-to
        #4 pick                     \ depth reg-to reg-from dom0 | pln reg-to | reg-to reg-from
        #4 pick                     \ depth reg-to reg-from dom0 | pln reg-to | reg-to reg-from dom0
        domain-calc-step-bc         \ depth reg-to reg-from dom0 | pln reg-to | stpx true | false
        \ cr ." after calc-step-bc: " .stack-structs-xt execute cr

        is-false if                 \ depth reg-to reg-from dom0 | pln reg-to |
            \ cr ." domain-get-plan2-bc: 1 returning f depth: " #4 pick . cr
            \ No step found, done.
            drop
            plan-deallocate
            2drop 2drop
            false
            \ cr ." domain-get-plan2-bc: loop: false exit 1" cr
            exit
        then

        \ Check if step backtracks.
        dup                                     \ depth reg-to reg-from dom0 | pln reg-to | stpx stpx
        #3 pick                                 \ depth reg-to reg-from dom0 | pln reg-to | stpx stpx pln
        plan-check-step-initial                 \ depth reg-to reg-from dom0 | pln reg-to | stpx bool
        if
            \ cr ." domain-get-plan2-bc: 2 returning f depth: " #5 pick . cr
            planstep-deallocate
            drop
            plan-deallocate
            2drop 2drop
            false
            \ cr ." domain-get-plan2-bc: loop: false exit 2" cr
            exit
        then

        \ Check if step intersects reg-to.
                                                \ depth reg-to reg-from dom0 | pln reg-to | stpx
        over                                    \ depth reg-to reg-from dom0 | pln reg-to | stpx reg-to
        over planstep-get-result-region         \ depth reg-to reg-from dom0 | pln reg-to | stpx reg-from stp-r
        region-intersects                       \ depth reg-to reg-from dom0 | pln reg-to | stpx bool
        if
            \ ." add intersecting step bc: " dup .planstep space ." plan: " #2 pick .plan cr
            \ Add intersecting step to plan.
                                            \ depth reg-to reg-from dom0 | pln reg-to | stpx
            #2 pick                         \ depth reg-to reg-from dom0 | pln reg-to | stpx pln
            \ Check if this is the first step.
            dup plan-is-empty
            if
                plan-push                       \ depth reg-to reg-from dom0 | pln reg-to |
                \ cr ." next interation of plan: " over .plan cr
                drop
                dup plan-get-initial-region     \ depth reg-to reg-from dom0 | pln reg-to |
            else                                \ depth reg-to reg-from dom0 | pln reg-to | stpx pln
                2dup                            \ depth reg-to reg-from dom0 | pln reg-to | stpx pln stpx pln
                plan-link-step-to-initial-region \ depth reg-to reg-from dom0 | pln reg-to | stpx pln, pln' t | f
                if                              \ depth reg-to reg-from dom0 | pln reg-to | stpx pln pln'
                    \ cr ." next interation of plan: " dup .plan cr
                    \ Replace previous plan with current plan.
                    nip nip                     \ depth reg-to reg-from dom0 | pln reg-to | pln'
                    rot                         \ depth reg-to reg-from dom0 | reg-to | pln' pln
                    plan-deallocate             \ depth reg-to reg-from dom0 | reg-to | pln'
                    nip                         \ depth reg-to reg-from dom0 | pln'
                    dup plan-get-initial-region \ depth reg-to reg-from dom0 | pln' reg-to |
                else                            \ depth reg-to reg-from dom0 | pln reg-to | stpx pln
                    \ cr ." domain-get-plan2-bc: 3 returning f depth: " #6 pick . cr
                    \ plan link failed, done.
                    drop                        \ depth reg-to reg-from dom0 | pln reg-to | stpx
                    planstep-deallocate
                    drop
                    plan-deallocate
                    2drop 2drop
                    false
                    \ cr ." domain-get-plan2-bc: loop: false exit 3" cr
                    exit
                then
            then
        else                                        \ depth reg-to reg-from dom0 | pln reg-to | stpx
        \ Process non-intersecting step.
                                                    \ depth reg-to reg-from dom0 | pln reg-to | stpx
            \ cr ." plan: " #2 pick .plan space ." add extending step bc: " dup .planstep cr
            \ Set up for recursion.
            #6 pick 1-                              \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth   ( -1 to prevent infinite recursion )
            over planstep-get-result-region         \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth stp-r
            #3 pick                                 \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth stp-r reg-to
            swap                                    \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth reg-to stp-r
            #6 pick                                 \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth reg-to stp-r dom
            \ cr ." calling domain-get-plan-bc depth " #3 pick . cr
            domain-get-plan-bc-xt execute           \ depth reg-to reg-from dom0 | pln reg-to | stpx | pln2 t | f
            if                                      \ depth reg-to reg-from dom0 | pln reg-to | stpx | pln2
                \ cr ." domain-get-plan2-bc: 3.1 returned t " dup .plan space ." depth: " #6 pick . space ." continuing" cr
                swap planstep-deallocate            \ depth reg-to reg-from dom0 | pln reg-to | pln2
                #2 pick                             \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln
                dup plan-is-empty                   \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln bool
                if                                  \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln
                    \ pln2 replaces pln.
                    drop                            \ depth reg-to reg-from dom0 | pln reg-to | pln2
                    nip                             \ depth reg-to reg-from dom0 | pln pln2
                    swap                            \ depth reg-to reg-from dom0 | pln2 pln
                    plan-deallocate                 \ depth reg-to reg-from dom0 | pln2
                    dup plan-get-initial-region     \ depth reg-to reg-from dom0 | pln2 reg-to
                else                                \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln
                    2dup                            \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln pln2 pln
                    swap                            \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln pln pln2
                    plan-link                       \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln, pln3 t | f
                    if
                        nip                         \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln3
                        swap plan-deallocate        \ depth reg-to reg-from dom0 | pln reg-to | pln3
                        rot plan-deallocate         \ depth reg-to reg-from dom0 | reg-to pln3
                        nip                         \ depth reg-to reg-from dom0 | pln3
                        dup plan-get-initial-region \ depth reg-to reg-from dom0 | pln3 reg-to-next
                    else                            \ depth reg-to reg-from dom0 | pln reg-to | pln2 pln
                        \ cr ." domain-get-plan2-bc: 4 returning f depth: " #6 pick . cr
                        drop
                        plan-deallocate
                        drop
                        plan-deallocate
                        2drop 2drop
                        false
                        \ cr ." domain-get-plan2-bc: loop: false exit 4" cr
                        exit
                    then
                then
            else                                \ depth reg-to reg-from dom0 | pln reg-to | stpx
                \ cr ." domain-get-plan2-bc: 5 returning f depth: " #6 pick . cr
                planstep-deallocate
                drop
                plan-deallocate
                2drop 2drop
                false
                \ cr ." domain-get-plan2-bc: loop: false exit 5" cr
                exit
            then
        then

        \ Check if the plan result, the current reg-from, is a subset of the from region.
                                            \ depth reg-to reg-from dom0 | pln' reg-to |
        \ cr ." domain-get-plan2-bc: Checking end of plan: " over .plan space ." cur reg-to: " dup .region space ." reg-from: " #3 pick .region space ." depth: " #5 pick . cr

        #3 pick                             \ depth reg-to reg-from dom0 | pln' reg-to | reg-from
        over                                \ depth reg-to reg-from dom0 | pln' reg-to | reg-from reg-to
        \ swap                                \ depth reg-to reg-from dom0 | pln' reg-to | reg-to reg-from
        region-superset-of                  \ depth reg-to reg-from dom0 | pln' reg-to | bool
        if
            \ Plan finished.
            drop                            \ depth reg-to reg-from dom0 | pln'

            \ Check results.
            #2 pick                         \ depth reg-to reg-from dom0 | pln' reg-from
            over plan-get-initial-region    \ depth reg-to reg-from dom0 | pln' reg-from pln-i
            region-superset-of              \ depth reg-to reg-from dom0 | pln' bool
            if
            else
                ." domain-get-plan2-bc: invalid plan initial region."
                abort
            then

            dup plan-get-result-region      \ depth reg-to reg-from dom0 | pln' pln-r
            #4 pick                         \ depth reg-to reg-from dom0 | pln' pln-r reg-to
            region-superset-of              \ depth reg-to reg-from dom0 | pln' bool
            if
            else
                ." domain-get-plan2-bc: invalid plan result region."
                abort
            then

                                            \ depth reg-to reg-from dom0 | pln'
            #2 pick                         \ depth reg-to reg-from dom0 | pln' reg-from
            over                            \ depth reg-to reg-from dom0 | pln' reg-from pln'
            plan-restrict-initial-region    \ depth reg-to reg-from dom0 | pln', pln'' t | f
            if                              \ depth reg-to reg-from dom0 | pln' pln''
                swap plan-deallocate        \ depth reg-to reg-from dom0 | pln''
                2nip nip nip
                true
                \ cr ." domain-get-plan2-bc: loop: true exit 6" cr
                exit
            else                            \ depth reg-to reg-from dom0 | pln'
                plan-deallocate             \ depth reg-to reg-from dom0
                2drop
                2drop
                false
                \ cr ." domain-get-plan2-bc: loop: false exit 7" cr
                exit
            then
        then

    again
;

\ Using a random depth-first backward-chaining strategy. Try a number
\ of times to find a plan to accomplish a desired sample.
: domain-get-plan-bc ( depth reg-to reg-from dom0 -- plan true | false )
    \ cr ." domain-get-plan-bc: " .stack-structs-xt execute cr
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #3 pick 0 #5 within is-false abort" invalid depth?"

    \ Check depth.
    #3 pick 1 < if
        cr ." domain-get-plan-bc: Depth exceeded." cr
        2drop 2drop
        false
        exit
    then

    #3 0 do
        #3 pick #3 pick #3 pick #3 pick         \ depth reg-to reg-from dom0 |
        domain-get-plan2-bc                     \ depth reg-to reg-from dom0 | pln t | f
        if                                      \ depth reg-to reg-from dom0 | pln
            \ Clean up.
            2nip nip nip                        \ pln
            \ Return.
            true                                \ pln t
            unloop
            exit
        then
    loop
                                    \ reg-to reg-from dom0

    \ Clean up.
    2drop 2drop
    \ Return.
    false
;

' domain-get-plan-bc to domain-get-plan-bc-xt

\ Try forward and backward chaining to make a plan
\ for going from an initial region to a non-intersecting result region.
: domain-get-plan-fb ( reg-to reg-from dom0 -- plan true | false )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region

    #2 random
    if
        \ Try forward-chaining first.
        #3                          \ reg-to reg-from dom0 | 3
        #3 pick #3 pick #3 pick     \ reg-to reg-from dom0 | 3 reg-to reg-from dom0
        domain-get-plan-fc          \ reg-to reg-from dom0 | pln t | f
        \ cr ." todo chng back to fc" cr
        if
            2nip nip
            cr ." plan found (fc) " dup .plan cr
            true
            exit
        then
        \ Try backward-chaining second.
        #3                          \ reg-to reg-from dom0 | 3
        #3 pick #3 pick #3 pick     \ reg-to reg-from dom0 | 3 reg-to reg-from dom0
        domain-get-plan-bc          \ reg-to reg-from dom0 | pln t | f
        if
            2nip nip
            cr ." plan found (bc*) " dup .plan cr
            true
            exit
        then
    else
        \ Try backward-chaining first.
        #3                          \ reg-to reg-from dom0 | 3
        #3 pick #3 pick #3 pick     \ reg-to reg-from dom0 | 3 reg-to reg-from dom0
        domain-get-plan-bc         \ reg-to reg-from dom0 | pln t | f
        if
            2nip nip
            cr ." plan found (bc) " dup .plan cr
            true
            exit
        then
        \ Try forward-chaining second.
        #3                          \ reg-to reg-from dom0 | 3
        #3 pick #3 pick #3 pick     \ reg-to reg-from dom0 | 3 reg-to reg-from dom0
        domain-get-plan-fc          \ reg-to reg-from dom0 | p t | f
        \ cr ." todo chng back to fc" cr
        if
            2nip nip
            cr ." plan found (fc*) " dup .plan cr
            true
            exit
        then
    then
    3drop
    false
;

\ Return a list of all steps that can make at least one needed change.
: domain-calc-steps-by-changes ( reg-to reg-from dom0 -- stp-lst )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." domain-calc-steps-by-changes: reg-to: " #2 pick .region space ." reg-from: " over .region cr

    \ Init return list.
    list-new                            \ reg-to reg-from dom0 stp-lst

    \ Get steps from each action.
    over domain-get-actions             \ reg-to reg-from dom0 stp-lst act-lst
    list-get-links                      \ reg-to reg-from dom0 stp-lst act-link
    begin
        ?dup
    while                               \ reg-to reg-from dom0 stp-lst act-link |
        dup link-get-data               \ | actx
        dup                             \ | actx actx
        #4 pick                         \ | actx actx dom
        domain-set-current-action       \ | actx
        #5 pick #5 pick rot             \ | reg-to reg-from actx
        action-calc-plansteps-by-changes    \ | act-stps'
        dup                             \ | act-stps' act-stps'
        #3 pick planstep-list-append    \ | act-stps'
        planstep-list-deallocate        \ |

        link-get-next
    repeat
                                        \ reg-to reg-from dom0 stp-lst
    2nip                                \ dom0 stp-lst
    nip                                 \ stp-lst
;

\ Asymmetric chaining.
\ Get rules for each required single bit change.
\ Find steps that change a single bit and contain no rules that intersect a from-region or goal-region.
\ Randomly choose one of those steps.
\ Try making a plan that goes from the from-region to the step initial-region.
\ Restrict the steps initial-region.
\ Try making a plan that goes from the step result-region to the goal region.
: domain-asymmetric-chaining ( reg-to reg-from dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." domain-asymmetric-chaining: reg-to: " #2 pick .region space ." reg-from: " over .region cr

    \ Find an asymmetric rule.
    #2 pick #2 pick                         \ reg-to reg-from dom0 | reg-to reg-from
    #2 pick                                 \ reg-to reg-from dom0 | reg-to reg-from dom0
    domain-calc-steps-by-changes            \ reg-to reg-from dom0 | plnstp-lst'
    dup list-is-empty if
        list-deallocate
        3drop
        false
        exit
    then

    \ Prep for loop by single-bit change.   \ reg-to reg-from dom0 | plnstp-lst'
    #3 pick #3 pick                         \ reg-to reg-from dom0 | plnstp-lst' reg-to reg-from
    changes-new-region-to-region            \ reg-to reg-from dom0 | plnstp-lst' cngs'
    
    dup changes-split                       \ reg-to reg-from dom0 | plnstp-lst' cngs' cng-lst'
    swap changes-deallocate                 \ reg-to reg-from dom0 | plnstp-lst' cng-lst'

    list-new swap                           \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst'
    dup list-get-links                      \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst' cng-link

    \ For each single-bit change, find steps that do not contain the sample initial states
    \ without any alternate steps that do.
    begin
        ?dup
    while                                   \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst' cng-link

        \ Find steps that provide the one-bit change, but possibly others.
        dup link-get-data                   \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst' cng-link cngx
        #4 pick                             \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst' cng-link cngx plnstp-lst'
        planstep-list-intersects-changes    \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst' cng-link plnstp-cng-lst'

        \ Check if the one-bit change is possible, else done.
        dup list-is-empty                   \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst' cng-link plnstp-cng-lst' bool
        if                                  \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst' cng-link plnstp-cng-lst'
        cr ." exit 2" cr
            planstep-list-deallocate        \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst' cng-link
            drop                            \ reg-to reg-from dom0 | plnstp-lst' asym-lst cng-lst'
            changes-list-deallocate         \ reg-to reg-from dom0 | plnstp-lst' asym-lst
            planstep-list-deallocate        \ reg-to reg-from dom0 | plnstp-lst'
            planstep-list-deallocate        \ reg-to reg-from dom0 |
            3drop
            false
            exit
        then

        \ Check if there are only steps that do not match the reg-to reg-from initial state.
        #7 pick                                 \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link plnstp-cng-lst' reg-to
        #7 pick                                 \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link plnstp-cng-lst' reg-to reg-from
        #2 pick                                 \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link plnstp-cng-lst' reg-to reg-from plnstp-cng-lst'
        planstep-list-any-from-to-intersections \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link plnstp-cng-lst' bool
        if
            planstep-list-deallocate            \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link
        else
            \ Save the steps.
            dup #4 pick                         \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link plnstp-cng-lst' plnstp-cng-lst' asym-lst
            planstep-list-append                \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link plnstp-cng-lst'
            planstep-list-deallocate            \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link
        then

        link-get-next                       \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst cng-link
    repeat

    \ Clean up.                             \ reg-to reg-from dom0 | plnstp-lst asym-lst cng-lst
    changes-list-deallocate                 \ reg-to reg-from dom0 | plnstp-lst asym-lst
    swap planstep-list-deallocate           \ reg-to reg-from dom0 | asym-lst

    \ Check list len.
    dup list-is-empty                     \ reg-to reg-from dom0 | asym-lst bool
    if
        planstep-list-deallocate            \ reg-to reg-from dom0 |
        3drop
        false
        exit
    then

    \ Randomly choose a step.
    dup list-get-length                     \ reg-to reg-from dom0 | asym-lst len
    random                                  \ reg-to reg-from dom0 | asym-lst inx
    over list-get-item                      \ reg-to reg-from dom0 | asym-lst stpx

    \ Get plan1 reg-from to step initial region.
    dup planstep-get-initial-region         \ reg-to reg-from dom0 | asym-lst stpx plnstp-i
    #4 pick                                 \ reg-to reg-from dom0 | asym-lst stpx plnstp-i reg-from
    #4 pick                                 \ reg-to reg-from dom0 | asym-lst stpx plnstp-i reg-from dom0
    domain-get-plan-fb                      \ reg-to reg-from dom0 | asym-lst stpx, plan1' t | f
    is-false if                             \ reg-to reg-from dom0 | asym-lst stpx
        drop
        planstep-list-deallocate
        3drop
        false
        exit
    then

    \ Link plan to step.                \ reg-to reg-from dom0 | asym-lst stpx plan1'
    swap over                           \ reg-to reg-from dom0 | asym-lst plan1' stpx plan1'
    plan-link-step-to-result-region     \ reg-to reg-from dom0 | asym-lst plan1', plan2' t | f
    is-false if                         \ reg-to reg-from dom0 | asym-lst plan1'
        plan-deallocate
        planstep-list-deallocate
        3drop
        false
        exit
    then

    \ Clean up.                         \ reg-to reg-from dom0 | asym-lst plan1' plan2'
    swap plan-deallocate
    swap planstep-list-deallocate       \ reg-to reg-from dom0 | plan2'

    \ Get plan part 2.
    #3 pick                             \ reg-to reg-from dom0 | plan2' reg-to
    over plan-get-result-region         \ reg-to reg-from dom0 | plan2' reg-to pln-r

    #3 pick                             \ reg-to reg-from dom0 | plan2' reg-to pln-r dom0
    domain-get-plan-fb                  \ reg-to reg-from dom0 | plan2', plan3' t | f
    is-false if                         \ reg-to reg-from dom0 | plan2'
        plan-deallocate
        3drop
        false
        exit
    then

    \ Link plan2 to plan3                   \ reg-to reg-from dom0 | plan2', plan3'
    swap                                    \ reg-to reg-from dom0 | plan3' plan2'
    2dup plan-link                          \ reg-to reg-from dom0 | plan3' plan2', plan3' t | f
    if                                      \ reg-to reg-from dom0 | plan3' plan2' plan3'
        swap plan-deallocate
        swap plan-deallocate
        2nip nip
        true
    else                                    \ reg-to reg-from dom0 | plan3' plan2'
        plan-deallocate
        plan-deallocate
        3drop
        false
    then
;
\ Get a plan for going between an initial region and a non-intersecting result region.
: domain-get-plan ( reg-to reg-from dom0 -- plan true | false )
    \ cr ." domain-get-plan: start: from " over .region space ." to " #2 pick .region cr 
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    
    #2 pick #2 pick swap                        \ | reg-from reg-to
    region-superset-of                          \ | bool
    if
        over dup rule-new-region-to-region      \ | rul
        0 #2 pick                               \ | rul 0 dom
        domain-find-action                      \ | rul, act0 t | f
        is-false abort" Action 0 not found?"
        planstep-new                            \ | plnstp
        over plan-new                           \ | plnstp pln
        tuck plan-push                          \ | pln
        2nip nip                                \ pln
        true
        exit
    then

    \ Check if its even possible.
    #2 pick #2 pick                             \ | reg-to reg-from
    changes-new-region-to-region                \ | cngs'
    #3 pick #3 pick                             \ | cngs' reg-to reg-from
    #3 pick                                     \ | cngs' reg-to reg-from dom0
    domain-calc-steps-by-changes                \ | cngs' plnstp-lst'
    cr ." steps: " dup .planstep-list cr
    dup planstep-list-union-changes             \ | cngs' plnstp-lst' plnstp-cngs'
    swap planstep-list-deallocate               \ | cngs' plnstp-cngs'
    cr ." cngs: " over .changes space ." planstep-list cngs: " dup .changes cr
    2dup                                        \ | cngs' plnstp-cngs' cngs' plnstp-cngs'
    changes-superset-of                         \ | cngs' plnstp-cngs' bool
    swap changes-deallocate                     \ | cngs' bool
    swap changes-deallocate                     \ | bool
    if
    else
        3drop
        false
        exit
    then

    3dup domain-get-plan-fb            \ reg-to reg-from dom0, plan t | f
    if
        2nip nip true                   \ plan t
    else
        domain-asymmetric-chaining      \ plan t | f
        if
            cr ." plan found (asm) " dup .plan cr
            true
        else
            false
        then
    then
;

\ Set the current domain.
: domain-set-current ( dom0 -- )
    \ Check arg.
    assert-tos-is-domain

    dup domain-get-parent-session
    session-set-current-domain-xt execute
;

' domain-set-current to domain-set-current-xt

: domain-get-number-actions ( dom -- na )
    \ Check arg.
    assert-tos-is-domain

    domain-get-actions      \ act-lst
    list-get-length         \ len
;

' domain-get-number-actions to domain-get-number-actions-xt

\ Return the complement af a state.
: domain-state-complement ( u1 dom0 -- list )
    \ Check arg.
    assert-tos-is-domain
    assert-nos-is-value

    domain-get-max-region               \ u1 reg-max

    region-subtract-state               \ list
;

\ Return ~A + ~B for a state pair.
: domain-state-pair-complement ( u2 u1 dom0 -- list )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-value
    assert-3os-is-value

    tuck                            \ u2 dom0 u1 dom0
    domain-state-complement         \ u2 dom0 comp1

    -rot                            \ comp1 u2 dom0
    domain-state-complement         \ comp1 comp2

    2dup region-list-set-union      \ comp1 comp2 list-u

    swap region-list-deallocate     \ comp1 list-u

    swap region-list-deallocate     \ list-u
;

' domain-state-pair-complement to domain-state-pair-complement-xt

