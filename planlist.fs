\ Functions for plan lists.

\ Check if tos is a list, if non-empty, with the first item being a plan.
: assert-tos-is-plan-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-plan
        drop
    then
;

\ Deallocate a plan list.
: plan-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-plan-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate plan instances in the list.
        [ ' plan-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Print a plan-list
: .plan-list ( list0 -- )
    \ Check args.
    assert-tos-is-plan-list

    ." ("
    current-session swap                \ sess list0
    list-get-links                      \ sess link

    begin
        ?dup
    while
        dup link-get-data               \ sess link plnx
        dup plan-get-domain             \ sess link plnx domx
        #3 pick                         \ sess link plnx domx sess
        session-set-current-domain-xt   \ sess link plnx sess xt
        execute                         \ sess link plnx
        .plan                           \ sess link

        link-get-next                   \ sess link
        dup 0<> if
            space
        then
    repeat

    drop
    ." )"
;

\ Push a plan to the end of a plan-list.
: plan-list-push-end ( stp1 list0 -- )
    \ Check args.
    assert-tos-is-plan-list
    assert-nos-is-plan

    over struct-inc-use-count
    list-push-end
;
