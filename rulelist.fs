\ Functions for rule lists.

\ Check TOS for rule-list.
: is-rule-list? ( tos -- t )
    assert( tos is-list? )

    dup list-is-empty?
    if
        drop
        true
    else
        list-get-links link-get-data
        assert( is-rule? )
        true
    then
;

\ Deallocate a rule list.
: rule-list-deallocate ( lst0 -- )
    \ Check args.
    assert( tos is-rule-list? )

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate rule instances in the list.
        [ ' rule-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Push a rule to the end of a rule-list.
: rule-list-push-end ( rul1 lst0 -- )
    \ Check args.
    assert( tos is-rule-list? )
    assert( nos is-rule? )

    list-push-end-struct
;

\ Push a rule to a rule-list.
: rule-list-push ( rul1 lst0 -- )
    \ Check args.
    assert( tos is-rule-list? )
    assert( nos is-rule? )

    list-push-struct
;

\ Print a rule-list
: .rule-list ( list0 -- )
    \ Check arg.
    assert( tos is-rule-list? )

    [ ' .rule ] literal swap .list
;

\ Return true if a rule-list is valid for corresponding domains.
: rule-list-corresponding? ( rul-lst0 -- bool )
    \ check arg.
    assert( tos is-list? )

    \ Check list length.
    dup list-get-length
    number-domains-gbl
    <> if
        drop
        false
        exit
    then

    \ Check all items in the list are rules.
    [ ' is-allocated-rule? ] literal over   \ rul-lst0 xt rul-lst0
    list-apply-all-true?                    \ rul-lst0 bool
    if
    else
        drop
        false
        exit
    then

    \ Prep for loop.
    list-get-links                          \ rul-lnk
    get-domain-list-gbl list-get-links      \ rul-lnk d-lnk

    \ Process each token.
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ rul-lnk d-lnk domx
        domain-get-num-bits-xt      \ rul-lnk d-lnk xt
        execute                     \ rul-lnk d-lnk dnb
        #2 pick link-get-data       \ rul-lnk d-lnk dnb rulx
        rule-get-num-bits           \ rul-lnk d-lnk dnb rnb
        =                           \ rul-lnk d-lnk bool
        if
        else
            2drop
            false
            exit
        then

        swap link-get-next
        swap link-get-next
    repeat
                                    \ rul-lnk
    drop
    true
;
