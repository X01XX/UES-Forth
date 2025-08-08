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

\ Check arg0 for bool, unconventional, leaves stack unchanged. 
: assert-arg0-is-bool ( u -- )
    dup 0=
    over -1 =
    or
    if
    else
        ." arg0 is not bool"
        abort
    then
;

\ Check arg1 for bool, unconventional, leaves stack unchanged. 
: assert-arg1-is-bool ( u ?? -- )
    over dup 0=
    swap -1 =
    or
    if
    else
        ." arg1 is not bool"
        abort
    then
;

: .bool ( b -- )
    \ Check arg.
    assert-arg0-is-bool

    0=
    if
        ." f"
    else
        ." t"
    then
;
