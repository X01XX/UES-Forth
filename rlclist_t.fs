

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
    is-false abort" region-list-corr (XXX1 xx0xx) not found in rlc-list?"
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
    #2 <> abort" Result length not 2?"

    \ Check results 1.
    s" (XXX1 xx0xx)" region-list-corr-from-string-a \ rlc rlc-lst rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" region-list-corr (XXX1 xx0xx) not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst

    \ Check results 2.
    swap                                            \ rlc-lst rlc
    [ ' region-list-corr-eq ] literal               \ rlc-lst rlc xt
    swap                                            \ rlc-lst xt rlc
    #2 pick list-member                             \ rlc-lst bool
    is-false abort" region-list-corr (xx1x XX0XX) not found in rlc-list?"

                                                    \ rlc-lst
    rlc-list-deallocate                             \

    cr ." rlc-list-test-push-nosubs - Ok" cr
;

: rlc-list-test-copy-nosubs
    \ Init rcl list.
    \ cr ." at 1 " .s cr
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
    \ cr ." before: " dup .rlc-list

    dup                                             \ rlc-lst rlc-lst
    rlc-list-copy-nosubs                            \ rlc-lst rlc-lst'
    \ cr ." after:  " dup .rlc-list

    \ Check result length.
    dup list-get-length                             \ rlc rlc-lst len
    1 <> abort" Result length not 1?"

    \ Check results 1.
    s" (000X 000XX)" region-list-corr-from-string-a \ rlc-lst rlc-lst' rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc-lst rlc-lst' rlc-t xt
    over #3 pick list-member                        \ rlc-lst rlc-lst' rlc-t bool
    is-false abort" region-list-corr (000X 000XX) not found in rlc-list?"
    region-list-deallocate                          \ rlc-lst rlc-lst'

    \ Clean up.
    swap
    rlc-list-deallocate                             \ rlc-lst
    rlc-list-deallocate                             \

    cr ." rlc-list-test-copy-nosubs - Ok" cr
;

: rlc-list-test-subtract-rlc
    \ Run against an intersecting rlc, in an rlc-list.

    \ Init rlc to subtract.
    s" (000X 001XX)" region-list-corr-from-string-a \ rlc

    \ Init rlc-list to subtract from.
    list-new                                        \ rlc rlc-lst

    \ Add intersecting rlc.
    s" (X001 00X0X)" region-list-corr-from-string-a \ rlc rlc-lst rlcx
    over rlc-list-push                              \ rlc rlc-lst

    \ Do subtraction.
    2dup rlc-list-subtract-rlc                      \ rlc rlc-lst rlc-left

    \ Check result.
    \ dup .rlc-list
    dup list-get-length #2 <> abort" List length not 2?"

    \ Check result 1.
    s" (1001 00x0x)" region-list-corr-from-string-a \ rlc rlc-lst rlc-left rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-left rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-left rlc-t bool
    is-false abort" region-list-corr (1001 00x0x) not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst rlc-left

    \ Check result 2.
    s" (x001 0000X)" region-list-corr-from-string-a \ rlc rlc-lst rlc-left rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-left rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-left rlc-t bool
    is-false abort" region-list-corr (x001 0000X) not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst rlc-left

    \ Clean up.
    rlc-list-deallocate         \ rlc rlc-lst
    rlc-list-deallocate         \ rlc
    region-list-deallocate      \


    \ Run against a non-intersection rlc, in an rlc-list.

    \ Init rlc to subtract.
    s" (000X 001XX)" region-list-corr-from-string-a \ rlc

    \ Init rlc-list to subtract from.
    list-new                                        \ rlc rlc-lst

    \ Add non-intersecting rlc.
    s" (X011 00X0X)" region-list-corr-from-string-a \ rlc rlc-lst rlcx
    over rlc-list-push                              \ rlc rlc-lst

    \ Do subtraction.
    2dup rlc-list-subtract-rlc   \ rlc rlc-lst rlc-left

    \ Check result.
    \ dup .rlc-list
    dup list-get-length 1 <> abort" List length not 1?"

    \ Check result 1.
    s" (X011 00X0X)" region-list-corr-from-string-a \ rlc rlc-lst rlc-left rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-left rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-left rlc-t bool
    is-false abort" region-list-corr (X011 00X0X) not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst rlc-left

    \ Clean up.
    rlc-list-deallocate         \ rlc rlc-lst
    rlc-list-deallocate         \ rlc
    region-list-deallocate      \

    \ Run against a subset rlc, in an rlc-list.

    \ Init rlc to subtract.
    s" (000X 001XX)" region-list-corr-from-string-a \ rlc

    \ Init rlc-list to subtract from.
    list-new                                        \ rlc rlc-lst

    \ Add a subset rlc
    s" (0000 0010X)" region-list-corr-from-string-a \ rlc rlc-lst rlcx
    over rlc-list-push                              \ rlc rlc-lst

    \ Do subtraction.
    2dup rlc-list-subtract-rlc   \ rlc rlc-lst rlc-left

    \ Check result.
    \ dup .rlc-list
    dup list-get-length 0<> abort" List length not 0?"

    \ Clean up.
    rlc-list-deallocate         \ rlc rlc-lst
    rlc-list-deallocate         \ rlc
    region-list-deallocate      \

    \ Run against a all three options.

    \ Init rlc to subtract.
    s" (000X 001XX)" region-list-corr-from-string-a \ rlc

    \ Init rlc-list to subtract from.
    list-new                                        \ rlc rlc-lst

    \ Add intersecting rlc.
    s" (X001 00X0X)" region-list-corr-from-string-a \ rlc rlc-lst rlcx
    over rlc-list-push

    \ Add a subset rlc.
    s" (0000 0010X)" region-list-corr-from-string-a \ rlc rlc-lst rlcx
    over rlc-list-push                              \ rlc rlc-lst

    \ Add non-intersecting rlc.
    s" (X011 00X0X)" region-list-corr-from-string-a \ rlc rlc-lst rlcx
    over rlc-list-push                              \ rlc rlc-lst
    cr dup .rlc-list space ." - " over .region-list-corr

    \ Do subtraction.
    2dup rlc-list-subtract-rlc   \ rlc rlc-lst rlc-left
    space ." = " dup .rlc-list cr

    \ Check result.
    \ dup .rlc-list
    dup list-get-length #3 <> abort" List length not 0?"

    \ Check result 1.
    s" (1001 00x0x)" region-list-corr-from-string-a \ rlc rlc-lst rlc-left rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-left rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-left rlc-t bool
    is-false abort" region-list-corr (1001 00x0x) not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst rlc-left

    \ Check result 2.
    s" (x001 0000X)" region-list-corr-from-string-a \ rlc rlc-lst rlc-left rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-left rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-left rlc-t bool
    is-false abort" region-list-corr (x000 0000X) not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst rlc-left

    \ Check result 3.
    s" (x011 00x0x)" region-list-corr-from-string-a \ rlc rlc-lst rlc-left rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-left rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-left rlc-t bool
    is-false abort" region-list-corr (x011 00x0x) not found in rlc-list?"
    region-list-deallocate                          \ rlc rlc-lst rlc-left

    \ Clean up.
    rlc-list-deallocate         \ rlc rlc-lst
    rlc-list-deallocate         \ rlc
    region-list-deallocate      \

    cr ." rlc-list-test-subtract-rlc - Ok" cr
;

: rlc-list-test-complement
    \ Init rlc-list.
    list-new                                        \ rlc-lst

    \ Add rlc 1.
    s" (X10X X1X0X)" region-list-corr-from-string-a \ rlc-lst rlcx
    over rlc-list-push                              \ rlc-lst

    \ Add rlc 2.
    s" (X11X X1X1X)" region-list-corr-from-string-a \ rlc-lst rlcx
    over rlc-list-push                              \ rlc-lst

    \ Get complement.
    dup rlc-list-complement                         \ rlc-lst rlc-lst'

    \ Check results.
    \ cr ." comp1: " dup .rlc-list cr
    dup list-get-length #4 <> abort" comp1 len not 4?"

    dup rlc-list-complement                         \ rlc-lst rlc-lst' rlc-lst''
    \ cr ." comp2: " dup .rlc-list cr
    dup list-get-length #2 <> abort" comp1 len not 2?"

    #2 pick                                         \ rlc-lst rlc-lst' rlc-lst'' rlc-lst
    over                                            \ rlc-lst rlc-lst' rlc-lst'' rlc-lst rlc-lst''
    rlc-list-eq                                     \ rlc-lst rlc-lst' rlc-lst'' bool
    is-false abort" Initial and result rlc-lists not equal?"

    \ Clean up.
    rlc-list-deallocate         \ rlc-lst rlc-lst'
    rlc-list-deallocate         \ rlc-lst
    rlc-list-deallocate         \

    cr ." rlc-list-test-complement - Ok" cr
;

: rlc-list-test-normalize
    \ Init rlc-list.
    list-new                                        \ rlc-lst

    \ Add rlc 1.
    s" (X10X X1X0X)" region-list-corr-from-string-a \ rlc-lst rlcx
    over rlc-list-push                              \ rlc-lst

    \ Add rlc 2.
    s" (X11X X1X0X)" region-list-corr-from-string-a \ rlc-lst rlcx
    over rlc-list-push                              \ rlc-lst

    \ Normalize
    dup rlc-list-normalize                         \ rlc-lst rlc-lst'

    \ Check results.
    \ cr ." norm: " dup .rlc-list cr
    dup list-get-length 1 <> abort" normalized len not 1?"

    \ Check result 1.
    s" (X1XX X1X0X)" region-list-corr-from-string-a \ rlc-lst rlc-lst' rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc-lst rlc-lst' rlc-t xt
    over #3 pick list-member                        \ rlc-lst rlc-lst' rlc-t bool
    is-false abort" region-list-corr (X1XX X1X0X) not found in rlc-lst' ?"
    region-list-deallocate                          \ rlc-lst rlc-lst'

    \ Clean up.
    rlc-list-deallocate         \ rlc-lst
    rlc-list-deallocate         \

    cr ." rlc-list-test-normalize - Ok" cr
;

: rlc-list-test-intersection-fragments
\ Init rlc-list.
    list-new                                        \ rlc-lst

    \ Add rlc 1.
    s" (X1X1 XX1X1)" region-list-corr-from-string-a \ rlc-lst rlcx
    over rlc-list-push                              \ rlc-lst

    \ Add rlc 2.
    s" (1XX1 X1X1X)" region-list-corr-from-string-a \ rlc-lst rlcx
    over rlc-list-push                              \ rlc-lst

    \ Normalize
    dup rlc-list-intersection-fragments             \ rlc-lst rlc-lst'

    \ Check results.
    \ cr ." results: " dup .rlc-list cr

    dup list-get-length #7 <> abort" result len not 7?"

    \ Check result intersection.
    s" (11X1 X1111)" region-list-corr-from-string-a \ rlc-lst rlc-lst' rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc-lst rlc-lst' rlc-t xt
    over #3 pick list-member                        \ rlc-lst rlc-lst' rlc-t bool
    is-false abort" region-list-corr (11X1 X1111) not found in rlc-lst' ?"
    region-list-deallocate                          \ rlc-lst rlc-lst'

    \ Clean up.
    rlc-list-deallocate         \ rlc-lst
    rlc-list-deallocate         \

    cr ." rlc-list-test-intersection-fragments - Ok" cr
;

: rlc-list-tests
    rlc-list-test-any-superset
    rlc-list-test-remove-subsets
    rlc-list-test-push-nosubs
    rlc-list-test-copy-nosubs
    rlc-list-test-subtract-rlc
    rlc-list-test-complement
    rlc-list-test-normalize
    rlc-list-test-intersection-fragments

    cr ." rlc-list tests - Ok" cr
;
