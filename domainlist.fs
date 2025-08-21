

: .domain-list ( domlst0 -- )
    drop
    cr ." .domain-list TODO" cr  
;

: domain-list-deallocate ( domlst0 -- )
     [ ' domain-deallocate ] literal over list-apply    \ Deallocate domain instances in the list.
    list-deallocate                                     \ Deallocate list and links.
;

: domain-list-push ( domx dom-lst -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-domain

    dup list-get-length     \ dom dom-lst len
    2 pick                  \ dom dom-lst len dom
    domain-set-inst-id      \ dom dom-lst
    1 pick struct-inc-use-count
    list-push
;


