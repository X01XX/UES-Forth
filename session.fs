\ Implement a Session struct and functions.

#31319 constant session-id
   #10 constant session-struct-number-cells

\ Struct fields
0                                               constant session-header-disp                        \ 16-bits [0] struct id [1] use count:w

session-header-disp                     cell+   constant session-domains-disp                       \ A domain-list, kind of like senses.
session-domains-disp                    cell+   constant session-current-domain-disp                \ A domain, or zero before first domain is added.
session-current-domain-disp             cell+   constant session-needs-disp                         \ A need-list.
session-needs-disp                      cell+   constant session-regioncorrrate-list-disp           \ Base regioncorr + rate, list.
session-regioncorrrate-list-disp        cell+   constant session-regioncorrrate-fragments-disp      \ Fragments of regioncorrrate-list.
session-regioncorrrate-fragments-disp   cell+   constant session-regioncorrrate-nq-disp             \ A list of of numbers, starting at zero, then regioncorrrate
                                                                                                    \ fragment Negative Qualities (nq), in descending order.
session-regioncorrrate-nq-disp          cell+   constant session-regioncorr-lol-by-rate-disp        \ A list of regioncorr lists, corresponding to session-regioncorrrate-nq items,
                                                                                                    \ where a plan can move within without encountering a lower rated fragment.
                                                                                                    \ Successive subtraction of regioncorrs from a the maximum regioncorr,
                                                                                                    \ leads to regioncorrs with intersections.
                                                                                                    \ So there is a path from one regioncorr, to another, through an intersection.
session-regioncorr-lol-by-rate-disp     cell+   constant session-pathstep-lol-by-rate-disp          \ A list of pathstep lists, corresponding to session-regioncorrrate-nq items.
session-pathstep-lol-by-rate-disp       cell+   constant session-points-disp                        \ Signed "points" counter based on current states in regioncorrrate fragments.
session-points-disp                     cell+   constant session-previous-points-disp               \ Previous session/user step value of session-points.


0 value session-mma     \ Storage for session mma instance.

\ Init session mma, return the addr of allocated memory.
: session-mma-init ( num-items -- ) \ sets region-mma.
    dup 1 <
    abort" session-mma-init: Invalid number of items."

    cr ." Initializing Session store."
    session-struct-number-cells swap mma-new to session-mma
;

\ Check instance type.
: is-allocated-session ( addr -- flag )
    get-first-word          \ w t | f
    if
        session-id =
    else
        false
    then
;

\ Check TOS for session, unconventional, leaves stack unchanged.
: assert-tos-is-session ( tos -- tos )
    dup is-allocated-session
    false? if
        s" TOS is not an allocated session"
        .abort-xt execute
    then
;

' assert-tos-is-session to assert-tos-is-session-xt

\ Start accessors.

: session-get-domains ( sess0 -- lst )  \ Return the domain-list from an session instance.
    \ Check arg.
    assert-tos-is-session

    session-domains-disp +  \ Add offset.
    @                       \ Fetch the field.
;

' session-get-domains to session-get-domains-xt

: _session-set-domains ( lst sess0 -- ) \ Set the domain-list for an session instance.
    \ Check arg.
    assert-tos-is-session
    assert-nos-is-list

    session-domains-disp +  \ Add offset.
    !struct                 \ Set the field.
;

\ Return the current domain from an session instance.
: session-get-current-domain ( sess0 -- dom )
    \ Check arg.
    assert-tos-is-session

    session-current-domain-disp +   \ Add offset.
    @                               \ Fetch the field.
;

' session-get-current-domain to session-get-current-domain-xt

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
    assert-nos-is-need-list

    session-needs-disp +        \ Add offset.
    !struct                     \ Set the field.
;

\ Update the session needs, deallocating the previous list, if any.
: _session-update-needs  ( ned-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-need-list

    dup session-get-needs       \ ned-lst sess0 prev-lst
    -rot                        \ prev-lst ned-lst sess0
    _session-set-needs          \ prev-lst
    need-list-deallocate
;

\ Return the session regioncorrrate list.
: session-get-regioncorrrate-list ( sess0 -- regcr-lst )
    \ Check arg.
    assert-tos-is-session

    session-regioncorrrate-list-disp +  \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the regioncorrrate list for an session instance.
: _session-set-regioncorrrate-list ( regcr-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorrrate-list

    session-regioncorrrate-list-disp +  \ Add offset.
    !struct                             \ Set the field.
;

\ Return the session need-list
: session-get-regioncorrrate-fragments ( sess0 -- regcr-lst )
    \ Check arg.
    assert-tos-is-session

    session-regioncorrrate-fragments-disp + \ Add offset.
    @                                       \ Fetch the field.
;

\ Set the need-list for an session instance.
: _session-set-regioncorrrate-fragments ( regcr-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorrrate-list

    session-regioncorrrate-fragments-disp +    \ Add offset.
    !struct                                    \ Set the field.
;

\ Update the session regioncorrrate-fragments, deallocating the previous list.
: _session-update-regioncorrrate-fragments  ( regcr-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorrrate-list

    dup session-get-regioncorrrate-fragments    \ regcr-lst sess0 prev-lst
    -rot                                        \ prev-lst regcr-lst sess0
    _session-set-regioncorrrate-fragments       \ prev-lst
    regioncorrrate-list-deallocate
;

\ Return the session regioncorrrate-nq list.
: session-get-regioncorrrate-nq ( sess0 -- regcr-lst )
    \ Check arg.
    assert-tos-is-session

    session-regioncorrrate-nq-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the session regioncorrrate-nq for an session instance.
: _session-set-regioncorrrate-nq ( regcr-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    session-regioncorrrate-nq-disp +    \ Add offset.
    !struct                             \ Set the field.
;

: _session-update-regioncorrrate-nq ( regcr-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-regioncorrrate-nq -rot  \ prev-list regcr-lst1 sess0

    \ Set the field.
    session-regioncorrrate-nq-disp +        \ prev-list regcr-lst1 sess0+
    !struct                                 \ prev-list

    \ Deallocate previous list.
    list-deallocate
;

\ Return the session regioncorr-lol-by-rate list.
: session-get-regioncorr-lol-by-rate ( sess0 -- regcr-lol )
    \ Check arg.
    assert-tos-is-session

    session-regioncorr-lol-by-rate-disp +   \ Add offset.
    @                                       \ Fetch the field.
;

\ Set the session-regioncorr-lol-by-rate list.
: _session-set-regioncorr-lol-by-rate ( regcr-lol1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    session-regioncorr-lol-by-rate-disp +   \ Add offset.
    !struct                                 \ Set the field.
;

\ Update the session-regioncorr-lol-by-rate list.
: _session-update-regioncorr-lol-by-rate ( regcr-lol1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-regioncorr-lol-by-rate -rot    \ prev-list regcr-lst1 sess0

    \ Set the field.
    session-regioncorr-lol-by-rate-disp +
    !struct                                     \ prev-list

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

    session-pathstep-lol-by-rate-disp +     \ Add offset.
    !struct                                 \ Set the field.
;

\ Update the session-pathstep-lol-by-rate list.
: _session-update-pathstep-lol-by-rate ( pthstp-lst1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-pathstep-lol-by-rate -rot  \ prev-lst pthstp-lst1 sess0

    \ Set the field.
    session-pathstep-lol-by-rate-disp +
    !struct                                 \ prev-lst

    dup struct-dec-use-count
    pathstep-lol-deallocate
;

: _session-set-points ( u1 sess0 -- )
    \ Check args.
    assert-tos-is-session

    session-points-disp +
    !
;

: session-get-points ( sess0 -- u )
    \ Check args.
    assert-tos-is-session

    session-points-disp +
    @
;

\ Copy the current points value to the previous points value.
: session-set-previous-points ( sess0 -- )
    \ Check args.
    assert-tos-is-session

    dup session-get-points          \ sess0 pnts
    swap                            \ pnts sess0
    session-previous-points-disp +
    !
;

: session-get-previous-points ( sess0 -- u )
    \ Check args.
    assert-tos-is-session

    session-previous-points-disp +
    @
;

\ End accessors.

\ Return an regc of max domain regions.
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
        domain-get-max-region       \ sess0 reg-lst d-lisk max-reg
        #2 pick                     \ sess0 reg-lst d-lisk max-reg reg-lst
        region-list-push-end        \ sess0 reg-lst d-lisk

        link-get-next               \ sess0 reg-lst d-link
    repeat
                                    \ sess0 reg-lst
    nip                             \ reg-lst
    regioncorr-new
;

' session-calc-max-regions to session-calc-max-regions-xt

\ Create a session instance.
: session-new ( -- sess ) \ new session pushed onto session stack.

    structinfo-list-store structinfo-list-project-deallocated-xt execute

    \ cr ." session-new: start " .s cr
    \ Allocate space.
    session-mma mma-allocate        \ ses

    \ Store id.
    session-id over                 \ ses id ses
    struct-set-id                   \ ses

    \ Init use count.
    0 over struct-set-use-count     \ ses

    \ Set domains list.
    list-new                        \ ses lst
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

    \ Init points.
    0 over _session-set-points                          \ sess
    dup session-set-previous-points                     \ sess

    dup to current-session-store
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
    dup session-get-regioncorrrate-list         \ sess0 lst
    .regioncorrrate-list
    cr

    cr ." regioncorrrate fragments: "
    dup session-get-regioncorrrate-fragments    \ sess0 lst
    .regioncorrrate-list
    cr

    cr ." regioncorr-lists, excluding lower value regioncorr fragments: "

    \ Prep for loop.
    dup session-get-pathstep-lol-by-rate        \ sess0 regciplist-lst
    list-get-links                              \ sess0 regciplist-lst-link

    over session-get-regioncorr-lol-by-rate     \ sess0 regciplist-lst-link rcllist-lst
    list-get-links                              \ sess0 regciplist-lst-link rcllist-link

    #2 pick session-get-regioncorrrate-nq       \ sess0 regciplist-lst-link rcllist-link rates-le0
    list-get-links                              \ sess0 regciplist-lst-link rcllist-link rates-link

    begin
        ?dup
    while
        cr  ."    rate:      " dup link-get-data #3 dec.r
        space ." regc list:    " over link-get-data .regioncorr-list
        cr cr #15 spaces ." pathstep list: " #2 pick link-get-data
        [ ' .pathstep ] literal over list-apply
        space list-get-length dec. cr

        link-get-next rot
        link-get-next rot
        link-get-next rot
    repeat
    cr
                                        \ sess0 rullstcorrlist-link rcllist-link
    2drop                               \ sess0

    drop
;

: session-deallocate ( sess0 -- ) \ Deallocate a session.
    \ Check arg.
    assert-tos-is-session

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
    dup session-get-pathstep-lol-by-rate   \ ses ruls-lst
    pathstep-lol-deallocate                \ ses

    \ Deallocate session.
    session-mma mma-deallocate

    0 to current-session-store

    structinfo-list-store structinfo-list-project-deallocated-xt execute
;

: session-get-sample ( act2 dom1 sess0 -- sample )  \ Get a sample from an action in a domain.
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

\ Return a sample from a domain/action, given numeric id values.
: session-get-sample-by-inst-id ( act-id2 dom-id1 sess0 -- sample t | f )
    \ Check args.
    assert-tos-is-session

    swap                            \ act-id2 sess0 dom-id1
    over session-get-domains        \ act-id2 sess0 dom-id dom-lst
    domain-list-find                \ act-id2 sess0, dom t | f
    if
                                    \ act-id2 sess0 dom
        rot                         \ sess0 dom act-id2
        over domain-get-actions     \ sess0 dom act-id2 act-lst
        action-list-find            \ sess0 dom, act t | f
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

    \ Save current domain.
    dup session-get-current-domain  \ sess0 cur-dom
    swap                            \ cur-dom sess0

    list-new                        \ cur-dom sess0 sat-lst
    over session-get-domains        \ cur-dom sess0 sta-lst dom-lst

    list-get-links                  \ cur-dom sess0 sta-lst link

    begin
        ?dup
    while
        dup link-get-data           \ cur-dom sess0 sta-lst link domx

        dup #4 pick session-set-current-domain

        domain-get-current-state    \ cur-dom sess0 sta-lst link stax
        #2 pick                     \ cur-dom sess0 sta-lst link stax sta-lst
        list-push-end               \ cur-dom sess0 sta-lst link

        link-get-next               \ cur-dom sess0 sta-lst link
    repeat
                                    \ cur-dom sess0 sta-lst

    \ Restore original current domain.
    -rot                            \ sta-lst cur-dom sess0
    session-set-current-domain      \ sta-lst
;

: session-get-current-regions ( sess0 -- regcorr )  \ Return a list of regions, one for each domain state, in domain list order.
    \ Check args.
    assert-tos-is-session

    \ Save current domain.
    dup session-get-current-domain  \ sess0 cur-dom
    swap                            \ cur-dom sess0

    \ Init return list.
    list-new                        \ cur-dom sess0 sat-lst
    over session-get-domains        \ cur-dom sess0 reg-lst dom-lst

    list-get-links                  \ cur-dom sess0 reg-lst link

    begin
        ?dup
    while
        dup link-get-data           \  sess0 reg-lst link domx

        dup #4 pick session-set-current-domain

        domain-get-current-state    \ cur-dom sess0 reg-lst link stax
        dup region-new              \ cur-dom sess0 reg-lst link regx
        #2 pick                     \ cur-dom sess0 reg-lst link regx reg-lst
        list-push-end               \ cur-dom sess0 reg-lst link

        link-get-next               \ cur-dom sess0 reg-lst link
    repeat
                                    \ cur-dom sess0 reg-lst
    \ Restore original current domain.
    -rot                            \ reg-lst cur-dom sess0
    session-set-current-domain      \ reg-lst

    regioncorr-new
;

: .session-current-states ( sess0 -- )  \ Print a list of current states.
    \ Check args.
    assert-tos-is-session

    \ Save current domain.
    dup session-get-current-domain  \ sess0 cur-dom
    swap                            \ cur-dom sess0

    dup session-get-domains         \ cur-dom sess0 dom-lst
    list-get-links                  \ cur-dom sess0 d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ cur-dom sess0 d-link domx
        #2 pick                     \ cur-dom sess0 d-link domx sess0
        session-set-current-domain  \ cur-dom sess0 d-link

        dup link-get-data           \ cur-dom sess0 d-link domx
        domain-get-current-state    \ cur-dom sess0 d-link d-sta
        .value                      \ cur-dom sess0 d-link

        link-get-next               \ cur-dom sess0 d-link-nxt
        dup 0<> if
            space
        then
    repeat
                                    \ cur-dom sess0
    \ Restore original current domain.
    session-set-current-domain      \
    ." )"
;

\ Aggregate all domain needs, store in session instance field.
: session-set-all-needs ( sess0 -- )
    \ Check args.
    assert-tos-is-session

    \ Init list to start appending domain need lists to.
    list-new                            \ s0 ned-lst

    over                                \ s0 ned-lst s0
    session-get-domains                 \ s0 ned-lst dom-lst

    \ Prep for loop.
    list-get-links                      \ s0 ned-lst d-link

    \ Scan two lists to get all needs
    begin
        ?dup
    while
                                        \ s0 ned-lst d-link

        \ Get region and domain
        dup link-get-data               \ s0 ned-lst d-link | domx

        \ Set current domain
        dup                             \ s0 ned-lst d-link | domx domx
        #4 pick                         \ s0 ned-lst d-link | domx domx s0
        session-set-current-domain      \ s0 ned-lst d-link | domx

        \ Get domain needs.
        domain-get-needs                \ s0 ned-lst d-link | d-neds'

        \ Aggregate needs.
        dup                             \ s0 ned-lst d-link | d-neds' d-neds'
        #3 pick                         \ s0 ned-lst d-link | d-neds' d-neds' ned-lst
        need-list-append                \ s0 ned-lst d-link | d-neds'

        \ Clean up.
        need-list-deallocate            \ s0 ned-lst d-link

        \ Get next links.
        link-get-next                   \ s0 ned-lst d-link
    repeat
                                        \ s0 ned-lst
    swap _session-update-needs          \
;

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

\ Return the highest rate for a regc, the highest rate regclst that has a superset regc.
: session-highest-rate-regclst ( regc1 sess0 -- n )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-regioncorrrate-nq           \ regc1 sess0 rate-lst
    list-get-links                              \ regc1 sess0 rate-link
    over session-get-regioncorr-lol-by-rate     \ regc1 sess0 rate-link regclst-lst
    list-get-links                              \ regc1 sess0 rate-link regclst-link

    begin
        ?dup
    while
        #3 pick                         \ regc1 sess0 rate-link regclst-link regc1
        over link-get-data              \ regc1 sess0 rate-link regclst-link regc1 regclst
        regioncorr-list-any-superset?   \ regc1 sess0 rate-link regclst-link bool
        if                              \ regc1 sess0 rate-link regclst-link
            over link-get-data          \ regc1 sess0 rate-link regclst-link rate
            2nip                        \ regc1 regclst-link rate
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

    dup session-get-regioncorrrate-nq       \ sess0 rate-lst
    list-get-links                          \ sess0 rate-link
    over session-get-regioncorr-lol-by-rate \ sess0 rate-link regclst-lst

    list-get-links                          \ sess0 rate-link regclst-link
    cr ." Lowest  Within"
    cr ." Rate    Regions" cr
    begin
        ?dup
    while
        cr
        over link-get-data #5 dec.r
        dup link-get-data               \ sess0 rate-link regclst-link regclst
        #3 spaces .regioncorr-list      \ sess0 rate-link regclst-link
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

    begin
        ?dup
    while                                           \ ret-lst sess0 rcllist-link rates-links
       \  cr  ." rate: " dup link-get-data #3 dec.r
       \ space ." regclst: " over link-get-data .regioncorr-list cr

        list-new                                    \ ret-lst sess0 rcllist-link rates-links rip-lst
        #2 pick link-get-data                       \ ret-lst sess0 rcllist-link rates-links rip-lst regcx
        list-get-links                              \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link
        begin
            ?dup
        while                                       \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link
            dup link-get-data                       \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx
            over link-get-next                      \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt
            begin
                ?dup
            while                                           \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt
                \ cr over ."     regcx: " .regioncorr
                dup link-get-data                           \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt
                \ space dup .regioncorr
                #2 pick                                     \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regcx
                2dup
                regioncorr-intersection                     \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regcx, regc-int' t | f
                if                                          \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regcx regc-int'
                    \ Add rulecorr for one regc to intersection.
                    tuck                                    \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regc-int' regcx regc-int'
                    swap                                    \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regc-int' regc-int' regcx-nxt
                    rulecorr-new-regc-to-regc               \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regc-int' rul-lc'
                    pathstep-new

                    \ dup space .rulecorr
                    dup                                     \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regc-int' rul-lc' rul-lc'
                    #7 pick                                 \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regc-int' rul-lc' rul-lc' rip-lst
                    pathstep-list-push-nosups              \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regc-int' rul-lc' bool
                    if
                        drop
                    else
                        pathstep-deallocate
                    then

                    \ Add rulecorr for other regc.
                    tuck                                    \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int' regcx-nxt regc-int'
                    swap                                    \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int' regc-int' regcx-nxt
                    rulecorr-new-regc-to-regc               \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int' rul-lc'

                    pathstep-new                            \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int' pthstpx'
                    dup                                     \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int' pthstpx' pthstpx'

                    #6 pick                                 \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int' pthstpx' pthstpx' rip-lst
                    pathstep-list-push-nosups               \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int' pthstpx' bool
                    if
                        drop                                \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int'
                    else
                        pathstep-deallocate                 \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regc-int'
                    then

                    \ Clean up.
                    regioncorr-deallocate                   \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt
                else                                        \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt regcx-nxt regcx
                    2drop                                   \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx regcx-link-nxt
                then
                \ cr
                link-get-next
            repeat
                                                    \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link regcx
            drop                                    \ ret-lst sess0 rcllist-link rates-links rip-lst regcx-link
            \ cr
            link-get-next
        repeat
                                                    \ ret-lst sess0 rcllist-link rates-links rip-lst
\         [ ' .pathstep ] literal over list-apply
\         space ." len: " dup list-get-length dec. cr
        #4 pick                                     \ ret-lst sess0 rcllist-link rates-links rip-lst ret-lst
        list-push-end-struct                        \ ret-lst sess0 rcllist-link rates-links
        link-get-next swap
        link-get-next swap
    repeat
    \ cr
    \ Clean up.                                     \ ret-lst sess0 rcllist-link
    2drop                                           \ ret-lst
    \ cr ." session-calc-pathstep-lol: end: " .stack-gbl cr
;

\ Process the given regioncorrrates.
: session-process-regioncorrrates ( sess0 -- )
    \ Check arg.
    assert-tos-is-session

    \ cr ." session-process-regioncorrrates" cr

    \ Get given regioncorrrates.
    dup session-get-regioncorrrate-list         \ sess0 regcr-lst

    \ cr ." Given regioncorrrates:  " dup .regioncorrrate-list cr

    regioncorrrate-list-to-regioncorr-list      \ sess0 regc-lst
    dup                                         \ sess0 regc-lst regc-lst

    \ Get regc fragments of the given regioncorrrates, that are subsets of any given regioncorrrate that they intersect.
    regioncorr-list-intersection-fragments      \ sess0 regc-lst frag-lst
    \ cr ." Fragment regcs: " dup .regioncorr-list cr

    swap regioncorr-list-deallocate             \ sess0 frag-lst

    \ Init new regioncorrrate list.
    list-new                                    \ sess0 frag-lst regcr-lst
    swap                                        \ sess0 regcr-lst frag-lst
    #2 pick session-get-regioncorrrate-list     \ sess0 regcr-lst frag-lst sess-lst
    swap                                        \ sess0 regcr-lst sess-lst frag-lst

    \ For each fragment, calc its aggregate rate, form an regioncorrrate, add it to the rclrate fragment list.
    dup list-get-links                          \ sess0 regcr-lst sess-lst frag-lst frag-link
    begin
        ?dup
    while
        \ Get fragment rate.
        dup link-get-data                       \ sess0 regcr-lst sess-lst frag-lst frag-link fragx
        dup                                     \ sess0 regcr-lst sess-lst frag-lst frag-link fragx fragx
        #4 pick                                 \ sess0 regcr-lst sess-lst frag-lst frag-link fragx fragx sess-lst
        regioncorrrate-list-rate-regioncorr     \ sess0 regcr-lst sess-lst frag-lst frag-link fragx rate

        \ Build/store regioncorrrate
        regioncorrrate-new                      \ sess0 regcr-lst sess-lst frag-lst frag-link regcrx
        #4 pick                                 \ sess0 regcr-lst sess-lst frag-lst frag-link regcrx regcr-lst
        regioncorrrate-list-push                \ sess0 regcr-lst sess-lst frag-lst frag-link

        link-get-next
    repeat
    \ Clean up.
                                                \ sess0 regcr-lst sess-lst frag-lst
    regioncorr-list-deallocate                  \ sess0 regcr-lst sess-lst
    drop                                        \ sess0 regcr-lst
    2dup swap                                   \ sess0 regcr-lst regcr-lst sess0
    _session-update-regioncorrrate-fragments    \ sess0 regcr-lst

    \ Clean up.

    \ Init value list.
    list-new                                    \ sess0 regcr-lst val-lst
    0 over list-push                            \ sess0 regcr-lst val-lst

    \ Prep for loop.
    swap                                        \ sess0 val-lst regcr-lst
    list-get-links                              \ sess0 val-lst link

    begin
        ?dup
    while
        dup link-get-data                       \ sess0 val-lst link regioncorrratex
        regioncorrrate-get-rate                 \ sess0 val-lst link ratex
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
    \ Given a starting regc and a goal regc:
    \
    \    Start in the highest negative FOM that the start and goal states are in.
    \    If a plan can work within that FOM, use it.
    \    Otherwise try the next least restrictive FOM, possibly encountering higher negative regioncorrrates.
    \
    dup [ ' > ] literal swap list-sort

    \ cr ." values: " [ ' . ] literal  over .list cr

    \ Calculate regc lists for change navigation.regionlist.fs

    \ Init running subtraction list.
    over session-calc-max-regions               \ sess0 val-lst max-regc
    list-new                                    \ sess0 val-lst max-regc sub-lst
    tuck                                        \ sess0 val-lst sub-lst max-regc sub-lst
    regioncorr-list-push                        \ sess0 val-lst sub-lst

    \ Init result rcl-list.
    list-new                                    \ sess0 val-lst sub-lst rslt-lst
    2dup list-push-struct                       \ sess0 val-lst sub-lst rslt-lst
    swap                                        \ sess0 val-lst rslt-lst sub-lst
                                                \ sess0 val-lst rslt-lst sub-lst

    #2 pick list-get-links                      \ sess0 val-lst rslt-lst sub-lst link

    begin
        ?dup
    while
        \ Get fragments matching the val-list current-value.
        dup link-get-data                       \ sess0 val-lst rslt-lst sub-lst link valx

        dup 0<> if
            #5 pick                                 \ sess0 val-lst rslt-lst sub-lst link val sess0
            session-get-regioncorrrate-fragments    \ sess0 val-lst rslt-lst sub-lst link val frag-lst
            regioncorrrate-list-match-rate-negative \ sess0 val-lst rslt-lst sub-lst link regc-lst

            \ Update sub-lst.
            dup                                     \ sess0 val-lst rslt-lst sub-lst link regc-lst regc-lst
            #3 pick                                 \ sess0 val-lst rslt-lst sub-lst link regc-lst regc-lst sub-lst
            regioncorr-list-subtract                \ sess0 val-lst rslt-lst sub-lst link regc-lst sub-lst2

            swap regioncorr-list-deallocate         \ sess0 val-lst rslt-lst sub-lst link sub-lst2
            rot drop                                \ sess0 val-lst rslt-lst link sub-lst2
            swap                                    \ sess0 val-lst rslt-lst sub-lst2 link

            \ Add regc list result list.
            over                                    \ sess0 val-lst rslt-lst sub-lst2 link sub-lst2
            \ cr ." regclst: " dup .regioncorr-list cr
            #3 pick                                 \ sess0 val-lst rslt-lst sub-lst2 link sub-lst2 rslt-lst
            list-push-struct                        \ sess0 val-lst rslt-lst sub-lst2 link
        else
            drop
        then

        link-get-next
    repeat
                                                \ sess0 val-lst rslt-lst sub-lst
    drop                                        \ sess0 val-lst rslt-lst

    \ Process result list.
    #2 pick _session-update-regioncorr-lol-by-rate     \ sess0 val-lst
    dup [ ' < ] literal swap list-sort
    \ cr ." Fragment values: " [ ' . ] literal  over .list cr

    over _session-update-regioncorrrate-nq      \ sess0

    \ dup .session-pathstep-lol-by-rate           \ sess0

    dup session-calc-pathstep-lol               \ sess0 rulecorr-list lists

    swap _session-update-pathstep-lol-by-rate
;

: session-add-domain ( dom1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-domain
    \ cr ." session-add-domain: start " .stack-gbl execute cr

    \ Add domain
    2dup                                \ dom1 sess0 dom1 sess0
    session-get-domains                 \ dom1 sess0 dom1 dom-lst
    domain-list-push-end                \ dom1 sess0

    \ Set current-domain, if it is zero/invalid.
    tuck session-set-current-domain     \ sess0

    session-process-regioncorrrates     \ To get rate 0, max region regc.
;

\ Add a regioncorrrate, to give a value to some arbitrary configuration of domain regions.
: session-add-regioncorrrate ( regcr1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorrrate

    tuck session-get-regioncorrrate-list   \ sess0 regcr1 regcr-lst
    regioncorrrate-list-push               \ sess0

    session-process-regioncorrrates        \ recalc with new regioncorrrate.
;

\ Return the rate and regc list for a path to satisfy a desired regioncorr.
: session-regc-rate ( regc1 sess0 -- regc rate )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-regioncorrrate-nq       \ regc1 sess0 rt-lst
    list-get-links                          \ regc1 sess0 rt-lnk
    swap session-get-regioncorr-lol-by-rate \ regc1 rt-lnk regc-lst
    list-get-links                          \ regc1 rt-lnk regc-lnk

    begin
        ?dup
    while
        #2 pick                             \ regc1 rt-lnk regc-lnk regc1
        over link-get-data                  \ regc1 rt-lnk regc-lnk regc1 regc-lstx
        regioncorr-list-any-superset?       \ regc1 rt-lnk regc-lnk bool
        if
            link-get-data                   \ regc1 rt-lnk regcx
            swap link-get-data              \ regc1 regcx rate
            rot drop                        \ regcx rate
            exit
        then

        link-get-next swap
        link-get-next swap
    repeat

    cr ." session-regc-rate: drop-through?" cr
    2drop
    0 list-new
;

\ Return plan-list-corr (plc), a multi-domain plan, for moving domain states from one regc to another.
: session-get-plc ( regc-to regc-from sess0 -- plc t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list
    assert-nos-is-list

    list-new                        \ regc-to regc-from sess0 ret-plc
    #3 pick list-get-links          \ regc-to regc-from sess0 ret-plc to-link
    #3 pick list-get-links          \ regc-to regc-from sess0 ret-plc to-link from-link
    #3 pick session-get-domains     \ regc-to regc-from sess0 ret-plc to-link from-link dom-lst
    list-get-links                  \ regc-to regc-from sess0 ret-plc to-link from-link dom-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ regc-to regc-from sess0 ret-plc to-link from-link dom-link domx
        #5 pick                     \ regc-to regc-from sess0 ret-plc to-link from-link dom-link domx sess0
        session-set-current-domain  \ regc-to regc-from sess0 ret-plc to-link from-link dom-link

        \ Check for no-op plan.
        #2 pick link-get-data       \ regc-to regc-from sess0 ret-plc to-link from-link dom-link reg-to
        #2 pick link-get-data       \ regc-to regc-from sess0 ret-plc to-link from-link dom-link reg-to reg-from
        region-subset?              \ regc-to regc-from sess0 ret-plc to-link from-link dom-link bool
        if
            \ Make no-op plan.
            dup link-get-data       \ regc-to regc-from sess0 ret-plc to-link from-link dom-link dom
            plan-new                \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx
            #2 pick link-get-data   \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx reg-from
            dup                     \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx reg-from reg-from
            rule-new-region-to-region   \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx rul'

            0                       \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx rul' 0
            tuck                    \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx alt-rul rul' 0
            #4 pick link-get-data   \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx alt-rul rul' 0 dom
            domain-find-action      \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx alt-rul rul', act t | f
            false? abort" action zero not found?"

            planstep-new            \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx stp
            over plan-push          \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx
            #4 pick                 \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx ret-plc
            plan-list-push-end      \ regc-to regc-from sess0 ret-plc to-link from-link dom-link
        else                        \ regc-to regc-from sess0 ret-plc to-link from-link dom-link
            \ Get plan for domain.
            #2 pick link-get-data       \ regc-to regc-from sess0 ret-plc to-link from-link dom-link reg-to
            #2 pick link-get-data       \ regc-to regc-from sess0 ret-plc to-link from-link dom-link reg-to reg-from
            #2 pick link-get-data       \ regc-to regc-from sess0 ret-plc to-link from-link dom-link reg-to reg-from domx
            domain-get-plan             \ regc-to regc-from sess0 ret-plc to-link from-link dom-link, plnx t | f
            if                          \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx
                \ Store plan.
                #4 pick                 \ regc-to regc-from sess0 ret-plc to-link from-link dom-link plnx ret-plc
                plan-list-push-end      \ regc-to regc-from sess0 ret-plc to-link from-link dom-link
            else                        \ regc-to regc-from sess0 ret-plc to-link from-link dom-link
                \ Return false.
                3drop
                plan-list-deallocate    \ regc-to regc-from sess0
                3drop
                false
                exit
            then
        then

        link-get-next rot
        link-get-next rot
        link-get-next rot
    repeat

    \ Return plan.                  \ regc-to regc-from sess0 ret-plc to-link from-link
    2drop                           \ regc-to regc-from sess0 ret-plc
    2nip                            \ sess0 ret-plc
    nip                             \ ret-plc
    true
;

\ Run a plan-list-corr (plc), a multi-domain plan, to move domain states from one regc to another.
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
        false? if
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


\ Return highest rate negative number, le 0, with regc-list having interserctions with a any regioncorr
\ in a rated regioncorr list stored in the session struct.
: session-find-highest-le-zero-rate ( regc1 sess0 -- rate )
    assert-tos-is-session
    assert-nos-is-regioncorr
    \ cr ." session-find-highest-le-zero-rate: regc1: " over .regioncorr

    \ Find highest rate.
    dup session-get-regioncorr-lol-by-rate  \ | regc-lol
    list-get-links                          \ | regc-lol-link

    over session-get-regioncorrrate-nq      \ | regc-lol-link rates-lst
    list-get-links                          \ | regc-lol-link rates-link

    \ Check each rate and list.
    begin
        ?dup
    while
        #3 pick                             \ | regc-lol-link rates-link regc1
        #2 pick link-get-data               \ | regc-lol-link rates-link regc1 regc-lst
        regioncorr-list-any-intersection?   \ | regc-lol-link rates-link bool
        if
            link-get-data                   \ regc1 sess0 | regc-lol-link rate
            2nip nip                        \ rate
            \ space ." rate: " dup dec. cr
            exit
        then

        link-get-next swap
        link-get-next swap
    repeat
    cr ." session-find-highest-le-zero-rate: Drop through?"
    abort
;

\ Return a regc-list for a given rate (number, le 0).
: session-find-regioncorr-list-by-rate ( rate1 regclst0 -- regioncorr-list )
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
: session-find-pathstep-list-by-rate ( rate1 sess0 -- pthstp-list )
    \ Check args.
    assert-tos-is-session
    \ cr ." session-find-pathstep-list-by-rate: start: "

    \ Find equal rate.
    dup session-get-pathstep-lol-by-rate    \ | rate1 sess0 pthstp-lst-lst
    list-get-links                          \ | rate1 sess0 pthstp-lst-link

    over session-get-regioncorrrate-nq      \ | rate1 sess0 pthstp-lst-link rates-lst
    list-get-links                          \ | rate1 sess0 pthstp-lst-link rates-link

    \ Check each rate and list.
    begin
        ?dup
    while
        dup link-get-data                   \ | rate1 sess0 pthstp-lst-link rates-link ratex
        #4 pick                             \ | rate1 sess0 pthstp-lst-link rates-link ratex rate1
        =                                   \ | rate1 sess0 pthstp-lst-link rates-link bool
        if
            drop                            \ | rate1 sess0 pthstp-lst-link
            link-get-data                   \ | rate1 sess0 pthstp-list
            nip nip                         \ pthstp-list
            exit
        then

        link-get-next swap
        link-get-next swap
    repeat
    cr ." session-find-pathstep-list-by-rate: Drop through?"
    abort
;

: assert-5os-is-valid-depth ( 5os 4os 3os nos tos -- 5os 4os 3os nos tos )
    #4 pick 0< abort" 5OS is not a valid depth"
;

\ Calculate a path, for from/to regioncorrs, given a rated pathstep list.
\ The logic is random depth-first.
\
\ Check all possibilites for linking a pathstep, having a needed change, to
\ another pathstep, having another needed change, in a loop, until there is
\ an intersection with the to-regions.
\
\ If the current from-regions intersect a pathstep that does not provide a needed change,
\ and that step does not intersect a pathstep that has a needed change, take the
\ intersecting pathstep's result-regions as a possible next current-from, and recurse.
\
\ So, given a intersecting pathstep, without a needed change, separated from a pathstep,
\ having a needed change, there would be a recursion for each pathstep, without a needed change,
\ that is between them.
: session-calc-path-fc ( depth regc-to regc-from pthstp-lst sess0 -- regc-seq t | f )
    \ cr ." session-calc-path-fc: start: depth: " #4 pick dec. space ." from " #2 pick .regioncorr space ." to " #3 pick .regioncorr cr
    \ cr .stack-gbl cr

    \ Check args.
    assert-tos-is-session
    assert-nos-is-pathstep-list
    assert-3os-is-regioncorr
    assert-4os-is-regioncorr
    assert-5os-is-valid-depth

    \ Check depth.
    #4 pick 0=
    if
        \ cr ." depth exceeded" cr
        2drop 2drop drop
        false
        exit
    then

    \ cr ." at 0: from: " #2 pick .regioncorr space ." to: " #3 pick .regioncorr cr

    \ Init return list.
    list-new                             \ depth regc-to regc-from pthstp-lst sess0 | ret-lst

    \ Prep for loop.
    #3 pick                              \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from

    \ One RegionCorr step forward per cycle, except for recursion.
    begin
        \ cr ." begin: cur path: " over .regioncorr-list space ." cur-from: " dup .regioncorr cr

        \ Check if at end.
        #5 pick                             \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from regc-to
        over                                \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from regc-to cur-from
        regioncorr-intersects?              \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from bool
        if
            #5 pick                         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from regc-to
            regioncorr-intersection         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst, int-from t | f
            false? abort" intersection failed?"
            over regioncorr-list-push-end   \ depth regc-to regc-from pthstp-lst sess0 | ret-lst
            2nip                            \ depth regc-to sess0 | ret-lst
            2nip                            \ sess0 | ret-lst
            nip

            \ Get rid of dups that may have been added due to recursion.
            \ The cur-from from a recursion needs to be deallocated, while other cur-froms
            \ are passed, or the result regions of a pathstep, that do not need to be deallocated.
            \ This discontinuity is solved here.
            dup regioncorr-list-copy-nosups \ ret-lst ret-lst2
            swap regioncorr-list-deallocate \ ret-lst2

            true
            \ cr ." true exit 1: " .stack-gbl cr
            exit
        then

        \ Check for a pathstep that intersects regc-from and regc-to.
        #5 pick over #5 pick                \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from regc-to cur-from pthstp-lst
        pathstep-list-intersects-both       \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from, pthstpx t | f
        if
            \ cr ." regc-to and regc-from in one pathstep" cr
            \ Add regc-from intersection to the return list.
            pathstep-get-initial-regions    \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from initial
            over                            \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from initial cur-from
            over                            \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from initial cur-from initial
            regioncorr-intersection         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from initial, from-int
            false? abort" intersection failed?"
            #3 pick                         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from initial from-int ret-lst
            regioncorr-list-push-end        \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from initial

            \ Add regc-to intersection to the return list.
            #6 pick                         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from initial regc-to
            regioncorr-intersection         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from, to-int
            false? abort" intersection failed?"
            #2 pick                         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from to-int ret-lst
            regioncorr-list-push-end        \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from

            \ Clean up.
            drop
            2nip                            \ depth regc-to sess0 | ret-lst
            2nip                            \ sess0 ret-lst
            nip                             \ ret-lst

            \ Get rid of dups that may have been added due to recursion.
            \ The cur-from from a recursion needs to be deallocated, while other cur-froms
            \ are passed, or the result regions of a pathstep, that do not need to be deallocated.
            \ This discontinuity is solved here.
            dup regioncorr-list-copy-nosups \ ret-lst ret-lst2
            swap regioncorr-list-deallocate \ ret-lst2

            true
            \ cr ." true exit 2: " .stack-gbl cr
            exit
        then

        \ Get pathsteps that cur-from intersects.
        dup                             \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from cur-from
        #4 pick                         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from cur-from pthstp-lst
        pathstep-list-intersecting-fc   \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from, pthstp-lst-int' t | f
        if
        else
            \ None found, return false.
            drop
            regioncorr-list-deallocate
            2drop 2drop drop
            false
            \ cr ." false exit 3: " .stack-gbl cr
            exit
        then

        \ Check if any intersecting pathsteps have a needed change.
                                            \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int'
        #6 pick #2 pick                     \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' regc-to cur-from
        changescorr-new-regc-to-regc        \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' cngs'
        dup                                 \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' cngs' cngs'
        #2 pick                             \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' cngs' cngs' pthstp-lst-int'
        pathstep-list-has-needed-change     \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' cngs', pthstp-lst-cngs' t | f
        if
            \ Best solution, loop continues.
            swap changescorr-deallocate     \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs'
            \ cr ." take pathstep that has a needed change" cr

            \ Choose a pathstep.
            dup list-get-length             \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' len
            random                          \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' inx
            over list-remove-item-struct    \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pathstpx
            \ cr ." pathstep used 1: " dup .pathstep cr

            \ Clean up.
            swap pathstep-list-deallocate   \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' patshpx
            swap pathstep-list-deallocate   \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from patshpx

            \ Save cur-from.
            swap                            \ depth regc-to regc-from pthstp-lst sess0 | ret-lst patshpx cur-from
            over                            \ depth regc-to regc-from pthstp-lst sess0 | ret-lst patshpx cur-from pthstpx
            pathstep-get-initial-regions    \ depth regc-to regc-from pthstp-lst sess0 | ret-lst patshpx cur-from stp-initial
            regioncorr-intersection         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst patshpx, cur-from-int t | f
            false? abort" intersection failed?"
            #2 pick                         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst patshpx cur-from-int ret-lst
            regioncorr-list-push-end        \ depth regc-to regc-from pthstp-lst sess0 | ret-lst patshpx

            \ Get next cur-from.
            dup pathstep-get-result-regions \ depth regc-to regc-from pthstp-lst sess0 | ret-lst patshpx nxt-from
            nip                             \ depth regc-to regc-from pthstp-lst sess0 | ret-lst nxt-from
        else
            \ cr ." Check if any pathsteps intersecting cur-from have results that intersect pathsteps that have needed changes" cr
                                            \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' cngs'
            \ Find any pathsteps that have a needed change.
            dup                             \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' cngs' cngs'
            #6 pick                         \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' cngs' cngs' pthstp-lst
            pathstep-list-has-needed-change \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' cngs', pthstp-lst-cngs' t | f
            if
                swap changescorr-deallocate     \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs'

                \ cr .stack-gbl
                \ cr ." intersecting pthstps: " over .pathstep-list space ." pthstps w/needed cng: " dup .pathstep-list cr

                \ Init list of pathstep-intersecting-results, that intersect at least one pathstep with needed changes.
                list-new                        \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst'

                \ For each pathstep cur-from intersects with, pair with pathsteps having a needed change.
                #2 pick list-get-links          \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link
                begin
                    ?dup
                while
                    \ Get cur-from intersecting pathstep result regions.
                    dup link-get-data           \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-stp
                    pathstep-get-result-regions \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt

                    \ Compare one cur-from intersecting pathstep results with pathsteps having at least one needed change.
                    #2 pick                     \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt pthstp-lst-cngs'
                    list-get-links              \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt cng-link

                    begin
                        ?dup
                    while
                        dup link-get-data               \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt cng-link cng-pthstpx
                        pathstep-get-initial-regions    \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt cng-link cng-initial
                        #2 pick                         \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt cng-link cng-initial int-rslt
                        \ cr ." comparing " over .regioncorr space ." and " dup .regioncorr cr
                        regioncorr-intersection         \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt cng-link, cng-int t | f
                        if
                            #4 pick                     \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt cng-link cng-int pairing-lst'
                            list-push-struct            \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt cng-link
                        then

                        link-get-next
                    repeat
                                                        \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link int-rslt
                    drop                                \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs' pairing-lst' int-link

                    link-get-next
                repeat
                                                \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs2' pairing-lst'
                dup list-is-empty?
                if
                    \ Third solution, loop is paused to recurse.
                    \ cr ." pairing list is empty" cr
                    list-deallocate                     \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs2'
                    pathstep-list-deallocate            \ | ret-lst cur-from pthstp-lst-int'

                    \ Get closest to regc-to possibilities.
                    #6 pick                                 \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int' regc-to
                    over                                    \ | ret-lst cur-from pthstp-lst-int' regc-to pthstp-lst-int'
                    pathstep-list-closest-result-regions    \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cls'
                    swap pathstep-list-deallocate           \ | ret-lst cur-from pthstp-lst-cls'

                    \ Select one, cur-from intersecting, closest to regc-to, pathstep.
                    dup list-get-length                 \ | ret-lst cur-from pthstp-lst-cls' len
                    random                              \ | ret-lst cur-from pthstp-lst-cls' inx
                    over                                \ | ret-lst cur-from pthstp-lst-cls' inx pthstp-lst-int'
                    list-remove-item-struct             \ | ret-lst cur-from pthstp-lst-cls' pthstpx
                    swap                                \ | ret-lst cur-from pthstpx pthstp-lst-cls'
                    pathstep-list-deallocate            \ | ret-lst cur-from pthstpx
                    \ cr ." pathstep used 2: " dup .pathstep cr

                    \ Step cur-from into result list.
                    dup pathstep-get-initial-regions    \ | ret-lst cur-from pthstpx stp-init
                    #2 pick                             \ | ret-lst cur-from pthstpx stp-init cur-from
                    regioncorr-intersection             \ | ret-lst cur-from pthstpx, cur-from' t | f
                    false? abort" intersection did not work?"
                    #3 pick                             \ | ret-lst cur-from pthstpx cur-from' ret-lst
                    list-push-end-struct                \ | ret-lst cur-from pthstpx

                    \ Get next cur-from, from the pathstep result regions.
                    pathstep-get-result-regions         \ | ret-lst cur-from nxt-from
                    nip                                 \ | ret-lst nxt-from

                    \ Prep for recursion.
                                                \ depth regc-to regc-from pthstp-lst sess0 | ret-lst nxt-from
                    #6 pick 1 - swap            \ depth regc-to regc-from pthstp-lst sess0 | ret-lst depth nxt-from
                    #6 pick swap                \ depth regc-to regc-from pthstp-lst sess0 | ret-lst depth regc-to nxt-from
                    #5 pick                     \ depth regc-to regc-from pthstp-lst sess0 | ret-lst depth regc-to nxt-from pthstp-lst
                    #5 pick                     \ depth regc-to regc-from pthstp-lst sess0 | ret-lst depth regc-to nxt-from pthstp-lst sess0
                    recurse                     \ depth regc-to regc-from pthstp-lst sess0 | ret-lst, ret-lst2 t | f
                    if
                        \ cr ." recursion worked" dup .regioncorr-list cr
                        \ cr ." cur path: " over .regioncorr-list cr
                        \ Add partial path to current path.
                        2dup swap regioncorr-list-append
                        regioncorr-list-deallocate
                        \ cr ." appended: " dup .regioncorr-list cr

                        \ Get last item in list for new cur-from.
                        \ The cur-from from a recursion needs to be deallocated, while other cur-froms
                        \ are passed, or the result regions of a pathstep, that do not need to be deallocated.
                        \ Taking an intersection of cur-from and a pathstep's initial regions,
                        \ the result being equal or a subset of cur-from, then storing the
                        \ intersection in the return list, causes the original cur-from to be undeallocated.
                        \ So keep the new cur-from in the list here, get-last-item instead of pop-end.
                        dup list-get-last-item  \ depth regc-to regc-from pthstp-lst sess0 | ret-lst, cur-from
                    else
                        \ cr ." recursion did NOT work" cr
                        regioncorr-list-deallocate
                        2drop 2drop drop
                        false
                        \ cr ." false exit 4: " .stack-gbl cr
                        exit
                    then
                else
                    \ Second best solution, loop continues.
                    \ cr ." pairing list is not empty" cr
                    cr dup .regioncorr-list cr
                    \ Choose one.
                    dup list-get-length             \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs2' pair-lst' len
                    random                          \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs2' pair-lst' inx
                    over                            \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs2' pair-lst' inx pair-lst'
                    list-remove-item-struct         \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs2' pair-lst' next-from
                    \ cr ." next-from chosen: " dup .regioncorr cr
                    \ Clean up.
                    swap regioncorr-list-deallocate \ | ret-lst cur-from pthstp-lst-int' pthstp-lst-cngs2' next-from
                    swap pathstep-list-deallocate   \ | ret-lst cur-from pthstp-lst-int' next-from
                    swap pathstep-list-deallocate   \ | ret-lst cur-from next-from
                    swap                            \ | ret-lst next-from cur-from

                    \ Store cur-from.
                    #2 pick                         \ | ret-lst next-from cur-from ret-lst
                    regioncorr-list-push-end        \ | ret-lst next-from
                then
            else
                changescorr-deallocate      \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from pthstp-lst-int'
                pathstep-list-deallocate    \ depth regc-to regc-from pthstp-lst sess0 | ret-lst cur-from
                drop                        \ depth regc-to regc-from pthstp-lst sess0 | ret-lst
                regioncorr-list-deallocate  \ depth regc-to regc-from pthstp-lst sess0
                2drop 2drop drop
                false
                \ cr ." false exit 5: " .stack-gbl cr
                exit
            then
        then

    again
;

\ Return a pathstep-list for changing state from a regioncorr to a goal regioncorr.
\ Early steps will change a regioncorr to an intersection with another regioncorr,
\ until the last regioncorr intersects the goal.
: session-calc-path ( regc-to regc-from sess0 -- pthstp-lst t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr
    \ cr ." session-calc-path: start: regc-from: " over .regioncorr space ." to: " #2 pick .regioncorr cr
    \ cr .stack-gbl cr

    #2 pick #2 pick regioncorr-intersects?
    abort" session-calc-plan: from/to intersect?"

    \ Get the lowest rate from reg-to and reg-from highest rates.
    #2 pick over                                \ regc-to regc-from sess0 | regc-to sess0
    session-find-highest-le-zero-rate           \ regc-to regc-from sess0 | rate-to

    #2 pick #2 pick                             \ regc-to regc-from sess0 | rate-to regc-from sess0
    session-find-highest-le-zero-rate           \ regc-to regc-from sess0 | rate-to rate-from

    min                                         \ regc-to regc-from sess0 | rate-min
    \ cr ." path rate: " dup dec. cr

    \ Get pathstep-list for rate.
    over                                        \ regc-to regc-from sess0 | rate-min sess0
    session-find-pathstep-list-by-rate          \ regc-to regc-from sess0 | pthstp-lst

    \ Get reg-to, restricted if needed.
    #3 pick over pathstep-list-intersection     \ regc-to regc-from sess0 | pthstp-lst, regc-to' t | f
    false? abort" intersection failed?"

    \ Get reg-from, restrictid if needed.
    #3 pick #2 pick pathstep-list-intersection  \ regc-to regc-from sess0 | pthstp-lst regc-to', regc-from' t | f
    false? abort" intersection failed?"

    #3                                          \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from' depth
    #2 pick                                     \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from' depth regc-to'
    #2 pick                                     \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from' depth regc-to regc-from'
    #5 pick                                     \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from' depth regc-to regc-from pthstp-lst
    #7 pick                                     \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from' depth regc-to regc-from pthstp-lst sess0

    \ Try forward chaining.
    session-calc-path-fc                        \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from', pthstp-lst2 t | f
    if                                          \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from' pthstp-lst2
        swap regioncorr-deallocate
        swap regioncorr-deallocate              \ regc-to regc-from sess0 | pthstp-lst pthstp-lst2
        2nip                                    \ regc-to | pthstp-lst pthstp-lst2
        nip nip                                 \ pthstp-lst2
        true
        exit
    then

                                                \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from'
    \ Try backward chaining.
\   #3                                          \ regc-to regc-from sess0 | pthstp-lst depth
\   #4 pick                                     \ regc-to regc-from sess0 | pthstp-lst depth regc-to
\   #4 pick                                     \ regc-to regc-from sess0 | pthstp-lst depth regc-to regc-from
\   #3 pick                                     \ regc-to regc-from sess0 | pthstp-lst depth regc-to regc-from, pthstp-lst2 t | f
\   session-calc-path-bc                        \ regc-to regc-from sess0 | pthstp-lst, pthstp-lst2 t | f
\   if
\       2nip                                    \ regc-to pthstp-lst pthstp-lst2
\       nip nip                                 \ pthstp-lst
\       true
\       exit
\   then

                                                \ regc-to regc-from sess0 | pthstp-lst regc-to' regc-from'
    regioncorr-deallocate                       \ regc-to regc-from sess0 | pthstp-lst regc-to'
    regioncorr-deallocate                       \ regc-to regc-from sess0 | pthstp-lst
    2drop                                       \ regc-to regc-from
    2drop
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

    #2 pick #2 pick regioncorr-intersects?
    if
        cr
        ." session-calc-plancorr: from/to intersect? "
        space ." from: " over .regioncorr
        space ." to: " #2 pick .regioncorr
        cr
    then

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
        \ cr ." at top while: " .stack-gbl cr
        \ Set current domain.
        dup link-get-data                   \ plnc-lst sess0 link-to link-from link-dom domx
        #4 pick                             \ plnc-lst sess0 link-to link-from link-dom domx sess0
        session-set-current-domain          \ plnc-lst sess0 link-to link-from link-dom
        \ cr ." after set domain: " .stack-gbl cr

        \ Get plan
        #2 pick link-get-data               \ plnc-lst sess0 link-to link-from link-dom reg-to
        #2 pick link-get-data               \ plnc-lst sess0 link-to link-from link-dom reg-to reg-from
        #2 pick link-get-data               \ plnc-lst sess0 link-to link-from link-dom reg-to reg-from domx
        \ cr ." before get plan: " .stack-gbl cr
        domain-get-plan                     \ plnc-lst sess0 link-to link-from link-dom, plnx t | f
        \  cr ." after get plan: " .stack-gbl cr
        if                                  \ plnc-lst sess0 link-to link-from link-dom plnx
            \ Add domain-plan to plan-list.
            #5 pick                         \ plnc-lst sess0 link-to link-from link-dom plnx plnc-lst
            \ cr ." at xx: " .stack-gbl cr
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

\ Return a plancorr list, given a regioncorr list.
: session-calc-plnclst-from-regc-lst ( regc-lst regc-to regc-from sess0 - -plnc-lst t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr
    assert-4os-is-regioncorr-list
    \ cr ." session-calc-plnclst-from-pthstplst: start: regc-from: " over .regioncorr space ." to: " #2 pick .regioncorr cr
    \ cr ." regc list: " #3 pick .regioncorr-list cr

    \ Init return list.
    2>r                         \ regc-lst regc-to, r: regc-from sess0
    list-new -rot               \ plnc-lst regc-lst regc-to, r: regc-from sess0
    2r>                         \ plnc-lst regc-lst regc-to regc-from sess0

    \ Promote regc-from, so its easier to replace.
    swap                        \ plnc-lst regc-lst regc-to sess0 regc-from

    \ In the loop, regc-from is temporary, so protect the passed regc-from from deallocation.
    regioncorr-copy                     \ plnc-lst regc-lst regc-to sess0 regc-from'

    \ Prep for loop.
    #3 pick list-get-links              \ plnc-lst regc-lst regc-to sess0 regc-from' regc-link
    \ ? check list first item matches regc-from?
    link-get-next

    begin
        ?dup
    while
        \ Get next pathstep intersection.
        dup link-get-data               \ plnc-lst regc-lst regc-to sess0 regc-from regc-link pthstpx

        \ Check if its the end.
        #4 pick over                    \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx regc-to regcx
        regioncorr-intersects?          \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx bool
        if                              \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx
            \ The end, get plancorr for regc-from to regc-to.
            #4 pick #3 pick #5 pick     \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx regc-to regc-from sess0
            \ cr ." get plan1 from: " over .regioncorr space ." to: " #2 pick .regioncorr
            session-calc-plancorr       \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx, plancorr t | f
            if
                \ space ." found: " dup .plancorr
                \ cr ." at 1: " .stack-gbl cr
                \ #5 pick                 \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-to
                \ #4 pick                 \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-to regc-from
                \ regioncorr-union        \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-limit'
                \ dup                     \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-limit' regc-limit'
                \ #2 pick                 \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-limit' regc-limit' plancorr
                \ plancorr-within-regc?   \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-limit' bool
                \ if
                \     space ." within: " dup .regioncorr space ." true" cr
                \ else
                \     space ." within: " dup .regioncorr space ." false" cr
                \ then
                \ regioncorr-deallocate

                #7 pick                 \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr plnc-lst
                plancorr-list-push-end  \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx
                2drop                   \ plnc-lst regc-lst regc-to sess0 regc-from
                regioncorr-deallocate   \ plnc-lst regc-lst regc-to sess0
                3drop                   \ plnc-lst
                true
                exit
            else                        \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx
                \ space ." not found" cr
                2drop                   \ plnc-lst regc-lst regc-to sess0 regc-from
                regioncorr-deallocate   \ plnc-lst regc-lst regc-to sess0
                3drop                   \ plnc-lst
                plancorr-list-deallocate
                false
                exit
            then
        else                            \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx
            \ Not the end, get plancorr for regc-from to pathstepx result-regions.
            #2 pick                     \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx regc-from
            #4 pick                     \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx regc-from sess0
            \ cr ." get plan2 from: " over .regioncorr space ." to: " #2 pick .regioncorr
            session-calc-plancorr       \ plnc-lst regc-lst regc-to sess0 regc-from regc-link, plancorr t | f
            if                          \ plnc-lst regc-lst regc-to sess0 regc-from regc-link plancorr
                \ space ." found: " dup .plancorr
                \ over link-get-data      \ plnc-lst regc-lst regc-to sess0 regc-from regc-link plancorr regcx
                \ #3 pick                 \ plnc-lst regc-lst regc-to sess0 regc-from regc-link plancorr regcx regc-from
                \ regioncorr-union        \ plnc-lst regc-lst regc-to sess0 regc-from regc-link plancorr regc-limit'
                \ dup                     \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-limit' regc-limit'
                \ #2 pick                 \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-limit' regc-limit' plancorr
                \ plancorr-within-regc?   \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regcx plancorr regc-limit' bool
                \ if
                \     space ." within: " dup .regioncorr space ." true" cr
                \ else
                \     space ." within: " dup .regioncorr space ." false" cr
                \ then
                \ regioncorr-deallocate

                \ Calc new regc-from
                dup plancorr-calc-result-regions    \ plnc-lst regc-lst regc-to sess0 regc-from regc-link plancorr regc-from'
                swap                                \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regc-from' plancorr

                \ Add plancorr to plancorr-list.
                #7 pick                 \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regc-from' plancorr plnc-lst
                plancorr-list-push-end  \ plnc-lst regc-lst regc-to sess0 regc-from regc-link regc-from'

                \ Replace previous regc-from.
                rot regioncorr-deallocate   \ plnc-lst regc-lst regc-to sess0 regc-link regc-from'
                swap                        \ plnc-lst regc-lst regc-to sess0 regc-from' regc-link

            else                        \ plnc-lst regc-lst regc-to sess0 regc-from regc-link
                space ." not found" cr
                drop                    \ plnc-lst regc-lst regc-to sess0 regc-from
                \ cr ." at deall 4: " cr
                regioncorr-deallocate   \ plnc-lst regc-lst regc-to sess0
                3drop                   \ plnc-lst
                plancorr-list-deallocate
                false
                exit
            then
        then

        link-get-next
    repeat
                                \ plnc-lst regc-lst regc-to sess0 regc-from
    regioncorr-deallocate       \ plnc-lst regc-lst regc-to sess0
    3drop                       \ plnc-lst
    true
;

\ Return a target regioncorr for a need.
\ That is, a regioncorr, with all regions equal to the domain maximum region,
\ except the need domain which will be the need target.
: session-regioncorr-for-need ( need1 sess0 -- regcorr )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-need

    \ Init region list, corresponding to domains.
    list-new                        \ need1 sess0 | reg-lst
    #2 pick need-get-domain         \ need1 sess0 | reg-lst ned-dom
    #2 pick session-get-domains     \ need1 sess0 | reg-lst ned-dom dom-lst

    \ Prep for loop.
    list-get-links                  \ need1 sess0 | reg-lst ned-dom dom-link

    \ For each domain.
    begin
        ?dup
    while
        dup link-get-data           \ need1 sess0 | reg-lst ned-dom dom-link domx
        #2 pick                     \ need1 sess0 | reg-lst ned-dom dom-link domx ned-dom
        =                           \ need1 sess0 | reg-lst ned-dom dom-link bool
        if
            #4 pick need-get-target \ need1 sess0 | reg-lst ned-dom dom-link ned-targ-sta
            dup region-new          \ need1 sess0 | reg-lst ned-dom dom-link regx
        else
            dup link-get-data       \ need1 sess0 | reg-lst ned-dom dom-link domx
            domain-get-max-region   \ need1 sess0 | reg-lst ned-dom dom-link regx
        then
        #3 pick                     \ need1 sess0 | reg-lst ned-dom dom-link regx reg-lst
        region-list-push-end        \ need1 sess0 | reg-lst ned-dom dom-link

        link-get-next               \ need1 sess0 | reg-lst ned-dom dom-link
    repeat
                                    \ need1 sess0 | reg-lst ned-dom
    \ Clean up.
    drop                            \ need1 sess0 | reg-lst
    nip nip                         \ reg-lst

    \ Return.
    regioncorr-new                  \ regcorr
;

\ Return a plan to change the current states to intersect a given regioncorr.
\ Avoid negative regioncorrs, if possible.
: session-change-to-plans ( regc-to sess0 -- plnc-lst t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-regioncorr
    \ cr ." session-change-to-plans: " over .regioncorr cr

    tuck                                            \ sess0 regc-to sses0

    session-get-current-regions                     \ sess0 regc-to regc-from'

    2dup                                            \ sess0 regc-to regc-from' regc-to regc-from'
    #4 pick                                         \ sess0 regc-to regc-from' regc-to regc-from' sess0

    session-calc-path                               \ sess0 regc-to regc-from', regc-lst' t | f
    if
        \ cr ." path found: " dup .regioncorr-list cr
        dup                                         \ sess0 regc-to regc-from' regc-lst' regc-lst'
        #3 pick                                     \ sess0 regc-to regc-from' regc-lst' regc-lst' regc-to
        #3 pick                                     \ sess0 regc-to regc-from' regc-lst' regc-lst' regc-to regc-from'
        #6 pick                                     \ sess0 regc-to regc-from' regc-lst' regc-lst' regc-to regc-from' sess0
        session-calc-plnclst-from-regc-lst          \ sess0 regc-to regc-from' regc-lst', plnc-lst t | f
        if
            \ cr ." session-change-to-plans: plans found: " dup .plancorr-list
            swap regioncorr-list-deallocate         \ sess0 regc-to regc-from' plnc-lst
            swap regioncorr-deallocate              \ sess0 regc-to plnc-lst
            nip nip                                 \ plnc-lst
            true
            exit
        else
            \ cr ." session-change-to-plans: plans not found"
        then
        regioncorr-list-deallocate                  \ sess0 regc-to regc-from'
    else
       \ cr ." no path found " cr
    then
                                                    \ sess0 regc-to regc-from'
    regioncorr-deallocate                           \ sess0 regc-to
    2drop
    false
;

: session-get-current-rate ( sess0 -- rate ) \ Return the rate of current domain states, based on being subset of regioncorrrates.
    \ Check arg.
    assert-tos-is-session

    \ Save current domain.
    dup session-get-current-domain                  \ sess0 cur-dom
    swap                                            \ cur-dom sess0

    dup session-get-current-regions                 \ cur-dom sess cur-regc
    dup                                             \ cur-dom sess cur-regc cur-regc
    #2 pick session-get-regioncorrrate-list         \ cur-dom sess cur-regc cur-regc regcr-lst
    regioncorrrate-list-rate-regioncorr             \ cur-dom sess regcr-lst rate
    swap regioncorr-deallocate                      \ cur-dom sess rate

    \ Restore current domain.
    -rot                                            \ rate cur-dom sess
    session-set-current-domain                      \ rate
;

\ Get current domain states rate, add to points value.
: session-update-points ( sess0 -- )
    \ Check args.
    assert-tos-is-session

    dup session-get-current-rate    \ sess rt
    dup rate-get-positive           \ sess rt pos
    swap                            \ sess pos rt
    dup rate-get-negative           \ sess pos rt neg
    swap rate-deallocate            \ sess pos neg
    +                               \ sess net
    swap                            \ net sess

    dup session-get-points          \ net sess0 u
    rot +                           \ sess0 new-pts
    swap                            \ new-pts sess0
    _session-set-points
;

' session-update-points to session-update-points-xt

\ Return the numebr of domains.
: session-get-number-domains ( sess0 -- u )
    \ Check arg.
    assert-tos-is-session

    session-get-domains
    list-get-length
;

' session-get-number-domains to session-get-number-domains-xt

\ Do a need.  Return true if the need has been satisfied.
: session-do-need ( ned1 sess0 -- bool )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-need

    over need-get-action            \ ned1 sess0 act
    #2 pick need-get-domain         \ ned1 sess0 act dom

    \ Set cur domain.
    dup                             \ ned1 sess0 act dom dom
    #3 pick                         \ ned1 sess0 act dom dom sess0
    session-set-current-domain      \ ned1 sess0 act dom

    \ See if a plan is needed.
    dup domain-get-current-state    \ ned1 sess0 act dom d-sta
    #4 pick need-get-target         \ ned1 sess0 act dom d-sta n-sta
    =                               \ ned1 sess0 act dom flag
    if
        \ No plan needed, get sample.
        2dup                        \ ned1 sess0 act dom act dom
        domain-get-sample           \ ned1 sess0 act dom sample
        sample-deallocate           \ ned1 sess0 act dom
        domain-get-inst-id
        cr ." Dom: " dec.           \ ned1 sess0 act
        .action cr                  \ ned1 sess0
        2drop                       \
        true
        exit
    then
                                    \ ned1 sess0 act dom
    #3 pick need-get-target         \ ned1 sess0 act dom t-sta
    over domain-get-current-state   \ ned1 sess0 act dom t-sta c-state
    sample-new                      \ ned1 sess0 act dom smpl'

    \ Create from/to regions.
    dup sample-get-result           \ ned1 sess0 act dom smpl' rslt
    dup region-new                  \ ned1 sess0 act dom smpl' reg-to'
    over sample-get-initial         \ ned1 sess0 act dom smpl' reg-to' initial
    dup region-new                  \ ned1 sess0 act dom smpl' reg-to' reg-from'
    2dup                            \ ned1 sess0 act dom smpl' reg-to' reg-from' reg-to' reg-from'
    #5 pick                         \ ned1 sess0 act dom smpl' reg-to' reg-from' reg-to' reg-from' dom

    domain-get-plan                 \ ned1 sess0 act dom smpl' reg-to' reg-from', plan' t | f
    if
        swap region-deallocate
        swap region-deallocate      \ ned1 sess0 act dom smpl' plan'
        dup plan-run                \ ned1 sess0 act dom smpl' plan' flag
        if
            cr ." plan succeeded " cr
                                    \ ned1 sess0 act dom smpl' plan'
            #3 pick                 \ ned1 sess0 act dom smpl' plan' act
            #3 pick                 \ ned1 sess0 act dom smpl' plan' act dom
            domain-get-sample       \ ned1 sess0 act dom smpl' plan' smpl'
            sample-deallocate       \ ned1 sess0 act dom smpl' plan'
            plan-deallocate         \ ned1 sess0 act dom smpl'
            sample-deallocate       \ ned1 sess0 act dom
            2drop 2drop
            true
            exit
        then
                                    \ ned1 sess0 act dom smpl' plan
        cr ." plan failed " cr
        plan-deallocate             \ ned1 sess0 act dom smpl'
        sample-deallocate           \ ned1 sess0 act dom
        2drop 2drop
        false
        exit
    then

    cr ." No plan found" cr
    region-deallocate               \ ned1 sess0 act dom smpl' reg-to'
    region-deallocate               \ ned1 sess0 act dom smpl'
    sample-deallocate               \ ned1 sess0 act dom
    2drop 2drop
    false
;

: session-behavior ( sess0 -- )  \ Change to the closest more-positive regioncorr fragment.

    dup session-get-current-rate                        \ sess0 rate'

    dup rate-is-negative                                \ sess0 rate' bool
    if
        cr ." current states are negative, seek closest non-negative states" cr
        rate-deallocate                                 \ sess0

        \ Get current states as regions.
        dup session-get-current-regions                 \ sess0 cur-regc'

        \ Get rate LE 0 regioncorr list.
        over session-get-regioncorr-lol-by-rate         \ sess0 cur-regc' regc-lol
        list-get-links                                  \ sess0 cur-regc' link-first-regc-lst
        link-get-data                                   \ sess0 cur-regc' regc-lst

        \ Get closest regcs.
        over swap                                       \ sess0 cur-regc' cur-regc' regc-lst
        regioncorr-list-closest-regioncorrs             \ sess0 cur-regc' clst-regc-lst'

        \ Clean up.
        swap regioncorr-deallocate                      \ sess0 clst-regc-lst'

        \ Choose a regioncorr.
        dup list-get-length                             \ sess0 clst-regc-lst' len
        random                                          \ sess0 clst-regc-lst' inx
        over list-get-item                              \ sess0 clst-regc-lst' regc

        \ Try to change path.
        #2 pick                                         \ sess0 clst-regc-lst' regc sess0
        session-change-to-plans                         \ sess0 clst-regc-lst', planc-lst' t | f
        if
            cr ." Plan found: " dup .plancorr-list cr
            dup plancorr-list-run-plans                 \ sess0 clst-regc-lst' planc-lst' bool
            if
                cr ." Plan suceeded" cr
            else
                cr ." Plan failed" cr
            then
            plancorr-list-deallocate                    \ sess0 clst-lst'
        else
            cr ." No plan found" cr
            over session-update-points
        then

        regioncorr-list-deallocate                      \ sess0
    else                                                \ sess0 rate'
        cr ." current states are not negative, seek closest more-positive regioncorr fragment" cr
        \ rate-deallocate                               \ sess0

        \ Find more-positive regioncorr fragments.
        dup                                             \ sess0 rate' rate'
        #2 pick session-get-regioncorrrate-fragments    \ sess0 rate' rate' frg-lst
        regioncorrrate-list-more-positive-regioncorrs   \ sess0 rate' mrp-lst'
        swap rate-deallocate                            \ sess0 mrp-lst'

        \ Check for no more-positive items found.
        dup list-is-empty?                              \ sess0 mrp-lst'
        if
            cr ." There are no more-positive regioncorr fragments" cr
            list-deallocate                             \ sess0
            session-update-points
            exit
        then

        \ Convert regioncorrrate list to regioncorr-list.
        dup regioncorrrate-list-to-regioncorr-list      \ sess0 mrp-lst' mrp-lst''
        swap regioncorrrate-list-deallocate             \ sess0 mrp-lst''

        \ Find closest regioncorr fragments of more positive regioncorr fragments.
        over session-get-current-regions dup            \ sess0 mrp-lst'' cur-regc' cur-regc'
        #2 pick                                         \ sess0 mrp-lst'' cur-regc' cur-regc' mrp-lst''
        regioncorr-list-closest-regioncorrs             \ sess0 mrp-lst'' cur-regc' cls-lst'
        swap regioncorr-deallocate                      \ sess0 mrp-lst'' cls-lst'
        swap regioncorr-list-deallocate                 \ sess0 cls-lst'

        \ Select one.
        dup list-get-length random                      \ sess0 cls-lst' inx
        over list-get-item                              \ sess0 cls-lst' regcx

         \ Try to change path.
        #2 pick                                         \ sess0 clst-lst' regcx sess
        session-change-to-plans                         \ sess0 clst-lst', planc-lst' t | f
        if
            cr ." Plan found: " dup .plancorr-list cr
            dup plancorr-list-run-plans                 \ sess0 clst-lst' planc-lst' bool
            if
                cr ." Plan suceeded" cr
            else
                cr ." Plan failed" cr
            then
            plancorr-list-deallocate                    \ sess0 clst-lst'
        else
            cr ." No plan found" cr
            over session-update-points
        then
        regioncorr-list-deallocate                      \ sess0
    then
                                                        \ sess0
    drop
;

\ Return a plan for a need.
: session-get-plan-for-need ( ned1 sess0 -- pln t | f )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-need

    \ Make need domain the current domain.
    over need-get-domain        \ ned1 sess0 dom
    tuck swap                   \ ned1 dom dom sess
    session-set-current-domain  \ ned1 dom

    domain-get-plan-for-need    \ pln t | f
;

\ Return a sample for a need.
: session-get-sample-for-need ( ned1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-need

    \ Make need domain the current domain.
    over need-get-domain        \ ned1 sess0 dom
    tuck swap                   \ ned1 dom dom sess
    session-set-current-domain  \ ned1 dom

    \ Check at target.
    over need-get-target        \ ned1 dom n-sta
    over                        \ ned1 dom n-sta dom
    domain-get-current-state    \ ned1 dom n-sta d-sta
    <> abort" current state does not match need"

    \ Get sample.
                                \ ned1 dom0
    over                        \ ned1 dom0 ned1
    need-get-action             \ ned1 dom0 act
    over                        \ ned1 dom0 act dom0
    domain-get-sample           \ ned1 dom0 smpl
    sample-deallocate           \ ned1 dom0

    \ Print info.
    cr ." Dom: "
    domain-get-inst-id #3 dec.r \ ned1
    need-get-action             \ act
    .action
;

\ Process a list of needs.
\ Return true if any need was processed,
\ false if no need matched the domain current state and/or
\ no plan was was found for any need.
: session-process-need-list ( ned-lst1 sess0 -- bool )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-need-list

    \ Init index list for need list.
    over list-get-length                        \ ned-lst1 sess0 len
    value-list-0-to-n                           \ ned-lst1 sess0 inx-lst'

    begin
        dup list-get-length                     \ ned-lst1 sess0 inx-lst' len
        random                                  \ ned-lst1 sess0 inx-lst' rnd-inx
        dup                                     \ ned-lst1 sess0 inx-lst' rnd-inx rnd-inx
        #2 pick                                 \ ned-lst1 sess0 inx-lst' rnd-inx rnd-inx inx-lst'
        list-get-item                           \ ned-lst1 sess0 inx-lst' rnd-inx ned-inx

        #4 pick list-get-item                   \ ned-lst1 sess0 inx-lst' rnd-inx nedx
        cr ." Need chosen: " space dup .need

        \ Check if no plan needed.
        dup need-get-domain                     \ ned-lst1 sess0 inx-lst' rnd-inx nedx dom
        #4 pick                                 \ ned-lst1 sess0 inx-lst' rnd-inx nedx dom sess0
        session-set-current-domain              \ ned-lst1 sess0 inx-lst' rnd-inx nedx
        dup need-get-domain                     \ ned-lst1 sess0 inx-lst' rnd-inx nedx dom
        domain-get-current-state                \ ned-lst1 sess0 inx-lst' rnd-inx nedx d-sta
        swap tuck                               \ ned-lst1 sess0 inx-lst' rnd-inx nedx d-sta nedx
        need-current-state-satisfies            \ ned-lst1 sess0 inx-lst' rnd-inx nedx bool
        if
            \ No plan needed.
            cr
            #3 pick                             \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0
            session-get-sample-for-need         \ ned-lst1 sess0 inx-lst' rnd-inx

            \ Need satisfied, done.
            drop
            list-deallocate
            2drop
            true
            exit
        else                                    \ ned-lst1 sess0 inx-lst' rnd-inx nedx
            \ Plan needed.

            \ Check if safer plan is needed, that is, a plan unlikely to pass through
            \ more negative regions that the current regions and goal regions are in..
            #3 pick                             \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0

            2dup                                \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 nedx sess0
            session-regioncorr-for-need         \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 to-regc'
            \ cr ." to regcorr: " dup .regioncorr cr

            over                                \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' sess0
            session-get-current-regions         \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from'
            \ cr ." from regcorr: " dup .regioncorr cr

            \ Get the lowest rate from reg-to and reg-from highest rates.
            over #3 pick                        \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' regc-to' sess
            session-find-highest-le-zero-rate   \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' rate-to

            over #4 pick                        \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' rate-to regc-from' sess0
            session-find-highest-le-zero-rate   \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' rate-to rate-from

            min                                 \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' rate-min
            \ cr ." plan rate: " dup dec. cr

            #3 pick                             \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' rate-min sess
            session-get-regioncorrrate-nq       \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' rate-min nq-lst
            list-get-last-item                  \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' rate-min lowest-rate
            <> if
                \ Rate is better than the lowest, so try safe planning.
                over #3 pick                    \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' regc-to' sess
                session-change-to-plans         \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from', plnc' t | f
                if
                    cr cr ." plan found: " dup .plancorr-list cr
                    dup plancorr-list-run-plans \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' plnc' bool
                    swap                        \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' bool plnc'
                    plancorr-list-deallocate    \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from' bool
                    swap regioncorr-deallocate  \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' bool
                    swap regioncorr-deallocate  \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 bool
                    nip                         \ ned-lst1 sess0 inx-lst' rnd-inx nedx bool

                    if
                        cr ." plan suceeded" cr
                        #3 pick                     \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0
                        session-get-sample-for-need \ ned-lst1 sess0 inx-lst' rnd-inx
                        drop                        \ ned-lst1 sess0 inx-lst'
                        list-deallocate             \ ned-lst1 sess0
                        2drop
                        true
                        exit
                    else
                        cr ." plan failed" cr
                        2drop                       \ ned-lst1 sess0 inx-lst'
                        list-deallocate             \ ned-lst1 sess0
                        2drop
                        true
                        exit
                    then
                else
                    \ cr ." session-change-to-plans: not found" cr
                                                \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from'
                    regioncorr-deallocate       \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to'
                    regioncorr-deallocate       \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0
                    drop                        \ ned-lst1 sess0 inx-lst' rnd-inx nedx
                then
            else
                \ Rate is lowest already.
                \ Clean up.                 \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to' regc-from'
                regioncorr-deallocate       \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0 regc-to'
                regioncorr-deallocate       \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0
                drop                        \ ned-lst1 sess0 inx-lst' rnd-inx nedx
            then

            \ Try unsafe plan.
            dup                             \ ned-lst1 sess0 inx-lst' rnd-inx nedx nedx
            #4 pick                         \ ned-lst1 sess0 inx-lst' rnd-inx nedx nedx sess0
            session-get-plan-for-need       \ ned-lst1 sess0 inx-lst' rnd-inx nedx, pln t | f
            if                              \ ned-lst1 sess0 inx-lst' rnd-inx nedx pln'
                cr cr ." plan found: " dup .plan cr
                dup plan-run                \ ned-lst1 sess0 inx-lst' rnd-inx nedx pln' bool
                if
                    cr ." plan suceeded" cr
                    plan-deallocate             \ ned-lst1 sess0 inx-lst' rnd-inx nedx
                    \ TODO check if need requires final sample?
                    #3 pick                     \ ned-lst1 sess0 inx-lst' rnd-inx nedx sess0
                    session-get-sample-for-need \ ned-lst1 sess0 inx-lst' rnd-inx
                    drop                        \ ned-lst1 sess0 inx-lst'
                    list-deallocate             \ ned-lst1 sess0
                    2drop
                    true
                    exit
                else
                    cr ." plan failed" cr
                    plan-deallocate             \ ned-lst1 sess0 inx-lst' rnd-inx nedx
                    2drop                       \ ned-lst1 sess0 inx-lst'
                    list-deallocate             \ ned-lst1 sess0
                    2drop
                    true
                    exit
                then
            else                            \ ned-lst1 sess0 inx-lst' rnd-inx nedx
                ." , no plan found."
                drop                        \ ned-lst1 sess0 inx-lst' rnd-inx
            then
        then

        \ Need not satisfied, try another, if any.
                                        \ ned-lst1 sess0 inx-lst' rnd-inx

        \ Remove the index, avoiding random picking the same need to try again.
        over                            \ ned-lst1 sess0 inx-lst' rnd-inx inx-lst'
        list-remove-item                \ ned-lst1 sess0 inx-lst' u
        drop                            \ ned-lst1 sess0 inx-lst'

        \ Check if index list is empty.
        dup list-get-length 0=
    until
                                        \ ned-lst1 sess0 inx-lst'
    list-deallocate                     \ ned-lst1 sess0
    2drop
    false
;

\ Automatic behaviours when the user presses the Enter key with no input.
: session-do-zero-token-command ( sess0 -- true ) \ Zero-token logic, get/show/act-on needs.
    \ Check arg.
    assert-tos-is-session

    dup session-get-needs           \ sess0 ned-lst

    dup list-get-length             \ sess0 ned-lst len
    0=
    if
        drop                        \ sess0
        session-behavior            \
        true
        exit
    then

    \ Check for Corner anchor square needs.
    need-type-cas over              \ sess0 ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ sess0 ned-lst, ned-lst' t | f
    if
        dup                         \ sess0 ned-lst ned-lst' ned-lst'
        #3 pick                     \ sess0 ned-lst ned-lst' ned-lst' sess0
        session-process-need-list   \ sess0 ned-lst ned-lst' bool
        swap need-list-deallocate   \ sess0 ned-lst bool
        if
            2drop
            true
            exit
        then
    then

    \ Check for Corner dissimilar square needs.
    need-type-cds over              \ sess0 ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ sess0 ned-lst, ned-lst' t | f
    if
        dup                         \ sess0 ned-lst ned-lst' ned-lst'
        #3 pick                     \ sess0 ned-lst ned-lst' ned-lst' sess0
        session-process-need-list   \ sess0 ned-lst ned-lst' bool
        swap need-list-deallocate   \ sess0 ned-lst bool
        if
            2drop
            true
            exit
        then
    then

    \ Check for Confirm Logical structure needs.
    need-type-cls over              \ sess0 ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ sess0 ned-lst, ned-lst' t | f
    if
        dup                         \ sess0 ned-lst ned-lst' ned-lst'
        #3 pick                     \ sess0 ned-lst ned-lst' ned-lst' sess0
        session-process-need-list   \ sess0 ned-lst ned-lst' bool
        swap need-list-deallocate   \ sess0 ned-lst bool
        if
            2drop
            true
            exit
        then
    then

    \ Check for Improve Logical Structure needs.
    need-type-ils over              \ sess0 ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ sess0 ned-lst, ned-lst' t | f
    if
        dup                         \ sess0 ned-lst ned-lst' ned-lst'
        #3 pick                     \ sess0 ned-lst ned-lst' ned-lst' sess0
        session-process-need-list   \ ned-lst ned-lst' bool
        swap need-list-deallocate   \ ned-lst bool
        if
            2drop
            true
            exit
        then
    then

    \ Check for State not in group.
    need-type-snig over             \ sess0 ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ sess0 ned-lst, ned-lst' t | f
    if
        dup                         \ sess0 ned-lst ned-lst' ned-lst'
        #3 pick                     \ sess0 ned-lst ned-lst' ned-lst' sess0
        session-process-need-list   \ sess0 ned-lst ned-lst' bool
        swap need-list-deallocate   \ sess0 ned-lst bool
        if
            2drop
            true
            exit
        then
    then

    \ Check for Confirm group.
    need-type-cg over               \ sess0 ned-lst ned-typ ned-lst
    need-list-find-all-match-type   \ sess0 ned-lst, ned-lst' t | f
    if
        dup                         \ sess0 ned-lst ned-lst' ned-lst'
        #3 pick                     \ sess0 ned-lst ned-lst' ned-lst' sess0
        session-process-need-list   \ sess0 ned-lst ned-lst' bool
        swap need-list-deallocate   \ sess0 ned-lst bool
        if
            2drop
            true
            exit
        then
    then

    \ Return.
    2drop
    true
;

: session-do-dn-command ( tkn-lst1 sess0 -- )   \ Do the "dn" command. Do a need, given it's number in the displayed list.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length #2 <>              \ tkn-lst1 sess0 bool
    if
        cr ." dn command has an invald number of arguments" cr
        2drop
        exit
    then

    over list-get-second-item               \ tkn-lst1 sess0 tkn1
    token-get-string                        \ tkn-lst1 sess0 c-addr u
    snumber?                                \ tkn-lst1 sess0, n t | f
    if                                      \ tkn-lst1 sess0 n
        cr ." You entered number " dup . cr

        \ Check lower bound.
        dup 0 <                             \ tkn-lst1 sess0 n flag
        if
            cr ." Number entered is out of range" cr
            2drop drop
            exit
        then                                \ tkn-lst1 sess0 n

        \ Check higher bound.
        over                                \ tkn-lst1 sess0 n sess0
        session-get-needs                   \ tkn-lst1 sess0 n ned-lst
        dup list-get-length                 \ tkn-lst1 sess0 n ned-lst ned-len
        #2 pick                             \ tkn-lst1 sess0 n ned-lst ned-len n
        swap                                \ tkn-lst1 sess0 n ned-lst n ned-len
        >=
        if                                  \ tkn-lst1 sess0 n ned-lst flag
            cr ." Number entered is out of range" cr
            2drop 2drop
            exit
        then                                \ tkn-lst1 sess0 n ned-lst

        \ Get selected need.
        list-get-item                       \ tkn-lst1 sess0 ned
        cr ." You chose need: " dup .need cr
        swap                                \ tkn-lst1 ned sess0
        session-do-need                     \ tkn-lst1 bool
        2drop
    else                                    \ tkn-lst1 sess0
        cr ." dn command argument not a number" cr
        2drop
    then
;

: session-do-to-command ( tkn-lst1 sess0 -- )  \ Do the "to" command. Change state to a given regioncorr.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length                        \ tkn-lst1 sess0 len
    over                                        \ tkn-lst0 sess0 len sess
    session-get-number-domains 1 +              \ tkn-lst0 sess0 len num-dom+
    <> if
        cr ." to command has an invalid number of arguments" cr
        2drop
        exit
    then

    \ Get goal regioncorr.
    over list-copy-after-first-struct           \ tkn-lst0 sess0 tkn-lst2'
    dup regioncorr-from-token-list              \ tkn-lst0 sess0 tkn-lst2' regc-to' t | f
    false? if
        cr ." to command arguments did not convert to a regioncorr" cr
        token-list-deallocate                   \ tkn-lst0 sess0
        2drop
        exit
    then
    swap token-list-deallocate                   \ tkn-lst0 sess0 regc-to'

    \ Get current regioncorr.
    over session-get-current-regions            \ tkn-lst0 sess0 regc-to regc-from'

    \ Check if the current states are already at the goal.
    2dup swap                                   \ tkn-lst0 sess0 regc-to' regc-from' regc-from' regc-to
    regioncorr-superset?                        \ tkn-lst0 sess0 regc-to' regc-from' bool
    if
        cr ." The current states are already at goal." cr
        regioncorr-deallocate
        regioncorr-deallocate
        2drop
        exit
    then

    cr ." from: " dup .regioncorr space ." to: " over .regioncorr cr
    regioncorr-deallocate                       \ tkn-lst0 sess0 regc-to'
    dup                                         \ tkn-lst0 sess0 regc-to' regc-to'
    #2 pick                                     \ tkn-lst0 sess0 regc-to' regc-to' sess0
    session-change-to-plans                     \ tkn-lst0 sess0 regc-to' plnc-lst' t | f
    if
        swap regioncorr-deallocate              \ tkn-lst0 sess0 plnc-lst'

        cr ." Plan found: " dup .plancorr-list cr
        dup plancorr-list-run-plans             \ tkn-lst0 sess0 planc-lst' bool
        if
            cr ." Plan suceeded" cr
        else
            cr ." Plan failed" cr
        then
        plancorr-list-deallocate                \ tkn-lst0 sess0
        2drop
    else
        cr ." No plan found" cr
        regioncorr-deallocate                   \ tkn-lst0 sess0
        2drop
    then
;

: session-do-pd-command ( tkn-lst1 sess0 -- ) \ Do the "pd" command. Print a Domain.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length                        \ tkn-lst1 sess0 len
    #2 <> if
        cr ." pd command: invalid number of arguments" cr
        2drop
        exit
    then

    over list-get-second-item                   \ tkn-lst0 sess0 tkn1
    token-get-string                            \ tkn-lst0 sess0 c-addr c-cnt

    \ Get domain.
    snumber?                                    \ tkn-lst0 sess0, n t | f
    if
        over                                    \ tkn-lst0 sess0  n sess0
        session-find-domain                     \ tkn-lst0 sess0, dom t | f
        if
            \ Set current domain.
            tuck swap                           \ tkn-lst1 dom dom sess0
            session-set-current-domain          \ tkn-lst1 dom
            .domain                             \ tnk-lst1
            drop
        else
            cr ." pd command: domain id value invalid" cr
            2drop
        then
    else
        cr ." pd command: domain id not a number" cr
        2drop
    then
;

: session-do-cds-command ( tkn-lst1 sess0 -- )    \ Do the "cds" command. Change Domain State command.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length                        \ tkn-lst1 sess0 len
    #3 <> if
        cr ." cds command: invalid number of arguments" cr
        2drop
        exit
    then

    \ Get domain.
    over list-get-second-item                   \ tkn-lst1 sess0 tkn1
    token-get-string                            \ tkn-lst1 sess0 c-addr u
    snumber?                                    \ tkn-lst1 sess0, n t | f
    if                                          \ tkn-lst1 sess0 n
        over session-find-domain                \ tkn-lst0 sess0, dom t | f
        if                                      \ tkn-lst0 sess0 dom
            \ Set current domain.
            dup #2 pick                         \ tkn-lst0 sess0 dom dom sess0
            session-set-current-domain          \ tkn-lst0 sess0 dom
        else
            cr ." cds command: domain id invalid value" cr
            2drop
            exit
        then
    else
        cr ." cds command: domain id not a number" cr
        2drop
        exit
    then

    \ Get state.
    #2 pick list-get-third-item                 \ tkn-lst0 sess0 dom tkn2
    token-get-string                            \ tkn-lst0 sess0 dom c-addr u
    snumber?                                    \ tkn-lst0 sess0 dom, num t | f
    if                                          \ tkn-lst0 sess0 dom num
        dup is-value?                           \ tkn-lst0 sess0 dom num bool
        if
            cr ." state " dup . cr
            swap                                \ tkn-lst0 sess0 sta dom
            domain-set-current-state            \ tkn-lst0 sess0
            2drop
        else
            cr ." cds command: state invalid value" cr
            2drop 2drop
        then
    else
        cr ." cds command: state not a number" cr
        drop 2drop
    then
;

: session-do-psd-command ( tkn-lst1 sess0 -- ) \ Do the "pds" command. Print square detail for a domain's action.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length                        \ tkn-lst1 sess0 len
    #3 <> if
        cr ." psd command: invalid number of arguments" cr
        2drop
        exit
    then

    \ Get domain.
    over list-get-second-item                   \ tkn-lst1 sess0 tkn1
    token-get-string                            \ tkn-lst1 sess0 c-addr-u
    snumber?                                    \ tkn-lst1 sess0, dom-id t | f
    if
        \ cr ." domain " dup . cr
        over session-find-domain                \ tkn-lst1 sess0, dom t | f
        if                                      \ tkn-lst1 sess0 dom
            \ Set current domain.
            dup #2 pick                         \ tkn-lst1 sess0 dom dom sess0
            session-set-current-domain          \ tkn-lst1 sess0 dom
        else
            cr ." psd command: domain id value invalid" cr
            2drop
            exit
        then
    else
        cr ." psd command: domain id not a number" cr
        2drop
        exit
    then

    \ Get action.
    #2 pick list-get-third-item                 \ tkn-lst1 sess0 dom tkn2
    token-get-string                            \ tkn-lst1 sess0 dom c-addr u
    snumber?                                    \ tkn-lst1 sess0 dom, act-id t | f
    if                                          \ tkn-lst1 sess0 dom act-id
        over                                    \ tkn-lst1 sess0 dom act-id dom
        domain-find-action                      \ tkn-lst1 sess0 dom, act t | f
        if
            tuck swap                           \ tkn-lst1 sess0 act act dom
            domain-set-current-action           \ tkn-lst1 sess0 act
            action-get-squares                  \ tkn-lst1 sess0 sqr-lst
            cr .square-list cr                  \ tkn-lst1 sess0
            2drop
            exit
        else
            cr ." psd command: action id value invalid" cr
            drop 2drop
        then
    else
        cr ." psd command: action id not a number" cr
        2drop
    then
;

: session-do-tos-command ( tkn-lst1 sess0 -- ) \ Do the "tos" command. Change a domain to a state.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length                            \ tkn-lst1 sess0 len
    #3 <> if
        cr ." tos command: invalid number of arguments" cr
        2drop
        exit
    then

    \ Get domain.
    over list-get-second-item                       \ tkn-lst1 sess0 tkn1
    token-get-string                                \ tkn-lst1 sess0 c-addr u
    snumber?                                        \ tkn-lst1 sess0, num t | f
    if
        over session-find-domain                    \ tkn-lst1 sess0, dom t | f
        if                                          \ tkn-lst1 sess0 dom
            \ Set current domain.
            2dup swap                               \ tkn-lst1 sess0 dom dom sess0
            session-set-current-domain              \ tkn-lst1 sess0 dom
        else
            cr ." tos command: domain id value invalid" cr
            2drop
            exit
        then
    else
        cr ." tos command: domain id not a number" cr
        2drop
        exit
    then

    \ Get state.
    #2 pick list-get-third-item                 \ tkn-lst1 sess0 dom tkn1
    token-get-string                            \ tkn-lst1 sess0 dom c-addr u
    snumber?                                    \ tkn-lst1 sess0 dom, to-sta t | f
    if
        dup is-value?                           \ tkn-lst1 sess0 dom to-sta bool
        if
            swap                                \ tkn-lst1 sess0 to-sta dom
            dup domain-get-current-state        \ tkn-lst1 sess0 to-sta dom cur-sta
            rot swap                            \ tkn-lst1 sess0 dom to-sta cur-sta
            2dup =
            if                                  \ tkn-lst1 sess0 dom to-sta cur-sta
                cr ." Already at that state."
                2drop 2drop drop
                exit
            then

            \ Do domain-get-plan
            dup region-new swap                 \ tkn-lst1 sess0 dom cur-reg' to-sta
            dup region-new swap                 \ tkn-lst1 sess0 dom to-reg' cur-reg'

            rot                                 \ tkn-lst1 sess0 to-reg' cur-reg' dom
            #2 pick swap                        \ tkn-lst1 sess0 to-reg' cur-reg' to-reg' dom
            #2 pick swap                        \ tkn-lst1 sess0 to-reg' cur-reg' to-reg' cur-reg' dom
            domain-get-plan                     \ tkn-lst1 sess0 to-reg' cur-reg', plan' t | f
            if                                  \ tkn-lst1 sess0 to-reg' cur-reg' plan'
                swap region-deallocate          \ tkn-lst1 sess0 to-reg' plan'
                swap region-deallocate          \ tkn-lst1 sess0 plan'
                dup                             \ tkn-lst1 sess0 plan' plan'
                plan-run                        \ tkn-lst1 sess0 plan' flag
                swap plan-deallocate            \ tkn-lst1 sess0 flag
                if
                    cr ." Plan succeeded" cr
                else
                    cr ." Plan failed" cr
                then
                2drop
            else                                \ tkn-lst1 sess0 to-reg cur-reg
                region-deallocate               \ tkn-lst1 sess0 to-reg
                region-deallocate               \ tkn-lst1 sess0
                2drop
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

: session-do-scs-command ( tkn-lst1 sess0 -- ) \ Do the "scs" command.  Sample the current state of a domain.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length                        \ tkn-lst1 sess0 len
    #3 <> if
        cr ." scs command: invalid number of arguments" cr
        2drop
        exit
    then

    \ Get domain.
    over list-get-second-item                   \ tkn-lst1 sess0 tkn1
    token-get-string                            \ tkn-lst1 sess0 c-addr u
    snumber?                                    \ tkn-lst1 sess0, dom-id t | f
    if
        \ cr ." domain " dup . cr
        over session-find-domain                \ tkn-lst1 sess0, dom t | f
        if
            \ Set current domain.
            2dup swap                           \ tkn-lst1 sess0 dom dom sess0
             session-set-current-domain         \ tkn-lst1 sess0 dom
        else
            cr ." scs command: domain id value invalid" cr
            2drop
            exit
        then
    else
        cr ." scs command: domain id not a number" cr
        2drop
        exit
    then

    \ Get action.
    #2 pick list-get-third-item                 \ tkn-lst1 sess0 dom tkn2
    token-get-string                            \ tkn-lst1 sess0 dom c-addr u
    snumber?                                    \ tkn-lst1 sess0 dom, act-id t | f
    if                                          \ tkn-lst1 sess0 dom act-id
        \ cr ." action " dup . cr
        over domain-find-action                 \ tkn-lst1 sess0 dom, act t | f
        if
            swap 2dup                           \ tkn-lst1 sess0 act dom act dom
            domain-set-current-action           \ tkn-lst1 sess0 act dom
            dup domain-get-current-state        \ tkn-lst1 sess0 act dom cur-sta
            rot                                 \ tkn-lst1 sess0 dom cur-sta act
            action-get-sample                   \ tkn-lst1 sess0 dom smpl'
            dup sample-get-result               \ tkn-lst1 sess0 dom smpl' smpl-r
            rot                                 \ tkn-lst1 sess0 smpl' smpl-r dom
            domain-set-current-state            \ tkn-lst1 sess0 smpl'
            sample-deallocate                   \ tkn-lst1 sess0
            2drop
        else
            cr ." scs command: action id value invalid" cr
            drop 2drop
        then
    else
        cr ." scs command: action id not a number" cr
        drop 2drop
    then
;

: session-do-pa-command ( tkn-lst1 sess0 -- ) \ Do "pa" command. Print a domain's action.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length                            \ tkn-lst1 sess0 len
    #3 <> if
        cr ." pa command: invalid number of arguments" cr
        2drop
        exit
    then

    \ Get domain.
    over list-get-second-item                       \ tkn-lst1 sess0 tkn1
    token-get-string                                \ tkn-lst1 sess0 c-addr u
    snumber?                                        \ tkn-lst1 sess0, u t | f
    if                                              \ tkn-lst1 sess0 u
        over session-find-domain                    \ tkn-lst1 sess0, dom t | f
        if
            \ Set current domain.
            2dup swap                               \ tkn-lst1 sess0 dom dom sess0
            session-set-current-domain              \ tkn-lst1 sess0 dom
        else
            cr ." pa command: domain id value invalid" cr
            2drop
            exit
        then
    else
        cr ." pa command: domain id not a number" cr
        2drop
        exit
    then
                                                    \ tkn-lst1 sess0 dom

    \ Get action.
    #2 pick list-get-third-item                     \ tkn-lst1 sess0 dom tkn2
    token-get-string                                \ tkn-lst1 sess0 dom c-addr u
    snumber?                                        \ tkn-lst1 sess0 dom, act-id t | f
    if                                              \ tkn-lst1 sess0 dom act-id
        over domain-find-action                     \ tkn-lst1 sess0 dom, act t | f
        if                                          \ tkn-lst1 sess0 dom act
            tuck swap                               \ tkn-lst1 sess0 act act dom
            domain-set-current-action               \ tkn-lst1 sess0 act
            .action                                 \ tkn-lst1 sess0
            2drop
        else                                        \ dom
            cr ." pa command: action id value invalid" cr
            drop 2drop
        then
    else                                            \ dom
        cr ." pa command: action id not a number" cr
        drop 2drop
    then
;

: session-do-sas-command ( tkn-lst1 sess0 -- ) \ Do the "sas" command. Take an arbitrary domain action for a given state.
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    over list-get-length                            \ tkn-lst1 sess0 len
    #4 <> if
        cr ." sas command: invalid number of arguments" cr
        2drop
        exit
    then

    \ Get domain.
    over list-get-second-item                       \ tkn-lst1 sess0 tkn1
    token-get-string                                \ tkn-lst1 sess0 c-addr u
    snumber?                                        \ tkn-lst1 sess0, dom-id t | f
    if
        over session-find-domain                    \ tkn-lst1 sess0, dom t | f
        if
            \ Set current domain.
            2dup swap                               \ tkn-lst1 sess0 dom dom sess0
            session-set-current-domain              \ tkn-lst1 sess0 dom
        else
            cr ." sas command: domain id value invalid" cr
            drop 2drop
            exit
        then
    else
        cr ." sas command: domain id not a number" cr
        2drop
        exit
    then

    \ Get action.
    #2 pick list-get-third-item                     \ tkn-lst1 sess0 dom tkn2
    token-get-string                                \ tkn-lst1 sess0 dom c-addr u
    snumber?                                        \ tkn-lst1 sess0 dom, act-id t | f
    if
        over domain-find-action                     \ tkn-lst1 sess0 dom, act t | f
        if
            swap 2dup                               \ tkn-lst1 sess0 act dom act dom
            domain-set-current-action               \ tkn-lst1 sess0 act dom
        else
            cr ." sas command: action id value invalid" cr
            drop 2drop
            exit
        then
    else
        cr ." sas command: action id not a number" cr
        drop 2drop
        exit
    then                                            \ tkn-lst1 sess0 sta-str act dom

    \ Get state.
    #3 pick list-get-fourth-item                    \ tkn-lst1 sess0 act dom tkn2
    token-get-string                                \ tkn-lst1 sess0 act dom c-addr u
    snumber?                                        \ tkn-lst1 sess0 act dom, sta t | f
    if                                              \ tkn-lst1 sess0 act dom sta
        dup is-not-value?                           \ tkn-lst1 sess0 act dom sta flag
        if
            cr ." sas command: state value invalid"
            2drop 2drop drop
            exit
        then

        2dup swap                                   \ tkn-lst1 sess0 act dom sta sta dom
        domain-set-current-state                    \ tkn-lst1 sess0 act dom sta
        rot                                         \ tkn-lst1 sess0 dom sta act
        action-get-sample                           \ tkn-lst1 sess0 dom smpl'
        dup sample-get-result                       \ tkn-lst1 sess0 dom smpl' smpl-r
        rot                                         \ tkn-lst1 sess0 smpl' smpl-r dom
        domain-set-current-state                    \ tkn-lst1 sess0 smpl'
        sample-deallocate                           \ tnk-lst0 sess0
        2drop
    else                                            \ tkn-lst1 sess0 act dom
        cr ." sas command: state not a number"
        2drop 2drop
    then
;

\ Do commands from user input.
\ Return true if the read-eval loop should continue.
: session-eval-user-input ( tkn-lst1 sess0 -- bool )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-token-list

    \ Check for no tokens
    over list-is-empty?                 \ tkn-lst1 sess0 bool
    if
        nip                             \ sess0
        session-do-zero-token-command   \ bool
        exit
    then

    \ Check command.
    over list-get-first-item            \ tkn-lst1 sess0 tkn0

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" ps" str=                         \ tkn-lst1 sess0 tkn0 bool
    if
        drop                            \ tkn-lst1 sess0
        swap list-get-length            \ sess0 len
        1 =                             \ sess0 bool
        if                              \ sess0
            \ Print Session.
            .session                    \
        else                            \ sess0
            drop                        \
            cr ." ps command: invalid number of arguments" cr
        then
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" pa" str=                         \ tkn-lst1 sess0 tkn0 bool
    if
        \ Print a domain's action.
        drop                            \ tkn-lst1 sess0
        session-do-pa-command           \
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" to" str=                         \ tkn-lst1 sess0 tkn0 bool
    if
        \ Go to a given regiorcorr.
        drop                            \ tkn-lst sess0
        session-do-to-command           \
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" q" str=                          \ tkn-lst1 sess0 tkn0 bool
    if
        2drop                           \ tkn-lst1
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

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" mu" str=                         \ tkn-lst1 sess0 tkn0 bool
    if
        2drop                           \ tkn-lst1
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

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" dn" str=                         \ tkn-lst1 sess0 tkn0 bool
    if
        \ Do a specific need number from the displayed list.
        drop                            \ tkn-lst1 sess0
        session-do-dn-command           \
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" pd" str=                         \ tkn-lst1 sess0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst1 sess0
        session-do-pd-command           \
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" cds" str=                        \ tkn-lst1 sess0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst1 sess0
        session-do-cds-command          \
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" psd" str=                        \ tkn-lst1 sess0 tkn0 bool
    if
        \ Print square detail for a given domain, action.
        drop                            \ tkn-lst1 sess0
        session-do-psd-command          \
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" tos" str=                        \ tkn-lst1 sess0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst1 sess0
        session-do-tos-command          \
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" scs" str=                        \ tkn-lst1 sess0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst1 sess0
        session-do-scs-command          \
        true
        exit
    then

    dup token-get-string                \ tkn-lst1 sess0 tkn0 c-addr u
    s" sas" str=                        \ tkn-lst1 sess0 tkn0 bool
    if
        \ Print a domain, given a domain ID.
        drop                            \ tkn-lst1 sess0
        session-do-sas-command          \
        true
        exit
    then

    cr ." Did not understand the command" cr
    drop 2drop                          \
    true
;

\ Get input of up to TOS characters from user, using the PAD area, up to a given number of characters.
\ Evaluate the input.
\ like: 80 s" Enter command: > " get-user-input
\
\ If this aborts, various things can be done:
\
\ Print all domains, and actions.
\   current-session-gbl  .session
\
\ Print Domain 1.
\    1  current-session-gbl  session-find-domain  drop  .domain
\
\ Print Domain 1, Act 4.
\    1  current-session-gbl  session-find-domain  drop  4  swap  domain-find-action  drop  .action
\
\ Print the squares of domain 1 action 4.
\    1  current-session-gbl  session-find-domain  drop  4  swap  domain-find-action  drop  action-get-squares  .square-list
\
\ Return a bool for continuing the REP loop.
\ Return false if the user enterd the q (quit) command, else true.
: session-get-user-input ( sess0 -- bool )
    \ Check arg.
    assert-tos-is-session

    \ Display needs.
    dup session-set-all-needs   \ sess0
    dup session-get-needs       \ sess0 ned-lst
    dup list-get-length         \ sess0 ned-lst len
    dup 0=
    if
        cr ." Needs: No needs found" cr
        2drop
    else
        drop
        cr ." Needs:" cr .need-list cr  \ sess0
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
    s" Enter command: > "       \ sess0 c-addr c
    type                        \ sess0
    \ Get chars, leaves num chars on TOS.
    pad                         \ sess0 p-addr
    dup                         \ sess0 p-addr p-addr
    #80                         \ sess0 p-addr p-addr #80
    accept                      \ sess0 pad-add n
    cr
    token-list-from-string      \ sess0 tkn-lst'
    2dup swap                   \ sess0 tkn-lst' tkn-lst' sess0
    session-eval-user-input     \ sess0 tkn-lst' bool
    swap token-list-deallocate  \ sess0 bool
    nip                         \ bool
;
