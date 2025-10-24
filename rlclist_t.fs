

: rlc-list-test-any-superset
    list-new                    \ rlc-lst
    s" (xx10 XX0XX)" region-list-corr-from-string-a
    over rlc-list-push          \ rlc-lst

    s" (xxx1 XX0XX)" region-list-corr-from-string-a
    swap                        \ rlc rlc-lst

    2dup rlc-list-any-superset  \ rlc rlc-lst bool
    abort" Superset found?"

    s" (xxx1 XX0XX)" region-list-corr-from-string-a
    over rlc-list-push          \ rlc rlc-lst

    2dup rlc-list-any-superset  \ rlc rlc-lst bool
    is-false abort" Superset not found?"

                                \ rlc rlc-lst
    rlc-list-deallocate         \ rlc
    region-list-deallocate      \

    cr ." rlc-list-test-any-superset - Ok" cr
;

: rlc-list-test-remove-subsets
    s" (xx1x XX0XX)" region-list-corr-from-string-a \ rlc

    list-new                    \ rlc rlc-lst
    s" (xx10 XX0XX)" region-list-corr-from-string-a
    over rlc-list-push          \ rlc rlc-lst

    s" (xxx1 XX0XX)" region-list-corr-from-string-a
    over rlc-list-push          \ rlc rlc-lst

    s" (x111 XX0XX)" region-list-corr-from-string-a
    over rlc-list-push          \ rlc rlc-lst

    cr ." before: " dup .rlc-list
    2dup rlc-list-remove-subsets    \ rlc rlc-lst
    cr ." after:  " dup .rlc-list

    \ Check results.
    dup list-get-length         \ rlc rlc-lst len
    1 <> abort" Result length not 1?"

    \ Check results 1.
    s" (XXX1 xx0xx)" region-list-corr-from-string-a \ rlc rlc-lst rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" region-list-corr not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst

                                \ rlc rlc-lst
    rlc-list-deallocate         \ rlc
    region-list-deallocate      \

    cr ." rlc-list-test-remove-subsets - Ok" cr
;

: rlc-list-test-push-nosubs
    s" (xx1x XX0XX)" region-list-corr-from-string-a \ rlc

    list-new                                        \ rlc rlc-lst
    s" (xx10 XX0XX)" region-list-corr-from-string-a
    over rlc-list-push                              \ rlc rlc-lst

    s" (xxx1 XX0XX)" region-list-corr-from-string-a
    over rlc-list-push                              \ rlc rlc-lst

    s" (x111 XX0XX)" region-list-corr-from-string-a
    over rlc-list-push                              \ rlc rlc-lst

    cr ." before: " dup .rlc-list
    2dup rlc-list-push-nosubs                       \ rlc rlc-lst bool
    is-false abort" push-nosubs failed?"
    cr ." after:  " dup .rlc-list

    \ Check results.
    dup list-get-length                             \ rlc rlc-lst len
    2 <> abort" Result length not 2?"

    \ Check results 1.
    s" (XXX1 xx0xx)" region-list-corr-from-string-a \ rlc rlc-lst rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" region-list-corr not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst

    \ Check results 2.
    swap                                            \ rlc-lst rlc
    [ ' region-list-corr-eq ] literal               \ rlc-lst rlc xt
    swap                                            \ rlc-lst xt rlc
    #2 pick list-member                             \ rlc-lst bool
    is-false abort" region-list-corr not found in rlc-list?"

                                                    \ rlc-lst
    rlc-list-deallocate                             \

    cr ." rlc-list-test-push-nosubs - Ok" cr
;

: rlc-list-test-copy-nosubs
    \ Init rcl list.
    cr ." at 1 " .s cr
    list-new                                        \ rcl-lst

    \ Add first rlc.
    s" (000X 000XX)" region-list-corr-from-string-a \ rlc-lst rclx
    over rlc-list-push                              \ rlc-lst

    \ Add subset rlc.
    s" (0001 00010)" region-list-corr-from-string-a \ rlc-lst rclx
    over rlc-list-push                              \ rlc-lst

    \ Add first duplicate rlc.
    s" (000X 000XX)" region-list-corr-from-string-a \ rlc-lst rclx
    over rlc-list-push                              \ rlc-lst
    cr ." before: " dup .rlc-list

    dup                                             \ rlc-lst rlc-lst
    rlc-list-copy-nosubs                            \ rlc-lst rlc-lst'
    cr ." after:  " dup .rlc-list

    \ Check result length.
    dup list-get-length                             \ rlc rlc-lst len
    1 <> abort" Result length not 1?"

    \ Check results 1.
    s" (000X 000XX)" region-list-corr-from-string-a \ rlc-lst rlc-lst' rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc-lst rlc-lst' rlc-t xt
    over #3 pick list-member                        \ rlc-lst rlc-lst' rlc-t bool
    is-false abort" region-list-corr not found in rlc-list?"

    region-list-deallocate                          \ rlc-lst rlc-lst'

    \ Clean up.
    swap
    rlc-list-deallocate                             \ rlc-lst
    rlc-list-deallocate                             \

    cr ." rlc-list-test-copy-nosubs - Ok" cr
;

: rlc-tests
    rlc-list-test-any-superset
    rlc-list-test-remove-subsets
    rlc-list-test-push-nosubs
    rlc-list-test-copy-nosubs

    cr ." rlc tests - Ok" cr
;
