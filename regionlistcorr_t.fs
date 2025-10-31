

: region-list-corr-test-superset

    \ Init lists.
    s" (0XX1 0XX0X)" region-list-corr-from-string-a \ rlc1
    s" (0XXX 0XXXX)" region-list-corr-from-string-a \ rlc1 rlc2

    2dup region-list-corr-superset      \ rlc1 rlc2 flag
    0= abort" List is not superset?"

    \ Try the reverse.
    swap                                \ rlc2 rlc1
    2dup region-list-corr-superset      \ rlc2 rlc1 flag
    abort" List is superset?"

    \ Clean up.
    region-list-deallocate              \ rlc2
    region-list-deallocate              \

    cr ." region-list-corr-test-superset - Ok" cr
;

: region-list-corr-test-intersects

    \ Init lists.
    s" (0XX1 0XX0X)" region-list-corr-from-string-a \ rlc1
    s" (0XXX 0XXXX)" region-list-corr-from-string-a \ rlc1 rlc2

    \ Test intersection.
    2dup region-list-corr-intersects    \ rlc1 rlc2 bool
    is-false abort" Lists do not intersect?"

    region-list-deallocate              \ rlc1

    \ Test non-intersection.
    s" (1XXX 0XXXX)" region-list-corr-from-string-a \ rlc1 lst3

    \ cr ." lst3: " dup .region-list-corr cr

    2dup region-list-corr-intersects    \ rlc1 lst3 bool
    abort" Lists do intersect?"

    \ Clean up.
    region-list-deallocate              \ rlc1
    region-list-deallocate              \

    cr ." region-list-corr-test-intersects - Ok" cr
;

: region-list-corr-test-subtract
    \ Init lists.
    s" (X1X1 0X10X)" region-list-corr-from-string-a \ rlc1
    s" (0X0X 0XXX1)" region-list-corr-from-string-a \ rlc1 rlc2

    cr dup .region-list-corr space ." - " over .region-list-corr
    \ Test subtraction.
    2dup region-list-corr-subtract      \ rlc1 rlc2, rlc-lst t | f

    if                                  \ rlc1 rlc2 rlc-lst
        \ Check list length.
        dup list-get-length #4 <> abort" list length is not 4?"
        [ ' .region-list-corr ] literal over list-apply

        \ Check results 1.
        s" (0x0x 0X0X1)" region-list-corr-from-string-a \ rlc1 rlc2 rlc-lst lst-t
        [ ' region-list-corr-eq ] literal               \ rlc1 rlc2 rlc-lst lst-t xt
        over #3 pick list-member                        \ rlc1 rlc2 rlc-lst lst-t bool
        is-false abort" region-list-corr not found in list of lists?"
        region-list-deallocate                          \ rlc1 rlc2 rlc-lst

        \ Check results 2.
        s" (0x0x 0XX11)" region-list-corr-from-string-a \ rlc1 rlc2 rlc-lst lst-t
        [ ' region-list-corr-eq ] literal               \ rlc1 rlc2 rlc-lst lst-t xt
        over #3 pick list-member                        \ rlc1 rlc2 rlc-lst lst-t bool
        is-false abort" region-list-corr not found in list of lists?"
        region-list-deallocate                          \ rlc1 rlc2 rlc-lst

        \ Check results 3.
        s" (0X00 0xxx1)" region-list-corr-from-string-a \ rlc1 rlc2 rlc-lst lst-t
        [ ' region-list-corr-eq ] literal               \ rlc1 rlc2 rlc-lst lst-t xt
        over #3 pick list-member                        \ rlc1 rlc2 rlc-lst lst-t bool
        is-false abort" region-list-corr not found in list of lists?"
        region-list-deallocate                          \ rlc1 rlc2 rlc-lst

        \ Check results 4.
        s" (000X 0xxx1)" region-list-corr-from-string-a \ rlc1 rlc2 rlc-lst lst-t
        [ ' region-list-corr-eq ] literal               \ rlc1 rlc2 rlc-lst lst-t xt
        over #3 pick list-member                        \ rlc1 rlc2 rlc-lst lst-t bool
        is-false abort" region-list-corr not found in list of lists?"
        region-list-deallocate                          \ rlc1 rlc2 rlc-lst

        \ Clean up list-of-lists.
        rlc-list-deallocate                             \ rlc1 rlc2
    else
        cr ." Subtract failed?"
        abort
    then

    \ Clean up.                     \ rlc1 rlc2
    region-list-deallocate          \ rlc1
    region-list-deallocate          \

    cr ." region-list-corr-test-subtract - Ok" cr
;

: region-list-corr-test-complement
    \ Init list 1.
    s" (X1X1 XX10X)" region-list-corr-from-string-a \ rlc

    cr ." rlc: " dup .region-list-corr

    dup                                 \ rlc rlc
    region-list-corr-complement         \ rlc rlc-lst

    cr ." complement: "
    [ ' .region-list-corr ] literal over list-apply cr

    \ Check list length.
    dup list-get-length #4 <> abort" list length is not 4?"
    \ [ ' .region-list-corr ] literal over list-apply

    \ Check results 1.
    s" (xxxx XX0XX)" region-list-corr-from-string-a \ rlc rlc-lst rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" region-list-corr not found in list of lists?"
    region-list-deallocate                          \ rlc rlc-lst

    \ Check results 2.
    s" (xxxx XXX1X)" region-list-corr-from-string-a \ rlc rlc-lst rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" region-list-corr not found in list of lists?"
    region-list-deallocate                          \ rlc rlc-lst

    \ Check results 3.
    s" (XXX0 xxxxx)" region-list-corr-from-string-a \ rlc rlc-lst rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" region-list-corr not found in list of lists?"
    region-list-deallocate                          \ rlc rlc-lst

    \ Check results 4.
    s" (X0XX xxxxx)" region-list-corr-from-string-a \ rlc rlc-lst rlc-t
    [ ' region-list-corr-eq ] literal               \ rlc rlc-lst rlc-t xt
    over #3 pick list-member                        \ rlc rlc-lst rlc-t bool
    is-false abort" region-list-corr not found in list of lists?"
    region-list-deallocate                          \ rlc rlc-lst

    \ Clean up list-of-lists.
    rlc-list-deallocate                             \ rlc

    region-list-deallocate                          \

    cr ." region-list-corr-test-complement - Ok" cr
;

: region-list-corr-test-distance
    \ Init lists.
    s" (X1X1 0X10X)" region-list-corr-from-string-a \ rlc1
    s" (1001 0XX11)" region-list-corr-from-string-a \ rlc1 rlc2

    2dup region-list-corr-distance      \ rlc1 rlc2 dist
    #2 <> abort" Distance not 2?"

    region-list-deallocate              \ rlc1
    region-list-deallocate              \

    cr ." region-list-corr-test-distance - Ok" cr
;

\ Assume domain0 is 4-bit, Domain 1 is 5-bit.
: region-list-corr-tests
    region-list-corr-test-superset
    region-list-corr-test-intersects
    region-list-corr-test-subtract
    region-list-corr-test-complement
    region-list-corr-test-distance
;

