\ Functions related to state lists where the states correspond to a session domain list.

\ Print a state-list corresponding to the session domain list.
: .state-list-corr ( sta-lst0 )
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

        over link-get-data          \ link0 d-link sta0
        .value                      \ link0 d-link

        swap link-get-next          \ d-link link0
        swap link-get-next          \ link0 d-link
        dup if
            space
        then
    repeat
                                    \ link0
    drop
    ." )"
;

