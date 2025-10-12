\ Functions that actions take to get samples.

\ Perform an action for any Domain, Act 0, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: act-0-get-sample ( res1 flag1 cur0 -- sample )
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
: domain-0-act-1-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup #3 and
    case
        0 of    \ XX00 -> XX01
            dup 1+ swap
        endof
        1 of    \ XX01 -> XX11
            dup #2 + swap
        endof
        #2 of    \ XX10 -> XX00
            dup #2 - swap
        endof
        #3 of    \ XX11 -> XX10
            dup 1-  swap
        endof
    endcase

    sample-new
;

\ Perform an action for Domain 0, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-0-act-2-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup #12 and
    case
        0 of    \ 00XX -> 01XX
            dup #4 + swap
        endof
        #4 of    \ 01XX -> 11XX
            dup #8 + swap
        endof
        #8 of    \ 10XX -> 00XX
            dup #8 - swap
        endof
        #12 of    \ 11XX -> 10XX
            dup #4 -  swap
        endof
    endcase

    sample-new
;

: domain-0-act-3-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup #3 and
    case
        0 of    \ XX00 -> XX10
            dup #2 + swap
        endof
        1 of    \ XX01 -> XX00
            dup 1- swap
        endof
        #2 of    \ XX10 -> XX11
            dup 1+ swap
        endof
        #3 of    \ XX11 -> XX01
            dup #2 -  swap
        endof
    endcase

    sample-new
;

\ Perform an action for Domain 0, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-0-act-4-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup #12 and
    case
        0 of    \ 00XX -> 10XX
            dup #8 + swap
        endof
        #4 of    \ 01XX -> 00XX
            dup #4 - swap
        endof
        #8 of    \ 10XX -> 11XX
            dup #4 + swap
        endof
        #12 of    \ 11XX -> 01XX
            dup #8 -  swap
        endof
    endcase

    sample-new
;

\ An action that changes all bits, allowing the quick, arbitrary,
\ establishment of a large reachable region, for testing purposes.
\ Using the commands:
\ sas 0 5 %0
\ sas 0 5 %1111
: domain-0-act-5-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value

    nip nip dup !not swap

    sample-new
;

\ Perform an action for Domain 1, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-1-act-1-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup #3 and
    case
        0 of    \ XXX00 -> XXX01
            dup 1+ swap
        endof
        1 of    \ XXX01 -> XXX11
            dup #2 + swap
        endof
        #2 of    \ XXX10 -> XXX00
            dup #2 - swap
        endof
        #3 of    \ XXX11 -> XXX10
            dup 1-  swap
        endof
    endcase

    sample-new
;

: domain-1-act-2-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup #12 and
    case
        0 of    \ X00XX -> X01XX
            dup #4 + swap
        endof
        #4 of    \ X01XX -> X11XX
            dup #8 + swap
        endof
        #8 of    \ X10XX -> X00XX
            dup #8 - swap
        endof
        #12 of    \ X11XX -> X10XX
            dup #4 -  swap
        endof
    endcase

    sample-new
;

: domain-1-act-3-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool

    nip nip

    dup #16 xor swap     \ XXXXX -> xXXXX
    sample-new
;

\ Do an act with two possible changes, 1 xor, 2 xor.
: domain-1-act-4-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value
    assert-nos-is-bool
    assert-3os-is-value

                            \ res1 flag1 cur0
    swap                    \ res1 cur0 flag1
    if                      \ res1 cur0, there is a previous result for this state.
        tuck                \ cur0 res1 cur0
        xor                 \ cur0 last-change
        1 =                 \ cur0 bool
        if                  \ cur0
            dup #2 xor      \ cur0 rslt
        else                \ cur0
            dup 1 xor       \ cur0 rslt
        then
    else                    \ res1 cur0, there is no previous result for this state. 
        nip                 \ cur0
        #2 random           \ cur0 0|1
        1+ over xor         \ cur0 rslt
    then
    swap                    \ rslt cur0

    sample-new
;

\ An action that changes all bits, allowing the quick, arbitrary,
\ establishment of a large reachable region, for testing purposes.
\ Using the commands:
\ sas 1 5 %0
\ sas 1 5 %11111
: domain-1-act-5-get-sample ( res1 flag1 cur0 -- sample )
    \ Check args
    assert-tos-is-value

    nip nip dup !not swap

    sample-new
;
