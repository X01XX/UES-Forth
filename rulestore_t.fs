: rulestore-test-new

    s" 00/11/00/10/" rule-from-string   \ rul1
    s" 00/10/01/11/" rule-from-string   \ rul1 rul2

    rulestore-new-2     \ rs2
    cr cr ." rulestore: " dup .rulestore

    rulestore-deallocate

    cr ." rulestore-test-new - Ok" cr
;

: rulestore-tests
    rulestore-test-new
;
