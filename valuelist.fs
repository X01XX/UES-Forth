\ Functions for value lists.

\ Deallocate a value list.
: value-list-deallocate ( list0 -- )
    [ ' value-deallocate ] literal over list-apply \ Deallocate value instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

\ Return the intersection of two value lists.
: value-list-set-intersection ( list1 list0 -- list-result )
    [ ' value-eq ] literal -rot        \ xt list1 list0
    list-intersection                   \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the union of two value lists.
: value-list-set-union ( list1 list0 -- list-result )
    [ ' value-eq ] literal -rot        \ xt list1 list0
    list-union                          \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the difference of two value lists.
: value-list-set-difference ( list1 list0 -- list-result )
    [ ' value-eq ] literal -rot        \ xt list1 list0
    list-difference                     \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Print a value-list
: .value-list ( list0 -- )
    \ Check list0 ID
    dup is-not-allocated-list
    if
        ." .value-list: list0 is not an allocated list."
        abort
    then
    [ ' .value ] literal swap .list
;

\ Push a value to a value-list.
: _value-list-push ( val1 list0 -- )
    \ Check list0 ID
    dup is-not-allocated-list
    if
        ." value-list-push: list0 is not an allocated list."
        abort
    then
    over is-not-allocated-value
    if
        ." value-list-push: val1 is not an allocated value."
        abort
    then
    
    over struct-inc-use-count
    list-push
;

\ Remove a value from a value-list, and deallocate.
: value-list-remove ( xt val list | item true | false )
    list-remove
    if
        value-deallocate
        true
    else
        false
    then
;

\ Push a value onto a list, if there are no duplicates in the list.
: value-list-push-nodups ( val1 list0 -- )
    \ Check list0 ID
    
    dup is-not-allocated-list
    if
        ." value-list-push-nosubs: list0 is not an allocated list."
        abort
    then
    \ Check val1 ID
    over is-not-allocated-value
    if
        ." value-list-push-nosubs: val1 is not an allocated value."
        abort
    then
    \ cr ." value-list-push-nosubs: " over .value

    \ Return if any value in the list is a superset of val1.
    2dup                                    \ val1 list0 val1 list0
    [ ' value-superset-of ] literal        \ val1 list0 val1 list0 xt
    -rot                                    \ val1 list0 xt val1 list0
    list-member                             \ val1 list0 flag
    if
        2drop
        false
        exit
    then
                                            \ val1 list0

    begin
        2dup
        [ ' value-superset-of ] literal -rot \ val1 list0 xt val1 list0
        value-list-remove                  \ val1 list0 | flag
    while
    repeat

    \ val1 list0
    value-list-push
    true
;

