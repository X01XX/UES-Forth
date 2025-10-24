\ Tests for the region-list struct functions.

: region-list-test-region-intersections-n

    list-new                                \ lst1
     s" X10X" region-from-string-a          \ lst1 regx
    over region-list-push                   \ lst1
    
    list-new                                \ lst1 lst2
    s" 0XX1" region-from-string-a           \ lst1 lst2 regx
    over region-list-push                   \ lst1 lst2

    s" 1XX1" region-from-string-a           \ lst1 lst2 regx
    over region-list-push                   \ lst1 lst2

    2dup region-list-region-intersections-n \ lst1 lst2 lst3

    dup list-get-length 1 <>
    abort" Result length invalid "

     s" X101" region-from-string-a  \ lst1 lst2 lst3 region

     over                       \ lst1 lst2 lst3 region lst3
     over swap                  \ lst1 lst2 lst3 region region lst3
     region-list-member         \ lst1 lst2 lst3 region flag
     0= abort" X101 not found"

    region-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-test-region-intersections-n - Ok"
;

: region-list-test-subtract-n

    \ Make subtrahend list.
    list-new                                \ lst1
    s" 0XX1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" 1XX1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    \ Make minuend list.
    list-new                                \ lst1 lst2
    s" X10X" region-from-string-a           \ lst1 lst2 regx
    over region-list-push                   \ lst1 lst2

    s" 0000" region-from-string-a           \ lst1 lst2 regx
    over region-list-push                   \ lst1 lst2

    \ Subtract regions.
    2dup region-list-subtract-n               \ lst1 lst2 lst3

    \ Check results.

    dup list-get-length #2 <>
    abort" list length invalid"

    s" 0X00" region-from-string-a   \ lst1 lst2 lst3 reg
    over                            \ lst1 lst2 lst3 reg lst3
    
    over swap                       \ lst1 lst2 lst3 reg reg lst3
    region-list-member              \ lst1 lst2 lst3 reg flag
    0= abort" 0x00 not found"
                                    \ lst1 lst2 lst3 reg
    region-deallocate               \ lst1 lst2 lst3

    s" X100" region-from-string-a   \ lst1 lst2 lst3 reg
    over                            \ lst1 lst2 lst3 reg lst3
    
    over swap                       \ lst1 lst2 lst3 reg reg lst3
    region-list-member              \ lst1 lst2 lst3 reg flag
    0= abort" x100 not found in "
                                    \ lst1 lst2 lst3 reg
    region-deallocate               \ lst1 lst2 lst3

    \ Finish.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-subtract-n - Ok"
;

: region-list-test-states
    \ Make region-list list, using regions made with duplicate states.
    list-new                                \ lst1
    #4  #7 region-new over region-list-push \ lst1
    #4 #13 region-new over region-list-push \ lst1
    #7 #13 region-new over region-list-push \ lst1

    dup region-list-states                  \ lst1 lst2

    dup list-get-length
    #3 <>
    abort" List length not 3?"

    [ ' = ] literal #4 #2 pick list-member
    0= abort" 4 not in list?"

    [ ' = ] literal #7 #2 pick list-member
    0= abort" 7 not in list?"

    [ ' = ] literal #13 #2 pick list-member
    0= abort" 13 not in list?"

    list-deallocate
    region-list-deallocate
    
    cr ." region-list-test-states - Ok"
;

: region-list-test-state-in-one-region

    \ Make a region-list.
    list-new                                \ lst1
    s" 01XX" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" X10X" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" X1X1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    #2 over                                 \ lst1 2 lst1
    region-list-state-in-one-region         \ lst1 flag
    abort" 2 in one region?"

    #4 over                                 \ lst1 4 lst1
    region-list-state-in-one-region         \ lst1 | flag
    abort" 4 in one region?"

    #6 over                                 \ lst1 6 lst1
    region-list-state-in-one-region         \ lst1 flag
    0= abort" 6 not in one region?"

    #7 over                                 \ lst1 7 lst1
    region-list-state-in-one-region         \ lst1 | flag
    abort" 7 in one region?"

    #12 over                                \ lst1 12 lst1
    region-list-state-in-one-region         \ lst1 flag
    0= abort" 12 not in one region?"

    #13 over                                \ lst1 13 lst1
    region-list-state-in-one-region         \ lst1 flag
    abort" 13 in one region?"

    #15 over                                \ lst1 15 lst1
    region-list-state-in-one-region         \ lst1 | flag
    0= abort" 15 not in one region?"

    region-list-deallocate
    
    cr ." region-list-test-state-in-one-region - Ok"
;

: region-list-test-states-in-one-region

    \ Make state list.
    list-new                                \ sta-lst
     #2 over list-push
     #4 over list-push
     #6 over list-push
     #7 over list-push
    #12 over list-push
    #13 over list-push
    #15 over list-push

    \ Make a region-list.
    list-new                                \ sta-lst reg-lst
    s" 01XX" region-from-string-a           \ sta-lst reg-lst regx
    over region-list-push                   \ sta-lst reg-lst

    s" X10X" region-from-string-a           \ sta-lst reg-lst regx
    over region-list-push                   \ sta-lst reg-lst

    s" X1X1" region-from-string-a           \ sta-lst reg-lst regx
    over region-list-push                   \ sta-lst reg-lst

    2dup region-list-states-in-one-region   \ sta-lst reg-lst sta-lst2

    \ cr ." states in one region: " dup .list-raw cr

    dup list-get-length
    #3 <>
    abort" List length not 3?"

    [ ' = ] literal #6 #2 pick list-member
    0= abort" 6 not in list?"

    [ ' = ] literal #12 #2 pick list-member
    0= abort" 12 not in list?"

    [ ' = ] literal #15 #2 pick list-member
    0= abort" 15 not in list?"

    list-deallocate
    region-list-deallocate
    list-deallocate

    cr ." region-list-test-states-in-one-region - Ok"
;

: region-list-test-unique-parts
    \ Make a region-list.
    list-new                                \ lst1

    s" X1X1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" X1X1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" 1XXX" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" XX1X" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    \ cr ." lst1: " dup .region-list cr

    dup                                     \ lst1 lst1
    region-list-unique-parts                \ lst1 lst2

    \ cr ." unique parts: " dup .region-list cr

    dup list-get-length                     \ lst1 lst2 len
    #5 <> abort" list not 5 long"           \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 0X10" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 0X10 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 001X" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 001X not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 1X00" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 1X00 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 100X" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 100X not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 0101" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 0101 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-unique-parts - Ok"
;

: region-list-test-intersection-fragments
    \ Make a region-list.
    list-new                                \ lst1

    s" X1X1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" X1X1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" 1XXX" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" XX1X" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    \ cr ." lst1: " dup .region-list cr

    dup                                     \ lst1 lst1
    region-list-intersection-fragments      \ lst1 lst2

    \ cr ." fragments: " dup .region-list cr

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 1111" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 1111 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 1101" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 1101 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 0111" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 0111 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 1X10" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 1X10 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 101X" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 101X not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 0X10" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 0X10 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 001X" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 001X not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 1X00" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 1X00 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 100X" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 100X not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 0101" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 0101 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-intersection-fragments - Ok"
;

: region-list-test-normalize
    list-new                                \ lst1

    s" 00X1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" 0111" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" X100" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" X101" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    dup region-list-normalize               \ lst1 lst2
    \ cr ." rslt: " dup .region-list cr

    \ Check results.
    dup list-get-length 2 <>                \ lst1 lst2 flag
    abort" result length not 2?"

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" 0XX1" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" 0XX1 not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Check member.
    dup                                     \ lst1 lst2 lst2
    s" X10X" region-from-string-a tuck      \ lst1 lst2 regx lst2 regx
    swap                                    \ lst1 lst2 regx regx lst2
    region-list-member 0=                   \ lst1 lst2 regx flag
    abort" X10X not a list member?"         \ lst1 lst2 regx
    region-deallocate                       \ lst1 lst2

    \ Clean up.
    region-list-deallocate                  \ lst1
    region-list-deallocate                  \

    cr ." region-list-test-normalize - Ok"
;

: region-list-test-copy-except
    list-new                                \ lst1

    s" 00X1" region-from-string-a           \ lst1 regx
    over region-list-push                   \ lst1

    s" 0111" region-from-string-a           \ lst1 regx
    over region-list-push-end               \ lst1

    s" X100" region-from-string-a           \ lst1 regx
    over region-list-push-end               \ lst1

    s" X111" region-from-string-a           \ lst1 regx
    1                                       \ lst1 regx 1
    #2 pick                                 \ lst1 regx lst1
    region-list-copy-except                 \ lst1 lst2

    \ Check results.
    dup list-get-length 3 <> abort" Result not length 3?"

    0 over list-get-item                    \ lst1 lst2 item0
    s" 00X1" region-from-string-a           \ lst1 lst2 item0 regx
    tuck region-eq                          \ lst1 slt2 regx flag
    swap region-deallocate                  \ lst1 lst2 flag
    is-false abort" first region not 00X1?" \ lst1 lst2

    1 over list-get-item                    \ lst1 lst2 item0
    s" X111" region-from-string-a           \ lst1 lst2 item0 regx
    tuck region-eq                          \ lst1 slt2 regx flag
    swap region-deallocate                  \ lst1 lst2 flag
    is-false abort" first region not X111?" \ lst1 lst2

    2 over list-get-item                    \ lst1 lst2 item0
    s" X100" region-from-string-a           \ lst1 lst2 item0 regx
    tuck region-eq                          \ lst1 slt2 regx flag
    swap region-deallocate                  \ lst1 lst2 flag
    is-false abort" first region not X100?" \ lst1 lst2

    \ Clean up.
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-copy-except - Ok"
;

: region-list-tests
    region-list-test-region-intersections-n
    region-list-test-subtract-n
    region-list-test-states
    region-list-test-states-in-one-region
    region-list-test-unique-parts
    region-list-test-intersection-fragments
    region-list-test-normalize
    region-list-test-copy-except
;

