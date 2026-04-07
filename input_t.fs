: input-test-token-list-from-string
    \ Straight-forward test.
    s" ab cde fghi" token-list-from-string
    dup list-get-length #3 <>
    abort" Three tokens not found"

    dup list-get-first-item token-get-string s" ab"   compare 0<> abort" ab not found"
    dup list-get-second-item token-get-string s" cde"  compare 0<> abort" cde not found"
    dup list-get-last-item token-get-string s" fghi" compare 0<> abort" fghi not found"
    token-list-deallocate

    depth 0<> abort" Test 1 stack not empty"

    \ Double up separators, and at start and end.
    s"  ab cde  fghi " token-list-from-string
    dup list-get-length #3 <>
    abort" Three tokens not found"

    dup list-get-first-item token-get-string s" ab"   compare 0<> abort" ab not found"
    dup list-get-second-item token-get-string s" cde"  compare 0<> abort" cde not found"
    dup list-get-last-item token-get-string s" fghi" compare 0<> abort" fghi not found"
    token-list-deallocate

    depth 0<> abort" Test 2 stack not empty"

    \ Try no string.
    s" " token-list-from-string
    dup list-get-length 0 <>
    abort" No tokens not found"
    list-deallocate

    depth 0<> abort" Test 3 stack not empty"

    \ Try only one separator.
    s"  " token-list-from-string
    dup list-get-length 0 <>
    abort" No tokens not found"
    list-deallocate

    depth 0<> abort" Test 4 stack not empty"

    \ Try only two separators.
    s"   " token-list-from-string
    dup list-get-length 0 <>
    abort" No tokens not found"
    list-deallocate

    cr ." input-test-token-list-from-string: Ok" cr
;

: input-tests
    input-test-token-list-from-string
;
