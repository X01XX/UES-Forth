\ Tests for the square-list struct functions.

: square-list-test-in-region
    cr ." square-test-in-region - start"

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
    abort" list length not 2?"

    \ Check the correct squares were returned.
    7 over square-list-find
    0= abort" 7 NOT found"

    drop

    13 over square-list-find
    0= abort" 13 NOT found"

    drop

    \ Deallocate
    square-list-deallocate
    square-list-deallocate
    region-deallocate

    cr ." square-test-in-region - Ok" cr
;

: square-list-tests
    square-list-test-in-region
;

