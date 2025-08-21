

: .action-list ( actlst0 -- )
    drop
    cr ." .action-list TODO" cr
;

: action-list-deallocate ( actlst0 -- )
    [ ' action-deallocate ] literal over list-apply \ Deallocate action instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

: action-list-push ( actx act-lst -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-action

    dup list-get-length     \ act act-lst len
    2 pick                  \ act act-lst len act
    action-set-inst-id      \ act act-lst
    1 pick struct-inc-use-count
    list-push
;

