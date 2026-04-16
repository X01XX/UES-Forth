\ Implement a Domain struct and functions.

#31379 constant domain-id
    #8 constant domain-struct-number-cells

\ Struct fields
0                                   constant domain-header-disp         \ 16-bits [0] struct id, [1] use count, [2] instance id (8 bits), num-bits (8 bits)
domain-header-disp          cell+   constant domain-parent-session-disp \ A session.  Domain does not change its use count.
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

\ Check instance type.
: is-allocated-domain ( addr -- flag )
    get-first-word          \ w t | f
    if
        domain-id =
    else
        false
    then
;

\ Check TOS for domain, unconventional, leaves stack unchanged.
: assert-tos-is-domain ( tos -- tos )
    dup is-allocated-domain
    false? if
        s" TOS is not an allocated domain"
       .abort-xt execute
    then
;

' assert-tos-is-domain to assert-tos-is-domain-xt

\ Check NOS for domain, unconventional, leaves stack unchanged.
: assert-nos-is-domain ( nos tos -- nos tos )
    over is-allocated-domain
    false? if
        s" NOS is not an allocated domain"
       .abort-xt execute
    then
;

' assert-nos-is-domain to assert-nos-is-domain-xt

\ Start accessors.

\ Return the parent session of the domain.
: domain-get-parent-session ( dom0 -- ses )
    \ Check arg.
    assert-tos-is-domain

    domain-parent-session-disp + \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-parent-session to domain-get-parent-session-xt

\ Set the parent session of an domain.
: _domain-set-parent-session ( ses1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-parent-session-disp +    \ Add offset.
    !                               \ Set the field.
;

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

    over cell #8 * >
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

\ Return the max-region of the domain.
: domain-get-max-region ( dom0 -- reg )
    \ Check arg.
    assert-tos-is-domain

    domain-max-region-disp +    \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-max-region to domain-get-max-region-xt

\ Set the max region of the domain.
: _domain-set-max-region ( reg1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-max-region-disp +    \ Add offset.
    !struct                     \ Set the field.
;

\ Return the all-bits-mask of the domain.
: domain-get-all-bits-mask ( dom0 -- msk )
    \ Check arg.
    assert-tos-is-domain

    domain-all-bits-mask-disp +    \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-all-bits-mask to domain-get-all-bits-mask-xt

\ Set the max region of the domain.
: _domain-set-all-bits-mask ( msk1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    domain-all-bits-mask-disp +    \ Add offset.
    !                               \ Set the field.
;

\ Return the ms-bit-mask of the domain.
: domain-get-ms-bit-mask ( dom0 -- msk )
    \ Check arg.
    assert-tos-is-domain

    domain-ms-bit-mask-disp +   \ Add offset.
    @                           \ Fetch the field.
;

' domain-get-ms-bit-mask to domain-get-ms-bit-mask-xt

\ Set the max region of the domain.
: _domain-set-ms-bit-mask ( msk1 dom0 -- )
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

    \ Set instance ID, based on its position in the session domain list.
    over                            \ nb1 ses0 dom sess0
    session-get-number-domains-xt   \ nb1 ses0 dom sess0 xt
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
    \ When making multi-step plans of all regions, a no-op for one domain preserves
    \ knowledge of all result states for subsequent steps.
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
: domain-deallocate ( dom0 -- )
    \ Check arg.
    assert-tos-is-domain

    dup struct-get-use-count      \ act0 count
    dup 0< abort" invalid use count"

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

\ Update parent session points counter.
: domain-update-session-points ( dom -- )
    \ Check args.
    assert-tos-is-domain

    domain-get-parent-session       \ sess
    session-update-points-xt        \ xt
    execute                         \
;

\ Get a sample from an action in a domain.
\ Call only from session-get-sample, since current-domain in set there.
: domain-get-sample ( act1 dom0 -- smpl )
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

\    cr
\    over domain-get-inst-id cr ." Dom: " #3 dec.r   \ act1 dom0 | smpl
\    space #2 pick action-get-inst-id ." Act: " #3 dec.r   \ smpl
\    space dup .sample
\    cr

    swap                            \ act1 smpl dom
    domain-update-session-points    \ act1 smpl

    nip
;

' domain-get-sample to domain-get-sample-xt

\ Get a sample from an action in a domain, for a step.
: domain-get-sample-step ( act1 dom0 -- smpl )
     \ Check args.
    assert-tos-is-domain
    assert-nos-is-action

    \ Set domain current action.
    2dup domain-set-current-action  \ act1 dom0

    \ Get action sample.
    dup domain-get-current-state    \ act1 dom0 | d-sta
    #2 pick                         \ act1 dom0 | d-sta act1
    action-get-sample-step          \ act1 dom0 | smpl

    \ Set domain current state.
    dup sample-get-result           \ act1 dom0 | smpl sta
    #2 pick                         \ act1 dom0 | smpl sta dom
    domain-set-current-state        \ act1 dom0 | smpl

    swap                            \ act1 smpl dom
    domain-update-session-points    \ act1 smpl

\    cr
\    over domain-get-inst-id cr ." Dom: " #3 dec.r   \ act1 dom0 | smpl
\    space #2 pick action-get-inst-id ." Act: " #3 dec.r   \ smpl
\    space dup .sample
\    cr

    nip
;

' domain-get-sample-step to domain-get-sample-step-xt

\ Return true if a domain id matches a number.
: domain-id-eq ( id1 dom0 -- flag )
    \ Check args.
    assert-tos-is-domain

    domain-get-inst-id
    =
;

: domain-get-needs ( dom0 -- ned-lst )
    \ Check args.
    assert-tos-is-domain

    \ dup domain-get-inst-id cr ." domain-get-needs: Dom: " .

    dup domain-get-current-state    \ dom0 sta
    swap                            \ sta dom0

    dup domain-get-actions          \ sta dom0 act-lst

    \ Init list to start appending action need lists to.
    list-new swap                   \ sta dom0 ret-lst act-lst

    \ Scan action-list, getting needs from each action.
    list-get-links                  \ sta dom0 ret-lst link

    begin
        ?dup
    while
        \ Set current action.
        dup link-get-data           \ sta dom0 ret-lst link actx
        #3 pick                     \ sta dom0 ret-lst link actx dom
        domain-set-current-action   \ sta dom0 ret-lst link

        \ Get action needs.
        #3 pick                     \ sta dom0 ret-lst link sta
        over link-get-data          \ sta dom0 ret-lst link sta actx
        action-get-needs            \ sta dom0 ret-lst link act-neds'

        \ Add needs to return list.
        dup #3 pick                 \ sta dom0 ret-lst link act-neds' act-neds' ret-lst
        need-list-append            \ sta dom0 ret-lst link act-neds'
        need-list-deallocate        \ sta dom0 ret-lst link

        link-get-next
    repeat
                                    \ sta dom0 ret-lst
    nip nip                         \ ret-lst
;

\ Return a list of all steps that can make at least one needed change.
: domain-calc-possible-steps ( reg-to reg-from dom0 -- plnstp-lst t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." domain-calc-possible-steps: reg-to: " #2 pick .region space ." reg-from: " over .region cr

    #2 pick #2 pick swap                    \ | reg-to reg-from
    region-superset?                        \ | bool
    abort" domain-calc-possible-steps: region subset?"

    \ Calc changes needed.
    #2 pick #2 pick                         \ | reg-to reg-from
    changes-new-region-to-region            \ | cngs'

    \ Init aggregate step list.
    list-new                                \ | cngs' stp-lst

    \ Get steps from each action.
    #2 pick domain-get-actions              \ | cngs' stp-lst act-lst
    list-get-links                          \ | cngs' stp-lst link
    begin
        ?dup
    while
        \ Set current action.
        dup link-get-data                   \ | cngs' stp-lst link actx
        #4 pick                             \ | cngs' stp-lst link actx dom
        domain-set-current-action           \ | cngs' stp-lst link

        \ Get steps from action.
        #2 pick                             \ | cngs' stp-lst link cngs'
        #6 pick #6 pick                     \ | cngs' stp-lst link cngs' reg-to reg-from
        #3 pick link-get-data               \ | cngs' stp-lst link cngs' reg-to reg-from actx
        action-calc-possible-steps          \ | cngs' stp-lst link act-stps'

        \ Store steps is return list.
        dup                                 \ | cngs' stp-lst link act-stps' act-stps'
        #3 pick planstep-list-append        \ | cngs' stp-lst link act-stps'
        planstep-list-deallocate            \ | cngs' stp-lst link

        link-get-next
    repeat
                                    \ reg-to reg-from dm0 | cngs' stp-lst

\    cr ." Dom: " #2 pick domain-get-inst-id #3 dec.r
\    space ." domain-calc-possible-steps: for: " #3 pick .region
\    space ." to " #4 pick .region
\    space ." Possible steps: " dup .planstep-list cr

    \ Clean up.                             \ reg-to reg-from dom0 | cngs' stp-lst
    swap changes-deallocate                 \ reg-to reg-from dom0 | stp-lst
    2nip nip                                \ stp-lst

    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
;

\ Return a step forward, from an initial region,
\ towards the goal result region.
\ If more than one step is found, randomly choose one,
\ to support a random depth-first strategy.
: domain-calc-step-fc ( reg-to reg-from dom0 -- step t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region

    #2 pick #2 pick                         \ reg-to reg-from dom0 | reg-to reg-from
    region-subset?                          \ reg-to reg-from dom0 | bool
    if
        \ cr ." region: " over .region space ." subset of: " #2 pick .region cr
        3drop
        false
        exit
    then

    domain-calc-possible-steps              \ stp-lst t | f
    \ Check for no steps.
    false?
    if
        false
        \ cr ." domain-calc-step-fc: returning false" cr
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
    dup list-get-first-item                 \ stp-lst lst-unw u-unw
    swap list-deallocate                    \ stp-lst u-unw

    \ Get steps with lowest number unwanted changes.
    over planstep-list-match-number-unwanted-changes    \ stp-lst stp-lst2
    swap planstep-list-deallocate                       \ stp-lst2

    \ Pick a step.
    dup list-get-length             \ stp-lst2 len
    random                          \ stp-lst2 inx
    over                            \ stp-lst2 inx stp-lst

    \ Extract step.
    planstep-list-remove-item       \ stp-lst2 stpx

    \ Clean up.                     \ stp-lst2 stpx
    swap planstep-list-deallocate   \ stpx
    \ cr ." domain-calc-step-fc: returning: " dup .step cr

\    cr ." Dom: " current-domain-id #3 dec.r
\    space ." domain-calc-step-fc: step: " dup .planstep
\    cr

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
: domain-get-plan2-fc ( depth reg-to reg-from dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #3 pick 0 < abort" invalid depth?"
    #2 pick #2 pick                                 \ | reg-to reg-from
    swap region-subset?                             \ | bool
    abort" domain-get-plan2-fc: Already at goal"    \ |
    \ cr ." domain-get-plan2-fc: start: depth: " #3 pick dec. space ." from: " over .region space ." to: " #2 pick .region space ." dom: " dup domain-get-inst-id dec. cr

    \ Check depth.
    #3 pick 0=
    if
        \ cr ." Depth exceeded." cr
        2drop 2drop
        false
        \ cr ." domain-get-plan2-fc: false exit 1" cr
        exit
    then

    \ Init return plan.
    dup plan-new                   \ depth reg-to reg-from dom0 | pln

    \ Establish current reg-from region, for loop.
    #2 pick                        \ depth reg-to reg-from dom0 | pln reg-from

    begin
        \ Get next step.
        #4 pick                     \ depth reg-to reg-from dom0 | pln reg-from | reg-to
        over                        \ depth reg-to reg-from dom0 | pln reg-from | reg-to reg-from
        #4 pick                     \ depth reg-to reg-from dom0 | pln reg-from | reg-to reg-from dom0
        domain-calc-step-fc         \ depth reg-to reg-from dom0 | pln reg-from | stpx t | f
        \ Return if no step.
        false? if                   \ depth reg-to reg-from dom0 | pln reg-from |
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
        over                                        \ depth reg-to reg-from dom0 | pln reg-from | stpx reg-from
        over planstep-get-initial-region            \ depth reg-to reg-from dom0 | pln reg-from | stpx reg-from stp-i
        region-intersects?                          \ depth reg-to reg-from dom0 | pln reg-from | stpx bool
        if                                          \ depth reg-to reg-from dom0 | pln reg-from | stpx
            \ Add intersecting step to plan.
                                                    \ depth reg-to reg-from dom0 | pln reg-from | stpx

            \ Check if this is the first step.
            #2 pick                                 \ depth reg-to reg-from dom0 | pln reg-from | stpx2 pln
            plan-is-empty                           \ depth reg-to reg-from dom0 | pln reg-from | stpx2 bool
            if                                      \ depth reg-to reg-from dom0 | pln reg-from | stpx2
                \ Restrict step.
                2dup                                \ depth reg-to reg-from dom0 | pln reg-from | stpx reg-from stpx
                planstep-restrict-initial-region    \ depth reg-to reg-from dom0 | pln reg-from | stpx stpx2
                swap planstep-deallocate            \ depth reg-to reg-from dom0 | pln reg-from | stpx2
                #2 pick                             \ depth reg-to reg-from dom0 | pln reg-from | stpx2 pln
                plan-push                           \ depth reg-to reg-from dom0 | pln reg-from |
                drop                                \ depth reg-to reg-from dom0 | pln
            else                                    \ depth reg-to reg-from dom0 | pln reg-from | stpx
                dup                                 \ depth reg-to reg-from dom0 | pln reg-from | stpx stpx
                #3 pick                             \ depth reg-to reg-from dom0 | pln reg-from | stpx stpx pln
                plan-link-step-to-result-region     \ depth reg-to reg-from dom0 | pln reg-from | stpx, pln t | f
                if                                  \ depth reg-to reg-from dom0 | pln reg-from | stpx, pln2
                    swap planstep-deallocate        \ depth reg-to reg-from dom0 | pln reg-from | pln2
                    nip                             \ depth reg-to reg-from dom0 | pln pln2
                    swap plan-deallocate            \ depth reg-to reg-from dom0 | pln2
                else                                \ depth reg-to reg-from dom0 | pln reg-from | stpx
                    planstep-deallocate             \ depth reg-to reg-from dom0 | pln reg-from
                    drop                            \ depth reg-to reg-from dom0 | pln
                    plan-deallocate                 \ depth reg-to reg-from dom0
                    2drop 2drop
                    false
                    exit
                then
            then
            dup plan-get-result-region          \ depth reg-to reg-from dom0 | pln reg-from |
        else                                    \ depth reg-to reg-from dom0 | pln reg-from | stpx
            \ Process non-intersecting step.
                                                \ depth reg-to reg-from dom0 | pln reg-from | stpx
            \ Set up for recursion.
            #6 pick 1-                          \ depth reg-to reg-from dom0 | pln reg-from | stpx | depth   ( -1 to prevent infinite recursion )
            over planstep-get-initial-region    \ depth reg-to reg-from dom0 | pln reg-from | stpx | depth stp-i
            #3 pick                             \ depth reg-to reg-from dom0 | pln reg-from | stpx | depth stp-i reg-from
            #6 pick                             \ depth reg-to reg-from dom0 | pln reg-from | stpx | depth stp-i reg-from dom

            recurse                                 \ depth reg-to reg-from dom0 | pln reg-from | stpx, pln2 t | f

            if                                      \ depth reg-to reg-from dom0 | pln reg-from | stpx | pln2
                \ cr ." returned from domain-get-plan-fc: t " dup .plan space ." depth: " #6 pick . space ." continuing" cr
                swap planstep-deallocate            \ depth reg-to reg-from dom0 | pln reg-from | pln2
                #2 pick plan-is-empty               \ depth reg-to reg-from dom0 | pln reg-from | pln2 bool
                if                                  \ depth reg-to reg-from dom0 | pln reg-from | pln2
                    \ pln2 replaces pln.
                    nip                             \ depth reg-to reg-from dom0 | pln pln3
                    swap plan-deallocate            \ depth reg-to reg-from dom0 | pln3
                else                                \ depth reg-to reg-from dom0 | pln reg-from | pln2
                    nip                             \ depth reg-to reg-from dom0 | pln pln2
                    2dup                            \ depth reg-to reg-from dom0 | pln pln2 pln pln2
                    swap                            \ depth reg-to reg-from dom0 | pln pln2 pln2 pln
                    plan-link                       \ depth reg-to reg-from dom0 | pln pln2, pln3 t | f
                    if                              \ depth reg-to reg-from dom0 | pln pln2 pln3
                        swap plan-deallocate        \ depth reg-to reg-from dom0 | pln pln3
                        swap plan-deallocate        \ depth reg-to reg-from dom0 | pln3
                    else                            \ depth reg-to reg-from dom0 | pln pln2
                        \ cr ." domain-get-plan-fc: 4 returning f depth: " #6 pick . cr
                        plan-deallocate             \ depth reg-to reg-from dom0 | pln
                        plan-deallocate             \ depth reg-to reg-from dom0 |
                        2drop 2drop
                        false
                        \ cr ." domain-get-plan2-fc: false exit 5" cr
                        exit
                    then
                then
                dup plan-get-result-region          \ depth reg-to reg-from dom0 | pln3 reg-from
            else                                    \ depth reg-to reg-from dom0 | pln reg-from | stpx
                planstep-deallocate
                drop
                plan-deallocate
                2drop 2drop
                false
                \ cr ." domain-get-plan2-fc: false exit 6" cr
                exit
            then
        then

        \ cr ." current plan: " over .plan cr

        \ Check if the plan result, the current reg-from, is a subset of the goal region.
                                            \ depth reg-to reg-from dom0 | pln' reg-from |
        \ cr ." domain-get-plan2-fc: Checking end of plan: " over .plan space ." cur reg-from: " dup .region space ." reg-to: " #4 pick .region space ." depth: " #5 pick . cr

        #4 pick                             \ depth reg-to reg-from dom0 | pln' reg-from | reg-to
        over                                \ depth reg-to reg-from dom0 | pln' reg-from | reg-to reg-from
        swap                                \ depth reg-to reg-from dom0 | pln' reg-from | reg-from reg-to
        \ cr ." reg-from: " over .region space ." reg-to: " dup .region cr
        region-superset?                    \ depth reg-to reg-from dom0 | pln' reg-from | bool
        if
            \ Plan finished.
            drop                            \ depth reg-to reg-from dom0 | pln'
            \ cr ." domain-get-plan-fc: 6 returning t " dup .plan space ." depth: " #3 pick . cr

            \ Check results.
            #2 pick                         \ depth reg-to reg-from dom0 | pln' reg-from
            over plan-get-initial-region    \ depth reg-to reg-from dom0 | pln' reg-from pln-i
            region-superset?                \ depth reg-to reg-from dom0 | pln' bool
            if
            else
                ." domain-get-plan2-fc: invalid plan initial region."
                abort
            then

            dup plan-get-result-region      \ depth reg-to reg-from dom0 | pln' pln-r
            #4 pick                         \ depth reg-to reg-from dom0 | pln' pln-r reg-to
            region-superset?                \ depth reg-to reg-from dom0 | pln' bool
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
: domain-get-plan-fc ( depth reg-to reg-from dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #3 pick 0 < abort" Invalid depth?"
    \ cr ." domain-get-plan-fc:  start: depth: " #3 pick dec. space ." from: " over .region space ." to: " #2 pick .region space ." dom: " dup domain-get-inst-id dec. cr

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

: domain-calc-step-bc ( reg-to reg-from dom0 -- step t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." domain-calc-step-bc: start: reg-to: " #2 pick .region space ." reg-from: " over .region cr

    over #3 pick                           \ reg-to reg-from dom0 | reg-from reg-to
    region-subset?                         \ reg-to reg-from dom0 | bool
    if
        cr ." region: " #2 pick .region space ." subset of: " over .region cr
        3drop
        false
        exit
    then

    domain-calc-possible-steps              \ stp-lst t | f

    \ Check for no steps.
    false?
    if
        false
        exit
    then

    \ Generate a list of each different number of unwanted changes in steps.
    list-new                                \ stp-lst lst-unw
    over list-get-links                     \ stp-lst lst-unw link
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
    dup list-get-first-item                 \ stp-lst lst-unw u-unw
    swap list-deallocate                    \ stp-lst u-unw

    \ Get steps with lowest number unwanted changes.
    over planstep-list-match-number-unwanted-changes    \ stp-lst stp-lst2
    swap planstep-list-deallocate                       \ stp-lst2

    \ Pick a step.
    dup list-get-length             \ stp-lst2 len
    random                          \ stp-lst2 inx
    over                            \ stp-lst2 inx stp-lst

    \ Extract step.
    planstep-list-remove-item           \ stp-lst2 stpx

    \ Clean up.                     \ stp-lst2 stpx
    swap planstep-list-deallocate   \ stpx
    \ cr ." domain-calc-step-bc: returning: " dup .step cr

    \ Return.
    true
;

\ Form a plan by getting successive steps closer, between
\ a sample result state to a sample initial state.
: domain-get-plan2-bc ( depth reg-to reg-from dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #3 pick 0 < abort" invalid depth?"
    #2 pick #2 pick                                 \ | reg-to reg-from
    swap region-subset?                             \ | bool
    abort" domain-get-plan2-bc: Already at goal"    \ |
    \ cr ." domain-get-plan2-bc: start: depth: " #3 pick dec. space ." from: " over .region space ." to: " #2 pick .region space ." dom: " dup domain-get-inst-id dec. cr

    \ Check depth.
    #3 pick 0=
    if
        \ cr ." Depth exceeded." cr
        2drop 2drop
        false
        \ cr ." domain-get-plan2-bc: false exit 1" cr
        exit
    then

    \ Init return plan.
    dup plan-new                                    \ depth reg-to reg-from dom0 | pln

    \ Establish current reg-to region, for loop.
    #3 pick                                         \ depth reg-to reg-from dom0 | pln reg-to

    begin
        \ Get next step.
        dup                                         \ depth reg-to reg-from dom0 | pln reg-to | reg-to
        #4 pick                                     \ depth reg-to reg-from dom0 | pln reg-to | reg-to reg-from
        #4 pick                                     \ depth reg-to reg-from dom0 | pln reg-to | reg-to reg-from dom0
        domain-calc-step-bc                         \ depth reg-to reg-from dom0 | pln reg-to | stpx t | f
        \ cr ." after calc-step-bc: " .stack-gbl cr

        false? if                                   \ depth reg-to reg-from dom0 | pln reg-to |
            \ No step found, done.
            drop
            plan-deallocate
            2drop 2drop
            false
            \ cr ." domain-get-plan2-bc: false exit 2" cr
            exit
        then

        \ Check if step intersects reg-to.
                                                    \ depth reg-to reg-from dom0 | pln reg-to | stpx
        over                                        \ depth reg-to reg-from dom0 | pln reg-to | stpx reg-to
        over planstep-get-result-region             \ depth reg-to reg-from dom0 | pln reg-to | stpx reg-from stp-r
        region-intersects?                          \ depth reg-to reg-from dom0 | pln reg-to | stpx bool
        if
            \ Add intersecting step to plan.
            #2 pick                                 \ depth reg-to reg-from dom0 | pln reg-to | stpx pln
            \ Check if this is the first step.
            plan-is-empty                           \ depth reg-to reg-from dom0 | pln reg-to | stpx bool
            if
                \ Restrict step.
                2dup                                \ depth reg-to reg-from dom0 | pln reg-to | stpx reg-to stpx
                planstep-restrict-result-region     \ depth reg-to reg-from dom0 | pln reg-to | stpx stpx2
                swap planstep-deallocate            \ depth reg-to reg-from dom0 | pln reg-to | stpx2
                #2 pick                             \ depth reg-to reg-from dom0 | pln reg-to | stpx2 pln
                plan-push                           \ depth reg-to reg-from dom0 | pln reg-to |
                \ cr ." next interation of plan: " over .plan cr
                drop                                \ depth reg-to reg-from dom0 | pln
            else                                    \ depth reg-to reg-from dom0 | pln reg-to | stpx
                dup                                 \ depth reg-to reg-from dom0 | pln reg-to | stpx stpx
                #3 pick                             \ depth reg-to reg-from dom0 | pln reg-to | stpx stpx pln
                plan-link-step-to-initial-region    \ depth reg-to reg-from dom0 | pln reg-to | stpx, pln2 t | f
                if                                  \ depth reg-to reg-from dom0 | pln reg-to | stpx pln2
                    swap planstep-deallocate        \ depth reg-to reg-from dom0 | pln reg-to | pln2
                    \ Replace previous plan with current plan.
                    nip                             \ depth reg-to reg-from dom0 | pln pln2
                    swap plan-deallocate            \ depth reg-to reg-from dom0 | pln2
                else                                \ depth reg-to reg-from dom0 | pln reg-to | stpx
                    \ cr ." domain-get-plan2-bc: 3 returning f depth: " #6 pick . cr
                    \ plan link failed, done.
                    planstep-deallocate
                    drop
                    plan-deallocate
                    2drop 2drop
                    false
                    \ cr ." domain-get-plan2-bc: false exit 4" cr
                    exit
                then
            then
            dup plan-get-initial-region             \ depth reg-to reg-from dom0 | pln reg-to |
        else                                        \ depth reg-to reg-from dom0 | pln reg-to | stpx
        \ Process non-intersecting step.
                                                    \ depth reg-to reg-from dom0 | pln reg-to | stpx
            \ Set up for recursion.
            #6 pick 1-                              \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth   ( -1 to prevent infinite recursion )
            #2 pick                                 \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth reg-to
            #2 pick planstep-get-result-region      \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth reg-to stp-r
            #6 pick                                 \ depth reg-to reg-from dom0 | pln reg-to | stpx | depth reg-to stp-r dom
            \ cr ." calling domain-get-plan-bc depth " #3 pick . cr
            recurse                                 \ depth reg-to reg-from dom0 | pln reg-to | stpx | pln2 t | f
            if                                      \ depth reg-to reg-from dom0 | pln reg-to | stpx | pln2
                swap planstep-deallocate            \ depth reg-to reg-from dom0 | pln reg-to | pln2
                #2 pick plan-is-empty               \ depth reg-to reg-from dom0 | pln reg-to | pln2 bool
                if                                  \ depth reg-to reg-from dom0 | pln reg-to | pln2
                    \ Restrict plan2
                    tuck                            \ depth reg-to reg-from dom0 | pln pln2 reg-to pln2
                    plan-restrict-result-region     \ depth reg-to reg-from dom0 | pln pln2, pln3 t | f
                    if                              \ depth reg-to reg-from dom0 | pln pln2 pln3
                        \ pln2 replaces pln.
                        swap plan-deallocate        \ depth reg-to reg-from dom0 | pln pln3
                        swap plan-deallocate        \ depth reg-to reg-from dom0 | pln3
                    else                            \ depth reg-to reg-from dom0 | pln pln2
                        plan-deallocate
                        plan-deallocate
                        2drop 2drop
                        false
                        exit
                    then
                else                                \ depth reg-to reg-from dom0 | pln reg-to | pln2
                    nip                             \ depth reg-to reg-from dom0 | pln pln2
                    2dup                            \ depth reg-to reg-from dom0 | pln pln2 pln pln2
                    plan-link                       \ depth reg-to reg-from dom0 | pln pln2, pln3 t | f
                    if
                        swap plan-deallocate        \ depth reg-to reg-from dom0 | pln pln3
                        swap plan-deallocate        \ depth reg-to reg-from dom0 | pln3
                    else                            \ depth reg-to reg-from dom0 | pln pln2
                        plan-deallocate
                        plan-deallocate
                        2drop 2drop
                        false
                        \ cr ." domain-get-plan2-bc: false exit 5" cr
                        exit
                    then
                then
                dup plan-get-initial-region         \ depth reg-to reg-from dom0 | pln3 reg-to-next
            else                                    \ depth reg-to reg-from dom0 | pln reg-to | stpx
                planstep-deallocate
                drop
                plan-deallocate
                2drop 2drop
                false
                \ cr ." domain-get-plan2-bc: loop: false exit 6" cr
                exit
            then
        then

        \ Check if the plan result, the current reg-from, is a subset of the from region.
                                            \ depth reg-to reg-from dom0 | pln' reg-to |
        \ cr ." domain-get-plan2-bc: Checking end of plan: " over .plan space ." cur reg-to: " dup .region space ." reg-from: " #3 pick .region space ." depth: " #5 pick . cr

        #3 pick                             \ depth reg-to reg-from dom0 | pln' reg-to | reg-from
        over                                \ depth reg-to reg-from dom0 | pln' reg-to | reg-from reg-to
        region-intersects?                  \ depth reg-to reg-from dom0 | pln' reg-to | bool
        if
            \ Plan finished.
            drop                            \ depth reg-to reg-from dom0 | pln'

            \ Restrict plan initial region to reg-from.
            #2 pick                         \ depth reg-to reg-from dom0 | pln' reg-from
            over                            \ depth reg-to reg-from dom0 | pln' reg-from pln'
            plan-restrict-initial-region    \ depth reg-to reg-from dom0 | pln', pln2 t | f
            if                              \ depth reg-to reg-from dom0 | pln' pln2
                swap plan-deallocate        \ depth reg-to reg-from dom0 | pln2
            else                            \ depth reg-to reg-from dom0 | pln'
                cr ." plan restrict initial failed?" cr
                cr ." reg: " #2 pick .region space dup .plan cr
                abort
            then

            \ Check results.
            #2 pick                         \ depth reg-to reg-from dom0 | pln' reg-from
            over plan-get-initial-region    \ depth reg-to reg-from dom0 | pln' reg-from pln-i
            region-subset?                  \ depth reg-to reg-from dom0 | pln' bool
            if
            else
                ." domain-get-plan2-bc: invalid plan initial region."
                cr dup .plan cr
                abort
            then

            dup plan-get-result-region      \ depth reg-to reg-from dom0 | pln' pln-r
            #4 pick                         \ depth reg-to reg-from dom0 | pln' pln-r reg-to
            region-superset?                \ depth reg-to reg-from dom0 | pln' bool
            if
            else
                cr dup .plan cr
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
                \ cr ." domain-get-plan2-bc: loop: true exit 7" cr
                exit
            else                            \ depth reg-to reg-from dom0 | pln'
                plan-deallocate             \ depth reg-to reg-from dom0
                2drop
                2drop
                false
                \ cr ." domain-get-plan2-bc: loop: false exit 8" cr
                exit
            then
        then

    again
;

\ Using a random depth-first backward-chaining strategy. Try a number
\ of times to find a plan to accomplish a desired sample.
: domain-get-plan-bc ( depth reg-to reg-from dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    #3 pick 0 < abort" Invalid depth?"
    \ cr ." domain-get-plan-bc:  start: depth: " #3 pick dec. space ." from: " over .region space ." to: " #2 pick .region space ." dom: " dup domain-get-inst-id dec. cr

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
: domain-get-plan-fb ( reg-to reg-from dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." domain-get-plan-fb:  start:           from: " over .region space ." to: " #2 pick .region space ." dom: " dup domain-get-inst-id dec. cr

    #2 random
    if
        \ Try forward-chaining first.
        #3                          \ reg-to reg-from dom0 | 3
        #3 pick #3 pick #3 pick     \ reg-to reg-from dom0 | 3 reg-to reg-from dom0
        domain-get-plan-fc          \ reg-to reg-from dom0 | pln t | f
        \ cr ." todo chng back to fc" cr
        if
            2nip nip
            \ cr ." plan found (fc) " dup .plan cr
            true
            exit
        then
        \ Try backward-chaining second.
        #3                          \ reg-to reg-from dom0 | 3
        #3 pick #3 pick #3 pick     \ reg-to reg-from dom0 | 3 reg-to reg-from dom0
        domain-get-plan-bc          \ reg-to reg-from dom0 | pln t | f
        if
            2nip nip
            \ cr ." plan found (bc*) " dup .plan cr
            true
            exit
        then
    else
        \ Try backward-chaining first.
        #3                          \ reg-to reg-from dom0 | 3
        #3 pick #3 pick #3 pick     \ reg-to reg-from dom0 | 3 reg-to reg-from dom0
        domain-get-plan-bc          \ reg-to reg-from dom0 | pln t | f
        if
            2nip nip
            \ cr ." plan found (bc) " dup .plan cr
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
            \ cr ." plan found (fc*) " dup .plan cr
            true
            exit
        then
    then
    3drop
    false
;

\ Asymmetric chaining.
\ Find needed single-bit changes, that can only be made with one rule,
\ a "chokepoint", and do not intersect the from-to regions.
\ Try making a plan that goes from the current state, to the rule, then to the goal.
: domain-asymmetric-chaining ( reg-to reg-from dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." domain-asymmetric-chaining: start:    from: " over .region space ." to: " #2 pick .region space ." dom: " dup domain-get-inst-id dec. cr

    \ Get possible steps containing a needed change.
    #2 pick #2 pick                         \ reg-to reg-from dom0 | reg-to reg-from
    #2 pick                                 \ reg-to reg-from dom0 | reg-to reg-from dom0
    domain-calc-possible-steps              \ reg-to reg-from dom0 | plnstp-lst' t | f
    if
       \ cr dup list-get-length . space ." pos asym steps found" cr
    else
        3drop
        false
        \ cr ." asym exit 1 false" cr
        exit
    then

    \ Find steps that make each needed bit change.
    \ If only one step is found, and there is no intersection with reg-to and reg-from, save it.
                                            \ reg-to reg-from dom0 | plnstp-lst'

    \ Init list for selected plansteps.
    list-new                                \ reg-to reg-from dom0 | plnstp-lst' sel-lst'

    \ Get isolated change bits.
    #4 pick #4 pick                         \ reg-to reg-from dom0 | plnstp-lst' asym-lst' reg-to reg-from
    changes-new-region-to-region            \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs'
    dup changes-split                       \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs' cngs-lst'
    swap changes-deallocate                 \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst'

    \ For each change bit,
    dup list-get-links                      \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk

    begin
        ?dup
    while
        \ Get change with isolated bit.
        dup link-get-data                   \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk cngx

        \ Get pathsteps that have the change.
        #4 pick                             \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk cngx plnstp-lst'
        planstep-list-change-intersection   \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk, cng-plnstp-lst' t | f
        if
            \ Check list length.
            dup list-get-length 1 =
            if
                \ Check if the planstep intersects reg-to or reg-from.
                #7 pick #7 pick                 \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk cng-plnstp-lst' reg-to reg-from
                #2 pick list-get-first-item     \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk cng-plnstp-lst' reg-to reg-from plnstrpx
                planstep-any-intersection?      \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk cng-plnstp-lst' bool
                if
                else
                    dup list-get-first-item     \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk cng-plnstp-lst' plnstpx
                    #4 pick                     \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk cng-plnstp-lst' plnstpx asym-lst
                    list-push-struct            \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk cng-plnstp-lst'
                then
            then
            planstep-list-deallocate         \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst' cngs-lnk
        then

        link-get-next
    repeat
                                            \ reg-to reg-from dom0 | plnstp-lst' asym-lst' cngs-lst'
    changes-list-deallocate                 \ reg-to reg-from dom0 | plnstp-lst' asym-lst'
    swap planstep-list-deallocate           \ reg-to reg-from dom0 | asym-lst'

    \ Check if any found.
    dup list-is-empty?
    if
        \ cr ." no asym steps found" cr
        list-deallocate
        2drop drop
        false
        \ cr ." asym exit 2 false" cr
        exit
    then

    \ cr ." all steps found: " dup .planstep-list cr

    \ Randomly choose a step.
    dup list-get-length                     \ reg-to reg-from dom0 | asym-lst' len
    random                                  \ reg-to reg-from dom0 | asym-lst' inx
    over list-get-item                      \ reg-to reg-from dom0 | asym-lst' stpx

    \ Get plan1 reg-from to step initial region.
    dup planstep-get-initial-region         \ reg-to reg-from dom0 | asym-lst' stpx plnstp-i
    #4 pick                                 \ reg-to reg-from dom0 | asym-lst' stpx plnstp-i reg-from
    #4 pick                                 \ reg-to reg-from dom0 | asym-lst' stpx plnstp-i reg-from dom0
    domain-get-plan-fb                      \ reg-to reg-from dom0 | asym-lst' stpx, plan1' t | f
    false? if                               \ reg-to reg-from dom0 | asym-lst' stpx
        drop
        planstep-list-deallocate
        3drop
        \ cr ." asym exit 3 false" cr
        false
        exit
    then

    \ Link plan to step.                \ reg-to reg-from dom0 | asym-lst stpx plan1'
    swap                                \ reg-to reg-from dom0 | asym-lst plan1' stpx
    dup #2 pick                         \ reg-to reg-from dom0 | asym-lst plan1' stpx stpx plan1'
    plan-link-step-to-result-region     \ reg-to reg-from dom0 | asym-lst plan1' stpx, plan2' t | f
    if
        nip                             \ reg-to reg-from dom0 | asym-lst plan1' plan2'
        swap plan-deallocate            \ reg-to reg-from dom0 | asym-lst plan2'
    else
        drop
        plan-deallocate
        planstep-list-deallocate
        3drop
        false
        \ cr ." asym exit 4 false" cr
        exit
    then

    \ Clean up.                         \ reg-to reg-from dom0 | asym-lst' plan2'
    swap planstep-list-deallocate       \ reg-to reg-from dom0 | plan2'

    \ Get plan part 2.
    #3 pick                             \ reg-to reg-from dom0 | plan2' reg-to
    over plan-get-result-region         \ reg-to reg-from dom0 | plan2' reg-to pln-r

    #3 pick                             \ reg-to reg-from dom0 | plan2' reg-to pln-r dom0
    domain-get-plan-fb                  \ reg-to reg-from dom0 | plan2', plan3' t | f
    false? if                           \ reg-to reg-from dom0 | plan2'
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
        \ cr ." asym exit 5 true: " dup .plan cr
        true
    else                                    \ reg-to reg-from dom0 | plan3' plan2'
        plan-deallocate
        plan-deallocate
        3drop
        false
        \ cr ." asym exit 6 false" cr
    then
;

\ Get a plan for going between an initial region and a non-intersecting result region.
: domain-get-plan ( reg-to reg-from dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." domain-get-plan:     start:           from: " over .region space ." to: " #2 pick .region space ." dom: " dup domain-get-inst-id dec.

    \ Check for no plan needed.
    #2 pick #2 pick swap                        \ | reg-from reg-to
    region-superset?                            \ | bool
    if
        \ Make no-change planstep.
        0                                       \ | alt-rul
        #3 pick #3 pick                         \ | alt-rul reg-to reg-from
        rule-new-region-to-region               \ | alt-rul rul
        0 #3 pick                               \ | alt-rul rul 0 dom
        domain-find-action                      \ | alt-rul rul, act0 t | f
        false? abort" Action 0 not found?"
        planstep-new                            \ | plnstp

        \ Store planstep.
        over plan-new                           \ | plnstp pln
        tuck plan-push                          \ | pln

        \ Return
        2nip nip                                \ pln
        \ space ." plan found 1: " dup .plan cr
        true
        exit
    then

    \ Check if its even possible.
    #2 pick #2 pick                             \ | reg-to reg-from
    changes-new-region-to-region                \ | cngs'
    #3 pick #3 pick                             \ | cngs' reg-to reg-from
    #3 pick                                     \ | cngs' reg-to reg-from dom0
    domain-calc-possible-steps                  \ | cngs', plnstp-lst' t | f
    false?
    if
        changes-deallocate
        3drop
        false
        exit
    then

    \ cr ." steps: " dup .planstep-list cr
    dup planstep-list-union-changes             \ | cngs' plnstp-lst' plnstp-cngs'
    swap planstep-list-deallocate               \ | cngs' plnstp-cngs'
    \ cr ." cngs: " over .changes space ." planstep-list cngs: " dup .changes cr
    2dup                                        \ | cngs' plnstp-cngs' cngs' plnstp-cngs'
    changes-superset-of                         \ | cngs' plnstp-cngs' bool
    swap changes-deallocate                     \ | cngs' bool
    swap changes-deallocate                     \ | bool
    if
    else
        3drop
        \ space ." plan NOT found 1" cr
        false
        exit
    then

    #3 0 do

        3dup domain-get-plan-fb            \ reg-to reg-from dom0, plan t | f
        if
            2nip nip
            \ space ." plan found 2: " dup .plan cr
            true                            \ plan t
            unloop
            exit
        then

        3dup domain-asymmetric-chaining      \ plan t | f
        if
            \ cr ." plan found (asm) " dup .plan cr
            \ space ." plan found 3" cr
            2nip nip
            true
            unloop
            exit
        then

    loop
    3drop
    false
;

\ Get a plan for going from the current state to a need target.
: domain-get-plan-for-need ( ned1 dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-need

    \ Calc to region.
    over need-get-target            \ ned1 dom0 n-sta
    dup region-new                  \ ned1 dom0 n-reg'

    \ Calc from region.
    over domain-get-current-state   \ ned1 dom0 n-reg' d-sta
    dup region-new                  \ ned1 dom0 n-reg' d-reg'

    \ Get plan.
    2dup                            \ ned1 dom0 n-reg' d-reg' n-reg' d-reg'
    #4 pick                         \ ned1 dom0 n-reg' d-reg' n-reg' d-reg' dom0
    domain-get-plan                 \ ned1 dom0 n-reg' d-reg', pln t | f

    \ Clean up.
    if
        swap region-deallocate      \ ned1 dom0 n-reg' pln
        swap region-deallocate      \ ned1 dom0 pln
        nip nip                     \ pln
        true
    else
        region-deallocate           \ ned1 dom0 n-reg'
        region-deallocate           \ ned1 dom0
        2drop
        false
    then
;

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
