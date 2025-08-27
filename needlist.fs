\ Functions for need lists.

\ Deallocate a need list.
: need-list-deallocate ( list0 -- )
    [ ' need-deallocate ] literal over list-apply   \ Deallocate need instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

\ Return the union of two need lists.
: need-list-set-union ( list1 list0 -- list-result )
    [ ' need-eq ] literal -rot        \ xt list1 list0
    list-union                          \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;


\ Print a need-list
: .need-list ( list0 -- )
    \ Check args.
    assert-tos-is-list
    [ ' .need ] literal swap .list
;

\ Push a need to a need-list, unless it is already in the list.
: need-list-push ( ned1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-need

    over struct-inc-use-count
    list-push
;
