\ Functions for corner lists.

\ Check if tos is an empty list, or has a corner instance as its first item.
: assert-tos-is-corner-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-corner
        drop
    then
;

\ Deallocate a corner list.
: corner-list-deallocate ( crn-lst0 -- )
    \ Check arg.
    assert-tos-is-corner-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ crn-lst0 uc
    #2 < if
        \ Deallocate corner instances in the list.
        [ ' corner-deallocate ] literal over        \ crn-lst0 xt crn-lst0
        list-apply                                  \ crn-lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Print a corner-list
: .corner-list ( list0 -- )
    \ Check arg.
    assert-tos-is-corner-list

    [ ' .corner ] literal swap .list
;

\ Push a corner to a corner-list.
: corner-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-corner-list
    assert-nos-is-corner

    list-push-struct
;

\ Return a list of corner find/confirm needs,
\ given a probable reachable region.
: corner-list-calc-needs ( reg1 crn-lst0 - ned-lst )
    \ Check args.
    assert-tos-is-corner-list
    assert-nos-is-region

    \ Init return list.
    list-new                            \ reg1 crn-lst0 | ret-lst
    
    \ Prep for loop.
    over                                \ reg1 crn-lst0 | ret-lst crn-lst0
    list-get-links                      \ reg1 crn-lst0 | ret-lst crn-link

    begin
        ?dup
    while
        \ Get needs for one corner.
        #3 pick                         \ reg1 crn-lst0 | ret-lst crn-link reg1
        over link-get-data              \ reg1 crn-lst0 | ret-lst crn-link reg1 crnx
        corner-calc-needs               \ reg1 crn-lst0 | ret-lst crn-link ned-lst'

        \ Aggregate needs.
        dup                             \ reg1 crn-lst0 | ret-lst crn-link ned-lst' ned-lst'
        #3 pick                         \ reg1 crn-lst0 | ret-lst crn-link ned-lst' ned-lst' ret-lst
        need-list-append                \ reg1 crn-lst0 | ret-lst crn-link ned-lst'
        need-list-deallocate            \ reg1 crn-lst0 | ret-lst crn-link

        link-get-next
    repeat
                                        \ reg1 crn-lst0 | ret-lst

    \ Clean up.
    nip nip                             \ ret-lst
;

: corner-list-find-corner ( sta1 crn0 -- crn t | f )
    \ Check args.
    assert-tos-is-corner-list
    assert-nos-is-value

    list-get-links                  \ sta1 crn-link

    begin
        ?dup
    while
        dup link-get-data           \ sta1 crn-link crn
        corner-get-anchor-square    \ sta1 crn-link crn-sqr
        square-get-state            \ sta1 crn-link crn-sta
        #2 pick =                   \ sta1 crn-link bool
        if
            dup link-get-data       \ sta1 crn-link crn
            nip nip                 \ crn
            true
            exit
        then

        link-get-next
    repeat
                                    \ sta1
    drop
    false
;
