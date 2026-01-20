\ Tests for plan struct functions.


: plan-test-restrict-initial-region ( act0 -- )

    \ Test 1.

    \ Init restriction region.
    %0000 %0001 region-new              \ reg

    \ Init plan.
    over action-get-parent-domain       \ act0 reg dom
    plan-new                            \ act0 reg pln

    \ Add first step.
    s" 00/XX/01/11/" rule-from-string   \ act0 reg pln rul
    #3 pick                             \ act0 reg pln rul act0
    planstep-new                        \ act0 reg pln stp
    over plan-push-end                  \ act0 reg pln

    \ Add second step.
    s" 01/XX/11/11/" rule-from-string   \ act0 reg pln rul
    #3 pick                             \ act0 reg pln rul ac0
    planstep-new                        \ act0 reg pln stp
    over plan-push-end                  \ act0 reg pln

    cr ." plan1: " dup .plan

    \ Restrict initial-region.
    2dup plan-restrict-initial-region   \ act0 reg pln, pln' t | f

    if                                  \ act0 reg pln pln'
        cr ." plan2: " dup .plan cr
        swap plan-deallocate            \ act0 reg pln'
    else                                \ act0 reg pln
        cr ." plan2: None" cr
        abort
    then

    \ Test results.
    dup plan-get-initial-region         \ act0 reg pln' initial
    %0001 %0001 region-new tuck         \ act0 reg pln' reg2 initial reg2
    region-neq abort" initial regions not eq?"
    region-deallocate                   \ act0 reg pln'

    dup plan-get-result-region          \ act0 reg pln' result
    %1011 %1011 region-new tuck         \ act0 reg pln' reg2 result reg2
    region-neq abort" result regions net eq?"
    region-deallocate                   \ act0 reg pln'

    \ Clean up.
    plan-deallocate
    region-deallocate

    \ Test 2.

    \ Init restriction region.
    %0000 %0100 region-new              \ act0 reg

    \ Init plan.
    over action-get-parent-domain       \ act0 reg dom
    plan-new                            \ act0 reg pln

    \ Add first step.
    s" 00/XX/01/X1/" rule-from-string   \ act0 reg pln rul
    #3 pick                             \ act0 reg pln rul act0
    planstep-new                        \ act0 reg pln stp
    over plan-push-end                  \ act0 reg pln

    \ Add second step.
    s" 01/XX/11/11/" rule-from-string   \ act0 reg pln rul
    #3 pick                             \ act0 reg pln rul act0
    planstep-new                        \ act0 reg pln stp
    over plan-push-end                  \ act0 reg pln

    cr ." plan1: " dup .plan

    \ Restrict initial-region.
    2dup plan-restrict-initial-region   \ act0 reg pln, pln' t | f

    if                                  \ act0 reg pln pln'
        cr ." plan2: " dup .plan cr
        swap -rot                       \ act0 pln reg pln'
    else                                \ act0 reg pln
        cr ." plan2: None" cr
        abort
    then

    \ Test results.
    dup plan-get-initial-region         \ act0 pln reg pln' initial
    %0000 %0100 region-new tuck         \ act0 pln reg pln' reg2 initial reg2
    region-neq abort" initial regions not eq?"
    region-deallocate                   \ act0 pln reg pln'

    dup plan-get-result-region          \ act0 pln reg pln' result
    %1111 %1011 region-new tuck         \ act0 reg pln' reg2 result reg2
    region-neq abort" result regions net eq?"
    region-deallocate                   \ act0 pln reg pln'

    \ Test use count of last step.
    1
    over plan-get-step-list             \ act0 pln reg pln' 1 stp-lst
    list-get-item                       \ act0 pln reg pln' stp1
    struct-get-use-count                \ act0 pln reg pln' u
    #2 <> abort" use count of last step not 2?"

    \ Clean up.
    plan-deallocate
    region-deallocate
    plan-deallocate                     \ act0
    drop

    cr ." plan-test-restrict-initial-region: Ok" cr
;

: plan-test-restrict-result-region ( act0 -- )
    \ Test 1.

    \ Init restriction region.
    %1011 %1010 region-new              \ act0 reg

    \ Init plan.
    over action-get-parent-domain       \ act0 reg dom
    plan-new                            \ act0 reg pln

    \ Add first step.
    s" 00/XX/01/Xx/" rule-from-string   \ act0 reg pln rul
    #3 pick                             \ act0 reg pln rul act0
    planstep-new                        \ act0 reg pln stp
    over plan-push-end                  \ act0 reg pln

    \ Add second step.
    s" 01/XX/11/X1/" rule-from-string   \ act0 reg pln rul
    #3 pick                             \ act0 reg pln rul act0
    planstep-new                        \ act0 reg pln stp
    over plan-push-end                  \ act0 reg pln

    cr ." plan1: " dup .plan

    \ Restrict initial-region.
    2dup plan-restrict-result-region    \ act0 reg pln, pln' t | f

    if                                  \ act0 reg pln pln'
        cr ." plan2: " dup .plan cr
        swap plan-deallocate            \ act0 reg pln'
    else                                \ act0 reg pln
        cr ." plan2: None" cr
        abort
    then

    \ Test results.
    dup plan-get-initial-region         \ act0 reg pln' initial
    %0000 %0001 region-new tuck         \ act0 reg pln' reg2 initial reg2
    region-neq abort" initial regions not eq?"
    region-deallocate                   \ act0 reg pln'

    dup plan-get-result-region          \ act0 reg pln' result
    %1011 %1011 region-new tuck         \ act0 reg pln' reg2 result reg2
    region-neq abort" result regions net eq?"
    region-deallocate                   \ act0 reg pln'

    \ Clean up.
    plan-deallocate
    region-deallocate                   \ act0
    drop

    cr ." plan-test-restrict-result-region: Ok" cr
;

: plan-test-link-step-to-initial-region ( act0 -- )
    cr ." at 1: " .stack-structs-xt execute cr
     \ Init plan.
     dup action-get-parent-domain       \ act0 dom
     plan-new                           \ act0 pln

    \ Add step to plan.
    s" 00/11/01/XX/" rule-from-string   \ act0 pln rul'
    #2 pick                             \ act0 pln rul' act0
    planstep-new                        \ act0 pln stp'
    over plan-push                      \ act0 pln

    \ create step to link.
    s" 10/XX/00/00/" rule-from-string       \ act0 pln rul'
    #2 pick                                 \ act0 pln rul' act0
    planstep-new                            \ act0 pln stp'

    swap                                    \ act0 stp pln'
    2dup plan-link-step-to-initial-region   \ act0 stp pln', pln-l' t | f
    if                                      \ act0 stp pln' pln-l'
        cr ." linked plan " dup .plan cr

        \ Test results.
        dup plan-get-initial-region         \ act0 stp pln' pln-l' initial
        %1100 %1100 region-new tuck         \ act0 stp pln' pln-l' regt' initial regt'
        region-neq abort" initial regions not eq?"
        region-deallocate                   \ act0 stp pln' pln-l'

        dup plan-get-result-region          \ act0 stp pln' pln-l' result
        %0110 %0110 region-new tuck         \ act0 stp pln' pln-l' regt' result regt'
        region-neq abort" result regions net eq?"
        region-deallocate                   \ act0 stp pln' pln-l'

        plan-deallocate                     \ act0 stp pln'
    else
        cr ." link failed"
    then
    plan-deallocate
    planstep-deallocate
    drop

    cr ." plan-test-link-step-to-initial-region: Ok" cr
;

: plan-test-link-step-to-result-region ( act0 -- )
    \ Init plan.
    dup action-get-parent-domain       \ act0 dom
    plan-new                           \ act0 pln'

    \ Add step to plan.
    s" 00/11/01/XX/" rule-from-string   \ act0 pln' rul'
    #2 pick                             \ act0 pln' rul' act0
    planstep-new                        \ act0 pln stp'
    over plan-push                      \ act0 pln

    \ create step to link.
    s" 01/XX/11/00/" rule-from-string       \ act0 pln' rul'
    #2 pick                                 \ act0 pln' rul' act0
    planstep-new                            \ act0 pln' stp'

    2dup swap                               \ act0 pln' stp' stp' pln'
    plan-link-step-to-result-region         \ act0 pln' stp', pln-l t | f
    if                                      \ act0 pln' stp' pln-l'
        cr ." linked plan " dup .plan cr

        \ Test results.
        dup plan-get-initial-region         \ act0 pln' stp' pln-l' initial
        %0100 %0100 region-new tuck         \ act0 pln' stp' pln-l' regt' initial regt'
        region-neq abort" initial regions not eq?"
        region-deallocate                   \ act0 pln' stp' pln-l'

        dup plan-get-result-region          \ act0 pln' stp' pln-l' result
        %1110 %1110 region-new tuck         \ act0 pln' stp' pln-l' regt' result regt'
        region-neq abort" result regions net eq?"
        region-deallocate                   \ act0 pln' stp' pln-l'

        plan-deallocate                     \ act0 pln' stp'
        planstep-deallocate                 \ act0 pln'
    else
        cr ." link failed"
        abort
    then

    plan-deallocate                         \ act0
    drop

    cr ." plan-test-link-step-to-result-region: Ok" cr
;

: plan-tests
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    dup rot                                         \ dom0 dom0 sess
    session-add-domain                              \ dom0

    0 swap domain-find-action                       \ act0 t | f
    is-false abort" act0 not found?"

    dup plan-test-restrict-initial-region
    dup plan-test-restrict-result-region
    dup plan-test-link-step-to-initial-region
    dup plan-test-link-step-to-result-region

                                                    \ act0
    drop

    current-session-deallocate
;
