\ Implement functions for a list of groups.

: group-list-deallocate ( domlst0 -- )
    [ ' group-deallocate ] literal over list-apply      \ Deallocate group instances in the list.
    list-deallocate                                     \ Deallocate list and links.
;

\ Find a group in a list, by state, if any.
: group-list-find ( reg1 list0 -- grp true | false )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    [ ' group-region-eq ] literal -rot list-find
;

\ Return true if a group with a given state is a member.
: group-list-member ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    [ ' group-region-eq ] literal -rot list-member
;

\ Print a group-list
: .group-list ( grp-list0 -- )
    \ Check args.
    assert-tos-is-list

    [ ' .group ] literal swap .list
;

\ Print a list of group regions.
: .group-list-regions ( grp-lst0 -- )
    \ Check args.
    assert-tos-is-list

    ." ("
    [ ' .group-region ] literal swap list-apply
    ." )"
;

\ Push a group to a group-list, unless it is already in the list.
: group-list-push ( grp1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-group

    cr ." adding group " over .group-region cr
    over struct-inc-use-count
    list-push
;

\ Remove a group from a group-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a group was removed.
: group-list-remove ( reg list -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    cr ." removing group " over .region cr
    [ ' group-region-eq ] literal   \ reg list xt
    -rot                            \ xt reg list
    
    list-remove                     \ grp true | false
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
    assert-tos-is-list
    assert-nos-is-square

    list-get-links              \ sqr1 link
    begin
        ?dup
    while
        dup link-get-data           \ sqr1 link grpx
        #2 pick                     \ sqr1 link grpx sqr1
        square-get-state            \ sqr1 link grpx sta
        over group-get-region       \ sqr1 link grpx sta regx
        region-superset-of-state    \ sqr1 link grpx flag
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
;

\  Have appropriate groups check a changed square.
: group-list-check-square ( sqr1 grp-lst0 -- )
    \ cr ." group-list-check-square - start" cr
    \ Check args.
    assert-tos-is-list
    assert-nos-is-square

    list-get-links              \ sqr1 link
    begin
        ?dup
    while
        dup link-get-data           \ sqr1 link grpx
        #2 pick                     \ sqr1 link grpx sqr1
        square-get-state            \ sqr1 link grpx sta
        over group-get-region       \ sqr1 link grpx sta regx
        region-superset-of-state    \ sqr1 link grpx flag
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
    \ cr ." group-list-check-square - end" cr
;

\ Return true, if a state is in at least one group region.
: group-list-state-in-group ( val1 grp-lst0 -- flag )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-value

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
    assert-tos-is-list
    assert-nos-is-value

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
