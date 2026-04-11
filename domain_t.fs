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
    false? abort" domain-test-state-pair-complement: lists ne?"

    \ Deallocate remaining struct instances.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    drop

    cr ." domain-test-state-pair-complement: Ok" cr
;

: domain-test-asymmetric-chaining
    session-new                     \ sess

    \ Init domain 0.
    #4 over domain-new              \ sess dom
    2dup                            \ sess dom sess dom
    swap                            \ sess dom dom sess
    session-add-domain              \ sess dom

    0  over domain-find-action      \ sess dom act0 t | f
    false? abort" act0 not found?"

    \ Add rule for 0->1.
    #1 #0 sample-new                \ sess dom act smpl'
    dup #2 pick action-add-sample   \ sess dom act smpl'
    sample-deallocate               \ sess dom act

    \ Add rule for 1->3.
    #3 #1 sample-new                \ sess dom act smpl'
    dup #2 pick action-add-sample   \ sess dom act smpl'
    sample-deallocate               \ sess dom act

    \ Add rule for 3->7.
    #7 #3 sample-new                \ sess dom act smpl'
    dup #2 pick action-add-sample   \ sess dom act smpl'
    sample-deallocate               \ sess dom act

    \ cr dup .action cr

    \ Create reg-to.
    #7 #7 region-new                \ sess dom act reg-to'

    \ Create reg-from.
    0 0 region-new                  \ sess dom act reg-to 'reg-from'

    2dup                            \ sess dom act reg-to 'reg-from' reg-to 'reg-from'
    #5 pick                         \ sess dom act reg-to 'reg-from' reg-to 'reg-from' dom
    domain-asymmetric-chaining      \ sess dom act reg-to 'reg-from', plan' t | f
    if
        \ cr ." plan found: " dup .plan cr
        dup plan-get-length         \ sess dom act reg-to 'reg-from' plan' len
        #3 <> abort" invalid plan length?"

        dup plan-get-initial-region \ sess dom act reg-to 'reg-from' plan' reg-init
        #2 pick                     \ sess dom act reg-to 'reg-from' plan' reg-init reg-from
        region-eq?                  \ sess dom act reg-to 'reg-from' plan' bool
        false? abort" Invalid initial region?"

        dup plan-get-result-region  \ sess dom act reg-to 'reg-from' plan' reg-rslt
        #3 pick                     \ sess dom act reg-to 'reg-from' plan' reg-rslt reg-to
        region-eq?                  \ sess dom act reg-to 'reg-from' plan' bool
        false? abort" Invalid result region?"

        plan-deallocate
    else
        cr ." no plan found" cr
        abort
    then

    \ Clean up.
    region-deallocate
    region-deallocate
    2drop
    session-deallocate

    cr ." domain-test-asymmetric-chaining: Ok" cr
;

: domain-tests
    session-new                     \ sess

    \ Init domain 0.
    #4 over domain-new              \ sess dom0
    2dup                            \ sess dom0 sess dom0
    swap                            \ sess dom0 dom0 sess
    session-add-domain              \ sess dom0

    dup domain-test-state-complement
    dup domain-test-state-pair-complement
    drop

    session-deallocate

    domain-test-asymmetric-chaining
;
