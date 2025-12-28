\ Functions for planstep lists.

\ Check if tos is a non-empty list, if non-empty, with the first item being a planstep.
: assert-tos-is-planstep-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-planstep
        drop
    then
;

\ Check if nos is a non-empty list, if non-empty, with the first item being a planstep.
: assert-nos-is-planstep-list ( nos tos -- nos tos )
    assert-tos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-planstep
        drop
    then
;

\ Deallocate a planstep list.
: planstep-list-deallocate ( plnstp-lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ plnstp-lst0 uc
    #2 < if
        \ Deallocate planstep instances in the list.
        [ ' planstep-deallocate ] literal over      \ plnstp-lst0 xt plnstp-lst0
        list-apply                                  \ plnstp-lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Print a planstep-list
: .planstep-list ( list0 -- )
    \ Check arg.
    assert-tos-is-planstep-list

    [ ' .planstep ] literal swap .list
;

\ Push a planstep to the end of a planstep-list.
: planstep-list-push-end ( plnstp1 list0 -- )
    \ Check args.
    assert-tos-is-planstep-list
    assert-nos-is-planstep

    over struct-inc-use-count
    list-push-end
;

\ Push a planstep to a planstep-list.
: planstep-list-push ( plnstp1 list0 -- )
    \ Check args.
    assert-tos-is-planstep-list
    assert-nos-is-planstep

    over struct-inc-use-count
    list-push
;

' planstep-list-push to planstep-list-push-xt

\ Append nos planstep-list to the tos planstep-list.
: planstep-list-append ( plnstp-lst1 plnstp-lst0 -- )
    \ Check args.
    assert-tos-is-planstep-list
    assert-nos-is-planstep-list

    swap                    \ plnstp-lst0 plnstp-lst1
    list-get-links          \ plnstp-lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ plnstp-lst0 link nedx
        #2 pick             \ plnstp-lst0 link nedx plnstp-lst0
        planstep-list-push  \ plnstp-lst0 link

        link-get-next
    repeat
                            \ plnstp-lst0
    drop
;

\ Remove a planstep from a planstep-list.
: planstep-list-remove-item ( inx1 plnstp-lst0 -- plnstpx true | false )
    \ Check arg.
    assert-tos-is-planstep-list

    list-remove-item        \ plnstpx true | false
    if
        dup struct-dec-use-count
        true
    else
        false
    then
;

\ Return a list of plansteps that have a sample with the desired changes.
: planstep-list-intersects-changes ( cngs1 plnstp-lst0 -- plnstp-lst )
    \ Check args.
    assert-tos-is-planstep-list
    assert-nos-is-changes

    \ Prep for loop.
    list-new -rot                   \ ret cngs1 plnstp-lst0
    list-get-links                  \ ret cngs1 link

    begin
        ?dup
    while
        over                        \ ret cngs1 link cngs1
        over link-get-data          \ ret cngs1 link cngs1 plnstpx
        planstep-intersects-changes \ ret cngs1 link flag
        if
            dup link-get-data       \ ret cngs1 link plnstpx
            #3 pick                 \ ret cngs1 link plnstpx ret
            planstep-list-push      \ ret cngs1 link
        then

        link-get-next
    repeat
    drop                            \ ret
;

\ Return a reversed planstep list.
: planstep-list-reverse ( plnstp-lst0 -- plnstp-lst )
    assert-tos-is-planstep-list

    \ Init return list.
    list-new swap           \ lst plnstp-lst0

    \ Prep for loop.
    list-get-links          \ lst link

    begin
        ?dup
    while
        dup link-get-data   \ lst link plnstpx
        #2 pick             \ lst link plnstpx lst
        planstep-list-push

        link-get-next
    repeat
;

: planstep-list-match-number-unwanted-changes ( u-unw1 plnstp-lst0 -- plnstp-lst )
    assert-tos-is-planstep-list

    \ Init return list.
    list-new swap               \ u-unw1 ret-lst plnstp-lst0

    \ Prep for loop.
    list-get-links              \ u-unw1 ret-lst link

    begin
        ?dup
    while
        dup link-get-data       \ u-unw1 ret-lst link plnstpx
        dup                     \ u-unw1 ret-lst link plnstpx plnstpx
        planstep-get-number-unwanted-changes    \ u-unw1 ret-lst link plnstpx plnstp-unw
        #4 pick                 \ u-unw1 ret-lst link plnstpx u-unw1 u-unw1
        =                       \ u-unw1 ret-lst link plnstpx bool
        if                      \ u-unw1 ret-lst link plnstpx
            #2 pick             \ u-unw1 ret-lst link plnstpx ret-lst
            planstep-list-push  \ u-unw1 ret-lst link
        else                    \ u-unw1 ret-lst link plnstpx
            drop                \ u-unw1 ret-lst link
        then

        link-get-next
    repeat

    \ Clean up, return.         \ u-unw1 ret-lst
    nip                         \ ret-lst
;

\ Pop the first planstep from a planstep-list.
: planstep-list-pop ( plnstp-lst0 -- plnstp t | f )
    list-pop        \ plnstp t | f
    if
        dup struct-dec-use-count
        true
    else
        false
    then
;

\ Return tru eif any planstep intersects reg-to or reg-from.
: planstep-list-any-from-to-intersections ( reg-to reg-from plnstp-lst0 -- bool )
    \ Check arg3.
    assert-tos-is-list
    assert-nos-is-region
    assert-3os-is-region

    list-get-links                      \ reg-to reg-from link

    begin
        ?dup
    while
        dup link-get-data               \ reg-to reg-from link plnstpx
        dup planstep-get-initial-region \ reg-to reg-from link plnstpx plnstp-i
        #3 pick                         \ reg-to reg-from link plnstpx plnstp-i reg-from
        region-intersects               \ reg-to reg-from link plnstpx bool
        if
            2drop 2drop
            true
            exit
        then

        planstep-get-result-region      \ reg-to reg-from link plnstp-r
        #3 pick                         \ reg-to reg-from link plnstp-r reg-to
        region-intersects               \ reg-to reg-from link bool
        if
            3drop
            true
            exit
        then

        link-get-next
    repeat

    2drop
    false
;

