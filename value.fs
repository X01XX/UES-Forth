\ Return true if a number is a valid value.
: is-value ( u0 -- flag )
    dup                                 \ u0 u0
    cur-domain-xt execute               \ u0 u0 dom
    domain-get-all-bits-mask-xt execute \ u0 u0 msk
    and                                 \ u0 u0'
    =                                   \ flag
;

\ Return true if a number is an invalid value.
: is-not-value ( u0 -- flag )
    dup                         \ u0 u0
    cur-domain-xt execute       \ u0 u0
    domain-get-all-bits-mask-xt
    execute                     \ u0 u0 msk
    and                         \ u0'
    <>                          \ flag
;

\ Check TOS for value, unconventional, leaves stack unchanged.
: assert-tos-is-value ( u -- u )
    dup is-not-value
    if
        s" TOS is not a valid value."
        .abort-xt execute
    then
;

\ Check NOS for value, unconventional, leaves stack unchanged.
: assert-nos-is-value ( u ?? -- u ??)
    over is-not-value
    if
        s" NOS is not a valid value."
        .abort-xt execute
    then
;

\ Check 3OS for value, unconventional, leaves stack unchanged.
: assert-3OS-is-value ( u ?? ?? -- u ?? ??)
    #2 pick is-not-value
    if
        s" 3OS is not a valid value."
        .abort-xt execute
    then
;

\ Print a value.
: .value ( val0 -- )
    \ Check arg.
    assert-tos-is-value

    \ Setup for bit-position loop.
    cur-domain-xt execute           \ val0 dom
    domain-get-ms-bit-mask-xt
    execute                         \ val0 ms-bit

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

\ Isolate a least-significant-bit from a value.
: value-isolate-lsb ( val0 -- val' bit )
    dup 1 < abort" invalid number"
    dup 1- over and     \ val0 val'  Remove lsb from val0.
    swap                \ val' val0
    over xor            \ val' lsb   Get lsb.
;

\ Return the number of bits in a value that are set to 1.
: value-num-bits ( u - nb )
    dup 0= if exit then

    \ Init counter
    0 swap              \ 0 u
    begin
        ?dup
    while
        \ Inc counter.
        swap 1+ swap            \ cnt+ u

        value-isolate-lsb drop  \ cnt+ u'
    repeat
;

\ Return true if a target value is between two values, inclusive.
\ That is, the target has no bit-position that is different from both other numbers,
\ That is, the target is within the region formed by the other two numbers.
: value-between ( target2 u1 u0 -- flag )
    rot tuck            \ u1 target2 u0 target2
    xor -rot            \ dif0 u1 target2
    xor                 \ dif0 dif1
    and                 \ dif-both
    0=                  \ flag
;

\ Return true if onlf one bit is set to one.
: value-1-bit-set ( val0 -- flag )
    \ Check for zero.
    dup 0= if
        drop
        false
        exit
    then

    value-isolate-lsb       \ rem bit
    drop
    0=
;

\ Return the bitwise "NOT" of an unsigned number,
\ while remaining within the bounds of allowable bits.
: !not ( u1 -- u2 )
    cur-domain-xt execute       \ u1 dom
    domain-get-all-bits-mask-xt \ u1 xt
    execute                     \ u1 all-bits
    tuck                        \ all u1 all
    xor                         \ all u1'
    and                         \ u1'' just to make sure.
;

\ Return the bitwise "NAND" of two unsigned numbers.
\ while remaining within the bounds of allowable bits.
: !nand ( u1 u2 -- u3 )
    and !not
;

\ Return the bitwise "NOR" of two unsigned numbers.
\ while remaining within the bounds of allowable bits.
: !nor ( u1 u2 -- u3 )
    or !not
;

\ Return the bitwise "NXOR" of two unsigned numbers.
\ while remaining within the bounds of allowable bits.
: !nxor ( u1 u2 -- u3 )
    xor !not
;
