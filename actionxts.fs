\ Functions that actions take to get samples.

\ Perform an action for any Domain, Act 0, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: act-0-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop
    dup
    sample-new
;

\ Perform an action for Domain 0, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-0-act-1-get-sample ( cur-sta1 act0 -- sample )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop

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
: domain-0-act-2-get-sample ( cur-sta1 act0 -- sample )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop

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

: domain-0-act-3-get-sample ( cur-sta1 act0 -- sample )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop

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
: domain-0-act-4-get-sample ( cur-sta1 act0-- sample )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop

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

\ An action that changes all bits.
: domain-0-act-5-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop
    dup !not swap

    sample-new
;

\ Exhibit X->0 and X->1 bit positions.
: domain-0-act-6-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop
    dup
    %0100 or
    %1110 and
    swap

    sample-new
;

\ Perform an action for Domain 1, Act 1, given:
\ Current state.
\ Flag, true if there is a previous result.
\ previous result, or 0 if none.
: domain-1-act-1-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop

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

: domain-1-act-2-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop

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

: domain-1-act-3-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop

    dup #16 xor swap     \ XXXXX -> xXXXX
    sample-new
;

\ Do an act with two possible changes, 1 xor, 2 xor.
: domain-1-act-4-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    over swap                   \ cur-sta1 cur-sta1 act0
    action-find-square          \ cur-sta1, sqr t | f
    if                          \ cur-sta1 sqr, there is a previous sample stored for this state.
        square-get-last-result  \ cur-sta1 last-rslt
        over                    \ cur-sta1 last-rslt cur-sta1
        xor                     \ cur-sta1 last-change
        1 =                     \ cur-sta1 bool
        if                      \ cur-sta1
            dup #2 xor          \ cur-sta1 rslt
        else                    \ cur-sta1
            dup 1 xor           \ cur-sta1 rslt
        then
    else                        \ cur-sta1, there is no previous sample stored for this state.
        #2 random               \ cur-sta1, 0 | 1
        1+ over xor             \ cur-sta1 rslt
    then
    swap                        \ rslt cur-sta1

    sample-new
;

\ An action that changes all bits.
: domain-1-act-5-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    drop
    dup !not swap

    sample-new
;

\ Do an act with three possible changes, to appear unpredictable.
: domain-1-act-6-get-sample ( cur-sta1 act0 -- smpl )
    \ Check args
    assert-tos-is-action
    assert-nos-is-value

    over swap                   \ cur-sta1 cur-sta1 act0
    action-find-square          \ cur-sta1, sqr t | f
    if                          \ cur-sta1 sqr, there is a previous sample stored for this state.
        square-get-last-result  \ cur-sta1 last-rslt
        over xor                \ cur-sta1 last-change

        case
            1 of
                dup #2 xor
            endof
            #2 of
                dup #4 xor
            endof
            #4 of
                dup 1 xor
            endof
        endcase
    else                    \ cur-sta1, there is no previous sample stored for this state.
        #2 random           \ cur-sta1 act0, 0 | 1
        1+ over xor         \ cur-sta1 rslt
    then
    swap                    \ rslt cur-sta1

    sample-new
;

