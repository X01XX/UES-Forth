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
