
\ Return the bitwise "NXOR" of two unsigned numbers.
\ while remaining within the bounds of allowable bits.
: !nxor ( u1 u2 -- u3 )
    xor !not
;

\ Check TOS for bool, unconventional, leaves stack unchanged. 
: assert-tos-is-bool ( u -- )
    dup 0=
    over -1 =
    or
    0=
    abort" TOS is not bool"
;

\ Check NOS for bool, unconventional, leaves stack unchanged. 
: assert-nos-is-bool ( u ?? -- )
    over dup 0=
    swap -1 =
    or
    0=
    abort" NOS is not bool"
;

\ Check 3OS for bool, unconventional, leaves stack unchanged. 
: assert-3os-is-bool ( u ?? ?? -- )
    #2 pick dup 0=
    swap -1 =
    or
    0=
    abort" 3OS is not bool"
;

: .bool ( b -- )
    \ Check arg.
    assert-tos-is-bool

    0=
    if
        ." f"
    else
        ." t"
    then
;
