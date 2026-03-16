\ Implement a planstep struct and functions.
\
\ A planstep may be added to a planstep list in a plan.

#37171 constant planstep-id
    #7 constant planstep-struct-number-cells

\ Struct fields.
0                                       constant planstep-header-disp           \ id (16) use count (16) number unwanted changes (8)
planstep-header-disp            cell+   constant planstep-action-disp           \ An action instance addr.
planstep-action-disp            cell+   constant planstep-rule-disp             \ A rule instance.
planstep-rule-disp              cell+   constant planstep-alt-rule-disp         \ A rule instance, or zero.
\ Store frequently used calculated fields, to decrease cycles and memory allocation/deallocation.
planstep-alt-rule-disp          cell+   constant planstep-initial-region-disp   \ A region instance addr.
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

\ Check instance type.
: is-allocated-planstep ( addr -- flag )
    get-first-word          \ w t | f
    if
        planstep-id =
    else
        false
    then
;

\ Check TOS for planstep, unconventional, leaves stack unchanged.
: assert-tos-is-planstep ( tos -- tos )
    dup is-allocated-planstep
    false? if
        s" TOS is not an allocated planstep"
        .abort-xt execute
    then
;

\ Check NOS for planstep, unconventional, leaves stack unchanged.
: assert-nos-is-planstep ( nos tos -- nos tos )
    over is-allocated-planstep
    false? if
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

\ Return the planstep alt-rule.
: planstep-get-alt-rule ( plnstp0 -- rul )
    \ Check arg.
    assert-tos-is-planstep

    planstep-alt-rule-disp +    \ Add offset.
    @                           \ Fetch the field.
;

\ Set the alt-rule of a planstep instance, use only in this file.
: _planstep-set-alt-rule ( rul1 plnstp0 -- )
    planstep-alt-rule-disp +    \ Add offset.
    over 0=
    if
        !                       \ Set the field to zero.
    else
        !struct                 \ Set the field.
    then
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

\ Return a new planstep, given a rule, group and action.
: planstep-new    ( alt-rul2 rul1 act0 -- plnstp )
    \ Check args.
    assert-tos-is-action-xt execute
    assert-nos-is-rule
    #2 pick
    0=
    if
    else
        assert-3os-is-rule
    then

   \ Allocate space.
    planstep-mma mma-allocate               \ alt-rul2 rul1 act0 plnstpx

    \ Store id.
    planstep-id over                        \ alt-rul2 rul1 act0 plnstpx id plnstpx
    struct-set-id                           \ alt-rul2 rul1 act0 plnstpx

    \ Init use count.
    0 over struct-set-use-count             \ alt-rul2 rul1 act0 plnstpx

    \ Set action.
    tuck                                    \ alt-rul2 rul1 plnstpx act0 plnstpx
    _planstep-set-action                    \ alt-rul2 rul1 plnstpx

    \ Set initial-region.
    over rule-calc-initial-region           \ alt-rul2 rul1 plnstpx reg
    over _planstep-set-initial-region       \ alt-rul2 rul1 plnstpx

    \ Set result-region.
    over rule-calc-result-region            \ alt-rul2 rul1 plnstpx reg
    over _planstep-set-result-region        \ alt-rul2 rul1 plnstpx

    \ Set changes.
    over rule-get-changes                   \ alt-rul2 rul1 plnstpx cngs
    over _planstep-set-changes              \ alt-rul2 rul1 plnstpx

    \ Set rule.
    tuck                                    \ alt-rul2 plnstpx rul1 plnstpx
    _planstep-set-rule                      \ alt-rul2 plnstpx

    \ Set alt-rule
    tuck                                    \ plnstpx alt-rul2 pln-tpx
    _planstep-set-alt-rule                  \ plnstpx

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

        dup planstep-get-alt-rule
        dup 0<>
        if
            rule-deallocate
        else
            drop
        then

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

    \ cr ." planstep-restrict-initial-region: reg: " over .region space dup planstep-get-rule .rule cr

    over                                \ reg1 plnstp0 reg1
    over planstep-get-initial-region    \ reg1 plnstp0 reg1 s-reg
    region-intersects                   \ reg1 plnstp0 bool
    false? abort" no intersection with planstep initial-region?"

    \ Copy number unwanted changes.
    dup planstep-get-number-unwanted-changes -rot    \ u-unw reg1 plnstp0

    \ Copy action, from planstep.
    dup planstep-get-action -rot    \ u-unw act reg1 plnstp0

    \ Calc new rules.
    dup planstep-get-alt-rule swap  \ u-unw act reg1 alt-rul plnstp0
    rot swap                        \ u-unw act alt-rul reg1 plnstp0
    planstep-get-rule               \ u-unw act alt-ru1 reg1 rul
    rule-restrict-initial-region    \ u-unw act alt-rul, rul' t | f
    false? abort" restrict-initial-region failed?"

    \ Make new planstep.
    rot                             \ u-unw alt-rul rul' act
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

    over                                    \ reg1 plnstp0 reg1
    over planstep-get-result-region         \ reg1 plnstp0 reg1 s-reg
    region-intersects                       \ reg1 plnstp0 bool
    false? abort" no intersection wint planstep result-region?"

    \ Get number unwanted changes.
    dup                                     \ reg1 plnstp0 plnstp0
    planstep-get-number-unwanted-changes    \ reg1 plnstp0 u-unw
    -rot                                    \ u-unw reg1 plnstp0
    swap                                    \ u-unw plnstp0 reg1

    \ Get alt-rule
    over planstep-get-alt-rule swap         \ u-unw plnstp0 alt-rul reg1

    \ Get rule.
    #2 pick planstep-get-rule               \ u-unw plnstp0 alt-rul reg1 rul
    rule-restrict-result-region             \ u-unw plnstp0 alt-rul, rul t | f
    false? abort" restrict-result-region failed?"

    \ Make new planstep.                    \ u-unw plnstp0 alt-rul rul
    rot                                     \ u-unw alt-rul rul plnstp0
    planstep-get-action                     \ u-unw alt-rul rul act
    planstep-new                            \ u-unw plnstp

    \ Set number unwanted changes.
    tuck planstep-set-number-unwanted-changes   \ plnstp
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
: ?planstep-links-two-regions ( reg-to reg-from plnstp0 -- bool )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-region
    assert-3os-is-region

                                    \ reg-to reg-from plnstp0
    planstep-get-rule               \ reg-to reg-from s-rul
    rule-restrict-initial-region    \ reg-to, s-rul' t | f
    false? if
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

\ Return the status of a sample, 0 = Expected/wanted, 1 = Unwanted, 2 = Unexpected.
: planstep-result-status ( smpl1 plnstp0 -- status )
    \ Check args.
    assert-tos-is-planstep
    assert-nos-is-sample

    \ Check for Expected/wanted.
    over sample-get-result      \ smpl1 plnstp0 s-rslt
    over                        \ smpl1 plnstp0 s-rslt plnstp0
    planstep-get-result-region  \ smpl1 plnstp0 s-rslt r-rslt
    region-superset-of-state?   \ smpl1 plnstp0 bool
    if
        2drop
        0
        exit
    then

    \ Check if there is an alternate rule.
    \ If no, return Unexpected.
    dup planstep-get-alt-rule   \ smpl1 plnstp0 alt-rul
    0= if
        2drop
        #2
        exit
    then

    \ Get alt rule result for sample initial.
    \ Return Unwanted (matches alt-rule result) or Unexpected.
    over sample-get-initial     \ smpl1 plnstp0 s-ini
    over planstep-get-alt-rule  \ smpl1 plnstp0 s-ini alt-rul
    rule-apply-to-state         \ smpl1 plnstp0 ar-rslt
    #2 pick sample-get-result   \ smpl1 plnstp0 ar-rslt s-rslt
    =
    if
        2drop
        1
    else
        2drop
        #2
    then
;
