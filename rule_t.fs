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

: rule-tests
    rule-test-restrict-initial-region
    rule-test-restrict-result-region
;

