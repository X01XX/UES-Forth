: rulestore-test-union

    \ Test two empty rulestores.
    rulestore-new-0
    rulestore-new-0
    2dup rulestore-union                \ rs1 rs2, rs3 true | false
    0= abort" Rulestore union of two empty rulestores should work"
    rulestore-deallocate
    rulestore-deallocate
    rulestore-deallocate

    \ Test two single-rule rulestores, that are compatible.
    s" 00/11/00/11/" rule-from-string rulestore-new-1
    s" 11/11/00/11/" rule-from-string rulestore-new-1
    2dup rulestore-union                \ rs1 rs2, rs3 true | false
    0= abort" Rulestore union of two single-rule rulestores should work"
    rulestore-deallocate
    rulestore-deallocate
    rulestore-deallocate

    \ Test two single-rule rulestores, that are not compatible.
    s" 00/11/00/11/" rule-from-string rulestore-new-1
    s" 11/11/00/10/" rule-from-string rulestore-new-1
    2dup rulestore-union                \ rs1 rs2, rs3 true | false
    abort" Rulestore union of two single-rule rulestores should not work"
    rulestore-deallocate
    rulestore-deallocate

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
        cr dup ." union:      " .rulestore cr
        rulestore-deallocate
    else
        cr ." rulestore-union rs1 rs2 failed" cr
        abort
    then
    rulestore-deallocate                \ rs1

    \ Try different order.
    s" 00/11/11/10/" rule-from-string   \ rs1 rul1
    s" 00/11/11/11/" rule-from-string   \ rs1 rul1 rul2
    rulestore-new-2                     \ rs1 rs2b
    cr ." rulestor2b: " dup .rulestore cr

    2dup rulestore-union                \ rs1 rs3b, rsx true | false
    if
        cr dup ." union:      " .rulestore cr
        rulestore-deallocate
    else
        cr ." rulestore-union rs1 rs2b failed" cr
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

    \ Test two pn-2 rulestores that should form a union, using change mask only.
    s" 11/11/11/00/" rule-from-string   \ rs1 rul1
    s" 11/11/11/01/" rule-from-string   \ rs1 rul1 rul2
    rulestore-new-2                     \ rs1 rs4
    cr ." rulestore1: " over .rulestore cr
    cr ." rulestore4: " dup .rulestore cr

    2dup rulestore-union                \ rs1 rs4, rsx true | false
    if
    cr ." union:      " dup .rulestore cr
        rulestore-deallocate
    else
        cr ." rulestore-union rs1 rs4 did not succeed?" cr
        abort
    then
    rulestore-deallocate                \ rs1
    rulestore-deallocate

    cr ." rulestore-test-union - Ok" cr
;

: rulestore-tests
    rulestore-test-union
;
