: rulestore-test-union

    \ Test two pn-2 rulestores that should form a union.
    s" 00/11/00/11/" rule-from-string   \ rul1
    s" 00/11/00/10/" rule-from-string   \ rul1 rul2
    rulestore-new-2                     \ rs1
    cr ." rulestore1: " dup .rulestore cr

    s" 00/11/11/11/" rule-from-string   \ rs1 rul1
    s" 00/11/11/10/" rule-from-string   \ rs1 rul1 rul2
    rulestore-new-2                     \ rs1 rs2
    cr ." rulestore2: " dup .rulestore cr

    2dup rulestore-union                \ rs1 rs3, rsx true | false
    if
        rulestore-deallocate
    else
        cr ." rulestore-union rs1 rs2 failed" cr
        abort
    then
    rulestore-deallocate                \ rs1

    \ Test two pn-2 rulestores that should not form a union, due to incompatibility.
    s" 01/11/11/00/" rule-from-string   \ rs1 rul1
    s" 00/11/11/00/" rule-from-string   \ rs1 rul1 rul2
    rulestore-new-2                     \ rs1 rs3
    cr ." rulestore3: " dup .rulestore cr

    2dup rulestore-union                \ rs1 rs3, rsx true | false
    if
        cr ." rulestore-union rs1 rs3 should not have succeeded" cr
        abort
    then
    rulestore-deallocate                \ rs1

    \ Test two pn-2 rulestores that should not form a union, due to too much compatibility.
    s" 11/11/11/00/" rule-from-string   \ rs1 rul1
    s" 11/11/11/01/" rule-from-string   \ rs1 rul1 rul2
    rulestore-new-2                     \ rs1 rs4
    cr ." rulestore4: " dup .rulestore cr

    2dup rulestore-union                \ rs1 rs4, rsx true | false
    if
        cr ." rulestore-union rs1 rs4 should not have succeeded" cr
        abort
    then
    rulestore-deallocate                \ rs1
    
    rulestore-deallocate

    cr ." rulestore-test-new - Ok" cr
;

: rulestore-tests
    rulestore-test-union
;
