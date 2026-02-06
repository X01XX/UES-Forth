\ Tests for the rule struct functions.

: rule-test-restrict-initial-region
    \ Test 1.
    s" 0011" region-from-string-a   \ reg1
    s" 00/01/11/10/" rule-from-string  \ reg1 rul1
    2dup                            \ reg1 rul1 reg1 rul1
    rule-restrict-initial-region    \ reg1 rul1, rul2 t | f
    if
        s" 00/01/11/10/" rule-from-string

        2dup rule-eq
        0= abort" rule-test-restrict-initial-region 1: rules ne?"

        rule-deallocate
        rule-deallocate
    else
        ." rule-test-restrict-initial-region 1: rule-restrict-initial-region failed?"
        abort
    then
    rule-deallocate
    region-deallocate

    \ Test 2.
    s" 1010" region-from-string-a   \ reg1
    s" X1/X1/X0/X0/" rule-from-string  \ reg1 rul1
    2dup                            \ reg1 rul1 reg1 rul1
    rule-restrict-initial-region    \ reg1 rul1, rul2 t | f
    if
        s" 11/01/10/00/" rule-from-string

        2dup rule-eq
        0= abort" rule-test-restrict-initial-region 2: rules ne?"

        rule-deallocate
        rule-deallocate
    else
        ." rule-test-restrict-initial-region 2: rule-restrict-initial-region failed?"
        abort
    then
    rule-deallocate
    region-deallocate

    \ Test 3.
    s" 1010" region-from-string-a   \ reg1
    s" XX/XX/Xx/Xx/" rule-from-string  \ reg1 rul1
    2dup                            \ reg1 rul1 reg1 rul1
    rule-restrict-initial-region    \ reg1 rul1, rul2 t | f
    if
        s" 11/00/10/01/" rule-from-string

        2dup rule-eq
        0= abort" rule-test-restrict-initial-region 3: rules ne?"

        rule-deallocate
        rule-deallocate
    else
        ." rule-test-restrict-initial-region 3: rule-restrict-initial-region failed?"
        abort
    then
    rule-deallocate
    region-deallocate

    cr ." rule-test-restrict-initial-region: Ok" cr
;

: rule-test-restrict-result-region
    \ Test 1.
    s" 0101" region-from-string-a       \ reg1

    s" 00/11/XX/XX/" rule-from-string   \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1, rul2 t | f
    if
        s" 00/11/00/11/" rule-from-string   \ reg1 rul1 rul2 rul3
        2dup rule-eq                        \ reg1 rul1 rul2 rul3 flag
        0= abort" rule-test-restrict-result-region: rules ne?"

        rule-deallocate
        rule-deallocate
    else
        ." rule-test-restrict-result-region 1: rule-test-restrict-result-region failed?"
        abort
    then
    rule-deallocate
    region-deallocate

    \ Test 2.
    s" 1001" region-from-string-a       \ reg1

    s" X1/X0/Xx/Xx/" rule-from-string   \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1, rul2 t | f
    if
        s" X1/X0/10/01/" rule-from-string   \ reg1 rul1 rul2 rul3
        2dup rule-eq                        \ reg1 rul1 rul2 rul3 flag
        0= abort" rule-test-restrict-result-region 2: rules ne?"
        rule-deallocate
        rule-deallocate
        rule-deallocate
        region-deallocate
    else
        ." rule-test-restrict-result-region: rule-test-restrict-result-region failed?"
        abort
    then

    cr ." rule-test-restrict-result-region: Ok" cr
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

    cr ." rule-test-restrict-to-region: Ok" cr
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

    cr ." rule-test-combine2: Ok" cr
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

    \ Test 4.
    s" 0XXX" region-from-string-a   \ reg-from
    s" 0X10" region-from-string-a   \ reg-from reg-to
    swap                            \ reg-to reg-from
    2dup rule-new-region-to-region  \ reg1 reg2 rul1

    \ cr ." rul1 " dup .rule cr
    s" 00/XX/X1/X0/" rule-from-string   \ reg1 reg2 rul1 rul2
    2dup rule-eq is-false abort" rule-test-new-region-to-region test4 region-eq failed?"
    rule-deallocate
    rule-deallocate
    region-deallocate
    region-deallocate

    cr ." rule-test-new-region-to-region: Ok" cr
;

: rule-test-calc-for-planstep-fc
    \ Test 1, reg-from intersects rule initial-region.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" Xx/XX/01/01/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-for-planstep-fc               \ reg-to reg-from rulx | rul' t | f
    if                                      \ reg-to reg-from rulx | rul'
        \ cr ." step1: " dup .rule cr
        s" 01/11/01/01/" rule-from-string   \ reg-to reg-from rulx | rul' rul-t'
        2dup                                \ reg-to reg-from rulx | rul' rul-t' rul' rul-t'
        rule-eq                             \ reg-to reg-from rulx | rul' rul-t' bool
        is-false abort" rule-test-calc-for-planstep-fc: 1: unexpected rule?"

        rule-deallocate                         \ reg-to reg-from rulx | rul'
        rule-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                \ reg-to reg-from rulx
        cr ." rule-test-calc-for-planstep-fc: 1 rule-calc-planstep-fc failed?"
        abort
    then

    \ Test 2, reg-from does not intersect the rule initial-region,
    \ but the rule initial region is reachable without using another needed change.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" 01/Xx/11/Xx/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-for-planstep-fc               \ reg-to reg-from rulx | rul' t | f
    if                                      \ reg-to reg-from rulx | rul'
        \ cr ." step2: " dup .rule cr
        s" 01/10/11/01/" rule-from-string   \ reg-to reg-from rulx | rul' rul-t'
        \ cr ." expt: " dup .rule space ." found: " over .rule cr
        2dup                                \ reg-to reg-from rulx | rul' rul-t' rul' rul-t'
        rule-eq                             \ reg-to reg-from rulx | rul' rul-t' bool
        is-false abort" rule-test-calc-for-planstep-fc: 2 unexpected rule?"

        rule-deallocate                     \ reg-to reg-from rulx | rul'
        rule-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                    \ reg-to reg-from rulx
        cr ." rule-calc-for-planstep-fc: 2 failed?"
        abort
    then

    \ Test 3, reg-from is a proper superset of reg-to.
    %0101 %0101 region-new                  \ reg-to
    %0111 %0001 region-new                  \ reg-to reg-from
    s" XX/X1/X0/XX/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-for-planstep-fc               \ reg-to reg-from rulx | rul' t | f
    if                                      \ reg-to reg-from rulx | rul'
        \ cr ." step3: " dup .rule cr
        s" 00/X1/X0/11/" rule-from-string   \ reg-to reg-from rulx | rul' rul-t'
        \ cr ." expt: " dup .rule space ." found: " over .rule cr
        2dup                                \ reg-to reg-from rulx | rul' rul-t' rul' rul-t'
        rule-eq                             \ reg-to reg-from rulx | rul' rul-t' bool
        is-false abort" rule-test-calc-for-planstep-fc: 3 unexpected rule?"

        rule-deallocate                     \ reg-to reg-from rulx | rul'
        rule-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                    \ reg-to reg-from rulx
        cr ." rule-calc-for-planstep-fc: 3 failed?"
        abort
    then

    cr ." rule-test-calc-for-planstep-fc: Ok" cr
;

: rule-test-calc-for-planstep-bc

    \ Test 1, reg-to intersects rule result-region.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" 01/XX/X0/11/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-for-planstep-bc               \ reg-to reg-from rulx | rul' t | f
    if                                      \ reg-to reg-from rulx | rul'
        \ cr dup .rule cr
        s" 01/11/X0/11/" rule-from-string   \ reg-to reg-from rulx | rul' rul-t'
        2dup                                \ reg-to reg-from rulx | rul' rul-t' rul' rul-t'
        rule-eq                             \ reg-to reg-from rulx | rul' rul-t' bool
        is-false abort" unexpected rule?"

        rule-deallocate                     \ reg-to reg-from rulx | rul'
        rule-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                \ reg-to reg-from rulx
        cr ." rule-calc-for-planstep-bc: 1 failed?"
        abort
    then

    \ Test 2, reg-to does not rule-number-unwanted-changesintersect the rule result-region,
    \ but the rule result region is reachable from reg-to without using another needed change.
    \ Reg-from to the rule initial-region does require a needed change.
    %1101 %1101 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" 01/Xx/11/XX/" rule-from-string       \ reg-to reg-from rulx
    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx | reg-to reg-from rulx
    rule-calc-for-planstep-bc               \ reg-to reg-from rulx | rul' t | f
    if                                      \ reg-to reg-from rulx | rul'
        \ Check planstep rule.
        \ cr ." rule: " dup .rule cr
        s" 01/01/11/11/" rule-from-string   \ reg-to reg-from rulx | rul' rul-t'
        2dup                                \ reg-to reg-from rulx | rul' rul-t' rul rul-t'
        rule-eq                             \ reg-to reg-from rulx | rul' rul-t' bool
        is-false abort" unexpected rule?"

        rule-deallocate                     \ reg-to reg-from rulx | rul'
        rule-deallocate
        rule-deallocate
        region-deallocate
        region-deallocate
    else                                \ reg-to reg-from rulx
        cr ." rule-calc-for-planstep-bc: 2 failed?"
        abort
    then


    cr ." rule-test-calc-for-planstep-bc: Ok" cr
;

: rule-test-number-unwanted-changes
    \ Test 1.
    %0111 %0111 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" XX/00/00/Xx/" rule-from-string       \ reg-to reg-from rulx

    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx reg-to reg-from rulx
    rule-number-unwanted-changes            \ reg-to reg-from rulx u
    cr ." Number unwanted changes: " dup .
    1 <> abort" Number unwanted changes ne 1?"
    
    rule-deallocate
    region-deallocate
    region-deallocate

    cr ." rule-test-number-unwanted-changes: Ok" cr
;

: rule-tests
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    swap                                            \ dom0 sess
    session-add-domain                              \

    rule-test-restrict-initial-region
    rule-test-restrict-result-region
    rule-test-restrict-to-region
    rule-test-combine2
    rule-test-new-region-to-region
    rule-test-calc-for-planstep-fc
    rule-test-calc-for-planstep-bc

    current-session-deallocate
;

