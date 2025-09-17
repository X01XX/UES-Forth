\ Functions for step lists.

\ Deallocate a step list.
: step-list-deallocate ( list0 -- )
    [ ' step-deallocate ] literal over list-apply   \ Deallocate step instances in the list.
    list-deallocate                                 \ Deallocate list and links.
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
        2 pick              \ lst0 link nedx lst0
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
            3 pick              \ ret smpl1 link step ret
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
            3 pick              \ ret smpl1 link step ret
            step-list-push      \ ret smpl1 link
        then

        link-get-next
    repeat
                                \ ret smpl1
    drop                        \ ret
;
