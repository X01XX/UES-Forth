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
    get-first-word          \ w t | f
    if
        rulecorr-id =
    else
        false
    then
;

\ Check TOS for rulecorr, unconventional, leaves stack unchanged.
: assert-tos-is-rulecorr ( tos -- tos )
    dup is-allocated-rulecorr
    false? if
        s" TOS is not an allocated rulecorr"
        .abort-xt execute
    then
;

\ Check NOS for rulecorr, unconventional, leaves stack unchanged.
: assert-nos-is-rulecorr ( nos tos -- nos tos )
    over is-allocated-rulecorr
    false? if
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
: rulecorr-new-regc-to-regc ( regc-to regc-from -- rulecorr )
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

\ Return the result regions from applying a regioncorr to a rulecorr's initial regions.
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
        false? if
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

\ Return the initial regions from applying a regioncorr to a rulecorr's result regions.
: rulecorr-apply-to-regioncorr-bc ( regc1 rulc0 -- regc t | f )
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
        rule-apply-to-region-bc     \ reg-lst reg-link rul-link d-link, reg-r' t | f
        false? if
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

\ Return true if the TOS rulecorr is a superset of the NOS rulecorr.
: rulecorr-superset-of ( rulc-sub rulc-sup -- bool )
    \ Check args.
    assert-tos-is-rulecorr
    assert-nos-is-rulecorr
    \ cr ." rulecorr-superset-of: sup: " dup .rulecorr cr
    \    ."                       sub: " over .rulecorr

    \ Prep for loop.
    swap rulecorr-get-list          \ rulc-sup sub-lst
    list-get-links                  \ rulc-sup sub-link

    swap rulecorr-get-list          \ sub-link sup-lst
    list-get-links                  \ sub-link sup-link

    cur-session-get-domain-list-xt  \ sub-link sup-link xt
    execute                         \ sub-link sup-link dom-lst
    list-get-links                  \ sub-link sup-link d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ sub-link sup-link d-link domx
        domain-set-current-xt
        execute                     \ sub-link sup-link d-link

        \ Get curent rules.
        #2 pick link-get-data       \ sub-link sup-link d-link sub-rul
        #2 pick link-get-data       \ sub-link sup-link d-link sub-rul sup-rul

        rule-superset-of            \ sub-link sup-link d-link bool
        if
        else
            3drop
            \ space ." false" cr
            false
            exit
        then

        \ Get next links.
        rot link-get-next           \ sup-link d-link sub-link+
        rot link-get-next           \ d-link sub-link+ sup-link+
        rot link-get-next           \ sub-link+ sup-link+ d-link+
    repeat

    \ space ." true" cr
    true
;

\ Return a rulecorr from a parsed string.
: rulecorr-from-parsed-string ( ... c-addr u cnt -- rulc t | f )

    \ Check number tokens.
    dup                         \ c-addr u cnt cnt
    current-session
    session-get-number-domains-xt
    execute                     \ c-addr u cnt cnt domain-count
    <> if                       \ c-addr u cnt
        0 do
            2drop
        loop
        false
        exit
    then
    drop                        \ c-addr u

    \ Process each rule, skip invalid rules.
                                            \ addr0 cnt0
    list-new                                \ addr0 cnt0 ret-lst
    cur-session-get-domain-list-xt execute
    list-get-links                          \ addr0 cnt0 ret-lst d-link
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ addr0 cnt0 ret-lst d-link domx
        domain-set-current-xt
        execute                     \ addr0 cnt0 ret-lst d-link

        \ Get one rule.
        2swap                       \ ret-lst d-link addr0 cnt0
        rule-from-string            \ ret-lst d-link, rulx

        #2 pick                 \ ret-lst d-link rulx ret-lst
        rule-list-push-end      \ ret-lst d-link


        link-get-next
    repeat

    \ Check results.                \ ret-lst
    dup list-get-length             \ ret-lst len
    current-session
    session-get-number-domains-xt
    execute                         \ ret-lst len dnum
    <> if
        rule-list-deallocate
        false
        exit
    then
                                \ ret-lst
    rulecorr-new                \ rulcorr
    true
;

\ Return a rulecorr from a string.
: rulecorr-from-string ( str-addr str-n -- rulc t | f )
    \ Get tokens.
    parse-string                \ [str-addr str-n ]+ tkn-cnt

    rulecorr-from-parsed-string
;

\ Return a rulecorr from a string, or abort.
: rulecorr-from-string-a ( str-addr str-n -- rulc )
    rulecorr-from-string    \ rulc t | f
    false? abort" rulecorr-from-string failed?"
;

: rulecorr-combine ( rulc-to rulc-from -- rulc )
    \ Check args.
    assert-tos-is-rulecorr
    assert-nos-is-rulecorr
    \ cr ." rulecorr-combine: from: " dup .rulecorr
    \ cr ."                     to: " over .rulecorr

    \ Init rule list.
    list-new -rot                   \ rul-lst-cmb rulc-to rulc-from

    \ Prep for loop.
    swap rulecorr-get-list          \ rul-lst-cmb rulc-from to-lst
    list-get-links                  \ rul-lst-cmb rulc-from to-link

    swap rulecorr-get-list          \ rul-lst-cmb to-link from-lst
    list-get-links                  \ rul-lst-cmb to-link from-link

    cur-session-get-domain-list-xt  \ rul-lst-cmb to-link from-link xt
    execute                         \ rul-lst-cmb to-link from-link dom-lst
    list-get-links                  \ rul-lst-cmb to-link from-link d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ rul-lst-cmb to-link from-link d-link domx
        domain-set-current-xt
        execute                     \ rul-lst-cmb to-link from-link d-link

        \ Get curent rules.
        #2 pick link-get-data       \ rul-lst-cmb to-link from-link d-link to-rul
        #2 pick link-get-data       \ rul-lst-cmb to-link from-link d-link to-rul from-rul

        rule-combine                \ rul-lst-cmb to-link from-link d-link rul-cmb

        \ Store rule combination.
        #4 pick                     \ rul-lst-cmb to-link from-link d-link rul-cmb rul-lst-cmb
        list-push-end-struct        \ rul-lst-cmb to-link from-link d-link

        \ Get next links.
        rot link-get-next           \ rul-lst-cmb from-link d-link to-link+
        rot link-get-next           \ rul-lst-cmb d-link to-link+ from-link+
        rot link-get-next           \ rul-lst-cmb to-link+ from-link+ d-link+
    repeat

                                    \ rul-lst-cmb to-link+ from-link+
    2drop                           \ rul-lst-cmb
    rulecorr-new                    \ rulc
    \ cr ."                      =: " dup .rulecorr cr
;

: pathstep-restrict-initial-regions ( regc1 rulc0 -- rulc t | f )
    \ Check args.
    assert-tos-is-rulecorr
    assert-nos-is-regioncorr

    \ Init rule list.
    list-new -rot                   \ rul-lst-rst regc1 rulc0

    \ Prep for loop.
    swap rulecorr-get-list          \ rul-lst-rst rulc0 regc1-lst
    list-get-links                  \ rul-lst-rst rulc0 regc1-link

    swap rulecorr-get-list          \ rul-lst-rst regc1-link rulc0-lst
    list-get-links                  \ rul-lst-rst regc1-link rulc0-link

    cur-session-get-domain-list-xt  \ rul-lst-rst regc1-link rulc0-link xt
    execute                         \ rul-lst-rst regc1-link rulc0-link dom-lst
    list-get-links                  \ rul-lst-rst regc1-link rulc0-link d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ rul-lst-rst regc1-link rulc0-link d-link domx
        domain-set-current-xt
        execute                     \ rul-lst-rst regc1-link rulc0-link d-link

        \ Get curent rules.
        #2 pick link-get-data       \ rul-lst-rst regc1-link rulc0-link d-link regc1-regx
        #2 pick link-get-data       \ rul-lst-rst regc1-link rulc0-link d-link regc1-regx rulc0-rulx

        rule-restrict-initial-region    \ rul-lst-rst regc1-link rulc0-link d-link, rulx2 t | f
        if
            \ Store rule combination.
            #4 pick                     \ rul-lst-rst regc1-link rulc0-link d-link rul-cmb rul-lst-rst
            list-push-end-struct        \ rul-lst-rst regc1-link rulc0-link d-link

            \ Get next links.
            rot link-get-next           \ rul-lst-rst rulc0-link d-link regc1-link+
            rot link-get-next           \ rul-lst-rst d-link regc1-link+ rulc0-link+
            rot link-get-next           \ rul-lst-rst regc1-link+ rulc0-link+ d-link+
        else
            2drop drop                  \ rul-lst-rst
            rule-list-deallocate        \
            false
            exit
        then
    repeat

                                    \ rul-lst-rst regc1-link+ rulc0-link+
    2drop                           \ rul-lst-rst
    rulecorr-new                    \ rulc
    true
;
