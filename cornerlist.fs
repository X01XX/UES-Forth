\ Functions for corner lists.

\ Check if tos is an empty list, or has a corner instance as its first item.
: assert-tos-is-corner-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-corner
        drop
    then
;

\ Check if nos is an empty list, or has a corner instance as its first item.
: assert-nos-is-corner-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
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

\ Return a list of anchors states.
: .corner-list-short ( crn-lst0 -- )
    \ Check arg.
    assert-tos-is-corner-list

    ." ("

    \ Prep for loop.
    list-get-links                  \ crn-link

    begin
        ?dup
    while
        dup link-get-data           \ crn-link crnx
        dup corner-get-region       \ crn-link crnx regx
        .region
        ." ="
        corner-get-anchor-state     \ crn-link stax
        .value                      \ crn-link

        link-get-next
        dup 0<> if space then
    repeat

    ." )"
;

\ Print a corner-list of lists.
: .corner-lol ( list0 -- )
    \ Check arg.
    assert-tos-is-list

    [ ' .corner ] literal swap .list
;

\ Push a corner to a corner-list.
: corner-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-corner-list
    assert-nos-is-corner

    list-push-struct
;

\ Return a list of corner find/confirm needs.
: corner-list-calc-needs ( crn-lst0 - ned-lst )
    \ Check args.
    assert-tos-is-corner-list

    \ Init return list.
    list-new                            \ crn-lst0 | ret-lst

    \ Prep for loop.
    over                                \ crn-lst0 | ret-lst crn-lst0
    list-get-links                      \ crn-lst0 | ret-lst crn-link

    begin
        ?dup
    while
        \ Get needs for one corner.
        dup link-get-data               \ crn-lst0 | ret-lst crn-link crnx
        corner-calc-needs               \ crn-lst0 | ret-lst crn-link ned-lst'

        \ Aggregate needs.
        dup                             \ crn-lst0 | ret-lst crn-link ned-lst' ned-lst'
        #3 pick                         \ crn-lst0 | ret-lst crn-link ned-lst' ned-lst' ret-lst
        need-list-append                \ crn-lst0 | ret-lst crn-link ned-lst'
        need-list-deallocate            \ crn-lst0 | ret-lst crn-link

        link-get-next
    repeat
                                        \ crn-lst0 | ret-lst

    \ Clean up.
    nip                                 \ ret-lst
;

: corner-list-find-corner ( sta1 crn-lst0 -- crn t | f )
    \ Check args.
    assert-tos-is-corner-list
    assert-nos-is-value

    list-get-links                  \ sta1 crn-link

    begin
        ?dup
    while
        dup link-get-data           \ sta1 crn-link crn
        corner-get-anchor-state     \ sta1 crn-link crn-sta
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

\ Return the states used in a corner list.
\ Can be a measure of the amount of state sharing between
\ corners in clusters.
\ A different arrangement of corners, could be measured as
\ better (lower number), equal, or worse (higher number),
: corner-list-states ( crn-lst0 -- sta-lst )
    \ Check arg.
    assert-tos-is-corner-list

    \ Init state list.
    list-new swap                       \ sta-lst crn-lst0
    list-get-links                      \ sta-lst crn-link

    begin
        ?dup
    while
        dup link-get-data               \ sta-lst crn-link crnx

        \ Add corner anchor state.
        dup                             \ sta-lst crn-link crnx crnx
        corner-get-anchor-state         \ sta-lst crn-link crnx sta
        #3 pick                         \ sta-lst crn-link crnx sta sta-lst
        value-list-push-nodups          \ sta-lst crn-link crnx bool
        drop                            \ sta-lst crn-link crnx

        \ Add each dissimilar square state.
        corner-get-external-states      \ sta-lst crn-link sta-lst
        list-get-links                  \ sta-lst crn-link sta-link

        begin
            ?dup
        while
            dup link-get-data           \ sta-lst crn-link sta-link sta
            #3 pick                     \ sta-lst crn-link sta-link sta sta-lst
            value-list-push-nodups      \ sta-lst crn-link sta-link bool
            drop                        \ sta-lst crn-link sta-link

            link-get-next
        repeat

        link-get-next
    repeat

                                        \ sta-lst
;

\ Return the number of states in a corner list.
: corner-list-number-states ( crn-lst0 -- u )
    \ Check arg.
    assert-tos-is-corner-list

    corner-list-states              \ sta-lst'
    dup list-get-length             \ sta-lst' len
    swap list-deallocate            \ len
;

\ Return true if all corners in a list are confirmed.
: corner-list-confirmed ( crn-lst0 -- bool )
    \ Check arg.
    assert-tos-is-corner-list

    list-get-links              \ crn-link

    begin
        ?dup
    while
        dup link-get-data       \ crn-link crnx
        corner-confirmed        \ crn-link bool
        if
        else
            drop
            false
            exit
        then

        link-get-next
    repeat
                                \
    true
;

\ Return true if tos is a corner-lst lol.
: assert-tos-is-corner-lol ( crn-lol0 -- bool )
    \ Check arg.
    assert-tos-is-list

    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-corner-list
        drop
    then
;

\ Deallocate a list of corner lists.
: corner-lol-deallocate ( crn-lol0 -- )
    \ Check arg.
    assert-tos-is-corner-lol

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                    \ crn-lol0 uc
    #2 < if
        dup list-get-links                      \ crn-lol0 crn-link
        begin
            ?dup
        while
            dup link-get-data                   \ crn-lol0 crn-link data
            corner-list-deallocate              \ crn-lol0 crn-link

            link-get-next
        repeat

        \ Deallocate the list.
        list-deallocate                         \
    else
        struct-dec-use-count
    then
;

\ Return true if a state is in any corner's region.
: corner-list-state-in-any-corner-region ( sta1 crn-lst0 -- bool )
    \ Check args.
    assert-tos-is-corner-list
    assert-nos-is-value

    list-get-links              \ sta1 crn-link

    begin
        ?dup
    while
        over                    \ sta1 crn-link sta1
        over link-get-data      \ sta1 crn-link sta1 crnx
        corner-state-in-region  \ sta1 crn-link bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                \ sta1
    drop                        \
    false
;

\ Return true if any carner uses a given state.
: corner-list-uses-state ( sta1 crn-lst0 -- bool )
    \ Check args.
    assert-tos-is-corner-list
    assert-nos-is-value

    list-get-links              \ sta1 crn-link

    begin
        ?dup
    while
        over                    \ sta1 crn-link sta1
        over link-get-data      \ sta1 crn-link sta1 crnx
        corner-uses-state       \ sta1 crn-link bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                \ sta1
    drop                        \
    false
;
