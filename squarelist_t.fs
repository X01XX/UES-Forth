\ Tests for the square-list struct functions.

: square-list-test-none-in-use
    assert-square-mma-none-in-use
    assert-rulestore-mma-none-in-use
    assert-rule-mma-none-in-use
    assert-list-mma-none-in-use
    assert-link-mma-none-in-use
    depth 0<>
    if
        cr ." stack not empty " .s cr
    then
;

: square-list-test-in-region
    cr ." square-test-in-region - start"

    square-list-test-none-in-use

    5 15 region-new                         \ reg
    list-new                                \ reg sqrs
    
     3  3 square-new over square-list-push  \ reg sqrs
     7  7 square-new over square-list-push  \ reg sqrs
    12 12 square-new over square-list-push  \ reg sqrs
    13 13 square-new over square-list-push  \ reg sqrs

    2dup square-list-in-region              \ reg sqrs sqrs-in

    \ cr ." squares in " dup .square-list cr

    \ Check list length.
    dup list-get-length
    2 <>
    if
        ." list length not 2?"
        abort
    then

    \ Check the correct squares were returned.
    7 over square-list-find
    if
\        cr ." 7 found" cr
        drop
    else
        cr ." 7 NOT found" cr
        abort
    then

    13 over square-list-find
    if
\        cr ." 13 found" cr
        drop
    else
        cr ." 13 NOT found" cr
        abort
    then

    \ Deallocate
    square-list-deallocate
    square-list-deallocate
    region-deallocate

    square-list-test-none-in-use

    cr ." square-test-in-region - Ok" cr
;

: square-list-tests
    square-list-test-in-region
;

