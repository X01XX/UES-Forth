
: pathstep-list-test-eq-initial-subset-result?
    cr ." pathstep-list-test-eq-initial-subset-result?: "

    session-new                                 \ sess

    \ Init domain 0.
    #4 over domain-new                          \ sess dom0
    over                                        \ sess dom0 sess
    session-add-domain                          \ sess

    s" (XX/X0/00/00/)" pathstep-from-string-a   \ sess pthstp1'
    \ cr ." pathstep 1: " dup .pathstep cr

    s" (X1/X0/00/00/)" pathstep-from-string-a   \ sess pthstp1' pthstp2
    \ cr ." pathstep 2: " dup .pathstep cr

   \ Test superset.
    list-new                                    \ sess pthstp1' pthstp2 pthstp-lst'
    tuck pathstep-list-push                     \ sess pthstp1' pthstp-lst'
    2dup                                        \ sess pthstp1' pthstp-lst' pthstp1' pthstp-lst'
    pathstep-list-any-eq-initial-subset-result? \ sess pthstp1' pthstp-lst' bool
    \ cr ." result 1: " dup .bool cr
    false? abort" should be true"

    pathstep-list-deallocate                    \ sess pthstp1'
    pathstep-deallocate                         \ sess

    session-deallocate

    ." Ok" cr
;

: pathstep-list-test-next-steps
    cr ." pathstep-list-test-next-steps: start "

    session-new                                 \ sess

    \ Init domain 0.
    #4 over domain-new                          \ sess dom0
    over                                        \ sess dom0 sess
    session-add-domain                          \ sess

    \ Init pathstep list.
    list-new                                    \ sess pthstp-lst'

    \ Add pathsteps.
    s" (00/11/00/01/)" pathstep-from-string-a   \ sess pthstp-lst' pthstp
    over pathstep-list-push                     \ sess pthstp-lst'

    s" (00/00/00/01/)" pathstep-from-string-a   \ sess pthstp-lst' pthstp
    over pathstep-list-push                     \ sess pthstp-lst'

    s" (00/00/01/11/)" pathstep-from-string-a   \ sess pthstp-lst' pthstp
    over pathstep-list-push                     \ sess pthstp-lst'

    \ Make to/from regioncorrs.
    s" (0111)" regioncorr-from-string-a swap    \ sess regc-to' pthstp-lst'
    s" (0100)" regioncorr-from-string-a swap    \ sess regc-to' regc-from' pthstp-lst'

    \ Check direct steps.
    #2 pick #2 pick #2 pick                     \ sess regc-to' regc-from' pthstp-lst' regc-to' regc-from' pthstp-lst'
    pathstep-list-next-steps-direct             \ sess regc-to' regc-from' pthstp-lst' pthstp-lst2'
    \ cr ." direct returns: " dup .pathstep-list cr

    \ Check result.
    dup list-get-length 1 <> abort" result list length ne 1?"

    \ Check first pathstep in list.
    dup list-get-first-item                     \ sess pthstp-lst2' item0
    dup pathstep-get-initial-regions            \ sess pthstp-lst2' item0 initial0
    s" (0100)" regioncorr-from-string-a         \ sess pthstp-lst2' item0 initial0 regcorr4'
    2dup regioncorr-eq?                         \ sess pthstp-lst2' item0 initial0 regcorr4' bool
    if
        \ Initial regions is ( 0100 )
        regioncorr-deallocate                   \ sess pthstp-lst2' item0 initial0
        drop                                    \ sess pthstp-lst2' item0
        pathstep-get-number-unwanted-changes    \ sess pthstp-lst2' num-unw
        0<> abort" invalid number unwanted changes for 0100->0101?"
    else
        cr ." invalid pathstep in list?" cr
        abort
    then

    pathstep-list-deallocate                    \ sess regc-to' regc-from' pthstp-lst'

    \ Check indirect steps.
    #2 pick #2 pick #2 pick                     \ sess regc-to' regc-from' pthstp-lst' regc-to' regc-from' pthstp-lst'
    pathstep-list-next-steps-indirect           \ sess regc-to' regc-from' pthstp-lst' pthstp-lst2' pthstp-lst3'
    \ cr ." indirect returns: " dup .pathstep-list cr

    \ Check result.
    dup list-get-length 1 <> abort" result list length ne 1?"

    \ Check first pathstep in list.
    dup list-get-first-item                     \ sess pthstp-lst2' item0
    dup pathstep-get-initial-regions            \ sess pthstp-lst2' item0 initial0
    s" (0000)" regioncorr-from-string-a         \ sess pthstp-lst2' item0 initial0 regcorr4'
    2dup regioncorr-eq?                         \ sess pthstp-lst2' item0 initial0 regcorr4' bool
    if
        \ Initial regions is ( 0100 )
        regioncorr-deallocate                   \ sess pthstp-lst2' item0 initial0
        drop                                    \ sess pthstp-lst2' item0
        pathstep-get-number-unwanted-changes    \ sess pthstp-lst2' num-unw
        1 <> abort" invalid number unwanted changes for 0000->0001?"
    else
        cr ." invalid pathstep in list?" cr
        abort
    then

    pathstep-list-deallocate                    \ sess regc-to' regc-from' pthstp-lst'

    \ Clean up.
    pathstep-list-deallocate                    \ sess regc-to' regc-from'
    regioncorr-deallocate                       \ sess regc-to'
    regioncorr-deallocate                       \ sess

    session-deallocate

    ." - Ok" cr
;

: pathstep-list-test-previous-steps
    cr ." pathstep-list-test-previous-steps: start "

    session-new                                 \ sess

    \ Init domain 0.
    #4 over domain-new                          \ sess dom0
    over                                        \ sess dom0 sess
    session-add-domain                          \ sess

    \ Init pathstep list.
    list-new                                    \ sess pthstp-lst'

    \ Add pathsteps.
    s" (00/11/01/11/)" pathstep-from-string-a   \ sess pthstp-lst' pthstp
    over pathstep-list-push                     \ sess pthstp-lst'

    s" (00/00/00/01/)" pathstep-from-string-a   \ sess pthstp-lst' pthstp
    over pathstep-list-push                     \ sess pthstp-lst'

    s" (00/00/01/11/)" pathstep-from-string-a   \ sess pthstp-lst' pthstp
    over pathstep-list-push                     \ sess pthstp-lst'

    \ Make to/from regioncorrs.
    s" (0111)" regioncorr-from-string-a swap    \ sess regc-to' pthstp-lst'
    s" (0100)" regioncorr-from-string-a swap    \ sess regc-to' regc-from' pthstp-lst'

    #2 pick #2 pick #2 pick                     \ sess regc-to' regc-from' pthstp-lst' regc-to' regc-from' pthstp-lst'
    pathstep-list-previous-steps-direct         \ sess regc-to' regc-from' pthstp-lst' pthstp-lst2'
    \ cr ." direct returns: " dup .pathstep-list cr

    \ Check result.
    dup list-get-length 1 <> abort" result list length ne 1?"

    \ Check first pathstep in list.
    dup list-get-first-item                     \ sess pthstp-lst2' item0
    dup pathstep-get-result-regions             \ sess pthstp-lst2' item0 initial0
    s" (0111)" regioncorr-from-string-a         \ sess pthstp-lst2' item0 initial0 regcorr4'
    2dup regioncorr-eq?                         \ sess pthstp-lst2' item0 initial0 regcorr4' bool
    if
        \ Initial regions is ( 0100 )
        regioncorr-deallocate                   \ sess pthstp-lst2' item0 initial0
        drop                                    \ sess pthstp-lst2' item0
        pathstep-get-number-unwanted-changes    \ sess pthstp-lst2' num-unw
        0<> abort" invalid number unwanted changes for 0100->0101?"
    else
        cr ." invalid pathstep in list?" cr
        abort
    then

    pathstep-list-deallocate                    \ sess regc-to' regc-from' pthstp-lst'

    #2 pick #2 pick #2 pick                     \ sess regc-to' regc-from' pthstp-lst' regc-to' regc-from' pthstp-lst'
    pathstep-list-previous-steps-indirect       \ sess regc-to' regc-from' pthstp-lst' pthstp-lst3'
    \ cr ." indirect returns: " dup .pathstep-list cr

    \ Check result.
    dup list-get-length 1 <> abort" result list length ne 1?"

    \ Check first pathstep in list.
    dup list-get-first-item                     \ sess pthstp-lst2' item0
    dup pathstep-get-result-regions             \ sess pthstp-lst2' item0 initial0
    s" (0011)" regioncorr-from-string-a         \ sess pthstp-lst2' item0 initial0 regcorr4'
    2dup regioncorr-eq?                         \ sess pthstp-lst2' item0 initial0 regcorr4' bool
    if
        \ Initial regions is ( 0100 )
        regioncorr-deallocate                   \ sess pthstp-lst2' item0 initial0
        drop                                    \ sess pthstp-lst2' item0
        pathstep-get-number-unwanted-changes    \ sess pthstp-lst2' num-unw
        1 <> abort" invalid number unwanted changes for 0000->0001?"
    else
        cr ." invalid pathstep in list?" cr
        abort
    then

    pathstep-list-deallocate                    \ sess regc-to' regc-from' pthstp-lst'

    \ Clean up.
    pathstep-list-deallocate                    \ sess regc-to' regc-from'
    regioncorr-deallocate                       \ sess regc-to'
    regioncorr-deallocate                       \ sess

    session-deallocate

    ." - Ok" cr
;

: pathsteplist-tests

    pathstep-list-test-eq-initial-subset-result?
    pathstep-list-test-next-steps
    pathstep-list-test-previous-steps
;
