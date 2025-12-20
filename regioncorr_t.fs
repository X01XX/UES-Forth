

: regioncorr-test-superset

    \ Init lists.
    s" (0XX1 0XX0X)" regioncorr-from-string-a \ rlc1
    s" (0XXX 0XXXX)" regioncorr-from-string-a \ rlc1 rlc2

    2dup regioncorr-superset      \ rlc1 rlc2 flag
    0= abort" List is not superset?"

    \ Try the reverse.
    swap                                \ rlc2 rlc1
    2dup regioncorr-superset      \ rlc2 rlc1 flag
    abort" List is superset?"

    \ Clean up.
    regioncorr-deallocate              \ rlc2
    regioncorr-deallocate              \

    cr ." regioncorr-test-superset - Ok" cr
;

: regioncorr-test-intersects

    \ Init lists.
    s" (0XX1 0XX0X)" regioncorr-from-string-a \ rlc1
    s" (0XXX 0XXXX)" regioncorr-from-string-a \ rlc1 rlc2

    \ Test intersection.
    2dup regioncorr-intersects    \ rlc1 rlc2 bool
    is-false abort" Lists do not intersect?"

    regioncorr-deallocate              \ rlc1

    \ Test non-intersection.
    s" (1XXX 0XXXX)" regioncorr-from-string-a \ rlc1 lst3

    \ cr ." lst3: " dup .regioncorr cr

    2dup regioncorr-intersects    \ rlc1 lst3 bool
    abort" Lists do intersect?"

    \ Clean up.
    regioncorr-deallocate              \ rlc1
    regioncorr-deallocate              \

    cr ." regioncorr-test-intersects - Ok" cr
;

: regioncorr-test-subtract
    \ Init lists.
    s" (X1X1 0X10X)" regioncorr-from-string-a \ rlc1
    s" (0X0X 0XXX1)" regioncorr-from-string-a \ rlc1 rlc2

    cr dup .regioncorr space ." - " over .regioncorr
    \ Test subtraction.
    2dup regioncorr-subtract      \ rlc1 rlc2, rlc-lst t | f

    if                                  \ rlc1 rlc2 rlc-lst
        \ Check list length.
        dup list-get-length #4 <> abort" list length is not 4?"
        [ ' .regioncorr ] literal over list-apply

        \ Check results 1.
        s" (0x0x 0X0X1)" regioncorr-from-string-a \ rlc1 rlc2 rlc-lst lst-t
        [ ' regioncorr-eq ] literal               \ rlc1 rlc2 rlc-lst lst-t xt
        over #3 pick list-member                        \ rlc1 rlc2 rlc-lst lst-t bool
        is-false abort" regioncorr not found in list of lists?"
        regioncorr-deallocate                          \ rlc1 rlc2 rlc-lst

        \ Check results 2.
        s" (0x0x 0XX11)" regioncorr-from-string-a \ rlc1 rlc2 rlc-lst lst-t
        [ ' regioncorr-eq ] literal               \ rlc1 rlc2 rlc-lst lst-t xt
        over #3 pick list-member                        \ rlc1 rlc2 rlc-lst lst-t bool
        is-false abort" regioncorr not found in list of lists?"
        regioncorr-deallocate                          \ rlc1 rlc2 rlc-lst

        \ Check results 3.
        s" (0X00 0xxx1)" regioncorr-from-string-a \ rlc1 rlc2 rlc-lst lst-t
        [ ' regioncorr-eq ] literal               \ rlc1 rlc2 rlc-lst lst-t xt
        over #3 pick list-member                        \ rlc1 rlc2 rlc-lst lst-t bool
        is-false abort" regioncorr not found in list of lists?"
        regioncorr-deallocate                          \ rlc1 rlc2 rlc-lst

        \ Check results 4.
        s" (000X 0xxx1)" regioncorr-from-string-a \ rlc1 rlc2 rlc-lst lst-t
        [ ' regioncorr-eq ] literal               \ rlc1 rlc2 rlc-lst lst-t xt
        over #3 pick list-member                        \ rlc1 rlc2 rlc-lst lst-t bool
        is-false abort" regioncorr not found in list of lists?"
        regioncorr-deallocate                          \ rlc1 rlc2 rlc-lst

        \ Clean up list-of-lists.
        regioncorr-list-deallocate                             \ rlc1 rlc2
    else
        cr ." Subtract failed?"
        abort
    then

    \ Clean up.                     \ rlc1 rlc2
    regioncorr-deallocate          \ rlc1
    regioncorr-deallocate          \

    cr ." regioncorr-test-subtract - Ok" cr
;

: regioncorr-test-complement
    \ Init list 1.
    s" (X1X1 XX10X)" regioncorr-from-string-a \ rlc

    cr ." rlc: " dup .regioncorr

    dup                                 \ rlc rlc
    regioncorr-complement         \ rlc rlc-lst

    cr ." complement: "
    [ ' .regioncorr ] literal over list-apply cr

    \ Check list length.
    dup list-get-length #4 <> abort" list length is not 4?"
    \ [ ' .regioncorr ] literal over list-apply

    \ Check results 1.
    s" (xxxx XX0XX)" regioncorr-from-string-a \ rlc rlc-lst rlc-t
    [ ' regioncorr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" regioncorr not found in list of lists?"
    regioncorr-deallocate                          \ rlc rlc-lst

    \ Check results 2.
    s" (xxxx XXX1X)" regioncorr-from-string-a \ rlc rlc-lst rlc-t
    [ ' regioncorr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" regioncorr not found in list of lists?"
    regioncorr-deallocate                          \ rlc rlc-lst

    \ Check results 3.
    s" (XXX0 xxxxx)" regioncorr-from-string-a \ rlc rlc-lst rlc-t
    [ ' regioncorr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" regioncorr not found in list of lists?"
    regioncorr-deallocate                          \ rlc rlc-lst

    \ Check results 4.
    s" (X0XX xxxxx)" regioncorr-from-string-a \ rlc rlc-lst rlc-t
    [ ' regioncorr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" regioncorr not found in list of lists?"
    regioncorr-deallocate                          \ rlc rlc-lst

    \ Clean up list-of-lists.
    regioncorr-list-deallocate                             \ rlc

    regioncorr-deallocate                          \

    cr ." regioncorr-test-complement - Ok" cr
;

: regioncorr-test-distance
    \ Init lists.
    s" (X1X1 0X10X)" regioncorr-from-string-a \ rlc1
    s" (1001 0XX11)" regioncorr-from-string-a \ rlc1 rlc2

    2dup regioncorr-distance      \ rlc1 rlc2 dist
    #2 <> abort" Distance not 2?"

    regioncorr-deallocate              \ rlc1
    regioncorr-deallocate              \

    cr ." regioncorr-test-distance - Ok" cr
;

\ Assume domain0 is 4-bit, Domain 1 is 5-bit.
: regioncorr-tests
    regioncorr-test-superset
    regioncorr-test-intersects
    regioncorr-test-subtract
    regioncorr-test-complement
    regioncorr-test-distance
;

