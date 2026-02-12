\ Implement a RuleStore.
\ This holds zero, one, or two rules.
\ If two rules, order does not matter.

#23173 constant rulestore-id
    #3 constant rulestore-struct-number-cells

\ Struct fields
0                               constant rulestore-header-disp  \ 16-bits, [0] struct id, [1] use count.
rulestore-header-disp   cell+   constant rulestore-rule-0-disp  \ Rule 0, or null.
rulestore-rule-0-disp   cell+   constant rulestore-rule-1-disp  \ Rule 1, or null.  Guaranteed null if rule 0 is null.

0 value rulestore-mma \ Storage for rulestore mma instance.

\ Init rulestore mma, return the addr of allocated memory.
: rulestore-mma-init ( num-items -- ) \ sets rulestore-mma.
    cr ." Initializing RuleStore store."
    rulestore-struct-number-cells swap mma-new to rulestore-mma
;

\ Check instance type.
: is-allocated-rulestore ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup rulestore-mma mma-within-array
    if
        struct-get-id
        rulestore-id =
    else
        drop false
    then
;

\ Check TOS for rulestore, unconventional, leaves stack unchanged.
: assert-tos-is-rulestore ( tos -- tos )
    dup is-allocated-rulestore
    is-false if
        s" TOS is not an allocated rulestore."
        .abort-xt execute
    then
;

\ Check NOS for rulestore, unconventional, leaves stack unchanged.
: assert-nos-is-rulestore ( nos tos -- nos tos )
    over is-allocated-rulestore
    is-false if
        s" NOS is not an allocated rulestore."
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the rule-0 field from a rulestore instance.
: rulestore-get-rule-0 ( rulstr0 -- rul )
    \ Check arg
    assert-tos-is-rulestore

    rulestore-rule-0-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Return the rule-1 field from a rulestore instance.
: rulestore-get-rule-1 ( rulstr0 -- rul )
    \ Check arg
    assert-tos-is-rulestore

    \ Get second rule.
    rulestore-rule-1-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Set the first field of a rulestore, use only in this file.
\ The second arg can be zero, or a rule.
: _rulestore-set-rule-0 ( rul1 rulstr0 -- )
    \ Check args
    assert-tos-is-rulestore

    rulestore-rule-0-disp + \ Add offset.

    \ Check if the passed value is zero or a rule.
    over
    if
        assert-nos-is-rule

        !struct                 \ Set the field.
    else
        !                       \ Set the field.
    then
;

\ Set the second field of a rulestore, use only in this file.
\ The second arg can be zero, or a rule.
: _rulestore-set-rule-1 ( rul1 rultr0 -- )
    \ Check args
    assert-tos-is-rulestore

    rulestore-rule-1-disp +     \ Add offset.

    \ Check if the passed value is zero or a rule.
    over
    if
        assert-nos-is-rule

        !struct                 \ Set the field.
    else
        !                       \ Set the field.
    then
;

\ End accessors.

\ Return a new rulestore instance, with no rules.
: rulestore-new-0  ( -- rulstr )
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
: rulestore-new-1  ( rul0 -- rulstr )
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
    tuck                        \ addr rul0 addr
    _rulestore-set-rule-0       \ addr

    \ Init rule 1
    0 over                      \ addr 0 addr
    _rulestore-set-rule-1       \ addr
;

\ Return a new rulestore instance, with two rules.
: rulestore-new-2  ( rul1 rul0 -- rulstr )
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

    swap                            \ rul1 addr rul0
    over _rulestore-set-rule-0      \ rul1 addr

    swap                            \ addr rul1
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
: rulestore-deallocate ( rulstr0 -- )
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
: rulestore-copy ( rulstr0 -- rulstr )
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
: rulestore-union-00 ( rulstr1 rulstr0 -- rulstr true | false )
    over rulestore-get-rule-0
    over rulestore-get-rule-0

    rule-union                  \ rulstr1 rulstr0, rul00 true | false
    0= if
        2drop
        false
        exit
    then

    -rot                        \ rul00 rulstr1 rulstr0
    rulestore-get-rule-1        \ rul00 rulstr1 rlu1
    swap                        \ rul00 rul1 rulstr1
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
: rulestore-union-10 ( rulstr1 rulstr0 -- rulstr true | false )
    over rulestore-get-rule-1   \ rulstr1 rulstr0 rulstr1-1
    over rulestore-get-rule-0   \ rulstr1 rulstr0 rulstr1-1 rulstr0-0

    rule-union                  \ rulstr1 rulstr0, rul01 true | false
    0= if
        2drop
        false
        exit
    then

    -rot                        \ rul01 rulstr1 rulstr0
    rulestore-get-rule-1        \ rul01 rulstr1 rulstr0-1
    swap                        \ rul01 rulstr0-1 rulstr1
    rulestore-get-rule-0        \ rul01 rulstr0-1 rulstr1-0

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
: rulestore-union-00-by-changes ( rulstr1 rulstr0 -- rulstr true | false )
    over rulestore-get-rule-0
    over rulestore-get-rule-0

    rule-union-by-changes       \ rulstr1 rulstr0, rul00 true | false
    0= if
        2drop
        false
        exit
    then

    -rot                        \ rul00 rulstr1 rulstr0
    rulestore-get-rule-1        \ rul00 rulstr1 rlu1
    swap                        \ rul00 rul1 rulstr1
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
: rulestore-union-10-by-changes ( rulstr1 rulstr0 -- rulstr true | false )
    over rulestore-get-rule-1   \ rulstr1 rulstr0 rulstr1-1
    over rulestore-get-rule-0   \ rulstr1 rulstr0 rulstr1-1 rulstr0-0

    rule-union-by-changes       \ rulstr1 rulstr0, rul01 true | false
    0= if
        2drop
        false
        exit
    then

    -rot                        \ rul01 rulstr1 rulstr0
    rulestore-get-rule-1        \ rul01 rulstr1 rulstr0-1
    swap                        \ rul01 rulstr0-1 rulstr1
    rulestore-get-rule-0        \ rul01 rulstr0-1 rulstr1-0

    rule-union-by-changes       \ rul01, rul10 true | false
    if                          \ rul01 rul10
        rulestore-new-2
        true
    else                        \ rul01
        rule-deallocate
        false
    then
;

: rulestore-union-by-changes ( rulstr1 rulstr0 -- rulstr t | f )
   \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-rulestore

    2dup rulestore-union-00-by-changes  \ rulstr1 rulstr2, rulstr3 true | false
    if                                  \ rulstr1 rulstr2 rulstr3
        nip nip true exit
    then

    rulestore-union-10-by-changes       \ rulstr3 true | false
;

\ Return the union of two pn-2 rulestores.
\ Check if one, of two, methods of matching works,
\ but not none or both.
: rulestore-union-2 ( rulstr1 rulstr0 -- rulstr true | false )
   \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-rulestore

    \ Try union by change-mask only.
    2dup rulestore-union-by-changes \ rulstr1 rulstr0, rulstr3 t | f
    if                              \ rulstr1 rulstr0 rulstr3
        nip nip
        true
        exit
    then

    \ Try union by change-mask and same-result, order 1.
    2dup rulestore-union-00         \ rulstr1 rulstr2, rulstr3 true | false
    if                              \ rulstr1 rulstr0 rulstr3
        nip nip
        true
        exit
    then

    \ Try union by change-mask and same-result, order 2.
    rulestore-union-10         \ rulstr3 true | false
;

\ Return the union of two rulestores.

: rulestore-union ( rulstr1 rulstr0 -- rulstr true | false )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-rulestore

    over rulestore-number-rules     \ rulstr1 rulstr0 nr1
    over rulestore-number-rules     \ rulstr1 rulstr0 nr1 nr0
    tuck                            \ rulstr1 rulstr0 nr0 nr1 nr0
    <> abort" rulestores have a different number of rules?"

                                    \ rulstr1 rulstr0 nr0

    case
        0 of
            2drop
            rulestore-new-0 true
        endof
        1 of
            rulestore-get-rule-0        \ rulstr1 r0
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

: rulestore-calc-changes ( rulstr0 -- cncgs )
    \ Check arg.
    assert-tos-is-rulestore

    \ Init null changes.
    0 0 changes-new swap        \ cngs rulstr0

    dup rulestore-get-rule-0    \ cngs rulstr0 rul0
    ?dup
    if
        rule-get-changes        \ cngs rulstr0 cng0
        rot                     \ rulstr0 cng0 cngs
        \ Get union changes
        2dup changes-calc-union \ rulstr0 cng0 cngs cngs'

        \ Cleanup
        swap changes-deallocate
        swap changes-deallocate
        swap                    \ cngs rulstr0
    else
                                \ cngs rulstr0
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
\ of rules that make the needed changes.
: rulestore-calc-for-plansteps-by-changes ( cngs1 ruls0 -- rul-lst )
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
    list-new                                \ cngs1 ruls0 rul-lst

    \ Process rule 0.
    over                                    \ cngs1 ruls0 rul-lst ruls0
    rulestore-get-rule-0                    \ cngs1 ruls0 rul-lst rul0
    ?dup if
        #3 pick                             \ cngs1 ruls0 rul-lst rul0 cngs1
        swap                                \ cngs1 ruls0 rul-lst cngs1 rul0
        rule-calc-for-planstep-by-changes   \ cngs1 ruls0 rul-lst, rul t | f
        if                                  \ cngs1 ruls0 rul-lst rul
            over                            \ cngs1 ruls0 rul-lst rul rul-lst
            list-push-struct                \ cngs1 ruls0 rul-lst
        then
    else                                    \ cngs1 ruls0 rul-lst
        nip nip                             \ rul-lst
        exit
    then
                                            \ cngs1 ruls0 rul-lst
    \ Process rule 1.
    over                                    \ cngs1 ruls0 rul-lst ruls0
    rulestore-get-rule-1                    \ cngs1 ruls0 rul-lst rul1
    ?dup if
        #3 pick                             \ cngs1 ruls0 rul-lst rul1 cngs1
        swap                                \ cngs1 ruls0 rul-lst cngs1 rul1
        rule-calc-for-planstep-by-changes   \ cngs1 ruls0 rul-lst, rul t | f
        if                                  \ cngs1 ruls0 rul-lst rul
            over                            \ cngs1 ruls0 rul-lst rulx rul-lst
            list-push-struct                \ cngs1 ruls0 rul-lst
        then
    then
                                            \ cngs1 ruls0 rul-lst
    nip nip                                 \ rul-lst
;

\ Given a rulestore, a from region (tos), and a goal region (nos), return a list
\ of rules that may help make the needed changes, for forward chaining.
\ These rule's initial-region may intersect reg-from, or be reachable from reg-from without making a needed change.
: rulestore-calc-for-plansteps-fc ( reg-to reg-from ruls0 -- rul-lst )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                                     \ | reg-to reg-from
    swap region-superset-of                             \ | bool
    abort" rulestore-calc-plansteps-fc 2: region subset?"

    \ Init return list.
    list-new                                \ reg-to reg-from ruls0 rul-lst

    \ Process rule 0.
    over                                    \ reg-to reg-from ruls0 rul-lst ruls0
    rulestore-get-rule-0                    \ reg-to reg-from ruls0 rul-lst rul0
    ?dup if
        #4 pick                             \ reg-to reg-from ruls0 rul-lst rul0 reg-to
        #4 pick                             \ reg-to reg-from ruls0 rul-lst rul0 reg-to reg-from
        rot                                 \ reg-to reg-from ruls0 rul-lst reg-to reg-from rul0
        rule-calc-for-planstep-fc           \ reg-to reg-from ruls0 rul-lst, rul t | f
        if                                  \ reg-to reg-from ruls0 rul-lst rul
            over                            \ reg-to reg-from ruls0 rul-lst rul rul-lst
            list-push-struct                \ reg-to reg-from ruls0 rul-lst
        then
    else                                    \ reg-to reg-from ruls0 rul-lst
        2nip                                \ ruls0 rul-lst
        nip                                 \ rul-lst
        exit
    then
                                            \ reg-to reg-from ruls0 rul-lst
    \ Process rule 1.
    over                                    \ reg-to reg-from ruls0 rul-lst ruls0
    rulestore-get-rule-1                    \ reg-to reg-from ruls0 rul-lst rul1
    ?dup if
        #4 pick #4 pick                     \ reg-to reg-from ruls0 rul-lst rul1 reg-to reg-from
        rot                                 \ reg-to reg-from ruls0 rul-lst reg-to reg-from rul1
        rule-calc-for-planstep-fc           \ reg-to reg-from ruls0 rul-lst, rul t | f
        if                                  \ reg-to reg-from ruls0 rul-lst rul
            over                            \ reg-to reg-from ruls0 rul-lst rulx rul-lst
            list-push-struct                \ reg-to reg-from ruls0 rul-lst
        then
    then
                                            \ reg-to reg-from ruls0 rul-lst
    2nip                                    \ ruls0 rul-lst
    nip                                     \ rul-lst
;

\ Given a rulestore, a from region (tos), and a goal region (nos), return a list
\ of rules that may help make the needed changes, for backward chaining.
\ These rule's result-region may intersect reg-to, or be reachable from reg-to without making a needed change.
: rulestore-calc-for-plansteps-bc ( reg-to reg-from ruls0 -- rul-lst )
    \ Check args.
    assert-tos-is-rulestore
    assert-nos-is-region
    assert-3os-is-region

    \ Init return list.
    list-new                                \ reg-to reg-from ruls0 rul-lst

    \ Process rule 0.
    over                                    \ reg-to reg-from ruls0 rul-lst ruls0
    rulestore-get-rule-0                    \ reg-to reg-from ruls0 rul-lst rul0
    ?dup if
        #4 pick                             \ reg-to reg-from ruls0 rul-lst rul0 reg-to
        #4 pick                             \ reg-to reg-from ruls0 rul-lst rul0 reg-to reg-from
        rot                                 \ reg-to reg-from ruls0 rul-lst reg-to reg-from rul0
        rule-calc-for-planstep-bc           \ reg-to reg-from ruls0 rul-lst, rul t | f
        if                                  \ reg-to reg-from ruls0 rul-lst rul
            over                            \ reg-to reg-from ruls0 rul-lst rul rul-lst
            list-push-struct                \ reg-to reg-from ruls0 rul-lst
        then
    else                                    \ reg-to reg-from ruls0 rul-lst
        2nip                                \ ruls0 rul-lst
        nip                                 \ rul-lst
        exit
    then
                                            \ reg-to reg-from ruls0 rul-lst
    \ Process rule 1.
    over                                    \ reg-to reg-from ruls0 rul-lst ruls0
    rulestore-get-rule-1                    \ reg-to reg-from ruls0 rul-lst rul1
    ?dup if
        #4 pick #4 pick                     \ reg-to reg-from ruls0 rul-lst rul1 reg-to reg-from
        rot                                 \ reg-to reg-from ruls0 rul-lst reg-to reg-from rul1
        rule-calc-for-planstep-bc           \ reg-to reg-from ruls0 rul-lst, rul t | f
        if                                  \ reg-to reg-from ruls0 rul-lst rul
            over                            \ reg-to reg-from ruls0 rul-lst rulx rul-lst
            list-push-struct                \ reg-to reg-from ruls0 rul-lst
        then
    then
                                            \ reg-to reg-from ruls0 rul-lst
    2nip                                    \ ruls0 rul-lst
    nip                                     \ rul-lst
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
