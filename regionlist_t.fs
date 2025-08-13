\ Tests for the region-list struct functions.

: region-list-test-region-intersections-n

    list-new                                \ lst1
     4 13 region-new over region-list-push  \ lst1
    list-new                                \ lst1 lst2
     1  7 region-new over region-list-push  \ lst1 lst2
    13 11 region-new over region-list-push  \ lst1 lst2

    2dup region-list-region-intersections-n \ lst1 lst2 lst3

    dup list-get-length 1 <>
    abort" Result length invalid "

     5 13 region-new            \ lst1 lst2 lst3 region
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
      1  7 region-new over region-list-push \ lst1
     13 11 region-new over region-list-push \ lst

    \ Make minuend list.
    list-new                                \ lst1 lst2
      4 13 region-new over region-list-push \ lst1 lst2
      0  0 region-new over region-list-push \ lst1 lst2

    \ Subtract regions.
    2dup region-list-subtract-n               \ lst1 lst2 lst3

    \ Check results.

    dup list-get-length 2 <>
    abort" list length invalid"

    0 4 region-new over             \ lst1 lst2 lst3 reg lst3
    over swap                       \ lst1 lst2 lst3 reg reg lst3
    region-list-member              \ lst1 lst2 lst3 reg flag
    0= abort" 0x00 not found"
                                    \ lst1 lst2 lst3 reg
    region-deallocate               \ lst1 lst2 lst3

    12 4 region-new over            \ lst1 lst2 lst3 reg lst3
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

: region-list-tests
    region-list-test-region-intersections-n
    region-list-test-subtract-n
;

