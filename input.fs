\ Return true if the domain current state satisfies the need.
: need-current-state-satisfies ( ned -- bool )
    \ Check arg.
    assert-tos-is-need

    dup need-get-domain         \ ned dom

    \ Set cur domain.
    dup                         \ ned dom dom
    current-session             \ ned dom dom sess
    session-set-current-domain  \ ned dom

    \ See if a plan is needed.
    domain-get-current-state    \ ned d-sta
    swap need-get-target        \ d-sta n-sta
    =
;

\ Take a sample for a need, the current state matches the need.
: need-take-sample ( ned -- )
    \ Check arg.
    assert-tos-is-need

    dup need-get-action             \ ned act
    over need-get-domain            \ ned act dom

    \ Set cur domain.
    dup                             \ ned act dom dom
    current-session                 \ ned act dom dom sess
    session-set-current-domain      \ ned act dom

    \ See if a plan is needed.
    dup domain-get-current-state    \ ned act dom d-sta
    #3 pick need-get-target         \ ned act dom d-sta n-sta
    =                               \ ned act dom flag
    if
        \ No plan needed, get sample.
        2dup                        \ ned act dom act dom
        domain-get-sample           \ ned act dom sample
        sample-deallocate           \ ned act dom
        domain-get-inst-id
        cr ." Dom: " #3 dec.r       \ ned act
        .action cr                  \ ned
        drop                        \
    else
        cr ." current state does not match need" cr
        abort
    then
;

\ Return a plan for a need.
: need-get-plan ( ned -- pln t | f )
    \ Check arg.
    assert-tos-is-need

    dup need-get-action             \ ned act
    over need-get-domain            \ ned act dom

    \ Set cur domain.
    dup                             \ ned act dom dom
    current-session                 \ ned act dom dom sess
    session-set-current-domain      \ ned act dom

    \ See if a plan is needed.
    dup domain-get-current-state    \ ned act dom d-sta
    #3 pick need-get-target         \ ned act dom d-sta n-sta
    =                               \ ned act dom flag
    abort" no plan needed?"
                                    \ ned act dom
    #2 pick need-get-target         \ ned act dom t-sta
    over domain-get-current-state   \ ned act dom t-sta c-state
    sample-new                      \ ned act dom smpl'

    \ Create from/to regions.
    dup sample-get-result           \ ned act dom smpl' rslt
    dup region-new                  \ ned act dom smpl' reg-to'
    over sample-get-initial         \ ned act dom smpl' reg-to' initial
    dup region-new                  \ ned act dom smpl' reg-to' reg-from'
    2dup                            \ ned act dom smpl' reg-to' reg-from' reg-to' reg-from'
    #5 pick                         \ ned act dom smpl' reg-to' reg-from' reg-to' reg-from' dom

    domain-get-plan                 \ ned act dom smpl' reg-to' reg-from', plan t | f
    if                              \ ned act dom smpl' reg-to' reg-from' plan
        swap region-deallocate      \ ned act dom smpl' reg-to' plan
        swap region-deallocate      \ ned act dom smpl' plan
        swap sample-deallocate      \ ned act dom plan
        2nip                        \ dom plan
        nip                         \ plan
        true
    else                            \ ned act dom smpl' reg-to' reg-from'
        region-deallocate           \ ned act dom smpl' reg-to'
        region-deallocate           \ ned act dom smpl'
        sample-deallocate           \ ned act dom
        3drop                       \
        false
    then
;

\ Run a plan for a need.
: need-run-plan ( plan1 ned0 -- bool )
    \ Check arg.
    assert-tos-is-need
    assert-nos-is-plan

    swap                        \ ned0 plan1
    plan-run                    \ ned0 flag
    nip                         \ flag
;

\ Do a need.  Return true if the need has been satisfied.
: do-need ( ned -- bool )
    \ Check arg.
    assert-tos-is-need

    dup need-get-action         \ ned act
    over need-get-domain        \ ned act dom

    \ Set cur domain.
    dup                         \ ned act dom dom
    current-session             \ ned act dom dom sess
    session-set-current-domain  \ ned act dom

    \ See if a plan is needed.
    dup domain-get-current-state    \ ned act dom d-sta
    #3 pick need-get-target         \ ned act dom d-sta n-sta
    =                               \ ned act dom flag
    if
        \ No plan needed, get sample.
        2dup                        \ ned act dom act dom
        domain-get-sample           \ ned act dom sample
        sample-deallocate           \ ned act dom
        domain-get-inst-id
        cr ." Dom: " dec.           \ ned act
        .action cr                  \ ned
        drop                        \
        true
        exit
    then
                                    \ ned act dom
    #2 pick need-get-target         \ ned act dom t-sta
    over domain-get-current-state   \ ned act dom t-sta c-state
    sample-new                      \ ned act dom smpl'

    \ Create from/to regions.
    dup sample-get-result           \ ned act dom smpl' rslt
    dup region-new                  \ ned act dom smpl' reg-to'
    over sample-get-initial         \ ned act dom smpl' reg-to' initial
    dup region-new                  \ ned act dom smpl' reg-to' reg-from'
    2dup                            \ ned act dom smpl' reg-to' reg-from' reg-to' reg-from'
    #5 pick                         \ ned act dom smpl' reg-to' reg-from' reg-to' reg-from' dom

    domain-get-plan                 \ ned act dom smpl' reg-to' reg-from', plan' t | f
    if
        swap region-deallocate
        swap region-deallocate      \ ned act dom smpl' plan'
        dup plan-run                \ ned act dom smpl' plan' flag
        if
            cr ." plan succeeded " cr
                                    \ ned act dom smpl' plan'
            #3 pick                 \ ned act dom smpl' plan' act
            #3 pick                 \ ned act dom smpl' plan' act dom
            domain-get-sample       \ ned act dom smpl' plan' smpl'
            sample-deallocate       \ ned act dom smpl' plan'
            plan-deallocate         \ ned act dom smpl'
            sample-deallocate       \ ned act dom
            3drop
            true
            exit
        then
                                    \ ned act dom smpl' plan
        cr ." plan failed " cr
        plan-deallocate         \ ned act dom smpl'
        sample-deallocate       \ ned act dom
        3drop
        false
        exit
    then

    cr ." No plan found" cr
    region-deallocate               \ ned act dom smpl' reg-to'
    region-deallocate               \ ned act dom smpl'
    sample-deallocate               \ ned act dom
    3drop
    false
;

: behavior ( -- )  \ Change to the closest more-positive regioncorr fragment.
    current-session                 \ sess

    dup session-get-current-rate    \ sess rate'

    dup rate-is-negative            \ sess rate' bool
    if
        cr ." current states are negative, seek closest non-negative states" cr
        rate-deallocate

        \ Get current states as regions.
        dup session-get-current-regions             \ sess cur-regc'

        \ Get rate LE 0 regioncorr list.
        over session-get-regioncorr-lol-by-rate     \ sess cur-regc' regc-lol
        list-get-links                              \ sess cur-regc' link-first-regc-lst
        link-get-data                               \ sess cur-regc' regc-lst

        \ Get closest regcs.
        over swap                                   \ sess cur-regc' cur-regc' regc-lst
        regioncorr-list-closest-regioncorrs         \ sess cur-regc' clst-regc-lst'

        \ Clean up.
        swap regioncorr-deallocate                  \ sess clst-regc-lst'

        \ Choose a regioncorr.
        dup list-get-length                         \ sess clst-regc-lst' len
        random                                      \ sess clst-regc-lst' inx
        over list-get-item                          \ sess clst-regc-lst' regc

        \ Try to change path.
        #2 pick                                     \ sess clst-regc-lst' regc sess
        session-change-to-plans                     \ sess clst-regc-lst', planc-lst' t | f
        if
            cr ." Plan found: " dup .plancorr-list cr
            dup plancorr-list-run-plans             \ sess clst-regc-lst' planc-lst' bool
            if
                cr ." Plan suceeded" cr
            else
                cr ." Plan failed" cr
            then
            plancorr-list-deallocate                    \ sess clst-lst'
        else
            cr ." No plan found" cr
        then

        regioncorr-list-deallocate                 \ sess
    else                                            \ sess rate'
        cr ." current states are not negative, seek closest more-positive regioncorr fragment" cr
        \ rate-deallocate                             \ sess

        \ Find more-positive regioncorr fragments.
        dup                                             \ sess rate' rate'
        #2 pick session-get-regioncorrrate-fragments    \ sess rate' rate' frg-lst
        regioncorrrate-list-more-positive-regioncorrs   \ sess rate' mrp-lst'
        swap rate-deallocate                            \ sess mrp-lst'

        \ Check for no more-positive items found.
        dup list-is-empty?
        if
            cr ." There are no more-positive regioncorr fragments" cr
            list-deallocate
            drop
            exit
        then

        \ Convert regioncorrrate list to regioncorr-list.
        dup regioncorrrate-list-to-regioncorr-list      \ sess mrp-lst' mrp-lst''
        swap regioncorrrate-list-deallocate             \ sess mrp-lst''

        \ Find closest regioncorr fragments of more positive regioncorr fragments.
        over session-get-current-regions dup            \ sess mrp-lst'' cur-regc' cur-regc'
        #2 pick                                         \ sess mrp-lst'' cur-regc' cur-regc' mrp-lst''
        regioncorr-list-closest-regioncorrs             \ sess mrp-lst'' cur-regc' cls-lst'
        swap regioncorr-deallocate                      \ sess mrp-lst'' cls-lst'
        swap regioncorr-list-deallocate                 \ sess cls-lst'

        \ Select one.
        dup list-get-length random                      \ sess cls-lst' inx
        over list-get-item                              \ sess cls-lst' regcx

         \ Try to change path.
        #2 pick                                         \ sess clst-lst' regcx sess
        session-change-to-plans                         \ sess clst-lst', planc-lst' t | f
        if
            cr ." Plan found: " dup .plancorr-list cr
            dup plancorr-list-run-plans                 \ sess clst-lst' planc-lst' bool
            if
                cr ." Plan suceeded" cr
            else
                cr ." Plan failed" cr
            then
            plancorr-list-deallocate                    \ sess clst-lst'
        else
            cr ." No plan found" cr
        then
        regioncorr-list-deallocate                      \ sess
    then
                                                        \ sess
    drop
;

\ Process a list of needs.
\ Return true if any need was processed,
\ false if no need matched the domain current state and/or
\ no plan was was found for any need.
: process-need-list ( ned-lst0 -- bool )
    \ Check arg.
    assert-tos-is-need-list

    \ Init index list for need list.
    dup list-get-length             \ ned-lst len
    value-list-0-to-n               \ ned-lst inx-lst'

    begin
        dup list-get-length             \ ned-lst inx-lst' len
        random                          \ ned-lst inx-lst' rnd-inx
        dup                             \ ned-lst inx-lst' rnd-inx rnd-inx
        #2 pick                         \ ned-lst inx-lst' rnd-inx rnd-inx inx-lst'
        list-get-item                   \ ned-lst inx-lst' rnd-inx ned-inx

        #3 pick list-get-item           \ ned-lst inx-lst' rnd-inx nedx
        cr ." Need chosen: " space dup .need

        \ Check if no plan needed.
        dup need-current-state-satisfies    \ ned-lst inx-lst' rnd-inx nedx bool
        if
            \ No plan needed.
            cr
            need-take-sample                \ ned-lst inx-lst' rnd-inx
            \ Need satisfied, done.
            drop
            list-deallocate
            drop
            true
            exit
        else                                \ ned-lst inx-lst' rnd-inx nedx
            \ Plan needed.

            \ Check if safer plan is needed, that is, a plan unlikely to pass through
            \ more negative regions that the current regions and goal regions are in..
            current-session                     \ ned-lst inx-lst' rnd-inx nedx sess

            2dup                                \ ned-lst inx-lst' rnd-inx nedx sess nedx sess
            session-regioncorr-for-need         \ ned-lst inx-lst' rnd-inx nedx sess to-regc'
            \ cr ." to regcorr: " dup .regioncorr cr

            over                                \ ned-lst inx-lst' rnd-inx nedx sess regc-to' sess
            session-get-current-regions         \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from'
            \ cr ." from regcorr: " dup .regioncorr cr

            \ Get the lowest rate from reg-to and reg-from highest rates.
            over #3 pick                        \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' regc-to' sess
            session-find-highest-le-zero-rate   \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' rate-to

            over #4 pick                        \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' rate-to regc-from' sess
            session-find-highest-le-zero-rate   \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' rate-to rate-from

            min                                 \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' rate-min
            \ cr ." path: rate: " dup dec. cr

            #3 pick                             \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' rate-min sess
            session-get-regioncorrrate-nq       \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' rate-min nq-lst
            list-get-last-item                  \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' rate-min lowest-rate
            <> if
                \ Rate is better than the lowest, so try safe planning.
                over #3 pick                    \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' regc-to' sess
                session-change-to-plans         \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from', plnc' t | f
                if
                    cr cr ." plan found: " dup .plancorr-list cr
                    dup plancorr-list-run-plans \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' plnc' bool
                    swap                        \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' bool plnc'
                    plancorr-list-deallocate    \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from' bool
                    swap regioncorr-deallocate  \ ned-lst inx-lst' rnd-inx nedx sess regc-to' bool
                    swap regioncorr-deallocate  \ ned-lst inx-lst' rnd-inx nedx sess bool
                    nip                         \ ned-lst inx-lst' rnd-inx nedx bool

                    if
                        cr ." plan suceeded" cr
                        need-take-sample            \ ned-lst inx-lst' rnd-inx
                        drop                        \ ned-lst inx-lst'
                        list-deallocate             \ ned-lst
                        drop
                        true
                        exit
                    else
                        cr ." plan failed" cr
                        2drop                       \ ned-lst inx-lst'
                        list-deallocate             \ ned-lst
                        drop
                        true
                        exit
                    then
                else
                    \ cr ." session-change-to-plans: not found" cr
                                                \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from'
                    regioncorr-deallocate       \ ned-lst inx-lst' rnd-inx nedx sess regc-to'
                    regioncorr-deallocate       \ ned-lst inx-lst' rnd-inx nedx sess
                    drop                        \ ned-lst inx-lst' rnd-inx nedx
                then
            else
                \ Rate is lowest already.
                \ Clean up.                 \ ned-lst inx-lst' rnd-inx nedx sess regc-to' regc-from'
                regioncorr-deallocate       \ ned-lst inx-lst' rnd-inx nedx sess regc-to'
                regioncorr-deallocate       \ ned-lst inx-lst' rnd-inx nedx sess
                drop                        \ ned-lst inx-lst' rnd-inx nedx
            then

            \ Try unsafe plan.
            dup need-get-plan               \ ned-lst inx-lst' rnd-inx nedx, pln t | f
            if                              \ ned-lst inx-lst' rnd-inx nedx pln'
                cr cr ." plan found: " dup .plan cr
                2dup swap                   \ ned-lst inx-lst' rnd-inx nedx pln' pln' nedx
                need-run-plan               \ ned-lst inx-lst' rnd-inx nedx pln' bool
                if
                    cr ." plan suceeded" cr
                    plan-deallocate             \ ned-lst inx-lst' rnd-inx nedx
                    \ TODO check if need requires final sample?
                    need-take-sample            \ ned-lst inx-lst' rnd-inx
                    drop                        \ ned-lst inx-lst'
                    list-deallocate             \ ned-lst
                    drop
                    true
                    exit
                else
                    cr ." plan failed" cr
                    plan-deallocate             \ ned-lst inx-lst' rnd-inx nedx
                    2drop                       \ ned-lst inx-lst'
                    list-deallocate             \ ned-lst
                    drop
                    true
                    exit
                then
            else                            \ ned-lst inx-lst' rnd-inx nedx
                ." , no plan found."
                drop                        \ ned-lst inx-lst' rnd-inx
            then
        then

        \ Need not satisfied, try another, if any.
                                        \ ned-lst inx-lst' rnd-inx

        \ Remove the index, avoiding random picking the same need to try again.
        over                            \ ned-lst inx-lst' rnd-inx inx-lst'
        list-remove-item                \ ned-lst inx-lst' u
        drop                            \ ned-lst inx-lst'

        \ Check if index list is empty.
        dup list-get-length 0=
    until
                                        \ ned-lst inx-lst'
    list-deallocate                     \ ned-lst
    drop
    false
;

: do-zero-token-command ( -- true ) \ Zero-token logic, get/show/act-on needs.
    current-session             \ sess
    session-get-needs           \ ned-lst

    dup list-get-length         \ ned-lst len
    0=
    if
        behavior
        \ ." No needs found" cr
        drop
        true
        exit
    then

    \ Check for Corner anchor square needs.
    need-type-cas over              \ ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ ned-lst, ned-lst' t | f
    if
        dup process-need-list       \ ned-lst ned-lst' bool
        swap need-list-deallocate   \ ned-lst bool
        if
            drop
            true
            exit
        then
    then

    \ Check for Corner dissimilar square needs.
    need-type-cds over              \ ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ ned-lst, ned-lst' t | f
    if
        dup process-need-list       \ ned-lst ned-lst' bool
        swap need-list-deallocate   \ ned-lst bool
        if
            drop
            true
            exit
        then
    then

    \ Check for Confirm Logical structure needs.
    need-type-cls over              \ ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ ned-lst, ned-lst' t | f
    if
        dup process-need-list       \ ned-lst ned-lst' bool
        swap need-list-deallocate   \ ned-lst bool
        if
            drop
            true
            exit
        then
    then

    \ Check for Improve Logical Structure needs.
    need-type-ils over              \ ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ ned-lst, ned-lst' t | f
    if
        dup process-need-list       \ ned-lst ned-lst' bool
        swap need-list-deallocate   \ ned-lst bool
        if
            drop
            true
            exit
        then
    then

    \ Check for State not in group.
    need-type-snig over             \ ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ ned-lst, ned-lst' t | f
    if
        dup process-need-list       \ ned-lst ned-lst' bool
        swap need-list-deallocate   \ ned-lst bool
        if
            drop
            true
            exit
        then
    then

    \ Check for Confirm group.
    need-type-cg over               \ ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ ned-lst, ned-lst' t | f
    if
        dup process-need-list       \ ned-lst ned-lst' bool
        swap need-list-deallocate   \ ned-lst bool
        if
            drop
            true
            exit
        then
    then

    \ Return.
    drop
    true
;

: do-dn-command ( tkn-lst0 -- )   \ Do the "dn" command. Do a need, given it's number in the displayed list.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length #2 <>
    if
        cr ." dn command has an invald number of arguments" cr
        drop
        exit
    then

    list-get-second-item                    \ tkn1
    token-get-string                        \ c-addr u
    snumber?                                \ n t | f
    if
        cr ." You entered number " dup . cr
        cr .stack-gbl cr

        \ Check lower bound.
        dup 0 <                             \ n flag
        if
            cr ." Number entered is out of range" cr
            cr .stack-gbl cr
            drop
            exit
        then                                \ n

        \ Check higher bound.
        current-session                     \ n sess
        session-get-needs                   \ n ned-lst
        dup list-get-length                 \ n ned-lst ned-len
        #2 pick                             \ n ned-lst ned-len n
        swap                                \ n ned-lst n ned-len
        >=
        if                                  \ n ned-lst flag
            cr ." Number entered is out of range" cr
            2drop
            exit
        then                                \ n ned-lst

        \ Get selected need.
        list-get-item                       \ ned
        cr ." You chose need: " dup .need cr
        do-need                             \ bool
        drop
    else
        cr ." dn command argument not a number" cr
    then
;

: do-to-command ( tkn-lst -- )  \ Do the "to" command. Change state to a given regioncorr.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length                         \ tkn-lst0 len
    current-session                             \ tkn-lst0 len sess
    session-get-number-domains 1 +              \ tkn-lst0 len num-dom+
    <> if
        cr ." to command has an invalid number of arguments" cr
        drop
        exit
    then

    \ Get goal regioncorr.
    list-copy-after-first-struct                \ tkn-lst2'
    dup regioncorr-from-token-list              \ tkn-lst2' regc-to' t | f
    false? if
        cr ." to command arguments did not convert to a regioncorr" cr
        token-list-deallocate                   \
        exit
    then
    swap token-list-deallocate                   \ regc-to'

    \ Get current regioncorr.
    current-session session-get-current-regions \ regc-to regc-from'

    \ Check if the current states are already at the goal.
    2dup swap                                   \ regc-to' regc-from' regc-from' regc-to
    regioncorr-superset?                        \ regc-to' regc-from' bool
    if
        cr ." The current states are already at goal." cr
        regioncorr-deallocate
        regioncorr-deallocate
        exit
    then

    cr ." from: " dup .regioncorr space ." to: " over .regioncorr cr
    regioncorr-deallocate                       \ regc-to'
    dup                                         \ regc-to' regc-to'
    current-session                             \ regc-to' regc-to' sess
    session-change-to-plans                     \ regc-to' plnc-lst' t | f
    if
        swap regioncorr-deallocate              \ plnc-lst'

        cr ." Plan found: " dup .plancorr-list cr
        dup plancorr-list-run-plans             \ planc-lst' bool
        if
            cr ." Plan suceeded" cr
        else
            cr ." Plan failed" cr
        then
        plancorr-list-deallocate                \
    else
        cr ." No plan found" cr
        regioncorr-deallocate
    then
;

: do-pd-command ( tkn-lst0 -- ) \ Do the "pd" command. Print a Domain.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length             \ tkn-lst0 len
    #2 <> if
        cr ." pd command: invalid number of arguments" cr
        drop
        exit
    then

    list-get-second-item                         \ tkn1
    token-get-string                             \ c-addr c-cnt

    \ Get domain.
    snumber?
    if
        current-session session-find-domain     \ dom t | f
        if
            \ Set current domain.
            dup current-session session-set-current-domain
            .domain
        else
            cr ." pd command: domain id value invalid" cr
        then
    else
        cr ." pd command: domain id not a number" cr
    then
;

: do-cds-command ( tkn-lst0 -- )    \ Do the "cds" command. Change Domain State command.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length             \ tkn-lst0 len
    #3 <> if
        cr ." cds command: invalid number of arguments" cr
        drop
        exit
    then

    \ Get domain.
    dup list-get-second-item                        \ tkn-lst0 tkn1
    token-get-string                                \ tkn-lst0 c-addr u
    snumber?
    if
        current-session session-find-domain         \ tkn-lst0, dom t | f
        if
            \ Set current domain.
            dup current-session session-set-current-domain
        else
            cr ." cds command: domain id invalid value" cr
            drop
            exit
        then
    else
        cr ." cds command: domain id not a number" cr
        drop
        exit
    then

    \ Get state.
    swap list-get-third-item                    \ dom tkn2
    token-get-string                            \ dom c-addr u
    snumber?                                    \ dom, num t | f
    if
        dup is-value?
        if
            cr ." state " dup . cr
            swap                                \ sta dom
            domain-set-current-state            \
        else
            cr ." cds command: state invalid value" cr
            2drop
        then
    else
        cr ." cds command: state not a number" cr
        drop
    then
;

: do-psd-command ( tkn-lst0 -- ) \ Do the "pds" command. Print square detail for a domain's action.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length             \ tkn-lst0 len
    #3 <> if
        cr ." psd command: invalid number of arguments" cr
        drop
        exit
    then

    \ Get domain.
    dup list-get-second-item                    \ tkn-lst0 tkn1
    token-get-string                            \ tkn-lst0 c-addr-u
    snumber?                                    \ tkn-lst0, dom-id t | f
    if
        \ cr ." domain " dup . cr
        current-session session-find-domain     \ tkn-lst0, dom t | f
        if
            \ Set current domain.
            dup current-session session-set-current-domain
        else
            cr ." psd command: domain id value invalid" cr
            drop
            exit
        then
    else
        cr ." psd command: domain id not a number" cr
        drop
        exit
    then

    \ Get action.
    over list-get-third-item                    \ tkn-lst0 dom tkn2
    token-get-string                            \ tkn-lst0 dom c-addr u
    snumber?                                    \ tkn-lst0 dom, act-id t | f
    if
        swap tuck domain-find-action            \ tkn-lst0 dom, act t | f
        if
            tuck swap                           \ tkn-lst0 act act dom
            domain-set-current-action           \ tkn-lst0 act
            action-get-squares                  \ tkn-lst sqr-lst
            cr .square-list cr
            drop
            exit
        else
            cr ." psd command: action id value invalid" cr
            2drop
        then
    else
        cr ." psd command: action id not a number" cr
        2drop
    then
;

: do-tos-command ( tkn-lst -- ) \ Do the "tos" command. Change a domain to a state.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length                             \ tkn-lst0 len
    #3 <> if
        cr ." tos command: invalid number of arguments" cr
        drop
        exit
    then

    \ Get domain.
    dup list-get-second-item                        \ tkn-lst0 tkn1
    token-get-string                                \ tkn-lst0 c-addr u
    snumber?                                        \ tkn-lst0, num t | f
    if
        current-session session-find-domain         \ tkn-lst0, dom t | f
        if
            \ Set current domain.
            dup current-session session-set-current-domain
        else
            cr ." tos command: domain id value invalid" cr
            drop
            exit
        then
    else
        cr ." tos command: domain id not a number" cr
        drop
        exit
    then

    \ Get state.
    over list-get-third-item                        \ tkn-lst0 dom tkn1
    token-get-string                                \ tkn-lst0 dom c-addr u
    snumber?                                        \ tkn-lst0 dom, to-sta t | f
    if
        dup is-value?
        if
            swap                                    \ tkn-lst0 to-sta dom
            dup domain-get-current-state            \ tkn-lst0 to-sta dom cur-sta
            rot swap                                \ tkn-lst0 dom to-sta cur-sta
            2dup =
            if                                      \ tkn-lst0 dom sta cur-sta
                cr ." Already at that state."
                2drop 2drop
                exit
            then

            \ Do domain-get-plan
            dup region-new swap                     \ tkn-lst0 dom cur-reg to-sta
            dup region-new swap                     \ tkn-lst0 dom to-reg cur-reg

            rot                                     \ tkn-lst0 to-reg cur-reg dom
            #2 pick swap                            \ tkn-lst0 to-reg cur-reg to-reg dom
            #2 pick swap                            \ tkn-lst0 to-reg cur-reg to-reg cur-reg dom
            domain-get-plan                         \ tkn-lst0 to-reg cur-reg, plan' t | f
            if                                      \ tkn-lst0 to-reg cur-reg plan'
                swap region-deallocate              \ tkn-lst0 to-reg plan'
                swap region-deallocate              \ tkn-lst0 plan'
                dup                                 \ tkn-lst0 plan' plan'
                plan-run                            \ tkn-lst0 plan' flag
                swap plan-deallocate                \ tkn-lst0 flag
                if
                    cr ." Plan succeeded" cr
                else
                    cr ." Plan failed" cr
                then
                drop
            else                                \ tkn-lst0 to-reg cur-reg
                region-deallocate               \ tkn-lst0 to-reg
                region-deallocate               \ tkn-lst0
                drop
                cr ." No plan found" cr
            then
        else
            cr ." tos command: state value invalid" cr
            2drop drop
        then
    else
        cr ." tos command: state not a number" cr
        2drop
    then
;

: do-scs-command ( tkn-lst0 -- ) \ Do the "scs" command.  Sample the current state of a domain.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length             \ tkn-lst0 len
    #3 <> if
        cr ." scs command: invalid number of arguments" cr
        drop
        exit
    then

    \ Get domain.
    dup list-get-second-item                        \ tkn-lst0 tkn1
    token-get-string                                \ tkn-lst0 c-addr u
    snumber?                                        \ tkn-lst0, dom-id t | f
    if
        \ cr ." domain " dup . cr
        current-session session-find-domain         \ tkn-lst0, dom t | f
        if
            \ Set current domain.
            dup current-session session-set-current-domain
        else
            cr ." scs command: domain id value invalid" cr
            drop
            exit
        then
    else
        cr ." scs command: domain id not a number" cr
        drop
        exit
    then

    \ Get action.
    over list-get-third-item                    \ tkn-lst0 dom tkn2
    token-get-string                            \ tkn-lst0 dom c-addr u
    snumber?                                    \ tkn-lst0 dom, act-id t | f
    if
        \ cr ." action " dup . cr
        swap tuck domain-find-action            \ tkn-lst0 dom, act t | f
        if
            swap 2dup                           \ tkn-lst0 act dom act dom
            domain-set-current-action           \ tkn-lst0 act dom
            dup domain-get-current-state        \ tkn-lst0 act dom cur-sta
            rot                                 \ tkn-lst0 dom cur-sta act
            action-get-sample                   \ tkn-lst0 dom smpl
            dup sample-get-result               \ tkn-lst0 dom smpl smpl-r
            rot                                 \ tkn-lst0 smpl smpl-r dom
            domain-set-current-state            \ tkn-lst0 smpl
            sample-deallocate                   \ tkn-lst0
            drop
        else
            2drop
            cr ." scs command: action id value invalid" cr
        then
    else
        cr ." scs command: action id not a number" cr
        2drop
    then
;

: do-pa-command ( tkn-lst0 -- ) \ Do "pa" command. Print a domain's action.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length             \ tkn-lst0 len
    #3 <> if
        cr ." pa command: invalid number of arguments" cr
        drop
        exit
    then

    \ Get domain.
    dup list-get-second-item                        \ tkn-lst0 tkn1
    token-get-string                                \ tkn-lst0 c-addr u
    snumber?                                        \ tkn-lst0, u t | f
    if
        current-session session-find-domain         \ tkn-lst0 dom t | f
        if
            \ Set current domain.
            dup current-session session-set-current-domain
        else
            cr ." pa command: domain id value invalid" cr
            drop
            exit
        then
    else
        cr ." pa command: domain id not a number" cr
        drop
        exit
    then
                                                    \ tkn-lst0 dom

    \ Get action.
    swap list-get-third-item                        \ dom tkn2
    token-get-string                                \ dom c-addr u
    snumber?                                        \ dom, act-id t | f
    if                                              \ dom act-id
        over domain-find-action                     \ dom, act t | f
        if                                          \ dom act
            tuck swap                               \ act act dom
            domain-set-current-action               \ act
            .action                                 \
        else                                        \ dom
            cr ." pa command: action id value invalid" cr
            drop
        then
    else                                            \ dom
        cr ." pa command: action id not a number" cr
        drop
    then
;

: do-sas-command ( tkn-lst0 -- ) \ Do the "sas" command. Take an arbitrary domain action for a given state.
    \ Check arg.
    assert-tos-is-token-list

    dup list-get-length             \ tkn-lst0 len
    #4 <> if
        cr ." sas command: invalid number of arguments" cr
        drop
        exit
    then

    \ Get domain.
    dup list-get-second-item                        \ tkn-lst0 tkn1
    token-get-string                                \ tkn-lst0 c-addr u
    snumber?                                        \ tkn-lst0, dom-id t | f
    if
        current-session session-find-domain         \ tkn-lst0, dom t | f
        if
            \ Set current domain.
            dup current-session session-set-current-domain  \ tkn-lst0 dom
        else
            cr ." sas command: domain id value invalid" cr
            drop
            exit
        then
    else
        cr ." sas command: domain id not a number" cr
        drop
        exit
    then

    \ Get action.
    over list-get-third-item                        \ tkn-lst0 dom tkn2
    token-get-string                                \ tkn-lst0 dom c-addr u
    snumber?                                        \ tkn-lst0 dom, act-id t | f
    if
        swap tuck domain-find-action                \ tkn-lst0 dom, act t | f
        if
            swap 2dup                               \ tkn-lst0 act dom act dom
            domain-set-current-action               \ tkn-lst0 act dom
        else
            cr ." sas command: action id value invalid" cr
            2drop
            exit
        then
    else
        cr ." sas command: action id not a number" cr
        2drop
        exit
    then                                            \ tkn-lst0 sta-str act dom

    \ Get state.
    #2 pick list-get-fourth-item                    \ tkn-lst0 act dom tkn2
    token-get-string                                \ tkn-lst0 act dom c-addr u
    snumber?                                        \ tkn-lst0 act dom, sta t | f
    if                                              \ tkn-lst0 act dom sta
        dup is-not-value?                           \ tkn-lst0 act dom sta flag
        if
            cr ." sas command: state value invalid"
            2drop 2drop
            exit
        then

        2dup swap                                   \ tkn-lst0 act dom sta sta dom
        domain-set-current-state                    \ tkn-lst0 act dom sta
        rot                                         \ tkn-lst0 dom sta act
        action-get-sample                           \ tkn-lst0 dom smpl
        dup sample-get-result                       \ tkn-lst0 dom smpl smpl-r
        rot                                         \ tkn-lst0 smpl smpl-r dom
        domain-set-current-state                    \ tkn-lst0 smpl
        sample-deallocate                           \ tnk-lst0
        drop
    else                                            \ tkn-lst0 act dom
        cr ." sas command: state not a number"
        3drop
    then
;

\ Do commands from user input.
\ Return true if the read-eval loop should continue.
: eval-user-input ( tkn-lst0 -- flag )
    \ Check arg.
    assert-tos-is-token-list

    \ Check for no tokens
    dup list-is-empty?
    if
        drop
        do-zero-token-command
        exit
    then

    \ Check command.
    dup list-get-first-item             \ tkn-lst0 tkn0

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" ps" str=                         \ tkn-lst0 tkn0 bool
    if
        drop                            \ tkn-lst0
        list-get-length                 \ len
        1 =
        if
            \ Print Session.
            current-session .session
        else
            cr ." ps command: invalid number of acrguments" cr
        then
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" pa" str=                         \ tkn-lst0 tkn0 bool
    if
        \ Print a domain's action.
        drop                           \
        do-pa-command
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" to" str=                         \ tkn-lst0 tkn0 bool
    if
        \ Go to a given regiorcorr.
        drop                            \ tkn-lst
        do-to-command
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" q" str=                          \ tkn-lst0 tkn0 bool
    if
        drop                            \ tkn-lst0
        list-get-length                 \ len
        1 =
        if
            \ Quit session.
            false
        else
            cr ." q command: invalid number of arguments" cr
            true
        then
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" mu" str=                         \ tkn-lst0 tkn0 bool
    if
        drop                            \ tkn-lst0
        list-get-length                 \ len
        1 =
        if
            \ Display Memory Usage.
            structinfo-list-store structinfo-list-print-memory-use-xt execute
        else
            cr ." mu command: invalid number of arguments" cr
        then
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" dn" str=                         \ tkn-lst0 tkn0 bool
    if
        \ Do a specific need number from the displayed list.
        drop                            \ tkn-lst bool
        do-dn-command                   \
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" pd" str=                         \ tkn-lst0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst0
        do-pd-command                   \
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" cds" str=                         \ tkn-lst0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst0
        do-cds-command                  \
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" psd" str=                         \ tkn-lst0 tkn0 bool
    if
        \ Print square detail for a given domain, action.
        drop                            \ tkn-lst0
        do-psd-command                  \
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" tos" str=                         \ tkn-lst0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst0
        do-tos-command                  \
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" scs" str=                        \ tkn-lst0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst0
        do-scs-command                  \
        true
        exit
    then

    dup token-get-string                \ tkn-lst0 tkn0 c-addr u
    s" sas" str=                         \ tkn-lst0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst0
        do-sas-command                  \
        true
        exit
    then

    cr ." Did not understand the command" cr
    2drop                               \
    true
;

\ Get input of up to TOS characters from user, using the PAD area, up to a given number of characters.
\ Evaluate the input.
\ like: 80 s" Enter command: > " get-user-input
\
\ If this aborts, various things can be done:
\
\ Print all domains, and actions.
\   current-session  .session
\
\ Print Domain 1.
\    1  current-session  session-find-domain  drop  .domain
\
\ Print Domain 1, Act 4.
\    1  current-session  session-find-domain  drop  4  swap  domain-find-action  drop  .action
\
\ Print the squares of domain 1 action 4.
\    1  current-session  session-find-domain  drop  4  swap  domain-find-action  drop  action-get-squares  .square-list
\
\ Return a bool for continuing the REP loop.
: get-user-input ( n c-addr cnt -- )
        \ Display needs.
        current-session             \ n c-addr cnt sess
        dup session-set-all-needs   \ n c-addr cnt sess
        session-get-needs           \ n c-addr cnt ned-lst
        dup list-get-length         \ n c-addr cnt ned-lst len
        dup 0=
        if
            cr ." Needs: No needs found" cr
            2drop
        else
            drop
            cr ." Needs:" cr .need-list cr  \ n c-addr cnt
            cr ." Press Enter to randomly choose a need."
        then

        cr ." q - to quit"
        cr
        cr ." ps - Print Session, all domains."
        cr ." pd <domain id> - Print Domain."
        cr ." pa <domain id> <action id> - Print Action."
        cr ." cds <domain ID> <state> - Change Domain current State, to an arbitrary value."
        cr ." psd <domain ID> <action ID> - Print Square Detail, for a given domain/action."
        cr ." scs <domain id> <action id> - Sample the Current State of a domain, with an action."
        cr ." sas <domain id> <action id> <state> - Sample an Arbitrary State. Change domain current state, then sample with an action."
        cr ." dn <number> - Do Need number."
         cr ." mu - Display Memory Use."
        cr ." tos <domain ID> <state> - TO domain State, from the current state, to an arbtrary value, by finding and executing a plan."
        cr ." to - Change all domain states, like: to (0X00 000X1). Leading zeros are required."
        cr
        cr ." <state> will usually be like: %0101, leading zeros can be ommitted."
        cr

        \ Display the prompt.
        cr
        type                    \ n
        \ Get chars, leaves num chars on TOS.
        pad dup rot accept      \ pad-addr pad-addr n
                                \ pad-addr c-cnt
        cr
        token-list-from-string  \ tkn-lst'
        dup eval-user-input     \ tkn-lst' bool
        swap token-list-deallocate
;
