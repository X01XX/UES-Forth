\ Implement a Session struct and functions.                                                                                             

#31319 constant session-id
    #9 constant session-struct-number-cells

\ Struct fields
0 constant session-header       \ 16-bits [0] struct id [1] use count
session-header                  cell+ constant session-domains-disp                 \ A domain-list
session-domains-disp            cell+ constant session-current-domain-disp          \ A domain, or zero before first domain is added.
session-current-domain-disp     cell+ constant session-needs-disp                   \ A need-list.
session-needs-disp              cell+ constant session-rlcrate-list-disp            \ Base region-list-corr + rate, list.
session-rlcrate-list-disp       cell+ constant session-rlcrate-fragments-disp       \ Fragments of rlcrate-list.
session-rlcrate-fragments-disp  cell+ constant session-rlcrate-le0-rates-disp       \ A list of of numbers, starting at zero, then rlcrate negative rates, in descending order.
session-rlcrate-le0-rates-disp  cell+ constant session-rlclist-by-rate-disp         \ rlc lists, corresponding to le0-rates, where a plan can move within without
                                                                                    \ encountering a lower rated rlc.
                                                                                    \ Within an rlclist, GT one rlc, there are intersections, so there is a path
                                                                                    \ from one rlc to another through an intersection.
session-rlclist-by-rate-disp    cell+ constant session-rules-by-rate-disp           \ List of rule-list-corr, representing an rlc translating to a smaller, imbedded, intersection
                                                                                    \ with another rlc.

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
    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    session-id =    
;

\ Check TOS for session, unconventional, leaves stack unchanged. 
: assert-tos-is-session ( arg0 -- arg0 )
    dup is-allocated-session
    is-false if
        s" TOS is not an allocated session"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the domain-list from an session instance.
: session-get-domains ( ses0 -- lst )
    \ Check arg.
    assert-tos-is-session

    session-domains-disp +  \ Add offset.
    @                       \ Fetch the field.
;

\ Set the domain-list for an session instance.
: _session-set-domains ( lst ses0 -- )
    \ Check arg.
    assert-tos-is-session
    assert-nos-is-list

    session-domains-disp +  \ Add offset.
    !                       \ Set the field.
;

\ Return the current domain from an session instance.
: session-get-current-domain ( ses0 -- dom )
    \ Check arg.
    assert-tos-is-session

    session-current-domain-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the current domain for an session instance.
: session-set-current-domain ( dom ses0 -- )
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
: _session-set-needs ( ned-lst ses0 -- )
    \ Check args.
    assert-tos-is-session
    over 0<> if
        assert-nos-is-list
        over struct-inc-use-count
    then

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
    dup 0=
    if
        drop
    else
        need-list-deallocate
    then
;

\ Return the session need-list
: session-get-rlcrate-list ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-rlcrate-list-disp + \ Add offset.
    @                           \ Fetch the field.
;

\ Set the need-list for an session instance.
: _session-set-rlcrate-list ( rlcrt-lst1 ses0 -- )
    \ Check args.
    assert-tos-is-session
    over 0<> if
        assert-nos-is-list
        over struct-inc-use-count
    then

    session-rlcrate-list-disp + \ Add offset.
    !                           \ Set the field.
;

\ Return the session need-list
: session-get-rlcrate-fragments ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-rlcrate-fragments-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the need-list for an session instance.
: _session-set-rlcrate-fragments ( rlcrt-lst1 ses0 -- )
    \ Check args.
    assert-tos-is-session
    over 0<> if
        assert-nos-is-list
        over struct-inc-use-count
    then

    session-rlcrate-fragments-disp +    \ Add offset.
    !                                   \ Set the field.
;

\ Return the session rlcrate-le0-rates list.
: session-get-rlcrate-le0-rates ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-rlcrate-le0-rates-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the session rlcrate-le0-rates for an session instance.
: _session-set-rlcrate-le0-rates ( rlcrt-lst1 ses0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over struct-inc-use-count

    session-rlcrate-le0-rates-disp +    \ Add offset.
    !                                   \ Set the field.
;

: _session-update-rlcrate-le0-rates ( rlcrt-lst1 ses0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-rlcrate-le0-rates
    list-deallocate

    over struct-inc-use-count

    session-rlcrate-le0-rates-disp +    \ Add offset.
    !                                   \ Set the field.
;

\ Return the session rlclist-by-rates list.
: session-get-rlclist-by-rate ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-rlclist-by-rate-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the session-rlclist-by-rates for an session instance.
: _session-set-rlclist-by-rate ( rlcrt-lst1 ses0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    over struct-inc-use-count

    session-rlclist-by-rate-disp +  \ Add offset.
    !                               \ Set the field.
;

\ Update the session-rlclist-by-rates for an session instance.
: _session-update-rlclist-by-rate ( rlcrt-lst1 ses0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-rlclist-by-rate
    dup  
    [ ' rlc-list-deallocate ] literal swap list-apply
    list-deallocate

    over struct-inc-use-count

    session-rlclist-by-rate-disp +  \ Add offset.
    !                               \ Set the field.
;

\ Return the session rules-by-rates list.
: session-get-rules-by-rate ( sess0 -- rlcrt-lst )
    \ Check arg.
    assert-tos-is-session

    session-rules-by-rate-disp +    \ Add offset.
    @                               \ Fetch the field.
;

\ Set the session rules-by-rates list.
: _session-set-rules-by-rate ( rlcrt-lst1 ses0 -- )
    \ Check args.
    assert-tos-is-session
    over 0<> if
        assert-nos-is-list
        over struct-inc-use-count
    then

    session-rules-by-rate-disp +    \ Add offset.
    !                               \ Set the fiel105612940047944d.
;

\ End accessors.

: session-stack-tos ( -- sess )
    session-stack stack-tos
;
' session-stack-tos to session-stack-tos-xt

\ Create an session, given an instance ID.
: current-session-new ( -- ) \ new session pushed onto session stack.
    
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
    list-new over _session-set-needs    \ ses

    \ Init rlcrate-list.
    list-new over _session-set-rlcrate-list  \ sess

    \ Init rlcrate-fragments.
    list-new over _session-set-rlcrate-fragments  \ sess

    \ Init session-rlcrate-le0-rates.
    list-new over _session-set-rlcrate-le0-rates

    \ Init session rlclist-by-rate.
    list-new over _session-set-rlclist-by-rate

    \ Init list of rule-list-corrs, by rate.
    list-new over _session-set-rules-by-rate

    session-stack stack-push
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
    list-get-links  \ sess0 link
    begin
        ?dup
    while
        dup link-get-data           \ sess0 link dom
        \ Set current domain
        dup                         \ sess0 link dom dom
        #3 pick                     \ sess0 link dom dom sess0
        session-set-current-domain  \ sess0 link dom
        \ Print domain
        .domain

        link-get-next               \ sess0 link
    repeat

    cr ." rlcrates: "
    dup session-get-rlcrate-list    \ sess0 lst
    .rlcrate-list
    cr

    cr ." rlcrate fragments: "
    dup session-get-rlcrate-fragments   \ sess0 lst
    .rlcrate-list
    cr

    cr ." rlclists, excluding lower value rlcrates: "
    dup session-get-rlclist-by-rate     \ sess0 rcllist-lst
    list-get-links                      \ sess0 rcllist-link 
    over session-get-rlcrate-le0-rates  \ sess0 rcllist-link rates-le0
    list-get-links                      \ sess0 rcllist-link rates-links
    begin
        ?dup
    while
        cr  ."    rate: " dup link-get-data #3 dec.r
        space ." rlclist: " over link-get-data .rlc-list

        link-get-next swap
        link-get-next swap
    repeat
    cr
                                        \ sess0 rcllist-link
    drop                                \ sess0

    drop                                \
;

\ Deallocate the session.
: current-session-deallocate ( -- )
    session-stack stack-tos         \ sess

    \ Clear fields.
    dup session-get-domains domain-list-deallocate
    dup session-get-needs need-list-deallocate
    dup session-get-rlcrate-list rlcrate-list-deallocate
    dup session-get-rlcrate-fragments rlcrate-list-deallocate

    dup session-get-rlcrate-le0-rates
    ?dup if
        list-deallocate
    then

    \ Deallocate a list of rlclists.
    dup session-get-rlclist-by-rate
    ?dup if
        dup
        [ ' rlc-list-deallocate ] literal swap list-apply
        list-deallocate
    then

    \ Deallocate a list of rule-list-corr lists.
    dup session-get-rules-by-rate       \ ses0 ruls-lst
    dup                                 \ sess0 ruls-lst ruls-lst
    [ ' rule-list-deallocate ] literal swap list-apply
    list-deallocate

    session-stack stack-pop drop
    session-mma mma-deallocate
;

: session-add-domain ( dom1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-domain
    \ cr ." session-add-domain: " over . cr

    \ Add domain
    2dup                    \ dom1 sess0 dom1 sess0
    session-get-domains     \ dom1 sess0 dom1 dom-lst
    domain-list-push-end    \ dom1 sess0

    \ Set current-domain, if it is zero/invalid.
    session-set-current-domain
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

    session-get-current-states      \ sta-lst
    dup .state-list-corr            \ sta-lst
    list-deallocate
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

: session-add-rlcrate ( rlcrt1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-rlcrate

    session-get-rlcrate-list        \ rlcrt1 rlcrt-lst
    rlcrate-list-push               \
;

: session-add-rlcrate-fragment ( rlcrt1 sess0 -- )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-rlcrate

    session-get-rlcrate-fragments   \ rlcrt1 rlcrt-lst
    rlcrate-list-push               \
;

\ Return an rlc of max domain regions.
: session-max-regions ( sess0 -- rlc )

    \ Get domain-list.
    session-get-domains                     \ dom-lst

   \ Init return list.
    list-new swap                           \ ret-lst dom-lst

    \ Prep for loop.
    list-get-links                          \ ret-lst d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ lst0 ret-lst link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ lst0 ret-lst link1 link0 d-link

        \ Add next region.
        dup link-get-data       \ ret-lst d-lisk domx
        domain-get-max-region-xt
        execute                 \ ret-lst d-lisk regx
        #2 pick                 \ ret-lst d-lisk regx ret-lst
        region-list-push-end    \ ret-lst d-lisk

        link-get-next           \ ret-lst d-link
    repeat
                                \ ret-lst
;

' session-max-regions to session-max-regions-xt

\ Process the given rlcrates.
: session-process-rlcrates ( sess0 -- )
    \ Check arg.
    assert-tos-is-session

   \  cr ." session-process-rlcrates" cr

    \ Get given rlcrates.
    dup session-get-rlcrate-list                \ sess0 rlcrt-lst

   \  cr ." Given rlcrates:  " dup .rlcrate-list cr
    
    rlcrate-list-to-rlc-list                    \ sess0 rlc-lst
    dup                                         \ sess0 rlc-lst rlc-lst

    \ Get rlc fragments of the given rlcrates, that are subsets of any given rlcrate that they intersect.
    rlc-list-intersection-fragments             \ sess0 rlc-lst rlc-lst2
    \ cr ." Fragment RLCs: " dup .rlc-list cr

    swap rlc-list-deallocate                    \ sess0 rlc-lst2

    \ Check fragments, and find values.
    over session-get-rlcrate-list               \ sess0 rlc-lst2 rlcrt-lst
    swap                                        \ sess0 rlcrt-lst rlc-lst2

    \ For each fragment, calc its aggregate rate, form an rlcrate, add it to the session-rclrate-fragments list.

    \ Init aggregate rate for the next fragment.
    0 0 rate-new                                \ sess0 rlcrt-lst rlc-lst2 rate-agg

    \ Prep for loop 1.
    over list-get-links                         \ sess0 rlcrt-lst rlc-lst2 rate-agg link

    begin
        ?dup
    while
        dup link-get-data                       \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx

        \ For each given rlcrate item.
        #4 pick list-get-links                  \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
        begin
            ?dup
        while
            \ Check if the loop1 rlc fragment interserts the loop2 given rlcrate.
            over                                    \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx
            over link-get-data                      \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcratey
            rlcrate-get-rlc                         \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty
            2dup                                    \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty rlcrtx rlcrty
            region-list-corr-intersects             \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty bool
            if
                \ Check that the loop2 intersecting rlcrate, is a rlc-superset of the loop1 fragment rlc. 
                2dup region-list-corr-superset      \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty bool
                if
                    \ Add the loop2 rlcrate rate to the aggregate rate for the loop1 rlc.
                                                    \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcrtx rlcrty
                    2drop                           \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
                    dup link-get-data               \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rlcratey
                    rlcrate-get-rate                \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rate

                    #4 pick                         \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2 rate rate-agg
                    rate-add                        \ sess0 rlcrt-lst rlc-lst2 rate-agg link | rlcrtx link2
                else
                    \ This should not happen, unless there is a problem with the rlc-list-intersection-fragments function.
                    cr .region-list-corr space ." not superset of " .region-list-corr space ." ?" cr
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

        \ Make rlcrate from loop1 fragment rlc and the aggregate rate.
        rot                                     \ sess0 rlcrt-lst rlc-lst2 link | rlcrtx rate-agg
        rlcrate-new                             \ sess0 rlcrt-lst rlc-lst2 link | rlcrate-new
        \ ." Fragment rlcrate: " dup .rlcrate cr

        \ Add the loop1 fragment rlcrate to the session rlcrate-fragments list. 
        #4 pick                                 \ sess0 rlcrt-lst rlc-lst2 link | rlcrate-new sess0
        session-get-rlcrate-fragments           \ sess0 rlcrt-lst rlc-lst2 link | rlcrate-new frg-lst
        rlcrate-list-push                       \ sess0 rlcrt-lst rlc-lst2 link |

        \ Prep for next loop1 fragment rlc cycle.
        0 0 rate-new                            \ sess0 rlcrt-lst rlc-lst2 link rate-agg
        swap                                    \ sess0 rlcrt-lst rlc-lst2 rate-agg link

        \ Next loop1 cycle.
        link-get-next                           \ sess0 rlcrt-lst rlc-lst2 rate-agg link
    repeat
                                                \ sess0 rlcrt-lst rlc-lst2 rate-agg
    \ Clean up.
    rate-deallocate                             \ sess0 rlcrt-lst rlc-lst2
    rlc-list-deallocate                         \ sess0 rlcrt-lst
    drop                                        \ sess0
    dup session-get-rlcrate-fragments           \ sess0 frg-lst
    \ cr ." Fragment rlcrates: " dup .rlcrate-list cr
                                                \ sess0 frg-lst
    \ Get all rate negative values.

    \ Init value list.
    list-new swap                               \ sess0 val-lst frg-lst
    list-get-links                              \ sess0 val-lst link
    
    begin
        ?dup
    while
        dup link-get-data                       \ sess0 val-lst link rlcratex
        rlcrate-get-rate                        \ sess0 val-lst link ratex
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
    \ That is, move anywhere in the FOM and not encounter a higher negative rlcrate.
    \
    \ The second lowest negative rate will have an FOM of max regions - lowest negative rlcrates.
    \
    \ The third lowest negative rate will have an FOM of max regions - lowest negative rlcrates - second-lowest negative rlcrates.
    \
    \ The zero rate FOM will be the maximum regions - all negative rlcrates.
    \
    \ Given a starting rlc and a goal rlc:
    \
    \    Start in the highest negative FOM that the start and goal states are in.
    \    If a plan can work within that FOM, use it.
    \    Otherwise try the next least restrictive FOM, possibly encountering higher negative rlcrates.
    \
    dup [ ' > ] literal swap list-sort

    
    \ cr ." values: " [ ' . ] literal  over .list cr

    \ Calculate rlc lists for change navigation.

    \ Init running subtraction list.
    over session-max-regions                    \ sess0 val-lst max-rlc
    list-new                                    \ sess0 val-lst max-rlc sub-lst
    tuck                                        \ sess0 val-lst sub-lst max-rlc sub-lst
    rlc-list-push                               \ sess0 val-lst sub-lst

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
        session-get-rlcrate-fragments           \ sess0 val-lst rslt-lst sub-lst link val frag-lst
        rlcrate-list-match-rate-negative-rlcs   \ sess0 val-lst rslt-lst sub-lst link rlc-lst

        \ Update sub-lst.
        dup                                     \ sess0 val-lst rslt-lst sub-lst link rlc-lst rlc-lst
        #3 pick                                 \ sess0 val-lst rslt-lst sub-lst link rlc-lst rlc-lst sub-lst
        rlc-list-subtract                       \ sess0 val-lst rslt-lst sub-lst link rlc-lst sub-lst2
        swap rlc-list-deallocate                \ sess0 val-lst rslt-lst sub-lst link sub-lst2
        rot drop                                \ sess0 val-lst rslt-lst link sub-lst2
        swap                                    \ sess0 val-lst rslt-lst sub-lst2 link

        \ Add rlc list result list.
        over                                    \ sess0 val-lst rslt-lst sub-lst2 link sub-lst2
        dup struct-inc-use-count
        \ cr ." rlclist: " dup .rlc-list cr
        #3 pick                                 \ sess0 val-lst rslt-lst sub-lst2 link sub-lst2 rslt-lst
        list-push                               \ sess0 val-lst rslt-lst sub-lst2 link

        link-get-next
    repeat
                                                \ sess0 val-lst rslt-lst sub-lst
    drop                                        \ sess0 val-lst rslt-lst

    \ Process result list.
    #2 pick _session-update-rlclist-by-rate     \ sess0 val-lst

    0 over list-push
    dup [ ' < ] literal swap list-sort
    \ cr ." values: " [ ' . ] literal  over .list cr

    over _session-update-rlcrate-le0-rates      \ sess0

    \ Process rlcs by rate.
    dup session-get-rlclist-by-rate                     \ sess rlc-lst-lst
    list-get-links                                      \ sess0 link
    begin
        ?dup
    while
        dup link-get-data                               \ sess0 link rlc-lst
        \ cr ." process " dup .rlc-list cr
        list-get-links                                  \ sess0 link link2
        begin
            ?dup
        while
            dup link-get-next                           \ sess0 link link2 link2+
            begin
                ?dup
            while
                over link-get-data                      \ sess0 link link2 link2+ rlc2
                over link-get-data                      \ sess0 link link2 link2+ rlc2 rlc2+
                \ cr ." compare "  2dup swap .region-list-corr space ." and " .region-list-corr
                2dup region-list-corr-intersection      \ sess0 link link2 link2+ rlc2 rlc2+, rlc-int t | f
                if
                    \ space ." int: " dup .region-list-corr
                                                        \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int
                    dup                                 \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int rlc-int
                    #3 pick                             \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int rlc-int rlc2
                    rule-list-corr-new-rlc-to-rlc       \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int, rul2 t | f
                    if
                        \ space ." rul " dup .rule-list-corr
                        rule-list-deallocate            \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int
                    else
                        abort" region-to-region failed?"
                    then

                    dup                                 \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int rlc-int
                    #2 pick                             \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int rlc-int rlc2+
                    rule-list-corr-new-rlc-to-rlc       \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int, rul2+ t | f
                    if
                        \ space ." rul " dup .rule-list-corr
                        rule-list-deallocate            \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int
                    else
                        abort" region-to-region failed?"
                    then
                                                        \ sess0 link link2 link2+ rlc2 rlc2+ rlc-int
                    region-list-deallocate              \ sess0 link link2 link2+ rlc2 rlc2+
                    2drop
                else
                    2drop
                then
                \ cr

                link-get-next
            repeat

            link-get-next
        repeat

        link-get-next
    repeat
                                                        \ sess0
    drop                                                \
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
: session-get-domain-list ( -- link )
    current-session     \ sess
    session-get-domains \ dom-lst
;

' session-get-domain-list to session-get-domain-list-xt

\ Return the rate and rlc list for a path to satisfy a desired region-list-corr.
: session-rlc-rate ( rlc1 sess0 -- rlc rate )
    \ Check args.
    assert-tos-is-session
    assert-nos-is-list

    dup session-get-rlcrate-le0-rates       \ rlc1 sess0 rt-lst
    list-get-links                          \ rlc1 sess0 rt-lnk
    swap session-get-rlclist-by-rate        \ rlc1 rt-lnk rlc-lst
    list-get-links                          \ rlc1 rt-lnk rlc-lnk

    begin
        ?dup
    while
        #2 pick                             \ rlc1 rt-lnk rlc-lnk rlc1
        over link-get-data                  \ rlc1 rt-lnk rlc-lnk rlc1 rlc-lstx
        rlc-list-any-superset               \ rlc1 rt-lnk rlc-lnk bool
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
            step-new                \ rlc-to rlc-from sess0 ret-plc to-link from-link dom-link plnx stp
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
