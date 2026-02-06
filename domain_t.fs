: domain-test-state-complement ( dom0 -- )
    #5                          \ dom0 #5
    over                        \ dom0 #5 dom0
    domain-state-complement     \ dom0 reg-lst'
    cr ." ~5 = " dup .region-list cr

    s" (1XXX XX1X X0XX XXX0)"
    region-list-from-string-a   \ dom0 reg-lst' test-lst'
    2dup region-list-eq         \ dom0 reg-lst' test-lst' bool
    if
    else
        cr ." region lists ne?" cr
        abort
    then

    region-list-deallocate
    region-list-deallocate
    drop

     cr ." domain-test-state-complement: Ok" cr
;

: domain-test-state-pair-complement ( dom0 -- )
    #4 #5                                   \ dom0 #4 #5
    #2 pick                                 \ dom0 #4 #5 dom0
    domain-state-pair-complement            \ dom0 list45'
    cr ." ~4 + ~5: " dup .region-list cr

    #3 #6                                   \ dom0 list45' #3 #6
    #3 pick                                 \ dom0 list45' #3 #6 dom0
    domain-state-pair-complement            \ dom0 list45' list36'
    cr ." ~3 + ~6: " dup .region-list cr

    2dup region-list-intersections-nosubs   \ dom0 list45' list36' ints'
    dup cr ." Possible regions = (~4 + ~5) & (~3 + ~6) = " .region-list

    s" (1XXX X11X XXX0 XXX1 X0XX)"
    region-list-from-string-a               \ dom0 list45' list36' list-int' list-test'
    2dup region-list-eq                     \ dom0 list45' list36' list-int' list-test' bool
    is-false abort" domain-test-state-pair-complement: lists ne?"

    \ Deallocate remaining struct instances.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    drop

    cr ." domain-test-state-pair-complement: Ok" cr
;

: domain-tests
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    tuck                                            \ dom0 sess dom0
    swap                                            \ dom0 dom0 sess
    session-add-domain                              \ dom0

    dup domain-test-state-complement
    dup domain-test-state-pair-complement

    drop
    current-session-deallocate
;
