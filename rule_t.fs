\ Tests for the rule struct functions.

: rule-test-restrict-initial-region
    s" 011X" region-from-string-a   \ reg1

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
    s" 0X00" region-from-string-a       \ reg1

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

    s" 0101" region-from-string-a       \ reg1

    s" Xx/Xx/XX/XX/" rule-from-string   \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1 rul2
    cr ." rslt2: " dup .rule cr
    rule-deallocate
    rule-deallocate
    region-deallocate

    s" 0101" region-from-string-a       \ reg1

    s" X0/X1/XX/XX/" rule-from-string   \ reg1 rul1

    2dup                                \ reg1 rul1 reg1 rul1
    rule-restrict-result-region         \ reg1 rul1 rul2
    cr ." rslt3: " dup .rule cr
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

: rule-test-get-forward-sample ( smpl1 rul0 -- smpl true | false )
    #11 #4 sample-new                   \ smpl
    s" XX/XX/XX/Xx/" rule-from-string   \ smpl rul
    2dup                                \ smpl rul smpl rul
    rule-calc-forward-sample            \ smpl rul, smpl true | false
    if
        cr ." sample: " dup .sample cr
        dup sample-get-result
        #5 <> abort" Step result is not 5"
        sample-deallocate
    else
        cr ." no sample" cr abort
    then
    rule-deallocate
    sample-deallocate

    cr ." rule-test-get-forward-sample - Ok" cr
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

: rule-test-get-backward-sample
    #15 #5 sample-new                    \ smpl
    s" 01/X1/11/XX/" rule-from-string    \ smpl rul
    2dup rule-calc-backward-sample       \ smpl rul, stpx t | f
    if
        cr ." Sample found: " dup .sample cr
        dup sample-get-initial
        #7 <> abort" Sample initial not 7?"
        sample-deallocate
    else
        ." sample not found" abort
    then
    rule-deallocate
    sample-deallocate

    cr ." rule-test-get-backward-step - Ok" cr
;

: rule-test-adjust-xb
    \ Test primary effects.
    %0010 %0100 changes-new             \ cngs
    cr dup .changes
    s" X1/X1/X0/X0/" rule-from-string   \ cngs rulx
    cr dup .rule
    2dup rule-adjust-xb                 \ cngs rulx rulx'
    cr dup .rule cr
    s" 11/01/10/00/" rule-from-string   \ cngs rulx rulx' rul-t
    2dup rule-eq                        \ cngs rulx rulx' rul-t flag
    0= abort" rules not eq 1?"

    rule-deallocate
    rule-deallocate
    rule-deallocate                     \ cngs

    \ Test non-changes, 2.
    cr dup .changes
    s" 01/01/10/10/" rule-from-string   \ cngs rulx
    cr dup .rule
    2dup rule-adjust-xb                 \ cngs rulx rulx'
    cr dup .rule cr
    2dup rule-eq                        \ cngs rulx rulx' flag
    0= abort" rules not eq 2?"
    rule-deallocate
    rule-deallocate                     \ cngs

    \ Test non-changes, 3.
    cr dup .changes
    s" Xx/Xx/Xx/Xx/" rule-from-string   \ cngs rulx
    cr dup .rule
    2dup rule-adjust-xb                 \ cngs rulx rulx'
    cr dup .rule cr
    2dup rule-eq                        \ cngs rulx rulx' flag
    0= abort" rules not eq 3?"
    rule-deallocate
    rule-deallocate                     \ cngs

    changes-deallocate
;

: rule-tests
    rule-test-restrict-initial-region
    rule-test-restrict-result-region
    rule-test-apply-to-state-f
    rule-test-get-forward-sample
    rule-test-restrict-to-region
    rule-test-get-backward-sample
    rule-test-adjust-xb
;

