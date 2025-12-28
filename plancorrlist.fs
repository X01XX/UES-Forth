\ Functions used on plancorr lits.

\ Check if tos is a list, if non-empty, with the first item being a plancorr.
: assert-tos-is-plancorr-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-plancorr
        drop
    then
;

\ Check if nos is a list, if non-empty, with the first item being a plancorr.
: assert-nos-is-plancorr-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-plancorr
        drop
    then
;

\ Deallocate a plancorr list.
: plancorr-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate plancorr instances in the list.
        [ ' plancorr-deallocate ] literal over      \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

: .plancorr-list ( rlc-lst -- )
    \ Check arg.
    assert-tos-is-plancorr-list

    s" (" type
    [ ' .plancorr ] literal swap    \ xt plancorr-list
    list-apply                      \
    s" )" type
;

\ Push a plancorr to the end of a plancorr-list.
: plancorr-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-plancorr

    over struct-inc-use-count
    list-push-end
;
