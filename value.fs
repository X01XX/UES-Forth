\ Return true if a number is a valid value.
: is-value ( u -- flag )
    dup domain-all-bits-xt execute and
    =
;
 
\ Return true if a number is an invalid value.
: is-not-value ( u -- flag )
    dup domain-all-bits-xt execute and
    <>
;

\ Check TOS for value, unconventional, leaves stack unchanged. 
: assert-tos-is-value ( u -- u )
    dup is-not-value
    abort" TOS is not a valid value."
;

\ Check NOS for value, unconventional, leaves stack unchanged. 
: assert-nos-is-value ( u ?? -- u ??)
    over is-not-value
    abort" NOS is not a valid value."
;

\ Check 3OS for value, unconventional, leaves stack unchanged. 
: assert-3OS-is-value ( u ?? ?? -- u ?? ??)
    2 pick is-not-value
    abort" 3OS is not a valid value."
;

\ Print a value.
: .value ( val0 -- )
    \ Check arg.
    assert-tos-is-value

    \ Setup for bit-position loop.
    domain-ms-bit-xt execute   ( val0 ms-bit)
    
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
