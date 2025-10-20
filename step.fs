\ Implement a step struct and functions.

#37171 constant step-id                                                                                  
    #4 constant step-struct-number-cells

\ Struct fields.
0 constant step-header                          \ id (16) use count (16) forward flag (8)
step-header   cell+ constant step-action        \ An action addr.
step-action   cell+ constant step-sample        \ A expected, or desired, sample.
step-sample   cell+ constant step-alt-sample    \ A possible alternate sample, a diversion, not an error, for a pn-2 action.
                                                \ If this is no change, resampling is all that would be needed.
                                                \ May be zero, for a pn-1 action.

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
    dup is-allocated-step 0=
    abort" TOS is not an allocated step"
;

' assert-tos-is-step to assert-tos-is-step-xt

\ Check NOS for step, unconventional, leaves stack unchanged. 
: assert-nos-is-step ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-step 0=
    abort" NOS is not an allocated step"
;

\ Start accessors.

\ Return the step action. 
: step-get-action ( addr -- act )
    \ Check arg.
    assert-tos-is-step

    step-action +       \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the action of a step instance, use only in this file.
: _step-set-action ( u1 addr -- )
    step-action +       \ Add offset.
    !                   \ Set field.
;

\ Return the step sample. 
: step-get-sample ( addr -- smpl )
    \ Check arg.
    assert-tos-is-step

    step-sample +       \ Add offset.
    @                   \ Fetch the field.
;

' step-get-sample to step-get-sample-xt

\ Set the sample of a step instance, use only in this file.
: _step-set-sample ( smpl addr -- )
    step-sample +       \ Add offset.
    !                   \ Set field.
;

\ Return the step alt-sample. 
: step-get-alt-sample ( addr -- smpl )
    \ Check arg.
    assert-tos-is-step

    step-alt-sample +   \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the alt-sample of a step instance, use only in this file.
: step-set-alt-sample ( smpl addr -- )
    step-alt-sample +   \ Add offset.
    !                   \ Set field.
;

' step-set-alt-sample to step-set-alt-sample-xt

\ Return step result state.
: step-get-result ( stp0 -- sta )
    \ Check arg.
    assert-tos-is-step

    step-get-sample
    sample-get-result
;

\ Return step initial state.
: step-get-initial ( stp0 -- sta )
    \ Check arg.
    assert-tos-is-step

    step-get-sample
    sample-get-initial
;

\ Return forward-flag from an step instance.
: step-get-forward ( act0 -- bool)
    \ Check arg.
    assert-tos-is-step

    \ Get forward flag.
    4c@
    0<>     \ Turn 255 into -1
;


\ Set the forward flag of an step instance, use only in this file.
: step-set-forward ( b1 act0 -- )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-bool

    \ Set forward flag, to 0 or 255.
    4c!
;

' step-set-forward to step-set-forward-xt

\ End accessors.

\ Return a new step, given:
\    Action.
\    Sample.
\    Alt-sample (may be zero).
\    Order-matters flag.
\        if false
\            Forward-chaining:  Rule initial region is a superset of the glidepath.
\            Backward-chaining: Rule result region is a superset of the glidepath.
\            i.e. Can be used anytime in the path. 
: step-new    ( alt-smpl2 smpl1 act0 -- step )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-sample

    dup action-get-inst-id          \ as2 s1 act0 act-id
    0<> if
        \ If action not zero, a change must happen.
        over sample-get-states =    \ as2 s1 act0 flag
        abort" Sample makes no change?"
    then

   \ Allocate space.
    step-mma mma-allocate           \ as2 s1 a0 addr

    \ Store id.
    step-id over                    \ as2 s1 a0 addr id addr
    struct-set-id                   \ as2 s1 a0 addr
        
    \ Init use count.
    0 over struct-set-use-count     \ as2 s1 a0 addr

    \ Set action.
    tuck                            \ as2 s1 addr a0 addr
    _step-set-action                \ as2 s1 addr

    \ Set sample.
    tuck                            \ as2 addr s1 addr
    over struct-inc-use-count
    _step-set-sample                \ as2 addr

    \ Check alt-sample.
    over
    ?dup
    if
      assert-tos-is-sample
      struct-inc-use-count
    then

    \ Set alt-sample
    tuck                            \ addr as2 addr
    step-set-alt-sample             \ addr

    \ Default to step forward.
    true over step-set-forward        \ addr
;

' step-new to step-new-xt

\ Print a sample for a step.
: .step-sample ( smpl1 stp0 -- )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-sample

    dup step-get-action                     \ smpl1 stp0 actx
    action-get-inst-id-xt execute           \ smpl1 stp0 act-id
    swap step-get-forward                   \ smpl1 act-id flag
    if                                      \ smpl1 act-id
        over sample-get-initial             \ smpl1 act-id s-i
        .value                              \ smpl1 act-id
        ." -"
        0 <# #S 0 #> type drop              \ smpl1
        ." ->"
        sample-get-result .value            \
    else
                                            \ smpl1 act-id
        over sample-get-initial             \ smpl1 act-id s-i
        .value                              \ smpl1 act-id
        ." <-"
        0 <# #S 0 #> type drop              \ smpl1
        ." -"
        sample-get-result .value            \
    then
;

: .step ( stp0 -- )
    \ Check arg.
    assert-tos-is-step

    dup step-get-sample     \ stp0 smpl
    over                    \ stp0 smpl stp0
    .step-sample            \ stp0

    dup step-get-alt-sample     \ stp0 smpl-alt
    ?dup
    if
        swap                    \ smps-alt stp0
        space ." Alt: " .step-sample
    else
        drop
    then
;

' .step to .step-xt

: step-deallocate ( stp0 -- )
    \ Check arg.
    assert-tos-is-step
    \ cr dup ." dealloc step: " .step cr

    dup struct-get-use-count      \ stp0 count

    #2 <
    if
        \ Deallocate instance.
        dup step-get-sample
        sample-deallocate
        dup step-get-alt-sample
        ?dup if
            sample-deallocate
        then
        step-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a steps' sample has a state that is in a given sample.
: step-intersects-sample ( smpl1 stp0 -- flag )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-sample

    step-get-sample     \ smpl1 stp-smpl
    sample-intersects   \ flag
;

\ Return true if a steps' sample changes intersects a given changes.
: step-intersects-changes ( cngs1 stp0 -- flag )
    \ Check args.
    assert-tos-is-step
    assert-nos-is-changes

    step-get-sample             \ cngs1 stp-smpl
    sample-calc-changes         \ cngs1 stp-cngs
    tuck                        \ stp-cngs csgs1 stp-cngs
    changes-intersect           \ stp-cngs flag
    swap changes-deallocate     \ flag
;

\ Return a forward-chaining step, given a rule, alternate-rule (may be zero) and sample.
: step-new-by-rule-f ( smpl3 alt-rul2 rul1 -- stp true | false )
    \ Check args.
    assert-tos-is-rule
    over if
        assert-nos-is-rule
    then
    assert-3os-is-sample

    \ Get rule forward sample.
    #2 pick over                    \ smpl3 alt-rul2 rul1 | smpl3 rul1
    rule-calc-forward-sample        \ smpl3 alt-rul2 rul1 | rul1-smpl t | f
    0= if                           \ smpl3 alt-rul2 rul1
        3drop
        false                       \ false
        exit
    then

    \ Get alt-rule forward sample, if any.
                                    \ smpl3 alt-rul2 rul1 | rul1-smpl
    #2 pick                         \ smpl3 alt-rul2 rul1 | rul1-smpl alt-rul2
    dup 0<>
    if                              \ smpl3 alt-rul2 rul1 | rul1-smpl alt-rul2
        \ Calc alternate sample, not bounded by smpl3.
        over sample-get-initial     \ smpl3 alt-rul2 rul1 | rul1-smpl alt-rul2 smpl-i
        swap                        \ smpl3 alt-rul2 rul1 | rul1-smpl smpl-i alt-rul2
        rule-apply-to-state-f       \ smpl3 alt-rul2 rul1 | rul1-smpl, alt-rul2-smpl t | f
        0= if                       
            ." Alt sample failed?" abort
        then
    \ else                          \ smpl3 alt-rul2 rul1 | rul1-smpl 0
    then                            \ smpl3 alt-rul2 rul1 | rul1-smpl alt-rul2-smpl

    swap                            \ smpl3 alt-rul2 rul1 | alt-rul2-smpl rul1-smpl
    cur-action-xt execute           \ smpl3 alt-rul2 rul1 | alt-rul2-smpl rul1-smpl act0
    step-new-xt execute             \ smpl3 alt-rul2 rul1 | stpx
    nip nip nip                     \ stpx
    true                            \ stpx true
;

' step-new-by-rule-f to step-new-by-rule-f-xt

\ Return a backward-chaining step, given a rule, alternate-rule (may be zero), and sample.
: step-new-by-rule-b ( smpl3 alt-rul2 rul1 -- stp true | false )
    \ Check args.
    assert-tos-is-rule
    over if
        assert-nos-is-rule
    then
    assert-3os-is-sample

    \ Get rule forward sample.
    #2 pick over                    \ smpl3 alt-rul2 rul1 | smpl3 rul1
    rule-calc-backward-sample       \ smpl3 alt-rul2 rul1 | rul1-smpl t | f
    0= if                           \ smpl3 alt-rul2 rul1
        3drop
        false                       \ false
        exit
    then

    \ Check sample is NE smpl3 result state, and between, inclusive the two sample states.
    
    
                                    \ smpl3 alt-rul2 rul1 | rul1-smpl
    \ Get alt-rule forward sample, if any.
                                    \ smpl3 alt-rul2 rul1 | rul1-smpl
    #2 pick                         \ smpl3 alt-rul2 rul1 | rul1-smpl alt-rul2
    dup 0<>
    if                              \ smpl3 alt-rul2 rul1 | rul1-smpl alt-rul2
        \ Calc alternate sample, not bounded by smpl3.
        over sample-get-initial     \ smpl3 alt-rul2 rul1 | rul1-smpl alt-rul2 smpl-i
        swap                        \ smpl3 alt-rul2 rul1 | rul1-smpl smpl-i alt-rul2
        rule-apply-to-state-f       \ smpl3 alt-rul2 rul1 | rul1-smpl, alt-rul2-smpl t | f
        0= if                       
            ." Alt sample failed?" abort
        then
    \ else                          \ smpl3 alt-rul2 rul1 | rul1-smpl 0
    then                            \ smpl3 alt-rul2 rul1 | rul1-smpl alt-rul2-smpl

    swap                            \ smpl3 alt-rul2 rul1 | alt-rul2-smpl rul1-smpl
    cur-action-xt execute           \ smpl3 alt-rul2 rul1 | alt-rul2-smpl rul1-smpl act0
    step-new-xt execute             \ smpl3 alt-rul2 rul1 | stpx
    false over                      \ smpl3 alt-rul2 rul1 | stpx f stpx
    step-set-forward-xt execute     \ smpl3 alt-rul2 rul1 | stpx
    nip nip nip                     \ stpx
    true                            \ stpx true
;

' step-new-by-rule-b to step-new-by-rule-b-xt
