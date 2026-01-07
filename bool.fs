' 0= alias is-false

\ Check TOS for bool, unconventional, leaves stack unchanged.
: assert-tos-is-bool ( tos -- tos )
    dup is-false
    over -1 =
    or
    is-false if
        s" TOS is not bool"
        .abort-xt execute
    then
;

\ Check NOS for bool, unconventional, leaves stack unchanged.
: assert-nos-is-bool ( nos tos -- nos tos )
    over dup is-false
    swap -1 =
    or
    is-false if
        s" NOS is not bool"
        .abort-xt execute
    then
;

: .bool ( b -- )
    \ Check arg.
    assert-tos-is-bool

    is-false
    if
        ." f"
    else
        ." t"
    then
;
