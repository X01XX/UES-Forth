\ Functions for plan lists.

\ Check TOS for plan-list.
: is-plan-list? ( tos -- t )
   dup is-list?            \ tos bool
    ifnot
        drop
        false
        exit
    then

    dup list-is-empty?      \ tos bool
    if
        drop
        true
        exit
    then

    list-get-links          \ link
    link-get-data           \ data
    is-plan?                \ bool
;

\ Deallocate a plan list.
: plan-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-plan-list? )

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
    assert( tos is-plan-list? )

    ." ("
    list-get-links                      \ link

    begin
        ?dup
    while
        dup link-get-data               \ link plnx
        dup plan-get-domain             \ link plnx domx
        domain-set-current-gbl          \ link plnx
        .plan                           \ link

        link-get-next                   \ link
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
    assert( tos is-plan-list? )
    assert( nos is-plan? )

    list-push-end-struct
;
