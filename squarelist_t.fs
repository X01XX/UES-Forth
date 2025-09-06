\ Tests for the square-list struct functions.

: square-list-test-in-region
    s" X1X1" region-from-string                 \ reg

    list-new                                    \ reg sqrs

     3  3 square-new over square-list-push      \ reg sqrs
     7  7 square-new over square-list-push      \ reg sqrs
    #12 #12 square-new over square-list-push    \ reg sqrs
    #13 #13 square-new over square-list-push    \ reg sqrs

    2dup square-list-in-region                  \ reg sqrs sqrs-in

    \ Check list length.
    dup list-get-length
    2 <>
    abort" list length not 2?"

    \ Check the correct squares were returned.
    7 over square-list-member
    0= abort" 7 NOT found"

    #13 over square-list-member
    0= abort" 13 NOT found"

    \ Deallocate
    square-list-deallocate
    square-list-deallocate
    region-deallocate

    cr ." square-list-test-in-region - Ok"
;

: square-list-test-states-in-region
    s" X1X1" region-from-string                 \ reg
    list-new                                    \ reg sqrs

     3  3 square-new over square-list-push      \ reg sqrs
     7  7 square-new over square-list-push      \ reg sqrs
    #12 #12 square-new over square-list-push    \ reg sqrs
    #13 #13 square-new over square-list-push    \ reg sqrs

    2dup square-list-states-in-region           \ reg sqrs stas-in

    \ Check list length.
    dup list-get-length
    2 <>
    abort" list length not 2?"

    \ Check the correct squares were returned.
    [ ' = ] literal 7 2 pick list-member
    0= abort" 7 NOT found"

    [ ' = ] literal #13 2 pick list-member
    0= abort" 13 NOT found"

    \ Deallocate
    list-deallocate
    square-list-deallocate
    region-deallocate

    cr ." square-list-test-states-in-region - Ok"
;

: square-list-tests
    square-list-test-in-region
    square-list-test-states-in-region
;

