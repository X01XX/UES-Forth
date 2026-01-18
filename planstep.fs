\ Implement a planstep struct and functions.
\
\ A planstep may be added to a planstep list in a plan.

#37171 constant planstep-id
    #6 constant planstep-struct-number-cells

\ Struct fields.
0                                       constant planstep-header-disp           \ id (16) use count (16) number unwanted changes (8)
planstep-header-disp            cell+   constant planstep-action-disp           \ An action instance addr.
planstep-action-disp            cell+   constant planstep-rule-disp             \ A rule instance addr.
\ Store frequently used calculated fields, to decrease cycles and memory allocation/deallocation.
planstep-rule-disp              cell+   constant planstep-initial-region-disp   \ A region instance addr.
planstep-initial-region-disp    cell+   constant planstep-result-region-disp    \ A region instance addr.
planstep-result-region-disp     cell+   constant planstep-changes-disp          \ A changes instance addr.

0 value planstep-mma \ Storage for planstep mma instance.

\ Init planstep mma, return the addr of allocated memory.
: planstep-mma-init ( num-items -- ) \ sets planstep-mma.
    dup 1 <
    abort" planstep-mma-init: Invalid number of items."

    cr ." Initializing PlanStep store."
    planstep-struct-number-cells swap mma-new to planstep-mma
;

\ Check planstep mma usage.
: assert-planstep-mma-none-in-use ( -- )
    planstep-mma mma-in-use 0<>
    abort" planstep-mma use GT 0"
;

\ Check instance type.
: is-allocated-planstep ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup planstep-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        planstep-id =
    else
        drop false
    then
;

\ Check TOS for planstep, unconventional, leaves stack unchanged.
: assert-tos-is-planstep ( tos -- tos )
    dup is-allocated-planstep
    is-false if
        s" TOS is not an allocated planstep"
        .abort-xt execute
    then
;

\ Check NOS for planstep, unconventional, leaves stack unchanged.
: assert-nos-is-planstep ( nos tos -- nos tos )
    over is-allocated-planstep
    is-false if
        s" NOS is not an allocated planstep"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the planstep action.
: planstep-get-action ( plnstp0 -- act )
    \ Check arg.
    assert-tos-is-planstep

    planstep-action-disp +  \ Add offset.
    @                       \ Fetch the field.
;

\ Set the action of a planstep instance, use only in this file.
: _planstep-set-action ( u1 plnstp0 -- )
    planstep-action-disp +  \ Add offset.
    !                       \ Set field.
;

\ Return the planstep rule.
: planstep-get-rule ( plnstp0 -- rul )
    \ Check arg.
    assert-tos-is-planstep

    planstep-rule-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the rule of a planstep instance, use only in this file.
: _planstep-set-rule ( rul1 plnstp0 -- )
    planstep-rule-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ Return the planstep initial-region.
: planstep-get-initial-region ( plnstp0 -- reg )
    \ Check arg.
    assert-tos-is-planstep

    planstep-initial-region-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the initial-region of a planstep instance, use only in this file.
: _planstep-set-initial-region ( reg1 plnstp0 -- )
    planstep-initial-region-disp +      \ Add offset.
    !struct                             \ Set the field.
;

\ Return the planstep rule.
: planstep-get-result-region ( plnstp0 -- reg )
    \ Check arg.
    assert-tos-is-planstep

    planstep-result-region-disp +   \ Add offset.
    @                               \ Fetch the field.
;

\ Set the result-region of a planstep instance, use only in this file.
: _planstep-set-result-region ( reg1 plnstp0 -- )
    planstep-result-region-disp +   \ Add offset.
    !struct                         \ Set the field.
;

\ Return the planstep changes.
: planstep-get-changes ( plnstp0 -- cngs )
    \ Check arg.
    assert-tos-is-planstep

    planstep-changes-disp + \ Add offset.
    @                   \ Fetch the field.
;

\ Set the changes of a planstep instance, use only in this file.
: _planstep-set-changes ( cngs1 plnstp0 -- )
    planstep-changes-disp + \ Add offset.
    !struct                 \ Set the field.
;

\ Return planstep number-unwanted-changes.
: planstep-get-number-unwanted-changes ( plnstp0 -- u )
    \ Check arg.
    assert-tos-is-planstep

    4c@
;

\ Set planstep number-unwanted-changes.
: planstep-set-number-unwanted-changes ( u plnstp0 -- )
    \ Check args.
    assert-tos-is-planstep

    4c!
;

' planstep-set-number-unwanted-changes to planstep-set-number-unwanted-changes-xt

\ End accessors.

\ Return a new planstep, given a rule and an action.
: planstep-new    ( rul1 act0 -- plnstp )
    \ Check args.
    assert-tos-is-action-xt execute
    assert-nos-is-rule

   \ Allocate space.
    planstep-mma mma-allocate               \ rul1 a0 plnstpx

    \ Store id.
    planstep-id over                        \ rul1 a0 plnstpx id plnstpx
    struct-set-id                           \ rul1 a0 plnstpx

    \ Init use count.
    0 over struct-set-use-count             \ rul1 a0 plnstpx

    \ Set action.
    tuck                                    \ rul1 plnstpx a0 plnstpx
    _planstep-set-action                    \ rul1 plnstpx

    \ Set initial-region.
    over rule-calc-initial-region           \ rul1 plnstpx reg
    over _planstep-set-initial-region       \ rul1 plnstpx

    \ Set result-region.
    over rule-calc-result-region            \ rul1 plnstpx reg
    over _planstep-set-result-region        \ rul1 plnstpx

    \ Set changes.
    over rule-get-changes                   \ rul1 plnstpx cngs
    over _planstep-set-changes              \ rul1 plnstpx

    \ Set rule.
    tuck                                    \ plnstpx rul1 plnstpx
    _planstep-set-rule                      \ plnstpx

    \ Init number-unwanted-changes.
    0 over                                  \ plnstpx int plnstpx
    planstep-set-number-unwanted-changes    \ plnstpx
;

' planstep-new to planstep-new-xt

: .planstep ( plnstp0 -- )
    \ Check arg.
    assert-tos-is-planstep

    dup planstep-get-action                 \ plnstp0 actx
    action-get-inst-id-xt execute           \ plnstp0 act-id
    ." [ " dec.

    dup planstep-get-rule                   \ plnstp0 rul
    .rule                                   \ plnstp0

    space planstep-get-number-unwanted-changes dec.
    space ." ]"
;

' .planstep to .planstep-xt

\ Deallocate a planstep instance.
: planstep-deallocate ( plnstp0 -- )
    \ Check arg.
    assert-tos-is-planstep

    dup struct-get-use-count      \ plnstp0 count

    #2 <
    if
        \ Deallocate imbedded structs.
        dup planstep-get-rule
        rule-deallocate

        dup planstep-get-initial-region
        region-deallocate

        dup planstep-get-result-region
        region-deallocate

        dup planstep-get-changes
        changes-deallocate

        \ Deallocate instance.
        planstep-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a planstep's changes intersects a given changes.
: planstep-intersects-changes ( cngs1 plnstp0 -- flag )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-changes

    planstep-get-changes            \ cngs1 s-cngs
    changes-intersect               \ flag
;

\ Return a planstep with a rule initial-region restricted by a given region.
: planstep-restrict-initial-region ( reg1 plnstp0 -- plnstp )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-region

    over                                \ reg1 plnstp0 reg1
    over planstep-get-initial-region    \ reg1 plnstp0 reg1 s-reg
    region-intersects                   \ reg1 plnstp0 bool
    is-false abort" no intersection wint planstep initial-region?"

    \ Copy number unwanted changes.
    dup planstep-get-number-unwanted-changes -rot    \ u-unw reg1 plnstp0

    \ Copy action, from planstep.
    dup planstep-get-action -rot        \ u-unw act reg1 plnstp0

    \ Calc new rule.
    planstep-get-rule               \ u-unw act reg1 rul
    rule-restrict-initial-region    \ u-unw act, rul' t | f
    is-false abort" restrict-initial-region failed?"

    \ Make new planstep.
    swap                            \ u-unw rul' act
    planstep-new                    \ u-unw plnstp

    \ Set number unwanted changes.
    tuck planstep-set-number-unwanted-changes   \ plnstp
;

\ Return a planstep with a rule result-region restricted by a given region.
: planstep-restrict-result-region ( reg1 plnstp0 -- plnstp )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-region
    \ cr ." planstep " dup .planstep space ." restrict result to: " over .region

    over                                \ reg1 plnstp0 reg1
    over planstep-get-result-region     \ reg1 plnstp0 reg1 s-reg
    region-intersects                   \ reg1 plnstp0 bool
    is-false abort" no intersection wint planstep result-region?"

    \ Copy number unwanted changes.
    dup planstep-get-number-unwanted-changes -rot    \ u-unw reg1 plnstp0

    \ Copy action, from planstep.
    dup planstep-get-action -rot        \ u-unw act reg1 plnstp0

    \ Calc new rule.
    planstep-get-rule                   \ u-unw act reg1 rul
    rule-restrict-result-region         \ u-unw act, rul' t | f
    is-false abort" restrict-result-region failed?"

    \ Make new planstep.
    swap                        \ u-unw rul' act
    planstep-new                \ u-unw plnstp

    \ Set number unwanted changes.
    tuck planstep-set-number-unwanted-changes   \ plnstp
;

\ Return a result from applying a planstep rule to a state, going forward.
: planstep-apply-to-state-f ( sta1 plnstp0 -- sta )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-value

    planstep-get-rule       \ sta1 rul
    rule-apply-to-state-fc  \ sta
;

\ Return true if two plansteps can be linked plnstp1 result region to plnstp0 initial region.
: planstep-can-be-linked ( plnstp1 plnstp0 -- bool )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-planstep

    swap planstep-get-result-region     \ plnstp0 reg1
    swap planstep-get-initial-region    \ reg1 reg0
    region-intersects                   \ bool
;

\ Return true if a planstep links two regions.
: planstep-links-two-regions ( reg-to reg-from plnstp0 -- bool )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-region
    assert-3os-is-region

                                    \ reg-to reg-from plnstp0
    planstep-get-rule               \ reg-to reg-from s-rul
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

\ Ruturn a planstep restricted, initial and result regions, to a given
\ region.
: planstep-restrict-to-region ( reg1 plnstp0 -- plnstp t | f )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-region

    2dup                    \ reg1 plnstp0 reg1 plnstp0
    planstep-get-rule       \ reg1 plnstp0 reg1 plnstp-rul
    rule-restrict-to-region \ reg1 plnstp0, rul' t | f
    if
        planstep-new        \ reg1 plnstp0 plnstp'
        nip nip             \ plnstp'
        true
    else
        2drop
        false
    then 
;
