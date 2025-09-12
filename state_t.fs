: state-test-not-a-or-not-b
    4 5 state-not-a-or-not-b        \ list
    cr ." ~4 + ~5: " dup .region-list cr

    3 6 state-not-a-or-not-b        \ list45 list36
    cr ." ~3 + ~6: " dup .region-list cr

    2dup region-list-region-intersections   \ list45 list36 ints
    dup cr ." Possible regions = (~4 + ~5) & (~3 + ~6) = " .region-list

    \ Deallocate remaining struct instances.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." state-test-not-a-or-not-b - Ok" cr
;

: state-tests
    state-test-not-a-or-not-b
;
