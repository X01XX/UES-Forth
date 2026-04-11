\ Tests for the square-list struct functions.

\ Return square states in a region.
: square-list-states-in-region ( reg1 sqr-lst0 -- ret-sta-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    \ Init return list.
    list-new -rot                   \ ret-lst reg1 sqr-lst0
    list-get-links                  \ ret-lst reg1 link
    begin
        ?dup
    while
        dup link-get-data           \ ret-lst reg1 link sqrx
        square-get-state            \ ret-lst reg1 link stax
        #2 pick                     \ ret-lst reg1 link stax reg1
        region-superset-of-state?   \ ret-lst reg1 link flag
        if
            \ Add state to return list.
            dup link-get-data       \ ret-lst reg1 link sqrx
            square-get-state        \ ret-lst reg1 link stax
            #3 pick                 \ ret-lst reg1 link stax ret-lst
            list-push               \ ret-lst reg1 link
        then

        link-get-next
    repeat
                                    \ ret-lst reg1
    drop                            \ ret-lst
;

: square-list-test-in-region
    s" X1X1" region-from-string-a                 \ reg

    list-new                                    \ reg sqrs

     #3  #3 square-new over square-list-push    \ reg sqrs
     #7  #7 square-new over square-list-push    \ reg sqrs
    #12 #12 square-new over square-list-push    \ reg sqrs
    #13 #13 square-new over square-list-push    \ reg sqrs

    2dup square-list-in-region                  \ reg sqrs sqrs-in

    \ Check list length.
    dup list-get-length
    #2 <>
    abort" list length not 2?"

    \ Check the correct squares were returned.
    #7 over square-list-member
    0= abort" 7 NOT found"

    #13 over square-list-member
    0= abort" 13 NOT found"

    \ Deallocate
    square-list-deallocate
    square-list-deallocate
    region-deallocate

    cr ." square-list-test-in-region: Ok"
;

: square-list-test-states-in-region
    s" X1X1" region-from-string-a               \ reg
    list-new                                    \ reg sqrs

     #3  #3 square-new over square-list-push    \ reg sqrs
     #7  #7 square-new over square-list-push    \ reg sqrs
    #12 #12 square-new over square-list-push    \ reg sqrs
    #13 #13 square-new over square-list-push    \ reg sqrs

    2dup square-list-states-in-region           \ reg sqrs stas-in

    \ Check list length.
    dup list-get-length
    #2 <>
    abort" list length not 2?"

    \ Check the correct squares were returned.
    [ ' = ] literal #7 #2 pick list-member
    0= abort" 7 NOT found"

    [ ' = ] literal #13 #2 pick list-member
    0= abort" 13 NOT found"

    \ Deallocate
    list-deallocate
    square-list-deallocate
    region-deallocate

    cr ." square-list-test-states-in-region: Ok"
;

: square-list-tests
    session-new                                     \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    over                                            \ sess dom0 sess
    session-add-domain                              \ sess

    square-list-test-in-region
    square-list-test-states-in-region

    session-deallocate
;

