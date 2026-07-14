\ Functions for changes lists.

\ Check TOS for changen-list.
: is-changes-list? ( tos -- t )
    dup is-list?            \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?      \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links          \ link
    link-get-data           \ data
    is-changes?             \ bool
;

\ Deallocate a changes list.
: changes-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-changes-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate changes instances in the list.
        [ ' changes-deallocate ] literal over       \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Print a changes-list
: .changes-list ( list0 -- )
    \ Check args.
    assert( tos is-changes-list? )
    [ ' .changes ] literal swap .list
;

\ Push a changes instance into a changes-list.
: changes-list-push ( cngs1 list0 -- )
    \ Check args.
    assert( tos is-changes-list? )
    assert( nos is-changes? )

    list-push-struct
;

\ Push a changes instance into a changes-list.
: changes-list-push-end ( cngs1 list0 -- )
    \ Check args.
    assert( tos is-changes-list? )
    assert( nos is-changes? )

    list-push-end-struct
;
