\ Functions for rlcintpair lists.

\ Deallocate a rlcintpair list.
: rlcintpair-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate rlcintpair instances in the list.
        [ ' rlcintpair-deallocate ] literal over    \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Push a rlcintpair to a rlcintpair-list.
: rlcintpair-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-rlcintpair

    over struct-inc-use-count
    list-push
;
