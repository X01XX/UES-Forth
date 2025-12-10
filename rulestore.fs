\ Implement a RuleStore.
\ This holds zero, one, or two rules.
\ If two rules, order does not matter.

#23173 constant rulestore-id
    #3 constant rulestore-struct-number-cells

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

\ Check TOS for rulestore, unconventional, leaves stack unchanged. 
: assert-tos-is-rulestore ( arg0 -- arg0 )
    dup is-allocated-rulestore
    is-false if
        s" TOS is not an allocated rulestore."
        .abort-xt execute
    then
;

\ Check NOS for rulestore, unconventional, leaves stack unchanged. 
: assert-nos-is-rulestore ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-rulestore
    is-false if
        s" NOS is not an allocated rulestore."
        .abort-xt execute
    then
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
            #2
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

    #2 <
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
        #2 of
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
    else                        \ rul00
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

    -rot                        \ rul01 rs1 rs0
    rulestore-get-rule-1        \ rul01 rs1 rs0-1
    swap                        \ rul01 rs0-1 rs1
    rulestore-get-rule-0        \ rul01 rs0-1 rs1-0

    rule-union                  \ rul01, rul10 true | false
    if                          \ rul01 rul10
        rulestore-new-2
        true
    else                        \ rul01
        rule-deallocate
        false
    then
;

\ Attempt to form union of two pn-2 rulestores, matching
\ rules 0 and 0 and 1 and 1.
: rulestore-union-00-by-changes ( rs1 rs0 -- rs true | false )
    over rulestore-get-rule-0
    over rulestore-get-rule-0

    rule-union-by-changes       \ rs1 rs0, rul00 true | false
    0= if
        2drop
        false
        exit
    then

    -rot                        \ rul00 rs1 rs0
    rulestore-get-rule-1        \ rul00 rs1 rlu1
    swap                        \ rul00 rul1 rs1
    rulestore-get-rule-1        \ rul00 rul1 rul1

    rule-union-by-changes       \ rul00, rul11 true | false
    if                          \ rul00 rul11
        rulestore-new-2
        true
    else                        \ rul00
        rule-deallocate
        false
    then
;

\ Attempt to form union of two pn-2 rulestores, matching
\ rules 0 and 1 and 1 and 0.
: rulestore-union-10-by-changes ( rs1 rs0 -- rs true | false )
    over rulestore-get-rule-1   \ rs1 rs0 rs1-1
    over rulestore-get-rule-0   \ rs1 rs0 rs1-1 rs0-0

    rule-union-by-changes       \ rs1 rs0, rul01 true | false
    0= if
        2drop
        false
        exit
    then

    -rot                        \ rul01 rs1 rs0
    rulestore-get-rule-1        \ rul01 rs1 rs0-1
    swap                        \ rul01 rs0-1 rs1
    rulestore-get-rule-0        \ rul01 rs0-1 rs1-0

    rule-union-by-changes       \ rul01, rul10 true | false
    if                          \ rul01 rul10
        rulestore-new-2
        true
    else                        \ rul01
        rule-deallocate
        false
    then
;

: rulestore-union-by-changes ( rs1 rs0 -- rsx t | f )
   \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-rulestore

    2dup rulestore-union-00-by-changes  \ rs1 rs2, rs3 true | false
    if                                  \ rs1 rs2 rs3
        nip nip true exit
    then
    
    rulestore-union-10-by-changes       \ rs3 true | false
;

\ Return the union of two pn-2 rulestores.
\ Check if one, of two, methods of matching works,
\ but not none or both.
: rulestore-union-2 ( rs1 rs0 -- rsx true | false )
   \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-rulestore

    \ Try union by change-mask only.
    2dup rulestore-union-by-changes \ rs1 rs0, rs3 t | f
    if                              \ rs1 rs0 rs3
        nip nip
        true
        exit
    then

    \ Try union by change-mask and same-result, order 1.
    2dup rulestore-union-00         \ rs1 rs2, rs3 true | false
    if                              \ rs1 rs0 rs3
        nip nip
        true
        exit
    then

    \ Try union by change-mask and same-result, order 2.
    rulestore-union-10         \ rs3 true | false
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
        #2 of
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

\ Given a rulestore (tos), and needed changes (nos), return a list
\ of steps that make the needed changes.
: rulestore-calc-steps-by-changes ( cngs1 ruls0 -- stp-lst )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-changes
    over changes-null
    if
        2drop
        false
        exit
    then

    \ Init return list.
    list-new                                \ cngs1 ruls0 stp-lst
    swap                                    \ cngs1 stp-lst ruls0
    rot                                     \ stp-lst ruls0 cngs1

    \ Process rule 0.
    dup                                     \ stp-lst ruls0 cngs1 cngs1
    #2 pick                                 \ stp-lst ruls0 cngs1 cngs1 ruls0
    rulestore-get-rule-0                    \ stp-lst ruls0 cngs1 cngs1 rul0
    ?dup if
        rule-calc-step-by-changes           \ stp-lst ruls0 cngs1, stp t | f
        if                                  \ stp-lst ruls0 cngs1 stp
            #3 pick                         \ stp-lst ruls0 cngs1 stp stp-lst
            step-list-push-xt execute       \ stp-lst ruls0 cngs1
        then
    else                                    \ stp-lst ruls0 cngs1
        2drop                               \ stp-lst
        exit
    then
                                            \ stp-lst ruls0 cngs1
    \ Process rule 1.
    swap                                    \ stp-lst cngs1 ruls0
    rulestore-get-rule-1                    \ stp-lst cngs1 rul1
    ?dup if
        rule-calc-step-by-changes           \ stp-lst, stp t | f
        if                                  \ stp-lst stp
            over                            \ stp-lst stpx stp-lst
            step-list-push-xt execute       \ stp-lst
        then
    else
        drop                                \ stp-lst
    then
;

\ Given a rulestore, a from region (tos), and a goal region (nos), return a list
\ of steps that may help make the needed changes, for forward chaining.
\ These steps' initial-region may intersect reg-from, or be reachable from reg-from without making a needed change.
: rulestore-calc-steps-fc ( reg-to reg-from ruls0 -- stp-lst )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                                 \ | reg-to reg-from
    2dup region-superset-of                         \ | reg-to reg-from bool
    abort" rulestore-calc-steps-fc: region subset?" \ | reg-to reg-from
    swap region-superset-of                         \ | bool
    abort" rulestore-calc-steps-fc: region subset?" \ |

    \ Init return list.
    list-new                                \ reg-to reg-from ruls0 stp-lst
    swap                                    \ reg-to reg-from stp-lst ruls0
    2swap                                   \ stp-lst ruls0 reg-to reg-from

    \ Process rule 0.
    #2 pick                                 \ stp-lst ruls0 reg-to reg-from ruls0
    rulestore-get-rule-0                    \ stp-lst ruls0 reg-to reg-from rul0
    ?dup if
        #2 pick                             \ stp-lst ruls0 reg-to reg-from rul0 reg-to
        #2 pick                             \ stp-lst ruls0 reg-to reg-from rul0 reg-to reg-from
        rot                                 \ stp-lst ruls0 reg-to reg-from reg-to reg-from rul0
        rule-calc-step-fc                   \ stp-lst ruls0 reg-to reg-from, stp t | f
        if                                  \ stp-lst ruls0 reg-to reg-from stp
            #4 pick                         \ stp-lst ruls0 reg-to reg-from stp stp-lst
            step-list-push-xt execute       \ stp-lst ruls0 reg-to reg-from
        then
    else                                    \ stp-lst ruls0 reg-to reg-from reg-to reg-from
        3drop                               \ stp-lst
        exit
    then
                                            \ stp-lst ruls0 reg-to reg-from
    \ Process rule 1.
    rot                                     \ stp-lst reg-to reg-from ruls0
    rulestore-get-rule-1                    \ stp-lst reg-to reg-from rul1
    ?dup if
        rule-calc-step-fc                   \ stp-lst, stp t | f
        if                                  \ stp-lst stp
            over                            \ stp-lst stpx stp-lst
            step-list-push-xt execute       \ stp-lst
        then
    else
        2drop                               \ stp-lst
    then
;

\ Given a rulestore, a from region (tos), and a goal region (nos), return a list
\ of steps that may help make the needed changes, for backward chaining.
\ These steps' result-region may intersect reg-to, or be reachable from reg-to without making a needed change.
: rulestore-calc-steps-bc ( reg-to reg-from ruls0 -- stp-lst )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-region
    assert-3os-is-region

    \ Init return list.
    list-new                                \ reg-to reg-from ruls0 stp-lst
    swap                                    \ reg-to reg-from stp-lst ruls0
    2swap                                   \ stp-lst ruls0 reg-to reg-from

    \ Process rule 0.
    #2 pick                                 \ stp-lst ruls0 reg-to reg-from ruls0
    rulestore-get-rule-0                    \ stp-lst ruls0 reg-to reg-from rul0
    ?dup if
        #2 pick                             \ stp-lst ruls0 reg-to reg-from rul0 reg-to
        #2 pick                             \ stp-lst ruls0 reg-to reg-from rul0 reg-to reg-from
        rot                                 \ stp-lst ruls0 reg-to reg-from reg-to reg-from rul0
        rule-calc-step-bc                   \ stp-lst ruls0 reg-to reg-from, stp t | f
        if                                  \ stp-lst ruls0 reg-to reg-from stp
            #4 pick                         \ stp-lst ruls0 reg-to reg-from stp stp-lst
            step-list-push-xt execute       \ stp-lst ruls0 reg-to reg-from
        then
    else                                    \ stp-lst ruls0 reg-to reg-from reg-to reg-from
        3drop                               \ stp-lst
        exit
    then
                                            \ stp-lst ruls0 reg-to reg-from
    \ Process rule 1.
    rot                                     \ stp-lst reg-to reg-from ruls0
    rulestore-get-rule-1                    \ stp-lst reg-to reg-from rul1
    ?dup if
        rule-calc-step-bc                   \ stp-lst, stp t | f
        if                                  \ stp-lst stp
            over                            \ stp-lst stpx stp-lst
            step-list-push-xt execute       \ stp-lst
        then
    else
        2drop                               \ stp-lst
    then
;

\ Return a pn value, based on the number of rules stored in a rulestore.
: rulestore-get-pn ( rul-str -- pn )
    \ Check arg.
    assert-tos-is-rulestore

    dup rulestore-get-rule-0            \ rul-str rul0
    0= if
        drop
        #3
        exit
    then

    rulestore-get-rule-1                \ rul1
    if
        #2
    else
        1
    then
;

\ Return true if rulestore has at least one needed change.
: rulestore-makes-change ( cngs1 rul-str0 -- flag )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-changes

    \ Check rule 0.
    2dup                        \ cngs1 rul-str0 cngs1 rul-str0
    rulestore-get-rule-0        \ cngs1 rul-str0 cngs1 rul-0
    rule-makes-change           \ cngs1 rul-str0 flag
    if
        2drop                   \
        true                    \ true
        exit
    then

    \ Check rule 1.             \ cngs1 rul-str0
    rulestore-get-rule-1        \ cngs1 rul-1
    ?dup if
        rule-makes-change       \ flag
    else
        drop                    \
        false                   \ false
    then
;
