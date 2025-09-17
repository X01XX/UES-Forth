\ Functions for changes lists.

\ Deallocate a changes list.
: changes-list-deallocate ( list0 -- )
    [ ' changes-deallocate ] literal over list-apply    \ Deallocate changes instances in the list.
    list-deallocate                                     \ Deallocate list and links.
;

\ Print a changes-list
: .changes-list ( list0 -- )
    \ Check args.
    assert-tos-is-list
    [ ' .changes ] literal swap .list
;

\ Push a changes instance into a changes-list.
: changes-list-push ( cngs1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-changes

    over struct-inc-use-count
    list-push
;

