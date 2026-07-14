\ Functions for group lists.

\ Check TOS for group-list.
: is-group-list? ( tos -- t )
   dup is-list?            \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?      \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links          \ link
    link-get-data           \ data
    is-group?               \ bool
;

\ Deallocate a group list.
: group-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-group-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate group instances in the list.
        [ ' group-deallocate ] literal over         \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Find a group in a list, by state, if any.
: group-list-find ( reg1 list0 -- grp t | f )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-region? )

    [ ' group-region-eq ] literal -rot list-find
;

\ Return true if a group with a given state is a member.
: group-list-member? ( reg1 list0 -- flag )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-region? )

    [ ' group-region-eq ] literal -rot list-member?
;

\ Print a group-list
: .group-list ( grp-list0 -- )
    \ Check args.
    assert( tos is-group-list? )

    [ ' .group ] literal swap .list
;

\ Print a list of group regions.
: .group-list-regions ( grp-lst0 -- )
    \ Check args.
    assert( tos is-group-list? )

    ." ("
    [ ' .group-region ] literal swap list-apply
    ." )"
;

\ Push a group to a group-list, unless it is already in the list.
: group-list-push ( grp1 list0 -- )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-group?-xt execute )

    cr ." Dom: " current-domain-id-gbl #3 dec.r
    space ." Act: " current-action-id-gbl #3 dec.r
    space ." adding group " over .group-region
    cr

    list-push-struct
;

\ Delete a group from a group-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a group was removed.
: group-list-delete ( reg1 grp-list0 -- bool )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-region? )

    cr ." Dom: " current-domain-id-gbl #3 dec.r
    space ." Act: " current-action-id-gbl #3 dec.r
    space ." deleting group " over .region
    cr

    [ ' group-region-eq ] literal   \ reg list xt
    -rot                            \ xt reg list

    list-remove                     \ grp t | f
    if
        group-deallocate
        true
    else
        false
    then
;

\ Add a new square to appropriate groups.
: group-list-add-square ( sqr1 grp-lst0 -- )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-square? )
    \ cr ." group-list-add-square: start" cr

    list-get-links              \ sqr1 link
    begin
        ?dup
    while
        dup link-get-data           \ sqr1 link grpx
        #2 pick                     \ sqr1 link grpx sqr1
        square-get-state            \ sqr1 link grpx sta
        over group-get-region       \ sqr1 link grpx sta regx
        region-superset-of-state?   \ sqr1 link grpx flag
        if
            #2 pick swap            \ sqr1 link sqr1 grpx
            group-add-square        \ sqr1 link
        else
            drop                    \ sqr1 link
        then

        link-get-next               \ sqr1 link
    repeat
                                    \ sqr1
    drop
    \ cr ." group-list-add-square: end" cr
;

\  Have appropriate groups check a changed square.
: group-list-check-square ( sqr1 grp-lst0 -- )
    \ cr ." group-list-check-square: start" cr
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-square? )
    \ cr ." group-list-check-square: start" cr

    list-get-links              \ sqr1 link
    begin
        ?dup
    while
        dup link-get-data           \ sqr1 link grpx
        #2 pick                     \ sqr1 link grpx sqr1
        square-get-state            \ sqr1 link grpx sta
        over group-get-region       \ sqr1 link grpx sta regx
        region-superset-of-state?   \ sqr1 link grpx flag
        if
            #2 pick swap            \ sqr1 link sqr1 grpx
            group-check-square      \ sqr1 link
        else
            drop                    \ sqr1 link
        then

        link-get-next               \ sqr1 link
    repeat
                                    \ sqr1
    drop
    \ cr ." group-list-check-square:  end" cr
;

\ Return true, if a state is in at least one group region.
: group-list-state-in-group ( val1 grp-lst0 -- flag )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-value? )

    list-get-links              \ val0 link
    begin
        ?dup
    while
        2dup                    \ val0 link val0 link
        link-get-data           \ val0 link val0 grp
        group-state-in          \ val0 link flag
        if                      \ val0 link
            2drop true exit
        then

        link-get-next
    repeat
                                \ val0
    drop
    false
;

\ Return true, if a state is in at least one group r-region.
: group-list-state-in-group-r ( val1 grp-lst0 -- flag )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-value? )

    list-get-links              \ val0 link
    begin
        ?dup
    while
        2dup                    \ val0 link val0 link
        link-get-data           \ val0 link val0 grp
        group-state-in-r        \ val0 link flag
        if                      \ val0 link
            2drop true exit
        then

        link-get-next
    repeat
                                \ val0
    drop
    false
;

\ Return true if any group uses a given state.
: group-list-uses-square? ( sta1 grp-lst0 -- bool )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-value? )

    list-get-links              \ sta1 grp-link

    begin
        ?dup
    while
        over                    \ sta1 grp-link sta1
        over link-get-data      \ sta1 grp-link sta1 grpx
        group-uses-square?      \ sta1 grp-link bool
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

\ Remove a square from a group list.
: group-list-remove-square ( sta1 grp-lst0 -- )
    \ Check args.
    assert( tos is-group-list? )
    assert( nos is-value? )

    list-get-links          \ sta1 grp-link

    begin
        ?dup
    while
        over                \ sta1 grp-link sta1
        over link-get-data  \ sta1 grp-link sta1 grp
        group-remove-square \ sta1 grp-link

        link-get-next
    repeat
                            \ sta1
    drop
;
