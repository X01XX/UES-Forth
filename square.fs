\ Implement a square struct and functions.

#23197 constant square-id                                                                                  
    #7 constant square-struct-number-cells

\ Struct fields
0 constant square-header                        \ id (16) use count (16) result count (16) pnc (8)
square-header   cell+ constant square-state
square-state    cell+ constant square-rules     \ A Rulestore.
square-rules    cell+ constant square-results   \ Circular buffer of 4 cells, starting here.
                                                \ The result count, mod 4, will be the next element to use
                                                \ for a new result.

0 value square-mma \ Storage for square mma instance.

\ Init square mma, return the addr of allocated memory.
: square-mma-init ( num-items -- ) \ sets square-mma.
    dup 1 < 
    abort" square-mma-init: Invalid number of items."

    cr ." Initializing Square store."
    square-struct-number-cells swap mma-new to square-mma
;

\ Check square mma usage.
: assert-square-mma-none-in-use ( -- )
    square-mma mma-in-use 0<>
    abort" square-mma use GT 0"
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

\ Check TOS for square, unconventional, leaves stack unchanged. 
: assert-tos-is-square ( arg0 -- arg0 )
    dup is-allocated-square 0=
    abort" TOS is not an allocated square"
;

\ Check NOS for square, unconventional, leaves stack unchanged. 
: assert-nos-is-square ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-square 0=
    abort" NOS is not an allocated square"
;

\ Check nos is a valid pn value.
: assert-nos-is-pn ( pn1 arg0 -- )
    over 1 <
    abort" nos is not a valid pn value"

    over #3 >
    abort" nos is nat a valid pn value"
;

\ Start accessors.

\ Return result count from the square header.
: square-get-result-count ( square-addr -- u-length )
    \ Check arg.
    assert-tos-is-square

    2w@ 
;

\ Set square result count, use only in this file.
: _square-set-result-count ( length-value sqr0 -- )
    2w! 
;

\ Increment square result count, use only in this file.
: _square-inc-result-count ( sqr0 -- )
    dup square-get-result-count      \ struct-addr result-count
    1+
    swap _square-set-result-count
;

\ Return the square state. 
: square-get-state ( addr -- u )
    \ Check arg.
    assert-tos-is-square

    square-state +      \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the state of a square instance, use only in this file.
: _square-set-state ( u1 addr -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-value

    square-state +      \ Add offset.
    !                   \ Set field.
;

\ Return square 8-bit pnc value, as a bool.
: square-get-pnc ( sqr0 -- bool )
    \ Check arg.
    assert-tos-is-square

    6c@
    0<>     \ Change 255 to -1
;

: _square-set-pnc ( pnc sqr -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-bool

    6c!
;

: square-get-rules ( sqr0 -- rulstr )
    \ Check arg.
    assert-tos-is-square

    square-rules + @
;

: _square-set-rules ( rulstr1 sqr0 -- )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-rulestore
    
    over struct-inc-use-count

    square-rules + !
;

: square-get-pn ( sqr0 -- pn )
    \ Check arg.
    assert-tos-is-square

    square-get-rules
    rulestore-get-pn

\    6c@
;

\ Replace old rules with new rules.
\ Deallocate old rules last, so the square instance field
\ is never invalid.
: _square-update-rules ( new-ruls1 sqr0 -- )
    \ Check arg.
    assert-tos-is-square
    assert-nos-is-rulestore

    \ Get\save old rules.
    dup square-get-rules        \ new-ruls1 sqr0 old-ruls
    -rot                        \ old-ruls new-rels1 sqr0

    \ Set new-rules
    _square-set-rules           \ old-ruls

    \ Dealloc old rules.
    rulestore-deallocate        \
;

\ Get results item, given index.
: square-get-result ( index1 sqr0 -- result )
    \ Check arg.
    assert-tos-is-square

    over dup                    \ i1 s0 i i
    0< swap #3 > or              \ i1 s0 flag
    abort" invalid index"

    \ Point to results array.
    square-results +            \ addr

    \ Point to item in array
    swap cells +                \ addr
    @
;

\ Set results item, given index and result, use only in this file.
: _square-set-result ( result2 index1 sqr0 -- )
    cr ." sqr " dup square-get-state .value
    space ." set result # " over dec.
    space ." to " #2 pick .value cr

    over dup                    \ r2 i1 s0 i i
    0< swap #3 > or             \ r2 i1 s0 flag
    abort" invalid index"

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
    assert-tos-is-value
    assert-nos-is-value

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
    #2 pick #2 pick                     \ r s addr r s
    rule-new                            \ r s addr rul
    rulestore-new-1                     \ r s addr rulstr
    over _square-set-rules              \ r s addr

    \ Set state
    tuck _square-set-state      \ r addr

    \ Set first result.
    tuck                        \ addr r addr
    square-results +            \ addr r addr+
    !                           \ addr

    \ Set pnc value.
    0 over _square-set-pnc      \ addr
;

: square-from-sample ( smpl -- sqr )
    \ Check arg.
    assert-tos-is-sample

    dup sample-get-result
    swap sample-get-initial
    square-new
;

: square-deallocate ( sqr0 -- )
    \ Check arg.
    assert-tos-is-square

    dup struct-get-use-count      \ sqr0 count

    #2 <
    if
        \ Deallocate instance.
        dup square-get-rules
        rulestore-deallocate
        square-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Print a pn value.
: .pn ( pn -- )
    case
        1 of ." 1" endof
       #2 of ." 2" endof
       #3 of ." U" endof
        ." Unexpected pn value"
        abort
    endcase
;

\ Print a pnc value.
: .pnc ( pnc -- )
    if
        ." T"
    else
        ." F"
    then
;

\ Print a square.
: .square ( sqr0 -- )
    \ Check arg.
    assert-tos-is-square

    dup square-get-state .value

    ."  pnc: "
    dup square-get-pnc .pnc

    ."  pn: "
    dup square-get-pn .pn

    ."  "
    dup square-get-rules
    .rulestore

    \ sqr0
    ."  rc "
    square-get-result-count .
;

: .square-state ( sqr -- )
    \ Check arg.
    assert-tos-is-square

    square-get-state .value space
;

\ Calc a pnc value for a square.
\ The most recent four consecutive samples is the whole sample Universe.
\ Four is the minimum number for seeing 2 different results, twice.
: _square-calc-pnc ( sqr0 -- bool )
    dup square-get-result-count     \ sqr0 count
    swap square-get-pn              \ count pn

    #3 =
    if
        drop
        true
    else
        #3 >
    then
;

\ Return a rule constructed a square state and the first result.
: _square-calc-rule-0 ( sqr0 -- rul )
    0 over square-get-result    \ sqr0 r0
    swap square-get-state       \ r0 sta
    rule-new
;

\ Return a rule constructed a square state and second result.
: _square-calc-rule-1 ( sqr0 -- rul )
    1 over square-get-result    \ sqr0 r1
    swap square-get-state       \ r1 sta
    rule-new
;

\ Return pn for a square, calculated from the result array.
: _square-calc-pn ( sqr0 -- pn )

    dup square-get-result-count             \ sqr0 rslt-cnt

    dup 1 =
    if                              \ sqr0 1
        nip
        exit
    then

    dup #2 =
    if                              \ sqr0 2
        drop                        \ sqr0
        \ Check if r0 == r1, pn == 1 or 2.
        0 over square-get-result        \ sqr0 r0
        swap                            \ r0 sqr0
        1 swap square-get-result        \ r0 r1
        = if                            \
            1                           \ 1
        else                            \
            #2                          \ 2
        then
        exit
    then

    dup #3 =
    if                                  \ sqr0 3
        drop                            \ sqr0
        \ For pn-1 and pn-2, r0 s/b == r2.
        0 over square-get-result        \ sqr0 r0
        over #2 swap square-get-result  \ sqr0 r0 r1
        <> if                           \ sqr0
            drop                        \
            #3                          \ 3
            exit
        then

        \ Check if r0 == r1, pn == 1 or 2.
        0 over square-get-result        \ sqr0 r0
        swap                            \ r0 sqr0
        1 swap square-get-result        \ r0 r1
        = if                            \
            1                           \ 1
        else                            \
            #2                          \ 2
        then
        exit
    then
    \ Result count > 3. 
                                        \ sqr0 >3
    drop                                \ sqr0

    \ For pn-1 and pn-2, r0 s/b == r2.
    0 over square-get-result            \ sqr0 r0
    over #2 swap square-get-result      \ sqr0 r0 r2
    <> if                               \ sqr0
        drop                            \
        #3                              \ 3
        exit
    then

    \ For pn-1 and pn-2, r1 s/b == r3.
    1 over square-get-result            \ sqr0 r1
    over #3 swap square-get-result      \ sqr0 r1 r3
    <> if                               \ sqr0
        drop                            \
        #3                              \ 3
        exit
    then

    \ Check if r0 == r1, pn == 1 or 2.
    0 over square-get-result        \ sqr0 r0
    swap                            \ r0 sqr0
    1 swap square-get-result        \ r0 r1
    = if                            \
        1                           \ 1
    else                            \
        #2                          \ 2
    then
;

\ Return a rulestore for a square, given a pn value.
: _square-calc-rules ( pn1 sqr0 -- rulestore )
    assert-tos-is-square
    assert-nos-is-pn

    swap                            \ sqr0 pn1
    case
    1 of                            \ sqr0
        \ Form a rule from the first result.
        0 swap                      \ 0 sqr0
        tuck square-get-result      \ sqr0 r0
        swap square-get-state       \ r0 s-sta
        rule-new                    \ rul
        \ Make rulestore from the rule.
        rulestore-new-1             \ rulstr
    endof
    #2 of                           \ sqr0
        \ Form a rule from the first result.
        0 over                      \ sqr0 0 sqr0
        square-get-result           \ sqr0 r0
        over square-get-state       \ sqr0 r0 s-sta
        rule-new                    \ sqr0 rul-0
        swap                        \ rul-0 sqr0
        \ Form a rule from the second result.
        1 over                      \ rul-0 sqr0 1 sqr0
        square-get-result           \ rul-0 sqr0 r1
        swap square-get-state       \ rul-0 r1 s-sta
        rule-new                    \ rul-0 rul-1
        \ Make rulestore from the rules.
        rulestore-new-2             \ rulstr
    endof
    #3 of
        drop
        rulestore-new-0             \ rulstr
    endof
    endcase
;

\ Add a new result to a square.
\ Return true if pn, or pnc changed.
: square-add-result ( val1 sqr0 -- flag )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-value

    \ Init return flag to false.
    false -rot                      \ rf val1 sqr0

    \ Store new result in square results array.
    tuck                            \ rf sqr0 val1 sqr0
    dup square-get-result-count     \ rf sqr0 val1 sqr0 rc
    #4 mod                          \ rf sqr0 val1 sqr0 inx, Calc next result position in result array.
    swap                            \ rf sqr0 val1 inx sqr0
    _square-set-result              \ rf sqr0
    dup _square-inc-result-count    \ rf sqr0, inc result count.

    \ Check for pn change, update rules, and return flag, if needed.
    dup square-get-pn                   \ rf sqr0 pn
    over _square-calc-pn                \ rf sqr0 pn pn-new
    tuck                                \ rf sqr0 pn-new pn pn-new
    =                                   \ rf sqr0 pn-new flag
    if                                  \ rf sqr0 pn-new
        drop                            \ rf sqr0
    else                                \ rf sqr0 pn-new
        \ Handle pn change.
        cr  ." Dom: " cur-domain-xt execute domain-get-inst-id-xt execute dec. space
            ." Act: " cur-action-xt execute action-get-inst-id-xt execute dec. space
            over square-get-state
            ." square " .value
            over square-get-pn
            space ." pn changed from " .pn
            space ." to " dup .pn cr

        \ Make new rules.
        over _square-calc-rules         \ rf sqr0 ruls-new

        \ Update rules.
        over _square-update-rules       \ rf sqr0

        \ Set return flag to true.
                                        \ rf sqr0
        nip true swap                   \ rf sqr0
    then

    \ Check for pnc change, update pnc, and return flag, if needed.
                                        \ rf sqr0
    dup square-get-pnc                  \ rf sqr0 pnc

    over _square-calc-pnc               \ rf sqr0 pnc pnc-new
    tuck =                              \ rf sqr0 pnc-new flag
    if                                  \ rf sqr0 pnc-new
        2drop                           \ rf
    else
        \ Update pnc.
        swap _square-set-pnc            \ rf
        \ Set return flag to true.
        drop true                       \ rf
    then
;

\ Add a result from a sample.
\ Return true if pn, or pnc changed.
: square-add-sample ( smpl1 sqr0 -- flag )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-sample

    over sample-get-initial     \ smpl1 sqr0 initial
    over square-get-state       \ smpl1 sqr0 initial state
    <>
    abort" Sample initial does not match square state"

    swap sample-get-result      \ sqr0 result
    swap                        \ result sqr0
    square-add-result           \
;

\ Return true if two squares ar equal.
: square-eq ( sqr1 sqr0 -- flag )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-square

    square-get-state
    swap
    square-get-state
    =
;

\ Compare two squares, TOS has pn 1, NOS has pn 1. 
: _square-compare-pn-1-1 ( sqr1 sqr0 -- char )
    square-get-rules rulestore-get-rule-0   \ sqr1 rul0
    swap                                    \ rul0 sqr1
    square-get-rules rulestore-get-rule-0   \ rul0 rul1
    rule-union                              \ rul-u true | false
    if
        rule-deallocate
        [char] C
    else
        [char] I
    then
;

\ Compare two squares, TOS has pn 2, NOS has pn 1.
: _square-compare-pn-1-2 ( sqr1 sqr2 -- char )
    \ If pn 1 and results GT 1, then it is not pn 2.
    over square-get-result-count        \ sqr1 sqr2 | uc1
    1 >
    if
        2drop
        [char] I
        exit
    then

    \ Get rule from pn 1 square.
                                                \ sqr1 sqr2 |
    over square-get-rules rulestore-get-rule-0  \ sqr1 sqr2 | s1-r0

    \ Check first rule of pn-2 square.
    over square-get-rules rulestore-get-rule-0  \ sqr1 sqr2 | s1-r0 s2-r0
    over rule-union                             \ sqr1 sqr2 | s1-r0, rulx t | f
    if                                          \ sqr1 sqr2 | s1-r0 rulx
        rule-deallocate                         \ sqr1 sqr2 | s1-r0 
        true                                    \ sqr1 sqr2 | s1-r0 f0 
    else                                        \ sqr1 sqr2 | s1-r0
        false                                   \ sqr1 sqr2 | s1-r0 f0
    then
                                                \ sqr1 sqr2 | s1-r0 f0
    swap                                        \ sqr1 sqr2 | f0 s1-r0

    \ Check second rule of pn-2 square.
    #2 pick                                     \ sqr1 sqr2 | f0 s1-r0 sqr2
    square-get-rules rulestore-get-rule-1       \ sqr1 sqr2 | f0 s1-r0 s2-r1
    rule-union                                  \ sqr1 sqr2 | f0, rulx t | f
    if                                          \ sqr1 sqr2 | f0 rulx
        rule-deallocate                         \ sqr1 sqr2 | f0
        true                                    \ sqr1 sqr2 | f0 f1
    else                                        \ sqr1 sqr2 | f0
        false                                   \ sqr1 sqr2 | f0 f1
    then

    \ Combine the two flags. Two false, or two true, are false.
    xor                                         \ sqr1 sqr2 | f01

    \ Clean up.
    nip nip                                     \ f01

    \ Set result
    if
        [char] M
    else
        [char] I
    then
;

\ Compare two squares, TOS has pn 2, NOS has pn 2.
: _square-compare-pn-2-2 ( sqr1 sqr0 -- char )
    \ Get union OK by two different orders.
    square-get-rules                \ sqr1 sqr0-ruls
    swap square-get-rules           \ sqr0-ruls sqr1-ruls
    rulestore-union-2               \ rulstr true | false
    if
        rulestore-deallocate
        [char] C
    else
        [char] I
    then
;

\ Compare two squares, TOS has pn 3/U, NOS has pn 1 or 2.
: _square-compare-pn-1or2-3 ( sqr1 sqr0 -- char )
    drop
    _square-calc-pnc    \ bool
    if
        [char] I
    else
        [char] M
    then
;

\ Return char C = Compatible, I = Incompatible, M = More samples needed.
: square-compare ( sqr1 sqr0 -- char )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-square
    2dup square-eq
    abort" squares eq?"

    over square-get-pn      \ sqr1 sqr0 pn1
    over square-get-pn      \ sqr1 sqr0 pn1 pn0

    case
        1 of
            case
                1 of
                    _square-compare-pn-1-1
                endof
                #2 of
                    swap _square-compare-pn-1-2
                endof
                #3 of
                    swap _square-compare-pn-1or2-3
                endof
                ." Unexpected pn value"
                abort
            endcase
        endof
        #2 of
            case
                1 of
                    _square-compare-pn-1-2
                endof
                #2 of
                    _square-compare-pn-2-2
                endof
                #3 of
                    swap _square-compare-pn-1or2-3
                endof
                ." Unexpected pn value"
                abort
            endcase
        endof
        #3 of
            case
                1 of
                    _square-compare-pn-1or2-3
                endof
                #2 of
                    _square-compare-pn-1or2-3
                endof
                #3 of
                    2drop
                    [char] C
                endof
                ." Unexpected pn value"
                abort
            endcase
        endof
        ." Unexpected pn value"
        abort
    endcase
;

\ Return true if two squares are incompatible.
: square-incompatible ( sqr1 sqr0 -- flag )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-square

    2dup square-eq
    if
        2drop
        false
        exit
    then

    square-compare [char] I =
;

\ Return true if a square state matches a value.
: square-state-eq ( val1 sqr0 -- flag )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-value

    square-get-state
    =
;

\ Return true if a square-state is a subset of a region.
: square-state-in-region ( reg1 sqr0 -- flag )
    \ Check args.
    assert-tos-is-square
    assert-nos-is-region

    square-get-state
    swap
    region-superset-of-state
;

\ Return the last, that is most-recent, result.
: square-get-last-result ( sqr -- rslt )
    \ Check arg.
    assert-tos-is-square

    dup square-get-result-count     \ sqr cnt
    1- #4 mod                       \ sqr index
    swap                            \ inx sqr
    square-get-result               \ rslt
;
