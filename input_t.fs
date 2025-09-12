: input-test-parse-user-input
    depth 0<> abort" Test 0 stack not empty"

    \ Straight-forward test.
    s" ab cde fghi" parse-user-input
    3 <>
    abort" Three tokens not found"

    s" ab"   compare 0<> abort" ab not found"
    s" cde"  compare 0<> abort" cde not found"
    s" fghi" compare 0<> abort" fghi not found"

    depth 0<> abort" Test 1 stack not empty"

    \ Double up separators, and at start and end.
    s"  ab cde  fghi " parse-user-input
    3 <>
    abort" Three tokens not found"

    s" ab"   compare 0<> abort" ab not found"
    s" cde"  compare 0<> abort" cde not found"
    s" fghi" compare 0<> abort" fghi not found"

    depth 0<> abort" Test 2 stack not empty"
    
    \ Try no string.
    s" " parse-user-input
    0 <>
    abort" No tokens not found"

    depth 0<> abort" Test 3 stack not empty"
    
    \ Try only one separator.
    s"  " parse-user-input
    0 <>
    abort" No tokens not found"

    depth 0<> abort" Test 4 stack not empty"
    
    \ Try only two separators.
    s"   " parse-user-input
    0 <>
    abort" No tokens not found"

    depth 0<> abort" Test 5 stack not empty"

    cr ." input-test-parse-user-input: Ok" cr
;

