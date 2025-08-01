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
    assert-arg0-is-list
    [ ' .square ] literal swap .list
;

\ Push a square to a square-list, unless it is already in the list.
: square-list-push ( reg1 list0 -- )
    \ Check args.
    assert-arg0-is-list
    assert-arg1-is-square

    2dup
    [ ' square-eq ] literal -rot
    execute
    if
        2drop
    else
        over struct-inc-use-count
        list-push
    then
;

\ Remove a square from a square-list, and deallocate.
: square-list-remove ( xt reg list -- bool )
    \ Check args.
    assert-arg0-is-list
    assert-arg1-is-square

    list-remove
    if
        square-deallocate
        true
    else
        false
    then
;


