\ Domain struct (TODO)

0 value current-domain      \ Address of the current domain.
4 value num-bits            \ Number bits used by current domain.

\ Functions that will eventually use current-domain value.

\ Return the number of bits used by the domain.
: domain-num-bits ( -- u )
    num-bits
;

\ Return most significant bit mask for a domain.
: domain-ms-bit ( -- u )
    1 num-bits 1- lshift
;

\ Return mask of all bits used.
: domain-all-bits ( -- u )
    domain-ms-bit 1- 1 lshift 1+
;

\ Tool functions that use domain functions.

\ Return true if a number is a valid value.
: is-value ( u -- flag )
    dup domain-all-bits and
    =
;
 
\ Return true if a number is an invalid value.
: is-not-value ( u -- flag )
    dup domain-all-bits and
    <>
;

\ Check arg0 for value, unconventional, leaves stack unchanged. 
: assert-arg0-is-value ( u -- u )
    dup is-not-value
    if
        ." arg0 is not a valid value."
        abort
    then
;

\ Check arg1 for value, unconventional, leaves stack unchanged. 
: assert-arg1-is-value ( u ?? -- u ??)
    over is-not-value
    if
        ." arg1 is not a valid value."
        abort
    then
;

\ Check arg2 for value, unconventional, leaves stack unchanged. 
: assert-arg2-is-value ( u ?? ?? -- u ?? ??)
    2 pick is-not-value
    if
        ." arg2 is not a valid value."
        abort
    then
;

\ Return the Boolean "NOT" of an unsigned number,
\ while remaining within the bounds of allowable bits.
: !not ( u1 -- u2 )
    domain-all-bits
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

\ Print a value.
: .value ( val0 -- )
    \ Check arg.
    assert-arg0-is-value

    \ Setup for bit-position loop.
    domain-ms-bit   ( val0 ms-bit)
    
    \ Process each bit.
    begin
      dup       \ val0 ms-bit ms-bit
    while
      \ Apply msb to value, to get an isolated bit.
      2dup
      and       \ val0 ms-bit bit

      if
        ." 1"
      else
        ." 0"
      then

      1 rshift   \ val0 ms-bit
    repeat
    2drop       \
;

\ Print state to state rule.
: .state-to-state ( r-val1 i-val0 -- )
    \ Check arg.
    assert-arg0-is-value
    assert-arg0-is-value

    \ Setup for bit-position loop.
    domain-ms-bit   ( r-val1 i-val0 ms-bit)

    \ Process each bit.
    begin
      dup       \ r-val1 i-val0 ms-bit ms-bit
    while
        \ Apply msb to i-val0, to get an isolated bit.
        2dup
        and       \ r-val1 i-val0 ms-bit bit

        if
            ." 1"
        else
            ." 0"
        then

        \ Apply msb to r-val1
        2 pick over and

        if
            ." 1"
        else
            ." 0"
        then

        1 rshift   \ r-val1 i-val0 ms-bit

        \ Check if separator is needed.
        dup if
            ." /"
        then
    repeat
    2drop drop       \
;
