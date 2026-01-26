\ Print a stack, after an abort, giving information about structs on the stack.

\ Print the type of one value.
: .stack-structs2 ( addr -- )
    dup link-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." lnk-u "
        else
            ." lnk "
        then
        drop
        exit
    then

    dup list-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." lst-u "
        else
            ." lst-"
            dup list-get-length dec.
        then
        drop
        exit
    then

    dup region-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." reg-u "
        else
            ." reg "
        then
        drop
        exit
    then

    dup rule-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." rul-u "
        else
            ." rul "
        then
        drop
        exit
    then

    dup rulestore-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." rulstr-u "
        else
            ." rulstr "
        then
        drop
        exit
    then

    dup square-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." sqr-u "
        else
            ." sqr "
        then
        drop
        exit
    then

    dup sample-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." smpl-u "
        else
            ." smpl "
        then
        drop
        exit
    then

    dup changes-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." cngs-u "
        else
            ." cngs "
        then
        drop
        exit
    then

    dup group-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." grp-u "
        else
            ." grp "
        then
        drop
        exit
    then

    dup need-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." ned-u "
        else
            ." ned "
        then
        drop
        exit
    then

    dup planstep-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." plnstp-u "
        else
            ." plnstp "
        then
        drop
        exit
    then

    dup plan-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." pln-u "
        else
            ." pln "
        then
        drop
        exit
    then

    dup plancorr-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." plnc-u "
        else
            ." plnc "
        then
        drop
        exit
    then

    dup rate-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." rate-u "
        else
            ." rate "
        then
        drop
        exit
    then

    dup regioncorrrate-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." regcrate-u "
        else
            ." regcrate "
        then
        drop
        exit
    then

    dup action-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." act-u "
        else
            ." act "
        then
        drop
        exit
    then

    dup domain-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." dom-u "
        else
            ." dom "
        then
        drop
        exit
    then

    dup regioncorr-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." regc-u "
        else
            ." regc "
        then
        drop
        exit
    then

    dup rulecorr-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." rulc-u "
        else
            ." rulc "
        then
        drop
        exit
    then

    dup pathstep-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." pthstp-u "
        else
            ." pthstp "
        then
        drop
        exit
    then

    dup changescorr-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." csgsc-u "
        else
            ." cngsc "
        then
        drop
        exit
    then

    dup corner-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." crn-u "
        else
            ." crn "
        then
        drop
        exit
    then

    dup current-session =
    if
        ." sess "
        drop
        exit
    then

    \ Default
    dec.
;

\ Cycle through each stack item, displaying its struct type.
: .stack-structs
    depth 0=
    if
        exit
    then
    depth 0 do
        depth 1- i - pick

        dup list-mma mma-within-array
        if
            dup struct-get-id
            0= if
                ." list-u "
            else
                ." list-"
                dup list-get-length dup abs 0 <# #S rot sign #> type
                dup list-get-length
                0<> if
                        ." -"
                        dup list-get-links link-get-data
                        .stack-structs2
                    else
                        space
                    then
            then
            drop
        else
            .stack-structs2
        then
    loop
;

' .stack-structs to .stack-structs-xt

: .abort ( c-addr u -- )
    cr type cr
    cr .s cr
    .stack-structs
    abort
;

' .abort to .abort-xt

