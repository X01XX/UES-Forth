\ Tests for the region struct functions.

: region-test-region-subtract
    \ cr ." region-test-region-subtract - start"

    s" X10X" region-from-string-a   \ reg1
    s" 1XX0" region-from-string-a   \ reg1 reg2

    2dup                            \ reg1 reg2 reg1 reg2
    region-subtract                 \ reg1 reg2 reg-lst

    swap region-deallocate          \ reg1 reg-lst
    swap region-deallocate          \ reg-lst

    dup list-get-length #2 <>
    abort" List length not 2?"

    s" 10X0" region-from-string-a   \ reg-lst reg3
    over                            \ reg-lst reg3 reg-lst
    over swap                       \ reg-lst reg3 reg3 reg-lst
    region-list-member              \ reg-lst reg3 flag
    0= abort" Region 10X0 not found?"

                                    \ reg-lst reg3
    region-deallocate               \ reg-lst

    s" 1X10" region-from-string-a   \ reg-lst reg3

    over                            \ reg-lst reg3 reg-lst
    over swap                       \ reg-lst reg3 reg3 reg-lst
    region-list-member              \ reg-lst reg3 flag
    0= abort" Region 1X10 not found?"

                                    \ reg-lst reg3
    region-deallocate               \ reg-lst
    
    region-list-deallocate

    cr ." region-test-region-subtract - Ok" cr
;

: region-test-states-in
    \ Make state list.
    list-new                                \ sta-lst
     #2 over list-push
     #4 over list-push
     #6 over list-push
     #7 over list-push
    #12 over list-push
    #13 over list-push
    #15 over list-push

    \ Make region
    s" XXX1" region-from-string-a           \ sta-lst reg

    \ Get states in region
    2dup region-states-in                   \ sta-lst reg sta-lst2

    \ cr ." states in: " dup .list-raw cr

    dup list-get-length
    #3 <>
    abort" List length not 3?"

    [ ' = ] literal #7 #2 pick list-member
    0= abort" 7 not in list?"

    [ ' = ] literal #13 #2 pick list-member
    0= abort" 13 not in list?"

    [ ' = ] literal #15 #2 pick list-member
    0= abort" 15 not in list?"

    list-deallocate
    region-deallocate
    list-deallocate

    cr ." region-test-states-in - Ok" cr
;

: region-tests
    0 set-domain
    region-test-region-subtract
    region-test-states-in
;

