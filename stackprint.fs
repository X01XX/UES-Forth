\ Print a stack, after an abort, giving information about structs on the stack.

\ Print the type of one value.
: .stack-structs2 ( addr -- )
    dup link-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." link-u "
        else
            ." link "
        then
        drop
        exit
    then

    dup list-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." list-u "
        else
            ." list-"
            dup list-get-length dec.
        then
        drop
        exit
    then

    dup region-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." region-u "
        else
            ." region "
        then
        drop
        exit
    then

    dup rule-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." rule-u "
        else
            ." rule "
        then
        drop
        exit
    then

    dup rulestore-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." rulestore-u "
        else
            ." rulestore "
        then
        drop
        exit
    then

    dup square-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." square-u "
        else
            ." square "
        then
        drop
        exit
    then

    dup sample-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." sample-u "
        else
            ." sample "
        then
        drop
        exit
    then

    dup changes-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." changes-u "
        else
            ." changes "
        then
        drop
        exit
    then

    dup group-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." group-u "
        else
            ." group "
        then
        drop
        exit
    then

    dup need-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." need-u "
        else
            ." need "
        then
        drop
        exit
    then

    dup step-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." step-u "
        else
            ." step "
        then
        drop
        exit
    then

    dup plan-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." plan-u "
        else
            ." plan "
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

    dup rlcrate-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." rlcrate-u "
        else
            ." rlcrate "
        then
        drop
        exit
    then

    dup action-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." action-u "
        else
            ." action "
        then
        drop
        exit
    then

    dup domain-mma mma-within-array
    if
        dup struct-get-id
        0= if
            ." domain-u "
        else
            ." domain "
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
        .stack-structs2 
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

