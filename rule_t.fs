\ Tests for the rule struct functions.

: rule-test-rule-restrict-initial-region
    cr ." rule-test-rule-restrict-initial-region - start"

    test-none-in-use


    test-none-in-use

    cr ." rule-test-rule-restrict-initial-region - Ok" cr
;

: rule-test-rule-restrict-result-region
    cr ." rule-test-rule-restrict-result-region - start"

    test-none-in-use


    test-none-in-use

    cr ." rule-test-rule-restrict-result-region - Ok" cr
;

: rule-tests
    rule-test-rule-restrict-initial-region
    rule-test-rule-restrict-result-region
;

