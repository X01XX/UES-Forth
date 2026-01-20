
\ A store for the current session.
0 value current-session-store

: current-session ( -- ses )
    current-session-store   \ ses
    ?dup
    if
    else
        cr ." No session is allocated."
        abort
    then
;

\ Return the current domain, instead of passing it down through arguments.
: current-domain
    current-session
    session-get-current-domain-xt
    execute
;

\ Return the current domain id.
: current-domain-id
    current-domain
    domain-get-inst-id-xt
    execute
;

\ Return the current domain all bits mask.
: current-all-bits-mask
    current-domain
    domain-get-all-bits-mask-xt
    execute
;

\ Return the current domain most significant bit mask.
: current-ms-bit-mask
    current-domain
    domain-get-ms-bit-mask-xt
    execute
;

\ Return the current domain max-region.
: current-max-region
    current-domain
    domain-get-max-region-xt
    execute
;

\ Return the current domain number bits.
: current-num-bits
    current-domain
    domain-get-num-bits-xt
    execute
;

\ Return the current action, instead of passing it down through arguments.
: current-action
    current-domain
    domain-get-current-action-xt
    execute
;

\ Return the current actino id.
: current-action-id
    current-action
    action-get-inst-id-xt
    execute
;

: .stack-gbl
    .stack-structs-xt execute
;
