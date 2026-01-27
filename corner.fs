\ Implement a corner struct.
\ A corner is an element in discovering, and maintaining, Logical Structure.
\ A state, and closest states.  Of the closest states, those that are dissimilar.
\ The region behind A = ~B & ~C & ~D ....

#53719 constant corner-id
    #3 constant corner-struct-number-cells

\ Struct fields
0                                   constant corner-header-disp             \ 16-bits, [0] struct id, [1] use count.
corner-header-disp          cell+   constant corner-anchor-square-disp      \ The anchor square, only one Logical Structure region.
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
: corner-new ( sqr-lst1 sqr0 -- crn )
   \ Check args.
    assert-tos-is-square
    assert-nos-is-square-list
    \ cr ." corner-new: start: " dup .square space over .square-list cr

    \ Check no square-list square eq anchor square.
    dup square-get-state #2 pick    \ sqr-lst1 sqr0 sta0 sqr-lst1
    square-list-member              \ sqr-lst1 sqr0 bool
    if
        cr ." duplicate square in list?" cr
        abort
    then

    \ Check no dissimilar squares blocking each other.
    list-new                        \ sqr-lst1 sqr0 reg-lst
    over square-get-state           \ sqr-lst1 sqr0 reg-lst sta0
    #3 pick list-get-links          \ sqr-lst1 sqr0 reg-lst sta0 link

    begin
        ?dup
    while
        dup link-get-data           \ sqr-lst1 sqr0 reg-lst sta0 link sqr-l
        square-get-state            \ sqr-lst1 sqr0 reg-lst sta0 link sta-l
        #2 pick                     \ sqr-lst1 sqr0 reg-lst sta0 link sta-l sta0
        region-new                  \ sqr-lst1 sqr0 reg-lst sta0 link regx
        #3 pick                     \ sqr-lst1 sqr0 reg-lst sta0 link regx reg-lst

        2dup
        region-list-any-subset-of   \ sqr-lst1 sqr0 reg-lst sta0 link regx reg-lst bool
        if cr ." corner-new: subset found? " dup .region-list space over .region cr then

        2dup
        region-list-any-superset-of \ sqr-lst1 sqr0 reg-lst sta0 link regx reg-lst bool
        if cr ." corner-new: superset found?" dup .region-list space over .region cr then

        list-push-struct            \ sqr-lst1 sqr0 reg-lst sta0 link

        link-get-next               \ sqr-lst1 sqr0 reg-lst sta0 link
    repeat
                                    \ sqr-lst1 sqr0 reg-lst sta0
    drop                            \ sqr-lst1 sqr0 reg-lst
    region-list-deallocate          \ sqr-lst1 sqr0

    \ Check all squares in dissimilar squares list are dissimilar to the anchor square.
    over list-get-links             \ sqr-lst1 sqr0 link
    begin
        ?dup
    while
        dup link-get-data           \ sqr-lst1 sqr0 link sqrx
        #2 pick                     \ sqr-lst1 sqr0 link sqrx sqr0
        square-incompatible         \ sqr-lst1 sqr0 link bool
        is-false if cr ." corner-new: square in list is compatible? " space dup link-get-data .square space over .square then

        link-get-next
    repeat

    \ Allocate space.
    corner-mma mma-allocate         \ sqr-lst1 sqr0 crn

    \ Store id.
    corner-id over                  \ sqr-lst1 sqr0 crn id crn
    struct-set-id                   \ sqr-lst1 sqr0 crn

    \ Store states
    tuck _corner-set-anchor-square      \ sqr-lst crn
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
    3drop

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
        dup corner-get-anchor-square square-deallocate
        dup corner-get-dissimilar-squares square-list-deallocate

        \ Deallocate instance.
        corner-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

