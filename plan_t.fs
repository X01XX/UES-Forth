\ Tests for plan struct functions.


: plan-test-restrict-initial-region

    \ Test 1.

    \ Init restriction region.
    %0000 %0001 region-new              \ reg

    \ Init plan.
    cur-domain-xt execute plan-new      \ reg pln

    \ Add first step.
    s" 00/XX/01/11/" rule-from-string   \ reg pln rul
    cur-action-xt execute               \ reg pln rul act
    step-new                            \ reg pln stp
    over plan-push-end                  \ reg pln

    \ Add second step.
    s" 01/XX/11/11/" rule-from-string   \ reg pln rul
    cur-action-xt execute               \ reg pln rul act
    step-new                            \ reg pln stp
    over plan-push-end                  \ reg pln
 
    cr ." plan1: " dup .plan

    \ Restrict initial-region.
    2dup plan-restrict-initial-region   \ reg pln, pln' t | f

    if                                  \ reg pln pln'
        cr ." plan2: " dup .plan cr
        swap plan-deallocate            \ reg pln'
    else                                \ reg pln
        cr ." plan2: None" cr
        abort
    then

    \ Test results.
    dup plan-get-initial-region         \ reg pln' initial
    %0001 %0001 region-new tuck         \ reg pln' reg2 initial reg2
    region-neq abort" initial regions not eq?"
    region-deallocate                   \ reg pln'

    dup plan-get-result-region          \ reg pln' result
    %1011 %1011 region-new tuck         \ reg pln' reg2 result reg2
    region-neq abort" result regions net eq?"
    region-deallocate                   \ reg pln'
    
    \ Clean up.
    plan-deallocate
    region-deallocate

    \ Test 2.

    \ Init restriction region.
    %0000 %0100 region-new              \ reg

    \ Init plan.
    cur-domain-xt execute plan-new      \ reg pln

    \ Add first step.
    s" 00/XX/01/X1/" rule-from-string   \ reg pln rul
    cur-action-xt execute               \ reg pln rul act
    step-new                            \ reg pln stp
    over plan-push-end                  \ reg pln

    \ Add second step.
    s" 01/XX/11/11/" rule-from-string   \ reg pln rul
    cur-action-xt execute               \ reg pln rul act
    step-new                            \ reg pln stp
    over plan-push-end                  \ reg pln
 
    cr ." plan1: " dup .plan

    \ Restrict initial-region.
    2dup plan-restrict-initial-region   \ reg pln, pln' t | f

    if                                  \ reg pln pln'
        cr ." plan2: " dup .plan cr
        swap -rot                       \ pln reg pln'
    else                                \ reg pln
        cr ." plan2: None" cr
        abort
    then

    \ Test results.
    dup plan-get-initial-region         \ pln reg pln' initial
    %0000 %0100 region-new tuck         \ pln reg pln' reg2 initial reg2
    region-neq abort" initial regions not eq?"
    region-deallocate                   \ pln reg pln'

    dup plan-get-result-region          \ pln reg pln' result
    %1111 %1011 region-new tuck         \ reg pln' reg2 result reg2
    region-neq abort" result regions net eq?"
    region-deallocate                   \ pln reg pln'

    \ Test use count of last step.
    1
    over plan-get-step-list             \ pln reg pln' 1 stp-lst
    list-get-item                       \ pln reg pln' stp1
    struct-get-use-count                \ pln reg pln' u
    2 <> abort" use count of last step not 2?"

    \ Clean up.
    plan-deallocate
    region-deallocate
    plan-deallocate

    cr ." plan-test-restrict-initial-region - Ok" cr
;

: plan-test-restrict-result-region
    \ Test 1.

    \ Init restriction region.
    %1011 %1010 region-new              \ reg

    \ Init plan.
    cur-domain-xt execute plan-new      \ reg pln

    \ Add first step.
    s" 00/XX/01/Xx/" rule-from-string   \ reg pln rul
    cur-action-xt execute               \ reg pln rul act
    step-new                            \ reg pln stp
    over plan-push-end                  \ reg pln

    \ Add second step.
    s" 01/XX/11/X1/" rule-from-string   \ reg pln rul
    cur-action-xt execute               \ reg pln rul act
    step-new                            \ reg pln stp
    over plan-push-end                  \ reg pln
 
    cr ." plan1: " dup .plan

    \ Restrict initial-region.
    2dup plan-restrict-result-region    \ reg pln, pln' t | f

    if                                  \ reg pln pln'
        cr ." plan2: " dup .plan cr
        swap plan-deallocate            \ reg pln'
    else                                \ reg pln
        cr ." plan2: None" cr
        abort
    then

    \ Test results.
    dup plan-get-initial-region         \ reg pln' initial
    %0000 %0001 region-new tuck         \ reg pln' reg2 initial reg2
    region-neq abort" initial regions not eq?"
    region-deallocate                   \ reg pln'

    dup plan-get-result-region          \ reg pln' result
    %1011 %1011 region-new tuck         \ reg pln' reg2 result reg2
    region-neq abort" result regions net eq?"
    region-deallocate                   \ reg pln'
    
    \ Clean up.
    plan-deallocate
    region-deallocate

    cr ." plan-test-restrict-result-region - Ok" cr
;

: plan-test-link-step-to-initial-region
     \ Init plan.
    cur-domain-xt execute plan-new      \ pln

    \ Add step to plan.
    s" 00/11/01/XX/" rule-from-string   \ pln rul
    cur-action-xt execute               \ pln rul act
    step-new                            \ pln stp
    over plan-push                      \ pln

    \ create step to link.
    s" 10/XX/00/00/" rule-from-string       \ pln rul
    cur-action-xt execute                   \ pln rul act
    step-new                                \ pln stp

    swap                                    \ stp pln
    2dup plan-link-step-to-initial-region   \ stp pln, pln-l t | f
    if                                      \ stp pln pln-l
        cr ." linked plan " dup .plan cr

        \ Test results.
        dup plan-get-initial-region         \ stp pln pln-l initial
        %1100 %1100 region-new tuck         \ stp pln pln-l initial reg2
        region-neq abort" initial regions not eq?"
        region-deallocate                   \ stp pln pln-l

        dup plan-get-result-region          \ stp pln pln-l result
        %0110 %0110 region-new tuck         \ stp pln pln-l reg2 result reg2
        region-neq abort" result regions net eq?"
        region-deallocate                   \ stp pln pln-l

        plan-deallocate
    else
        cr ." link failed"
    then
    plan-deallocate
    drop

    cr ." plan-test-link-step-to-initial-region - Ok" cr
;

: plan-test-link-step-to-result-region
     \ Init plan.
    cur-domain-xt execute plan-new      \ pln

    \ Add step to plan.
    s" 00/11/01/XX/" rule-from-string   \ pln rul
    cur-action-xt execute               \ pln rul act
    step-new                            \ pln stp
    over plan-push                      \ pln

    \ create step to link.
    s" 01/XX/11/00/" rule-from-string       \ pln rul
    cur-action-xt execute                   \ pln rul act
    step-new                                \ pln stp

    swap                                    \ stp pln
    2dup plan-link-step-to-result-region    \ stp pln, pln-l t | f
    if                                      \ stp pln pln-l
        cr ." linked plan " dup .plan cr

        \ Test results.
        dup plan-get-initial-region         \ stp pln pln-l initial
        %0100 %0100 region-new tuck         \ stp pln pln-l initial reg2
        region-neq abort" initial regions not eq?"
        region-deallocate                   \ stp pln pln-l

        dup plan-get-result-region          \ stp pln pln-l result
        %1110 %1110 region-new tuck         \ stp pln pln-l reg2 result reg2
        region-neq abort" result regions net eq?"
        region-deallocate                   \ stp pln pln-l

        plan-deallocate
    else
        cr ." link failed"
    then

    plan-deallocate
    drop

    cr ." plan-test-link-step-to-result-region - Ok" cr
;

: plan-tests
    plan-test-restrict-initial-region
    plan-test-restrict-result-region
    plan-test-link-step-to-initial-region
    plan-test-link-step-to-result-region
;
