\ Functions for rule list corresponding (to domains) lists.

\ Return a rule-list-corr for translating from one region-list-corr (rlc-from) to another (rlc-to).
: rule-list-corr-new-rlc-to-rlc ( rlc-to rlc-from -- rule-list-corr )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Init return list.
    list-new -rot                   \ ret-lst rlc-to rlc-from

    \ Prep for loop.
    list-get-links swap
    list-get-links swap             \ ret-lst link-to link-from

    begin
        ?dup
    while
        over link-get-data          \ ret-lst link-to link-from reg-to
        over link-get-data          \ ret-lst link-to link-from reg2 reg-from
        rule-new-region-to-region   \ ret-lst link-to link-from rulx ( rule may have no changes )
        #3 pick                     \ ret-lst link-to link-from rulx ret-lst
        rule-list-push-end          \ ret-lst link-to link-from

        link-get-next swap
        link-get-next swap
    repeat
                                    \ ret-lst link-to
    drop                            \ ret-lst
;

\ Print a rule-list corresponding to the session domain list.
: .rule-list-corr ( rullstcorr0 )
    \ Check arg.
    assert-tos-is-list
    dup list-get-length
    session-get-number-domains-xt execute
    <> abort" Lists have different length?"

    list-get-links                                      \ link0
    session-get-domain-list-xt execute list-get-links   \ link0 d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ link0 d-link domx
        domain-set-current-xt
        execute                     \ link0 d-link

        over link-get-data          \ link0 d-link reg0
        .rule                     \ link0 d-link

        swap link-get-next      \ d-link link0
        swap link-get-next      \ link0 d-link
        dup if
            space
        then
    repeat
                                \ link0
    drop
    ." )"
;

: rule-list-corr-list-deallocate ( rul-lst-corr-lst0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' rule-list-deallocate ] literal over     \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list. 
        list-deallocate                            \
    else
        struct-dec-use-count
    then
;

\ Deallocate a list of lists of rule-list-corr.
: rule-list-corr-lol-deallocate ( rul-lst-corr-lst-lol0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' rule-list-corr-list-deallocate ] literal over   \ lst0 xt lst0
        list-apply                                          \ lst0

        \ Deallocate the list. 
        list-deallocate                            \
    else
        struct-dec-use-count
    then
;
