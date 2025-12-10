\ Implement a step struct and functions.
\
\ A step may be added to a step list in a plan.

#37171 constant step-id                                                                                  
    #6 constant step-struct-number-cells

\ Struct fields.
0 constant step-header                                              \ id (16) use count (16) number unwanted changes (8)
step-header                 cell+ constant step-action-disp         \ An action instance addr.
step-action-disp            cell+ constant step-rule-disp           \ A rule instance addr.
\ Store frequently used calculated fields, to decrease cycles and memory allocation/deallocation.
step-rule-disp              cell+ constant step-initial-region-disp \ A region instance addr.
step-initial-region-disp    cell+ constant step-result-region-disp  \ A region instance addr.
step-result-region-disp     cell+ constant step-changes-disp        \ A changes instance addr.

0 value step-mma \ Storage for step mma instance.

\ Init step mma, return the addr of allocated memory.
: step-mma-init ( num-items -- ) \ sets step-mma.
    dup 1 < 
    abort" step-mma-init: Invalid number of items."

    cr ." Initializing Step store."
    step-struct-number-cells swap mma-new to step-mma
;

\ Check step mma usage.
: assert-step-mma-none-in-use ( -- )
    step-mma mma-in-use 0<>
    abort" step-mma use GT 0"
;

\ Check instance type.
: is-allocated-step ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup step-mma mma-within-array 0=
    if  
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    step-id =    
;

\ Check TOS for step, unconventional, leaves stack unchanged. 
: assert-tos-is-step ( arg0 -- arg0 )
    dup is-allocated-step
    is-false if
        s" TOS is not an allocated step"
        .abort-xt execute
    then
;

\ Check NOS for step, unconventional, leaves stack unchanged. 
: assert-nos-is-step ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-step
    is-false if
        s" NOS is not an allocated step"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the step action. 
: step-get-action ( stp0 -- act )
    \ Check arg.
    assert-tos-is-step

    step-action-disp +  \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the action of a step instance, use only in this file.
: _step-set-action ( u1 stp0 -- )
    step-action-disp +  \ Add offset.
    !                   \ Set field.
;

\ Return the step rule. 
: step-get-rule ( stp0 -- rul )
    \ Check arg.
    assert-tos-is-step

    step-rule-disp +    \ Add offset.
    @                   \ Fetch the field.
;

\ Set the rule of a step instance, use only in this file.
: _step-set-rule ( rul1 stp0 -- )
    step-rule-disp +    \ Add offset.
    !                   \ Set field.
;

\ Return the step initial-region. 
: step-get-initial-region ( stp0 -- reg )
    \ Check arg.
    assert-tos-is-step

    step-initial-region-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Set the initial-region of a step instance, use only in this file.
: _step-set-initial-region ( reg1 stp0 -- )
    step-initial-region-disp +      \ Add offset.
    !                               \ Set field.
;

\ Return the step rule. 
: step-get-result-region ( stp0 -- reg )
    \ Check arg.
    assert-tos-is-step

    step-result-region-disp +   \ Add offset.
    @                           \ Fetch the field.
;

\ Set the result-region of a step instance, use only in this file.
: _step-set-result-region ( reg1 stp0 -- )
    step-result-region-disp +   \ Add offset.
    !                           \ Set field.
;

\ Return the step changes. 
: step-get-changes ( stp0 -- cngs )
    \ Check arg.
    assert-tos-is-step

    step-changes-disp + \ Add offset.
    @                   \ Fetch the field.
;

\ Set the changes of a step instance, use only in this file.
: _step-set-changes ( cngs1 stp0 -- )
    step-changes-disp + \ Add offset.
    !                   \ Set field.
;

\ Return step number-unwanted-changes.
: step-get-number-unwanted-changes ( stp0 -- u )
    \ Check arg.
    assert-tos-is-step

    4c@
;

\ Set step number-unwanted-changes.
: step-set-number-unwanted-changes ( u stp0 -- )
    \ Check args.
    assert-tos-is-step

    4c!
;

' step-set-number-unwanted-changes to step-set-number-unwanted-changes-xt

\ End accessors.

\ Return a new step, given a rule and an action.
: step-new    ( rul1 act0 -- step )
    \ Check args.
    assert-tos-is-action-xt execute
    assert-nos-is-rule

\    over rule-is-valid              \ rul1 stp0 bool
\    if
\    else
\        cr ." step-new: invalid rule? " over .rule cr
\        abort
\    then

   \ Allocate space.
    step-mma mma-allocate           \ rul1 a0 stpx

    \ Store id.
    step-id over                    \ rul1 a0 stpx id stpx
    struct-set-id                   \ rul1 a0 stpx

    \ Init use count.
    0 over struct-set-use-count     \ rul1 a0 stpx

    \ Set action.
    tuck                            \ rul1 stpx a0 stpx
    _step-set-action                \ rul1 stpx

    \ Set initial-region.
    over rule-calc-initial-region   \ rul1 stpx reg
    1 over struct-set-use-count
    over _step-set-initial-region   \ rul1 stpx

    \ Set result-region.
    over rule-calc-result-region   \ rul1 stpx reg
    1 over struct-set-use-count
    over _step-set-result-region   \ rul1 stpx

    \ Set changes.
    over rule-get-changes           \ rul1 stpx cngs
    1 over struct-set-use-count
    over _step-set-changes          \ rul1 stpx

    \ Set rule.
    tuck                            \ stpx rul1 stpx
    over struct-inc-use-count
    _step-set-rule                  \ stpx

    \ Init number-unwanted-changes.
    0 over                              \ stpx int stpx
    step-set-number-unwanted-changes    \ stpx
;

' step-new to step-new-xt

: .step ( stp0 -- )
    \ Check arg.
    assert-tos-is-step

    dup step-get-action                     \ stp0 actx
    action-get-inst-id-xt execute           \ stp0 act-id
    ." [ " dec.

    dup step-get-rule       \ stp0 rul
    .rule                   \ stp0

    space step-get-number-unwanted-changes dec.
    space ." ]"
;

' .step to .step-xt

\ Deallocate a step instance.
: step-deallocate ( stp0 -- )
    \ Check arg.
    assert-tos-is-step

    dup struct-get-use-count      \ stp0 count

    #2 <
    if
        \ Deallocate imbedded structs.
        dup step-get-rule
        rule-deallocate

        dup step-get-initial-region
        region-deallocate

        dup step-get-result-region
        region-deallocate

        dup step-get-changes
        changes-deallocate

        \ Deallocate instance.
        step-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a step's changes intersects a given changes.
: step-intersects-changes ( cngs1 stp0 -- flag )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-changes

    step-get-changes            \ cngs1 s-cngs
    changes-intersect           \ flag
;

\ Return a step with a rule initial-region restricted by a given region.
: step-restrict-initial-region ( reg1 stp0 -- stp )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-region

    over                            \ reg1 stp0 reg1
    over step-get-initial-region    \ reg1 stp0 reg1 s-reg
    region-intersects               \ reg1 stp0 bool
    is-false abort" no intersection wint step initial-region?"

    \ Copy number unwanted changes.
    dup step-get-number-unwanted-changes -rot    \ u-unw reg1 stp0

    \ Copy action, from step.
    dup step-get-action -rot        \ u-unw act reg1 stp0

    \ Calc new rule.
    step-get-rule                   \ u-unw act reg1 rul
    rule-restrict-initial-region    \ u-unw act, rul' t | f
    is-false abort" rule-restrict-initial-region failed?"

    \ Make new step.
    swap                        \ u-unw rul' act
    step-new                    \ u-unw stp

    \ Set number unwanted changes.
    tuck step-set-number-unwanted-changes   \ stp
;

\ Return a step with a rule result-region restricted by a given region.
: step-restrict-result-region ( reg1 stp0 -- stp )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-region
    \ cr ." step " dup .step space ." restrict result to: " over .region

    over                            \ reg1 stp0 reg1
    over step-get-result-region     \ reg1 stp0 reg1 s-reg
    region-intersects               \ reg1 stp0 bool
    is-false abort" no intersection wint step result-region?"

    \ Copy number unwanted changes.
    dup step-get-number-unwanted-changes -rot    \ u-unw reg1 stp0

    \ Copy action, from step.
    dup step-get-action -rot        \ u-unw act reg1 stp0

    \ Calc new rule.
    step-get-rule                   \ u-unw act reg1 rul
    rule-restrict-result-region     \ u-unw act, rul' t | f
    is-false abort" rule-restrict-result-region failed?"

    \ Make new step.
    swap                        \ u-unw rul' act
    step-new                    \ u-unw stp

    \ Set number unwanted changes.
    tuck step-set-number-unwanted-changes   \ stp
;

\ Return a result from applying a step rule to a state, going forward.
: step-apply-to-state-f ( sta1 stp0 -- sta )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-value

    step-get-rule           \ sta1 rul
    rule-apply-to-state-f   \ sta
;

\ Return true if two steps can be linked stp1 result region to stp0 initial region.
: step-can-be-linked ( stp1 stp0 -- bool )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-step

    swap step-get-result-region     \ stp0 reg1
    swap step-get-initial-region    \ reg1 reg0
    region-intersects               \ bool
;

\ Return true if a step links two regions.
: step-links-two-regions ( reg-to reg-from stp0 -- bool )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-region
    assert-3os-is-region

                                    \ reg-to reg-from stp0
    step-get-rule                   \ reg-to reg-from s-rul
    rule-restrict-initial-region    \ reg-to, s-rul' t | f
    is-false if
        drop
        false
        exit
    then

                                    \ reg-to s-rul'
    tuck                            \ s-rul' reg-to s-rul'
    rule-restrict-result-region     \ s-rul', s-rul'' t | f
    if
        rule-deallocate
        rule-deallocate
        true
    else
        rule-deallocate
        false
    then
;

\ Return a copy of a step.
: step-copy ( stp0 -- stp )
    \ Check arg.
    assert-tos-is-step

    dup step-get-rule                   \ stp0 rul
    over step-get-action                \ stp0 rul act
    step-new                            \ stp0 stp
    swap                                \ stp stp0
    step-get-number-unwanted-changes    \ stp u-unw
    over                                \ stp u-unw stp
    step-set-number-unwanted-changes    \ stp
;
