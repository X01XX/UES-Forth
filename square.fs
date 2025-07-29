\ Implement a square struct and functions.

23197 constant square-id                                                                                  
    7 constant square-struct-number-cells

\ Struct fields
0 constant square-header
square-header   cell+ constant square-state    \ id (16) use count (16) result count (16) pn (8) pnc (8)
                                               \ The result count, mod 4, will be the next element to use for a new result.
square-state    cell+ constant square-rules      
square-rules    cell+ constant square-results  \ Circular buffer of 4 cells, starting here.

0 value square-mma \ Storage for square mma instance.

\ Init square mma, return the addr of allocated memory.
: square-mma-init ( num-items -- ) \ sets square-mma.
    dup 1 < 
    if  
        ." square-mma-init: Invalid number of items."
        abort
    then

    cr ." Initializing Region store."
    square-struct-number-cells swap mma-new to square-mma
;

\ Check instance type.
: is-allocated-square ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup square-mma mma-within-array 0=
    if  
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    square-id =    
;

: is-not-allocated-square ( addr -- flag )
    is-allocated-square 0=
;

\ Check arg0 for square, unconventional, leaves stack unchanged. 
: assert-arg0-is-square ( arg0 -- arg0 )
    dup is-allocated-square 0=
    if  
        cr ." arg0 is not an allocated square"
        abort
    then
;

\ Check arg1 for square, unconventional, leaves stack unchanged. 
: assert-arg1-is-square ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-square 0=
    if  
        cr ." arg1 is not an allocated square"
        abort
    then
;

\ Start accessors.

\ Return result count from the square header.
: square-get-result-count ( square-addr -- u-length )
    \ Check arg.
    assert-arg0-is-square

    2w@ 
;

\ Set square result count, use only in this file.
: _square-set-result-count ( length-value square-addr -- )
    \ Check arg.
    assert-arg0-is-square

    2w! 
;

\ Increment square result count, use only in this file.
: _square-inc-result-count ( struct-addr -- )
    \ Check arg.
    assert-arg0-is-square

    dup square-get-result-count      \ struct-addr result-count
    1+
    swap _square-set-result-count
;

\ Return the square state. 
: square-get-state ( addr -- u)
    \ Check arg.
    assert-arg0-is-square

    square-state +      \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the state of a square instance, use only in this file.
: _square-set-state ( u1 addr -- )
    \ Check args.
    assert-arg0-is-square
    assert-arg1-is-value

    square-state +      \ Add offset.
    !                   \ Set first field.
;

: square-get-pn ( sqr0 -- pn )
    \ Check arg.
    assert-arg0-is-square

    6c@
;

: _square-set-pn ( pn1 sqr0 -- )
    \ Check args.
    assert-arg0-is-square

    over 1 <
    if
        ." _square-set-pn: invalid pn value"
        abort
    then

    over 3 >
    if
        ." _square-set-pn: invalid pn value"
        abort
    then

    6c!
;

\ Return square 8-bit pnc value, as a bool.
: square-get-pnc ( sqr0 -- bool )
    \ Check arg.
    assert-arg0-is-square

    7c@
    0<>     \ Change 255 to -1
;

: _square-set-pnc ( pnc sqr -- )
    \ Check arg.
    assert-arg0-is-square
    assert-arg1-is-bool

    7c!
;

: square-get-rules ( sqr0 -- rulstr )
    \ Check arg.
    assert-arg0-is-square

    square-rules + @
;

: _square-set-rules ( rulstr1 sqr0 -- )
    \ Check args.
    assert-arg0-is-square
    assert-arg1-is-rulestore

    over struct-inc-use-count

    square-rules + !
;

\ Get results item, given index and result.
: square-get-result ( index1 sqr0 -- result )
    \ Check arg.
    assert-arg0-is-square

    over dup                    \ i1 s0 i i
    0< swap 3 > or              \ i1 s0 flag
    if
        cr ." invalid index"
        abort
    then

    \ Point to results array.
    square-results +            \ addr

    \ Point to item in array
    swap cells +                \ addr
    @
;

\ Set results item, given index and result, use only in this file.
: _square-set-result ( result2 index1 sqr0 -- )
    \ Check arg.
    assert-arg0-is-square
    assert-arg2-is-value

    over dup                    \ r2 i1 s0 i i
    0< swap 3 > or              \ r2 i1 s0 flag
    if
        cr ." invalid index"
        abort
    then

    \ Point to results array.
    square-results +            \ addr

    \ Point to item in array
    swap cells +                \ r2 addr
    !
;

\ For square results.
\     Address = (0 through 3) cells square-results +
\     Next index to store into = results-count 4 mod
\     Most recently stored result = results-count 1- 4 mod

\ End accessors.

\ Return a new square, given a state and result.
: square-new    ( result state -- square )
    \ Check args.
    assert-arg0-is-value
    assert-arg1-is-value

   \ Allocate space.
    square-mma mma-allocate     \ r s addr

    \ Store id.
    square-id over              \ r s addr id addr
    struct-set-id               \ r s addr
        
    \ Init use count.
    0 over struct-set-use-count \ r s addr

    \ Set result count.
    1 over _square-set-result-count     \ r s addr

    \ Set rules
    2 pick 2 pick                       \ r s addr r s
    rule-new                            \ r s addr rul
    rulestore-new-1                     \ r s addr rulstr
    over _square-set-rules              \ r s addr

    \ Set state
    swap over _square-set-state  \ r addr

    \ Set first result.
    swap over                   \ addr r addr
    square-results +            \ addr r addr+
    !                           \ addr

    \ Set pn value.
    1 over _square-set-pn       \ addr

    \ Set pnc value.
    0 over _square-set-pnc      \ addr
;

: square-deallocate ( sqr0 -- )
    \ Check arg.
    assert-arg0-is-square

    dup struct-get-use-count      \ sqr0 count

    2 <
    if
        \ Deallocate instance.
        dup square-get-rules
        rulestore-deallocate
        square-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Print a square.
: .square ( sqr0 -- )
    \ Check arg.
    assert-arg0-is-square

    dup square-get-state .value

    ."  pnc: "
    dup square-get-pnc
    if
        ." t"
    else
        ." f"
    then

    ."  pn: "
    dup square-get-pn
    case
        1 of ." 1" endof
        2 of ." 2" endof
        ." U"
    endcase
    ."  "
    dup square-get-rules
    .rulestore

    \ sqr0
    ."  rc "
    square-get-result-count .
;

\ Return true if pn = 1.
\ All results must be equal.
: _square-check-pn-1 ( sqr0 -- flag )
    \ Check args.
    assert-arg0-is-square

    \ Get results count, must be at least 1.
    dup square-get-result-count     \ sqr0 rc

    \ Exit if no more checks can be made.
    dup 1 =
    if
        2drop true exit
    then

    \ rc must be > 1, check r0 = r1
    over                            \ sqr0 rc sqr0
    0 swap square-get-result        \ sqr0 rc r0
    2 pick                          \ sqr0 rc r0 sqr0
    1 swap square-get-result        \ sqr0 rc r0 r1
    over <>                         \ sqr0 rc r0 flag
    if
        2drop drop false exit
    then

    \ Exit if no more checks can be made.
    over 2 =
    if
        2drop drop true exit
    then

    \ rc must be > 2, check r0 = r2
    2 pick                          \ sqr0 rc r0 sqr0
    2 swap square-get-result        \ sqr0 rc r0 r2
    over <>                         \ sqr0 rc r0 flag
    if
        2drop drop true exit
    then

    \ Exit if no more checks can be made.
    over 3 =
    if
        2drop drop true exit
    then

    \ rc must be > 3, check r0 = r3
    nip                             \ sqr0 r0
    swap                            \ r0 sqr0
    3 swap square-get-result        \ r0 r3
    =                               \ flag
;

\ Return true if pn = 2.
\ r0 <> r1, r0 = r2.
\ r1 <> r2, r1 = r3.
: _square-check-pn-2 ( sqr0 -- flag )
    \ Check args.
    assert-arg0-is-square

    \ Get results count, must be at least 1.
    dup square-get-result-count     \ sqr0 rc

    \ Exit if no more checks can be made.
    dup 1 =
    if
        2drop false exit
    then

    \ rc must be > 1, check r0 <> r1
    over                            \ sqr0 rc sqr0
    0 swap square-get-result        \ sqr0 rc r0
    2 pick                          \ sqr0 rc r0 sqr0
    1 swap square-get-result        \ sqr0 rc r0 r1
    over =                          \ sqr0 rc r0 flag
    if
        2drop drop false exit
    then

    \ Exit if no more checks can be made.
    over 2 =
    if
        2drop drop true exit
    then

    \ rc must be > 2, check r0 = r2
    2 pick                          \ sqr0 rc r0 sqr0
    2 swap square-get-result        \ sqr0 rc r0 r2
    <>                              \ sqr0 rc flag
    if
        2drop false exit
    then

    \ Exit if no more checks can be made.
    dup 3 =
    if
        2drop true exit
    then

    \ Check r1 = r3
    drop                            \ sqr0
    1 over square-get-result        \ sqr0 r1
    swap                            \ r1 sqr0
    3 swap square-get-result        \ r1 r3
    =
    if
        true
    else
        false
    then
;

\ Return current pn number.
: square-calc-pn ( sqr0 -- u )
    \ Check arg.
    assert-arg0-is-square

    dup _square-check-pn-1         \ sqr0 flag
    if
        drop 1 exit
    then

    _square-check-pn-2             \ flag
    if
        2
    else
        3
    then
;

\ Calc a pnc value for a square.
: square-calc-pnc ( sqr0 -- bool )
    \ Check arg.
    assert-arg0-is-square

    dup square-get-result-count     \ sqr0 count
    swap square-get-pn              \ count pn

    case
        1 of
            2 >
        endof
        2 of
            3 >
        endof
        3 of
            drop
            true
        endof
    endcase
;

\ Return a rule constructed a square state and the first result.
: square-calc-rule-0 ( sqr0 -- rul )
    \ Check arg.
    assert-arg0-is-square

    0 over square-get-result    \ sqr0 r0
    swap square-get-state       \ r0 sta
    rule-new
;

\ Return a rule constructed a square state and second result.
: square-calc-rule-1 ( sqr0 -- rul )
    \ Check arg.
    assert-arg0-is-square

    1 over square-get-result    \ sqr0 r1
    swap square-get-state       \ r1 sta
    rule-new
;

\ Return rules for a square.
: square-calc-rules ( sqr0 -- rulestore )
    \ Check arg.
    assert-arg0-is-square

    dup square-get-pn                   \ sqr0 pn

    case                                \ sqr0
        1 of
            square-calc-rule-0          \ rul0
            rulestore-new-1             \ rulstr
        endof
        2 of
            dup square-calc-rule-0      \ sqr0 rul0
            swap square-calc-rule-1     \ rul0 rul1
            rulestore-new-2             \ rulstr
        endof
        3 of
            drop
            rulestore-new-0             \ sqr0 rulstr
        endof
    endcase
;

\ Add a new result to a square.
\ Return true if pn, or pnc changed.
: square-add-result ( val1 sqr0 -- flag )
    \ Check args.
    assert-arg0-is-square
    assert-arg1-is-value

    \ Set return flag to false.
    false -rot                      \ rf val1 sqr0

    \ Save square addr.
    swap over                       \ rf sqr0 val1 sqr0

    \ Get result count
    dup square-get-result-count     \ rf sqr0 val1 sqr0 rc

    \ Calc next result position in result array.
    4 mod                           \ rf sqr0 val1 sqr0 inx

    swap                            \ rf sqr0 val1 inx sqr0

    _square-set-result              \ rf sqr0

    \ Inc result count.
    dup _square-inc-result-count        \ rf sqr0

    \ Check for pn change.
    dup square-get-pn                   \ rf sqr0 pn

    \ Get new value
    over square-calc-pn                 \ rf sqr0 pn pn-new
    \ cr ." pn check " .s cr

    \ Check for change
    swap over <>                        \ rf sqr0 pn-new flag
    if
        \ Save new pn
        over _square-set-pn             \ rf sqr0

        dup square-get-rules            \ rf sqr0 rules
        rulestore-deallocate            \ rf sqr0

        dup square-calc-rules           \ rf sqr0 ruls
        over _square-set-rules          \ rf sqr0
        nip true swap                   \ rf sqr0
    else
        drop                            \ rf sqr0
    then

    dup square-get-pnc                  \ rf sqr0 pnc
    over square-calc-pnc                \ rf sqr0 pnc pnc-new
    swap over                           \ rf sqr0 pnc-new pnc pnc-new

    xor if
        swap _square-set-pnc            \ rf
        drop true                       \ rf
    else
        2drop
    then
;
