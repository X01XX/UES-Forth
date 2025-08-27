

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
    over struct-inc-use-count
    list-push
;

\ Find a action in a list, by instance id, if any.
: action-list-find ( id1 list0 -- dom true | false )
    \ Check arg.
    assert-tos-is-list

    [ ' action-id-eq ] literal -rot list-find
;

\ Append nos need-list to the tos need-list.
: need-list-append ( lst1 lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    swap                    \ lst0 lst1
    list-get-links          \ lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ lst0 link nedx
        2 pick              \ lst0 link nedx lst0
        need-list-push      \ lst0 link

        link-get-next
    repeat
                        \ lst0
    drop
;

\ Return list of needs, combined, from each action;
: action-list-get-needs ( lst0 -- needs )
    \ Check args.
    assert-tos-is-list

    \ Init list to start appending action need lists to.
    list-new swap           \ lst-ret lst0

    \ Scan action-list, getting needs from each action.
    list-get-links          \ lst-ret link
    begin
        ?dup
    while
        dup link-get-data           \ lst-ret link actx

        action-get-needs            \ lst-ret link act-neds
        rot                         \ link act-neds lst-ret
        2dup                        \ link act-neds lst-ret act-neds lst-ret
        need-list-append            \ link act-neds lst-ret'
        swap need-list-deallocate   \ link lst-ret'
        swap                        \ lst-ret' link

        link-get-next
    repeat
                            \ lst-ret
;
