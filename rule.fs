
#23131 constant rule-id
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
    abort" rule-mma-init: Invalid number of items."

    cr ." Initializing Rule store."
    rule-struct-number-cells swap mma-new to rule-mma
;

\ Check rule mma usage.
: assert-rule-mma-none-in-use ( -- )
    rule-mma mma-in-use 0<>
    abort" rule-mma use GT 0"
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

\ Check TOS for rule, unconventional, leaves stack unchanged. 
: assert-tos-is-rule ( rul0 -- )
    dup is-allocated-rule 0=
    abort" TOS is not an allocated rule."
;

\ Check NOS for rule, unconventional, leaves stack unchanged. 
: assert-nos-is-rule ( rul1 ??? -- )
    over is-allocated-rule 0=
    abort" NOS is not an allocated rule."
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
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

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
    assert-tos-is-rule

    dup rule-get-m00 swap   \ m00 rule
    dup rule-get-m01 swap   \ m00 m01 rule
    dup rule-get-m11 swap   \ m00 m01 m11 rule
    rule-get-m10            \ m00 m01 m11 m10
;

\ Return true if two rules are equal.
: rule-eq ( rul1 rul0 -- flag )
    \ Check arg.
    assert-tos-is-rule
    assert-nos-is-rule

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
    assert-tos-is-rule

    \ Set up masks and most-significant-bit,
    \ the basis of each cycle.
    rule-get-masks                      \ m00 m01 m11 m10
    cur-domain-xt execute               \ m00 m01 m11 m10 dom
    domain-get-ms-bit-mask-xt execute   \ m00 m01 m11 m10 ms

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
              0 of ." 0?" endof
              1 of ." 00" endof
              2 of ." 01" endof
              3 of ." 0X" endof
              4 of ." 11" endof
              5 of ." XX" endof
              6 of ." X1" endof
              7 of ." 3?" endof
              8 of ." 10" endof
              9 of ." X0" endof
            #10 of ." Xx" endof
            #11 of ." 3?" endof
            #12 of ." 1X" endof
            #13 of ." 3?" endof
            #14 of ." 3?" endof
            #15 of ." 4?" endof
        endcase

        1 rshift        \ shift ms bit right one position.
        ." /"
    repeat
    \ m00 m01 m11 m10 0
    2drop 2drop drop
;

\ Deallocate a rule.
: rule-deallocate ( rul0 -- )
    \ Check arg.
    assert-tos-is-rule

    dup struct-get-use-count      \ rule-addr count

    2 <
    if
        \ Deallocate instance.
        rule-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return rule initial region.
: rule-calc-initial-region ( rul0 -- reg0 )
    \ Check arg.
    assert-tos-is-rule

    rule-get-masks      \ m00 m01 m11 m10
    or -rot             \ most-ones m00 m01
    or !not             \ most-ones most-zeros
    region-new
;

\ Return rule result region.
: rule-calc-result-region ( rul0 -- reg0 )
    \ Check arg.
    assert-tos-is-rule

    rule-get-masks      \ m00 m01 m11 m10
    -rot                \ m00 m10 m01 m11
    or -rot             \ most-ones m00 m01
    or !not             \ most-ones most-zeros
    region-new
;

\ Return true if two rules intersect.
: rule-intersects ( rul1 rul0 -- flag )
    \ Check arg.
    assert-tos-is-rule
    assert-nos-is-rule

    \ Get rules initial regions.
    rule-calc-initial-region swap   \ initial-0 rul1
    rule-calc-initial-region        \ initial-0 initial-1
    2dup                            \ initial-0 initial-1 initial-0 initial-1

    \ Calc result.
    region-intersects           \ initial-0 initial-1 flag

    \ Cleanup.
    swap region-deallocate      \ initial-0  flag
    swap region-deallocate      \ flag
;

\ Return true if all bit positions in a rule are represented.
: rule-all-bits-set ( rul0 -- flag )
    \ Check arg.
    assert-tos-is-rule

    \ Or all mask bits.
    rule-get-masks          \ m00 m01 m11 m10
    or or or                \ m-all

    \ Check that all bit positions are used.
    cur-domain-xt execute               \ m-all dom
    domain-get-all-bits-mask-xt execute \ m-all msk
    =
;

\ Return the valid result of a rule intersection, or false.
\ A valid intersection may not have the same initial region as the intersection
\ of the two rules initial regions.
\ As X1 & Xx = 01, X1 & XX = 11, X0 & Xx = 10, X0 & XX = 00.
: rule-intersection ( rul1 rul0 -- result true | false )
    \ Check arg.$590F6CBD5980
    assert-tos-is-rule
    assert-nos-is-rule

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
    tuck _rule-set-m10      \ m00 m01 m11 rul
    tuck _rule-set-m11      \ m00 m01 rul
    tuck _rule-set-m01      \ m00 rul
    tuck _rule-set-m00      \ rul

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
\    assert-tos-is-rule
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
    assert-tos-is-rule
    assert-nos-is-rule

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
    tuck _rule-set-m10      \ m00 m01 m11 rul
    tuck _rule-set-m11      \ m00 m01 rul
    tuck _rule-set-m01      \ m00 rul
    tuck _rule-set-m00      \ rul

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
    assert-tos-is-rule
    assert-nos-is-region

    tuck                        \ rul0 reg1 rul0
    rule-calc-initial-region    \ rul0 reg1 reg-initial
    2dup                        \ rul0 reg1 reg-initial reg1 reg-initial
    region-intersects           \ rul0 reg1 reg-initial flag
    0= abort" rule-restrict-initial-region: Region does not intersect?"

                                \ rul0 reg1 reg-initial
    region-deallocate           \ rul0 reg1
    
    dup region-high-state swap  \ rul0 high reg1
    region-low-state            \ rul0 high low

    !not                        \ rul0 ones zeros
    \ cr ." ones: " over .value cr
    \ cr ." zereos: " dup .value cr

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

    tuck                        \ n00 n01 n11 rul r10 rul
    _rule-set-m10               \ n00 n01 n11 rul

    tuck                        \ n00 n01 rul n11 rul
    _rule-set-m11               \ n00 n01 rul

    tuck                        \ n00 rul n01 rul
    _rule-set-m01               \ n00 rul

    tuck                        \ rul n00 rul
    _rule-set-m00               \ rul
;

\ Return a rule restricted to an intersecting result region.
: rule-restrict-result-region ( reg1 rul0 -- rul )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    tuck                        \ rul0 reg1 reg0
    rule-calc-result-region     \ rul0 reg1 reg-result
    2dup                        \ rul0 reg1 reg-result reg1 reg-result 
    
    region-intersects           \ rul0 reg1 reg-result flag
    0= abort" rule-restrict-result-region: Region does not intersect?"
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

    tuck                        \ n00 n10 n11 rul r01 rul
    _rule-set-m01               \ n00 n10 n11 rul

    tuck                        \ n00 n10 rul n11 rul
    _rule-set-m11               \ n00 n10 rul

    tuck                        \ n00 rul n10 rul
    _rule-set-m10               \ n00 rul

    tuck                        \ rul n00 rul
    _rule-set-m00               \ rul
;

\ Add one to the m00 mask of a rule.
: rule-adjust-mask-m00 ( rul -- )
    dup rule-get-m00        \ rul m00
    1+
    swap                    \ m00 rul
    _rule-set-m00
;

\ Add one to the m01 mask of a rule.
: rule-adjust-mask-m01 ( rul -- )
    dup rule-get-m01        \ rul m01
    1+
    swap                    \ m01 rul
    _rule-set-m01
;

\ Add one to the m11 mask of a rule.
: rule-adjust-mask-m11 ( rul -- )
    dup rule-get-m11        \ rul m11
    1+
    swap                    \ m11 rul
    _rule-set-m11
;

\ Add one to the m10 mask of a rule.
: rule-adjust-mask-m10 ( rul -- )
    dup rule-get-m10        \ rul m10
    1+
    swap                    \ m10 rul
    _rule-set-m10
;

\ Shift all masks left by 1 bit position.
: rule-lshift ( rul -- )
    dup rule-get-m00 1 lshift over _rule-set-m00
    dup rule-get-m01 1 lshift over _rule-set-m01
    dup rule-get-m11 1 lshift over _rule-set-m11
    rule-get-m10 1 lshift over _rule-set-m10
;

: rule-adjust-masks ( ci cr rul0 -- )
    \ Check arg
    assert-tos-is-rule

    dup rule-lshift
    -rot                \ rul0 ci cr
    swap                \ rul0 cr ci
    
    \ Process two chars
    case
        [char] 0 of
            case
                [char] 0 of \ process 0->0
                    rule-adjust-mask-m00
                endof
                [char] 1 of \ process 0->1
                    rule-adjust-mask-m01
                endof
                cr ." unexpected char" abort
            endcase
        endof
        [char] 1 of
            case
                [char] 0 of \ process 1->0
                    rule-adjust-mask-m10
                endof
                [char] 1 of \ process 1->1
                    rule-adjust-mask-m11
                endof
                cr ." unexpected char" abort
            endcase
        endof
        [char] X of
            case
                [char] 0 of \ process X->0 (1->0, 0->0)
                    dup rule-adjust-mask-m00
                    rule-adjust-mask-m10
                endof
                [char] 1 of \ process X->1 (1->1, 0->1)
                    dup rule-adjust-mask-m11
                    rule-adjust-mask-m01
                endof
                [char] X of \ process X->X (1->1, 0->0)
                    dup rule-adjust-mask-m11
                    rule-adjust-mask-m00
                endof
                [char] x of \ process X->x (1->0, 0->1)
                    dup rule-adjust-mask-m10
                    rule-adjust-mask-m01
                endof
                cr ." unexpected char" abort
            endcase
        endof
        cr ." unexpected char" abort
    endcase
;

\ Return a rule from a string, like 00/01/11/10/XX/Xx/X0/X1/
\ The slash (/) characters, *including the last*, are important as separators.
\ The uderscore (_) character can also be used.
: rule-from-string ( addr n -- rule )
    \ Init rule.
    _rule-allocate                  \ addr n rul
    0 over _rule-set-m00
    0 over _rule-set-m01
    0 over _rule-set-m11
    0 over _rule-set-m10            \ addr n rul

    -rot                            \ rul addr n
    \ Check each pair of characters.
    0                               \ rul addr n 0
    do                              \ rul addr
        dup c@                      \ rul addr ci

        case
            [char] / of             \ rul ci cr addr
                -rot                \ rul addr ci cr
                3 pick              \ rul addr ci cr rul
                rule-adjust-masks   \ rul addr
            endof
            [char] _ of
                -rot                \ rul addr ci cr
                3 pick              \ rul addr ci cr rul
                rule-adjust-masks   \ rul addr
            endof
            tuck                    \ rul cx addr
        endcase
        1+
    loop
                                    \ rul addr+
    drop
;

: rule-get-changes ( rul0 -- cngs )
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m10 swap   \ m10 rul0
    rule-get-m01            \ m10 m01
    changes-new
;

\ Apply a rule to a given state, forward-chaining, returning a sample.
: rule-apply-to-state-f ( sta1 rul0 -- smpl true | false )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-value

    \ Check if the state is not in the rule initial region.
    2dup                            \ sta1 rul0 | sta1 rul0
    rule-calc-initial-region        \ sta1 rul0 | sta1 regx
    tuck                            \ sta1 rul0 | regx sta1 regx
    region-superset-of-state        \ sta1 rul0 | regx flag
    swap region-deallocate          \ sta1 rul0 | flag
    0= if
        2drop false exit
    then

    \ Get m10 mask that affects the given state.
                                    \ sta1 rul0
    over swap                       \ sta1 sta1 rul0
    2dup rule-get-m10 and           \ sta1 sta1 rul0 m10
    -rot                            \ sta1 m10 sta1 rul0
    rule-get-m01                    \ sta1 m10 sta1 r-m01
    swap invert and                 \ sta1 m10 m01
    or                              \ sta1 msk
    over xor                        \ sta1 sta2
    swap                            \ sta2 sta1
    sample-new                      \ smpl
    true
;

\ Return a forward step for a rule.
: rule-calc-forward-step ( smpl1 rul0 -- step true | false )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-sample

    \ Get a sample from rul0, and smpl1 initial state, if possible.
    over sample-get-initial     \ smpl1 rul0 | smp-i
    over rule-apply-to-state-f  \ smpl1 rul0 | smpl2 true | false
    0= if
        2drop false exit
    then

    \ Check if rule did not change the smpl1 initial state.
    dup sample-r-ne-i           \ smpl1 rul0 | smpl2 flag
    0= if
        sample-deallocate
        2drop false exit
    then

    \ Check if the smpl2 result is beween the smpl1 initial and result, inclusive,
    \ except we know its not EQ smpl1 initial.
    dup sample-get-result       \ smpl1 rul0 | smpl2 smp2-r
    3 pick                      \ smpl1 rul0 | smpl2 smp2-r smpl1
    sample-state-between        \ smpl1 rul0 | smpl2 flag
    0= if
        sample-deallocate
        2drop false exit
    then

    \ Make a step
    0 swap                      \ smpl1 rul0 | 0 smpl2
    cur-action-xt execute       \ smpl1 rul0 | 0 smpl2 actx
    step-new-xt execute         \ smpl1 rul0 | stpx
    nip nip
    true
;

\ Return true if a rule changes at least one bit.
: rule-makes-change ( rul0 -- flag )
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m01        \ rul0 m01
    0<> if
        drop
        true
        exit
    then

    rule-get-m10            \ m10
    0<>
;

\ Ruturn a rule restricted, initial and result regions, to a given
\ region, and the rule still changes at least one bit.
: rule-restrict-to-region ( reg1 rul0 -- rul true | false )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    \ Check if reg1 intersects the rule initial region.
    2dup                            \ reg1 rul0 reg1 rul0
    rule-calc-initial-region        \ reg1 rul0 reg1 reg-i
    tuck                            \ reg1 rul0 reg-i reg1 reg-i
    region-intersects               \ reg1 rul0 reg-i flag
    swap region-deallocate          \ reg1 rul0 flag
    0= if
        2drop
        false
        exit
    then

    \ Restrict the rules' initial region.
    2dup                            \ reg1 rul0 reg1 rul0
    rule-restrict-initial-region    \ reg1 rul0 rul'

    \ Check if the restricted rules' result region intersects reg1.
    dup rule-calc-result-region     \ reg1 rul0 rul' reg-r
    dup 4 pick                      \ reg1 rul0 rul' reg-r reg-r reg1
    region-intersects               \ reg1 rul0 rul' reg-r flag
    swap region-deallocate          \ reg1 rul0 rul' flag
    0= if
        rule-deallocate
        2drop
        false
        exit
    then

    \ Check if reg1 is a superset of the restricted rules' result region.
    dup rule-calc-result-region     \ reg1 rul0 rul' reg-r
    dup 4 pick                      \ reg1 rul0 rul' reg-r reg-r reg1
    region-superset-of              \ reg1 rul0 rul' reg-r flag
    swap region-deallocate          \ reg1 rul0 rul' flag
    0= if
        nip nip                     \ rul'
        dup rule-makes-change       \ rul' flag
        if
            true
        else
            rule-deallocate
            false
        then
        exit
    then

    \ Restrict the restricted rules' result region to reg1.
                                    \ reg1 rul0 rul'
    nip                             \ reg1 rul'
    tuck                            \ rul' reg1 rul'
    rule-restrict-result-region     \ rul' rul''

    \ Clean up.
    swap rule-deallocate            \ rul''

    \ Return.
    dup rule-makes-change           \ rul' flag
    if
        true
    else
        rule-deallocate
        false
    then
;

\ Apply a rule to a given state, backward-chaining, returning a sample.
\ For X->0, the result will be 1->0.
\ For X->1, the result will be 0->1.
: rule-apply-to-state-b ( sta1 rul0 -- smpl true | false )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-value

    \ Check if the state is not in the rule result region.
    2dup                            \ sta1 rul0 | sta1 rul0
    rule-calc-result-region         \ sta1 rul0 | sta1 regx (dl)
    tuck                            \ sta1 rul0 | regx sta1 regx
    region-superset-of-state        \ sta1 rul0 | regx flag
    swap region-deallocate          \ sta1 rul0 | flag
    0= if
        2drop false exit
    then

    \ Get m10 mask that affects the given state.
                                    \ sta1 rul0
    over swap                       \ sta1 sta1 rul0
    over invert                     \ sta1 sta1 rul0 sta1'
    over rule-get-m10 and           \ sta1 sta1 rul0 m10'
    -rot                            \ sta1 m10' sta1 rul0action-get-steps-by-changes

    \ Get m01 mask that affects the given state.
    rule-get-m01                    \ sta1 m10 sta1 r-m01
    and                             \ sta1 m10 m01
    or                              \ sta1 msk
    over xor                        \ sta1 sta2
    sample-new                      \ smpl
    true
;

\ Return a backward step for a rule.
\ The rule is restricted to the region formed by the sample states,
\ to manage X->0 and X->1 rule bit positions.
: rule-calc-backward-step ( smpl1 rul0 -- step true | false )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-sample

    \ cr ." rule-calc-backward-step: " dup .rule space over .sample cr

    \ Restrict the possible rule initial state to the glidepath.
    \ Affects the rule result region.
    over sample-to-region               \ smpl1 rul0 s-reg
    tuck swap                           \ smpl1 s-reg s-reg rul0
    rule-restrict-to-region             \ smpl1 s-reg, rul0' t | f
    if
        swap region-deallocate          \ smpl1 rul0'
    else
        region-deallocate
        drop
        false
        exit
    then

    \ Check if rule may apply to the sample.
    dup rule-calc-result-region         \ smpl1 rul0' r-reg (dl)
    2 pick sample-get-result            \ smpl1 rul0' r-reg s-r
    over region-superset-of-state       \ smpl1 rul0' r-reg flag
    swap region-deallocate              \ smpl1 rul0' flag
    0= if
        rule-deallocate
        drop
        false
        exit
    then

    \ Get initial value from rule.
                                        \ smpl1 rul0'
    over sample-get-result              \ smpl1 rul0' s-r
    over                                \ smpl1 rul0' s-r rul0'
    rule-apply-to-state-b               \ smpl1 rul0', smpl2 t | f
    0= if
        rule-deallocate
        drop
        false
        exit
    then
    swap rule-deallocate                \ smpl1 smpl2

    \ Check if rule did not change the smpl1 result state.
    dup sample-r-ne-i                   \ smpl1 smpl2 flag
    0= if
        sample-deallocate
        drop
        false
        exit
    then

    \ Make step.
    nip 0 swap              \ 0 smpl2
    cur-action-xt execute   \ 0 smpl2 actx
    step-new-xt execute     \ stpx
    true
;

\ Return true if a rules' changes intersect at least one bit of a changes instance.
: rule-instersects-changes ( cngs1 rul0 -- flag )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes
    \ Check changes m01 + m10, not zero
    over changes-get-masks or 0= abort" changes cannot be zero"

    over changes-get-m01    \ cngs1 rul0 cm01
    over rule-get-m01       \ cngs1 rul0 cm01 rm01
    and 0<> if              \ cngs1 rul0
        2drop
        true
        exit
    then

    rule-get-m10            \ cngs1 rm10
    swap changes-get-m10    \ rm10 cm10
    and 0<>
;

\ Restrict a rule to a changes instance.
\ Xs in the rules' initial region, corresponding to a changes bit,
\ will become non-X.
\ For a 0 bit in a state, corresponding to a X->1 bit in a rule,
\ you don't need 0->1 then X->0, 0->0 will do.
: rule-restrict-to-changes  ( cngs1 rul0 -- rul )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes
    \ Check cm10 & cm01 is zero.
    over changes-get-masks and 0<> abort" changes cannot be doubled up"

    \ Init return rule.
    0 0 rule-new                \ cngs1 rul0 rrule-test-restrict-to-regionul' |

    \ Get m01 set in changes and rule.
    2 pick changes-get-m01      \ | cm01
    2 pick rule-get-m01         \ | cm01 r01
    and                         \ | m01'

    \ Invert to mask out bits.
    invert                      \ | ~m01'

    \ Calc return rule m10.
    2 pick rule-get-m10         \ | ~m01' rm10
    over and                    \ | ~m01' rm10'
    2 pick _rule-set-m10        \ | ~m01'

    \ Calc return rule m11.
    2 pick rule-get-m11         \ | ~m01' rm11
    and                         \ | rm11'
    over _rule-set-m11          \ |

    \ Get m10 set in changes and rule.
    2 pick changes-get-m10      \ | cm10
    2 pick rule-get-m10         \ | cm10 r10
    and                         \ | m01'

    \ Invert to mask out bits.
    invert                      \ | ~m10'

    \ Calc return rule m01.
    2 pick rule-get-m01         \ | ~m10' rm01
    over and                    \ | ~m10' rm01'
    2 pick _rule-set-m01        \ | ~m10'

    \ Calc return rule m00.
    2 pick rule-get-m00         \ | ~m10' rm00
    and                         \ | rm00'
    over _rule-set-m00          \ |

    \ Return                    \ cngs1 rul0 rul'
    nip nip
;

\ Return true if a rules' change intersects a changes' changes.
: rule-intersects-changes ( csgs1 rul0 -- flag )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes

    over changes-get-m01 over rule-get-m01 and
    0<> if
        2drop
        true
        exit
    then

    rule-get-m10 swap changes-get-m10 and
    0<>
;

\ Get a predicted sample from a rule, by changes needed in a given sample,
\ forward-chaining.
: rule-get-sample-by-changes-f ( smpl2 rul0 -- smpl t | f )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-sample
    over sample-r-ne-i 0= abort" sample does not change?"

    \ Check if rule has wanted changes.
    over sample-calc-changes            \ s2 r0 cngs (dl)
    swap                                \ s2 cngs r0
    2dup rule-intersects-changes        \ s2 cngs r0 flag
    0= if
        drop
        changes-deallocate
        drop
        false
        exit
    then

    \ Restrict rule to changes, like X->1 to 0->1, for 0->1 in changes.
    over swap                           \ s2 cngs cngs r0
    rule-restrict-to-changes            \ s2 cngs r0' (dl)
    swap changes-deallocate             \ s2 r0' |
    
    \ Check forward path.
    over sample-get-initial dup         \ | sta-i sta-i
    2 pick                              \ | sta-i sta-i r0'
    rule-calc-initial-region            \ | sta-i sta-i reg-i (dl)
    tuck region-superset-of-state       \ | sta-i reg-i flag
    0= if
        2dup                            \ | sta-i reg-i sta-i reg-i
        region-translate-state          \ | sta-i reg-i sta-i'
        rot drop                        \ | reg-i sta-i'
        swap                            \ | sta-i' reg-i
    then
    region-deallocate                   \ | sta-i

    \ sta-i now known to intersect rule initial region.
                                        \ | sta-i
    over rule-apply-to-state-f          \ | smpl2 t | f
    0= abort" apply failed?"

    \ Clean up.
    swap rule-deallocate                \ s2 smpl2
    nip
    true
;
