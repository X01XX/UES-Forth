\ Functions for rlc lists.

\ Deallocate an rlc list.
: rlc-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    2 < if
        \ Deallocate rlc instances in the list.
        [ ' region-list-deallocate ] literal over           \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

' rlc-list-deallocate to rlc-list-deallocate-xt

: .rlc-list ( rlc-lst -- )
    s" (" type
    [ ' .region-list-corr ] literal swap    \ xt rlc-list
    list-apply                              \
    s" )" type
;

\ Return true if a region-list-corr-list (list of lists) contains
\ a superset of a gives rlc.
: rlc-list-any-superset ( rlc1 rlc-lst0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    list-get-links                  \ rlc1 link0

    begin
        ?dup
    while
        over                        \ rlc1 link0 rlc1
        over link-get-data          \ rlc1 link0 rlc1 rlcx
        region-list-corr-superset   \ rlc1 link0 bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                    \ rlc1
    drop                            \
    false                           \ bool
;

\ For a rlc-list, remove subsets of a given rlc.
: rlc-list-remove-subsets ( rlc1 rlc-list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    begin
        [ ' region-list-corr-subset ] literal   \ rlc1 rlc-list0 xt
        #2 pick #2 pick                         \ rlc1 rlc-list0 xt rlc1 rlc-list0
        list-remove                             \ rlc1 rlc-list0, rlcx t | f
        if
            region-list-deallocate              \ rlc1 rlc-list0
        else
            2drop
            exit
        then
    again
;

\ Push an rlc into a rlc-list.
: rlc-list-push  ( rlc1 rlc-list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Inc list use count.
    over struct-inc-use-count

    list-push                           \
;

\ Add a rlc to an rlc list, removing subsets.
\ A duplicate in the list will be like a superset, the code will not push.
\ Return true if the push succeeds.
\ You may want to deallocate the rlc if the push fails.
: rlc-list-push-nosubs ( rlc1 rlc-list0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Skip if any supersets/eq are in the list.
    2dup rlc-list-any-superset      \ rlc1 rlc-list0 bool
    if
        2drop
        false
        exit
    then

    2dup rlc-list-remove-subsets

    rlc-list-push
    true
;

\ Return an rcl-list with no duplicates or subsets.
\ Return true if the push succeeds.
: rlc-list-copy-nosubs ( list0 -- rcl-list )
    \ Check arg.
    assert-tos-is-list

    list-new swap               \ ret-lst rlc-list-0

    list-get-links              \ ret-lst link

    begin
        ?dup
    while
        dup link-get-data       \ ret-lst link rlcx
        #2 pick                 \ ret-lst link rlcx ret-lst
        rlc-list-push-nosubs    \ ret-lst link bool
        drop                    \ ret-lst link

        link-get-next           \ ret-lst link
    repeat
                                \ ret-lst
;
