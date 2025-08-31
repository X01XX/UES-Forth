

: .action-list ( actlst0 -- )
    \ Check args.
    assert-tos-is-list

    [ ' .action ] literal swap list-apply
;

: action-list-deallocate ( actlst0 -- )
    [ ' action-deallocate ] literal over list-apply \ Deallocate action instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

: action-list-push-end ( actx act-lst -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-action

    dup list-get-length         \ act act-lst len
    2 pick                      \ act act-lst len act
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

