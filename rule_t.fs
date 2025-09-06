\ Tests for the rule struct functions.

: rule-test-restrict-initial-region
    s" 011X" region-from-string     \ reg1

    s" XX/11/X0/10/" rule-from-string  \ reg1 rul1 

    2dup                            \ reg1 rul1 reg1 rul1
    rule-restrict-initial-region    \ reg1 rul1 rul2

    s" 00/11/10/10/" rule-from-string 
    
    2dup rule-eq
    0= abort" rules ne?"

    rule-deallocate
    rule-deallocate
    rule-deallocate
    region-deallocate

    cr ." rule-test-restrict-initial-region - Ok" cr
;

: rule-test-restrict-result-region
    s" 0X00" region-from-string         \ reg1

    s" XX/11/X0/10/" rule-from-string   \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1 rul2

    s" 00/11/X0/10/" rule-from-string   \ reg1 rul1 rul2 rul3
    2dup rule-eq                        \ reg1 rul1 rul2 rul3 flag

    0= abort" rule initial region ne?"

    rule-deallocate
    rule-deallocate
    rule-deallocate
    region-deallocate

    cr ." rule-test-restrict-result-region - Ok" cr
;

: rule-test-apply-to-state-f
    s" 01/10/X1/X1/" rule-from-string          \ rul
    cr ." rule: " dup .rule cr
    %0101 over                                  \ rul 1 rul
    rule-apply-to-state-f                       \ rul smpl true
    if
        cr ." smpl " dup .sample cr
        dup sample-get-result
        %1011 <>
        abort" Result not expected"
        sample-deallocate
    else
        ." sample not made?" abort
    then
    rule-deallocate

    s" Xx/Xx/X0/X0/" rule-from-string          \ rul
    cr ." rule: " dup .rule cr
    %0101 over                                  \ rul 1 rul
    rule-apply-to-state-f                       \ rul smpl true
    if
        cr ." smpl " dup .sample cr
        dup sample-get-result
        %1000 <>
        abort" Result not expected"
        sample-deallocate
    else
        ." sample not made?" abort
    then
    rule-deallocate

    s" XX/XX/00/11/" rule-from-string          \ rul
    cr ." rule: " dup .rule cr
    %0101 over                                  \ rul 1 rul
    rule-apply-to-state-f                       \ rul smpl true
    if
        cr ." smpl " dup .sample cr
        dup sample-get-result
        %0101 <>
        abort" Result not expected"
        sample-deallocate
    else
        ." sample not made?" abort
    then
    rule-deallocate

    cr ." rule-test-apply-to-state-f - Ok" cr
;

: rule-test-get-forward-step ( smpl1 rul0 -- step true | false )
    #11 4 sample-new                    \ smpl
    s" XX/XX/XX/Xx/" rule-from-string   \ smpl rul
    2dup                                \ smpl rul smpl rul
    rule-get-forward-step               \ smpl rul, stp true | false
    if
        cr ." step: " dup .step cr
        dup step-get-sample sample-get-result
        5 <> abort" Step result is not 5"
        step-deallocate
    else
        cr ." no step" cr abort
    then
    rule-deallocate
    sample-deallocate

    cr ." rule-test-get-forward-step - Ok" cr
;

: rule-tests
    rule-test-restrict-initial-region
    rule-test-restrict-result-region
    rule-test-apply-to-state-f
    rule-test-get-forward-step
;

