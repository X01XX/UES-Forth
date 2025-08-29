\ Implement a Domain struct and functions.

31379 constant domain-id
    4 constant domain-struct-number-cells

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

: is-not-allocated-domain ( addr -- flag )
    is-allocated-domain 0=
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

    over 255 >
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

    over 64 >
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
    2 pick                  \ u1 dom0 all-bits u1
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
    cr cr ." Dom: " .

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

    2 <
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

    2dup domain-set-current-action

    swap                        \ dom1 act1
    dup action-get-sample       \ dom1 act1 smpl

    dup sample-get-result       \ dom1 act1 smpl sta
    3 pick                      \ dom1 act1 smpl sta dom 
    domain-set-current-state    \ dom1 act1 smpl

    rot domain-get-inst-id cr ." Dom: " .   \ act1 smpl
    swap action-get-inst-id ." Act: " .     \ smpl
    dup .sample cr
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

    dup domain-get-inst-id cr ." domain-get-needs: Dom: " .
    space ." reachable region: " over .region cr
    nip

    dup domain-get-current-state    \ dom0 sta
    swap                            \ sta dom0
    
    dup domain-get-actions          \ sta dom0 act-lst

    \ Init list to start appending action need lists to.
    list-new swap                   \ sta dom0 lst-ret act-lst

    \ Scan action-list, getting needs from each action.
    list-get-links                  \ sta dom0 lst-ret link
    begin
        ?dup
    while
        3 pick                      \ sta dom0 lst-ret link sta
        over link-get-data          \ sta dom0 lst-ret link sta actx

        dup 5 pick                  \ sta dom0 lst-ret link sta actx actx dom
        domain-set-current-action   \ sta dom0 lst-ret link sta actx

        action-get-needs            \ sta dom0 lst-ret link act-neds
        rot                         \ sta dom0 link act-neds lst-ret
        2dup                        \ sta dom0 link act-neds lst-ret act-neds lst-ret
        need-list-append            \ sta dom0 link act-neds lst-ret'
        swap need-list-deallocate   \ sta dom0 link lst-ret'
        swap                        \ sat dom0 lst-ret' link

        link-get-next
    repeat
                                    \ sta dom0 lst-ret
    nip nip                         \ lst-ret
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
        dup 4 pick                  \ dom0 cng-agg link actx actx dom
        domain-set-current-action   \ dom0 cng-agg link actx

        \ Get aggregate action changes.
        action-calc-changes         \ dom0 cng-agg link act-cngs

        \ Aggregate changes.
        rot                         \ dom0 link act-cngs cng-agg
        2dup changes-union          \ dom0 link act-cngs cng-agg cng-agg'

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
