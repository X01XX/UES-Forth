\ Implement a RuleStore.
\ This holds zero, one, or two rules.
\ If two rules, order does not matter.

#23173 constant rulestore-id
     3 constant rulestore-struct-number-cells

\ Struct fields
0 constant rulestore-header     \ 16-bits [0] struct id [1] use count.
rulestore-header cell+ constant rulestore-rule-0
rulestore-rule-0 cell+ constant rulestore-rule-1

0 value rulestore-mma \ Storage for rulestore mma instance.

\ Init rulestore mma, return the addr of allocated memory.
: rulestore-mma-init ( num-items -- ) \ sets rulestore-mma.
    cr ." Initializing RuleStore store."
    rulestore-struct-number-cells swap mma-new to rulestore-mma
;

\ Check rulestore mma usage.
: assert-rulestore-mma-none-in-use ( -- )
    rulestore-mma mma-in-use 0<>
    abort" rulestore-mma use GT 0"
;

\ Check instance type.
: is-allocated-rulestore ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup rulestore-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    rulestore-id =     
;

: is-not-allocated-rulestore ( addr -- flag )
    is-allocated-rulestore 0=
;

\ Check TOS for rulestore, unconventional, leaves stack unchanged. 
: assert-tos-is-rulestore ( arg0 -- arg0 )
    dup is-allocated-rulestore 0=
    abort" TOS is not an allocated rulestore."
;

\ Check NOS for rulestore, unconventional, leaves stack unchanged. 
: assert-nos-is-rulestore ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-rulestore 0=
    abort" NOS is not an allocated rulestore."
;

\ Start accessors.

\ Return the first field from a rulestore instance.
: rulestore-get-rule-0 ( addr -- u)
    \ Check arg
    assert-tos-is-rulestore

    rulestore-rule-0 +  \ Add offset.
    @                   \ Fetch the field.
;
 
\ Return the second field from a rulestore instance.
: rulestore-get-rule-1 ( addr -- u)
    \ Check arg
    assert-tos-is-rulestore

    \ Get second rule.
    rulestore-rule-1 +  \ Add offset.
    @                   \ Fetch the field.
;

\ Get both rules.
: rulestore-get-rules ( rulstr -- rul1 rul0 )
    \ Check arg
    assert-tos-is-rulestore

    dup rulestore-get-rule-1    \ rulstr rul1
    swap rulestore-get-rule-0   \ rul1 rul0
;
 
\ Set the first field of a rulestore, use only in this file.
\ The second arg can be zero, or a rule.
: _rulestore-set-rule-0 ( rul0 addr -- )
    \ Check args
    assert-tos-is-rulestore
    over 0<>
    if
        assert-nos-is-rule
    then

    rulestore-rule-0 +  \ Add offset.
    !                   \ Set first field.
;

\ Set the second field of a rulestore, use only in this file.
\ The second arg can be zero, or a rule. 
: _rulestore-set-rule-1 ( rul0 addr -- )
    \ Check args
    assert-tos-is-rulestore
    over 0<>
    if
        assert-nos-is-rule
    then

    rulestore-rule-1 +  \ Add offset.
    !                   \ Set second field.
;

\ End accessors.

\ Return a new rulestore instance, with no rules.
: rulestore-new-0  ( -- rulestore )
    \ Allocate space.                                                                         
    rulestore-mma mma-allocate  \ addr

    \ Store id.
    rulestore-id over           \ addr id addr
    struct-set-id               \ addr
        
    \ Init use count.
    0 over                      \ addr 0 addr
    struct-set-use-count        \ addr

    \ Init rule 0
    0 over                      \ addr 0 addr
    _rulestore-set-rule-0       \ addr

    \ Init rule 1
    0 over                      \ addr 0 addr
    _rulestore-set-rule-1       \ addr
;

\ Return a new rulestore instance, with one rule.
: rulestore-new-1  ( rul0 -- rulestore )
    \ Check arg.
    assert-tos-is-rule

    \ Allocate space.                                                                         
    rulestore-mma mma-allocate  \ rul0 addr

    \ Store id.
    rulestore-id over           \ rul0 addr id addr
    struct-set-id               \ rul0 addr
        
    \ Init use count.
    0 over                      \ rul0 addr 0 addr
    struct-set-use-count        \ rul0 addr

    \ Store rule 0
    over                        \ rul0 addr rul0
    struct-inc-use-count        \ rul0 addr
    tuck                        \ addr rul0 addr
    _rulestore-set-rule-0       \ addr

    \ Init rule 1
    0 over                      \ addr 0 addr
    _rulestore-set-rule-1       \ addr
;

\ Return a new rulestore instance, with two rules.
: rulestore-new-2  ( rul1 rul0 -- rulestore )
    \ Check args.
    assert-tos-is-rule
    assert-nos-is-rule

    \ Check that the rules are not equal.
    2dup rule-eq
    abort" rulestore-new-2: rules cannot be equal."

    \ Check that the rule initial regions are equal.
    over rule-calc-initial-region   \ rul1 rul0 reg1
    over rule-calc-initial-region   \ rul1 rul0 reg1 reg0
    2dup region-eq 0=               \ rul1 rul0 reg1 reg0 flag
    abort" rulestore-new-2: Rules must have the same initial region."

    region-deallocate
    region-deallocate

    \ Allocate space.                                                                         
    rulestore-mma mma-allocate  \ rul1 rul0 addr

    \ Store id.
    rulestore-id over           \ rul1 rul0 addr id addr
    struct-set-id               \ rul1 rul0 addr

    \ Init use count.
    0 over                      \ rul1 rul0 addr 0 addr
    struct-set-use-count        \ rul1 rul0 addr

    swap dup struct-inc-use-count   \ rul1 addr rul0
    over _rulestore-set-rule-0      \ rul1 addr

    swap dup struct-inc-use-count   \ addr rul1
    over _rulestore-set-rule-1      \ addr
;

\ Return number of rules in a RuleStore.
: rulestore-number-rules ( rulstr0 -- u )
    dup rulestore-get-rule-0
    if
        rulestore-get-rule-1
        if
            2
        else
            1
        then
    else
        rulestore-get-rule-1
        abort" Invalid rulestore configuration"
        0
    then
;

\ Deallocate a rulestore.
: rulestore-deallocate ( rs0 -- )
    \ Check args.
    assert-tos-is-rulestore

    dup struct-get-use-count      \ reg0 count

    2 <
    if 
        \ Deallocate/clear fields.
        dup rulestore-get-rule-0
        dup
        if
            rule-deallocate
            0 over _rulestore-set-rule-0
        else
            drop
        then

        dup rulestore-get-rule-1
        dup
        if
            rule-deallocate
            0 over _rulestore-set-rule-1
        else
            drop
        then

        \ Deallocate instance.
        rulestore-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: .rulestore ( rul0 -- )
    \ Check arg.
    assert-tos-is-rulestore

    ." ["
    dup rulestore-get-rule-0
    dup
    if
        .rule
    else
        drop
    then

    rulestore-get-rule-1
    dup if
        space
        .rule
    else
        drop
    then
    ." ]"
;

\ Return a copy of a rulestore.
: rulestore-copy ( rs0 -- rs )
    \ Check arg.
    assert-tos-is-rulestore

    dup rulestore-number-rules  \ rs0 nr
    case
        0 of
            drop
            rulestore-new-0
        endof
        1 of
            rulestore-get-rule-0 rulestore-new-1
        endof
        2 of
            dup rulestore-get-rule-0
            swap rulestore-get-rule-1
            rulestore-new-2
        endof
    endcase
;

\ Attempt to form union of two pn-2 rulestores, matching
\ rules 0 and 0 and 1 and 1.
: rulestore-union-00 ( rs1 rs0 -- rs true | false )
    over rulestore-get-rule-0
    over rulestore-get-rule-0

    rule-union                  \ rs1 rs0, rul00 true | false
    0= if
        2drop
        false
        exit
    then

    -rot                        \ rul00 rs1 rs0
    rulestore-get-rule-1        \ rul00 rs1 rlu1
    swap                        \ rul00 rul1 rs1
    rulestore-get-rule-1        \ rul00 rul1 rul1

    rule-union                  \ rul00, rul11 true | false
    if                          \ rul00 rul11
        rulestore-new-2
        true
    else                        \ rul0
        rule-deallocate
        false
    then
;

\ Attempt to form union of two pn-2 rulestores, matching
\ rules 0 and 1 and 1 and 0.
: rulestore-union-10 ( rs1 rs0 -- rs true | false )
    over rulestore-get-rule-1   \ rs1 rs0 rs1-1
    over rulestore-get-rule-0   \ rs1 rs0 rs1-1 rs0-0

    rule-union                  \ rs1 rs0, rul01 true | false
    0= if
        2drop
        false
        exit
    then
    \ cr ." ru 10 " dup .rule cr

    -rot                        \ rul01 rs1 rs0
    rulestore-get-rule-1        \ rul01 rs1 rs0-1
    swap                        \ rul01 rs0-1 rs1
    rulestore-get-rule-0        \ rul01 rs0-1 rs1-0

    rule-union                  \ rul01, rul10 true | false
    if                          \ rul01 rul10
        \ cr ." ru 01 " dup .rule cr
        rulestore-new-2
        true
    else                        \ rul01
        rule-deallocate
        false
    then
;

\ Return the union of two pn-2 rulestores.
\ Check if one, of two, methods of matching works,
\ but not none or both.
: rulestore-union-2 ( rs1 rs0 -- rsx true | false )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-rulestore

    2dup rulestore-union-00         \ rs1 rs2, rs3 true | false
    if                              \ rs1 rs2 rs3
        -rot                        \ rs3 rs1 rs0
        rulestore-union-10          \ rs3, rs4 true | false
        if                          \ rs3 rs4
            \ cr ." too compatible" cr
            rulestore-deallocate
            rulestore-deallocate
            false
        else                        \ rs3
            true                    \ rs3 true
        then
    else                            \ rs1 rs2
        rulestore-union-10          \ rs3 true | false
    then
;

\ Return the union of two rulestores.
: rulestore-union ( rs1 rs0 -- ret true | false )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-rulestore

    over rulestore-number-rules     \ rs1 rs0 nr1
    over rulestore-number-rules     \ rs1 rs0 nr1 nr0
    tuck                            \ rs1 rs0 nr0 nr1 nr0
    <> abort" rulestores have a different number of rules?"

                                    \ rs1 rs0 nr0

    case
        0 of
            2drop
            rulestore-new-0 true
        endof
        1 of
            rulestore-get-rule-0        \ rs1 r0
            swap rulestore-get-rule-0   \ r0 r1
            rule-union                  \ rule true | false
            if
                rulestore-new-1 true
            else
                false
            then
        endof
        2 of
            rulestore-union-2
        endof
    endcase
;

: rulestore-calc-changes ( rs0 -- cncgs )
    \ Check arg.
    assert-tos-is-rulestore

    \ Init null changes.
    0 0 changes-new swap        \ cngs rs0

    dup rulestore-get-rule-0    \ cngs rs0 rul0
    ?dup
    if
        rule-get-changes        \ cngs rs0 cng0
        rot                     \ rs0 cng0 cngs
        \ Get union changes
        2dup changes-calc-union \ rs0 cng0 cngs cngs'

        \ Cleanup
        swap changes-deallocate
        swap changes-deallocate
        swap                    \ cngs rs0
    else
                                \ cngs rs0
        drop                    \ cngs
        exit
    then

    rulestore-get-rule-1    \ cngs rul1
    ?dup
    if
        rule-get-changes        \ cngs cng1

        \ Get union changes
        2dup changes-calc-union \ cng1 cngs cngs'

        \ Cleanup
        swap changes-deallocate
        swap changes-deallocate \ cngs
    then
                                \ cngs
;

\ Given a rulestore and a desired sample, return a list
\ of steps that may help, in forward-chaining.
\ The steps' samples' states may not match any state in the desired sample.
\ So one option would be to make a plan from the given sample's initial state
\ to the steps' initial state.
: rulestore-get-steps-by-changes-f ( smpl1 ruls0 -- stp-lst )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-sample

    \ Init return list.
    list-new -rot                       \ ret smpl1 ruls0 |
    \ cr ." at 1 " .s cr

    \ Process rule 0.
    2dup rulestore-get-rule-0           \ | smpl1 rul0
    rule-get-sample-by-changes-f        \ | smpl-r0 t | f
    if
        over rulestore-get-rule-1       \ | smpl-r0 rul1
        dup
        if
            over sample-get-initial     \ | smpl-r0 rul1 smpl-r0-i
            tuck swap                   \ | smpl-r0 smpl-r0-i smpl-r0-i rul1
            rule-apply-to-state-f       \ | smpl-r0 smpl-r0-i r1-s-r
            swap sample-new             \ | smpl-r0 smpl-r1
            swap                        \ | smpl-r1 smpl-r0
        else
            swap                        \ | 0 smpl-r0
        then
        cur-action-xt execute
        step-new-xt execute             \ | stpx
        3 pick                          \ | stpx ret
        step-list-push-xt execute       \ |
    then
                                        \ ret smpl1 ruls0 |

    \ Process rule 1.
    dup rulestore-get-rule-1            \ | rul1
    dup if
        2 pick swap                     \ | smpl1 rul1
        rule-get-sample-by-changes-f    \ | smpl-r1 t | f
        if
            dup sample-get-initial      \ | smpl-r1 r1-s-i
            2 pick                      \ | smpl-r1 r1-s-i ruls
            rulestore-get-rule-0        \ | smpl-r1 r1-s-i rul0
            rule-apply-to-state-f       \ | smpl-r1 r1-s-i r0-s-r
            swap sample-new             \ | smpl-r1 smpl-r0
            cur-action-xt execute
            swap step-new-xt execute    \ | stpx
            3 pick                      \ | stpx ret
            step-list-push-xt execute   \ |
        then
        \ cr ." at 2 " .s cr
        2drop
    else
        \ cr ." at 3 " .s cr
        3drop
    then
;

\ Given a rulestore and a desired sample, return a list
\ of steps that may help, in backward-chaining.
\ The steps' samples' states may not match any state in the desired sample.
\ So one option would be to make a plan from the given sample's result state
\ to the steps' result state.
: rulestore-get-steps-by-changes-b ( smpl1 ruls0 -- stp-lst )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-sample

    \ Init return list.
    list-new -rot                       \ ret smpl1 ruls0 |
    \ cr ." at 1 " .s cr

    \ Process rule 0.
    2dup rulestore-get-rule-0           \ | smpl1 rul0
    rule-get-sample-by-changes-b        \ | smpl-r0 t | f
    if
        over rulestore-get-rule-1       \ | smpl-r0 rul1
        dup
        if
            over sample-get-result      \ | smpl-r0 rul1 smpl-r0-i
            tuck swap                   \ | smpl-r0 smpl-r0-i smpl-r0-i rul1
            rule-apply-to-state-b       \ | smpl-r0 smpl-r0-i r1-s-r
            swap sample-new             \ | smpl-r0 smpl-r1
            swap                        \ | smpl-r1 smpl-r0
        else
            swap                        \ | 0 smpl-r0
        then
        cur-action-xt execute
        step-new-xt execute             \ | stpx
        3 pick                          \ | stpx ret
        step-list-push-xt execute       \ |
    then
                                        \ ret smpl1 ruls0 |

    \ Process rule 1.
    dup rulestore-get-rule-1            \ | rul1
    dup if
        2 pick swap                     \ | smpl1 rul1
        rule-get-sample-by-changes-b    \ | smpl-r1 t | f
        if
            dup sample-get-result       \ | smpl-r1 r1-s-i
            2 pick                      \ | smpl-r1 r1-s-i ruls
            rulestore-get-rule-0        \ | smpl-r1 r1-s-i rul0
            rule-apply-to-state-b       \ | smpl-r1 r1-s-i r0-s-r
            swap sample-new             \ | smpl-r1 smpl-r0
            cur-action-xt execute
            swap step-new-xt execute    \ | stpx
            3 pick                      \ | stpx ret
            step-list-push-xt execute   \ |
        then
        \ cr ." at 2 " .s cr
        2drop
    else
        \ cr ." at 3 " .s cr
        3drop
    then
;
