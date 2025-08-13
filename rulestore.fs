\ Implement a RuleStore.
\ This holds zero, one, or two rules.
\ If two rules, order does not matter.

23173 constant rulestore-id
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
    over rule-initial-region    \ rul1 rul0 reg1
    over rule-initial-region    \ rul1 rul0 reg1 reg0
    2dup region-eq 0=           \ rul1 rul0 reg1 reg0 flag
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

