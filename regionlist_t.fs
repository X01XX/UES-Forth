\ Tests for the region-list struct functions.

: region-list-test-region-intersections-n

    s" (rX10X)" region-list-from-string-a       \ lst1

    s" (r1XX1 r0XX1)" region-list-from-string-a \ lst1

    2dup region-list-intersections-n            \ lst1 lst2 lst3

    s" (rX101)" region-list-from-string-a       \ lst1 lst2 lst3 lst4
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
    s" (r1XX1 r0XX1)" region-list-from-string-a   \ lst1

    \ Make minuend list.
    s" (r0000 rX10X)" region-list-from-string-a   \ lst1 lst2

    \ Subtract regions.
    2dup region-list-subtract-n                 \ lst1 lst2 lst3

    \ Check results.
    s" (rX100 r0X00)" region-list-from-string-a   \ lst1 lst2 lst3 lst4

    2dup region-list-eq                         \ lst1 lst2 lst3 lst4 bool
    false? abort" region-list-test-subtract-n: region-lists invaid?"

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
    s" (rX1X1 rX10X r01XX)" region-list-from-string-a   \ lst1

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

: region-list-test-intersection-fragments
    \ Make a region-list.
    s" (rXX1X r1XXX rX1X1 rX1X1)" region-list-from-string-a \ lst1

    dup                                     \ lst1 lst1
    region-list-intersection-fragments      \ lst1 lst2

    \ Check.
    s" (r1111 r101x r1x10 r0111 r1101 r0101 r100x r1x00 r0x10 r001x)" region-list-from-string-a \ lst1 lst2 lst3
    2dup region-list-eq                     \ lst1 lst2 lst3 bool
    false? abort" region-list-test-intersection-fragments: 1 region lists ne?"

    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    \ Test with subset.

    \ Make a region-list.
    s" (r011X rXX1X)" region-list-from-string-a \ lst1

    \ cr ." lst1: " dup .region-list cr

    dup                                     \ lst1 lst1
    region-list-intersection-fragments      \ lst1 lst2

    \ cr ." fragments 2: " dup .region-list     \ lst1 lst2

    \ Check length.
    dup list-get-length                     \ lst1 lst2 len
    #3 <> abort" Result length not 3?"

    \ Check list.
    s" (r011X r1X1X rX01X)" region-list-from-string-a   \ lst1 lst2 lst3

    2dup region-list-eq                             \ lst1 lst2 lst3
    false? abort" region-list-test-intersection-fragments: 2 region lists ne?"

    \ Clean up.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-intersection-fragments: Ok"
;

: region-list-test-normalize
    s" (rX101 rX100 r0111 r00X1)" region-list-from-string-a \ lst1

    dup region-list-normalize               \ lst1 lst2
    \ cr ." rslt: " dup .region-list cr

    \ Check results.
    s" (rX10X r0XX1)" region-list-from-string-a \ lst1 lst2 lst3
    2dup region-list-eq                         \ lst1 lst2 lst3 bool
    false? abort" region-list-test-normalize: 1 region lists ne?"

    \ Clean up.
    region-list-deallocate                  \ lst1 lst2
    region-list-deallocate                  \ lst1
    region-list-deallocate                  \

    cr ." region-list-test-normalize: Ok"
;

: region-list-test-copy-except
    s" (rX100 r0111 r00X1)" region-list-from-string-a   \ lst1

    s" rX111" region-from-string-a          \ lst1 regx
    1                                       \ lst1 regx 1
    #2 pick                                 \ lst1 regx lst1
    region-list-copy-except                 \ lst1 lst2

    \ Check results.
    s" (rX100 rX111 r00X1)" region-list-from-string-a   \ lst1 lst2 lst3
    2dup region-list-eq                             \ lst1 lst2 lst3 bool
    false? abort" region-list-test-copy-except: region lists ne?"

    \ Clean up.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-copy-except: Ok"
;

: region-list-test-push-nosubs
    \ Test 1, pushing a region that is a subset of regions in a list.
    s" r10x1" region-from-string-a               \ reg'
    s" (rxx01 r10xx)" region-list-from-string-a \ reg' reg-lst'
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
    s" r0xx0" region-from-string-a              \ reg'
    s" (rxx01 r10xx)" region-list-from-string-a \ reg' reg-lst'
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
    s" r1xxx" region-from-string-a              \ reg'
    s" (rxx01 r10xx)" region-list-from-string-a \ reg' reg-lst'
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

: region-list-test-push-nosups
    \ Test 1, pushing a region that is a superset of regions in a list.
    s" r1xx1" region-from-string-a              \ reg'
    s" (r1x01 r10x1)" region-list-from-string-a \ reg' reg-lst'
    2dup
    region-list-push-nosups                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed?" cr
        cr dup .region-list cr
        abort
    else
        cr ." reg NOT pushed"
        cr dup .region-list cr
        dup list-get-length #2 <> abort" List not 2 in length?"
    then

    region-list-deallocate
    region-deallocate

    \ Test 2, pushing a region that is not a superset of regions in a list.
    s" r1110" region-from-string-a              \ reg'
    s" (r1x01 r10x1)" region-list-from-string-a \ reg' reg-lst'
    2dup
    region-list-push-nosups                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed" cr
        cr dup .region-list cr
        dup list-get-length #3 <> abort" list len not 3?"
    else
        cr ." reg NOT pushed?"
        cr dup .region-list cr
        abort
    then

    region-list-deallocate
    drop

    \ Test 3, pushing a region that is a subset of regions in a list.
    s" r1011" region-from-string-a              \ reg'
    s" (r1x01 r10x1)" region-list-from-string-a \ reg' reg-lst'
    2dup
    region-list-push-nosups                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed" cr
        cr dup .region-list cr
        dup list-get-length #2 <> abort" list len not 3?"
    else
        cr ." reg NOT pushed?"
        cr dup .region-list cr
        abort
    then

    region-list-deallocate
    drop

    cr ." region-list-test-push-nosups: Ok"
;

: region-list-test-remove-superset
    \ Test 1, pushing a region that is a subset of regions in a list.
    s" r1001" region-from-string-a              \ reg'
    s" (r1x01 r00x1)" region-list-from-string-a \ reg' reg-lst'
    2dup
    region-list-remove-superset                 \ reg1` reg-lst' bool
    if
        cr dup .region-list cr
        cr ." reg removed" cr
    else
        cr dup .region-list cr
        cr ." reg not removed"
        abort
    then

    region-list-deallocate
    region-deallocate

    cr ." region-list-test-remove-superset: Ok"
;

: region-list-tests
    session-new                                     \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    2dup                                            \ sess dom0 sess dom0
    swap                                            \ sess dom0 dom0 sess
    session-add-domain                              \ sess dom0

    region-list-test-region-intersections-n
    region-list-test-subtract-n
    region-list-test-states
    region-list-test-state-in-one-region
    region-list-test-intersection-fragments
    region-list-test-normalize
    region-list-test-copy-except
    region-list-test-push-nosubs
    region-list-test-push-nosups
    region-list-test-remove-superset

    drop

    session-deallocate
;

