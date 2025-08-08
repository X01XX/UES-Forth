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

\ Print a value.
: .value ( val0 -- )
    \ Check arg.
    assert-arg0-is-value

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
