\ Functions for rule list corresponding (to domains) lists.

\ Return a rule-list-corr for translating from one region-list-corr (rlc0) to another (rlc1).
: rule-list-corr-new-rlc-to-rlc ( rlc2 rlc1 -- rule-lc t | f )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Init return list.
    list-new -rot                   \ ret-lst rlc2 rlc1

    \ Prep for loop.
    list-get-links swap
    list-get-links swap             \ ret-lst link2 link1

    begin
        ?dup
    while
        over link-get-data          \ ret-lst link2 link1 reg2
        over link-get-data          \ ret-lst link2 link1 reg2 reg1
        rule-new-region-to-region   \ ret-lst link2 link1 rulx ( rule may have no changes )
        #3 pick                     \ ret-lst link2 link1 rulx ret-lst 
        rule-list-push-end          \ ret-lst link2 link1

        link-get-next swap
        link-get-next swap
    repeat
                                    \ ret-lst link2
    drop                            \ ret-lst
    true
;

\ Print a rule-list corresponding to the session domain list.
: .rule-list-corr ( reg-lst0 )
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


