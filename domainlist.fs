
: domain-list-deallocate ( domlst0 -- )
    \ Check arg.
    assert-tos-is-list

    [ ' domain-deallocate ] literal over list-apply    \ Deallocate domain instances in the list.
    list-deallocate                                     \ Deallocate list and links.
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


