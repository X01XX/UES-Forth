\ Tests for the rule struct functions.

: rule-test-restrict-initial-region
    s" 011X" region-from-string-a   \ reg1
    s" XX/11/X0/10/" rule-from-string  \ reg1 rul1 
    2dup                            \ reg1 rul1 reg1 rul1
    rule-restrict-initial-region    \ reg1 rul1, rul2 t | f
    if
        s" 00/11/10/10/" rule-from-string 
    
        2dup rule-eq
        0= abort" rules ne?"

        rule-deallocate
        rule-deallocate
    else
        ." rule-restrict-initial-region failed?"
        abort
    then
    rule-deallocate
    region-deallocate

    cr ." rule-test-restrict-initial-region - Ok" cr
;

: rule-test-restrict-result-region
    \ Test 1.
    s" 0X00" region-from-string-a       \ reg1

    s" XX/11/X0/10/" rule-from-string   \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1, rul2 t | f
    if
        s" 00/11/X0/10/" rule-from-string   \ reg1 rul1 rul2 rul3
        2dup rule-eq                        \ reg1 rul1 rul2 rul3 flag
        0= abort" rules ne?"

        rule-deallocate
        rule-deallocate
    else
        ." rule-test-restrict-result-region failed?"
        abort
    then
    rule-deallocate
    region-deallocate

    \ Test 2.
    s" 0101" region-from-string-a       \ reg1

    s" Xx/Xx/XX/XX/" rule-from-string   \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1, rul2 t | f
    if
        \ cr ." rslt2: " dup .rule cr
        s" 10/01/00/11/" rule-from-string   \ reg1 rul1 rul2 rul3
        2dup rule-eq                        \ reg1 rul1 rul2 rul3 flag
        0= abort" rules ne?"
        rule-deallocate
        rule-deallocate
        rule-deallocate
        region-deallocate
    else
        ." rule-test-restrict-result-region failed?"
        abort
    then

    \ Test 3.
    s" 0101" region-from-string-a       \ reg1

    s" X0/X1/XX/XX/" rule-from-string   \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1, rul2 t | f
    if
        \ cr ." rslt3: " dup .rule cr
        s" X0/X1/00/11/" rule-from-string   \ reg1 rul1 rul2 rul3
        2dup rule-eq                        \ reg1 rul1 rul2 rul3 flag
        0= abort" rules ne?"
        rule-deallocate
        rule-deallocate
        rule-deallocate
        region-deallocate
    else
        ." rule-test-restrict-result-region failed?"
        abort
    then

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

: rule-test-restrict-to-region
    #5 #15 region-new                   \ reg
    s" 01/X1/11/XX/" rule-from-string   \ reg rul
    2dup rule-restrict-to-region        \ reg rul rul'
    if
    else
        cr ." rule restriction failed?" abort
    then
    s" 01/11/11/11/" rule-from-string   \ reg rul rul' rul2
    2dup rule-eq                        \ reg rul rul' rul2 flag
    if
    else
        cr ." invalid rule?" cr abort
    then

    rule-deallocate
    rule-deallocate
    rule-deallocate
    region-deallocate

    cr ." rule-test-restrict-to-region - Ok" cr
;

: rule-test-combine2
    \ Test 1.
    s" 00/00/01/01/" rule-from-string   \ rul0
    s" 00/01/11/10/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 1: " dup .rule cr
    s" 00/01/01/00/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 1 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 2.
    s" 11/11/10/10/" rule-from-string   \ rul0
    s" 11/10/00/01/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 2: " dup .rule cr
    s" 11/10/10/11/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 2 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 3.
    s" 00/00/00/00/" rule-from-string   \ rul0
    s" Xx/XX/X0/X1/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 3: " dup .rule cr
    s" 01/00/00/01/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 3 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 4.
    s" 01/01/01/01/" rule-from-string   \ rul0
    s" Xx/XX/X0/X1/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 4: " dup .rule cr
    s" 00/01/00/01/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 4 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 5.
    s" 11/11/11/11/" rule-from-string   \ rul0
    s" Xx/XX/X0/X1/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 5: " dup .rule cr
    s" 10/11/10/11/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 5 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 6.
    s" 10/10/10/10/" rule-from-string   \ rul0
    s" Xx/XX/X0/X1/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 6: " dup .rule cr
    s" 11/10/10/11/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 6 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 7.
    s" Xx/Xx/Xx/Xx/" rule-from-string   \ rul0
    s" Xx/XX/X0/X1/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 7: " dup .rule cr
    s" XX/Xx/X0/X1/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 7 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 8.
    s" Xx/Xx/Xx/Xx/" rule-from-string   \ rul0
    s" 00/01/11/10/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 8: " dup .rule cr
    s" 10/11/01/00/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 8 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 9.
    s" XX/XX/XX/XX/" rule-from-string   \ rul0
    s" Xx/XX/X0/X1/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 9: " dup .rule cr
    s" Xx/XX/X0/X1/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 9 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 10.
    s" XX/XX/XX/XX/" rule-from-string   \ rul0
    s" 00/01/11/10/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 10: " dup .rule cr
    s" 00/01/11/10/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 10 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 11.
    s" X0/X0/X0/X0/" rule-from-string   \ rul0
    s" Xx/XX/X0/X1/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 11: " dup .rule cr
    s" X1/X0/X0/X1/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 11 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 12.
    s" X0/X0/X0/X0/" rule-from-string   \ rul0
    s" 00/01/00/01/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 12: " dup .rule cr
    s" X0/X1/X0/X1/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 12 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 13.
    s" X1/X1/X1/X1/" rule-from-string   \ rul0
    s" Xx/XX/X0/X1/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    \ cr ." rule test 13: " dup .rule cr
    s" X0/X1/X0/X1/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 13 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 14.
    s" X1/X1/X1/X1/" rule-from-string   \ rul0
    s" 11/10/11/10/" rule-from-string   \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine2                       \ rul1 rul0 rul01
    cr ." rule test 14: " dup .rule cr
    s" X1/X0/X1/X0/" rule-from-string   \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    is-false abort" rule-test-combine2: test 14 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    cr ." rule-test-combine2 - Ok" cr
;

: rule-test-new-region-to-region
    \ Test 1.
    s" 0011" region-from-string-a   \ reg-from
    s" 0110" region-from-string-a   \ reg-from reg-to
    swap                            \ reg-to reg-from
    2dup rule-new-region-to-region  \ reg1 reg2 rul1

    \ cr ." rul1 " dup .rule cr
    s" 00/01/11/10/" rule-from-string   \ reg1 reg2 rul1 rul2
    2dup rule-eq is-false abort" rule-test-new-region-to-region test1 region-eq failed?"
    rule-deallocate
    rule-deallocate
    region-deallocate
    region-deallocate
    
    
    \ Test 2.
    s" XXX1" region-from-string-a   \ reg-from
    s" 01XX" region-from-string-a   \ reg-from reg-to
    swap                            \ reg-to reg-from
    2dup rule-new-region-to-region  \ reg1 reg2 rul1

    \ cr ." rul1 " dup .rule cr
    s" X0/X1/XX/11/" rule-from-string   \ reg1 reg2 rul1 rul2
    2dup rule-eq is-false abort" rule-test-new-region-to-region test2 region-eq failed?"
    rule-deallocate
    rule-deallocate
    region-deallocate
    region-deallocate

    \ test 3.
    s" 0101" region-from-string-a   \ reg-from
    s" XXXX" region-from-string-a   \ reg-from reg-to
    swap                            \ reg-to reg-from
    2dup rule-new-region-to-region  \ reg1 reg2 rul1

    \ cr ." rul1 " dup .rule cr
    s" 00/11/00/11/" rule-from-string   \ reg1 reg2 rul1 rul2
    2dup rule-eq is-false abort" rule-test-new-region-to-region test3 region-eq failed?"
    rule-deallocate
    rule-deallocate
    region-deallocate
    region-deallocate

    cr ." rule-test-new-region-to-region - Ok" cr
;

: rule-test-isolate-changes
    \ m10   m01
    %0010 %0100 changes-new             \ cngs
    cr dup .changes
    s" X1/Xx/Xx/X0/" rule-from-string   \ cngs rulx
    cr dup .rule
    2dup rule-isolate-changes           \ cngs rulx rulx'
    cr dup .rule cr
    s" X1/01/10/X0/" rule-from-string   \ cngs rulx rulx' rul-t
    2dup rule-eq                        \ cngs rulx rulx' rul-t flag
    0= abort" rules not eq 1?"

    rule-deallocate
    rule-deallocate
    rule-deallocate                     \ cngs
    changes-deallocate

    cr ." rule-test-isolate-changes - Ok" cr
;

: rule-test-calc-step-fc
    \ Test 1, reg-from intersects rule initial-region.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" Xx/XX/00/01/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-step-fc                       \ reg-to reg-from rulx | stp t | f
    if                                      \ reg-to reg-from rulx | stp
        \ cr dup .step cr
        dup step-get-rule                   \ reg-to reg-from rulx | stp stp-rul
        s" 01/11/00/01/" rule-from-string   \ reg-to reg-from rulx | stp stp-rul rul-t'
        tuck                                \ reg-to reg-from rulx | stp rul-t' stp-rul rul-t'
        rule-eq                             \ reg-to reg-from rulx | stp rul-t' bool
        is-false abort" rule-test-calc-step-fc 1: unexpected rule?"

        rule-deallocate
        step-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                \ reg-to reg-from rulx
        cr ." rule-test-calc-step-fc 1: rule-calc-step-fc failed?"
        abort
    then

    \ Test 2, reg-from does not intersect the rule initial-region,
    \ but the rule initial region is reachable without using another needed change.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" 01/XX/11/Xx/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-step-fc                       \ reg-to reg-from rulx | stp t | f
    if                                      \ reg-to reg-from rulx | stp
        dup step-get-rule                   \ reg-to reg-from rulx | stp stp-rul
        s" 01/11/11/01/" rule-from-string   \ reg-to reg-from rulx | stp stp-rul rul-t'
        \ cr ." expt: " dup .rule space ." found: " over .rule cr
        tuck                                \ reg-to reg-from rulx | stp rul-t' stp-rul rul-t'
        rule-eq                             \ reg-to reg-from rulx | stp rul-t' bool
        is-false abort" rule-test-calc-step-fc 2: unexpected rule?"

        rule-deallocate
        step-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                \ reg-to reg-from rulx
        cr ." rule-calc-step-fc 2: failed?"
        abort
    then

    cr ." rule-test-calc-step-fc - Ok" cr
;

: rule-test-calc-step-bc
    \ Test 1, reg-to intersects rule result-region.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" 01/XX/00/11/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-step-bc                       \ reg-to reg-from rulx | stp t | f
    if                                      \ reg-to reg-from rulx | stp
        \ cr dup .step cr
        dup step-get-rule                   \ reg-to reg-from rulx | stp stp-rul
        s" 01/11/00/11/" rule-from-string   \ reg-to reg-from rulx | stp stp-rul rul-t'
        tuck                                \ reg-to reg-from rulx | stp rul-t' stp-rul rul-t'
        rule-eq                             \ reg-to reg-from rulx | stp rul-t' bool
        is-false abort" unexpected rule?"

        rule-deallocate
        step-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                \ reg-to reg-from rulx
        cr ." rule-calc-step-bc failed?"
        abort
    then

    \ Test 2, reg-from does not intersect the rule initial-region,
    \ but the rule initial region is reachable without using another needed change.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" 01/Xx/11/11/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-step-bc                       \ reg-to reg-from rulx | stp t | f
    if                                      \ reg-to reg-from rulx | stp
        \ cr dup .step cr
        dup step-get-rule                   \ reg-to reg-from rulx | stp stp-rul
        s" 01/Xx/11/11/" rule-from-string   \ reg-to reg-from rulx | stp stp-rul rul-t'
        tuck                                \ reg-to reg-from rulx | stp rul-t' stp-rul rul-t'
        rule-eq                             \ reg-to reg-from rulx | stp rul-t' bool
        is-false abort" unexpected rule?"

        rule-deallocate
        step-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                \ reg-to reg-from rulx
        cr ." rule-calc-step-bc failed?"
        abort
    then

    \ Test 3, reg-from does not intersect the rule initial-region,
    \ the rule initial region is not reachable without using another needed change.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" 01/XX/00/00/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-step-bc                       \ reg-to reg-from rulx | stp t | f
    abort" step returned?" 
    rule-deallocate
    region-deallocate
    region-deallocate

    cr ." rule-test-calc-step-bc - Ok" cr
;

: rule-tests
    0 set-domain
    rule-test-restrict-initial-region
    rule-test-restrict-result-region
    rule-test-apply-to-state-f
    rule-test-restrict-to-region
    rule-test-combine2
    rule-test-new-region-to-region
    rule-test-isolate-changes
    rule-test-calc-step-fc
    \ rule-test-calc-step-bc
;

