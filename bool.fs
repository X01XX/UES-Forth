\ Return the Boolean "NOT" of an unsigned number,
\ while remaining within the bounds of allowable bits.
: !not ( u1 -- u2 )
    domain-all-bits-xt execute
    xor
;

\ Return the Boolean "NAND" of two unsignreud numbers.
\ while remaining within the bounds of allowable bits.
: !nand ( u1 u2 -- u3 )
    and !not
;

\ Return the Boolean "NOR" of two unsigned numbers.
\ while remaining within the bounds of allowable bits.
: !nor ( u1 u2 -- u3 )
    or !not
;

\ Return the Boolean "NXOR" of two unsigned numbers.
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
