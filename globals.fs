
\ Return the current session, instead of passing it down through arguments.
: current-session
    session-stack-tos-xt execute
;

\ Return the current domain, instead of passing it down through arguments.
: current-domain
    current-session
    session-get-current-domain-xt execute
;

\ Return the current action, instead of passing it down through arguments.
: current-action
    current-domain
    domain-get-current-action-xt execute
;

