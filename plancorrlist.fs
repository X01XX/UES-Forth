\ Functions used on plancorr lits.

\ Check TOS for plancorr-list.
: is-plancorr-list? ( tos -- t )
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
    is-plancorr?            \ bool
;

\ Deallocate a plancorr list.
: plancorr-list-deallocate ( lst0 -- )
    \ Check arg.
    assert( tos is-plancorr-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate plancorr instances in the list.
        [ ' plancorr-deallocate ] literal over      \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

: .plancorr-list ( plnc-lst -- )
    \ Check arg.
    assert( tos is-plancorr-list? )

    s" (" type
    [ ' .plancorr ] literal swap    \ xt plancorr-list
    list-apply                      \
    s" )" type
;

\ Push a plancorr to the end of a plancorr-list.
: plancorr-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert( tos is-plancorr-list? )
    assert( nos is-plancorr? )

    list-push-end-struct
;

\ Run a plancorr list plancorrs, left to right.
: plancorr-list-run-plans ( plnc-lst -- bool )
    \ Check arg.
    assert( tos is-plancorr-list? )

    \ Prep for loop.
    list-get-links              \ link

    begin
        ?dup
    while
        dup link-get-data       \ link plnc
        plancorr-run            \ link, t | f
        ifnot
            drop
            false
            exit
        then

        link-get-next
    repeat

    \ Return
    true
;
