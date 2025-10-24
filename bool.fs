' 0= alias is-false

\ Check TOS for bool, unconventional, leaves stack unchanged. 
: assert-tos-is-bool ( u -- )
    dup is-false
    over -1 =
    or
    is-false abort" TOS is not bool"
;

\ Check NOS for bool, unconventional, leaves stack unchanged. 
: assert-nos-is-bool ( u ?? -- )
    over dup is-false
    swap -1 =
    or
    0=
    abort" NOS is not bool"
;

\ Check 3OS for bool, unconventional, leaves stack unchanged. 
: assert-3os-is-bool ( u ?? ?? -- )
    #2 pick dup is-false
    swap -1 =
    or
    0=
    abort" 3OS is not bool"
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
