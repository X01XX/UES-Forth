\ Functions for changes lists.

\ Check if tos is an empty list, or has a changes instance as its first item.
: assert-tos-is-changes-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-changes
        drop
    then
;

\ Deallocate a changes list.
: changes-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-changes-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate changes instances in the list.
        [ ' changes-deallocate ] literal over       \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Print a changes-list
: .changes-list ( list0 -- )
    \ Check args.
    assert-tos-is-changes-list
    [ ' .changes ] literal swap .list
;

\ Push a changes instance into a changes-list.
: changes-list-push ( cngs1 list0 -- )
    \ Check args.
    assert-tos-is-changes-list
    assert-nos-is-changes

    list-push-struct
;

\ Push a changes instance into a changes-list.
: changes-list-push-end ( cngs1 list0 -- )
    \ Check args.
    assert-tos-is-changes-list
    assert-nos-is-changes

    list-push-end-struct
;
