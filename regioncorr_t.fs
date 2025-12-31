

: regioncorr-test-superset

    \ Init lists.
    s" (0XX1 0XX0X)" regioncorr-from-string-a \ regc1
    s" (0XXX 0XXXX)" regioncorr-from-string-a \ regc1 regc2

    2dup regioncorr-superset        \ regc1 regc2 flag
    0= abort" List is not superset?"

    \ Try the reverse.
    swap                            \ regc2 regc1
    2dup regioncorr-superset        \ regc2 regc1 flag
    abort" List is superset?"

    \ Clean up.
    regioncorr-deallocate           \ regc2
    regioncorr-deallocate           \

    cr ." regioncorr-test-superset: Ok" cr
;

: regioncorr-test-intersects

    \ Init lists.
    s" (0XX1 0XX0X)" regioncorr-from-string-a \ regc1
    s" (0XXX 0XXXX)" regioncorr-from-string-a \ regc1 regc2

    \ Test intersection.
    2dup regioncorr-intersects    \ regc1 regc2 bool
    is-false abort" Lists do not intersect?"

    regioncorr-deallocate              \ regc1

    \ Test non-intersection.
    s" (1XXX 0XXXX)" regioncorr-from-string-a \ regc1 lst3

    \ cr ." lst3: " dup .regioncorr cr

    2dup regioncorr-intersects    \ regc1 lst3 bool
    abort" Lists do intersect?"

    \ Clean up.
    regioncorr-deallocate              \ regc1
    regioncorr-deallocate              \

    cr ." regioncorr-test-intersects: Ok" cr
;

: regioncorr-test-subtract
    \ Init lists.
    s" (X1X1 0X10X)" regioncorr-from-string-a \ regc1
    s" (0X0X 0XXX1)" regioncorr-from-string-a \ regc1 regc2

    cr dup .regioncorr space ." - " over .regioncorr
    \ Test subtraction.
    2dup regioncorr-subtract      \ regc1 regc2, regc-lst t | f

    if                                  \ regc1 regc2 regc-lst
        \ Check list length.
        dup list-get-length #4 <> abort" list length is not 4?"
        [ ' .regioncorr ] literal over list-apply

        \ Check results 1.
        s" (0x0x 0X0X1)" regioncorr-from-string-a       \ regc1 regc2 regc-lst lst-t
        [ ' regioncorr-eq ] literal                     \ regc1 regc2 regc-lst lst-t xt
        over #3 pick list-member                        \ regc1 regc2 regc-lst lst-t bool
        is-false abort" regioncorr not found in list of lists?"
        regioncorr-deallocate                           \ rlc1 rlc2 rlc-lst

        \ Check results 2.
        s" (0x0x 0XX11)" regioncorr-from-string-a       \ regc1 regc2 regc-lst lst-t
        [ ' regioncorr-eq ] literal                     \ regc1 regc2 regc-lst lst-t xt
        over #3 pick list-member                        \ regc1 regc2 regc-lst lst-t bool
        is-false abort" regioncorr not found in list of lists?"
        regioncorr-deallocate                           \ regc1 regc2 regc-lst

        \ Check results 3.
        s" (0X00 0xxx1)" regioncorr-from-string-a       \ regc1 regc2 regc-lst lst-t
        [ ' regioncorr-eq ] literal                     \ regc1 regc2 regc-lst lst-t xt
        over #3 pick list-member                        \ regc1 regc2 regc-lst lst-t bool
        is-false abort" regioncorr not found in list of lists?"
        regioncorr-deallocate                           \ regc1 regc2 regc-lst

        \ Check results 4.
        s" (000X 0xxx1)" regioncorr-from-string-a       \ regc1 regc2 regc-lst lst-t
        [ ' regioncorr-eq ] literal                     \ regc1 regc2 regc-lst lst-t xt
        over #3 pick list-member                        \ regc1 regc2 regc-lst lst-t bool
        is-false abort" regioncorr not found in list of lists?"
        regioncorr-deallocate                           \ regc1 regc2 regc-lst

        \ Clean up list-of-lists.
        regioncorr-list-deallocate                      \ regc1 regc2
    else
        cr ." Subtract failed?"
        abort
    then

    \ Clean up.                     \ regc1 regc2
    regioncorr-deallocate          \ regc1
    regioncorr-deallocate          \

    cr ." regioncorr-test-subtract: Ok" cr
;

: regioncorr-test-complement
    \ Init list 1.
    s" (X1X1 XX10X)" regioncorr-from-string-a \ regc

    cr ." regc: " dup .regioncorr

    dup                                 \ regc regc
    regioncorr-complement               \ regc regc-lst

    cr ." complement: "
    [ ' .regioncorr ] literal over list-apply cr

    \ Check list length.
    dup list-get-length #4 <> abort" list length is not 4?"
    \ [ ' .regioncorr ] literal over list-apply

    \ Check results 1.
    s" (xxxx XX0XX)" regioncorr-from-string-a       \ regc regc-lst regc-t
    [ ' regioncorr-eq ] literal                     \ regc regc-lst regc-t xt
    over #3 pick list-member                        \ regc regc-lst regc-t bool
    is-false abort" regioncorr not found in list of lists?"
    regioncorr-deallocate                          \ regc regc-lst

    \ Check results 2.
    s" (xxxx XXX1X)" regioncorr-from-string-a       \ regc regc-lst regc-t
    [ ' regioncorr-eq ] literal                     \ regc regc-lst regc-t xt
    over #3 pick list-member                        \ regc regc-lst regc-t bool
    is-false abort" regioncorr not found in list of lists?"
    regioncorr-deallocate                          \ regc regc-lst

    \ Check results 3.
    s" (XXX0 xxxxx)" regioncorr-from-string-a       \ regc regc-lst regc-t
    [ ' regioncorr-eq ] literal                     \ regc regc-lst regc-t xt
    over #3 pick list-member                        \ regc regc-lst regc-t bool
    is-false abort" regioncorr not found in list of lists?"
    regioncorr-deallocate                          \ regc regc-lst

    \ Check results 4.
    s" (X0XX xxxxx)" regioncorr-from-string-a       \ regc regc-lst regc-t
    [ ' regioncorr-eq ] literal                     \ regc regc-lst regc-t xt
    over #3 pick list-member                        \ regc regc-lst regc-t bool
    is-false abort" regioncorr not found in list of lists?"
    regioncorr-deallocate                           \ regc regc-lst

    \ Clean up list-of-lists.
    regioncorr-list-deallocate                      \ regc

    regioncorr-deallocate                          \

    cr ." regioncorr-test-complement: Ok" cr
;

: regioncorr-test-distance
    \ Init lists.
    s" (X1X1 0X10X)" regioncorr-from-string-a   \ regc1
    s" (1001 0XX11)" regioncorr-from-string-a   \ regc1 regc2

    2dup regioncorr-distance                    \ regc1 regc2 dist
    #2 <> abort" Distance not 2?"

    regioncorr-deallocate                       \ regc1
    regioncorr-deallocate                       \

    cr ." regioncorr-test-distance: Ok" cr
;

\ Assume domain0 is 4-bit, Domain 1 is 5-bit.
: regioncorr-tests
    regioncorr-test-superset
    regioncorr-test-intersects
    regioncorr-test-subtract
    regioncorr-test-complement
    regioncorr-test-distance
;

