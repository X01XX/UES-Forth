\ Tests for the rule struct functions.

: rule-test-restrict-initial-region
    \ Test 1.
    s" 0011" region-from-string-a   \ reg1
    s" 00/01/11/10/" rule-from-string-a \ reg1 rul1
    2dup                            \ reg1 rul1 reg1 rul1
    rule-restrict-initial-region    \ reg1 rul1, rul2 t | f
    if
        s" 00/01/11/10/" rule-from-string-a

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
    s" X1/X1/X0/X0/" rule-from-string-a \ reg1 rul1
    2dup                            \ reg1 rul1 reg1 rul1
    rule-restrict-initial-region    \ reg1 rul1, rul2 t | f
    if
        s" 11/01/10/00/" rule-from-string-a

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
    s" XX/XX/Xx/Xx/" rule-from-string-a \ reg1 rul1
    2dup                            \ reg1 rul1 reg1 rul1
    rule-restrict-initial-region    \ reg1 rul1, rul2 t | f
    if
        s" 11/00/10/01/" rule-from-string-a

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

    s" 00/11/XX/XX/" rule-from-string-a \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1, rul2 t | f
    if
        s" 00/11/00/11/" rule-from-string-a \ reg1 rul1 rul2 rul3
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

    s" X1/X0/Xx/Xx/" rule-from-string-a \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1, rul2 t | f
    if
        s" X1/X0/10/01/" rule-from-string-a \ reg1 rul1 rul2 rul3
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
    s" 01/X1/11/XX/" rule-from-string-a \ reg rul
    2dup rule-restrict-to-region        \ reg rul rul'
    if
    else
        cr ." rule restriction failed?" abort
    then
    s" 01/11/11/11/" rule-from-string-a \ reg rul rul' rul2
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

: rule-test-combine
    \ Test 1.
    s" 00/00/01/01/" rule-from-string-a \ rul0
    s" 00/01/11/10/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 1: " dup .rule cr
    s" 00/01/01/00/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 1 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 2.
    s" 11/11/10/10/" rule-from-string-a \ rul0
    s" 11/10/00/01/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 2: " dup .rule cr
    s" 11/10/10/11/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 2 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 3.
    s" 00/00/00/00/" rule-from-string-a \ rul0
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 3: " dup .rule cr
    s" 01/00/00/01/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 3 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 4.
    s" 01/01/01/01/" rule-from-string-a \ rul0
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 4: " dup .rule cr
    s" 00/01/00/01/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 4 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 5.
    s" 11/11/11/11/" rule-from-string-a \ rul0
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 5: " dup .rule cr
    s" 10/11/10/11/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 5 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 6.
    s" 10/10/10/10/" rule-from-string-a \ rul0
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 6: " dup .rule cr
    s" 11/10/10/11/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 6 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 7.
    s" Xx/Xx/Xx/Xx/" rule-from-string-a \ rul0
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 7: " dup .rule cr
    s" XX/Xx/X0/X1/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 7 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 8.
    s" Xx/Xx/Xx/Xx/" rule-from-string-a \ rul0
    s" 00/01/11/10/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 8: " dup .rule cr
    s" 10/11/01/00/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 8 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 9.
    s" XX/XX/XX/XX/" rule-from-string-a \ rul0
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 9: " dup .rule cr
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 9 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 10.
    s" XX/XX/XX/XX/" rule-from-string-a \ rul0
    s" 00/01/11/10/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 10: " dup .rule cr
    s" 00/01/11/10/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 10 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 11.
    s" X0/X0/X0/X0/" rule-from-string-a \ rul0
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 11: " dup .rule cr
    s" X1/X0/X0/X1/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 11 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 12.
    s" X0/X0/X0/X0/" rule-from-string-a \ rul0
    s" 00/01/00/01/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 12: " dup .rule cr
    s" X0/X1/X0/X1/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 12 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 13.
    s" X1/X1/X1/X1/" rule-from-string-a \ rul0
    s" Xx/XX/X0/X1/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    \ cr ." rule test 13: " dup .rule cr
    s" X0/X1/X0/X1/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 13 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    \ Test 14.
    s" X1/X1/X1/X1/" rule-from-string-a \ rul0
    s" 11/10/11/10/" rule-from-string-a \ rul0 rul1
    swap                                \ rul1 rul0
    2dup
    rule-combine                        \ rul1 rul0 rul01
    cr ." rule test 14: " dup .rule cr
    s" X1/X0/X1/X0/" rule-from-string-a \ rul1 rul0 rul01 rult
    2dup rule-eq                        \ rul1 rul0 rul01 rult bool
    false? abort" rule-test-combine: test 14 failed?"
    rule-deallocate
    rule-deallocate
    rule-deallocate
    rule-deallocate

    cr ." rule-test-combine: Ok" cr
;

: rule-test-new-region-to-region

    \ Test 1.
    s" 0011" region-from-string-a   \ reg-from
    s" 0110" region-from-string-a   \ reg-from reg-to
    swap                            \ reg-to reg-from
    2dup rule-new-region-to-region  \ reg1 reg2 rul1

    \ cr ." rul1 " dup .rule cr
    s" 00/01/11/10/" rule-from-string-a \ reg1 reg2 rul1 rul2
    2dup rule-eq false? abort" rule-test-new-region-to-region test1 rule-eq failed?"
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
    s" X0/X1/XX/11/" rule-from-string-a \ reg1 reg2 rul1 rul2
    2dup rule-eq false? abort" rule-test-new-region-to-region test2 rule-eq failed?"
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
    s" 00/11/00/11/" rule-from-string-a \ reg1 reg2 rul1 rul2
    2dup rule-eq false? abort" rule-test-new-region-to-region test3 rule-eq failed?"
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
    s" 00/XX/X1/X0/" rule-from-string-a \ reg1 reg2 rul1 rul2
    2dup rule-eq false? abort" rule-test-new-region-to-region test4 rule-eq failed?"
    rule-deallocate
    rule-deallocate
    region-deallocate
    region-deallocate

    cr ." rule-test-new-region-to-region: Ok" cr
;

: rule-test-number-unwanted-changes
    \ Test 1.
    %0111 %0111 region-new                  \ reg-to
    %0100 %0100 region-new                  \ reg-to reg-from
    s" XX/00/00/Xx/" rule-from-string-a     \ reg-to reg-from rulx

    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx reg-to reg-from rulx
    rule-number-unwanted-changes            \ reg-to reg-from rulx u
    \ cr ." Number unwanted changes: " dup .
    1 <> abort" Number unwanted changes ne 1?"

    rule-deallocate
    region-deallocate
    region-deallocate

    \ Test 2.
    %1010 %1010 region-new                  \ reg-to
    %1100 %1100 region-new                  \ reg-to reg-from
    s" 00/11/01/10/" rule-from-string-a     \ reg-to reg-from rulx

    #2 pick #2 pick #2 pick                 \ reg-to reg-from rulx reg-to reg-from rulx
    rule-number-unwanted-changes            \ reg-to reg-from rulx u
    \ cr ." Number unwanted changes: " dup .
    1 <> abort" Number unwanted changes ne 1?"

    rule-deallocate
    region-deallocate
    region-deallocate

    cr ." rule-test-number-unwanted-changes: Ok" cr
;

: rule-tests
    session-new                                     \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    over                                            \ sess dom0 sess
    session-add-domain                              \ sess

    rule-test-number-unwanted-changes
    rule-test-restrict-initial-region
    rule-test-restrict-result-region
    rule-test-restrict-to-region
    rule-test-combine
    rule-test-new-region-to-region

    session-deallocate
;

