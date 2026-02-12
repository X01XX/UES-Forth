\ Implement a struct and functions for a list of rules, corresponding to domains.

#53171 constant rulecorr-id
    #2 constant rulecorr-struct-number-cells

\ Struct fields
0                              constant rulecorr-header-disp    \ 16-bits [0] struct id [1] use count.
rulecorr-header-disp   cell+   constant rulecorr-list-disp      \ Rule list corresponding, in bits used, to the session domain list.

0 value rulecorr-mma \ Storage for rule mma instance.

\ Init rule mma, return the addr of allocated memory.
: rulecorr-mma-init ( num-items -- ) \ sets rulecorr-mma.
    dup 1 <
    abort" rulecorr-mma-init: Invalid number of items."

    cr ." Initializing RuleCorr store."
    rulecorr-struct-number-cells swap mma-new to rulecorr-mma
;

\ Check instance type.
: is-allocated-rulecorr ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup rulecorr-mma mma-within-array
    if
        struct-get-id
        rulecorr-id =
    else
        drop false
    then
;

\ Check TOS for rulecorr, unconventional, leaves stack unchanged.
: assert-tos-is-rulecorr ( tos -- tos )
    dup is-allocated-rulecorr
    is-false if
        s" TOS is not an allocated rulecorr"
        .abort-xt execute
    then
;

\ Check NOS for rulecorr, unconventional, leaves stack unchanged.
: assert-nos-is-rulecorr ( nos tos -- nos tos )
    over is-allocated-rulecorr
    is-false if
        s" NOS is not an allocated rulecorr"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the rulecorr list field from a rule instance.
: rulecorr-get-list ( rulc0 -- rul-lst )
    \ Check arg.
    assert-tos-is-rulecorr

    rulecorr-list-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the rulecorr list field of a rule instance, use only in this file.
: _rulecorr-set-list ( lst1 rulc0 -- )
    \ Check args.
    assert-tos-is-rulecorr

    \ Store list
    rulecorr-list-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ End accessors.

\ Create a rulecorr-list-corr from a rulecorr-list-corr-list on the stack.
: rulecorr-new ( rul-lst0 -- rulc )
    \ check arg.
    assert-tos-is-rule-list
    dup list-get-length
    current-session session-get-number-domains-xt execute
    <> abort" rulecorr-new: invalid list length?"

    \ Allocate space.
    rulecorr-mma mma-allocate   \ rul-lst0 rulc

    \ Store id.
    rulecorr-id over            \ rul-lst0 rulc id rulc
    struct-set-id               \ rul-lst0 rulc

    \ Init use count.
    0 over struct-set-use-count \ rul-lst0 rulc

    tuck                        \ rulc rul-lst0 rulc
    _rulecorr-set-list          \ rulc
;

\ Print a rule-list corresponding to the session domain list.
: .rulecorr ( rulc0 -- )
    \ Check arg.
    assert-tos-is-rulecorr

    rulecorr-get-list               \ rul-lst
    list-get-links                  \ rc-link
    cur-session-get-domain-list-xt  \ rc-link xt
    execute                         \ rc-link dom-lst
    list-get-links                  \ rc-link d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ rc-link d-link domx
        domain-set-current-xt
        execute                     \ rc-link d-link

        over link-get-data          \ rc-link d-link reg0
        .rule                       \ rc-link d-link

        swap link-get-next          \ d-link rc-link
        swap link-get-next          \ rc-link d-link
        dup if
            space
        then
    repeat
                                    \ rc-link
    drop
    ." )"
;

\ Deallocate the given rulecorr, if its use count is 1 or 0.
: rulecorr-deallocate ( rulc0 -- )
    \ Check arg.
    assert-tos-is-rulecorr

    dup struct-get-use-count        \ rulc0 count

    #2 <
    if
        \ Deallocate fields.
        dup rulecorr-get-list       \ rulc0 rul-lst
        rule-list-deallocate        \ rulc0

        \ Deallocate instance.
        rulecorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return a rulecorr for translating from one rule-list-corr (rulecorr-from) to another (rulecorr-to).
: rulecorr-new-regioncorr-to-regioncorr ( regc-to regc-from -- rulecorr )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    \ Init return list.
    list-new -rot                           \ rul-lst regc-to regc-from

    \ Prep for loop.
    regioncorr-get-list list-get-links swap \ rul-lst link-from regc-to
    regioncorr-get-list list-get-links swap \ rul-lst link-to link-from

    begin
        ?dup
    while
        over link-get-data          \ rul-lst link-to link-from reg-to
        over link-get-data          \ rul-lst link-to link-from reg-to reg-from
        rule-new-region-to-region   \ rul-lst link-to link-from rulx ( rule may have no changes )
        #3 pick                     \ rul-lst link-to link-from rulx rul-lst
        rule-list-push-end          \ rul-lst link-to link-from

        link-get-next swap
        link-get-next swap
    repeat
                                    \ rul-lst link-to
    drop                            \ rul-lst
    rulecorr-new                    \ rulc
;

: rulecorr-calc-initial-regions ( rulc -- regc )
    \ Check arg.
    assert-tos-is-rulecorr

    \ Init region list.
    list-new swap                   \ reg-lst rulc

    \ Prep for loop.
    rulecorr-get-list               \ reg-lst rul-lst
    list-get-links                  \ reg-lst rc-link
    cur-session-get-domain-list-xt  \ reg-lst rc-link xt
    execute                         \ reg-lst rc-link dom-lst
    list-get-links                  \ reg-lst rc-link d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ reg-lst rc-link d-link domx
        domain-set-current-xt
        execute                     \ reg-lst rc-link d-link

        \ Calc initial region.
        over link-get-data          \ reg-lst rc-link d-link rulx
        rule-calc-initial-region    \ reg-lst rc-link d-link reg-i'

        \ Store initial region.
        #3 pick                     \ reg-lst rc-link d-link reg-i' reg-lst
        list-push-end-struct        \ reg-lst rc-link d-link

        swap link-get-next          \ reg-lst d-link rc-link
        swap link-get-next          \ reg-lst rc-link d-link
    repeat
                                    \ reg-lst rc-link
    drop                            \ reg-lst
    regioncorr-new                  \ regc
;

: rulecorr-calc-result-regions ( rulc -- regc )
    \ Check arg.
    assert-tos-is-rulecorr

    \ Init region list.
    list-new swap                   \ reg-lst rulc

    \ Prep for loop.
    rulecorr-get-list               \ reg-lst rul-lst
    list-get-links                  \ reg-lst rul-link
    cur-session-get-domain-list-xt  \ reg-lst rul-link xt
    execute                         \ reg-lst rul-link dom-lst
    list-get-links                  \ reg-lst rul-link d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ reg-lst rul-link d-link domx
        domain-set-current-xt
        execute                     \ reg-lst rul-link d-link

        \ Calc result region.
        over link-get-data          \ reg-lst rul-link d-link rulx
        rule-calc-result-region    \ reg-lst rul-link d-link reg-i'

        \ Store result region.
        #3 pick                     \ reg-lst rul-link d-link reg-i' reg-lst
        list-push-end-struct        \ reg-lst rul-link d-link

        swap link-get-next          \ reg-lst d-link rul-link
        swap link-get-next          \ reg-lst rul-link d-link
    repeat
                                    \ reg-lst rul-link
    drop                            \ reg-lst
    regioncorr-new                  \ regc
;

\ Return changescorr from a rulecorr.
: rulecorr-get-changes ( rulc -- cngsc )
    \ Check arg.
    assert-tos-is-rulecorr

    \ Init region list.
    list-new swap                   \ cngs-lst rulc

    \ Prep for loop.
    rulecorr-get-list               \ cngs-lst rul-lst
    list-get-links                  \ cngs-lst rc-link

    begin
        ?dup
    while
        \ Get changes.
        dup link-get-data           \ cngs-lst rc-link rulx
        rule-get-changes            \ cngs-lst rc-link cngs'

        \ Store changes.
        #2 pick                     \ cngs-lst rc-link cngs' cngs-lst
        list-push-end-struct        \ cngs-lst rc-link

        link-get-next               \ cngs-lst rc-link
    repeat
                                    \ cngs-lst
    changescorr-new                 \ regc
;

\ Apply a rulecorr to a regioncorr, forward-chaining.
: rulecorr-apply-to-regioncorr-fc ( regc1 rulc0 -- regc t | f )
    \ Check args.
    assert-tos-is-rulecorr
    assert-nos-is-regioncorr

    \ Init region list.
    list-new -rot                   \ reg-lst regc1 rulc0

    \ Prep for loop.
    rulecorr-get-list               \ reg-lst regc1 rul-lst
    list-get-links                  \ reg-lst regc1 rul-link

    swap                            \ reg-lst rul-link regc1
    regioncorr-get-list             \ reg-lst rul-link reg-lst
    list-get-links                  \ reg-lst rul-link reg-link
    swap                            \ reg-lst reg-link rul-link

    cur-session-get-domain-list-xt  \ reg-lst reg-link rul-link xt
    execute                         \ reg-lst reg-link rul-link dom-lst
    list-get-links                  \ reg-lst reg-link rul-link d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ reg-lst reg-link rul-link d-link domx
        domain-set-current-xt
        execute                     \ reg-lst reg-link rul-link d-link

        \ Calc result of applying rule.
        #2 pick link-get-data       \ reg-lst reg-link rul-link d-link regx
        #2 pick link-get-data       \ reg-lst reg-link rul-link d-link regx rulx
        rule-apply-to-region-fc     \ reg-lst reg-link rul-link d-link, reg-r' t | f
        is-false if
            3drop
            region-list-deallocate
            false
            exit
        then

        \ Store result region.
        #4 pick                     \ reg-lst reg-link rul-link d-link reg-r' reg-lst
        list-push-end-struct        \ reg-lst reg-link rul-link d-link

        rot link-get-next           \ reg-lst rul-link d-link reg-link+
        rot link-get-next           \ reg-lst d-link reg-link+ rul-link+
        rot link-get-next           \ reg-lst reg-link+ rul-link+ d-link+
    repeat
                                    \ reg-lst reg-link rul-link
    2drop                           \ reg-lst
    regioncorr-new                  \ regc
    true
;
