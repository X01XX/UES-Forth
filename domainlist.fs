\ Functions for domain lists.

\ Deallocate a domain list.                                                                                                             
: domain-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate domain instances in the list.
        [ ' domain-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

: domain-list-push-end ( domx dom-lst -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-domain

    dup list-get-length         \ dom dom-lst len
    #2 pick                     \ dom dom-lst len dom
    domain-set-inst-id          \ dom dom-lst
    over struct-inc-use-count   \ dom dom-lst  (limited usefulness, so far, but follow convention)
    list-push-end               \
;

\ Find a domain in a list, by instance id, if any.
: domain-list-find ( id1 list0 -- dom true | false )
    \ Check args.
    assert-tos-is-list

    [ ' domain-id-eq ] literal -rot list-find
;


