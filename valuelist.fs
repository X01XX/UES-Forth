\ Functions for value lists.

\ To deallocate a value list, use list-deallocate.

\ Check if tos is a list, if non-empty, with the first item being a value.
: assert-tos-is-value-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-value
        drop
    then
;

\ Check if nos is a list, if non-empty, with the first item being a value.
: assert-nos-is-value-list ( tos -- tos )
    assert-nos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-value
        drop
    then
;

\ Return the intersection of two value lists.
: value-list-set-intersection ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-value-list
    assert-nos-is-value-list

    [ ' = ] literal -rot        \ xt list1 list0
    list-intersection           \ list-result
;

\ Return the union of two value lists.
: value-list-set-union ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-value-list
    assert-nos-is-value-list

    [ ' = ] literal -rot        \ xt list1 list0
    list-union                  \ list-result
;

\ Return the difference of two value lists.
: value-list-set-difference ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-value-list
    assert-nos-is-value-list

    [ ' = ] literal -rot        \ xt list1 list0
    list-difference             \ list-result
;

\ Print a value-list
: .value-list ( list0 -- )
    \ Check arg.
    assert-tos-is-value-list

    [ ' .value ] literal swap .list
;

\ value-list-push ( val1 list0 -- ) use list-push

\ Remove a value from a value-list.
: value-list-remove ( val list -- item true | false )
    \ Check args.
    assert-tos-is-value-list
    assert-nos-is-value

    [ ' = ] literal -rot
    list-remove
;

\ Push a value onto a list, if there are no duplicates in the list.
: value-list-push-nodups ( val1 list0 -- flag )
    \ Check args.
    assert-tos-is-value-list
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

\ Return a list of numbers, 0 up to (not including) a given value.
: value-list-0-to-n ( n -- lst )
    \ Check arg.
    dup 1 < abort" value-list-0-to-n: invalid number?"
    
    \ Init index list for need list.
    list-new swap                   \ inx-lst cnt
    0                               \ inx-lst cnt 0
    do
        i over list-push-end
    loop
;
