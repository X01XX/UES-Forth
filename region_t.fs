\ Tests for the region struct functions.

: region-test-region-subtract
    \ cr ." region-test-region-subtract - start"

     4 13 region-new    \ reg1
    10 12 region-new    \ reg1 reg2
    2dup                \ reg1 reg2 reg1 reg2
    region-subtract     \ reg1 reg2 reg-lst

    swap region-deallocate  \ reg1 reg-lst
    swap region-deallocate  \ reg-lst

    dup list-get-length 2 <>
    abort" List length not 2?"

    8 10 region-new                 \ reg-lst reg3
    over                            \ reg-lst reg3 reg-lst
    over swap                       \ reg-lst reg3 reg3 reg-lst
    region-list-member              \ reg-lst reg3 flag
    0= abort" Region 10X0 not found?"

                                    \ reg-lst reg3
    region-deallocate               \ reg-lst
    
    14 10 region-new                \ reg-lst reg3
    over                            \ reg-lst reg3 reg-lst
    over swap                       \ reg-lst reg3 reg3 reg-lst
    region-list-member              \ reg-lst reg3 flag
    0= abort" Region 1X10 not found?"

                                    \ reg-lst reg3
    region-deallocate               \ reg-lst
    
    region-list-deallocate

    cr ." region-test-region-subtract - Ok" cr
;

: region-tests
    region-test-region-subtract
;

