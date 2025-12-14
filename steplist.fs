\ Functions for step lists.

\ Deallocate a step list.
: step-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate step instances in the list.
        [ ' step-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Print a step-list
: .step-list ( list0 -- )
    \ Check args.
    assert-tos-is-list

    [ ' .step ] literal swap .list
;

\ Push a step to the end of a step-list.
: step-list-push-end ( stp1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-step

    over struct-inc-use-count
    list-push-end
;

\ Push a step to a step-list.
: step-list-push ( stp1 list0 -- )
    \ Check args.
    \ cr ." at step-list-push: " .s cr
    assert-tos-is-list
    assert-nos-is-step

    over struct-inc-use-count
    list-push
;

' step-list-push to step-list-push-xt

\ Append nos step-list to the tos step-list.
: step-list-append ( lst1 lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    swap                    \ lst0 lst1
    list-get-links          \ lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ lst0 link nedx
        #2 pick             \ lst0 link nedx lst0
        step-list-push      \ lst0 link

        link-get-next
    repeat
                        \ lst0
    drop
;

: step-list-remove-item ( inx1 lst0 -- stpx true | false )
    \ Check arg.
    assert-tos-is-list

    list-remove-item        \ stpx true | false
    if
        dup struct-dec-use-count
        true
    else
        false
    then
;

\ Return a list of steps that have a sample with the desired changes.
: step-list-intersects-changes ( cngs1 stp-lst0 -- stp-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-changes

    \ Prep for loop.
    list-new -rot               \ ret cngs1 lst0
    list-get-links              \ ret cngs1 link

    begin
        ?dup
    while
        over                    \ ret cngs1 link cngs1
        over link-get-data      \ ret cngs1 link cngs1 stpx
        step-intersects-changes \ ret cngs1 link flag
        if
            dup link-get-data   \ ret cngs1 link stpx
            #3 pick             \ ret cngs1 link stpx ret
            step-list-push      \ ret cngs1 link
        then

        link-get-next
    repeat
    drop                        \ ret
;

\ Return a reversed step list.
: step-list-reverse ( stp-lst0 -- stp-lst )
    \ Check arg.
    assert-tos-is-list

    \ Init return list.
    list-new swap       \ lst stp-lst0

    \ Prep for loop.
    list-get-links       \ lst link

    begin
        ?dup
    while
        dup link-get-data   \ lst link stpx
        #2 pick             \ lst link stpx lst
        step-list-push

        link-get-next
    repeat
;

: step-list-match-number-unwanted-changes ( u-unw1 stp-lst0 -- stp-lst )
    \ Check arg.
    assert-tos-is-list

    \ Init return list.
    list-new swap       \ u-unw1 ret-lst stp-lst0

    \ Prep for loop.
    list-get-links       \ u-unw1 ret-lst link

    begin
        ?dup
    while
        dup link-get-data   \ u-unw1 ret-lst link stpx
        dup                 \ u-unw1 ret-lst link stpx stpx
        step-get-number-unwanted-changes    \ u-unw1 ret-lst link stpx stp-unw
        #4 pick             \ u-unw1 ret-lst link stpx u-unw1 u-unw1
        =                   \ u-unw1 ret-lst link stpx bool
        if                  \ u-unw1 ret-lst link stpx
            #2 pick         \ u-unw1 ret-lst link stpx ret-lst
            step-list-push  \ u-unw1 ret-lst link
        else                \ u-unw1 ret-lst link stpx
            drop            \ u-unw1 ret-lst link
        then

        link-get-next
    repeat

    \ Clean up, return.     \ u-unw1 ret-lst
    nip                     \ ret-lst
;

\ Pop the first step from a step-list.
: step-list-pop ( stp-lst0 -- stp t | f )
    list-pop        \ stp t | f
    if
        dup struct-dec-use-count
        true
    else
        false
    then
;

\ Return tru eif any step intersects reg-to or reg-from.
: step-list-any-from-to-intersections ( reg-to reg-from stp-lst0 -- bool )
    \ Check arg3.
    assert-tos-is-list
    assert-nos-is-region
    assert-3os-is-region

    list-get-links                  \ reg-to reg-from link

    begin
        ?dup
    while
        dup link-get-data           \ reg-to reg-from link stpx
        dup step-get-initial-region \ reg-to reg-from link stpx stp-i
        #3 pick                     \ reg-to reg-from link stpx stp-i reg-from
        region-intersects           \ reg-to reg-from link stpx bool
        if
            2drop 2drop
            true
            exit
        then

        step-get-result-region      \ reg-to reg-from link stp-r
        #3 pick                     \ reg-to reg-from link stp-r reg-to
        region-intersects           \ reg-to reg-from link bool
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
