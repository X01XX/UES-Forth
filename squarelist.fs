\ Functions for square lists.

\ Deallocate a square list.
: square-list-deallocate ( list0 -- )
    [ ' square-deallocate ] literal over list-apply \ Deallocate square instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

\ Return the intersection of two square lists.
: square-list-set-intersection ( list1 list0 -- list-result )
    [ ' square-eq ] literal -rot        \ xt list1 list0
    list-intersection                   \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the union of two square lists.
: square-list-set-union ( list1 list0 -- list-result )
    [ ' square-eq ] literal -rot        \ xt list1 list0
    list-union                          \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the difference of two square lists.
: square-list-set-difference ( list1 list0 -- list-result )
    [ ' square-eq ] literal -rot        \ xt list1 list0
    list-difference                     \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Print a square-list
: .square-list ( list0 -- )
    \ Check args.
    assert-tos-is-list
    [ ' .square ] literal swap .list
;

\ Print a list of square states.
: .square-list-states ( sqrlst0 -- )
    ." ("
    [ ' .square-state ] literal swap list-apply
    ." )"
;

\ Push a square to a square-list, unless it is already in the list.
: square-list-push ( sqr1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-square

    2dup
    [ ' square-eq ] literal -rot
    list-find
    if
        drop 2drop
        cr ." square-list-push: dropping" cr
    else
        over struct-inc-use-count
        list-push
    then
;

\ Remove the first square, idetified by xt, from a square-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if an square was removed.
: square-list-remove ( xt val list -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-square

    list-remove
    if
        square-deallocate
        true
    else
        false
    then
;

\ Return true if a square-state is a subset of a region.
: square-state-in-region ( reg1 sqr0 -- flag )
    square-get-state
    swap
    region-superset-of-state
;

\ Return squares in a given region.
: square-list-in-region ( reg1 list0 -- list )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    [ ' square-state-in-region ] literal -rot       \ xt reg1 list0
    list-find-all                                   \ ret-list
    [ ' struct-inc-use-count ] literal over         \ ret-list xt ret-list
    list-apply                                      \ ret-list
;

\ Return squares in a given region.
: square-list-in-region2 ( reg1 list0 -- list )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    list-get-links                  \ reg1 link0
    list-new -rot                   \ ret-list reg1 link0
    begin
        dup
    while
        dup link-get-data           \ ret-list reg1 link data
        dup square-get-state        \ ret-list reg1 link data sta
        3 pick                      \ ret-list reg1 link data sta reg1
        region-superset-of-state    \ ret-list reg1 link data flag
        if
            3 pick                  \ ret-list reg1 link data ret-list
            over struct-inc-use-count
            list-push               \ ret-list reg1 link
        else
            drop                    \ ret-list reg1 link
        then
        
        link-get-next               \ ret-list reg1 link-next
    repeat
    \ ret-list reg1 0
    2drop                           \ ret-list
;

\ Return true if a square state matches a value.
: square-match ( val1 sqr0 -- flag )
    square-get-state
    =
;

\ Find a square in a list, if any.
: square-list-find ( val1 list0 -- sqr true | false )
    [ ' square-match ] literal -rot list-find
;



