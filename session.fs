\ Implement a Session struct and functions.                                                                                             

31319 constant session-id
    4 constant session-struct-number-cells

\ Struct fields
0 constant session-header    \ 16-bits [0] struct id [1] use count
session-header              cell+ constant session-domains              \ A domain-list
session-domains             cell+ constant session-current-domain       \ A domain, or zero before first domain is added.
session-current-domain      cell+ constant session-needs                \ A need-list.

0 value session-addr \ Storage for session address.

\ Check instance type.
: is-allocated-session ( addr -- flag )
    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    session-id =    
;

: is-not-allocated-session ( addr -- flag )
    is-allocated-session 0=
;

\ Check TOS for session, unconventional, leaves stack unchanged. 
: assert-tos-is-session ( arg0 -- arg0 )
    dup is-allocated-session 0=
    abort" TOS is not an allocated session"
;

\ Start accessors.

\ Return the domain-list from an session instance.
: session-get-domains ( ses0 -- lst )
    \ Check arg.
    assert-tos-is-session

    session-domains +   \ Add offset.
    @                   \ Fetch the field.
;

\ Set the domain-list for an session instance.
: _session-set-domains ( lst ses0 -- )
    \ Check arg.
    assert-tos-is-session
    assert-nos-is-list

    session-domains +   \ Add offset.
    !                   \ Set the field.
;

\ Return the current domain from an session instance.
: session-get-current-domain ( ses0 -- dom )
    \ Check arg.
    assert-tos-is-session

    session-current-domain +    \ Add offset.
    @                           \ Fetch the field.
;

' session-get-current-domain to session-get-current-domain-xt

\ Set the current domain for an session instance.
: session-set-current-domain ( dom ses0 -- )
    \ Check arg.
    assert-tos-is-session
    over 0<> if
        assert-nos-is-domain
    then

    session-current-domain +    \ Add offset.
    !                           \ Set the field.
;

' session-set-current-domain to session-set-current-domain-xt

\ Return the session need-list
: session-get-needs ( sess0 -- ned-lst )
    \ Check arg.
    assert-tos-is-session

    session-needs +             \ Add offset.
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

    session-needs +             \ Add offset.
    !                           \ Set the field.
;

\ Update the session needs, deallocating the previous list, if any.
: _session-update-needs  ( ned-lst sess0 -- )
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

\ End accessors.

\ Create an session, given an instance ID.
: session-new ( -- addr)
    
    \ Allocate space.
    \ session-mma mma-allocate        \ses
    session-struct-number-cells cells allocate
    abort" Session allocation failed"

    \ Store id.
    session-id over                 \ ses id ses
    struct-set-id                   \ ses

    \ Init use count.
    1 over struct-set-use-count     \ ses

    \ Set domains list.             
    list-new                        \ ses lst
    dup struct-inc-use-count        \ ses lst
    over _session-set-domains       \ ses

    \ Zero-out current domain.
    0 over session-set-current-domain

    \ Zero out need-list
    0 over _session-set-needs       \ ses
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
        3 pick                      \ sess0 link dom dom sess0
        session-set-current-domain  \ sess0 link dom
        \ Print domain
        .domain

        link-get-next               \ sess0 link
    repeat

    drop
;

\ Deallocate a session.
: session-deallocate ( ses0 -- )
    \ Check arg.
    assert-tos-is-session

    \ Clear fields.
    dup session-get-domains domain-list-deallocate
    dup session-get-needs
    ?dup if need-list-deallocate then

    \ Deallocate instance.
    0 over struct-set-id
    free
    abort" session free failed"
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

    nip nip nip
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

: .session-current-state ( sess0 -- )
    \ Check args.
    assert-tos-is-session

    dup session-get-domains             \ sess0 dom-lst

    list-get-links                      \ sess0 link
    ." ("
    begin
        ?dup
    while
        dup link-get-data           \  sess0 link domx

        dup 3 pick session-set-current-domain
        
        domain-get-current-state    \ sess0 link stax
        .value

        link-get-next               \ sess0 link
        dup if
            space
        then
    repeat
    ." )"
                                    \ sess0
    drop
;

: session-set-all-needs ( sess0 -- )
    \ Check args.
    assert-tos-is-session

    dup session-get-domains         \ sess0 dom-lst

    \ Init list to start appending domain need lists to.
    list-new swap                   \ sess0 lst-ret lst0

    \ Scan domain-list, getting needs from each domain.
    list-get-links                  \ sess0 lst-ret link
    begin
        ?dup
    while
        dup link-get-data           \ sess0 lst-ret link domx

        dup 4 pick                  \ sess0 lst-ret link domx domx sess0
        session-set-current-domain  \ sess0 lst-ret link domx

        domain-get-needs            \ sess0 lst-ret link dom-neds
        rot                         \ sess0 link dom-neds lst-ret
        2dup                        \ sess0 link dom-neds lst-ret dom-neds lst-ret
        need-list-append            \ sess0 link dom-neds lst-ret'
        swap need-list-deallocate   \ sess0 link lst-ret'
        swap                        \ sess0 lst-ret' link

        link-get-next
    repeat
                                    \ sess0 lst-ret

    \ Store needs in session.
    swap _session-update-needs      \
;

\ Return the current domain.
: cur-domain ( -- dom )
    current-session session-get-current-domain
;

' cur-domain to cur-domain-xt
