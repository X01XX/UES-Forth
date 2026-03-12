\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ A state, and closest dissimilar states.
\ Once developed, the anchor square-state should be in only one region.

#53719 constant corner-id
    #5 constant corner-struct-number-cells

\ Struct fields
0                                   constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count.
corner-header-disp          cell+   constant corner-parent-action-disp      \ An action.
corner-parent-action-disp   cell+   constant corner-anchor-state-disp       \ The anchor state.
corner-anchor-state-disp    cell+   constant corner-external-states-disp    \ State, limiting the region A is in.
corner-external-states-disp cell+   constant corner-region-disp             \ A region.

0 value corner-mma \ Storage for corner mma instance.

\ Init corner mma, return the addr of allocated memory.
: corner-mma-init ( num-items -- ) \ sets corner-mma.
    dup 1 <
    abort" corner-mma-init: Invalid number of items."

    cr ." Initializing Corner store."
    corner-struct-number-cells swap mma-new to corner-mma
;

\ Check instance type.
: is-allocated-corner ( addr -- flag )
    get-first-word          \ w t | f
    if
        corner-id =
    else
        false
    then
;

\ Check TOS for corner, unconventional, leaves stack unchanged.
: assert-tos-is-corner ( tos -- tos )
    dup is-allocated-corner
    is-false? if
        s" TOS is not an allocated corner"
        .abort-xt execute
    then
;

\ Check NOS for corner, unconventional, leaves stack unchanged.
: assert-nos-is-corner ( nos tos -- nos tos )
    over is-allocated-corner
    is-false? if
        s" NOS is not an allocated corner"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the parent-action field from a corner instance.
: corner-get-parent-action ( crn0 -- act)
    \ Check arg.
    assert-tos-is-corner

    corner-parent-action-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Set the parent-action field from a corner instance, use only in this file.
: _corner-set-parent-action ( act1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-action-xt execute

    corner-parent-action-disp + \ Add offset.
    !                           \ Set the field.
;


\ Return the anchor-state field from a corner instance.
: corner-get-anchor-state ( crn0 -- sta )
    \ Check arg.
    assert-tos-is-corner

    corner-anchor-state-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Return the external-states list field from a corner instance.
: corner-get-external-states ( crn0 -- sta-lst )
    \ Check arg.
    assert-tos-is-corner

    corner-external-states-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the anchor-state field from a corner instance, use only in this file.
: _corner-set-anchor-state ( sta1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-value

    corner-anchor-state-disp +  \ Add offset.
    !                           \ Set the field.
;

\ Set the external-states list field from a corner instance, use only in this file.
: _corner-set-external-states ( sta-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-value-list

    corner-external-states-disp +      \ Add offset.
    !struct                             \ Set the field.
;

\ Return the region field from a corner instance.
: corner-get-region ( crn0 -- reg )
    \ Check arg.
    assert-tos-is-corner

    corner-region-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the region field from a corner instance, use only in this file.
: _corner-set-region ( reg1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-region

    corner-region-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a corner from an acnchor state and region.
: corner-new ( reg2 sta1 act0 -- crn )
    \ Check args.

        assert-tos-is-action-xt execute
        assert-nos-is-value
        assert-3os-is-region

        \ cr ." Dom: " current-domain-id #3 dec.r space
        \    ." Act: " current-action-id #3 dec.r space
        \    ." corner-new: " over .state space #2 pick .value cr

        \ Check that anchor state is in region.
        over                                \ reg2 sta1 act0 sta1
        #3 pick                             \ reg2 sta1 act0 sta1 reg2
        region-superset-of-state?           \ reg2 sta1 act0 bool
        is-false? abort" Anchor square not in region?"

    \ Allocate space.
    corner-mma mma-allocate             \ reg2 sta1 act0 crn

    \ Store id.
    corner-id over                      \ reg2 sta1 act0 crn id crn
    struct-set-id                       \ reg2 sta1 act0 crn

    \ Init use count.
    0 over struct-set-use-count         \ reg2 sta1 act0 crn

    \ Set parent action.
    tuck                                \ reg2 sta1 crn act0 crn
    _corner-set-parent-action           \ reg2 sta1 crn

    \ Generate external states list.

        \ Init external states list.
        list-new                            \ reg2 sta1 crn ext-lst

        \ Get single-bit edge masks.
        #3 pick region-edge-mask            \ reg2 sta1 crn ext-lst msk'
        value-split dup list-get-links      \ reg2 sta1 crn ext-lst msk-lst' msk-link

        \ Generate a state external to sta1 for eack edge mask.
        begin
            ?dup
        while
            dup link-get-data               \ reg2 sta1 crn ext-lst msk' msk-link mskx
            #5 pick                         \ reg2 sta1 crn ext-lst msk' msk-link mskx sta1
            xor                             \ reg2 sta1 crn ext-lst msk' msk-link ext-sta
            #3 pick                         \ reg2 sta1 crn ext-lst msk' msk-link ext-sta ext-lst
            list-push                       \ reg2 sta1 crn ext-lst msk' msk-link

            link-get-next
        repeat                              \ reg2 sta1 crn ext-lst msk'

        \ Clean up.
        list-deallocate                     \ reg2 sta1 crn ext-lst

    \ Store external state list.
    over _corner-set-external-states    \ reg2 sta1 crn

    \ Store anchor state.
    tuck _corner-set-anchor-state       \ reg2 crn

    \ Store region.
    tuck _corner-set-region             \ crn
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    ." ("
    dup corner-get-anchor-state     \ crn0 sta
    .value                          \ crn0

    ."  limited by:  "

    dup corner-get-external-states  \ crn0 ext-sta-lst
    .value-list                     \ crn0

    space ." in "

    \ Print regions.
    corner-get-region               \ reg
    .region                         \
    ." )"
;

\ Deallocate a corner.
: corner-deallocate ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    dup struct-get-use-count      \ smp0 count

    #2 <
    if
        \ Clear fields.
        dup corner-get-external-states list-deallocate
        dup corner-get-region region-deallocate

        \ Deallocate instance.
        corner-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Given a region, get samples of adjacent, external states,
\ unless one turns out to be compatible.
: corner-get-adjacent-state-needs ( crn0 -- ned-lst )
    \ Check arg.
    assert-tos-is-corner

    \ Init return need list.
    list-new                                \ crn0 ret-lst
    over corner-get-parent-action           \ crn0 ret-lst act0

    \ Check eternal, adjacent, states for first sample, or additional samples.
    #2 pick corner-get-external-states      \ crn0 ret-lst act0 ext-sta-lst
    list-get-links                          \ crn0 ret-lst act0 ext-sta-link

    begin
        ?dup
    while
        dup link-get-data                   \ crn0 ret-lst act0 ext-sta-link ext-sta
        #2 pick                             \ crn0 ret-lst act0 ext-sta-link ext-sta act0
        action-state-confirmed-xt execute   \ crn0 ret-lst act0 ext-sta-link bool

        if
        else
            \ Add need for sample.
            need-type-cds                   \ crn0 ret-lst act0 ext-sta-link ned-type
            over link-get-data              \ crn0 ret-lst act0 ext-sta-link ned-type ext-sta
            #3 pick                         \ crn0 ret-lst act0 ext-sta-link ned-type ext-sta act0
            action-make-need-xt execute     \ crn0 ret-lst act0 ext-sta-link nedx
            #3 pick                         \ crn0 ret-lst act0 ext-sta-link nedx ret-lst
            list-push-struct                \ crn0 ret-lst act0 ext-sta-link
        then

        link-get-next
    repeat

    \ Clean up.
                                            \ crn0 ret-lst act0
    drop                                    \ crn0 ret-lst
    nip                                     \ ret-lst
;

\ Return a list of find/confirm needs for one corner.
: corner-calc-needs ( crn0 -- ned-lst )
    \ Check args.
    assert-tos-is-corner

    \ Check anchor square needs.
    dup corner-get-anchor-state             \ crn sta
    over corner-get-parent-action           \ crn0 sta act0

    action-state-confirmed-xt execute       \ crn0 bool

    if
    else
        \ Add need for sample.
        need-type-cas                   \ crn0 ned-type
        over corner-get-anchor-state    \ crn0 ned-type sta
        #2 pick                         \ crn0 ned-type sta crn0
        corner-get-parent-action        \ crn0 ned-type sta act0
        action-make-need-xt execute     \ crn0 nedx
        nip                             \ nedx
        list-new tuck                   \ ned-lst nedx ned-lst
        list-push-struct                \ ned-lst
        exit
    then

                                        \ crn0
    corner-get-adjacent-state-needs     \ ned-lst
;

\ Return true if a corner is confirmed.
: corner-confirmed (  crn0 -- bool )
    \ Check args.
    assert-tos-is-corner

    corner-calc-needs       \ ned-lst'
    dup list-is-empty       \ ned-lst' bool
    swap                    \ bool ned-lst'
    need-list-deallocate    \ bool
;

\ Return true if a given state is in a corner's region.
: corner-state-in-region ( sta1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-value

    corner-get-region           \ sta1 crn0
    region-superset-of-state?   \ bool
;

\ Return true if a corner uses a given state.
: corner-uses-state ( sta1 crn0 -- bool )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-value

    2dup                            \ sta1 crn0 sta1 crn0
    corner-get-anchor-state         \ sta1 crn0 sta1 a-sta
    =                               \ sta1 crn0 bool
    if
        2drop
        true
        exit
    then

    corner-get-external-states      \ sta1 ext-sta-lst
    [ ' = ] literal                 \ sta1 ext-sta-lst xt
    -rot                            \ xt sta1 ext-sta-lst
    list-member                     \ bool
;
