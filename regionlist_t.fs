\ Tests for the region-list struct functions.

: region-list-test-region-intersections-n

    s" (X10X)" region-list-from-string-a        \ lst1

    s" (1XX1 0XX1)" region-list-from-string-a   \ lst1

    2dup region-list-intersections-n            \ lst1 lst2 lst3

    s" (X101)" region-list-from-string-a        \ lst1 lst2 lst3 lst4
    2dup region-list-eq                         \ lst1 lst2 lst3 lst4 bool
    0= abort" X101 not found"

    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-test-region-intersections-n: Ok"
;

: region-list-test-subtract-n

    \ Make subtrahend list.
    s" (1XX1 0XX1)" region-list-from-string-a   \ lst1 

    \ Make minuend list.
    s" (0000 X10X)" region-list-from-string-a   \ lst1 lst2

    \ Subtract regions.
    2dup region-list-subtract-n                 \ lst1 lst2 lst3

    \ Check results.
    s" (X100 0X00)" region-list-from-string-a   \ lst1 lst2 lst3 lst4

    2dup region-list-eq                         \ lst1 lst2 lst3 lst4 bool
    is-false abort" region-list-test-subtract-n: region-lists invaid?"

    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-subtract-n: Ok"
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

    cr ." region-list-test-states: Ok"
;

: region-list-test-state-in-one-region

    \ Make a region-list.
    s" (X1X1 X10X 01XX)" region-list-from-string-a  \ lst1

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

    cr ." region-list-test-state-in-one-region: Ok"
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
    s" (X1X1 X10X 01XX)" region-list-from-string-a  \ sta-lst rge-lst

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

    cr ." region-list-test-states-in-one-region: Ok"
;

: region-list-test-intersection-fragments
    \ Make a region-list.
    s" (XX1X 1XXX X1X1 X1X1)" region-list-from-string-a \ lst1

    dup                                     \ lst1 lst1
    region-list-intersection-fragments      \ lst1 lst2

    \ Check.
    s" (1111 101x 1x10 0111 1101 0101 100x 1x00 0x10 001x)" region-list-from-string-a   \ lst1 lst2 lst3
    2dup region-list-eq                     \ lst1 lst2 lst3 bool
    is-false abort" region-list-test-intersection-fragments: 1 region lists ne?"
   
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    \ Test with subset.

    \ Make a region-list.
    s" (011X XX1X)" region-list-from-string-a   \ lst1

    \ cr ." lst1: " dup .region-list cr

    dup                                     \ lst1 lst1
    region-list-intersection-fragments      \ lst1 lst2

    \ cr ." fragments 2: " dup .region-list     \ lst1 lst2

    \ Check length.
    dup list-get-length                     \ lst1 lst2 len
    #3 <> abort" Result length not 3?"

    \ Check list.
    s" (011X 1X1X X01X)" region-list-from-string-a  \ lst1 lst2 lst3

    2dup region-list-eq                             \ lst1 lst2 lst3
    is-false abort" region-list-test-intersection-fragments: 2 region lists ne?"

    \ Clean up.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-intersection-fragments: Ok"
;

: region-list-test-normalize
    s" (X101 X100 0111 00X1)" region-list-from-string-a \ lst1

    dup region-list-normalize               \ lst1 lst2
    \ cr ." rslt: " dup .region-list cr

    \ Check results.
    s" (X10X 0XX1)" region-list-from-string-a   \ lst1 lst2 lst3
    2dup region-list-eq                         \ lst1 lst2 lst3 bool
    is-false abort" region-list-test-normalize: 1 region lists ne?"

    \ Clean up.
    region-list-deallocate                  \ lst1 lst2
    region-list-deallocate                  \ lst1
    region-list-deallocate                  \

    cr ." region-list-test-normalize: Ok"
;

: region-list-test-copy-except
    s" (X100 0111 00X1)" region-list-from-string-a \ lst1

    s" X111" region-from-string-a           \ lst1 regx
    1                                       \ lst1 regx 1
    #2 pick                                 \ lst1 regx lst1
    region-list-copy-except                 \ lst1 lst2

    \ Check results.
    s" (X100 X111 00X1)" region-list-from-string-a  \ lst1 lst2 lst3
    2dup region-list-eq                             \ lst1 lst2 lst3 bool
    is-false abort" region-list-test-copy-except: region lists ne?"

    \ Clean up.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-copy-except: Ok"
;

: region-list-test-push-nosubs
    \ Test 1, pushing a region that is a subset of regions in a list.
    s" 10x1" region-from-string-a               \ reg'
    s" (xx01 10xx)" region-list-from-string-a   \ reg' reg-lst'
    2dup
    region-list-push-nosubs                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed?" cr
        abort
    else
        cr ." reg NOT pushed"
        dup list-get-length                     \ reg1` reg-lst' len
        #2 = if
        else
            cr ." list len ne 2?" cr
            abort
        then
    then

    region-list-deallocate
    region-deallocate

    \ Test 2, pushing a region that is not a subset of regions in a list.
    s" 0xx0" region-from-string-a               \ reg'
    s" (xx01 10xx)" region-list-from-string-a   \ reg' reg-lst'
    2dup
    region-list-push-nosubs                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed" cr
        cr dup .region-list cr
        dup list-get-length                     \ reg1` reg-lst' len
        #3 = if
        else
            cr ." list len ne 3?" cr
            abort
        then
    else
        cr ." reg NOT pushed?"
        abort
    then

    region-list-deallocate
    drop

    \ Test 3, pushing a region that is not a superset of a region in a list.
    s" 1xxx" region-from-string-a               \ reg'
    s" (xx01 10xx)" region-list-from-string-a   \ reg' reg-lst'
    2dup
    region-list-push-nosubs                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed" cr
        cr dup .region-list cr
        dup list-get-length                     \ reg1` reg-lst' len
        #2 = if
        else
            cr ." list len ne 2?" cr
            abort
        then
    else
        cr ." reg NOT pushed?"
        abort
    then

    region-list-deallocate
    drop

    cr ." region-list-test-push-nosubs: Ok"
;

: region-list-tests
    0 set-domain
    region-list-test-region-intersections-n
    region-list-test-subtract-n
    region-list-test-states
    region-list-test-states-in-one-region
    region-list-test-intersection-fragments
    region-list-test-normalize
    region-list-test-copy-except
;

