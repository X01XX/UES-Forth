
#23131 constant rule-id
    #5 constant rule-struct-number-cells

\ Struct fields.
0                       constant rule-header-disp   \ 16-bits, [0] struct id, [1] use count.
rule-header-disp cell+  constant rule-m00-disp      \ 0->0 mask.
rule-m00-disp    cell+  constant rule-m01-disp      \ 0->1 mask.
rule-m01-disp    cell+  constant rule-m11-disp      \ 1->1 mask.
rule-m11-disp    cell+  constant rule-m10-disp      \ 1->0 mask.

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

\ Check TOS for rule, unconventional, leaves stack unchanged.
: assert-tos-is-rule ( rul0 -- )
    dup is-allocated-rule
    is-false if
        s" TOS is not an allocated rule."
        .abort-xt execute
    then
;

\ Check NOS for rule, unconventional, leaves stack unchanged.
: assert-nos-is-rule ( rul1 arg0 -- )
    over is-allocated-rule
    is-false if
        s" NOS is not an allocated rule."
        .abort-xt execute
    then
;

\ Check 3OS for rule, unconventional, leaves stack unchanged.
: assert-3os-is-rule ( rul2 arg1 arg0 -- )
    #2 pick is-allocated-rule
    is-false if
        s" 3OS is not an allocated rule."
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the m00 field of a rule instance.
: rule-get-m00 ( rul0 -- u)
    \ Check arg.
    assert-tos-is-rule

    rule-m00-disp + \ Add offset.
    @               \ Fetch the field.
;

\ Set the m00 field of a rule instance, use only in this file.
: _rule-set-m00 ( u1 rul0 -- )
    rule-m00-disp + \ Add offset.
    !               \ Set the field.
;

\ Return the m01 field of a rule instance.
: rule-get-m01 ( rul0 -- u)
    \ Check arg.
    assert-tos-is-rule

    rule-m01-disp + \ Add offset.
    @               \ Fetch the field.
;

\ Set the m01 field of a rule instance, use only in this file.
: _rule-set-m01 ( u1 rul0 -- )
    rule-m01-disp + \ Add offset.
    !               \ Set the field.
;

\ Return the m11 field of a rule instance.
: rule-get-m11 ( rul0 -- u)
    \ Check arg.
    assert-tos-is-rule

    rule-m11-disp + \ Add offset.
    @               \ Fetch the field.
;

\ Set the m11 field of a rule instance, use only in this file.
: _rule-set-m11 ( u1 rul0 -- )
    rule-m11-disp + \ Add offset.
    !               \ Set the field.
;

\ Return the m10 field of a rule instance.
: rule-get-m10 ( rul0 -- u)
    \ Check arg.
    assert-tos-is-rule

    rule-m10-disp + \ Add offset.
    @               \ Fetch the field.
;

\ Set the m10 field of a rule instance, use only in this file.
: _rule-set-m10 ( u1 rul0 -- )
    rule-m10-disp + \ Add offset.
    !               \ Set the field.
;

\ End accessors.

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
    #3 pick !not            \ u-r u-i addr u-i-not u-r-not
    and over                \ u-r u-i addr m00 addr
    _rule-set-m00           \ u-r u-i addr

    over !not               \ u-r u-i addr u-i-not
    #3 pick                 \ u-r u-i addr u-i-not u-r
    and over                \ u-r u-i addr m01 addr
    _rule-set-m01           \ u-r u-i addr

    over                    \ u-r u-i addr u-i
    #3 pick                 \ u-r u-i addr u-i u-r
    and over                \ u-r u-i addr m11 addr
    _rule-set-m11           \ u-r u-i addr

    over                    \ u-r u-i addr u-i
    #3 pick !not            \ u-r u-i addr u-i u-r-not
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
        #5 pick         \ m00 m01 m11 m10 ms | 0 m00
        #2 pick         \ m00 m01 m11 m10 ms | 0 m00 ms
        and             \ m00 m01 m11 m10 ms | 0 zero-or-non-zero
        if
            1+          \ m00 m01 m11 m10 ms | sum
        then

        \ Check m01
        #4 pick         \ m00 m01 m11 m10 ms | sum m01
        #2 pick         \ m00 m01 m11 m10 ms | sum m01 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            #2 +        \ m00 m01 m11 m10 ms | sum
        then

        \ Check m11
        #3 pick         \ m00 m01 m11 m10 ms | sum m11
        #2 pick         \ m00 m01 m11 m10 ms | sum m11 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            #4 +        \ m00 m01 m11 m10 ms | sum
        then

        \ Check m10
        #2 pick         \ m00 m01 m11 m10 ms | sum m10
        #2 pick         \ m00 m01 m11 m10 ms | sum m10 ms
        and             \ m00 m01 m11 m10 ms | sum zero-or-non-zero
        if
            #8 +        \ m00 m01 m11 m10 ms | sum
        then

        \ Print rule position.
        \ Of 4 masks, one or two can have a bit set and be valid.
        \ Not zero, three or four.
        case
              0 of ." 0?" endof
              1 of ." 00" endof
             #2 of ." 01" endof
             #3 of ." 0X" endof
             #4 of ." 11" endof
             #5 of ." XX" endof
             #6 of ." X1" endof
             #7 of ." 3?" endof
             #8 of ." 10" endof
             #9 of ." X0" endof
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

    #2 <
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

\ Return true if a rule's change intersects a changes' changes.
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

\ Return true if a rule's initial region intersects a region.
: rule-initial-region-intersects-region ( reg1 rul0 -- bool )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    rule-calc-initial-region    \ reg1 initial'
    tuck region-intersects      \ initial' bool
    swap region-deallocate      \ bool
;

\ Return true if a rule's result region intersects a region.
: rule-result-region-intersects-region ( reg1 rul0 -- bool )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    rule-calc-result-region     \ reg1 initial'
    tuck region-intersects      \ initial' bool
    swap region-deallocate      \ bool
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
    \ Check arg.
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
    #2 pick rule-get-m00    \ rul1 rul0 | 0m00 1m00
    and                     \ rul1 rul0 | m00
    -rot                    \ m00 | rul1 rul0

    \ Intersect m01         \ m00 | rul1 rul0
    dup rule-get-m01        \ m00 | rul1 rul0 | 0m01
    #2 pick rule-get-m01    \ m00 | rul1 rul0 | 0m01 1m01
    and                     \ m00 | rul1 rul0 | m01
    -rot                    \ m00 m01 | rul1 rul0

    \ Intersect m11         \ m00 m01 | rul1 rul0
    dup rule-get-m11        \ m00 m01 | rul1 rul0 | 0m11
    #2 pick rule-get-m11    \ m00 m01 | rul1 rul0 | 0m11 1m11
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

\ Or the masks of two rules, not checking if the result is valid.
: rule-or ( rul1 rul0 -- rul )
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
;

\ Return a rule's change mask.
: rule-change-mask ( rul0 -- mask )
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m01        \ rul0 m01
    swap rule-get-m10       \ m01 m10
    or                      \ mask
;

\ Return the valid result of a rule union, or false.
\ Valid result bit positions can be:
\  Change:      0->1, 1->0, X->x (that is, x-not)
\  No change:   0->0, 1->1, X->X
: rule-union-by-changes ( rul1 rul0 -- result true | false )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule

    \ Check if a union can be made.
    over rule-change-mask       \ rul1 rul0 rcm1
    over rule-change-mask       \ rul1 rul0 rcm1 rcm0
    <> if
        2drop
        false
        exit
    then

    \ Make union.
    rule-or                     \ rul
    true
;

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

\ Return a rule for translating one region (tos) to an equal, or subset of another region (nos).
\ 0->X becomes 0->0, 1->X becomes 1->1, by the principle of Least Change, and in that case,
\ the result is a proper subset of the target region.
\ There is no possibility of an X->x position in the result rule.
: rule-new-region-to-region ( reg-to reg-from -- rul )
    \ Check arg.
    assert-tos-is-region
    assert-nos-is-region

    \ 2dup cr .region space .region cr

    2dup change-masks-region-to-region  \ reg-to reg-from r10 r01
    2swap                               \ r10 r01 reg-to reg-from

    \ Get reg-from masks.
    dup region-x-mask -rot              \ r10 r01 fx reg-to reg-from
    dup region-0-mask -rot              \ r10 r01 fx f0 reg-to reg-from
    region-1-mask swap                  \ r10 r01 fx f0 f1 reg-to

    \ Get reg-to masks.
    dup region-x-mask swap              \ r10 r01 fx f0 f1 tx reg-to
    dup region-0-mask swap              \ r10 r01 fx f0 f1 tx t0 reg-to
    region-1-mask                       \ r10 r01 fx f0 f1 tx t0 t1

    \ Calc combined masks.
    #5 pick #3 pick and                 \ r10 r01 fx f0 f1 tx t0 t1 | mxx
    #6 pick #3 pick and                 \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0
    #7 pick #3 pick and                 \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1
    #7 pick #6 pick and                 \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x
    #8 pick #6 pick and                 \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00
    #8 pick #8 pick and                 \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x
    #9 pick #7 pick and                 \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11

    \ Calc r00.
    #2 pick #6 pick #8 pick #6 pick     \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11 | m00 mx0 mxx m0x
    or or or                            \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11 | r00

    \ Calc r11.
     over #6 pick #9 pick #5 pick       \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11 | r00 m11 mx1 mxx m1x
     or or or                           \ r10 r01 fx f0 f1 tx t0 t1 | mxx mx0 mx1 m0x m00 m1x m11 | r00 r11

     \ Clean up.
     2nip 2nip 2nip 2nip 2nip 2nip      \ r10 r01 fx r00 r11
     rot drop                           \ r10 r01 r00 r11

    \ Init rule
    _rule-allocate                      \ r10 r01 r00 r11 rul

    \ Build rule.
    tuck _rule-set-m11                  \ r10 r01 r00 rul
    tuck _rule-set-m00                  \ r10 r01 rul
    tuck _rule-set-m01                  \ r10 rul
    tuck _rule-set-m10                  \ rul
;

\ Return a rule restricted to an intersecting initial region.
: rule-restrict-initial-region ( reg1 rul0 -- rul t | f )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    \ cr ." rule-restrict-initial-region: " over .region space dup .rule cr
    tuck                        \ rul0 reg1 rul0
    rule-calc-initial-region    \ rul0 reg1 reg-initial'
    2dup                        \ rul0 reg1 reg-initial' reg1 reg-initial'
    \ cr ." rule-restrict-initial-region: reg1: " over .region space ." rule initial: " dup .region cr
    region-intersects           \ rul0 reg1 reg-initial' flag
    swap region-deallocate      \ rul0 reg1 flag
    is-false if
        2drop
        false
        exit
    then

    dup region-high-state swap  \ rul0 high reg1
    region-low-state            \ rul0 high low

    !not                        \ rul0 ones zeros
    \ cr ." ones: " over .value cr
    \ cr ." zereos: " dup .value cr

    #2 pick rule-get-m00        \ rul0 ones zeros | m00
    over and                    \ rul0 ones zeros | n00
    #3 pick rule-get-m01        \ rul0 ones zeros | n00 m01
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
    true
;

\ Return a rule restricted to a result region.
: rule-restrict-result-region ( reg1 rul0 -- rul t | f )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    tuck                        \ rul0 reg1 rul0
    rule-calc-result-region     \ rul0 reg1 reg-result'
    2dup                        \ rul0 reg1 reg-result reg1 reg-result'

    region-intersects           \ rul0 reg1 reg-result' flag
    swap region-deallocate      \ rul0 reg1 flag
    is-false if
        2drop
        false
        exit
    then

    dup region-high-state swap  \ rul0 high reg1
    region-low-state            \ rul0 high low

    !not                        \ rul0 ones zeros

    #2 pick rule-get-m00        \ rul0 ones zeros | m00
    over and                    \ rul0 ones zeros | n00
    #3 pick rule-get-m10        \ rul0 ones zeros | n00 m10
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
    true
;

\ Add one to the m00 mask of a rule.
: _rule-adjust-mask-m00 ( rul -- )
    dup rule-get-m00        \ rul m00
    1+
    swap                    \ m00 rul
    _rule-set-m00
;

\ Add one to the m01 mask of a rule.
: _rule-adjust-mask-m01 ( rul -- )
    dup rule-get-m01        \ rul m01
    1+
    swap                    \ m01 rul
    _rule-set-m01
;

\ Add one to the m11 mask of a rule.
: _rule-adjust-mask-m11 ( rul -- )
    dup rule-get-m11        \ rul m11
    1+
    swap                    \ m11 rul
    _rule-set-m11
;

\ Add one to the m10 mask of a rule.
: _rule-adjust-mask-m10 ( rul -- )
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

\ Givin initial and result chars,
\ left-shift all rule masks, set the right-most rule mask
\ bit positions.  For the rule-from-string function.
: _rule-adjust-masks ( ci cr rul0 -- )
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
                    _rule-adjust-mask-m00
                endof
                [char] 1 of \ process 0->1
                    _rule-adjust-mask-m01
                endof
                cr ." unexpected char" abort
            endcase
        endof
        [char] 1 of
            case
                [char] 0 of \ process 1->0
                    _rule-adjust-mask-m10
                endof
                [char] 1 of \ process 1->1
                    _rule-adjust-mask-m11
                endof
                cr ." unexpected char" abort
            endcase
        endof
        [char] X of
            case
                [char] 0 of \ process X->0 (1->0, 0->0)
                    dup _rule-adjust-mask-m00
                    _rule-adjust-mask-m10
                endof
                [char] 1 of \ process X->1 (1->1, 0->1)
                    dup _rule-adjust-mask-m11
                    _rule-adjust-mask-m01
                endof
                [char] X of \ process X->X (1->1, 0->0)
                    dup _rule-adjust-mask-m11
                    _rule-adjust-mask-m00
                endof
                [char] x of \ process X->x (1->0, 0->1)
                    dup _rule-adjust-mask-m10
                    _rule-adjust-mask-m01
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
                #3 pick             \ rul addr ci cr rul
                _rule-adjust-masks   \ rul addr
            endof
            [char] _ of
                -rot                \ rul addr ci cr
                #3 pick             \ rul addr ci cr rul
                _rule-adjust-masks   \ rul addr
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
    or                              \ sta1 change-msk
    over xor                        \ sta1 sta2
    swap                            \ sta2 sta1
    sample-new                      \ smpl
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
: rule-restrict-to-region ( reg1 rul0 -- rul t | f )
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

    \ Restrict the rule's initial region.
    2dup                            \ reg1 rul0 reg1 rul0
    rule-restrict-initial-region    \ reg1 rul0, rul' t | f
    is-false if
        2drop
        false
        exit
    then

    \ Check if the restricted rule's result region intersects reg1.
    dup rule-calc-result-region     \ reg1 rul0 rul' reg-r
    dup #4 pick                     \ reg1 rul0 rul' reg-r reg-r reg1
    region-intersects               \ reg1 rul0 rul' reg-r flag
    swap region-deallocate          \ reg1 rul0 rul' flag
    0= if
        rule-deallocate
        2drop
        false
        exit
    then

    \ Check if reg1 is a superset of the restricted rule's result region.
    dup rule-calc-result-region     \ reg1 rul0 rul' reg-r
    dup #4 pick                     \ reg1 rul0 rul' reg-r reg-r reg1
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

    \ Restrict the restricted rule's result region to reg1.
                                    \ reg1 rul0 rul'
    nip                             \ reg1 rul'
    tuck                            \ rul' reg1 rul'
    rule-restrict-result-region     \ rul', rul'' t | f
    is-false if
        rule-deallocate
        false
        exit
    then

    \ Clean up.
    swap rule-deallocate            \ rul''

    \ Return.
    dup rule-makes-change           \ rul'' flag
    if
        true
    else
        rule-deallocate
        false
    then
;

\ Return true if rule has at least one needed change.
: rule-changes-null ( rul0 -- flag )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes

    over changes-get-m01        \ cngs1 rul0 c-m01
    over rule-get-m01           \ cngs1 rul0 c-m01 r-m01
    and                         \ cngs1 rul0 same-ones-mask
    0<> if
        2drop                   \
        false
        exit
    then

                                \ cngs1 rul0
    rule-get-m10                \ cngs1 r-m10
    swap changes-get-m10        \ r-m10 c-m10
    and                         \ same-ones-mask
    0=                          \ flag
;

\ Return a rule with given changes isolated for forward chaining.
\ Matching change m10 positions and X->x becomes 1->0.
\ Matching change m01 positions and X->x becomes 0->1.
\ For non-intersecting, reachable, rules, for forward, or backward, chaining.
: rule-isolate-changes ( cngs1 rul0 -- rul)
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes
    \ Rule should have some changes.
    2dup rule-changes-null      \ cngs1 rul0 bool
    abort" rule has no changes?"

    over changes-duplex
    abort" changes duplex?"

    over changes-null
    abort" changes null?"

    \ Get Xx mask
                                \ cngs1 rul0 |
    dup rule-get-m01            \ cngs1 rul0 | r-m01
    over rule-get-m10           \ cngs1 rul0 | r-m01 r-m10
    and                         \ cngs1 rul0 | mXx |

    \ Calc new m01, m01 & ~(mXx & needed m10).
    over rule-get-m01           \ cngs1 rul0 | mXx | r-m01
    over                        \ cngs1 rul0 | mXx | r-m01 mXx
    #4 pick                     \ cngs1 rul0 | mXx | r-m01 mXx cngs1
    changes-get-m10             \ cngs1 rul0 | mXx | r-m01 mXx c-m10
    and                         \ cngs1 rul0 | mXx | r-m01 c-m10'
    !not                        \ cngs1 rul0 | mXx | r-m01 c-m01-ok
    and                         \ cngs1 rul0 | mXx | r-m01'

    \ Calc new m10, m10 & ~(mXx & needed m01).
    #2 pick rule-get-m10        \ cngs1 rul0 | mXx | r-m01' r-m10
    #2 pick                     \ cngs1 rul0 | mXx | r-m01' r-m10 mXx
    #5 pick                     \ cngs1 rul0 | mXx | r-m01' r-m10 mXx cngs1
    changes-get-m01             \ cngs1 rul0 | mXx | r-m01' r-m10 mXx c-m01
    and                         \ cngs1 rul0 | mXx | r-m01' r-m10 c-m01'
    !not                        \ cngs1 rul0 | mXx | r-m01' r-m10 c-m10-ok
    and                         \ cngs1 rul0 | mXx | r-m01' r-m10'

    \ Get other masks.
    #3 pick                     \ cngs1 rul0 | mXx | r-m01' r-m10' rul0
    dup                         \ cngs1 rul0 | mXx | r-m01' r-m10' rul0 rul0
    rule-get-m00 swap           \ cngs1 rul0 | mXx | r-m01' r-m10' m00 rul0
    rule-get-m11                \ cngs1 rul0 | mXx | r-m01' r-m10' m00 m11

    \ Build result rule.
    _rule-allocate              \ cngs1 rul0 | mXx | r-m01' r-m10' m00 m11 rul
    tuck _rule-set-m11          \ cngs1 rul0 | mXx | r-m01' r-m10' m00 rul
    tuck _rule-set-m00          \ cngs1 rul0 | mXx | r-m01' r-m10' rul
    tuck _rule-set-m10          \ cngs1 rul0 | mXx | r-m01' rul
    tuck _rule-set-m01          \ cngs1 rul0 | mXx | rul

    \ Clean up.
    2nip nip                    \ rul
;

\ Apply a rule to a given region, backward-chaining, returning an initial region.
\ A from/to pair of regions is passed, to form a needed rule, part of which may be
\ satisfied by the target rule, rul0.
: rule-apply-to-region-bc ( reg-to reg-from rul0 -- rule t | f )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick region-intersects abort" from\to regions intersect?"

    \ Restrict the result region of the rule.
    #2 pick over                    \ reg-to reg-from rul0 | reg-to rul0
    rule-restrict-result-region     \ reg-to reg-from rul0 | rul0' t | f
    if
    else
        3drop
        false
        exit
    then
    \ TODO

    \ Get rule changes that are needed in reg-from -> reg-to.
    over                            \ reg-to reg-from rul0 | reg-to' rul0' reg-to'
    #4 pick                         \ reg-to reg-from rul0 | reg-to' rul0' reg-to' reg-from
    changes-new-region-to-region    \ reg-to reg-from rul0 | reg-to' rul0' ft-cngs'
    dup                             \ reg-to reg-from rul0 | reg-to' rul0' ft-cngs'
    over                            \ reg-to reg-from rul0 | reg-to' rul0' ft-cngs' rul0'
    rule-get-changes                \ reg-to reg-from rul0 | reg-to' rul0' ft-cngs' cngs-rul'
    2dup changes-intersection       \ reg-to reg-from rul0 | reg-to' rul0' ft-cngs' cngs-rul' cngs-needed'
    swap changes-deallocate         \ reg-to reg-from rul0 | reg-to' rul0' ft-cngs' cngs-needed'
    swap changes-deallocate         \ reg-to reg-from rul0 | reg-to' rul0' cngs-needed'

    \ Check if the target rule' has any needed changes.
    dup changes-null                \ reg-to reg-from rul0 | reg-to' rul0' cngs-needed' bool
    if                              \ reg-to reg-from rul0 | reg-to' rul0' cngs-needed'
        changes-deallocate
        rule-deallocate
        region-deallocate
        3drop
        false
        exit
    then
                                    \ reg-to reg-from rul0 | reg-to' rul0' cngs-needed'

    \ Isolate needed changes in the rule that may be in X->0, X->1, or X->x.
    2dup swap                       \ reg-to reg-from rul0 | reg-to' rul0' cngs-needed' cngs-needed' rul0'
    rule-isolate-changes            \ reg-to reg-from rul0 | reg-to' rul0' cngs-needed' rul0''

    \ Clean up.
    swap changes-deallocate         \ reg-to reg-from rul0 | reg-to' rul0' rul0''
    swap rule-deallocate            \ reg-to reg-from rul0 | reg-to' rul0''
    swap region-deallocate          \ reg-to reg-from rul0 | rul0''
    2nip nip                        \ rul0''
    true
;

\ Given a rule and wanted changes, return the number of unwanted changes.
: rule-number-unwanted-changes ( cngs1 rul0 -- u )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes

    rule-get-changes            \ cngs1 cngs0'
    swap                        \ cngs0' cngs1
    changes-invert              \ cngs0' cngs1'
    2dup changes-intersection   \ cngs0' cngs1' cngs-un'
    swap changes-deallocate     \ cngs0' cngs-un'
    swap changes-deallocate     \ cngs-un'
    dup changes-number-changes  \ cngs-un' u
    swap changes-deallocate     \ u
;

\ Return a step for a rule (tos) and needed changes (nos).
\ If the rule has a needed change, a step will be returned.
: rule-calc-step-by-changes ( cngs1 rul0 -- step t | f )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes

    2dup rule-intersects-changes    \ cgs1 rul0 bool
    if
        nip                         \ rul0
        cur-action-xt execute       \ rul0 act
        step-new-xt execute         \ stp
        true
    else
        2drop
        false
    then
;

: rule-has-needed-changes ( reg-to reg-from rul0 -- bool )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region

    #2 pick #2 pick                             \ | reg-to reg-from
    changes-new-region-to-region                \ | ned-cngs'

    \ Check if rule has a needed change.
    2dup swap                                   \ | ned-cngs' ned-cngs' rul0
    rule-intersects-changes                     \ | ned-cngs' bool
    swap changes-deallocate                     \ | bool
    2nip nip                                    \ bool
;

\ Return true if using a rule is premature, in forward-chaining.
: rule-use-is-premature-fc  ( reg-to reg-from rul0 -- bool )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region

    \ Calc reg-from to rule initial-region changes.
    rule-calc-initial-region            \ reg-to reg-from rul-i'
    2dup swap                           \ reg-to reg-from rul-i' rul-i' reg-from
    changes-new-region-to-region        \ reg-to reg-from rul-i' to-cngs'
    swap region-deallocate              \ reg-to reg-from to-cngs'

    \ Calc needed changes reg-from to reg-to.
    -rot                                \ to-cngs' reg-to reg-from
    changes-new-region-to-region        \ to-cngs' ned-cngs'

    \ Check if they intersect.
    2dup changes-intersect              \ to-cngs' ned-cngs' bool

    \ Clean up.
    swap changes-deallocate
    swap changes-deallocate             \ bool
;

\ Return true if using a rule is premature, in backward-chaining.
: rule-use-is-premature-bc  ( reg-to reg-from rul0 -- bool )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region

    \ Calc from rule result region to reg-to.
    rule-calc-result-region             \ reg-to reg-from rul-r'
    #2 pick over                        \ reg-to reg-from rul-r' reg-to rul-r'
    changes-new-region-to-region        \ reg-to reg-from rul-r' to-cngs'
    swap region-deallocate              \ reg-to reg-from to-cngs'

    \ Calc needed changes reg-from to reg-to.
    -rot                                \ to-cngs' reg-to reg-from
    changes-new-region-to-region        \ to-cngs' ned-cngs'

    \ Check if they intersect.
    2dup changes-intersect              \ to-cngs' ned-cngs' bool

    \ Clean up.
    swap changes-deallocate
    swap changes-deallocate             \ bool
;

\ Combine tos rule to nos rule.
\ The tos rule result region has to intersect the nos initial rule.
: rule-combine2 ( rul1-to rul0-from -- rul )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule

    over rule-calc-initial-region   \ rul1 rul0 rul1-i
    over rule-calc-result-region    \ rul1 rul0 rul1-i rul0-r

    2dup region-intersects          \ rul1 rul0 rul1-i rul0-r bool
    is-false abort" rules not linked?"

                                    \ rul1 rul0 rul1-i rul0-r
    region-deallocate
    region-deallocate               \ rul1 rul0

    \ Calc m00, (rul0-m00 & rul1-m00) + (rul0-m01 rlu1-m10).
    dup rule-get-m00                \ rul1 rul0 rul0-m00
    #2 pick rule-get-m00            \ rul1 rul0 rul0-m00 rul1-m00
    and                             \ rul1 rul0 m00-a
    over rule-get-m01               \ rul1 rul0 m00-a rul0-m01
    #3 pick rule-get-m10            \ rul1 rul0 m00-a rul0-m01 rul1-m10
    and                             \ rul1 rul0 m00-a m00-b
    or                              \ rul1 rul0 m00
    -rot                            \ m00 rul1 rul0

    \ Calc m01, (rul0-m00 & rul1-m01) + (rul0-m01 rlu1-m11).
    dup rule-get-m00                \ m00 rul1 rul0 rul0-m00
    #2 pick rule-get-m01            \ m00 rul1 rul0 rul0-m00 rul1-m01
    and                             \ m00 rul1 rul0 m01-a
    over rule-get-m01               \ m00 rul1 rul0 m00-a rul0-m01
    #3 pick rule-get-m11            \ m00 rul1 rul0 m00-a rul0-m01 rul1-m11
    and                             \ m00 rul1 rul0 m00-a m01-b
    or                              \ m00 rul1 rul0 m01
    -rot                            \ m00 m01 rul1 rul0

    \ Calc m011, (rul0-m11 & rul1-m11) + (rul0-m10 rlu1-m01).
    dup rule-get-m11                \ m00 m01 rul1 rul0 rul0-m11
    #2 pick rule-get-m11            \ m00 m01 rul1 rul0 rul0-m11 rul1-m11
    and                             \ m00 m01 rul1 rul0 m11-a
    over rule-get-m10               \ m00 m01 rul1 rul0 m11-a rul0-m10
    #3 pick rule-get-m01            \ m00 m01 rul1 rul0 m11-a rul0-m10 rul1-m01
    and                             \ m00 m01 rul1 rul0 m11-a m11-b
    or                              \ m00 m01 rul1 rul0 m11
    -rot                            \ m00 m01 m11 rul1 rul0

    \ Calc m010, (rul0-m11 & rul1-m10) + (rul0-m10 rlu1-m00).
    dup rule-get-m11                \ m00 m01 m11 rul1 rul0 rul0-m11
    #2 pick rule-get-m10            \ m00 m01 m11 rul1 rul0 rul0-m11 rul1-m10
    and                             \ m00 m01 m11 rul1 rul0 m10-a
    over rule-get-m10               \ m00 m01 m11 rul1 rul0 m10-a rul0-m10
    #3 pick rule-get-m00            \ m00 m01 m11 rul1 rul0 m10-a rul0-m10 rul1-m00
    and                             \ m00 m01 m11 rul1 rul0 m10-a m10-b
    or                              \ m00 m01 m11 rul1 rul0 m10
    -rot                            \ m00 m01 m11 m10 rul1 rul0

    \ Make new rule.
    2drop _rule-allocate            \ m00 m01 m11 m10 rul
    tuck _rule-set-m10              \ m00 m01 m11 rul
    tuck _rule-set-m11              \ m00 m01 rul
    tuck _rule-set-m01              \ m00 rul
    tuck _rule-set-m00              \ rul
;

\ Get a step from a given rule, for forward chaining, from a given region (reg-from), to another, non-intersecting, given region (reg-to), if possible.
\
\ A rule with an initial-region that does not intersect reg-from, but the initial-region does intersect the union of reg-from and reg-to,
\ will necessarily require a needed change to go from reg-from to the rule's initial-region, will therefore be premature to use,
\ and will return false.
\
\ A returned step's rule will have at least one wanted change.
\
\ A returned step's rule with an initial-region that does intersect reg-from, with a result intersecting the union of reg-from and reg-to,
\ will have no unwanted changes.
\
\ A returned step's rule with an initial-region that does not intersect reg-from, will have at least one unwanted change.
\
\ If the rule has a needed change,
\ If the rule initial-region intersects reg-from, or
\ going from reg-from to the rule initial-region does not contain a needed change, a step will be returned.
: rule-calc-step-fc ( reg-to reg-from rul0 -- step t | f )
    \ cr ." rule-calc-step-fc: from: " over .region space ." to: " #2 pick .region space ." rule: " dup .rule cr
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                             \ | reg-to reg-from
    2dup region-superset-of                     \ | reg-to reg-from bool
    abort" rule-calc-step-fc: region subset?"   \ | reg-to reg-from
    2dup swap region-superset-of                \ | reg-to reg-from bool
    abort" rule-calc-step-fc: region subset?"   \ | reg-to reg-from

    \ Check for needed changes.

    \ Get needed changes.
    changes-new-region-to-region                \ reg-to reg-from rul0 | ned-cngs'

    \ Check if rule contains at least one needed change.
    2dup swap rule-intersects-changes           \ | ned-cngs' bool
    if
    else                                        \ reg-to reg-from rul0 | ned-cngs'
        changes-deallocate
        3drop
        false
        exit
    then

                                                \ reg-to reg-from rul0 | ned-cngs'

    \ Check if reg-from intersects rule initial-region.

    #2 pick #2 pick                             \ | ned-cngs' reg-from rul0
    rule-restrict-initial-region                \ | ned-cngs', rul0' t | f
    if                                          \ | ned-cngs' rul0'

        \ Get number unwanted changes.
        2dup                                    \ | ned-cngs' rul0' ned-cngs' rul0'
        rule-number-unwanted-changes            \ | ned-cngs' rul0' u-unw
        -rot                                    \ | u-unw ned-cngs' rul0'

        \ Make step.
        cur-action-xt execute                   \ | u-unw ned-cngs' rul0' act
        step-new-xt execute                     \ | u-unw ned-cngs' stpx
        swap changes-deallocate                 \ | u-unw stpx

        \ Set number unwanted changes.
        tuck                                    \ | stpx u-unw stpx
        step-set-number-unwanted-changes-xt     \ | stpx u-unw stpx xt
        execute                                 \ | stpx

        \ Clean up.                             \ | stpx
        2nip nip                                \ stpx

        \ Return
        true
        exit
    then

    \ reg-from does not intersect rule initial-region.
                                                \ reg-to reg-from rul0 | ned-cngs'

    \ Check if rule use is premature.
    #3 pick #3 pick #3 pick                     \ reg-to reg-from rul0 | ned-cngs' reg-to reg-from rul0
    rule-use-is-premature-fc                    \ reg-to reg-from rul0 | ned-cngs' bool
    if
        changes-deallocate
        3drop
        false
        exit
    then

                                                \ reg-to reg-from rul0 | ned-cngs'

    \ Calc rule from reg-from to rul0 initial-region.
    #2 pick #2 pick                             \ reg-to reg-from rul0 | ned-cngs' reg-from rul0
    rule-calc-initial-region                    \ reg-to reg-from rul0 | ned-cngs' reg-from rul-i'
    tuck swap                                   \ reg-to reg-from rul0 | ned-cngs' rul-i' rul-i' reg-from
    rule-new-region-to-region                   \ reg-to reg-from rul0 | ned-cngs' rul-i' rul1'
    swap region-deallocate                      \ reg-to reg-from rul0 | ned-cngs' rul1'

    \ Restrict rule to likely limits based on reg-from.
    \ 1 X->1 = 1->1
    \ 0 X->1 = 0->1
    \ 1 X->0 = 1->0
    \ 0 X->0 = 0->0
    \ 1 X->X = 1->1
    \ 0 X->X = 0->0
    \ 1 X->x = 1->0
    \ 0 X->x = 0->1
    dup rule-calc-result-region                 \ reg-to reg-from rul0 | ned-cngs' rul1' rul1-r'
    dup                                         \ reg-to reg-from rul0 | ned-cngs' rul1' rul1-r' rul1-r'
    #4 pick                                     \ reg-to reg-from rul0 | ned-cngs' rul1' rul1-r' rul1-r' rul0
    rule-restrict-initial-region                \ reg-to reg-from rul0 | ned-cngs' rul1' rul1-r', rul0' t | f
    is-false abort" rule-calc-step-fc: rule-restrict-initial-region failed?"

    swap region-deallocate                      \ reg-to reg-from rul0 | ned-cngs' rul1' rul0'

    \ Calc number unwanted changes.
    2dup swap                                   \ reg-to reg-from rul0 | ned-cngs' rul1' rul0' rul0' rul1'
    rule-combine2                               \ reg-to reg-from rul0 | ned-cngs' rul1' rul0' rul3'
    #3 pick over                                \ reg-to reg-from rul0 | ned-cngs' rul1' rul0' rul3' ned-cngs' rul3'
    rule-number-unwanted-changes                \ reg-to reg-from rul0 | ned-cngs' rul1' rul0' rul3' u-unw
    swap rule-deallocate                        \ reg-to reg-from rul0 | ned-cngs' rul1' rul0' u-unw
    rot rule-deallocate                         \ reg-to reg-from rul0 | ned-cngs' rul0' u-unw
    rot changes-deallocate                      \ reg-to reg-from rul0 | rul0' u-unw
    swap                                        \ reg-to reg-from rul0 | u-unw rul0'

    \ Make step.
    cur-action-xt execute                       \ | u-unw rul0' act
    step-new-xt execute                         \ | u-unw stp

    \ Update unwanted number of changes.
    tuck                                        \ | stp u-unw stp
    step-set-number-unwanted-changes-xt         \ | stp u-unw stp xt
    execute                                     \ | stp
    \ cr ." indirect step: " dup .step-xt execute cr

    \ Clean up.
    2nip nip                                     \ stp

    \ Return
    true
;

\ Get a step from a rule, if it can be applied to a from-region (tos) to-region (nos) pair, for the purpose of backward chaining.
\ If the rule has a needed change,
\ If the rule result-region intersects reg-to, or
\ going from reg-to to the rule result-region does not contain a needed change, a step will be returned.
: rule-calc-step-bc ( reg-to reg-from rul0 -- step t | f )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." rule-calc-step-bc: to: " #2 pick .region space ." from: " over .region space ." rule: " dup .rule cr

    #2 pick #2 pick                             \ | reg-to reg-from
    2dup region-superset-of                     \ | reg-to reg-from bool
    abort" rule-calc-step-bc: region subset?"   \ | reg-to reg-from
    2dup swap region-superset-of                \ | reg-to reg-from bool
    abort" rule-calc-step-bc: region subset?"   \ | reg-to reg-from

    \ Check for needed changes.

    \ Get needed changes.
    changes-new-region-to-region                \ | ned-cngs'

    \ Check if rule contains at least one needed change.
    2dup swap rule-intersects-changes           \ reg-to reg-from rul0 | ned-cngs' bool
    if
    else
        changes-deallocate
        3drop
        false
        exit
    then

    \ Check if rule reg-to intersects rule result-region.
    #3 pick #2 pick                             \ reg-to reg-from rul0 | ned-cngs' reg-to rul0
    rule-restrict-result-region                 \ | ned-cngs', rul0' t | f
    if                                          \ | ned-cngs' rul0'

        \ Get number unwanted changes.
        2dup                                    \ | ned-cngs' rul0' ned-cngs' rul0'
        rule-number-unwanted-changes            \ | ned-cngs' rul0' u-unw
        -rot                                    \ | u-unw ned-cngs' rul0'

        \ Make step.
        cur-action-xt execute                   \ | u-unw ned-cngs' rul0' act
        step-new-xt execute                     \ | u-unw ned-cngs' stpx
        swap changes-deallocate                 \ | u-unw stpx

        \ Set number unwanted changes.
        tuck                                    \ | stpx u-unw stpx
        step-set-number-unwanted-changes-xt     \ | stpx u-unw stpx xt
        execute                                 \ | stpx

        \ Clean up.                             \ | stpx
        2nip nip                                \ stpx

        \ Return
        true
        exit
    then

    \ reg-to does not intersect rule result-region.

    \ Check if rule use is premature.           \ reg-to reg-from rul0 | ned-cngs'
    #3 pick #3 pick #3 pick                     \ reg-to reg-from rul0 | ned-cngs' reg-to reg-from rul0
    rule-use-is-premature-bc                    \ reg-to reg-from rul0 | ned-cngs' bool
    if
        changes-deallocate
        3drop
        false
        exit
    then

    \ Calc rule from rul0 result-region to reg-to.
                                                \ reg-to reg-from rul0 | ned-cngs'
    #3 pick #2 pick                             \ reg-to reg-from rul0 | ned-cngs' reg-to rul0
    rule-calc-result-region                     \ reg-to reg-from rul0 | ned-cngs' reg-to rul-r'

    \ Calc translated rule result region.
    2dup swap                                   \ reg-to reg-from rul0 | ned-cngs' reg-to rul-r' reg-to rul-r'
    region-translate-to-region                  \ reg-to reg-from rul0 | ned-cngs' reg-to rul-r' rul-r''

    swap region-deallocate                      \ reg-to reg-from rul0 | ned-cngs' reg-to rul-r''

    \ Calc rule from rule result-region, adjusted, to reg-to.
    tuck                                        \ | ned-cngs' rul-r'' reg-to rul-r''
    rule-new-region-to-region                   \ | ned-cngs' rul-r'' rul1'
    swap region-deallocate                      \ | ned-cngs' rul1'

    \ Restrict rule to likely limits based on reg-to.
    \ X->1 1 = X->1
    \ X->0 0 = X->0
    \ X->X 1 = 1->1
    \ X->X 0 = 0->0
    \ X->x 0 = 1->0
    \ X->x 1 = 0->1
    dup rule-calc-initial-region dup            \ | ned-cngs' rul1' rul1-i' rul1-i'
    #4 pick                                     \ | ned-cngs' rul1' rul1-i' rul1-i' rul0
    rule-restrict-result-region                 \ | ned-cngs' rul1' rul1-i', rul0' t | f
    is-false abort" rule-calc-step-bc: rule-restrict-result-region failed?"

    swap region-deallocate                      \ | ned-cngs' rul1' rul0'

    \ Calc number unwanted changes.
    2dup rule-combine2                          \ | ned-cngs' rul1' rul0' rul3'
    #3 pick over                                \ | ned-cngs' rul1' rul0' rul3' ned-cngs' rul3'
    rule-number-unwanted-changes                \ | ned-cngs' rul1' rul0' rul3' u-unw
    swap rule-deallocate                        \ | ned-cngs' rul1' rul0' u-unw
    rot rule-deallocate                         \ | ned-cngs' rul0' u-unw
    rot changes-deallocate                      \ | rul0' u-unw
    swap                                        \ | u-unw rul0'

    \ Make step.
    cur-action-xt execute                       \ | u-unw rul0' act
    step-new-xt execute                         \ | u-unw stp

    \ Update unwanted number of changes.
    tuck                                        \ | stp u-unw stp
    step-set-number-unwanted-changes-xt         \ | stp u-unw stp xt
    execute                                     \ | stp

    \ Clean up.
    2nip nip                                     \ stp

    \ Return
    true
;

\ Combine tos rule to nos rule.
\ If the tos rule result region does not intersect the nos initial rule,
\ calculate a rule between them.
: rule-combine ( rul1 rul0 -- rul )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule

    over rule-calc-initial-region   \ rul1 rul0 rul1-reg-i
    over rule-calc-result-region    \ rul1 rul0 rul1-reg-i rul0-reg-r

    2dup region-intersects          \ rul1 rul0 rul1-reg-i rul0-reg-r bool
    if
        region-deallocate
        region-deallocate
        rule-combine2               \ rul
        exit
    then

                                    \ rul1 rul0 rul1-reg-i rul0-reg-r
    2dup rule-new-region-to-region  \ rul1 rul0 rul1-reg-i rul0-reg-r rul-between

    swap region-deallocate
    swap region-deallocate          \ rul1 rul0 rul-bet

    tuck                            \ rul1 rul-bet rul0 rul-bet
    swap                            \ rul1 rul-bet rul-bet rul0
    rule-combine2                   \ rul1 rul-bet rul-0b

    swap rule-deallocate            \ rul1 rul-0b

    tuck                            \ rul-0b rul1 rul-0b
    rule-combine2                   \ rul-0b rul-0b1

    swap rule-deallocate            \ rul-0b1
;

\ Return true if a rule is valid.
: rule-is-valid ( rul0 - bool )
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m00            \ rul0 m00
    over rule-get-m01           \ rul0 m00 m01
    and                         \ rul0 m0x
    0<> if
        \ cr ." rule " over .rule space ." m0x: " dup .value cr
        2drop
        false
        exit
    then

    dup rule-get-m11            \ rul0 m11
    over rule-get-m10           \ rul0 m11 m10
    and                         \ rul0 m1x
    0<> if
        \ cr ." rule " over .rule space ." m1x: " dup .value cr
        2drop
        false
        exit
    then

    dup rule-get-m00 swap       \ m00 rul0
    dup rule-get-m01 swap       \ m00 m01 rul0
    dup rule-get-m11 swap       \ m00 m01 m11 rul0
    rule-get-m10                \ m00 m01 m11 m10
    or or or                    \ mxx
    cur-domain-xt execute       \ mxx dom
    domain-get-all-bits-mask-xt \ mxx dom xt
    execute                     \ mxx dxx
    \ cr ." mxx: " over .value space ." dxx : " dup .value cr

    =                           \ bool
;
