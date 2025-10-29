\ Functions for plan lists.

\ Deallocate a plan list.
: plan-list-deallocate ( lst0 -- )
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
    assert-tos-is-list
    [ ' .plan ] literal swap .list
;

\ Push a plan to the end of a plan-list.
: plan-list-push-end ( stp1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-plan

    over struct-inc-use-count
    list-push-end
;
