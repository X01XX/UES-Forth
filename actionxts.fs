\ Functions that actions take to get samples.

\ Perform an action for any Domain, Act 0, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: act-0-get-sample ( res1 flag cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip dup
    sample-new
;

\ Perform an action for Domain 0, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-0-act-1-get-sample ( res1 flag cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup 1 xor swap
    sample-new
;

\ Perform an action for Domain 1, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-1-act-1-get-sample ( res1 flag cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup 2 xor swap
    sample-new
;
