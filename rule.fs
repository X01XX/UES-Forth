
#23131 constant rule-id
    #5 constant rule-struct-number-cells

\ Struct fields.
0                       constant rule-header-disp   \ 16-bits, [0] struct id, [1] use count.
rule-header-disp cell+  constant rule-m00-disp      \ 0->0 mask.
rule-m00-disp    cell+  constant rule-m01-disp      \ 0->1 mask.
rule-m01-disp    cell+  constant rule-m11-disp      \ 1->1 mask.
rule-m11-disp    cell+  constant rule-m10-disp      \ 1->0 mask.

0 value rule-mma    \ Storage for rule mma instance.

: rule-mma-init ( num-items -- ) \ Init rule mma, return the addr of allocated memory.
    dup 1 <
    abort" rule-mma-init: Invalid number of items."

    cr ." Initializing Rule store."
    rule-struct-number-cells swap mma-new to rule-mma
;

: assert-rule-mma-none-in-use ( -- )    \ Check all rule array items are unallocated.
    rule-mma mma-in-use 0<>
    abort" rule-mma use GT 0"
;

\ Check instance type.

: is-allocated-rule ( addr -- flag )    \ Check if an address is within the rule array.
    \ Insure the given addr cannot be an invalid addr.
    dup rule-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        rule-id =
    else
        drop false
    then
;

: assert-tos-is-rule ( tos -- tos ) \ Check TOS for rule, unconventional, leaves stack unchanged.
    dup is-allocated-rule
    is-false if
        s" TOS is not an allocated rule."
        .abort-xt execute
    then
;

: assert-nos-is-rule ( nos tos -- nos tos ) \ Check NOS for rule, unconventional, leaves stack unchanged.
    over is-allocated-rule
    is-false if
        s" NOS is not an allocated rule."
        .abort-xt execute
    then
;

\ Start accessors.

: rule-get-m00 ( rul0 -- u) \ Return the m00 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m00-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m00 ( u1 rul0 -- )  \ Set the m00 field of a rule instance, use only in this file.
    rule-m00-disp + \ Add offset.
    !               \ Set the field.
;

: rule-get-m01 ( rul0 -- u) \ Return the m01 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m01-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m01 ( u1 rul0 -- )  \ Set the m01 field of a rule instance, use only in this file.
    rule-m01-disp + \ Add offset.
    !               \ Set the field.
;

: rule-get-m11 ( rul0 -- u) \ Return the m11 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m11-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m11 ( u1 rul0 -- )  \ Set the m11 field of a rule instance, use only in this file.
    rule-m11-disp + \ Add offset.
    !               \ Set the field.
;

: rule-get-m10 ( rul0 -- u) \ Return the m10 field of a rule instance.
    \ Check arg.
    assert-tos-is-rule

    rule-m10-disp + \ Add offset.
    @               \ Fetch the field.
;

: _rule-set-m10 ( u1 rul0 -- )  \ Set the m10 field of a rule instance, use only in this file.
    rule-m10-disp + \ Add offset.
    !               \ Set the field.
;

\ End accessors.

: _rule-allocate ( -- rul0 )    \ Allocate a rule, setting id and use count only, use only in this file.
    \ Allocate space.
    rule-mma mma-allocate   \ addr

    \ Store id.
    rule-id over            \ addr id addr
    struct-set-id           \ addr

    \ Init use count.
    0 over struct-set-use-count \ addr
;

: rule-new ( u-result u-initial -- addr)    \ Create a rule from two numbers on the stack.
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

: rule-get-masks ( rul0 -- m00 m01 m11 m10 )    \ Push all four masks onto stack.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m00 swap   \ m00 rule
    dup rule-get-m01 swap   \ m00 m01 rule
    dup rule-get-m11 swap   \ m00 m01 m11 rule
    rule-get-m10            \ m00 m01 m11 m10
;

: rule-eq ( rul1 rul0 -- flag ) \ Return true if two rules are equal.
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

: .rule ( rul0 -- ) \ Print a rule.
    \ Check arg.
    assert-tos-is-rule

    \ Set up masks and most-significant-bit,
    \ the basis of each cycle.
    rule-get-masks                      \ m00 m01 m11 m10
    current-ms-bit-mask                 \ m00 m01 m11 m10 ms

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

: rule-deallocate ( rul0 -- )   \ Deallocate a rule.
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

: rule-calc-initial-region ( rul0 -- reg0 ) \ Return rule initial region.
    \ Check arg.
    assert-tos-is-rule

    rule-get-masks      \ m00 m01 m11 m10
    or -rot             \ most-ones m00 m01
    or !not             \ most-ones most-zeros
    region-new
;

: rule-calc-result-region ( rul0 -- reg0 )  \ Return rule result region.
    \ Check arg.
    assert-tos-is-rule

    rule-get-masks      \ m00 m01 m11 m10
    -rot                \ m00 m10 m01 m11
    or -rot             \ most-ones m00 m01
    or !not             \ most-ones most-zeros
    region-new
;

: rule-intersects ( rul1 rul0 -- flag ) \ Return true if two rules intersect.
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

: rule-intersects-changes ( csgs1 rul0 -- flag )    \ Return true if a rule's change intersects a changes' changes.
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

: rule-initial-region-intersects ( reg1 rul0 -- bool )  \ Return true if a rule's initial region intersects a region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    rule-calc-initial-region    \ reg1 initial'
    tuck region-intersects      \ initial' bool
    swap region-deallocate      \ bool
;

: rule-result-region-intersects ( reg1 rul0 -- bool )   \ Return true if a rule's result region intersects a region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    rule-calc-result-region     \ reg1 initial'
    tuck region-intersects      \ initial' bool
    swap region-deallocate      \ bool
;

: rule-all-bits-set ( rul0 -- flag )    \ Return true if all bit positions in a rule are represented.
    \ Check arg.
    assert-tos-is-rule

    \ Or all mask bits.
    rule-get-masks          \ m00 m01 m11 m10
    or or or                \ m-all

    \ Check that all bit positions are used.
    current-all-bits-mask   \ m-all msk
    =
;

\ A valid intersection may not have the same initial region as the intersection
\ of the two rules initial regions.
\ As X1 & Xx = 01, X1 & XX = 11, X0 & Xx = 10, X0 & XX = 00.
: rule-intersection ( rul1 rul0 -- result true | false )    \ Return the valid result of a rule intersection, or false.
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

: rule-or ( rul1 rul0 -- rul )  \ Or the masks of two rules, not checking if the result is valid.
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

: rule-change-mask ( rul0 -- mask ) \ Return a rule's change mask.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m01        \ rul0 m01
    swap rule-get-m10       \ m01 m10
    or                      \ mask
;

: rule-union-by-changes ( rul1 rul0 -- result true | false )    \ If two rule changes (m01, m10) are equal, form a union.
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

: rule-union ( rul1 rul0 -- result true | false )   \ Return the result of a rule union, if valid.
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

\ 0->X becomes 0->0, 1->X becomes 1->1, by the principle of Least Change.
\ There is no possibility of an X->x position in the result rule.
: rule-new-region-to-region ( reg-to reg-from -- rul )  \ Return a rule for translating one region (tos) to another.
    \ Check arg.
    assert-tos-is-region
    assert-nos-is-region

    \ 2dup cr .region space .region cr

    2dup change-masks-region-to-region  \ reg-to reg-from r10 r01
    2swap                               \ r10 r01 reg-to reg-from

    \ Get reg-from masks.
    dup region-x-mask -rot              \ r10 r01 fx reg-to reg-from
    dup region-edge-0-mask -rot         \ r10 r01 fx f0 reg-to reg-from
    region-edge-1-mask swap             \ r10 r01 fx f0 f1 reg-to

    \ Get reg-to masks.
    dup region-x-mask swap              \ r10 r01 fx f0 f1 tx reg-to
    dup region-edge-0-mask swap         \ r10 r01 fx f0 f1 tx t0 reg-to
    region-edge-1-mask                  \ r10 r01 fx f0 f1 tx t0 t1

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

: rule-restrict-initial-region ( reg1 rul0 -- rul t | f )   \ Return a rule restricted to an intersecting initial region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    \ cr ." rule-restrict-initial-region: " over .region space dup .rule cr

    2dup rule-initial-region-intersects  \ reg1 rul0 bool 
    is-false if
        2drop
        false
        exit
    then

    swap                        \ rul0 reg1
    dup region-high-state swap  \ rul0 high reg1
    region-low-state            \ rul0 high low

    !not                        \ rul0 ones zeros

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

: rule-restrict-result-region ( reg1 rul0 -- rul t | f )    \ Return a rule restricted to a result region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    2dup rule-result-region-intersects  \ reg1 rul0 bool 
    is-false if
        2drop
        false
        exit
    then

    swap                        \ rul0 reg1
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

: _rule-adjust-mask-m00 ( rul -- )  \ Add one to the m00 mask of a rule.
    dup rule-get-m00        \ rul m00
    1+
    swap                    \ m00 rul
    _rule-set-m00
;

: _rule-adjust-mask-m01 ( rul -- )  \ Add one to the m01 mask of a rule.
    dup rule-get-m01        \ rul m01
    1+
    swap                    \ m01 rul
    _rule-set-m01
;

: _rule-adjust-mask-m11 ( rul -- )  \ Add one to the m11 mask of a rule.
    dup rule-get-m11        \ rul m11
    1+
    swap                    \ m11 rul
    _rule-set-m11
;

: _rule-adjust-mask-m10 ( rul -- )  \ Add one to the m10 mask of a rule.
    dup rule-get-m10        \ rul m10
    1+
    swap                    \ m10 rul
    _rule-set-m10
;

: rule-lshift ( rul -- )    \ Shift all masks left by 1 bit position.
    dup rule-get-m00 1 lshift over _rule-set-m00
    dup rule-get-m01 1 lshift over _rule-set-m01
    dup rule-get-m11 1 lshift over _rule-set-m11
    rule-get-m10 1 lshift over _rule-set-m10
;

\ Givin initial and result chars,
\ left-shift all rule masks, set the right-most rule mask
\ bit positions.  For the rule-from-string function.
: _rule-adjust-masks ( ci cr rul0 -- )  \ Adjust rule, by rule-from-string.
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

\ The slash (/) characters, *including the last*, are important as separators.
\ The uderscore (_) character can also be used.
: rule-from-string ( addr n -- rule )   \ Return a rule from a string, like 00/01/11/10/XX/Xx/X0/X1/
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

: rule-get-changes ( rul0 -- cngs ) \ Return a changes struct, from a rule.
    \ Check arg.
    assert-tos-is-rule

    dup rule-get-m10 swap   \ m10 rul0
    rule-get-m01            \ m10 m01
    changes-new
;

: rule-apply-to-state-fc ( sta1 rul0 -- smpl true | false )  \ Apply a rule to a given state, forward-chaining, returning a sample.
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

: rule-makes-change ( rul0 -- flag )    \ Return true if a rule changes at least one bit.
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

: rule-restrict-to-region ( reg1 rul0 -- rul t | f )    \ Return a rule restricted, initial and result regions, to a given region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    \ Restrict the rule's initial region.
    2dup                            \ reg1 rul0 reg1 rul0
    rule-restrict-initial-region    \ reg1 rul0, rul1' t | f
    is-false if
        2drop
        false
        exit
    then

    \ Restrict the restricted rule's result region to reg1.
                                    \ reg1 rul0 rul1'
    nip                             \ reg1 rul1'
    tuck                            \ rul1' reg1 rul1'
    rule-restrict-result-region     \ rul1', rul2' t | f
    if                              \ rul1' rul2
        swap rule-deallocate        \ rul2'
        true
    else                            \ rul1'
        rule-deallocate
        false
    then
;

: rule-changes-null ( rul0 -- flag )    \ Return true if rule has at least one needed change.
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

: rule-combine2 ( rul1-to rul0-from -- rul )    \ Return a rule by combining tos rule to nos rule.
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

\ Given a rule and reg-to reg-from, return the number of unwanted changes.
\ Used to compare plansteps.
: rule-number-unwanted-changes ( reg-to reg-from rul0 -- u )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region

    \ Get reg-from to rul0 initial region, restrict rule, then get rule result region.
    over                            \ reg-to reg-from rul0 reg-from
    over rule-calc-initial-region   \ reg-to reg-from rul0 reg-from rul-i'
    tuck swap                       \ reg-to reg-from rul0 rul-i' rul-i' reg-from
    region-translate-to-region      \ reg-to reg-from rul0 rul-i' reg-from'
    swap region-deallocate          \ reg-to reg-from rul0 reg-from'

    \ Get rule restricted.
    tuck swap                       \ reg-to reg-from reg-from' reg-from' rul0

    rule-restrict-initial-region    \ reg-to reg-from reg-from', rul0' t | f
    is-false abort" rule-restrict-initial-region: failed?"
    swap region-deallocate          \ reg-to reg-from rul0'

    \ Get rule final result region.
    dup rule-calc-result-region     \ reg-to reg-from rul0' rul-r'
    swap rule-deallocate            \ reg-to reg-from rul-r'

    \ Get changes for reg-from to rule result region.
    2dup swap                       \ reg-to reg-from rul-r' rul-r' reg-from
    changes-new-region-to-region    \ reg-to reg-from rul-r' cngs-from-rul'
    swap region-deallocate          \ reg-to reg-from cngs-from-rul'

    \ Get changes for reg-from to reg-to.
    -rot                            \ cngs-from-rul' reg-to reg-from
    changes-new-region-to-region    \ cngs-from-rul' cngs-from-to'

    \ Get changes in cngs-from-rul not in cngs-from-to.
    dup changes-invert              \ cngs-from-rul' cngs-from-to' cngs-from-to''
    swap changes-deallocate         \ cngs-from-rul' cngs-from-to''
    2dup changes-intersection       \ cngs-from-rul' cngs-from-to'' cngs-from-rul''
    swap changes-deallocate         \ cngs-from-rul' cngs-from-rul''
    swap changes-deallocate         \ cngs-from-rul''

    \ Get unwanted changes.
    dup changes-number-changes      \ cngs-from-rul'' u
    swap changes-deallocate         \ u
;

: rule-calc-for-planstep-by-changes ( cngs1 rul0 -- rul t | f )    \ Return a rule cantaining needed changes for a step for a step.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-changes

    2dup rule-intersects-changes    \ cgs1 rul0 bool
    if
        nip                         \ rul0
        true
    else
        2drop
        false
    then
;

: rule-can-be-used-first  ( reg-to reg-from rul0 -- bool )    \ Return true if reg-from to rule initial region involves no needed changes.
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
    is-false
;

: rule-can-be-used-last  ( reg-to reg-from rul0 -- bool )    \ Return true if reg-from to rule result region involves all needed changes.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." rule-can-be-used-last: rul0: " dup .rule cr

    \ Calc the needed changes, reg-from to reg-to.
    rot                                             \ reg-from rul0 reg-to
    #2 pick                                         \ reg-from rul0 reg-to reg-from
    changes-new-region-to-region                    \ reg-from rul0 cngs-ned'
    -rot                                            \ cngs-ned' reg-from rul0

    \ Calc the rule changes, reg-from to rule result region.
    rule-calc-result-region                         \ cngs-ned' reg-from rul-r'
    tuck swap                                       \ cngs-ned' rul-r' rul-r' reg-from
    changes-new-region-to-region                    \ cngs-ned' rul-r' cngs-rul'
    swap region-deallocate                          \ cngs-ned' cngs-rul'

    \ Check if the rule changes are a superset of the needed changes.
    2dup changes-superset-of                        \ cngs-ned' cngs-rul' bool
    swap changes-deallocate                         \ cngs-ned' bool
    swap changes-deallocate                         \ bool
;

\ Return true if a rule contains at least one needed change, reg-from to reg-to.
: rule-contains-needed-change ( reg-to reg-from rul0 -- bool )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region

    \ Get needed changes.
    -rot                                        \ rul0 reg-to reg-from
    changes-new-region-to-region                \ rul0 ned-cngs'

    \ Check if rule contains at least one needed change.
    tuck swap                                   \ ned-cngs' ned-cngs' rul0
    rule-intersects-changes                     \ ned-cngs' bool
    swap changes-deallocate                     \ bool 
;

\ Return a rule, or a subset of a rule, for forward chaining, from a given region (reg-from), to another, non-intersecting, given region (reg-to), if possible.
\
\ A rule with an initial-region that does not intersect reg-from, but the initial-region does intersect the union of reg-from and reg-to,
\ will necessarily require a needed change to go from reg-from to the rule's initial-region, will therefore be premature to use,
\ and will return false.
\
\ A returned rule will have at least one wanted change.
\
\ A returned rule with an initial-region that does intersect reg-from, with a result intersecting the union of reg-from and reg-to,
\ will have no unwanted changes.
\
\ A returned rule with an initial-region that does not intersect reg-from, will have at least one unwanted change.
\
: rule-calc-for-planstep-fc ( reg-to reg-from rul0 -- rul t | f )
   \  cr ." rule-calc-step-fc: from: " over .region space ." to: " #2 pick .region space ." rule: " dup .rule cr
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                             \ | reg-to reg-from
    swap region-superset-of                     \ | bool
    abort" rule-calc-planstep-fc: 2 region subset?" \ |

    \ Check for needed changes.
    #2 pick #2 pick #2 pick                     \ | reg-to reg-from rul0
    rule-contains-needed-change                 \ | bool
    if
    else
        3drop
        false
        exit
    then

    \ Check if reg-from intersects rule initial-region.

    2dup                                        \ | reg-from rul0
    rule-restrict-initial-region                \ | rul0' t | f
    if                                          \ | rul0'
        \ Clean up.
        2nip nip                                \ rul0'

        \ Return
        true
        exit
    then

    \ reg-from does not intersect rule initial-region.
                                                \ reg-to reg-from rul0 |

    \ Translate reg-from to rul0 initial-region.
    dup rule-calc-initial-region dup            \ reg-to reg-from rul0 | rul-i' rul-i'
    #3 pick                                     \ reg-to reg-from rul0 | rul-i' rul-i' reg-from
    region-translate-to-region                  \ reg-to reg-from rul0 | rul-i' reg-i'
    swap region-deallocate                      \ reg-to reg-from rul0 | reg-i'

    \ Restrict rule.
    dup                                         \ reg-to reg-from rul0 | reg-i' reg-i'
    #2 pick                                     \ reg-to reg-from rul0 | reg-i' reg-i' rul0
    rule-restrict-initial-region                \ reg-to reg-from rul0 | reg-i', rul0' t | f
    is-false abort" rule-restrict-initial-region: failed?"
    swap region-deallocate                      \ reg-to reg-from rul0 | rul0'

    \ Check if rule can be a first rule.
    #3 pick #3 pick #3 pick                     \ reg-to reg-from rul0 | rul0' reg-to reg-from rul0
    rule-can-be-used-first                      \ reg-to reg-from rul0 | rul0' bool
    if
    else
        rule-deallocate
        3drop
        false
        exit
    then

    \ Clean up.
    2nip nip                                     \ rul0'

    \ Return
    true
;

\ Return a rule, or a subset of a rule, if it can be applied to a from-region (tos) to-region (nos) pair, for the purpose of backward chaining.
\ If the rule has a needed change,
\ If the rule result-region intersects reg-to, or
\ going from reg-to to the rule result-region does not contain a needed change, a step will be returned.
: rule-calc-for-planstep-bc ( reg-to reg-from rul0 -- rul t | f )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region
    assert-3os-is-region
    \ cr ." rule-calc-step-bc: to: " #2 pick .region space ." from: " over .region space ." rule: " dup .rule cr

    #2 pick #2 pick                             \ | reg-to reg-from
    swap region-superset-of                     \ | bool
    abort" rule-calc-for-planstep-bc: 2 region subset?" \ |

    \ Check for needed changes.
    #2 pick #2 pick #2 pick                     \ | reg-to reg-from rul0
    rule-contains-needed-change                 \ | bool
    if
    else
        3drop
        false
        exit
    then

    \ Check if rule reg-to intersects rule result-region.
    #2 pick over                                \ reg-to reg-from rul0 | reg-to rul0
    rule-restrict-result-region                 \ | rul0' t | f
    if                                          \ | rul0'
        \ Clean up.                             \ | rul0'
        2nip nip                                \ rul0'

        \ Return
        true
        exit
    then

    \ reg-to does not intersect rule result-region.

                                                \ reg-to reg-from rul0 |
    \ Translate reg-to to rul0 result-region.
    dup rule-calc-result-region dup             \ reg-to reg-from rul0 | rul-r' rul-r'
    #4 pick                                     \ reg-to reg-from rul0 | rul-r' reg-to rul-r' reg-to
    region-translate-to-region                  \ reg-to reg-from rul0 | rul-r' reg-r'
    swap region-deallocate                      \ reg-to reg-from rul0 | reg-r'

    \ Restrict rule.
    dup                                         \ reg-to reg-from rul0 | reg-r' reg-r'
    #2 pick                                     \ reg-to reg-from rul0 | reg-r' reg-r' rul0
    rule-restrict-result-region                 \ reg-to reg-from rul0 | reg-r', rul0' t | f
    is-false abort" rule-restrict-result-region: failed?"
    swap region-deallocate                      \ reg-to reg-from rul0 | rul0'

    \ Check if rule can be the last.            \ reg-to reg-from rul0 | rul0'
    #3 pick #3 pick #2 pick                     \ reg-to reg-from rul0 | rul0' reg-to reg-from rul0'
    rule-can-be-used-last                       \ reg-to reg-from rul0 | rul0' bool
    if
    else
        rule-deallocate
        3drop
        false
        \ cr ." rule-calc-for-planstep-bc: 2 false" cr
        exit
    then

    \ Clean up.
    2nip nip                                     \ rul0'

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

: rule-is-valid ( rul0 - bool ) \ Return true if a rule is valid.
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
    current-all-bits-mask       \ mxx dxx
    \ cr ." mxx: " over .value space ." dxx : " dup .value cr

    =                           \ bool
;

: rule-apply-to-region-fc ( reg1 rul0 -- reg t | f )    \ Return the result of applying a rule to an initial-region intersecting region.
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-region

    \ Restrict rule initial region.
    rule-restrict-initial-region    \ rul0' t | f
    if

        \ Get result region.
        dup rule-calc-result-region     \ rul0' reg

        \ Clean up.
        swap rule-deallocate            \ reg

        true
    else
        false
    then
;
