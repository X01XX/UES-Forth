: rulestore-test-new

    4 5 rule-new        \ rul1  5->4
    3 5 rule-new        \ rul2  5->3

    rulestore-new-2     \ rs2
    cr cr ." rulestore: " dup .rulestore

    rulestore-deallocate

    cr ." rulestore-test-new - Ok" cr
;

: rulestore-tests
    rulestore-test-new
;
