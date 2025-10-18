\ Implement a Domain struct and functions.

#31379 constant domain-id
    #4 constant domain-struct-number-cells

\ Struct fields
0 constant domain-header    \ 16-bits (16) struct id (16) use count (8) instance id (8) num-bits
domain-header               cell+ constant domain-actions               \ A action-list
domain-actions              cell+ constant domain-current-state         \ A state/value.
domain-current-state        cell+ constant domain-current-action        \ An action addr.

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
    \ Insure the given addr cannot be an invalid addr.
    dup domain-mma mma-within-array 0=
    if  
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    domain-id =    
;

\ Check TOS for domain, unconventional, leaves stack unchanged. 
: assert-tos-is-domain ( arg0 -- arg0 )
    dup is-allocated-domain 0=
    abort" TOS is not an allocated domain"
;

' assert-tos-is-domain to assert-tos-is-domain-xt

\ Check NOS for domain, unconventional, leaves stack unchanged. 
: assert-nos-is-domain ( arg1 arg0 -- arg0 )
    over is-allocated-domain 0=
    abort" NOS is not an allocated domain"
;

' assert-nos-is-domain to assert-nos-is-domain-xt

\ Start accessors.

\ Return the action-list from an domain instance.
: domain-get-actions ( dom0 -- lst )
    \ Check arg.
    assert-tos-is-domain

    domain-actions +    \ Add offset.
    @                   \ Fetch the field.
;

\ Return the action-list from an domain instance.
: _domain-set-actions ( lst dom0 -- )
    \ Check arg.
    assert-tos-is-domain
    assert-nos-is-list

    domain-actions +    \ Add offset.
    !                   \ Set the field.
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

    domain-current-state +
    @
;

' domain-get-current-state to domain-get-current-state-xt

\ Set the current state of a domain instance.
: domain-set-current-state ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    \ check state for validity
    1 over                  \ u1 dom0 1 dom0
    domain-get-num-bits     \ u1 dom0 1 nb
    1- lshift               \ u1 dom0 ms-bit
    1- 1 lshift 1+          \ u1 dom0 all-bits
    #2 pick                 \ u1 dom0 all-bits u1
    tuck                    \ u1 dom0 u1 all-bits u1
    and                     \ u1 dom0 u1 U2
    <> abort" invalid state"

    \ Set inst id.
    domain-current-state +
    !
;

\ Return the current actien from a domain instance.
: domain-get-current-action ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    domain-current-action +
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
    domain-current-action +
    !
;

\ End accessors.

\ Create a domain, given the number of bits to be used.
\
\ The domain instance ID defaults to zero.
\ The instance ID will likely be reset to match its position in a list,
\ using domain-set-inst-id, which avoids duplicates and may be useful as an index into the list.
\
\ The current state defaults to zero, but can be set with domain-set-current-state.
: domain-new ( nb0 -- addr)
    
    \ Allocate space.
    domain-mma mma-allocate         \ nb0 dom

    \ Store struct id.
    domain-id over                  \ nb0 dom id dom
    struct-set-id                   \ nb0 dom

    \ Init use count.current-domain domain-add-action
    0 over struct-set-use-count     \ nb0 dom

    \ Set intance ID.
    0 over                          \ nb0 dom 0 dom
    domain-set-inst-id              \ nb0 dom

    \ Set num bits.
    2dup _domain-set-num-bits       \ nb0 dom

    \ Set actions list.             
    list-new                        \ nb0 dom lst
    dup struct-inc-use-count        \ nb0 dom lst
    2dup swap                       \ nb0 dom lst lst dom
    _domain-set-actions             \ nb0 dom lst

    \ Add action 0.
    rot                             \ dom lst nb0
    [ ' act-0-get-sample ] literal  \ dom lst nb0 xt
    action-new                      \ dom lst act
    tuck swap                       \ dom act act lst
    
    action-list-push-end            \ dom act

    over domain-set-current-action  \ dom
    0 over domain-set-current-state \ dom
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

        \ Deallocate instance.
        domain-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: domain-add-action ( xt1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    dup domain-get-num-bits     \ xt1 dom0 nb
    rot                         \ dom0 nb xt1

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
    nip nip nip                     \ ret-lst
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

\ Return the current-action.
: cur-action ( -- act )
    cur-domain-xt execute domain-get-current-action
;

' cur-action to cur-action-xt

\ Return the all bits mask.
: domain-get-all-bits-mask ( dom -- mask )
    \ Check args.
    assert-tos-is-domain

    domain-get-num-bits     \ u
    1-                      \ u'    Don't just take 2^n, as it might be the maximum number of bits.
    1 swap lshift           \ u''   Get most-significant-bit.
    1-                      \ u'''  Get all bits 1 except the msb.
    1 lshift                \ u'''' Get all bits 1 except the least-significant-bit.
    1+                      \ mask  Maku lsb 1.
;

' domain-get-all-bits-mask to domain-get-all-bits-mask-xt

\ Return the most-significant-bit mask.
: domain-get-ms-bit-mask ( dom -- mask )
    \ Check args.
    assert-tos-is-domain

    domain-get-num-bits     \ u
    1-                      \ u'    Don't just take 2^n, as it might be the maximum number of bits.
    1 swap lshift           \ mask
;

' domain-get-ms-bit-mask to domain-get-ms-bit-mask-xt

\ Return the maximum region for the domains' number of bits.
\ Caller to deallocate the region.
: domain-get-max-region ( dom -- reg )
    domain-get-all-bits-mask    \ msk
    0 region-new                \ reg
;

' domain-get-max-region to domain-get-max-region-xt

\ Return a step forward, from a sample initial state,
\ towards the sample result state.
\ That is, within the union formed by the two sample states.
\ If more than one step is found, randomly choose one,
\ to support a random depth-first strategy.
: domain-get-step-f ( smpl1 dom0 -- step true | false )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    \ Init aggregate step list.
    list-new swap                   \ smpl1 stp-lst dom0

    \ Get steps from each action.
    dup domain-get-actions          \ smpl1 stp-lst dom0 act-lst
    list-get-links                  \ smpl1 stp-lst dom0 link
    begin
        ?dup
    while                           \ smpl1 stp-lst dom0 link |
        dup link-get-data           \ | actx
        dup                         \ | actx actx
        #3 pick                     \ | actx actx dom
        domain-set-current-action   \ | actx
        #4 pick swap                \ | smpl1 actx
        action-get-forward-steps    \ | act-stps
        dup                         \ | act-stps act-stps
        #4 pick step-list-append    \ | act-stps
        step-list-deallocate        \ |

        link-get-next
    repeat
                                    \ smpl1 stp-lst dom0
    \ cr ." Dom: " dup domain-get-inst-id .
    \ space ." for: " 2 pick .sample
    \ space ." Possible steps: " over .step-list cr

    \ Isolate step list
    drop nip                        \ stp-lst
    dup list-is-empty               \ stp-lst flag
    if
        step-list-deallocate
        false
        exit
    then

    \ Pick a step.
    dup list-get-length             \ stp-lst len
    random                          \ stp-lst inx
    over                            \ stp-lst inx stp-lst

    \ Extract step.
    step-list-remove-item           \ stp-lst, stpx true | false
    0= abort" Step not found?"      \ stp-lst stpx

    \ Clean up.
    swap step-list-deallocate       \ stpx
    \ Return.
    true
;

\ Form a plan by getting successive steps closer, between
\ a sample initial state to a sample result state.
: domain-get-plan2-f ( smpl1 dom0 -- plan true | false )
    \ cr ." domain-get-plan2-f" cr
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    \ Copy sample for later deallocation in loop.
    swap sample-copy swap           \ smpl2 dom0
    \ Init return plan.
    dup plan-new-xt execute         \ smpl2 dom0 pln
    -rot                            \ pln smpl2 dom0

    begin
        2dup domain-get-step-f      \ pln smpl2 dom0 | stpx true | false

        0= if
            drop
            sample-deallocate
            plan-deallocate-xt execute
            false
            exit
         then

        \ Check if this is the last step needed.
        #2 pick                     \ pln smpl2 dom0 | stpx smpl2
        sample-get-result           \ pln smpl2 dom0 | stpx r-2
        over step-get-result        \ pln smpl2 dom0 | stpx smpl2-r stp-r
        = if                        \ pln smpl2 dom0 | stpx
            \ Clean up.
            swap drop               \ pln smpl2 stpx
            swap sample-deallocate  \ pln stpx
            \ Add step to plan.
            over                    \ pln stpx pln
            plan-push-end-xt        \ pln stpx pln xt
            execute                 \ pln
            \ Return.
            true
            exit
        then

        \ Add step to plan.         \ pln smpl2 dom0 | stpx
        dup                         \ pln smpl2 dom0 | stpx stpx
        #4 pick                     \ pln smpl2 dom0 | stpx stpx pln
        plan-push-end-xt execute    \ pln smpl2 dom0 | stpx
        \ Create new sample.
        step-get-result             \ pln smpl2 dom0 | stp-r
        #2 pick sample-get-result   \ pln smpl2 dom0 | stp-r smpl-r
        swap                        \ pln smpl2 dom0 | smpl-r stp-r
        sample-new                  \ pln smpl2 dom0 | smpl3
        \ Clean up.
        rot sample-deallocate       \ pln dom0 smpl3
        swap                        \ pln smpl3 dom0
    again
;

\ Using a random depth-first forward-chaining strategy. Try a number
\ of times to find a plan to accomplish a desired sample.
: domain-get-plan-f ( smpl1 dom0 -- plan true | false )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    #3 0 do
        2dup domain-get-plan2-f \ smpl1 dom0 | pln t | f
        if                      \ smpl1 dom0 | pln
            \ Clean up.
            nip nip             \ pln
            \ Return.
            true                \ pln t
            unloop
            exit
        then
    loop
                                \ smpl1 dom0

    \ Clean up.
    2drop
    \ Return.
    false
;

' domain-get-plan-f to domain-get-plan-f-xt

: domain-get-step-b ( smpl1 dom0 -- step true | false )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    \ Init aggregate step list.
    list-new swap                   \ smpl1 stp-lst dom0

    \ Get steps from each action.
    dup domain-get-actions          \ smpl1 stp-lst dom0 act-lst
    list-get-links                  \ smpl1 stp-lst dom0 link
    begin
        ?dup
    while                           \ smpl1 stp-lst dom0 link |
        dup link-get-data           \ | actx
        dup                         \ | actx actx
        #3 pick                     \ | actx actx dom
        domain-set-current-action   \ | actx
        #4 pick swap                \ | smpl1 actx
        action-get-backward-steps   \ | act-stps
        dup                         \ | act-stps act-stps
        #4 pick step-list-append    \ | act-stps
        step-list-deallocate        \ |

        link-get-next
    repeat
                                    \ smpl1 stp-lst dom0
    \ cr ." Dom: " dup domain-get-inst-id .
    \ space ." for: " 2 pick .sample
    \ space ." Possible steps: " over .step-list cr

    \ Isolate step list
    drop nip                        \ stp-lst
    dup list-is-empty               \ stp-lst flag
    if
        step-list-deallocate
        false
        exit
    then

    \ Pick a step.
    dup list-get-length             \ stp-lst len
    random                          \ stp-lst inx
    over                            \ stp-lst inx stp-lst

    \ Extract step.
    step-list-remove-item           \ stp-lst, stpx true | false
    0= abort" Step not found?"      \ stp-lst stpx

    \ Clean up.
    swap step-list-deallocate       \ stpx
    \ Return.
    true
;

\ Form a plan by getting successive steps closer, between
\ a sample result state to a sample initial state.
: domain-get-plan2-b ( smpl1 dom0 -- plan true | false )
    \ cr ." domain-get-plan2-b" cr
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    \ Copy sample for later deallocation in loop.
    swap sample-copy swap           \ smpl2 dom0
    \ Init return plan.
    dup plan-new-xt execute         \ smpl2 dom0 pln
    -rot                            \ pln smpl2 dom0

    begin
        2dup domain-get-step-b      \ pln smpl2 dom0 | stpx true | false

        0= if
            drop
            sample-deallocate
            plan-deallocate-xt execute
            false
            exit
         then

        \ Check if this is the last step needed.
        #2 pick                     \ pln smpl2 dom0 | stpx smpl2
        sample-get-initial          \ pln smpl2 dom0 | stpx s-i
        over step-get-initial       \ pln smpl2 dom0 | stpx s-i stp-i
        = if                        \ pln smpl2 dom0 | stpx
            \ Clean up.
            swap drop               \ pln smpl2 stpx
            swap sample-deallocate  \ pln stpx
            \ Add step to plan.
            over                    \ pln stpx pln
            plan-push-xt            \ pln stpx pln xt
            execute                 \ pln
            \ Return.
            true
            exit
        then

        \ Add step to plan.         \ pln smpl2 dom0 | stpx
        dup                         \ pln smpl2 dom0 | stpx stpx
        #4 pick                     \ pln smpl2 dom0 | stpx stpx pln
        plan-push-xt execute        \ pln smpl2 dom0 | stpx

        \ Create new sample.
        step-get-initial            \ pln smpl2 dom0 | stp-r
        #2 pick sample-get-initial  \ pln smpl2 dom0 | stp-r smpl-r
        sample-new                  \ pln smpl2 dom0 | smpl3
        \ Clean up.
        rot sample-deallocate       \ pln dom0 smpl3
        swap                        \ pln smpl3 dom
    again
;

\ Using a random depth-first backward-chaining strategy. Try a number
\ of times to find a plan to accomplish a desired sample.
: domain-get-plan-b ( smpl1 dom0 -- plan true | false )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    #3 0 do
        2dup domain-get-plan2-b \ smpl1 dom0 | pln t | f
        if                      \ smpl1 dom0 | pln
            \ Clean up.
            nip nip             \ pln
            \ Return.
            true                \ pln t
            unloop
            exit
        then
    loop
                                \ smpl1 dom0

    \ Clean up.
    2drop
    \ Return.
    false
;

' domain-get-plan-b to domain-get-plan-b-xt

\ Try forward and backward chaining to make a plan
\ for going from the initial state of a sample to the result state.
: domain-get-plan2-fb ( smpl1 dom0 -- plan true | false )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    #2 random
    if
        \ Try forward-chaining.
        2dup domain-get-plan-f      \ smpl1 dom0 | p t | f
        if
            nip nip
            cr ." plan found (fc) " dup .plan-xt execute cr
            true
            exit
        then
        \ Try backward-chaining.
        domain-get-plan-b      \ p t | f
        if
            cr ." plan found (bc*) " dup .plan-xt execute cr
            true
            exit
        then
    else
        \ Try backward-chaining.
        2dup domain-get-plan-b      \ smpl1 dom0 | p t | f
        if
            nip nip
            cr ." plan found (bc) " dup .plan-xt execute cr
            true
            exit
        then
        \ Try forward-chaining.
        domain-get-plan-f      \ p t | f
        if
            cr ." plan found (fc*) " dup .plan-xt execute cr
            true
            exit
        then
    then
    false
;

: domain-get-steps-by-changes-f ( smpl1 dom0 -- stp-lst )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    \ Init return list.
    list-new swap                   \ smpl1 stp-lst dom0

    \ Get steps from each action.
    dup domain-get-actions          \ smpl1 stp-lst dom0 act-lst
    list-get-links                  \ smpl1 stp-lst dom0 link
    begin
        ?dup
    while                               \ smpl1 stp-lst dom0 link |
        dup link-get-data               \ | actx
        dup                             \ | actx actx
        #3 pick                         \ | actx actx dom
        domain-set-current-action       \ | actx
        #4 pick swap                    \ | smpl1 actx
        action-get-steps-by-changes-f   \ | act-stps
        dup                             \ | act-stps act-stps
        #4 pick step-list-append        \ | act-stps
        step-list-deallocate            \ |

        link-get-next
    repeat
    drop                                \ smpl1 stp-lst
    nip                                 \ stp-lst
;

\ Asymmetric chaining, forward.
\ Look for rules that contain a needed bit change,
\ do not intersect a region formed by the two states of a sample, that is fails rule-restrict-to-region,
\ and no rule exists with the same bit change that does intersect
\ the union.
: domain-asymmetric-chaining-f ( smpl1 dom0 -- plan t | f )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    \ Find an asymmetric rule.
    2dup                                    \ smpl1 dom0 | smpl1 dom0
    domain-get-steps-by-changes-f           \ smpl1 dom0 | stp-lst
    dup list-is-empty if
        list-deallocate
        2drop
        false
        exit
    then
    
    \ Prep for loop by single-bit change.
    #2 pick sample-calc-changes             \ smpl1 dom0 | stp-lst cngs
    dup changes-split                       \ smpl1 dom0 | stp-lst cngs cng-lst
    swap changes-deallocate                 \ smpl1 dom0 | stp-lst cng-lst
    list-new swap                           \ smpl1 dom0 | stp-lst asym-lst cng-lst
    dup list-get-links                      \ smpl1 dom0 | stp-lst asym-lst cng-lst link

    \ For each single-bit change, find steps that do not contain the sample initial states
    \ without any alternate steps that do.
    begin
        ?dup
    while                                   \ smpl1 dom0 | stp-lst asym-lst cng-lst link
        assert-tos-is-link

        \ Find steps that provide the one-bit change, but possibly others.
        dup link-get-data                   \ smpl1 dom0 | stp-lst asym-lst cng-lst link cng1
        #4 pick                             \ smpl1 dom0 | stp-lst asym-lst cng-lst link cng1 stp-lst
        step-list-intersects-changes        \ smpl1 dom0 | stp-lst asym-lst cng-lst link stp-cng-lst
        dup list-get-length                 \ smpl1 dom0 | stp-lst asym-lst cng-lst link sc-lst len

        \ Check if the one-bit change is possible, else done.
        0= if                               \ smpl1 dom0 | stp-lst asym-lst cng-lst link sc-lst
            step-list-deallocate            \ smpl1 dom0 | stp-lst asym-lst cng-lst link
            drop                            \ smpl1 dom0 | stp-lst asym-lst cng-lst
            changes-list-deallocate         \ smpl1 dom0 | stp-lst asym-lst
            step-list-deallocate            \ smpl1 dom0 | stp-lst
            step-list-deallocate            \ smpl1 dom0 |
            2drop
            false
            exit
        then

        \ Check if there are only steps that do not match the smpl1 initial state.
        dup                                 \ smpl1 dom0 | stp-lst asym-lst cng-lst link sc-lst sc-lst
        #7 pick                             \ smpl1 dom0 | stp-lst asym-lst cng-lst link sc-lst sc-lst smpl1
        swap step-list-any-match-sample     \ smpl1 dom0 | stp-lst asym-lst cng-lst link sc-lst flag
        0= if                               \ smpl1 dom0 | stp-lst asym-lst cng-lst link sc-lst
            \ Save the steps.
            dup #4 pick                     \ smpl1 dom0 | stp-lst asym-lst cng-lst link sc-lst sc-lst asym-lst
            step-list-append                \ smpl1 dom0 | stp-lst asym-lst cng-lst link sc-lst
        then
        step-list-deallocate                \ smpl1 dom0 | stp-lst asym-lst cng-lst link

        link-get-next                       \ smpl1 dom0 | stp-lst asym-lst cng-lst link
    repeat

    changes-list-deallocate                 \ smpl1 dom0 | stp-lst asym-lst
    swap step-list-deallocate               \ smpl1 dom0 | asym-lst

    dup list-get-length                     \ smpl1 dom0 | asym-lst len
    0<> if
        \ Randomly choose a step.
        dup list-get-length                 \ smpl1 dom0 | asym-lst len
        random                              \ smpl1 dom0 | asym-lst inx
        over list-get-item                  \ smpl1 dom0 | asym-lst stpx
        dup step-get-sample                 \ smpl1 dom0 | asym-lst stpx smpl2

        \ Get plan smpl1-i to smpl2-i.
        dup sample-get-initial              \ smpl1 dom0 | asym-lst stpx smpl2 s2-i
        #5 pick sample-get-initial          \ smpl1 dom0 | asym-lst stpx smpl2 s2-i s1-i
        sample-new                          \ smpl1 dom0 | asym-lst stpx smpl2 smpl3

        dup                                 \ smpl1 dom0 | asym-lst stpx smpl2 smpl3 smpl3
        #5 pick                             \ smpl1 dom0 | asym-lst stpx smpl2 smpl3 smpl3 dom0
        domain-get-plan2-fb                 \ smpl1 dom0 | asym-lst stpx smpl2 smpl3, plan t | f
        if
            swap sample-deallocate          \ smpl1 dom0 | asym-lst stpx smpl2 plan1

            \ Get plan 2
            #5 pick sample-get-result       \ smpl1 dom0 | asym-lst stpx smpl2 plan1 s1-r
            #2 pick sample-get-result       \ smpl1 dom0 | asym-lst stpx smpl2 plan1 s1-r s2-r
            sample-new                      \ smpl1 dom0 | asym-lst stpx smpl2 plan1 smpl4

            dup #6 pick                     \ smpl1 dom0 | asym-lst stpx smpl2 plan1 smpl4 smpl4 dom0
            domain-get-plan2-fb             \ smpl1 dom0 | asym-lst stpx smpl2 plan1 smpl4, plan2 t | f
            if                              \ smpl1 dom0 | asym-lst stpx smpl2 plan1 smpl4 plan2
                swap sample-deallocate      \ smpl1 dom0 | asym-lst stpx smpl2 plan1 plan2

                \ Add step to plan1.
                #3 pick                     \ smpl1 dom0 | asym-lst stpx smpl2 plan1 plan2 stpx
                #2 pick                     \ smpl1 dom0 | asym-lst stpx smpl2 plan1 plan2 stpx plan1
                plan-push-end-xt execute    \ smpl1 dom0 | asym-lst stpx smpl2 plan1 plan2

                \ Add plan2
                2dup swap                   \ smpl1 dom0 | asym-lst stpx smpl2 plan1 plan2 plan2 plan1
                plan-append-xt execute      \ smpl1 dom0 | asym-lst stpx smpl2 plan1 plan2

                plan-deallocate-xt execute  \ smpl1 dom0 | asym-lst stpx smpl2 plan1
                cr ." plan found (afc): " dup .plan-xt execute cr
                nip nip                     \ smpl1 dom0 | asym-lst plan1
                swap step-list-deallocate   \ smpl1 dom0 plan1
                nip nip                     \ plan1
                true
                exit
            else                            \ smpl1 dom0 | asym-lst stpx smpl2 plan1 smpl4
                sample-deallocate           \ smpl1 dom0 | asym-lst stpx smpl2 plan1
                plan-deallocate-xt execute  \ smpl1 dom0 | asym-lst stpx smpl2
                2drop                       \ smpl1 dom0 | asym-lst
                step-list-deallocate        \ smpl1 dom0
                2drop
                false
                \ cr ." domain-asymmetric-chaining-f: exit 4 " .s cr
                exit
            then
        else
                                            \ smpl1 dom0 | asym-lst stpx smpl2 smpl3
            sample-deallocate               \ smpl1 dom0 | asym-lst stpx smpl2
            2drop                           \ smpl1 dom0 | asym-lst
            step-list-deallocate            \ smpl1 dom0
            2drop
            false
            \ cr ." domain-asymmetric-chaining-f: exit 5 " .s cr
            exit
        then
    else
        step-list-deallocate                \ smpl1 dom0 |
        2drop
        false
    then
;

\ Get a plan for going between the initial state of a sample to the result state.
: domain-get-plan ( smpl1 dom0 -- plan true | false )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample
    \ cr ." domain-get-plan" cr

    2dup domain-get-plan2-fb            \ smpl1 dom0, plan t | f
    if
        nip nip true                    \ plan t
    else
        domain-asymmetric-chaining-f    \ plan t | f
    then
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

: domain-get-steps-by-changes-b ( smpl1 dom0 -- stp-lst )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-sample

    \ Init return list.
    list-new swap                   \ smpl1 stp-lst dom0

    \ Get steps from each action.
    dup domain-get-actions          \ smpl1 stp-lst dom0 act-lst
    list-get-links                  \ smpl1 stp-lst dom0 link
    begin
        ?dup
    while                               \ smpl1 stp-lst dom0 link |
        dup link-get-data               \ | actx
        dup                             \ | actx actx
        #3 pick                         \ | actx actx dom
        domain-set-current-action       \ | actx
        #4 pick swap                    \ | smpl1 actx
        action-get-steps-by-changes-b   \ | act-stps
        dup                             \ | act-stps act-stps
        #4 pick step-list-append        \ | act-stps
        step-list-deallocate            \ |

        link-get-next
    repeat
    drop                                \ smpl1 stp-lst
    nip                                 \ stp-lst
;


