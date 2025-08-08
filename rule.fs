
23131 constant rule-id
    5 constant rule-struct-number-cells

\ Struct fields.
0 constant rule-header      \ 16-bits [0] struct id [1] use count.
rule-header cell+ constant rule-m00
rule-m00    cell+ constant rule-m01
rule-m01    cell+ constant rule-m11
rule-m11    cell+ constant rule-m10

0 value rule-mma    \ Storage for rule mma instance.

\ Init rule mma, return the addr of allocated memory.
: rule-mma-init ( num-items -- ) \ sets rule-mma.
    dup 1 < 
    if  
        ." rule-mma-init: Invalid number of items."
        abort
    then

    cr ." Initializing Rule store."
    rule-struct-number-cells swap mma-new to rule-mma
;

\ Check rule mma usage.
: assert-rule-mma-none-in-use ( -- )
    rule-mma mma-in-use 0<>
    if
        ." rule-mma use GT 0"
        abort
    then
;

\ Start accessors.

\ Return the m00 field of a rule instance.
: rule-get-m00 ( rul0 -- u)
    rule-m00 +      \ Add offset.
    @               \ Fetch the field.
;

\ Set the m00 field of a rule instance, use only in this file.
: _rule-set-m00 ( u1 rul0 -- )
    rule-m00 +      \ Add offset.
    !               \ Set the field.
;

\ Return the m01 field of a rule instance.
: rule-get-m01 ( rul0 -- u)
    rule-m01 +      \ Add offset.
    @               \ Fetch the field.
;

\ Set the m01 field of a rule instance, use only in this file. 
: _rule-set-m01 ( u1 rul0 -- )
    rule-m01 +      \ Add offset.
    !               \ Set the field.
;

\ Return the m11 field of a rule instance.
: rule-get-m11 ( rul0 -- u)
    rule-m11 +      \ Add offset.
    @               \ Fetch the field.
;

\ Set the m11 field of a rule instance, use only in this file. 
: _rule-set-m11 ( u1 rul0 -- )
    rule-m11 +      \ Add offset.
    !               \ Set the field.
;

\ Return the m10 field of a rule instance.
: rule-get-m10 ( rul0 -- u)
    rule-m10 +      \ Add offset.
    @               \ Fetch the field.
;

\ Set the m10 field of a rule instance, use only in this file. 
: _rule-set-m10 ( u1 rul0 -- )
    rule-m10 +      \ Add offset.
    !               \ Set the field.
;

\ End accessors.

\ Check instance type.

: is-allocated-rule ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup rule-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    rule-id =     
;

: is-not-allocated-rule ( addr -- flag )
    is-allocated-rule 0=
;

\ Check arg0 for rule, unconventional, leaves stack unchanged. 
: assert-arg0-is-rule ( rul0 -- )
    dup is-allocated-rule 0=
    if  
        cr ." arg0 is not an allocated rule."
        abort
    then
;

\ Check arg1 for rule, unconventional, leaves stack unchanged. 
: assert-arg1-is-rule ( rul1 ??? -- )
    over is-allocated-rule 0=
    if  
        cr ." arg1 is not an allocated rule."
        abort
    then
;

\ Allocate a rule, setting id and use count only, use only in this file. 

\ Allocate a rule, setting id and use count only, use only in this file. 
: _rule-allocate ( -- rul0 )
    \ Allocate space.
    rule-mma mma-allocate   \ addr

    \ Store id.
    rule-id over            \ addr id addr
    struct-set-id           \ addr

    \ Init use count.
    0 over struct-set-use-count \ addr
;

\ Create a rule from two numbers on the stack.
: rule-new ( u-result u-initial -- addr)
    \ Check u-initial.
    dup is-not-value
    if
        ." rule-new: u-initial is invalid"
        abort
    then

    \ Check u-result.
    over is-not-value
    if
        ." rule-new: u-result is invalid"
        abort
    then

    _rule-allocate          \ u-r u-i addr

    \ Store fields.
    over !not               \ u-r u-i addr u-i-not
    3 pick !not             \ u-r u-i addr u-i-not u-r-not
    and over                \ u-r u-i addr m00 addr
    _rule-set-m00           \ u-r u-i addr

    over !not               \ u-r u-i addr u-i-not
    3 pick                  \ u-r u-i addr u-i-not u-r
    and over                \ u-r u-i addr m01 addr
    _rule-set-m01           \ u-r u-i addr

    over                    \ u-r u-i addr u-i
    3 pick                  \ u-r u-i addr u-i u-r
    and over                \ u-r u-i addr m11 addr
    _rule-set-m11           \ u-r u-i addr

    over                    \ u-r u-i addr u-i
    3 pick !not             \ u-r u-i addr u-i u-r-not
    and over                \ u-r u-i addr m10 addr
    _rule-set-m10           \ u-r u-i addr

    \ Return result.
    nip nip                 \ addr
;

\ Push all masks onto stack.
: rule-get-masks ( rul0 -- m00 m01 m11 m10 )
    \ Check arg.
    assert-arg0-is-rule

    dup rule-get-m00 swap   \ m00 rule
    dup rule-get-m01 swap   \ m00 m01 rule
    dup rule-get-m11 swap   \ m00 m01 m11 rule
    rule-get-m10            \ m00 m01 m11 m10
;

\ Return true if two rules are equal.
: rule-eq ( rul1 rul0 -- flag )
    \ Check arg.
    assert-arg0-is-rule
    assert-arg1-is-rule

    \ Check m00
    2dup rule-get-m00           \ rul1 rul0 rul1 0m00
    swap rule-get-m00           \ rul1 rul0 0m00 1m00
    <> if
        2drop
        false
        exit
    then

    \ Check m01
    2dup rule-get-m01           \ rul1 rul0 rul1 0m01
    swap rule-get-m01           \ rul1 rul0 0m01 1m01
    <> if
        2drop
        false
        exit
    then

    \ Check m11
    2dup rule-get-m11           \ rul1 rul0 rul1 0m11
    swap rule-get-m11           \ rul1 rul0 0m11 1m11
    <> if
        2drop
        false
        exit
    then

    \ Check m10
    rule-get-m10                \ rul1 0m10
    swap rule-get-m10           \ 0m10 1m10
    <> if
        false
    else
        true
    then
;

\ Print a rule.
: .rule ( rul0 -- )
    \ Check arg.
    assert-arg0-is-rule

    \ Set up masks and most-significant-bit,
    \ the basis of each cycle.
    rule-get-masks      \ m00 m01 m11 m10
    domain-ms-bit-xt execute       \ m00 m01 m11 m10 ms |

    begin
        dup
    while               \ ms-bit is gt 0
                        \ m00 m01 m11 m10 ms |
        0               \ m00 m01 m11 m10 ms | 0

        \ Check m00
        5 pick          \ m00 m01 m11 m10 ms | 0 m00
        2 pick          \ m00 m01 m11 m10 ms | 0 m00 ms
        and             \ m00 m01 m11 m10 ms | 0 zero-or-non-zero
        if
            1+          \ m00 m01 m11 m10 ms | sum
        then
 
        \ Check m01
        4 pick          \ m00 m01 m11 m10 ms | sum m01
        2 pick          \ m00 m01 m11 m10 ms | sum m01 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            2 +         \ m00 m01 m11 m10 ms | sum
        then
 
        \ Check m11
        3 pick          \ m00 m01 m11 m10 ms | sum m11
        2 pick          \ m00 m01 m11 m10 ms | sum m11 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            4 +         \ m00 m01 m11 m10 ms | sum
        then
 
        \ Check m10
        2 pick          \ m00 m01 m11 m10 ms | sum m10
        2 pick          \ m00 m01 m11 m10 ms | sum m10 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            8 +         \ m00 m01 m11 m10 ms | sum
        then
 
        \ Print rule position.
        \ Of 4 masks, one or two can have a bit set and be valid.
        \ Not zero, three or four.
        case
            0  of ." 0?" endof
            1  of ." 00" endof
            2  of ." 01" endof
            3  of ." 0X" endof
            4  of ." 11" endof
            5  of ." XX" endof
            6  of ." X1" endof
            7  of ." 3?" endof
            8  of ." 10" endof
            9  of ." X0" endof
            10 of ." Xx" endof
            11 of ." 3?" endof
            12 of ." 1X" endof
            13 of ." 3?" endof
            14 of ." 3?" endof
            15 of ." 4?" endof
        endcase

        1 rshift        \ shift ms bit right one position.
        dup if
            ." /"
        then
    repeat
    \ m00 m01 m11 m10 0
    2drop 2drop drop
;

\ Deallocate a rule.
: rule-deallocate ( rul0 -- )
    \ Check arg.
    assert-arg0-is-rule

    dup struct-get-use-count      \ rule-addr count

    2 <
    if
        \ Clear fields.
        0 over _rule-set-m00
        0 over _rule-set-m01
        0 over _rule-set-m11
        0 over _rule-set-m10

        \ Deallocate instance.
        rule-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return rule initial region.
: rule-initial-region ( rul0 -- reg0 )
    \ Check arg.
    assert-arg0-is-rule

    rule-get-masks      \ m00 m01 m11 m10
    or -rot             \ most-ones m00 m01
    or !not             \ most-ones most-zeros
    region-new
;

\ Return rule result region.
: rule-result-region ( rul0 -- reg0 )
    \ Check arg.
    assert-arg0-is-rule

    rule-get-masks      \ m00 m01 m11 m10
    -rot                \ m00 m10 m01 m11
    or -rot             \ most-ones m00 m01
    or !not             \ most-ones most-zeros
    region-new
;

\ Return true if two rules intersect.
: rule-intersects ( rul1 rul0 -- flag )
    \ Check arg.
    assert-arg0-is-rule
    assert-arg1-is-rule

    \ Get rules initial regions.
    rule-initial-region swap    \ initial-0 rul1
    rule-initial-region         \ initial-0 initial-1
    2dup                        \ initial-0 initial-1 initial-0 initial-1

    \ Calc result.
    region-intersects           \ initial-0 initial-1 flag

    \ Cleanup.
    swap region-deallocate      \ initial-0  flag
    swap region-deallocate      \ flag
;

\ Return true if all bit positions in a rule are represented.
: rule-all-bits-set ( rul0 -- flag )
    \ Check arg.
    assert-arg0-is-rule

    \ Or all mask bits.
    rule-get-masks          \ m00 m01 m11 m10
    or or or                \ m-all

    \ Check that all bit positions are used.
    domain-all-bits-xt execute =
;

\ Return the valid result of a rule intersection, or false.
\ A valid intersection may not have the same initial region as the intersection
\ of the two rules initial regions.
\ As X1 & Xx = 01, X1 & XX = 11, X0 & Xx = 10, X0 & XX = 00.
: rule-intersection ( rul1 rul0 -- result true | false )
    \ Check arg.
    assert-arg0-is-rule
    assert-arg1-is-rule

    \ Check for non-intersection of initial regions.
    2dup rule-intersects    \ rul1 rul0 flag
    0= if
        2drop
        false
        exit
    then

    \ Intersect m00         \ rul1 rul0
    dup rule-get-m00        \ rul1 rul0 | 0m00
    2 pick rule-get-m00     \ rul1 rul0 | 0m00 1m00 
    and                     \ rul1 rul0 | m00
    -rot                    \ m00 | rul1 rul0

    \ Intersect m01         \ m00 | rul1 rul0
    dup rule-get-m01        \ m00 | rul1 rul0 | 0m01
    2 pick rule-get-m01     \ m00 | rul1 rul0 | 0m01 1m01 
    and                     \ m00 | rul1 rul0 | m01
    -rot                    \ m00 m01 | rul1 rul0

    \ Intersect m11         \ m00 m01 | rul1 rul0
    dup rule-get-m11        \ m00 m01 | rul1 rul0 | 0m11
    2 pick rule-get-m11     \ m00 m01 | rul1 rul0 | 0m11 1m11 
    and                     \ m00 m01 | rul1 rul0 | m11
    -rot                    \ m00 m01 m11 | rul1 rul0

    \ Intersect m01         \ m00 m01 m11 | rul1 rul0
    rule-get-m01            \ m00 m01 m11 | rul1 0m01
    swap rule-get-m01       \ m00 m01 m11 | 0m11 1m01 
    and                     \ m00 m01 m11 m10

    \ Start new rule.
    _rule-allocate          \ m00 m01 m11 m10 rul

    \ Set each field.
    swap over _rule-set-m10 \ m00 m01 m11 rul
    swap over _rule-set-m11 \ m00 m01 rul
    swap over _rule-set-m01 \ m00 rul
    swap over _rule-set-m00 \ rul

    \ Check rule.
    dup rule-all-bits-set       \ rul flag

    if
        true                    \ rul flag
    else
        rule-deallocate
        false
    then
;

\ Prune 0->X and 1->X positions in a rule.
\
\ An invalid union may be partially valid with additional massaging:
\    X1 + XX = 11 (discard 0X)
\    X1 + Xx = 01 (discard 1X)
\    X0 + XX = 00 (discard 1X)
\    X0 + Xx = 10 (discard 0X)
\ Then call rule-all-bits-set, to see if the result is valid.
\ : _rule-prune ( rul0 - rul0 )
\    \ Check arg.
\    assert-arg0-is-rule
\
\    dup rule-get-masks          \ rul0 m00 m01 m11 m10
\
\    \ Get 1->X and 0->X positions.
\    and -rot and                \ rul0 m1X m0X
\
\    dup if                      \ rul0 m1X m0X
\        \ m0x is not zero.
\
\        \ Get bits that are not 0X.
\        !not                    \ rul0 m1x n0x
\
\        \ Adjust m00.
\        2 pick                  \ rul0 m1x n0x rul0
\        rule-get-m00            \ rul0 m1x n0x m00
\        over and                \ rul0 m1x n0x new-m00
\        3 pick                  \ rul0 m1x n0x new-m00 rul0
\        _rule-set-m00           \ rul0 m1x n0x
\         
\        \ Adjust m01.
\        2 pick                  \ rul0 m1x n0x rul0
\        rule-get-m01            \ rul0 m1x n0x m01
\        and                     \ rul0 m1x new-m01
\        2 pick                  \ rul0 m1x new-m01 rul0
\        _rule-set-m01           \ rul0 m1x
\
\    else                        \ rul0 m1x 0
\        drop                    \ rul0 m1x
\    then
\    
\    dup if                      \ rul0 m1X
\        \ m1x is not zero.
\
\        \ Get bits that are not 1X.
\        !not                    \ rul0 n1x
\
\        \ Adjust m11.
\        over                    \ rul0 n1x rul0
\        rule-get-m11            \ rul0 n1x m11
\        over and                \ rul0 n1x new-m11
\        2 pick                  \ rul0 n1x new-m11 rul0
\        _rule-set-m11           \ rul0 n1x 
\         
\        \ Adjust m10.
\        over                    \ rul0 n1x rul0
\        rule-get-m10            \ rul0 n1x m10
\        and                     \ rul0 new-m10
\        over                    \ rul0 new-m10 rul0
\        _rule-set-m10           \ rul0
\
\    else                        \ rul0 0
\        drop                    \ rul0
\    then
\ ;

\ Return the valid result of a rule union, or false.
: rule-union ( rul1 rul0 -- result true | false )
    \ Check args.
    assert-arg0-is-rule
    assert-arg1-is-rule

    \ Combine m00           \ rul1 rul0
    over rule-get-m00       \ rul1 rul0 | 1m00
    over rule-get-m00       \ rul1 rul0 | 1m00 0m00 
    or                      \ rul1 rul0 | m00
    -rot                    \ m00 | rul1 rul0

    \ Combine m01           \ m00 | rul1 rul0
    over rule-get-m01       \ m00 | rul1 rul0 | 1m01
    over rule-get-m01       \ m00 | rul1 rul0 | 1m01 0m01 
    or                      \ m00 | rul1 rul0 | m01
    -rot                    \ m00 m01 | rul1 rul0

    \ Combine m11           \ m00 m01 | rul1 rul0
    over rule-get-m11       \ m00 m01 | rul1 rul0 | 1m11
    over rule-get-m11       \ m00 m01 | rul1 rul0 | 1m11 0m11 
    or                      \ m00 m01 | rul1 rul0 | m11
    -rot                    \ m00 m01 m11 | rul1 rul0

    \ Combine m10           \ m00 m01 m11 | rul1 rul0
    rule-get-m10            \ m00 m01 m11 | rul1 0m10
    swap rule-get-m10       \ m00 m01 m11 | 0m10 1m10 
    or                      \ m00 m01 m11 m10

    \ Start new rule.
    _rule-allocate          \ m00 m01 m11 m10 rul

    \ Set each field.
    swap over _rule-set-m10 \ m00 m01 m11 rul
    swap over _rule-set-m11 \ m00 m01 rul
    swap over _rule-set-m01 \ m00 rul
    swap over _rule-set-m00 \ rul

    \ Check rule.

    \ _rule-prune             \ rul
    \ dup rule-all-bits-set   \ rul flag
    \ if
    \    true                 \ rul flag
    \ else
    \     rule-deallocate
    \     false
    \ then

    \ Check for 0->X.
    dup rule-get-m00
    over rule-get-m01
    and
    if
        rule-deallocate
        false
        exit
    then

    \ Check for 1->X.
    dup rule-get-m11
    over rule-get-m10
    and
    if
        rule-deallocate
        false
    else
        true
    then
;

\ Return a rule restricted to an intersecting initial region.
: rule-restrict-initial-region ( reg1 rul0 -- rul )
    \ Check args.
    assert-arg0-is-rule
    assert-arg1-is-rule

    swap                        \ rul0 reg1
    over rule-initial-region    \ rul0 reg1 reg-initial
    2dup                        \ rul0 reg1 reg-initial reg1 reg-initial
    region-intersects           \ rul0 reg1 reg-initial flag
    0= if
        ." rule-restrict-initial-region: Region does not intersect?"
        abort
    then
                                \ rul0 reg1 reg-initial
    region-deallocate           \ rul0 reg1
    
    dup region-high-state swap  \ rul0 high reg1
    region-low-state            \ rul0 high low

    !not                        \ rul0 ones zeros
    binary
    ." ones: " over . cr
    ." zereos: " dup . cr
    decimal

    2 pick rule-get-m00         \ rul0 ones zeros | m00
    over and                    \ rul0 ones zeros | n00
    3 pick rule-get-m01         \ rul0 ones zeros | n00 m01
    rot and                     \ rul0 ones n00 n01
    2swap                       \ n00 n01 rul0 ones
    
    over rule-get-m11           \ n00 n01 rul0 ones m11
    over and                    \ n00 n01 rul0 ones n11
    -rot                        \ n00 n01 n11 rul0 ones
    swap                        \ n00 n01 n11 ones rul0
    rule-get-m10 and            \ n00 n01 n11 n10

    _rule-allocate              \ n00 n01 n11 n10 rul

    swap over                   \ n00 n01 n11 rul r10 rul
    _rule-set-m10               \ n00 n01 n11 rul

    swap over                   \ n00 n01 rul n11 rul
    _rule-set-m11               \ n00 n01 rul

    swap over                   \ n00 rul n01 rul
    _rule-set-m01               \ n00 rul

    swap over                   \ rul n00 rul
    _rule-set-m00               \ rul
;

\ Return a rule restricted to an intersecting result region.
: rule-restrict-result-region ( reg1 rul0 -- rul )
    \ Check args.
    assert-arg0-is-rule
    assert-arg1-is-rule

    swap                        \ rul0 reg1
    over rule-result-region     \ rul0 reg1 reg-result
    2dup                        \ rul0 reg1 reg-result reg1 reg-result 
    
    region-intersects           \ rul0 reg1 reg-result flag
    0= if
        ." rule-restrict-result-region: Region does not intersect?"
        abort
    then
                                \ rul0 reg1 reg-result
    region-deallocate           \ rul0 reg1

    dup region-high-state swap  \ rul0 high reg1
    region-low-state            \ rul0 high low

    !not                        \ rul0 ones zeros

    2 pick rule-get-m00         \ rul0 ones zeros | m00
    over and                    \ rul0 ones zeros | n00
    3 pick rule-get-m10         \ rul0 ones zeros | n00 m10
    rot and                     \ rul0 ones n00 n10
    2swap                       \ n00 n10 rul0 ones
    
    over rule-get-m11           \ n00 n10 rul0 ones m11
    over and                    \ n00 n10 rul0 ones n11
    -rot                        \ n00 n10 n11 rul0 ones
    swap                        \ n00 n10 n11 ones rul0
    rule-get-m01 and            \ n00 n10 n11 n01

    _rule-allocate              \ n00 n10 n11 n01 rul

    swap over                   \ n00 n10 n11 rul r01 rul
    _rule-set-m01               \ n00 n10 n11 rul

    swap over                   \ n00 n10 rul n11 rul
    _rule-set-m11               \ n00 n10 rul

    swap over                   \ n00 rul n10 rul
    _rule-set-m10               \ n00 rul

    swap over                   \ rul n00 rul
    _rule-set-m00               \ rul
;


