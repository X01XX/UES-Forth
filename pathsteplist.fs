\ Functions for a PathStep list.

\ Check if tos is a non-empty list, with the first item being a pathstep.
: assert-tos-is-pathstep-list ( tos -- )
    assert-tos-is-list
    assert-tos-list-is-not-empty
    dup list-get-links link-get-data
    assert-tos-is-pathstep
    drop
;

\ Check if nos is a non-empty list, with the first item being a pathstep.
: assert-nos-is-pathstep-list ( nos tos -- )
    assert-nos-is-list
    assert-nos-list-is-not-empty
    over list-get-links link-get-data
    assert-tos-is-pathstep
    drop
;

: pathstep-list-deallocate ( rulc-lst0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ rulc-lst0 uc
    #2 < if
        \ Deallocate pathsteps in the list.
        [ ' pathstep-deallocate ] literal over      \ rulc-lst0 xt rulc-lst0
        list-apply                                  \ rulc-lst0

        \ Deallocate the list.
        list-deallocate
    else
        struct-dec-use-count
    then
;

\ Deallocate a list of lists of pathstep.
: pathstep-lol-deallocate ( rulc-lol0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ rulc-lol0 uc
    #2 < if
        \ Deallocate pathstep instances in the list.
        [ ' pathstep-list-deallocate ] literal over \ rulc-lol0 xt rulc-lol0
        list-apply                                  \ rulc-lol0

        \ Deallocate the list. 
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

: .pathstep-list ( rlc-lst -- )
    \ Check arg.
    assert-tos-is-list

    s" (" type
    [ ' .pathstep ] literal swap    \ xt pathstep-list
    list-apply                      \
    s" )" type
;
