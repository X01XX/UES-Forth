decimal

\ Exponential function, exponent s/b GE zero.
\ If at the limit, 2 ^ 63, use "u." instead of "." to see the number.
: exp ( n u -- u2 ) \ u3 = u1^u2,
    dup 0<
    abort" exp: exponent is negative"

    1 swap 0
    ?do 
        cr .s
        over * 
        dup 0=
        abort" exp: overflow"
    loop 
    nip \ Do exponentiation.
;

\ A shorter version of clearstack.
: cs ( -- ) \ Side effect: Data stack is cleared.
    clearstack
;

\ Show base in decimal, while preserving the base.
: base-dec ( -- )  \ Side effect: Print a number.
    base @         \ Get current base.
    decimal dup .  \ Make copy and display as decimal.
    base !         \ Restore base.
;

\ Set base to binary.
: binary  ( -- ) \ Side effect: Session base is changed.
    2 base !
;

\ Return true if exactly one bit is set.
: one-bit-set ( n -- flag )
    dup 0>
    if
      dup 1 - and
      0=
    else              \ Not GT 0
      dup 0<
      if              \ LT 0
         1 lshift 0=  \ Check for only the MSBit being set.
      \ else zero drops through as the false flag.
      then
    then
;

\ Count the number of bits set to one.
: num-one-bits ( n -- u )
    0 swap             \ Init counter, put below working number.
    begin
        dup
    while 
        swap 1 + swap   \ Increment counter.
        dup 1 - and     \ Remove one bit set to one.
    repeat
    drop                \ Drop working cell, leaving result.
;

\ Isolate LSB from a non-zero number.
\ Return changed number and a single-bit number.
: isolate-a-bit ( u1 -- u2 u3 )
    depth
    0= abort" isolate-a-bin: no argument on stack"

    dup 0=
    abort" isolate-a-bit: argument is zero"

    \ Remove lsb.
    dup 1- over and     \ u u-lsb 

    \ Isolate lsb.
    tuck xor            \ u-lsb lsb
;

: 3drop ( x y z -- )
    2drop drop
;

\ Return true if a number is between two numbers.
: between-inclusive ( target num1 num2 -- flag )
    2dup < if   \ num1 LT num2
        2 pick swap     \ target num1 target num2
        \ Check target vs num2.
        > if
            2drop false exit
          then
        \ Check target vs num1.
        < if
            false exit
          then
    else
        2 pick swap     \ target num1 target num2
        \ Check target vs num2.
        < if
            2drop false exit
          then
        \ Check target vs num1.
        > if
            false exit
          then
    then
    true
;

\ Fetch the [0], 16 bits of a cell.
: 0w@ ( addr -- u )
    uw@
;

\ Store a number in the [0], 16 bits of a cell.
: 0w! ( u addr -- )
    w!
;

\ Fetch the [1], 16 bits of a cell.
: 1w@ ( addr -- u )
    2 + uw@
;

\ Store a number in the [1], 16 bits of a cell.
: 1w! ( u addr -- )
    2 + w!
;

\ Fetch the [2] 16 bits of a cell.
: 2w@ ( addr -- u )
    4 + uw@
;

\ Store a number in the [2], 16 bits of a cell.
: 2w! ( u addr -- )
    4 + w!
;

\ Fetch the [3] 16 bits of a cell.
: 3w@ ( addr -- u )
    6 + uw@
;

\ Store a number in the [3], 16 bits of a cell.
: 3w! ( u addr -- )
    6 + w!
;

\ Fetch the [0], 8 bits of a cell.
: 0c@ ( addr -- u )
    c@
;

\ Store a number in the [0], 8 bits of a cell.
: 0c! ( u addr -- )
    c!
;

\ Fetch the [1], 8 bits of a cell.
: 1c@ ( addr -- u )
    1+ c@
;

\ Store a number in the [1], 8 bits of a cell.
: 1c! ( u addr -- )
    1+ c!
;

\ Fetch the [2], 8 bits of a cell.
: 2c@ ( addr -- u )
    2 + c@
;

\ Store a number in the [2], 8 bits of a cell.
: 2c! ( u addr -- )
    2 + c!
;

\ Fetch the [3], 8 bits of a cell.
: 3c@ ( addr -- u )
    3 + c@
;

\ Store a number in the [3], 8 bits of a cell.
: 3c! ( u addr -- )
    3 + c!
;

\ Fetch the [4], 8 bits of a cell.
: 4c@ ( addr -- u )
    4 + c@
;

\ Store a number in the [4], 8 bits of a cell.
: 4c! ( u addr -- )
    4 + c!
;

\ Fetch the [5], 8 bits of a cell.
: 5c@ ( addr -- u )
    5 + c@
;

\ Store a number in the [5], 8 bits of a cell.
: 5c! ( u addr -- )
    5 + c!
;

\ Fetch the [6], 8 bits of a cell.
: 6c@ ( addr -- u )
    6 + c@
;

\ Store a number in the [6], 8 bits of a cell.
: 6c! ( u addr -- )
    6 + c!
;

\ Fetch the [7], 8 bits of a cell.
: 7c@ ( addr -- u )
    7 + c@
;

\ Store a number in the [7], 8 bits of a cell.
: 7c! ( u addr -- )
    7 + c!
;

\ Store a string on the stack to a given address.
: string! ( string-addr length target-addr -- )
    2dup c!         \ Store the length at target[0].
    1+              \ Point to target[1].
    swap cmove      \ Move characters to target[1].
;

\ Fetch a string from a given address, put on stack.
: string@ ( string-addr -- string-addr+1 length )
    dup c@          \ addr length
    swap 1+ swap    \ addr+1 length
;

\ Return the struct id from a struct instance.
: struct-get-id ( addr -- u1 )
    0w@               \ Fetch the ID. 
;

\ Set the struct id,
: struct-set-id ( u addr -- )
    0w!    \ Store the ID.
;

\ Get struct use count.
: struct-get-use-count ( struct-addr -- u-uc )
    1w@ 
;

\ Set struct use count.
: struct-set-use-count ( u-16 struct-addr -- )
    1w! 
;

\ Decrement struct use count.
: struct-dec-use-count ( struct-addr -- )
    dup struct-get-use-count      \ struct-addr use-count
    dup 0 <
    abort" use count cannot be negative."

    1-
    swap struct-set-use-count
;

\ Increment struct use count.
: struct-inc-use-count ( struct-addr -- )
    dup struct-get-use-count      \ struct-addr use-count
    1+  
    swap struct-set-use-count
;


\ Return most significant bit mask for a given number of bits.
: ms-bit ( nb0 -- u )
    1 swap              \ 1 nb0
    1-                  \ 1 nb0-
    lshift              \ msb
;

\ Return mask of all bits used, given number of bits.
: all-bits ( nb0 -- u )
    ms-bit
    1-
    1 lshift
    1 +
;

\ The reverse of 2rot.
: -2rot 2rot 2rot ;

\ Return a terrible random number, from 0 to a given limit - 1.
\ Used to exercise different sections of code, purity of randomness is not important. 
: random ( limit -- result )
    dup 1 < abort" limit is zero?"

    \ Get the stack pointer.
    sp@

    \ Add seconds + minute-seconds.
    time&date 2drop 2drop 60 * + +

    \ Add limit.
    over +

    \ Get the return address, a code pointer.
    r@

    \ Modify return addr.
    xor

    \ Get result.
    swap mod
;
