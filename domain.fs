\ Implement a Domain struct and functions.                                                                                             

31379 constant domain-id
    2 constant domain-struct-number-cells

\ Struct fields
0 constant domain-header    \ 16-bits [0] struct id [1] use count [2] instance id [3] num-bits
domain-header               cell+ constant domain-actions               \ A action-list

0 value domain-mma \ Storage for domain mma instance.

\ Init domain mma, return the addr of allocated memory.
: domain-mma-init ( num-items -- ) \ sets domain-mma.
    dup 1 < 
    if  
        ." domain-mma-init: Invalid number of items."
        abort
    then

    cr ." Initializing Action store."
    domain-struct-number-cells swap mma-new to domain-mma
;

\ Check instance type.
: is-allocated-domain ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup domain-mma mma-within-array 0=
    if  
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    domain-id =    
;

: is-not-allocated-domain ( addr -- flag )
    is-allocated-domain 0=
;

\ Check arg0 for domain, unconventional, leaves stack unchanged. 
: assert-arg0-is-domain ( arg0 -- arg0 )
    dup is-allocated-domain 0=
    if  
        cr ." arg0 is not an allocated domain"
        abort
    then
;

\ Start accessors.

\ Return the action-list from an domain instance.
: domain-get-actions ( dom0 -- lst )
    \ Check arg.
    assert-arg0-is-domain

    domain-actions +    \ Add offset.
    @                   \ Fetch the field.
;

\ Return the action-list from an domain instance.
: _domain-set-actions ( lst dom0 -- )
    \ Check arg.
    assert-arg0-is-domain
    assert-arg1-is-list

    domain-actions +    \ Add offset.
    !                   \ Set the field.
;

\ Return the instance ID from an domain instance.
: domain-get-inst-id ( dom0 -- u)
    \ Check arg.
    assert-arg0-is-domain

    \ Get intst ID.
    2w@
;
 
\ Set the instance ID of an domain instance, use only in this file.
: _domain-set-inst-id ( u1 dom0 -- )
    \ Check args.
    assert-arg0-is-domain

    \ Set inst id.
    2w!
;

\ Return the number bits used by a domain instance.
: domain-get-num-bits ( dom0 -- u)
    \ Check arg.
    assert-arg0-is-domain

    \ Get intst ID.
    3w@
;
 
\ Set the number bits used by a domain instance, use only in this file.
: _domain-set-num-bits ( u1 dom0 -- )
    \ Check args.
    assert-arg0-is-domain

    over 1 <
    if
        ." Invalid number of bits."
        abort
    then

    over 64 >
    if
        ." Invalid number of bits."
        abort
    then

    \ Set inst id.
    3w!
;

\ End accessors.

\ Create an domain, given an instance ID and number of bits to be used.
: domain-new ( nb1 id0  -- addr)
    
    \ Allocate space.
    domain-mma mma-allocate         \ nb1 id0 dom

    \ Store id.
    domain-id over                  \ nb1 id0 dom id dom
    struct-set-id                   \ nb1 id0 dom

    \ Init use count.
    0 over struct-set-use-count     \ nb1 id0 dom

    \ Set intance ID.
    swap over _domain-set-inst-id   \ nb1 dom

    \ Set num bits.
    swap over _domain-set-num-bits   \ dom

    \ Set actions list.             
    list-new                        \ dom lst
    dup struct-inc-use-count        \ dom lst
    over _domain-set-actions        \ dom
;

\ Print a domain.
: .domain ( act0 -- )
    \ Check arg.
    assert-arg0-is-domain

    dup domain-get-inst-id
    ." Sess: " .

    dup domain-get-actions
    dup list-get-length
    ."  num actions: " .
    ." actions " .action-list
;

\ Deallocate a domain.
: domain-deallocate ( act0 -- )
    \ Check arg.
    assert-arg0-is-domain

    dup struct-get-use-count      \ act0 count

    2 <
    if 
        \ Clear fields.
        dup domain-get-actions action-list-deallocate

        \ Deallocate instance.
        domain-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

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
