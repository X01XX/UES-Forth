\ Functions for action lists.

\ Check if tos is a list, if non-empty, with the first item being a action.
: assert-tos-is-action-list ( tos -- )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-action
        drop
    then
;

: .action-list ( actlst0 -- )
    \ Check args.
    assert-tos-is-list

    [ ' .action ] literal swap list-apply
;

\ Deallocate an action list.
: action-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate action instances in the list.
        [ ' action-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

: action-list-push-end ( actx act-lst -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-action

    dup list-get-length         \ act act-lst len
    #2 pick                     \ act act-lst len act
    action-set-inst-id          \ act act-lst
    over struct-inc-use-count   \ act act-lst (limited usefulness, so far, but follow convention)
    list-push-end
;

\ Find a action in a list, by instance id, if any.
: action-list-find ( id1 list0 -- dom true | false )
    \ Check arg.
    assert-tos-is-list

    [ ' action-id-eq ] literal -rot list-find
;

