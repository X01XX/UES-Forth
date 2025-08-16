\ Functions for value lists.

\ To deallocate a value list, use list-deallocate.

\ Return the intersection of two value lists.
: value-list-set-intersection ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    [ ' = ] literal -rot        \ xt list1 list0
    list-intersection           \ list-result
;

\ Return the union of two value lists.
: value-list-set-union ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    [ ' = ] literal -rot        \ xt list1 list0
    list-union                  \ list-result
;

\ Return the difference of two value lists.
: value-list-set-difference ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    [ ' = ] literal -rot        \ xt list1 list0
    list-difference             \ list-result
;

\ Print a value-list
: .value-list ( list0 -- )
    \ Check arg.
    assert-tos-is-list

    [ ' .value ] literal swap .list
;

\ value-list-push ( val1 list0 -- ) use list-push

\ Remove a value from a value-list.
: value-list-remove ( val list | item true | false )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-value

    [ ' = ] literal -rot
    list-remove
;

\ Push a value onto a list, if there are no duplicates in the list.
: value-list-push-nodups ( val1 list0 -- flag )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-value

    \ Return if any value in the list is a superset of val1.
    2dup                                    \ val1 list0 val1 list0
    [ ' = ] literal                         \ val1 list0 val1 list0 xt
    -rot                                    \ val1 list0 xt val1 list0
    list-member                             \ val1 list0 flag
    if
        2drop
        false
        exit
    then
                                            \ val1 list0

    list-push
    true
;

