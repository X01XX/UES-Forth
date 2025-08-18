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

: .group-region ( grp -- )
    \ Check arg.
    assert-tos-is-group

    group-get-region
    .region space
;

\ Print a list of group regions.
: .group-list-regions ( grp-lst0 -- )
    \ Check args.
    assert-tos-is-list

    ." ("
    [ ' .group-region ] literal swap list-apply
    ." )"
;

\ Push a group to a group-list.
: group-list-push ( grp1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-group

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
