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

    dup 3 and
    case
        0 of    \ XX00 -> XX01
            dup 1+ swap
        endof
        1 of    \ XX01 -> XX11
            dup 2 + swap
        endof
        2 of    \ XX10 -> XX00
            dup 2 - swap
        endof
        3 of    \ XX11 -> XX10
            dup 1-  swap
        endof
    endcase

    sample-new
;

\ Perform an action for Domain 0, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-0-act-2-get-sample ( res1 flag cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup 12 and
    case
        0 of    \ 00XX -> 01XX
            dup 4 + swap
        endof
        4 of    \ 01XX -> 11XX
            dup 8 + swap
        endof
        8 of    \ 10XX -> 00XX
            dup 8 - swap
        endof
        12 of    \ 11XX -> 10XX
            dup 4 -  swap
        endof
    endcase

    sample-new
;

: domain-0-act-3-get-sample ( res1 flag cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup 3 and
    case
        0 of    \ XX00 -> XX10
            dup 2 + swap
        endof
        1 of    \ XX01 -> XX00
            dup 1- swap
        endof
        2 of    \ XX10 -> XX11
            dup 1+ swap
        endof
        3 of    \ XX11 -> XX01
            dup 2 -  swap
        endof
    endcase

    sample-new
;

\ Perform an action for Domain 0, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-0-act-4-get-sample ( res1 flag cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup 12 and
    case
        0 of    \ 00XX -> 10XX
            dup 8 + swap
        endof
        4 of    \ 01XX -> 00XX
            dup 4 - swap
        endof
        8 of    \ 10XX -> 11XX
            dup 4 + swap
        endof
        12 of    \ 11XX -> 01XX
            dup 8 -  swap
        endof
    endcase

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

    dup 3 and
    case
        0 of    \ XXX00 -> XXX01
            dup 1+ swap
        endof
        1 of    \ XXX01 -> XXX11
            dup 2 + swap
        endof
        2 of    \ XXX10 -> XXX00
            dup 2 - swap
        endof
        3 of    \ XXX11 -> XXX10
            dup 1-  swap
        endof
    endcase

    sample-new
;

: domain-1-act-2-get-sample ( res1 flag cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup 12 and
    case
        0 of    \ X00XX -> X01XX
            dup 4 + swap
        endof
        4 of    \ X01XX -> X11XX
            dup 8 + swap
        endof
        8 of    \ X10XX -> X00XX
            dup 8 - swap
        endof
        12 of    \ X11XX -> X10XX
            dup 4 -  swap
        endof
    endcase

    sample-new
;

: domain-1-act-3-get-sample ( res1 flag cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup 16 xor swap     \ XXXXX -> xXXXX
    sample-new
;
