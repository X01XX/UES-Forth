\ Functions for plan lists.

\ Deallocate a plan list.
: plan-list-deallocate ( list0 -- )
    [ ' plan-deallocate ] literal over list-apply   \ Deallocate plan instances in the list.
    list-deallocate                                 \ Deallocate list and links.
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
