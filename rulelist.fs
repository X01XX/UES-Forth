\ Functions for rule lists.

\ Deallocate a rule list.
: rule-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate rule instances in the list.
        [ ' rule-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Push a rule to the end of a rule-list.
: rule-list-push-end ( rul1 lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-rule

    over struct-inc-use-count
    list-push-end
;

\ Push a rule to a rule-list.
: rule-list-push ( rul1 lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-rule

    over struct-inc-use-count
    list-push
;

\ Print a rule-list
: .rule-list ( list0 -- )
    \ Check arg.
    assert-tos-is-list

    [ ' .rule ] literal swap .list
;
