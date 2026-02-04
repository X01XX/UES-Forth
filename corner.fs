\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ A state, and closest dissimilar states.
\ Once developed, the anchor square-state should be in only one region.

#53719 constant corner-id
    #4 constant corner-struct-number-cells

\ Struct fields
0                                   constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count.
corner-header-disp          cell+   constant corner-parent-action-disp      \ An action.
corner-parent-action-disp   cell+   constant corner-anchor-square-disp      \ The anchor square, only one Logical Structure region.
corner-anchor-square-disp   cell+   constant corner-dissimilar-squares-disp \ Squares, limiting the region A is in.  Adjacent squares are best.

0 value corner-mma \ Storage for corner mma instance.

\ Init corner mma, return the addr of allocated memory.
: corner-mma-init ( num-items -- ) \ sets corner-mma.
    dup 1 <
    abort" corner-mma-init: Invalid number of items."

    cr ." Initializing Corner store."
    corner-struct-number-cells swap mma-new to corner-mma
;

\ Check corner mma usage.
: assert-corner-mma-none-in-use ( -- )
    corner-mma mma-in-use 0<>
    abort" corner-mma use GT 0"
;

\ Check instance type.
: is-allocated-corner ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup corner-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        corner-id =
    else
        drop false
    then
;

\ Check TOS for corner, unconventional, leaves stack unchanged.
: assert-tos-is-corner ( tos -- tos )
    dup is-allocated-corner
    is-false if
        s" TOS is not an allocated corner"
        .abort-xt execute
    then
;

\ Check NOS for corner, unconventional, leaves stack unchanged.
: assert-nos-is-corner ( nos tos -- nos tos )
    over is-allocated-corner
    is-false if
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


\ Return the anchor-square field from a corner instance.
: corner-get-anchor-square ( crn0 -- sqr)
    \ Check arg.
    assert-tos-is-corner

    corner-anchor-square-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Return the dissimilar-squares list field from a corner instance.
: corner-get-dissimilar-squares ( crn0 -- sqr-lst )
    \ Check arg.
    assert-tos-is-corner

    corner-dissimilar-squares-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the anchor-square field from a corner instance, use only in this file.
: _corner-set-anchor-square ( sqr1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square

    corner-anchor-square-disp + \ Add offset.
    !struct                     \ Set the field.
;

\ Set the dissimilar-squares list field from a corner instance, use only in this file.
: _corner-set-dissimilar-squares ( sqr-lst1 crn0 -- )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-square-list

    corner-dissimilar-squares-disp +    \ Add offset.
    !struct                             \ Set the field.
;

\ End accessors.

\ Create a corner from an acnchor square and list of dissimilar squares.
: corner-new ( sqr-lst2 sqr1 act0 -- crn )
    \ Check args.
    assert-tos-is-action-xt execute
    assert-nos-is-square
    assert-3os-is-square-list
    \ cr ." corner-new: start: " dup .square space over .square-list cr

    \ Check no square-list square eq anchor square.
    over square-get-state #3 pick       \ sqr-lst2 sqr1 act0 sta1 sqr-lst2
    square-list-member                  \ sqr-lst2 sqr1 act0 bool
    if
        cr ." duplicate square in list?" cr
        abort
    then

    \ Check no dissimilar squares blocking each other.
    list-new                            \ sqr-lst2 sqr1 act0 reg-lst
    #2 pick square-get-state            \ sqr-lst2 sqr1 act0 reg-lst sta1
    #4 pick list-get-links              \ sqr-lst2 sqr1 act0 reg-lst sta1 link

    begin
        ?dup
    while
        dup link-get-data               \ sqr-lst2 sqr1 act0 reg-lst sta1 link sqr-l
        square-get-state                \ sqr-lst2 sqr1 act0 reg-lst sta1 link sta-l
        #2 pick                         \ sqr-lst2 sqr1 act0 reg-lst sta1 link sta-l sta1
        region-new                      \ sqr-lst2 sqr1 act0 reg-lst sta1 link regx
        #3 pick                         \ sqr-lst2 sqr1 act0 reg-lst sta1 link regx reg-lst

        2dup
        region-list-any-subset-of       \ sqr-lst2 sqr1 act0 reg-lst sta1 link regx reg-lst bool
        if cr ." corner-new: subset found? " dup .region-list space over .region cr then

        2dup
        region-list-any-superset-of     \ sqr-lst2 sqr1 act0 reg-lst sta1 link regx reg-lst bool
        if cr ." corner-new: superset found?" dup .region-list space over .region cr then

        list-push-struct                \ sqr-lst2 sqr1 act0 reg-lst sta1 link

        link-get-next                   \ sqr-lst2 sqr1 act0 reg-lst sta1 link
    repeat
                                        \ sqr-lst2 sqr1 act0 reg-lst sta1
    drop                                \ sqr-lst2 sqr1 act0 reg-lst
    region-list-deallocate              \ sqr-lst2 sqr1 act0

    \ Check all squares in dissimilar squares list are dissimilar to the anchor square.
    #2 pick list-get-links              \ sqr-lst2 sqr1 act0 link
    begin
        ?dup
    while
        dup link-get-data               \ sqr-lst2 sqr1 act0 link sqrx
        #3 pick                         \ sqr-lst2 sqr1 act0 link sqrx sqr1
        square-incompatible             \ sqr-lst2 sqr1 act0 link bool
        is-false if cr ." corner-new: square in list is compatible? " space dup link-get-data .square space over .square then

        link-get-next
    repeat

    \ Allocate space.
    corner-mma mma-allocate             \ sqr-lst2 sqr1 act0 crn

    \ Store id.
    corner-id over                      \ sqr-lst2 sqr1 act0 crn id crn
    struct-set-id                       \ sqr-lst2 sqr1 act0 crn

    \ Set parent action.
    tuck                                \ sqr-lst2 sqr1 crn act0 crn
    _corner-set-parent-action           \ sqr-lst2 sqr1 crn

    \ Store anchor square.
    tuck _corner-set-anchor-square      \ sqr-lst2 crn

    \ Store close, disimmilar, squares.
    tuck _corner-set-dissimilar-squares \ crn
;

\ Print a corner.
: .corner ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    ." ("
    dup corner-get-anchor-square    \ crn0 sqr
    dup square-get-state            \ crn0 sqr sta
    dup .value                      \ crn0 sqr sta

    ."  limited by:  "

    #2 pick                         \ crn0 sqr sta crn0
    corner-get-dissimilar-squares   \ crn0 sqr sta dis-lst
    list-get-links                  \ crn0 sqr sta dis-link

    begin
        ?dup
    while
        dup link-get-data           \ crn0 sqr sta dis-link dis-sqr

        \ Print square state.
        dup square-get-state        \ crn0 sqr sta dis-link dis-sqr dis-sta
        dup .value                  \ crn0 sqr sta dis-link dis-sqr dis-sta

        \ Check if square is adjacent.
        #3 pick                     \ crn0 sqr sta dis-link dis-sqr dis-sta sta
        value-adjacent              \ crn0 sqr sta dis-link dis-sqr bool
        is-false if ." +" then

        \ Check if square is incompatible.
        #3 pick                     \ crn0 sqr sta dis-link dis-sqr sqr
        square-incompatible         \ crn0 sqr sta dis-link bool
        is-false if ." ?" then

        link-get-next
        dup 0<> if space then
    repeat
                                        \ crn0 sqr sta

    ." ) in "

    \ Print LS regions the corner anchor state is in.
    #2 pick corner-get-parent-action    \ crn0 sqr sta act
    action-get-logical-structure-xt     \ crn0 sqr sta act xt
    execute                             \ crn0 sqr sta ls-lst
    region-list-regions-state-in        \ crn0 sqr sta in-lst'
    dup .region-list                    \ crn0 sqr in-lst'
    region-list-deallocate              \ crn0 sqr

    2drop
;

\ Deallocate a corner.
: corner-deallocate ( crn0 -- )
    \ Check arg.
    assert-tos-is-corner

    dup struct-get-use-count      \ smp0 count

    #2 <
    if
        \ Clear fields.
        dup corner-get-anchor-square square-deallocate
        dup corner-get-dissimilar-squares square-list-deallocate

        \ Deallocate instance.
        corner-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if nos corner anchor state is in more Logical structure regions than tos corner anchor state.
: corner-compare-number-ls-regions-in  ( crn1 crn0 -- flag )
    current-action                      \ | act0
    dup action-get-logical-structure-xt \ | act0  xt
    execute                             \ | act0 ls-lst

    \ Get the number of LS regions crn1 anchor is in.
    #3 pick                             \ | act0  ls-lst crn1
    corner-get-anchor-square            \ | act0  ls-lst crn1-a-sqr
    square-get-state                    \ | act0  ls-lst crn1-a-sta
    over                                \ | act0  ls-lst crn1-a ls-lst
    region-list-number-regions-state-in \ | act0  ls-lst u1
    
    \ Get the number of LS regions crn0 anchor is in.
    #3 pick                             \ | act0  ls-lst u1 crn0
    corner-get-anchor-square            \ | act0  ls-lst u1 crn0-a-sqr
    square-get-state                    \ | act0  ls-lst u1 crn0-a-sta
    #2 pick                             \ | act0  ls-lst u1 crn0-a ls-lst
    region-list-number-regions-state-in \ | act0  ls-lst u1 u0

    >                                   \ | act0  ls-lst bool

    \ Clean up.
    2nip                                \ crn1 ls-lst bool
    nip nip                             \ bool
;

\ Given a region, get samples of adjacent, external states,
\ unless one turns out to be compatible.
: corner-get-adjacent-state-needs ( reg1 crn0 -- ned-lst )
    \ Check args.
    assert-tos-is-corner

    \ Set up second frame.
    list-new                                \ reg1 crn0 | ret-lst
    over corner-get-anchor-square           \ reg1 crn0 | ret-lst anc-sqr
    dup square-get-state                    \ reg1 crn0 | ret-lst anc-sqr anc-sta
    #3 pick corner-get-parent-action        \ reg1 crn0 | ret-lst anc-sqr anc-sta act0


    \ Get bits to change.
    #5 pick region-edge-mask                \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 edg-msk

    \ Check for all-X region.
    dup 0=
    if
        2drop 2drop                         \ reg1 crn0 | ret-lst
        nip nip
        exit
    then

    value-split                             \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' |

    \ Check current external, adjacent, squares.
    dup list-get-links                      \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link

    begin
        ?dup
    while
        dup link-get-data                   \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link msk
        #4 pick                             \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link msk anc-sta
        xor                                 \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax
        #3 pick                             \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax act0
        action-find-square-xt execute       \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link, sqrx t | f
        if
            #5 pick                         \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link sqrx anc-sqr
            square-compatible               \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link bool
            if
                \ External, adjacent, square is compatible, sta2 cannot be an anchor for reg1.
                drop                        \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' |
                list-deallocate             \ reg1 crn0 | ret-lst anc-sqr anc-sta act0
                3drop                       \ reg1 crn0 | ret-lst
                nip nip
                exit
            then
        then

        link-get-next
    repeat
                                            \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' |

    \ Check eternal, adjacent, states for first sample, or additional samples.

    dup list-get-links                      \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link

    begin
        ?dup
    while
        dup link-get-data                   \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link mskx
        #4 pick                             \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link mskx anc-sta
        xor                                 \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax

        dup                                 \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax stax
        #4 pick                             \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax stax act0
        action-find-square-xt execute       \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax, sqrx t | f
        if
            square-get-pnc                  \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax pnc
            if
                false                       \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax false
            else
                true                        \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax true
            then
        else
            true                            \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link stax true
        then

        if
            \ Add need for sample.
            need-type-tpc swap              \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link ned-type stax
            #4 pick                         \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link ned-type stax act0
            action-make-need-xt execute     \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link nedx
            #6 pick                         \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link nedx ret-lst
            list-push-struct                \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link
        else
            drop                            \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' | msk-link
        then

        link-get-next
    repeat
                                            \ reg1 crn0 | ret-lst anc-sqr anc-sta act0 msk-lst1' |
    list-deallocate                         \ reg1 crn0 | ret-lst anc-sqr anc-sta act0
    3drop                                   \ reg1 crn0 | ret-lst
    nip nip
;

\ Return a list of find/confirm needs for one corner,
\ given a probable reachable region.
: corner-calc-needs ( reg1 crn0 -- ned-lst )
    \ Check args.
    assert-tos-is-corner
    assert-nos-is-region

    \ Set up second frame.
    dup corner-get-parent-action                \ reg1 crn0 | act0
    list-new                                    \ reg1 crn0 | act0 ret-lst
    over                                        \ reg1 crn0 | act0 ret-lst act0
    action-get-logical-structure-xt execute     \ reg1 crn0 | act0 ret-lst ls-lst
    #3 pick                                     \ reg1 crn0 | act0 ret-lst ls-lst crn0
    corner-get-anchor-square                    \ reg1 crn0 | act0 ret-lst ls-lst sqr
    square-get-state                            \ reg1 crn0 | act0 ret-lst ls-lst sta1 |

    \ Check anchor square pnc needs.
    dup                                         \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sta1
    #4 pick                                     \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sta1 act0
    action-find-square-xt execute               \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr t | f
    is-false abort" anchor square not found?"

    square-get-pnc                              \ reg1 crn0 | act0 ret-lst ls-lst sta1 | pnc
    if
    else
        \ Need more samples of anchor square.

        \ Make need.
        need-type-cas                           \ reg1 crn0 | act0 ret-lst ls-lst sta1 | ned-type
        over                                    \ reg1 crn0 | act0 ret-lst ls-lst sta1 | ned-type sta1
        #5 pick                                 \ reg1 crn0 | act0 ret-lst ls-lst sta1 | ned-type sta1 act0
        action-make-need-xt execute             \ reg1 crn0 | act0 ret-lst ls-lst sta1 | ned

        \ Store need.
        #3 pick                                 \ reg1 crn0 | act0 ret-lst ls-lst sta1 | ned ret-lst
        list-push-struct                        \ reg1 crn0 | act0 ret-lst ls-lst sta1 |

        \ Clean up, return.
        2drop                                   \ reg1 crn0 | act0 ret-lst
        2nip nip                                \ ret-lst
        exit
    then

    \ Check for dissimilar square pnc needs.
    #4 pick                                     \ reg1 crn0 | act0 ret-lst ls-lst sta1 | crn0
    corner-get-dissimilar-squares               \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-lst

    \ Prep for loop.
    list-get-links                              \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link

    \ For each dissimilar square, check pnc.
    begin
        ?dup
    while
        dup link-get-data                       \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link sqr
        square-get-pnc
        if
        else
            \ Need more samples of dissimilar square.

            \ Make need.
            need-type-cds                       \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned-type
            over link-get-data square-get-state \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned-type sta1
            #6 pick                             \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned-type sta1 act0
            action-make-need-xt execute         \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned

            \ Store need.
            #4 pick                             \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned ret-lst
            list-push-struct                    \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link
        then

        link-get-next
    repeat

    \ If there are any needs, return.
                                                \ reg1 crn0 | act0 ret-lst ls-lst sta1 |
    #2 pick list-is-not-empty                   \ reg1 crn0 | act0 ret-lst ls-lst sta1 | bool
    if
        2drop                                   \ reg1 crn0 | act0 ret-lst
        2nip nip                                \ ret-lst
        exit
    then
    
    \ Check for dissimilar square closer needs.
    #4 pick                                     \ reg1 crn0 | act0 ret-lst ls-lst sta1 | crn0
    corner-get-dissimilar-squares               \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-lst

    \ Prep for loop.
    list-get-links                              \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link

    begin
        ?dup
    while
        dup link-get-data square-get-state      \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link sta
        #2 pick                                 \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link sta sta1
        value-adjacent                          \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link bool
        if
        else
            \ Calculate an arbitrary state between.
            dup link-get-data square-get-state  \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link sta
            #2 pick                             \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link sta sta1
            over xor                            \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link sta dif-msk
            value-isolate-lsb                   \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link sta dif-msk2
            xor                                 \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link sta2

            \ Make need.
            need-type-cdsb swap                 \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned-type sta2
            #6 pick                             \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned-type sta2 act0
            action-make-need-xt execute         \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned

            \ Store need.
            #4 pick                             \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link ned ret-lst
            list-push-struct                    \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sqr-link
        then

        link-get-next
    repeat

    \ If there are any needs, return.
                                                \ reg1 crn0 | act0 ret-lst ls-lst sta1 |
    #2 pick list-is-not-empty                   \ reg1 crn0 | act0 ret-lst ls-lst sta1 | bool
    if
        2drop                                   \ reg1 crn0 | act0 ret-lst
        2nip nip                                \ ret-lst
        exit
    then

    \ Check for external square test needs.
    dup                                         \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sta1
    #2 pick                                     \ reg1 crn0 | act0 ret-lst ls-lst sta1 | sta1 ls-lst
    region-list-regions-state-in                \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst'

\    dup list-get-length 1 >
\    if
\        cr ." possible corner in multiple regions" cr
\    then

    \ Prep for loop.
    dup list-get-links                          \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link
    begin
        ?dup
    while
        \ Restrict LS region, if needed.
        #7 pick                                 \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link reg1
        over link-get-data                      \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link reg1 regx
        region-intersection                     \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link, reg3' t | f
        if
            dup                                 \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link reg3' reg3'
            \ cr ." eval " dup .region space ." and " over .value
            #8 pick                             \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link reg3' reg3' crn0
            corner-get-adjacent-state-needs     \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link reg3' ned-lst'
            swap region-deallocate              \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link ned-lst'
            \ space ." adj needs: " dup .need-list cr
            dup                                 \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link ned-lst' ned-lst'
            #6 pick                             \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link ned-lst' ned-lst' ret-lst
            need-list-append                    \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link ned-lst'
            need-list-deallocate                \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst' reg-link
        then

        link-get-next
    repeat
                                                \ reg1 crn0 | act0 ret-lst ls-lst sta1 | reg-lst'
    region-list-deallocate                      \ reg1 crn0 | act0 ret-lst ls-lst sta1
    2drop                                       \ reg1 crn0 | act0 ret-lst
    2nip nip                                    \ ret-lst
;
