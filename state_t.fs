: state-test-not-a-or-not-b
    #4 #5 state-not-a-or-not-b        \ list
    cr ." ~4 + ~5: " dup .region-list cr

    #3 #6 state-not-a-or-not-b        \ list45 list36
    cr ." ~3 + ~6: " dup .region-list cr

    2dup region-list-intersections-nosubs   \ list45 list36 ints
    dup cr ." Possible regions = (~4 + ~5) & (~3 + ~6) = " .region-list

    s" (1XXX X11X XXX0 XXX1 X0XX)"
    region-list-from-string-a       \ list57' list5D' list-int' list-test
    2dup region-list-eq             \ list57' list5D' list-int' list-test bool
    is-false abort" state-test-not-a-or-not-b: lists ne?"

    \ Deallocate remaining struct instances.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." state-test-not-a-or-not-b: Ok" cr
;

: state-test-complement \ Test ~5.
    #5 state-complement         \ reg-lst'
    cr ." ~5 = " dup .region-list cr

    s" (1XXX XX1X X0XX XXX0)"
    region-list-from-string-a   \ reg-lst' test-lst'
    2dup region-list-eq         \ reg-lst' test-lst' bool
    if
    else
        cr ." region lists ne?" cr
        abort
    then

    region-list-deallocate
    region-list-deallocate

     cr ." state-test-complement: Ok" cr
;

: state-test-boolean-algebra \ Test (~5 + ~7) & (~5 + ~D) = ~5 + (~7 & ~D)

    \ Calc (~5 + ~7) & (~5 + ~D)
    #7 #5 state-not-a-or-not-b        \ list57'
    cr ." ~5 + ~7: " dup .region-list cr

    $D #5 state-not-a-or-not-b        \ list57' list5D'
    cr ." ~5 + ~D: " dup .region-list cr

    2dup region-list-intersections-nosubs   \ list57' list5D' rslt1'
    cr ." (~5 + ~7) & (~5 & ~D): " dup .region-list cr

    \ Clean up
    swap region-list-deallocate
    swap region-list-deallocate

    \ Calc ~5 + (~7 & ~D)
    #5 state-complement                     \ rslt1' ~5
    #7 state-complement                     \ rslt1' ~5 ~7
    $D state-complement                     \ rslt1' ~5 ~7 ~D
    2dup region-list-intersections-nosubs   \ rslt1' ~5 ~7 ~D (~7 & ~D)

    \ Clean up
    swap region-list-deallocate
    swap region-list-deallocate             \ rlst1' ~5 (~7 & ~D)

    2dup region-list-union-nosubs           \ rlst1' ~5 (~7 & ~D) rlst2'
    cr ." ~5 + (~7 & ~D): " dup .region-list cr

    \ Clean up
    swap region-list-deallocate
    swap region-list-deallocate             \ rlst1' rslt2'

    \ Test eq
    2dup region-list-eq                     \ rlst1' rslt2' bool
    is-false abort" state-test-boolean-algebra: lists ne?"

    region-list-deallocate
    region-list-deallocate

    cr ." state-test-boolean-algebra: Ok" cr
;

: state-tests
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    swap                                            \ dom0 sess
    session-add-domain                              \

    state-test-complement
    state-test-not-a-or-not-b
    state-test-boolean-algebra

    current-session-deallocate
;
