\ Functions for changes lists.

\ Deallocate a changes list.
: changes-list-deallocate ( lst0 -- )
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

\ Check if tos is a non-empty list, with the first item being a changes struct.
: assert-tos-is-changes-list ( tos -- )
    assert-tos-is-list
    assert-tos-list-is-not-empty
    dup list-get-links link-get-data
    assert-tos-is-changes
    drop
;
