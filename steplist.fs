\ Functions for step lists.

\ Deallocate a step list.
: step-list-deallocate ( list0 -- )
    [ ' step-deallocate ] literal over list-apply   \ Deallocate step instances in the list.
    list-deallocate                                 \ Deallocate list and links.
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
