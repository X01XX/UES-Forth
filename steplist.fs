\ Functions for step lists.

\ Deallocate a step list.                                                                                                             
: step-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    2 < if
        \ Deallocate step instances in the list.
        [ ' step-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

' step-list-deallocate to step-list-deallocate-xt

\ Print a step-list
: .step-list ( list0 -- )
    \ Check args.
    assert-tos-is-list

    [ ' .step ] literal swap .list
;

' .step-list to .step-list-xt

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

' step-list-append to step-list-append-xt

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

\ Return steps that intersect (one, or two, equal states) a given sample.
: step-list-step-intersections ( smpl1 stp-lst0 -- stp-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-sample

    \ Prep for loop.
    list-new -rot               \ ret smpl1 lst0
    list-get-links              \ ret smpl1 link

    begin
        ?dup
    while
        over                    \ ret smpl1 link smpl1 
        over link-get-data      \ ret smpl1 link smpl1 step
        step-intersects-sample  \ ret smpl1 link flag
        if
            dup link-get-data   \ ret smpl1 link step
            #3 pick             \ ret smpl1 link step ret
            step-list-push      \ ret smpl1 link
        then

        link-get-next
    repeat
                                \ ret smpl1
    drop                        \ ret
;

\ Return steps that do not intersect (one, or two, equal states) a given sample.
: step-list-step-non-intersections ( smpl1 stp-lst0 -- stp-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-sample

    \ Prep for loop.
    list-new -rot               \ ret smpl1 lst0
    list-get-links              \ ret smpl1 link

    begin
        ?dup
    while
        over                    \ ret smpl1 link smpl1 
        over link-get-data      \ ret smpl1 link smpl1 step
        step-intersects-sample  \ ret smpl1 link flag
        0= if
            dup link-get-data   \ ret smpl1 link step
            #3 pick             \ ret smpl1 link step ret
            step-list-push      \ ret smpl1 link
        then

        link-get-next
    repeat
                                \ ret smpl1
    drop                        \ ret
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

\
: step-list-any-match-sample ( smpl1 lst0 -- flag )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-sample

    list-get-links          \ smpl1 link
    begin
        ?dup
    while
        2dup link-get-data  \ smpl1 link smpl1 stpx
        step-get-sample     \ smpl1 link smpl1 stp-smpl
        sample-intersects   \ smpl1 link flag
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat

    drop
    false
;

