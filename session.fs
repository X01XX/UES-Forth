\ Implement a Session struct and functions.

#31319 constant session-id
    #9 constant session-struct-number-cells

\ Struct fields
0                                               constant session-header-disp                        \ 16-bits [0] struct id [1] use count
session-header-disp                     cell+   constant session-domains-disp                       \ A domain-list, kind of like senses.
session-domains-disp                    cell+   constant session-current-domain-disp                \ A domain, or zero before first domain is added.
session-current-domain-disp             cell+   constant session-needs-disp                         \ A need-list.
session-needs-disp                      cell+   constant session-regioncorrrate-list-disp           \ Base regioncorr + rate, list.
session-regioncorrrate-list-disp        cell+   constant session-regioncorrrate-fragments-disp      \ Fragments of regioncorrrate-list.
session-regioncorrrate-fragments-disp   cell+   constant session-regioncorrrate-nq-disp             \ A list of of numbers, starting at zero, then rate Negative Qualities,
                                                                                                    \ in descending order.
session-regioncorrrate-nq-disp          cell+   constant session-regioncorr-lol-by-rate-disp        \ A list of regioncorr lists, corresponding to session-regioncorrrate-nq items,
                                                                                                    \ where a plan can move within without encountering a lower rated fragment.
                                                                                                    \ Within an regioncorr list, GT one item, there are intersections,
                                                                                                    \ so there is a path from one rcl, to another, through an intersection.
session-regioncorr-lol-by-rate-disp     cell+   constant session-pathstep-lol-by-rate-disp          \ A list of pathstep lists, corresponding to session-regioncorrrate-nq items.

0 value session-mma     \ Storage for session mma instance.

0 value session-stack   \ Stack for controlled access to session instances,
                        \ primarily for testing purposes.

\ Init session mma, return the addr of allocated memory.
: session-mma-init ( num-items -- ) \ sets region-mma.
    dup 1 <
    abort" session-mma-init: Invalid number of items."

    cr ." Initializing Session store."
    session-struct-number-cells over mma-new to session-mma

    \ Create stack for session instances.
    stack-new to session-stack
;

\ Check instance type.
: is-allocated-session ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup session-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        session-id =
    else
        drop false
    then
;

\ Check TOS for session, unconventional, leaves stack unchanged.
: assert-tos-is-session ( tos -- tos )
    dup is-allocated-session
    is-false if
        s" TOS is not an allocated session"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the domain-list from an session instance.
: session-get-domains ( sess0 -- lst )
    \ Check arg.
    assert-tos-is-session

    session-domains-disp +  \ Add offset.
    @                       \ Fetch the field.
;

\ Set the domain-list for an session instance.
: _session-set-domains ( lst sess0 -- )
    \ Check arg.
    assert-tos-is-session
    assert-nos-is-list

    session-domains-disp +  \ Add offset.
    !                       \ Set the field.
;

\ Return the current domain from an session instance.
: session-get-current-domain ( sess0 -- dom )
    \ Check arg.
    assert-tos-is-session

    session-current-domain-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the current domain for an session instance.
: session-set-current-domain ( dom sess0 -- )
    \ Check arg.
    assert-tos-is-session
    over 0<> if
        assert-nos-is-domain
    then

    session-current-domain-disp +   \ Add offset.
    !                               \ Set the field.
;

' session-set-current-domain to session-set-current-domain-xt

\ Return the session need-list
: session-get-needs ( sess0 -- ned-lst )
    \ Check arg.
    assert-tos-is-session

    session-needs-disp +        \ Add offset.
    @                           \ Fetch the field.
;

\ Set the need-list for an session instance.
: _session-set-needs ( ned-lst sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over struct-inc-use-count
    session-needs-disp +        \ Add offset.
    !                           \ Set the field.
;

\ Update the session needs, deallocating the previous list, if any.
: _session-update-needs  ( ned-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-needs       \ ned-lst sess0 prev-lst
    -rot                        \ prev-lst ned-lst sess0
    _session-set-needs          \ prev-lst
    need-list-deallocate
;

\ Return the session need-list
: session-get-regioncorrrate-list ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-regioncorrrate-list-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Set the need-list for an session instance.
: _session-set-regioncorrrate-list ( rlcrt-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over struct-inc-use-count

    session-regioncorrrate-list-disp + \ Add offset.
    !                           \ Set the field.
;

\ Return the session need-list
: session-get-regioncorrrate-fragments ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-regioncorrrate-fragments-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the need-list for an session instance.
: _session-set-regioncorrrate-fragments ( rlcrt-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over struct-inc-use-count
    session-regioncorrrate-fragments-disp +    \ Add offset.
    !                                   \ Set the field.
;

\ Return the session regioncorrrate-nq list.
: session-get-regioncorrrate-nq ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-regioncorrrate-nq-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the session regioncorrrate-nq for an session instance.
: _session-set-regioncorrrate-nq ( rlcrt-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over struct-inc-use-count
    session-regioncorrrate-nq-disp +    \ Add offset.
    !                                   \ Set the field.
;

: _session-update-regioncorrrate-nq ( rlcrt-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-regioncorrrate-nq -rot  \ prev-list rlcrt-lst1 sess0

    \ Set the field.
    over struct-inc-use-count               \ prev-list rlcrt-lst1 sess0
    session-regioncorrrate-nq-disp +        \ prev-list rlcrt-lst1 sess0+
    !                                       \ prev-list

    \ Deallocate previous list.
    list-deallocate
;

\ Return the session regioncorr-lol-by-rate list.
: session-get-regioncorr-lol-by-rate ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-regioncorr-lol-by-rate-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the session-regioncorr-lol-by-rate list.
: _session-set-regioncorr-lol-by-rate ( rlcrt-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over struct-inc-use-count
    session-regioncorr-lol-by-rate-disp +  \ Add offset.
    !                               \ Set the field.
;

\ Update the session-regioncorr-lol-by-rate list.
: _session-update-regioncorr-lol-by-rate ( rlcrt-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-regioncorr-lol-by-rate -rot    \ prev-list rlcrt-lst1 sess0

    \ Set the field.
    over struct-inc-use-count
    session-regioncorr-lol-by-rate-disp +
    !                                       \ prev-list

    dup struct-dec-use-count
    regioncorr-lol-deallocate
;

\ Return session-pathstep-lol-by-rate list.
: session-get-pathstep-lol-by-rate ( sess0 -- pthstp-lst )
    \ Check arg.
    assert-tos-is-session

    session-pathstep-lol-by-rate-disp + \ Add offset.
    @                                   \ Fetch the field.
;

\ Set session-pathstep-lol-by-rate list.
: _session-set-pathstep-lol-by-rate ( pthstp-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over struct-inc-use-count
    session-pathstep-lol-by-rate-disp +    \ Add offset.
    !                                           \ Set the field.
;

\ Update the session-pathstep-lol-by-rate list.
: _session-update-pathstep-lol-by-rate ( pthstp-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-pathstep-lol-by-rate -rot  \ prev-lst pthstp-lst1 sess0

    \ Set the field.
    over struct-inc-use-count
    session-pathstep-lol-by-rate-disp +
    !                                       \ prev-lst

    dup struct-dec-use-count
    pathstep-lol-deallocate
;
\ End accessors.

: session-stack-tos ( -- sess )
    session-stack stack-tos
;

' session-stack-tos to session-stack-tos-xt

\ Return an rlc of max domain regions.
: session-calc-max-regions ( sess0 -- regioncorr )

    \ Get domain-list.
    dup session-get-domains         \ sess0 dom-lst

    \ Init return list.
    list-new swap                   \ sess0 reg-lst dom-lst

    \ Prep for loop.
    list-get-links                  \ sess0 reg-lst d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ sess0 reg-lst d-link domx
        #3 pick                     \ sess0 reg-lst d-link domx sess0
        session-set-current-domain  \ sess0 reg-lst d-link

        \ Add next region.
        dup link-get-data           \ sess0 reg-lst d-lisk domx
        domain-get-max-region       \ sess0 reg-lst d-lisk regx
        #2 pick                     \ sess0 reg-lst d-lisk regx reg-lst
        region-list-push-end        \ sess0 reg-lst d-lisk

        link-get-next               \ sess0 reg-lst d-link
    repeat
                                    \ sess0 reg-lst
    nip                             \ reg-lst
    regioncorr-new
;

' session-calc-max-regions to session-calc-max-regions-xt

\ Create an session, given an instance ID.
: current-session-new ( -- ) \ new session pushed onto session stack.
    \ cr ." current-session-new: start " .s cr
    \ Allocate space.
    session-mma mma-allocate        \ ses

    \ Store id.
    session-id over                 \ ses id ses
    struct-set-id                   \ ses

    \ Init use count.
    0 over struct-set-use-count     \ ses

    \ Set domains list.
    list-new                        \ ses lst
    dup struct-inc-use-count        \ ses lst
    over _session-set-domains       \ ses

    \ Zero-out current domain.
    0 over session-set-current-domain

    \ Init need-list.
    list-new over _session-set-needs                    \ sess

    \ Init regioncorrrate-list.
    list-new over _session-set-regioncorrrate-list      \ sess

    \ Init regioncorrrate-fragments.
    list-new over _session-set-regioncorrrate-fragments \ sess

    \ Init session-regioncorrrate-nq.
    list-new                                            \ sess lst
    over _session-set-regioncorrrate-nq                 \ sess

    \ Init session regioncorr-lol-by-rate.
    list-new
    over _session-set-regioncorr-lol-by-rate            \ sess

    \ Init rulecorr list, by rate.
    list-new
    over _session-set-pathstep-lol-by-rate              \ sess

    session-stack stack-push
    \ cr ." current-session-new: end " .s cr
;

\ Print a session.
: .session ( sess0 -- )
    \ Check arg.
    assert-tos-is-session

    cr ." Sess: "
    dup session-get-domains
    dup list-get-length
    ."  num domains: " .
    ." domains "

                                                \ sess0 dom-lst
    list-get-links                              \ sess0 link
    begin
        ?dup
    while
        dup link-get-data                       \ sess0 link dom
        \ Set current domain
        dup                                     \ sess0 link dom dom
        #3 pick                                 \ sess0 link dom dom sess0
        session-set-current-domain              \ sess0 link dom
        \ Print domain
        .domain

        link-get-next                           \ sess0 link
    repeat

    cr ." regioncorrrates: "
    dup session-get-regioncorrrate-list                \ sess0 lst
    .regioncorrrate-list
    cr

    cr ." regioncorrrate fragments: "
    dup session-get-regioncorrrate-fragments           \ sess0 lst
    .regioncorrrate-list
    cr

    cr ." regioncorr-lists, excluding lower value regioncorr fragments: "

    \ Prep for loop.
    dup session-get-pathstep-lol-by-rate   \ sess0 rlciplist-lst
    list-get-links                              \ sess0 rlciplist-lst-link

    over session-get-regioncorr-lol-by-rate            \ sess0 rlciplist-lst-link rcllist-lst
    list-get-links                              \ sess0 rlciplist-lst-link rcllist-link

    #2 pick session-get-regioncorrrate-nq       \ sess0 rlciplist-lst-link rcllist-link rates-le0
    list-get-links                              \ sess0 rlciplist-lst-link rcllist-link rates-link

    begin
        ?dup
    while
        cr  ."    rate:      " dup link-get-data #3 dec.r
        space ." regc list:    " over link-get-data .regioncorr-list
        cr cr 15 spaces ." pathstep list: " #2 pick link-get-data
        [ ' .pathstep ] literal over list-apply
        space list-get-length dec. cr

        link-get-next rot
        link-get-next rot
        link-get-next rot
    repeat
    cr
                                        \ sess0 rullstcorrlist-link rcllist-link
    2drop drop                          \
;

\ Deallocate the session.
: current-session-deallocate ( -- )
    session-stack stack-tos         \ sess

    \ Clear fields.
    dup session-get-domains domain-list-deallocate
    dup session-get-needs need-list-deallocate
    dup session-get-regioncorrrate-list regioncorrrate-list-deallocate
    dup session-get-regioncorrrate-fragments regioncorrrate-list-deallocate

    dup session-get-regioncorrrate-nq
    ?dup if
        list-deallocate
    then

    \ Deallocate a list of regclsts.
    dup session-get-regioncorr-lol-by-rate
    regioncorr-lol-deallocate

    \ Deallocate a list of rulcorrlst list.
    session-get-pathstep-lol-by-rate   \ ruls-lst
    pathstep-lol-deallocate           \

    \ Deallocate session.
    session-stack stack-pop
    session-mma mma-deallocate
;

\ Get a sample from an action in a domain.
: session-get-sample ( act2 dom1 sess0 -- sample )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-domain
    assert-3os-is-action

    2dup session-set-current-domain
    -rot                        \ sess0 act2 dom1
    2dup domain-get-sample      \ sess0 act2 dom1 sample
    cr dup .sample

    2nip nip
;

\ Return a sample fom a domain/action, given numeric id values.
: session-get-sample-by-inst-id ( act-id2 dom-id1 sess0 -- sample true | false )
    \ Check args.
    assert-tos-is-session

    swap                            \ act-id2 sess0 dom-id1
    over session-get-domains        \ act-id2 sess0 dom-id dom-lst
    domain-list-find                \ act-id2 sess0, dom true | false
    if
                                    \ act-id2 sess0 dom
        rot                         \ sess0 dom act-id2
        over domain-get-actions     \ sess0 dom act-id2 act-lst
        action-list-find            \ sess0 dom, act true | false
        if                          \ sess0 dom act
            swap                    \ sess0 act dom
            rot                     \ act dom sess0
            session-get-sample      \ sample
        else
            cr ." Action not found" cr
            2drop false
        then
    else
        cr ." Domain not found" cr
        2drop false
    then
;

\ Return a list of states, one for each domain, in domain list order.
: session-get-current-states ( sess0 -- sta-corr-lst )
    \ Check args.
    assert-tos-is-session

    list-new                        \ sess0 sat-lst
    over session-get-domains        \ sess0 sta-lst dom-lst

    list-get-links                   \ sess0 sta-lst link

    begin
        ?dup
    while
        dup link-get-data           \  sess0 sta-lst link domx

        dup #4 pick session-set-current-domain

        domain-get-current-state    \ sess0 sta-lst link stax
        #2 pick                     \ sess0 sta-lst link stax sta-lst
        list-push-end               \ sess0 sta-lst link

        link-get-next               \ sess0 sta-lst link
    repeat
                                    \ sess0 sta-lst
    nip
;

\ Return a list of regions, one for each domain, in domain list order.
: session-get-current-regions ( sess0 -- reg-corr-lst )
    \ Check args.
    assert-tos-is-session

    list-new                        \ sess0 sat-lst
    over session-get-domains        \ sess0 reg-lst dom-lst

    list-get-links                   \ sess0 reg-lst link

    begin
        ?dup
    while
        dup link-get-data           \  sess0 reg-lst link domx

        dup #4 pick session-set-current-domain

        domain-get-current-state    \ sess0 reg-lst link stax
        dup region-new              \ sess0 reg-lst link regx
        #2 pick                     \ sess0 reg-lst link regx reg-lst
        list-push-end               \ sess0 reg-lst link

        link-get-next               \ sess0 reg-lst link
    repeat
                                    \ sess0 reg-lst
    nip
;

\ Print a list of current states.
: .session-current-states ( sess0 -- )
    \ Check args.
    assert-tos-is-session

    dup session-get-domains         \ sess0 dom-lst
    list-get-links                  \ sess0 d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ sess0 d-link domx
        #2 pick                     \ sess0 d-link domx sess0
        session-set-current-domain  \ sess0 d-link

        dup link-get-data           \ sess0 d-link domx
        domain-get-current-state    \ sess0 d-link d-sta
        .value                      \ sess0 d-link

        link-get-next               \ sess0 d-link-nxt
        dup 0<> if
            space
        then
    repeat
                                    \ sess0
    drop
    ." )"

\    session-get-current-states      \ sta-lst
\    dup .state-list-corr            \ sta-lst
\    list-deallocate
;

\ Print a list of reachable regions.
: .session-reachable-regions ( sess0 -- )
    \ Check args.
    assert-tos-is-session

    dup session-get-domains             \ sess0 dom-lst

    list-get-links                      \ sess0 link
    ." ("
    begin
        ?dup
    while
        dup link-get-data               \  sess0 link domx

        dup #3 pick                     \ sess0 link domx domx sess0
        session-set-current-domain      \ sess0 link domx

        domain-calc-reachable-region    \ sess0 link regx
        dup .region
        region-deallocate               \ sess0 link

        link-get-next                   \ sess0 link
        dup if
            space
        then
    repeat
    ." )"
                                    \ sess0
    drop
;

\ Get aggregate changes.
: session-calc-reachable-regions ( sess0 -- reg-lst )
    \ Check args.
    assert-tos-is-session

    dup session-get-domains             \ sess0 dom-lst

    \ Init reachable region list.
    list-new swap                       \ sess0 reg-lst lst0

    \ Scan domain-list, getting needs from each domain.
    list-get-links                      \ sess0 reg-lst link
    begin
        ?dup
    while
        dup link-get-data               \ sess0 reg-lst link domx

        dup #4 pick                      \ sess0 reg-lst link domx domx sess0
        session-set-current-domain      \ sess0 reg-lst link domx

        domain-calc-reachable-region    \ sess0 reg-lst link dom-reg
        #2 pick                         \ sess0 reg-lst link dom-reg reg-lst
        region-list-push-end            \ sess0 reg-lst link

        link-get-next
    repeat
                                        \ sess0 reg-lst

    \ dup .region-list
    nip
;

\ Aggregate all domain needs, store in session instance field.
: session-set-all-needs ( sess0 -- )
    \ Check args.
    assert-tos-is-session

    \ Init list to start appending domain need lists to.
    list-new                            \ s0 ned-lst

    over session-calc-reachable-regions \ s0 ned-lst reg-lst
    tuck                                \ s0 reg-lst ned-lst reg-lst

    #3 pick                             \ s0 reg-lst ned-lst reg-lst s0
    session-get-domains                 \ s0 reg-lst ned-lst reg-lst dom-lst

    \ Prep for loop.
    list-get-links swap                 \ s0 reg-lst ned-lst d-link reg-lst
    list-get-links swap                 \ s0 reg-lst ned-lst r-link d-link

    \ Scan two lists to get all needs
    begin
        ?dup
    while
                                        \ s0 reg-lst ned-lst r-link d-link

        \ Get region and domain
        over link-get-data              \ s0 reg-lst ned-lst r-link d-link | regx
        over link-get-data              \ s0 reg-lst ned-lst r-link d-link | regx domx

        \ Set current domain
        dup                             \ s0 reg-lst ned-lst r-link d-link | regx domx domx
        #7 pick                         \ s0 reg-lst ned-lst r-link d-link | regx domx domx s0
        session-set-current-domain      \ s0 reg-lst ned-lst r-link d-link | regx domx

        \ Get domain needs.
        domain-get-needs                \ s0 reg-lst ned-lst r-link d-link | d-neds

        \ Aggregate needs.
        dup                             \ s0 reg-lst ned-lst r-link d-link | d-neds d-neds
        #4 pick                         \ s0 reg-lst ned-lst r-link d-link | d-neds d-neds ned-lst
        need-list-append                \ s0 reg-lst ned-lst r-link d-link | d-neds

        \ Clean up.
        need-list-deallocate            \ s0 reg-lst ned-lst r-link d-link

        \ Get next links.
        link-get-next swap              \ s0 reg-lst ned-lst d-link' r-link
        link-get-next swap              \ s0 reg-lst ned-lst r-link' d-link'
    repeat
                                        \ s0 reg-lst ned-lst 0
    drop                                \ s0 reg-lst ned-lst
    swap region-list-deallocate         \ s0 ned-lst
    swap _session-update-needs          \
;

\ Return the current domain.
: cur-domain ( -- dom )
    current-session session-get-current-domain
;

' cur-domain to cur-domain-xt

\ Return a domain, given a domain ID.
: session-find-domain ( u1 sess0 -- dom t | f )
    \ Check args.
    assert-tos-is-session
    over 0 < if
        2drop
        false
        exit
    then

    tuck session-get-domains    \ sess0 u1 dom-lst
    2dup list-get-length        \ sess0 u1 dom-lst u1 len
    >= if                       \ sess0 u1 dom-lst
        3drop
        false
        exit
    then

    list-get-item               \ sess0 dom
    tuck swap                   \ dom dom sess0
    session-set-current-domain  \ dom
    true
;

: session-add-regioncorrrate-fragment ( rlcrt1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorrrate

    session-get-regioncorrrate-fragments   \ rlcrt1 rlcrt-lst
    regioncorrrate-list-push               \
;

\ Return the highest rate for a rlc, the highest rate regclst that has a superset rlc.
: session-highest-rate-regclst ( rlc1 sess0 -- n )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-regioncorrrate-nq   \ rlc1 sess0 rate-lst
    list-get-links                      \ rlc1 sess0 rate-link
    over session-get-regioncorr-lol-by-rate    \ rlc1 sess0 rate-link regclst-lst
    list-get-links                      \ rlc1 sess0 rate-link regclst-link

    begin
        ?dup
    while
        #3 pick                         \ rlc1 sess0 rate-link regclst-link rlc1
        over link-get-data              \ rlc1 sess0 rate-link regclst-link rlc1 regclst
        regioncorr-list-any-superset           \ rlc1 sess0 rate-link regclst-link bool
        if                              \ rlc1 sess0 rate-link regclst-link
            over link-get-data          \ rlc1 sess0 rate-link regclst-link rate
            2nip                        \ rlc1 regclst-link rate
            nip nip                     \ rate
            exit
        then

        swap link-get-next
        swap link-get-next
    repeat
    cr ." session-highest-rate-regclst: drop-through?"
    abort
;

: .session-pathstep-lol-by-rate ( sess0 -- )
    \ Check arg.
    assert-tos-is-session

    dup session-get-regioncorrrate-nq   \ sess0 rate-lst
    list-get-links                      \ sess0 rate-link
    over session-get-regioncorr-lol-by-rate    \ sess0 rate-link regclst-lst
    list-get-links                      \ sess0 rate-link regclst-link
    cr ." Lowest  Within"
    cr ." Rate    Regions" cr
    begin
        ?dup
    while
        cr
        over link-get-data #5 dec.r
        dup link-get-data               \ sess0 rate-link regclst-link regclst
        #3 spaces .regioncorr-list                 \ sess0 rate-link regclst-link
        cr

        swap link-get-next
        swap link-get-next
    repeat
                                        \ sess0 rate-link
    2drop
;

\ Calculate pathstep lists for session-pathstep-lol-by-rate.
: session-calc-pathstep-lol ( sess0 -- rullstcorr-lst )
    \ Check arg.
    assert-tos-is-session

    \ Init return list.
    list-new swap                                   \ ret-lst sess0

    \ Prep for loop.
    dup session-get-regioncorr-lol-by-rate          \ ret-lst sess0 rcllist-lst
    list-get-links                                  \ ret-lst sess0 rcllist-link
    over session-get-regioncorrrate-nq              \ ret-lst sess0 rcllist-link rates-le0
    list-get-links                                  \ ret-lst sess0 rcllist-link rates-links
   \  cr

    begin
        ?dup
    while                                           \ ret-lst sess0 rcllist-link rates-links
        \ cr  ." rate: " dup link-get-data #3 dec.r
        \ space ." regclst: " over link-get-data .regioncorr-list cr

        list-new                                    \ ret-lst sess0 rcllist-link rates-links rip-lst
        #2 pick link-get-data                       \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx
        list-get-links                              \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link
        begin
            ?dup
        while                                       \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link
            dup link-get-data                       \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx
            over link-get-next                      \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt
            begin
                ?dup
            while                                           \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt
                \ cr over ."     rlcx: " .regioncorr
                dup link-get-data                           \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt
                \ space dup .regioncorr
                #2 pick                                     \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlcx
                2dup
                regioncorr-intersection                     \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlcx, rlc-int' t | f
                if                                          \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlcx rlc-int'
                    \ Add rulecorr for one rlc to intersection.
                    tuck                                    \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlc-int' rlcx rlc-int'
                    swap                                    \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlc-int' rlc-int' rlcx-nxt
                    rulecorr-new-regioncorr-to-regioncorr   \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlc-int' rul-lc'
                    pathstep-new

                    \ dup space .rulecorr
                     #6 pick                                \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlc-int' rul-lc' rip-lst
                     list-push-struct                       \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlc-int'

                    \ Add rulecorr for other rlc.
                    tuck                                    \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlc-int' rlcx-nxt rlc-int'
                    swap                                    \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlc-int' rlc-int' rlcx-nxt
                    rulecorr-new-regioncorr-to-regioncorr   \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlc-int' rul-lc'
                    pathstep-new
                    \ dup space .rulecorr
                    #5 pick                                 \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlc-int' rul-lc' rip-lst
                    list-push-struct                        \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlc-int'

                    \ Clean up.
                    regioncorr-deallocate                   \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt
                else                                        \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt rlcx-nxt rlcx
                    2drop                                   \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx rlcx-link-nxt
                then
                \ cr
                link-get-next
            repeat
                                                    \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link rlcx
            drop                                    \ ret-lst sess0 rcllist-link rates-links rip-lst rlcx-link
            \ cr
            link-get-next
        repeat
                                                    \ ret-lst sess0 rcllist-link rates-links rip-lst
        \ [ ' .rulecorr ] literal over list-apply
        \ space ." len: " dup list-get-length dec. cr
        dup struct-inc-use-count
        #4 pick                                     \ ret-lst sess0 rcllist-link rates-links rip-lst ret-lst
        list-push-end                               \ ret-lst sess0 rcllist-link rates-links
        link-get-next swap
        link-get-next swap
    repeat
    \ cr
    \ Clean up.                                     \ ret-lst sess0 rcllist-link
    2drop                                           \ ret-lst
   \  cr ." session-calc-pathstep-lol: end: " .stack-structs-xt execute cr
;

\ Process the given regioncorrrates.
: session-process-regioncorrrates ( sess0 -- )
    \ Check arg.
    assert-tos-is-session

   \  cr ." session-process-regioncorrrates" cr

    \ Get given regioncorrrates.
    dup session-get-regioncorrrate-list                \ sess0 rlcrt-lst

    \ cr ." Given regioncorrrates:  " dup .regioncorrrate-list cr

    regioncorrrate-list-to-regioncorr-list                    \ sess0 rlc-lst
    dup                                         \ sess0 rlc-lst rlc-lst

    \ Get rlc fragments of the given regioncorrrates, that are subsets of any given regioncorrrate that they intersect.
    regioncorr-list-intersection-fragments             \ sess0 rlc-lst rlc-lst2
    \ cr ." Fragment RLCs: " dup .regioncorr-list cr

    swap regioncorr-list-deallocate                    \ sess0 rlc-lst2

    \ Check fragments, and find values.
    over session-get-regioncorrrate-list               \ sess0 rlc-lst2 rlcrt-lst
    swap                                        \ sess0 rlcrt-lst rlc-lst2

    \ For each fragment, calc its aggregate rate, form an regioncorrrate, add it to the session-rclrate-fragments list.

    \ Init aggregate rate for the next fragment.
    0 0 rate-new                                \ sess0 rlcrt-lst rlc-lst2 rate-agg

    \ Prep for loop 1.
    over list-get-links                         \ sess0 rlcrt-lst rlc-lst2 rate-agg link

    begin
        ?dup
    while
        dup link-get-data                       \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx

        \ For each given regioncorrrate item.
        #4 pick list-get-links                  \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
        begin
            ?dup
        while
            \ Check if the loop1 rlc fragment interserts the loop2 given regioncorrrate.
            over                                    \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx
            over link-get-data                      \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx regioncorrratey
            regioncorrrate-get-regioncorr           \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty
            2dup                                    \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty rlcrtx rlcrty
            regioncorr-intersects                   \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty bool
            if
                \ Check that the loop2 intersecting regioncorrrate, is a rlc-superset of the loop1 fragment rlc.
                2dup regioncorr-superset            \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty bool
                if
                    \ Add the loop2 regioncorrrate rate to the aggregate rate for the loop1 rlc.
                                                    \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty
                    2drop                           \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
                    dup link-get-data               \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 regioncorrratey
                    regioncorrrate-get-rate                \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rate

                    #4 pick                         \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rate rate-agg
                    rate-add                        \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
                else
                    \ This should not happen, unless there is a problem with the regioncorr-list-intersection-fragments function.
                    cr .regioncorr space ." not superset of " .regioncorr space ." ?" cr
                    abort
                then
                                                \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
            else
                2drop                           \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
            then

            \ Next loop2 cycle.
            link-get-next                       \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
        repeat
                                                \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx

        \ Make regioncorrrate from loop1 fragment rlc and the aggregate rate.
        rot                                     \ sess0 rlcrt-lst rlc-lst2 link | rlcrtx rate-agg
        regioncorrrate-new                             \ sess0 rlcrt-lst rlc-lst2 link | regioncorrrate-new
        \ ." Fragment regioncorrrate: " dup .regioncorrrate cr

        \ Add the loop1 fragment regioncorrrate to the session regioncorrrate-fragments list.
        #4 pick                                 \ sess0 rlcrt-lst rlc-lst2 link | regioncorrrate-new sess0
        session-get-regioncorrrate-fragments           \ sess0 rlcrt-lst rlc-lst2 link | regioncorrrate-new frg-lst
        regioncorrrate-list-push                       \ sess0 rlcrt-lst rlc-lst2 link |

        \ Prep for next loop1 fragment rlc cycle.
        0 0 rate-new                            \ sess0 rlcrt-lst rlc-lst2 link rate-agg
        swap                                    \ sess0 rlcrt-lst rlc-lst2 rate-agg link

        \ Next loop1 cycle.
        link-get-next                           \ sess0 rlcrt-lst rlc-lst2 rate-agg link
    repeat
                                                \ sess0 rlcrt-lst rlc-lst2 rate-agg
    \ Clean up.
    rate-deallocate                             \ sess0 rlcrt-lst rlc-lst2
    regioncorr-list-deallocate                         \ sess0 rlcrt-lst
    drop                                        \ sess0
    dup session-get-regioncorrrate-fragments           \ sess0 frg-lst
    \ cr ." Fragment regioncorrrates: " dup .regioncorrrate-list cr
                                                \ sess0 frg-lst
    \ Get all rate negative values.

    \ Init value list.
    list-new swap                               \ sess0 val-lst frg-lst
    list-get-links                              \ sess0 val-lst link

    begin
        ?dup
    while
        dup link-get-data                       \ sess0 val-lst link regioncorrratex
        regioncorrrate-get-rate                        \ sess0 val-lst link ratex
        rate-get-negative                       \ sess0 val-lst link n
        #2 pick                                 \ sess0 val-lst link n val-lst

        \ Check if its already in the list.
        [ ' = ] literal #2 pick #2 pick         \ sess0 val-lst link n val-lst xt n val-lst
        list-member                             \ sess0 val-lst link n val-lst bool
        if
            2drop
        else
            list-push                           \ sess0 val-lst link
        then

        link-get-next
    repeat
                                                \ sess0 val-lst
    \ Sort so the numerically lowest negative is first.
    \
    \ The lowest negative will have a Freedom of Movement (FOM) of the maximum regions.
    \ That is, move anywhere in the FOM and not encounter a higher negative regioncorrrate.
    \
    \ The second lowest negative rate will have an FOM of max regions - lowest negative regioncorrrates.
    \
    \ The third lowest negative rate will have an FOM of max regions - lowest negative regioncorrrates - second-lowest negative regioncorrrates.
    \
    \ The zero rate FOM will be the maximum regions - all negative regioncorrrates.
    \
    \ Given a starting rlc and a goal rlc:
    \
    \    Start in the highest negative FOM that the start and goal states are in.
    \    If a plan can work within that FOM, use it.
    \    Otherwise try the next least restrictive FOM, possibly encountering higher negative regioncorrrates.
    \
    dup [ ' > ] literal swap list-sort

    \ cr ." values: " [ ' . ] literal  over .list cr

    \ Calculate rlc lists for change navigation.regionlist.fs

    \ Init running subtraction list.
    over session-calc-max-regions               \ sess0 val-lst max-rlc
    list-new                                    \ sess0 val-lst max-rlc sub-lst
    tuck                                        \ sess0 val-lst sub-lst max-rlc sub-lst
    regioncorr-list-push                               \ sess0 val-lst sub-lst

    \ Init result rcl-list.
    list-new                                    \ sess0 val-lst sub-lst rslt-lst
    over struct-inc-use-count
    2dup list-push                              \ sess0 val-lst sub-lst rslt-lst
    swap                                        \ sess0 val-lst rslt-lst sub-lst
                                                \ sess0 val-lst rslt-lst sub-lst
    #2 pick list-get-links                      \ sess0 val-lst rslt-lst sub-lst link

    begin
        ?dup
    while
        \ Get fragments matching the val-list current-value.
        dup link-get-data                       \ sess0 val-lst rslt-lst sub-lst link valx
        \ cr ." val: " dup . cr
        #5 pick                                 \ sess0 val-lst rslt-lst sub-lst link val sess0
        session-get-regioncorrrate-fragments    \ sess0 val-lst rslt-lst sub-lst link val frag-lst
        regioncorrrate-list-match-rate-negative \ sess0 val-lst rslt-lst sub-lst link rlc-lst

        \ Update sub-lst.
        dup                                     \ sess0 val-lst rslt-lst sub-lst link rlc-lst rlc-lst
        #3 pick                                 \ sess0 val-lst rslt-lst sub-lst link rlc-lst rlc-lst sub-lst
        regioncorr-list-subtract                \ sess0 val-lst rslt-lst sub-lst link rlc-lst sub-lst2
        swap regioncorr-list-deallocate         \ sess0 val-lst rslt-lst sub-lst link sub-lst2
        rot drop                                \ sess0 val-lst rslt-lst link sub-lst2
        swap                                    \ sess0 val-lst rslt-lst sub-lst2 link

        \ Add rlc list result list.
        over                                    \ sess0 val-lst rslt-lst sub-lst2 link sub-lst2
        dup struct-inc-use-count
        \ cr ." regclst: " dup .regioncorr-list cr
        #3 pick                                 \ sess0 val-lst rslt-lst sub-lst2 link sub-lst2 rslt-lst
        list-push                               \ sess0 val-lst rslt-lst sub-lst2 link

        link-get-next
    repeat
                                                \ sess0 val-lst rslt-lst sub-lst
    drop                                        \ sess0 val-lst rslt-lst

    \ Process result list.
    #2 pick _session-update-regioncorr-lol-by-rate     \ sess0 val-lst

    0 over list-push
    dup [ ' < ] literal swap list-sort
\    cr ." Fragment values: " [ ' . ] literal  over .list cr

    over _session-update-regioncorrrate-nq      \ sess0

\    dup .session-pathstep-lol-by-rate                \ sess0

    dup session-calc-pathstep-lol         \ sess0 rulecorr-list lists

    swap _session-update-pathstep-lol-by-rate
;

: session-add-domain ( dom1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-domain
    \ cr ." session-add-domain: start " .stack-structs-xt execute cr

    \ Add domain
    2dup                                \ dom1 sess0 dom1 sess0
    session-get-domains                 \ dom1 sess0 dom1 dom-lst
    domain-list-push-end                \ dom1 sess0

    \ Set current-domain, if it is zero/invalid.
    tuck session-set-current-domain     \ sess0

    session-process-regioncorrrates     \ To get rate 0, max region rlc.
;

\ Add a regioncorrrate, to give a value to some arbitrary configuration of domain regions.
: session-add-regioncorrrate ( rlcrt1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorrrate

    tuck session-get-regioncorrrate-list   \ sess0 rlcrt1 rlcrt-lst
    regioncorrrate-list-push               \ sess0

    session-process-regioncorrrates        \ recalc with new regioncorrrate.
;

\ Return the number of domains.
: session-get-number-domains ( -- u )
    current-session
    session-get-domains
    list-get-length
;

' session-get-number-domains to session-get-number-domains-xt

: set-domain ( u1 )
    current-session             \ u1 sess
    tuck session-get-domains    \ sess u1 dom-lst
    list-get-item               \ sess dom
    swap                        \ dom sess
    session-set-current-domain
;

\ Return the session domain list.
: cur-session-get-domain-list ( -- link )
    current-session     \ sess
    session-get-domains \ dom-lst
;

' cur-session-get-domain-list to cur-session-get-domain-list-xt

\ Return the rate and rlc list for a path to satisfy a desired regioncorr.
: session-rlc-rate ( rlc1 sess0 -- rlc rate )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-regioncorrrate-nq       \ rlc1 sess0 rt-lst
    list-get-links                          \ rlc1 sess0 rt-lnk
    swap session-get-regioncorr-lol-by-rate        \ rlc1 rt-lnk rlc-lst
    list-get-links                          \ rlc1 rt-lnk rlc-lnk

    begin
        ?dup
    while
        #2 pick                             \ rlc1 rt-lnk rlc-lnk rlc1
        over link-get-data                  \ rlc1 rt-lnk rlc-lnk rlc1 rlc-lstx
        regioncorr-list-any-superset               \ rlc1 rt-lnk rlc-lnk bool
        if
            link-get-data                   \ rlc1 rt-lnk rlcx
            swap link-get-data              \ rlc1 rlcx rate
            rot drop                        \ rlcx rate
            exit
        then

        link-get-next swap
        link-get-next swap
    repeat

    cr ." session-rlc-rate: drop-through?" cr
    2drop
    0 list-new
;

\ Return plan-list-corr (plc), a multi-domain plan, for moving domain states from one rlc to another.
: session-get-plc ( rlc-to rlc-from sess0 -- plc t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list
    assert-nos-is-list

    list-new                        \ rlc-to rlc-from sess0 ret-plc
    #3 pick list-get-links          \ rlc-to rlc-from sess0 ret-plc to-link
    #3 pick list-get-links          \ rlc-to rlc-from sess0 ret-plc to-link from-link
    #3 pick session-get-domains     \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-lst
    list-get-links                  \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link domx
        #5 pick                     \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link domx sess0
        session-set-current-domain  \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link

        \ Check for noop plan.
        #2 pick link-get-data       \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link reg-to
        #2 pick link-get-data       \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link reg-to reg-from
        region-subset-of            \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link bool
        if
            \ Make noop plan.
            dup link-get-data       \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link dom
            plan-new                \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx
            #3 pick link-get-data   \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx reg-to
            #3 pick link-get-data   \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx reg-to reg-from
            rule-new-region-to-region   \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx rul'

            0                       \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx rul' 0
            #3 pick link-get-data   \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx rul' 0 dom
            domain-find-action      \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx rul', act t | f
            is-false abort" action zero not found?"

                                    \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx rul' act
            planstep-new            \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx stp
            over plan-push          \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx
            #4 pick                 \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx ret-plc
            plan-list-push-end      \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link
        else                        \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link
            \ Get plan for domain.
            #2 pick link-get-data       \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link reg-to
            #2 pick link-get-data       \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link reg-to reg-from
            #2 pick link-get-data       \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link reg-to reg-from domx
            domain-get-plan             \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link, plnx t | f
            if                          \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx
                \ Store plan.
                #4 pick                 \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx ret-plc
                plan-list-push-end      \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link
            else                        \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link
                \ Return false.
                3drop
                plan-list-deallocate    \ rlc-to rlc-from sess0
                3drop
                false
                exit
            then
        then

        link-get-next rot
        link-get-next rot
        link-get-next rot
    repeat

    \ Return plan.                  \ rlc-to rlc-from sess0 ret-plc to-link from-link
    2drop                           \ rlc-to rlc-from sess0 ret-plc
    2nip                            \ sess0 ret-plc
    nip                             \ ret-plc
    true
;

\ Run a plan-list-corr (plc), a multi-domain plan, to move domain states from one rlc to another.
: session-run-plc ( plc1 sess0 -- bool )
\ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over list-get-links             \ plc1 sess0 plc1-link
    over session-get-domains        \ plc1 sess0 plc1-link dom-lst
    list-get-links                  \ plc1 sess0 plc1-link dom-link

    begin
        ?dup
    while
        \ Run domain plan.
        over link-get-data          \ plc1 sess0 plc1-link dom-link pln
        plan-run                    \ plc1 sess0 plc1-link dom-link bool
        is-false if
            2drop
            2drop
            false
            exit
        then

        link-get-next swap
        link-get-next swap
    repeat

                                    \ plc1 sess0 plc1-link
    3drop
    true
;


\ Return highest rate (number, le 0) with regc-list having interserctions with a given regc.
: session-find-rate ( regc1 sess0 -- rate )
    assert-tos-is-session
    assert-nos-is-regioncorr

    \ Find highest rate.
    dup session-get-regioncorr-lol-by-rate  \ | regc-lol
    list-get-links                          \ | regc-lol-link

    over session-get-regioncorrrate-nq      \ | regc-lol-link rates-lst
    list-get-links                          \ | regc-lol-link rates-link

    \ Check each rate and list.
    begin
        ?dup
    while
        #3 pick                             \ | regc-lol-link rates-link rlc1
        #2 pick link-get-data               \ | regc-lol-link rates-link regc-to regc-from regclst
        regioncorr-list-intersects          \ | regc-lol-link rates-link bool
        if
            link-get-data                   \ | regc-lol-link rate
            2nip nip                        \ rate
            exit
        then

        link-get-next swap
        link-get-next swap
    repeat
    cr ." session-find-rate: Drop through?"
    abort
;

\ Return a regc-list for a given rate (number, le 0).
: session-find-regioncorr-list-by-rate ( rate1 rlclst0 -- regioncorr-list )
    \ Check args.
    assert-tos-is-session

    \ Find highest rate.
    dup session-get-regioncorr-lol-by-rate  \ | regclst-lst
    list-get-links                          \ | regclst-link

    over session-get-regioncorrrate-nq      \ | regclst-link rates-lst
    list-get-links                          \ | regclst-link rates-link

    \ Check each rate and list.
    begin
        ?dup
    while
        dup link-get-data                   \ | regclst-link rates-link ratex
        #4 pick                             \ | regclst-link rates-link ratex rate1
        =                                   \ | regclst-link rates-link bool
        if
            drop                            \ | regclst-link
            link-get-data                   \ | regioncorr-list
            nip nip                         \ regioncorr-list
            exit
        then

        link-get-next swap
        link-get-next swap
    repeat
    cr ." session-find-regioncorr-lol-by-rate: Drop through?"
    abort
;

\ Return a pathstep list for a given rate (number, le 0).
: session-find-pathstep-list-by-rate ( rate1 rlc-lol0 -- regc-list )
    \ Check args.
    assert-tos-is-session

    \ Find highest rate.
    dup session-get-pathstep-lol-by-rate       \ | regclst-lst
    list-get-links                          \ | regclst-link

    over session-get-regioncorrrate-nq      \ | regclst-link rates-lst
    list-get-links                          \ | regclst-link rates-link

    \ Check each rate and list.
    begin
        ?dup
    while
        dup link-get-data                   \ | regclst-link rates-link ratex
        #4 pick                             \ | regclst-link rates-link ratex rate1
        =                                   \ | regclst-link rates-link bool
        if
            drop                            \ | regclst-link
            link-get-data                   \ | rlcip-list
            nip nip                         \ rlcip-list
            exit
        then

        link-get-next swap
        link-get-next swap
    repeat
    cr ." session-find-rulecorr-lol-by-rate-disp: Drop through?"
    abort
;

\ Calculate a path, for from/to regioncorrs, given a rate's pathstep list.
: session-calc-path-fc ( regc-to regc-from pthstp-lst1 sess0 -- pthstp-lst t | f )
    \ cr ." session-calc-path-fc: start: from " #2 pick .regioncorr space ." to " #3 pick .regioncorr cr
    \ Check args.
    assert-tos-is-session
    assert-nos-is-pathstep-list
    assert-3os-is-regioncorr
    assert-4os-is-regioncorr

    \ Init return list.             \ regc-to regc-from pthstp-lst1 sess0
    2>r                             \ regc-to regc-from, r: pthstp-lst1 sess0
    list-new                        \ regc-to regc-from ret-lst, r: pthstp-lst1 sess0
    -rot                            \ ret-lst regc-to regc-from, r: pthstp-lst1 sess0
    2r>                             \ ret-lst regc-to regc-from pthstp-lst1 sess0

    \ Promote regc-from to make it easier to replac.
    rot                             \ ret-lst regc-to pthstp-lst1 sess0 | regc-from

    \ In the loop, regc-from is temporary, so protect the passed regc-from from one deallocation.
    dup struct-one-free-deallocate  \ ret-lst regc-to pthstp-lst1 sess0 | regc-from


    begin

        #3 pick                         \ ret-lst regc-to pthstp-lst1 sess0 | regc-from reg-to
        over                            \ ret-lst regc-to pthstp-lst1 sess0 | regc-from reg-to regc-from
        #4 pick                         \ ret-lst regc-to pthstp-lst1 sess0 | regc-from reg-to regc-from pthstp-lst1
        pathstep-list-get-steps-fc      \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstp-lst2
       \  cr ." pathsteps applying to regc-from: " dup .pathstep-list cr

        \ Check for empty list.
        dup list-is-empty
        if
            pathstep-list-deallocate
            regioncorr-deallocate
            2drop 2drop
            false
            exit
        then
                                                                \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstp-lst2

        \ Select pathsteps with the least number-unwanted-changes.
        dup pathstep-list-filter-min-number-unwanted-changes    \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstp-lst2 pthstp-lst3
        swap pathstep-list-deallocate                           \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstp-lst3
       \  cr ." pathsteps filtered: " dup .pathstep-list cr

        \ Choose a pathstep with the least number-unwanted-changes.
        dup list-get-length random          \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstp-lst3 num
        over pathstep-list-remove-item      \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstp-lst3, pthstpx t | f
        is-false abort" pathstep-list item not removed?"
        swap pathstep-list-deallocate       \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx
        \ cr ." pathstep chosen: " dup .pathstep cr

        \ Check if step is already in the return list.
        [ ' = ] literal                     \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx xt
        over                                \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx xt pthstpx
        #7 pick                             \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx pthstpx ret-lst
        list-member                         \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx bool
        if
        \    cr ." step already in list" cr
            drop
            regioncorr-deallocate
            3drop
            pathstep-list-deallocate
            false
            exit
        then

        \ Add pathstep to return pathstep-list.
        dup                                 \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx pthstpx
        #6 pick                             \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx pthstpx ret-lst
        pathstep-list-push-end              \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx

        \ Check if regc-to intersects pathstep rule result regions.
        dup pathstep-get-initial-regions    \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthstpx pthstpx-r
        #5 pick                             \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthsptx pthstpx-r regc-to
        regioncorr-intersects               \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthsptx bool
        if
            \ Clean up.                     \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthsptx
            drop
            regioncorr-deallocate
            3drop
            \ cr ." session-calc-path-using-pathsteps-fc: found end: " dup .pathstep-list cr

            \ Return
            true
            exit
        then

        \ Apply pathstep rule to regc-from, to get next regc-from.
                                            \ ret-lst regc-to pthstp-lst1 sess0 | regc-from pthsptx
        over swap                           \ ret-lst regc-to pthstp-lst1 sess0 | regc-from regc-from pthsptx
        pathstep-get-rules                  \ ret-lst regc-to pthstp-lst1 sess0 | regc-form regc-from pthstp-rulc
        rulecorr-apply-to-regioncorr-fc     \ ret-lst regc-to pthstp-lst1 sess0 | regc-from regc-from' t | f
        is-false abort" false returned?"
       \  cr ." new regc-from: " dup .regioncorr cr
        swap regioncorr-deallocate

    again
;

\ Return a pathstep-list for changing state from/to the given regioncorrs.
\ Early steps will change a regioncorr to an intersection with another pathstep.
\ The last pathstep's initial region will intersect the goal regions.
: session-calc-path ( regc-to regc-from sess0 -- pthstp-lst t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr
    \ cr ." session-calc-path: start: regc-from: " over .regioncorr space ." to: " #2 pick .regioncorr cr

    #2 pick #2 pick regioncorr-intersects
    abort" session-calc-plan: from/to intersect?"

    #2 pick over                                \ regc-to regc-from sess0 | regc-to sess0
    session-find-rate                           \ regc-to regc-from sess0 | rate-to
    #2 pick #2 pick                             \ regc-to regc-from sess0 | rate-to regc-from sess0
    session-find-rate                           \ regc-to regc-from sess0 | rate-to rate-from
    min                                         \ regc-to regc-from sess0 | rate-min
    cr ." rate: " dup dec. cr
    dup                                         \ regc-to regc-from sess0 | rate-min rate-min
    #2 pick                                     \ regc-to regc-from sess0 | rate-min rate-min sess0
    session-find-regioncorr-list-by-rate        \ regc-to regc-from sess0 | rate-min regc-lst
    cr ." regc-lst: " dup .regioncorr-list cr

    dup                                         \ regc-to regc-from sess0 | rate-min regc-lst regc-lst
    #5 pick swap #5 pick swap                   \ regc-to regc-from sess0 | rate-min regc-lst regc-to regc-from regclst
    regioncorr-list-intersects-both             \ regc-to regc-from sess0 | rate-min regc-lst, rlcx t | f
    if                                          \ regc-to regc-from sess0 | rate-min regc-lst rlcx
        cr ." both intersect at: " dup .regioncorr cr
        \ TODO make pathstep list, with single pathstep, regc-from to regc-to.
        \ true
        \ exit
        2drop                                   \ regc-to regc-from sess0 | rate-min
    else                                        \ regc-to regc-from sess0 | rate-min regc-lst
        over                                    \ regc-to regc-from sess0 | rate-min regc-lst rate-min

        #3 pick                                 \ regc-to regc-from sess0 | rate-min regc-lst rate-min sess0
        session-find-pathstep-list-by-rate      \ regc-to regc-from sess0 | rate-min regc-lst pthstp-lst

        #5 pick swap                            \ regc-to regc-from sess0 | rate-min regc-lst regc-to pthstp-lst
        #5 pick swap                            \ regc-to regc-from sess0 | rate-min regc-lst regc-to regc-from pthstp-lst
        #5 pick                                 \ regc-to regc-from sess0 | rate-min regc-lst regc-to regc-from pthstp-lst sess
        session-calc-path-fc                    \ regc-to regc-from sess0 | rate-min regc-lst, pthstp-lst t | f
        if
            2nip 2nip nip                       \ pthstp-lst
            true
            exit
        then
        drop                                    \ regc-to regc-from sess0 | rate-min
    then
                                                \ regc-to regc-from sess0 | rate-min
    2drop 2drop
    false
;

\ Return a plan to change from one regioncorr to another.
\ Does not insur ea plan stays within a desired region, a future improvement.
: session-calc-plancorr ( regc-to regc-from sess0 -- plancorr t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr
    \ cr ." session-calc-plancorr: start: regc-from: " over .regioncorr space ." to: " #2 pick .regioncorr cr

    #2 pick #2 pick regioncorr-intersects
    abort" session-calc-plancorr: from/to intersect?"

    \ Init planlist.
    list-new swap                           \ regc-to regc-from plnc-lst sess0
    2swap                                   \ plnc-lst sess0 regc-to regc-from

    \ Prep for loop.
    swap regioncorr-get-list list-get-links \ plnc-lst sess0 reg-from link-to
    swap regioncorr-get-list list-get-links \ plnc-lst sess0 link-to link-from
    #2 pick session-get-domains             \ plnc-lst sess0 link-to link-from dom-lst
    list-get-links                          \ plnc-lst sess0 link-to link-from link-dom

    begin
        ?dup
    while
        \ cr ." at top while: " .stack-structs-xt execute cr
        \ Set current domain.
        dup link-get-data                   \ plnc-lst sess0 link-to link-from link-dom domx
        #4 pick                             \ plnc-lst sess0 link-to link-from link-dom domx sess0
        session-set-current-domain          \ plnc-lst sess0 link-to link-from link-dom
        \ cr ." after set domain: " .stack-structs-xt execute cr
        
        \ Get plan
        #2 pick link-get-data               \ plnc-lst sess0 link-to link-from link-dom reg-to
        #2 pick link-get-data               \ plnc-lst sess0 link-to link-from link-dom reg-to reg-from
        #2 pick link-get-data               \ plnc-lst sess0 link-to link-from link-dom reg-to reg-from domx
        \ cr ." before get plan: " .stack-structs-xt execute cr
        domain-get-plan                     \ plnc-lst sess0 link-to link-from link-dom, plnx t | f
       \  cr ." after get plan: " .stack-structs-xt execute cr
        if                                  \ plnc-lst sess0 link-to link-from link-dom plnx
            \ Add domain-plan to plan-list.
            #5 pick                         \ plnc-lst sess0 link-to link-from link-dom plnx plnc-lst
            \ cr ." at xx: " .stack-structs-xt execute cr
            plan-list-push-end              \ plnc-lst sess0 link-to link-from link-dom
        else                                \ plnc-lst sess0 link-to link-from link-dom
            \ No plan, exit.
            2drop 2drop                     \ plnc-lst
            plan-list-deallocate            \
            false
            exit
        then

        link-get-next rot
        link-get-next rot
        link-get-next rot
    repeat
                                            \ plnc-lst sess0 link-to link-from
    3drop
    plancorr-new                            \ plnc
    true
;

\ Return a plancorr list, given a linked ( pathsteps intersect left-to-right) pathstep-list.
: session-calc-plnclst-from-pthstplst ( pthstp-lst regc-to regc-from sess0 - -plnc-lst t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr
    assert-4os-is-pathstep-list
    \ cr ." session-calc-plnclst-from-pthstplst: start: regc-from: " over .regioncorr space ." to: " #2 pick .regioncorr cr

    \ Init return list.
    2>r                         \ pthstp-lst regc-to, r: regc-from sess0
    list-new -rot               \ plnc-lst pthstp-lst regc-to, r: regc-from sess0
    2r>                         \ plnc-lst pthstp-lst regc-to regc-from sess0

    \ Promote regc-from, so its easier to replace.
    swap                        \ plnc-lst pthstp-lst regc-to sess0 regc-from

    \ In the loop, regc-from is temporary, so protect the passed regc-from from one deallocation.
    dup struct-one-free-deallocate

    \ Prep for loop.
    #3 pick list-get-links              \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link

    begin
        ?dup
    while
        \ Get next pathstep.
        dup link-get-data               \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx

        \ Check if its the end.
        #4 pick over                    \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx regc-to pthstpx
        pathstep-get-initial-regions    \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx regc-to pthstpx-i
        regioncorr-intersects           \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx bool
        if                              \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx
            \ The end, get plancorr for regc-from to regc-to.
            #4 pick #3 pick #5 pick     \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx regc-to regc-from sess0
            session-calc-plancorr       \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx, plancorr t | f
            if
                #7 pick                 \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx plancorr plnc-lst
                plancorr-list-push-end  \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx
                2drop                   \ plnc-lst pthstp-lst regc-to sess0 regc-from
                regioncorr-deallocate   \ plnc-lst pthstp-lst regc-to sess0
                3drop                   \ plnc-lst
                true
                exit
            then
        else                            \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx
            \ Not the end, get plancorr for regc-from to pathstepx result-regions.
            pathstep-get-result-regions \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx-r
            #2 pick                     \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx-r regc-from
            #4 pick                     \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link pthstpx-r regc-from sess0
            session-calc-plancorr       \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link, plancorr t | f
            if                          \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link plancorr
                \ Calc new regc-from
                dup plancorr-calc-result-regions    \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link plancorr regc-from'
                swap                                \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link regc-from' plancorr

                \ Add plancorr to plancorr-list.
                #7 pick                 \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link regc-from' plancorr plnc-lst
                plancorr-list-push-end  \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link regc-from'

                \ Replace previous regc-from.
                rot regioncorr-deallocate   \ plnc-lst pthstp-lst regc-to sess0 pthstp-link regc-from'
                swap                        \ plnc-lst pthstp-lst regc-to sess0 regc-from' pthstp-link

            else                        \ plnc-lst pthstp-lst regc-to sess0 regc-from pthstp-link
                drop                    \ plnc-lst pthstp-lst regc-to sess0 regc-from
                regioncorr-deallocate   \ plnc-lst pthstp-lst regc-to sess0
                3drop                   \ plnc-lst
                plancorr-list-deallocate
                false
                exit
            then
        then

        link-get-next
    repeat
                                \ plnc-lst pthstp-lst regc-to sess0 regc-from
    drop                        \ plnc-lst pthstp-lst regc-to regc-from
    regioncorr-deallocate       \ plnc-lst pthstp-lst regc-to
    2drop                       \ plnc-lst
    true
;
