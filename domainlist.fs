

: .domain-list ( domlst0 -- )
    drop
    cr ." .domain-list TODO" cr  
;

: domain-list-deallocate ( domlst0 -- )
     [ ' domain-deallocate ] literal over list-apply    \ Deallocate domain instances in the list.
    list-deallocate                                     \ Deallocate list and links.
;

: domain-list-push-end ( domx dom-lst -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-domain

    dup list-get-length     \ dom dom-lst len
    2 pick                  \ dom dom-lst len dom
    domain-set-inst-id      \ dom dom-lst
    over struct-inc-use-count
    list-push-end
;

\ Find a domain in a list, by instance id, if any.
: domain-list-find ( id1 list0 -- dom true | false )
    \ Check args.
    assert-tos-is-list

    [ ' domain-id-eq ] literal -rot list-find
;

\ Return needs aggregated from all domains.
: domain-list-get-needs ( lst0 -- ned-lst )
    \ Check args.
    assert-tos-is-list

    \ Init list to start appending domain need lists to.
    list-new swap           \ lst-ret lst0

    \ Scan domain-list, getting needs from each domain.
    list-get-links          \ lst-ret link
    begin
        ?dup
    while
        dup link-get-data           \ lst-ret link domx

        domain-get-needs            \ lst-ret link dom-neds
        rot                         \ link dom-neds lst-ret
        2dup                        \ link dom-neds lst-ret dom-neds lst-ret
        need-list-append            \ link dom-neds lst-ret'
        swap need-list-deallocate   \ link lst-ret'
        swap                        \ lst-ret' link

        link-get-next
    repeat
                            \ lst-ret
;

