

: regioncorr-list-test-any-superset
    list-new                            \ regc-lst
    s" (xx10 XX0XX)" regioncorr-from-string-a
    over regioncorr-list-push           \ regc-lst

    s" (xxx1 XX0XX)" regioncorr-from-string-a
    swap                                \ regc regc-lst

    2dup regioncorr-list-any-superset   \ regc regc-lst bool
    abort" Superset found?"

    s" (xxx1 XX0XX)" regioncorr-from-string-a
    over regioncorr-list-push           \ regc regc-lst

    2dup regioncorr-list-any-superset   \ regc regc-lst bool
    is-false abort" Superset not found?"

                                        \ regc regc-lst
    regioncorr-list-deallocate          \ regc
    regioncorr-deallocate               \

    cr ." regioncorr-list-test-any-superset: Ok" cr
;

: regioncorr-list-test-remove-subsets
    s" (xx1x XX0XX)" regioncorr-from-string-a   \ regc

    list-new                                    \ regc regc-lst
    s" (xx10 XX0XX)" regioncorr-from-string-a
    over regioncorr-list-push                   \ regc regc-lst

    s" (xxx1 XX0XX)" regioncorr-from-string-a
    over regioncorr-list-push                   \ regc regc-lst

    s" (x111 XX0XX)" regioncorr-from-string-a
    over regioncorr-list-push                   \ regc regc-lst

    cr ." before: " dup .regioncorr-list
    2dup regioncorr-list-remove-subsets         \ regc regc-lst
    cr ." after:  " dup .regioncorr-list

    \ Check results.
    dup list-get-length                         \ regc regc-lst len
    1 <> abort" Result length not 1?"

    \ Check results 1.
    s" (XXX1 xx0xx)" regioncorr-from-string-a   \ regc regc-lst regc-t
    [ ' regioncorr-eq ] literal                 \ regc regc-lst regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-t bool
    is-false abort" regioncorr (XXX1 xx0xx) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst

                                                \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate                       \

    cr ." regioncorr-list-test-remove-subsets: Ok" cr
;

: regioncorr-list-test-push-nosubs
    s" (xx1x XX0XX)" regioncorr-from-string-a   \ regc

    list-new                                    \ regc regc-lst
    s" (xx10 XX0XX)" regioncorr-from-string-a
    over regioncorr-list-push                   \ regc regc-lst

    s" (xxx1 XX0XX)" regioncorr-from-string-a
    over regioncorr-list-push                   \ regc regc-lst

    s" (x111 XX0XX)" regioncorr-from-string-a
    over regioncorr-list-push                   \ regc regc-lst

    cr ." before: " dup .regioncorr-list
    2dup regioncorr-list-push-nosubs            \ regc regc-lst bool
    is-false abort" push-nosubs failed?"
    cr ." after:  " dup .regioncorr-list

    \ Check results.
    dup list-get-length                         \ regc regc-lst len
    #2 <> abort" Result length not 2?"

    \ Check results 1.
    s" (XXX1 xx0xx)" regioncorr-from-string-a   \ regc regc-lst regc-t
    [ ' regioncorr-eq ] literal                 \ regc regc-lst regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-t bool
    is-false abort" regioncorr (XXX1 xx0xx) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst

    \ Check results 2.
    swap                                        \ regc-lst regc
    [ ' regioncorr-eq ] literal                 \ regc-lst regc xt
    swap                                        \ regc-lst xt regc
    #2 pick list-member                         \ regc-lst bool
    is-false abort" regioncorr (xx1x XX0XX) not found in regioncorr-list?"

                                                \ regc-lst
    regioncorr-list-deallocate                  \

    cr ." regioncorr-list-test-push-nosubs: Ok" cr
;

: regioncorr-list-test-copy-nosubs
    \ Init rcl list.
    \ cr ." at 1 " .s cr
    list-new                                    \ rcl-lst

    \ Add first regc.
    s" (000X 000XX)" regioncorr-from-string-a   \ regc-lst rclx
    over regioncorr-list-push                   \ regc-lst

    \ Add subset regc.
    s" (0001 00010)" regioncorr-from-string-a   \ regc-lst rclx
    over regioncorr-list-push                   \ regc-lst

    \ Add first duplicate regc.
    s" (000X 000XX)" regioncorr-from-string-a   \ regc-lst rclx
    over regioncorr-list-push                   \ regc-lst
    \ cr ." before: " dup .regioncorr-list

    dup                                         \ regc-lst regc-lst
    regioncorr-list-copy-nosubs                 \ regc-lst regc-lst'
    \ cr ." after:  " dup .regioncorr-list

    \ Check result length.
    dup list-get-length                         \ regc regc-lst len
    1 <> abort" Result length not 1?"

    \ Check results 1.
    s" (000X 000XX)" regioncorr-from-string-a   \ regc-lst regc-lst' regc-t
    [ ' regioncorr-eq ] literal                 \ regc-lst regc-lst' regc-t xt
    over #3 pick list-member                    \ regc-lst regc-lst' regc-t bool
    is-false abort" regioncorr (000X 000XX) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc-lst regc-lst'

    \ Clean up.
    swap
    regioncorr-list-deallocate                  \ regc-lst
    regioncorr-list-deallocate                  \

    cr ." regioncorr-list-test-copy-nosubs: Ok" cr
;

: regioncorr-list-test-subtract-regc
    \ Run against an intersecting regc, in an regioncorr-list.

    \ Init regc to subtract.
    s" (000X 001XX)" regioncorr-from-string-a   \ regc

    \ Init regioncorr-list to subtract from.
    list-new                                    \ regc regc-lst

    \ Add intersecting regc.
    s" (X001 00X0X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst

    \ Do subtraction.
    2dup regioncorr-list-subtract-regioncorr    \ regc regc-lst regc-left

    \ Check result.
    \ dup .regioncorr-list
    dup list-get-length #2 <> abort" List length not 2?"

    \ Check result 1.
    s" (1001 00x0x)" regioncorr-from-string-a   \ regc regc-lst regc-left regc-t
    [ ' regioncorr-eq ] literal                 \ regc regc-lst regc-left regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-left regc-t bool
    is-false abort" regioncorr (1001 00x0x) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst regc-left

    \ Check result 2.
    s" (x001 0000X)" regioncorr-from-string-a   \ regc regc-lst regc-left regc-t
    [ ' regioncorr-eq ] literal                 \ regc regc-lst regc-left regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-left regc-t bool
    is-false abort" regioncorr (x001 0000X) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst regc-left

    \ Clean up.
    regioncorr-list-deallocate                  \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate                       \


    \ Run against a non-intersection regc, in an regioncorr-list.

    \ Init regc to subtract.
    s" (000X 001XX)" regioncorr-from-string-a   \ regc

    \ Init regioncorr-list to subtract from.
    list-new                                    \ regc regc-lst

    \ Add non-intersecting regc.
    s" (X011 00X0X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst

    \ Do subtraction.
    2dup regioncorr-list-subtract-regioncorr    \ regc regc-lst regc-left

    \ Check result.
    \ dup .regioncorr-list
    dup list-get-length 1 <> abort" List length not 1?"

    \ Check result 1.
    s" (X011 00X0X)" regioncorr-from-string-a   \ regc regc-lst regc-left regc-t
    [ ' regioncorr-eq ] literal                 \ regc regc-lst regc-left regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-left regc-t bool
    is-false abort" regioncorr (X011 00X0X) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst regc-left

    \ Clean up.
    regioncorr-list-deallocate                  \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate                       \

    \ Run against a subset regc, in an regioncorr-list.

    \ Init regc to subtract.
    s" (000X 001XX)" regioncorr-from-string-a   \ regc

    \ Init regioncorr-list to subtract from.
    list-new                                    \ regc regc-lst

    \ Add a subset regc
    s" (0000 0010X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst

    \ Do subtraction.
    2dup regioncorr-list-subtract-regioncorr    \ regc regc-lst regc-left

    \ Check result.
    \ dup .regioncorr-list
    dup list-get-length 0<> abort" List length not 0?"

    \ Clean up.
    regioncorr-list-deallocate                  \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate                       \

    \ Run against a all three options.

    \ Init regc to subtract.
    s" (000X 001XX)" regioncorr-from-string-a   \ regc

    \ Init regioncorr-list to subtract from.
    list-new                                    \ regc regc-lst

    \ Add intersecting regc.
    s" (X001 00X0X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push

    \ Add a subset regc.
    s" (0000 0010X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst

    \ Add non-intersecting regc.
    s" (X011 00X0X)" regioncorr-from-string-a   \ regc regc-lst regcx
    over regioncorr-list-push                   \ regc regc-lst
    cr dup .regioncorr-list space ." - " over .regioncorr

    \ Do subtraction.
    2dup regioncorr-list-subtract-regioncorr    \ regc regc-lst regc-left
    space ." = " dup .regioncorr-list cr

    \ Check result.
    \ dup .regioncorr-list
    dup list-get-length #3 <> abort" List length not 0?"

    \ Check result 1.
    s" (1001 00x0x)" regioncorr-from-string-a   \ regc regc-lst regc-left regc-t
    [ ' regioncorr-eq ] literal                 \ regc regc-lst regc-left regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-left regc-t bool
    is-false abort" regioncorr (1001 00x0x) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst regc-left

    \ Check result 2.
    s" (x001 0000X)" regioncorr-from-string-a   \ regc regc-lst regc-left regc-t
    [ ' regioncorr-eq ] literal                 \ regc regc-lst regc-left regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-left regc-t bool
    is-false abort" regioncorr (x000 0000X) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst regc-left

    \ Check result 3.
    s" (x011 00x0x)" regioncorr-from-string-a   \ regc regc-lst regc-left regc-t
    [ ' regioncorr-eq ] literal                 \ regc regc-lst regc-left regc-t xt
    over #3 pick list-member                    \ regc regc-lst regc-left regc-t bool
    is-false abort" regioncorr (x011 00x0x) not found in regioncorr-list?"
    regioncorr-deallocate                       \ regc regc-lst regc-left

    \ Clean up.
    regioncorr-list-deallocate                  \ regc regc-lst
    regioncorr-list-deallocate                  \ regc
    regioncorr-deallocate                       \

    cr ." regioncorr-list-test-subtract-regc: Ok" cr
;

: regioncorr-list-test-complement
    \ Init regioncorr-list.
    list-new                                    \ regc-lst

    \ Add regc 1.
    s" (X10X X1X0X)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Add regc 2.
    s" (X11X X1X1X)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Get complement.
    dup regioncorr-list-complement              \ regc-lst regc-lst'

    \ Check results.
    \ cr ." comp1: " dup .regioncorr-list cr
    dup list-get-length #4 <> abort" comp1 len not 4?"

    dup regioncorr-list-complement              \ regc-lst regc-lst' regc-lst''
    \ cr ." comp2: " dup .regioncorr-list cr
    dup list-get-length #2 <> abort" comp1 len not 2?"

    #2 pick                                     \ regc-lst regc-lst' regc-lst'' regc-lst
    over                                        \ regc-lst regc-lst' regc-lst'' regc-lst regc-lst''
    regioncorr-list-eq                          \ regc-lst regc-lst' regc-lst'' bool
    is-false abort" Initial and result regioncorr-lists not equal?"

    \ Clean up.
    regioncorr-list-deallocate                  \ regc-lst regc-lst'
    regioncorr-list-deallocate                  \ regc-lst
    regioncorr-list-deallocate                  \

    cr ." regioncorr-list-test-complement: Ok" cr
;

: regioncorr-list-test-normalize
    \ Init regioncorr-list.
    list-new                                    \ regc-lst

    \ Add regc 1.
    s" (X10X X1X0X)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Add regc 2.
    s" (X11X X1X0X)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Normalize
    dup regioncorr-list-normalize               \ regc-lst regc-lst'

    \ Check results.
    \ cr ." norm: " dup .regioncorr-list cr
    dup list-get-length 1 <> abort" normalized len not 1?"

    \ Check result 1.
    s" (X1XX X1X0X)" regioncorr-from-string-a   \ regc-lst regc-lst' regc-t
    [ ' regioncorr-eq ] literal                 \ regc-lst regc-lst' regc-t xt
    over #3 pick list-member                    \ regc-lst regc-lst' regc-t bool
    is-false abort" regioncorr (X1XX X1X0X) not found in regc-lst' ?"
    regioncorr-deallocate                       \ regc-lst regc-lst'

    \ Clean up.
    regioncorr-list-deallocate                  \ regc-lst
    regioncorr-list-deallocate                  \

    cr ." regioncorr-list-test-normalize: Ok" cr
;

: regioncorr-list-test-intersection-fragments
\ Init regioncorr-list.
    list-new                                    \ regc-lst

    \ Add regc 1.
    s" (X1X1 XX1X1)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Add regc 2.
    s" (1XX1 X1X1X)" regioncorr-from-string-a   \ regc-lst regcx
    over regioncorr-list-push                   \ regc-lst

    \ Normalize
    dup regioncorr-list-intersection-fragments  \ regc-lst regc-lst'

    \ Check results.
    \ cr ." results: " dup .regioncorr-list cr

    dup list-get-length #7 <> abort" result len not 7?"

    \ Check result intersection.
    s" (11X1 X1111)" regioncorr-from-string-a   \ regc-lst regc-lst' regc-t
    [ ' regioncorr-eq ] literal                 \ regc-lst regc-lst' regc-t xt
    over #3 pick list-member                    \ regc-lst regc-lst' regc-t bool
    is-false abort" regioncorr (11X1 X1111) not found in regc-lst' ?"
    regioncorr-deallocate                       \ regc-lst regc-lst'

    \ Clean up.
    regioncorr-list-deallocate                  \ regc-lst
    regioncorr-list-deallocate                  \

    cr ." regioncorr-list-test-intersection-fragments: Ok" cr
;

: regioncorr-list-tests
    current-session-new                             \ ses

    \ Init domain 0.
    #4 over domain-new                              \ ses dom0
    over                                            \ ses dom0 ses
    session-add-domain                              \ ses

    \ Init domain 1.
    #5 over domain-new                              \ ses dom0
    swap                                            \ dom0 ses
    session-add-domain                              \

    regioncorr-list-test-any-superset
    regioncorr-list-test-remove-subsets
    regioncorr-list-test-push-nosubs
    regioncorr-list-test-copy-nosubs
    regioncorr-list-test-subtract-regc
    regioncorr-list-test-complement
    regioncorr-list-test-normalize
    regioncorr-list-test-intersection-fragments

    current-session-deallocate

    cr ." regioncorr-list tests: Ok" cr
;
