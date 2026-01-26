\ Functions for corner lists.

\ Check if tos is an empty list, or has a corner instance as its first item.
: assert-tos-is-corner-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-corner
        drop
    then
;

\ Deallocate a corner list.
: corner-list-deallocate ( crn-lst0 -- )
    \ Check arg.
    assert-tos-is-corner-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ crn-lst0 uc
    #2 < if
        \ Deallocate corner instances in the list.
        [ ' corner-deallocate ] literal over        \ crn-lst0 xt crn-lst0
        list-apply                                  \ crn-lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Print a corner-list
: .corner-list ( list0 -- )
    \ Check arg.
    assert-tos-is-corner-list

    [ ' .corner ] literal swap .list
;

\ Push a corner to a corner-list.
: corner-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-corner-list
    assert-nos-is-corner

    list-push-struct
;

