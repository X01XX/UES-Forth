17317 constant mask-id
   2 constant mask-struct-number-cells
 100 constant mask-stack-number-items

0 constant mask-header
mask-header cell+ constant mask-value

\ Init mask mma, return the addr of allocated memory.
: mask-mma-init ( -- addr )
    mask-struct-number-cells mask-stack-number-items mma-new
;


\ Create a region from a number on the stack.
: mask-new ( u1 -- addr)
    \ Check the number of items on the stack.
    depth 1 < 
    if
        abort" mask-new: data stack has too few items"
    then

    \ Check u1.
    dup 0<
    if
        abort" mask-new: u1 is negative"
    then
    dup all-bits >
    if
        abort" mask-new u1 is too large"
    then

   \ Allocate space.
    mask-mma mma-allocate

    \ Check result.
    dup
    if
      dup >r   \ Save allocated addr.
      dup mask-id swap !   \ Store mask ID in first cell.
      mask-value +    \ Point it value in struct.
      !        \ Store value into mask.
      r>       \ Restore allocated addr.
    else
      abort" mask allocation failed"
    then
;

\ Return the value of a mask.
: mask-get-value ( addr -- u)
    \ Check mask ID
    dup @
    mask-id <>
    if
        abort" mask-get-value: argument is not a mask."
    then
    \ Get first word.
    mask-value +    \ Add offset.
    @
;

\ Print a mask.
: mask-print ( addr -- )
    \ Check mask ID
    dup @
    mask-id <>
    if
        abort" mask-print: argument is not a mask."
    then

    \ Setup for bit-position loop.
    mask-get-value
    ms-bit       ( st2 st1 ms-bit)

    \ Process each bit.
    begin
      dup
    while

      \ Apply msb to state 1
      over      \ Get value
      over      \ Get msb and isolate state 1 bit.
      and       \ Isolate state 1 bit corresponding to the msb.

      if
        ." 1"
      else
        ." 0"
      then

      1 rshift
    repeat
    2drop  \ Drop value msb
;

\ Deallocate a mask.
: mask-deallocate ( addr -- )
    \ Check mask ID
    dup @
    mask-id <>
    if
        abort" mask-deallocate: argument is not a mask."
    then
    mask-mma
    mma-deallocate
;

\ Return the Boolean "OR" of two masks.
: mask-or ( msk1 msk2 -- msk3)
    \ Check msk1 ID
    over @
    mask-id <>
    if
        abort" mask-or: arg 1 is not a mask."
    then
    \ Check msk2 ID
    dup @
    mask-id <>
    if
        abort" mask-or: arg 2 is not a mask."
    then

    \ Get mask values.
    mask-get-value
    swap
    mask-get-value

    \ Calc result.
    or
    mask-new
;

\ Return the Boolean "AND" of two masks.
: mask-and ( msk1 msk2 -- msk3)
    \ Check msk1 ID
    over @
    mask-id <>
    if
        abort" mask-and: arg 1 is not a mask."
    then
    \ Check msk2 ID
    dup @
    mask-id <>
    if
        abort" mask-and: arg 2 is not a mask."
    then

    \ Get mask values.
    mask-get-value
    swap
    mask-get-value

    \ Calc result.
    and
    mask-new
;

\ Return the Boolean "XOR" of two masks.
: mask-xor ( msk1 msk2 -- msk3)
    \ Check msk1 ID
    over @
    mask-id <>
    if
        abort" mask-xor: arg 1 is not a mask."
    then
    \ Check msk2 ID
    dup @
    mask-id <>
    if
        abort" mask-xor: arg 2 is not a mask."
    then

    \ Get mask values.
    mask-get-value
    swap
    mask-get-value

    \ Calc result.
    xor
    mask-new
;

\ Return the Boolean "NOT" of a mask.
: mask-not ( msk1 -- msk2 )
    \ Check msk1 ID
    dup @
    mask-id <>
    if
        abort" mask-not: arg 1 is not a mask."
    then

    \ Calc result.
    mask-get-value
    !not
    mask-new
;

\ Return a mask of same bits.
: mask-nxor ( msk1 msk2 -- msk3 )
    \ Check msk1 ID
    over @
    mask-id <>
    if
        abort" mask-nxor: arg 1 is not a mask."
    then
    \ Check msk2 ID
    dup @
    mask-id <>
    if
        abort" mask-nxor: arg 2 is not a mask."
    then

    \ Get mask values.
    mask-get-value
    swap
    mask-get-value

    \ Calc result.
    !nxor
    mask-new
;

\ Return the Boolean "NOR" of two masks.
: mask-nor ( msk1 msk2 -- msk3)
    \ Check msk1 ID
    over @
    mask-id <>
    if
        abort" mask-nor: arg 1 is not a mask."
    then
    \ Check msk2 ID
    dup @
    mask-id <>
    if
        abort" mask-nor: arg 2 is not a mask."
    then

    \ Get mask values.
    mask-get-value
    swap
    mask-get-value

    \ Calc result.
    !nor
    mask-new
;

\ Return the Boolean "NAND" of two masks.
: mask-nand ( msk1 msk2 -- msk3)
    \ Check msk1 ID
    over @
    mask-id <>
    if
        abort" mask-nand: arg 1 is not a mask."
    then
    \ Check msk2 ID
    dup @
    mask-id <>
    if
        abort" mask-nand: arg 2 is not a mask."
    then

    \ Get mask values.
    mask-get-value
    swap
    mask-get-value

    \ Calc result.
    !nand
    mask-new
;

\ Return true if a mask is all zeros.
: mask-is-low ( msk1 -- flag )
    \ Check msk1 ID
    dup @
    mask-id <>
    if
        abort" mask-nand: arg 1 is not a mask."
    then
    \ Calc result.
    mask-get-value
    0=
;


