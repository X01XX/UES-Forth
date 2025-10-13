\ Tests for the square struct functions.

: assert-square-pn-1 ( sqr0 -- sqr0 )
    dup square-get-pn
    1 <>
    abort" square pn not 1"

    dup square-get-rules rulestore-number-rules
    1 <>
    abort" square rulestore does not have 1 rule."
;

: assert-square-pn-2 ( sqr0 -- sqr0 )
    dup square-get-pn
    #2 <>
    abort" square pn not 2"

    dup square-get-rules rulestore-number-rules
    #2 <>
    if
        ." square rulestore does not have 2 rules."
        abort
    then
;

: assert-square-pn-3 ( sqr0 -- sqr0 )
    dup square-get-pn
    #3 <>
    abort" square pn not 3"

        dup square-get-rules rulestore-number-rules
    0<>
    abort" square rulestore does not have 0 rules."
;

: assert-square-pnc-t ( sqr0 -- sqr0 )
    dup square-get-pnc
    0= abort" square pnc is not true"
;

: assert-square-pnc-f ( sqr0 -- sqr0 )
    dup square-get-pnc
    abort" square pnc is not false"
;

: square-test-add-result
    cr ." square-test-add-result - start"

    cr ."    pn 1 to pnc = t"

    #4 #5 square-new
    \ cr ." square: " dup .square  ."  stack " .s cr
    assert-square-pnc-f
    assert-square-pn-1

    #4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    abort" flag true?"
    assert-square-pnc-f
    assert-square-pn-1

    #4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    abort" flag true?"
    assert-square-pnc-f
    assert-square-pn-1

    #4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    0= abort" flag not true?"
    assert-square-pnc-t
    assert-square-pn-1

    cr ."    pn 1 to U"

    #3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    0= abort" flag not true?"
    assert-square-pnc-t
    assert-square-pn-3

    cr ."    pn U to 1"

    #3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    abort" flag true?"
    assert-square-pnc-t
    assert-square-pn-3

    #3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    abort" flag true?"
    assert-square-pnc-t
    assert-square-pn-3

    #3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    0= abort" flag not true?"
    assert-square-pnc-t
    assert-square-pn-1

    cr ."    pn 1 to 2 to pnc = t"

    #4 #5 square-new
    \ cr ." square: " dup .square  ."  stack " .s cr

    #3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    0= abort" flag not true?"
    assert-square-pnc-f
    assert-square-pn-2

    #4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    abort" flag true?"
    assert-square-pnc-f
    assert-square-pn-2

    #3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    0= abort" flag not true?"
    assert-square-pnc-t
    assert-square-pn-2

    cr ."    pn 2 to U"

    #3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    0= abort" flag not true?"
    assert-square-pnc-t
    assert-square-pn-3

     cr ."    pn U to 2"

    #4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    abort" flag true?"
    assert-square-pnc-t
    assert-square-pn-3

    #3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    abort" flag true?"
    assert-square-pnc-t
    assert-square-pn-3

    #4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    0= abort" flag not true?"
    assert-square-pnc-t
    assert-square-pn-2

    square-deallocate
    square-deallocate

    cr ." square-test-add-result - Ok" cr
;

: assert-char-C ( char -- )
    [char] C <>
    abort" Returned char not C"
;

: assert-char-I ( char -- )
    [char] I <>
    abort" Returned char not I"
;

: assert-char-M ( char -- )
    [char] M <>
    abort" Returned char not M"
;

: square-test-compare
    cr ." square-test-compare - start"

    cr ."    pn 1 to 1 compatible:"
    #4 #5 square-new
    #5 #6 square-new
    2dup square-compare
    assert-char-C
    square-deallocate
    square-deallocate

    cr ."    pn 1 to 1 incompatible:"
    #4 #5 square-new
    1 #6 square-new
    2dup square-compare
    assert-char-I
    square-deallocate
    square-deallocate

    cr ."    pn 2 to 2 compatible:"
    #5 #5 square-new
    #10 over square-add-result drop  \ sqr5-5-a
    1 1 square-new
    #14 over square-add-result drop  \ sqr5-5-10 sqr-1-1-e
    \ 2dup ." sqr " .square cr ." sqr " .square
    2dup square-compare
    assert-char-C

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 2 incompatible:"
    \ Try incompatible.
    #5 #5 square-new
    #10 over square-add-result drop  \ sqr5-5-a
    1 1 square-new
    0 over square-add-result drop  \ sqr1-1-0
    2dup square-compare
    assert-char-I

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 2 compatible:"
    #5 #5 square-new
    #10 over square-add-result drop \ sqr5-5-a
     #5 over square-add-result drop  \ sqr5-5-a
    #10 over square-add-result drop \ sqr5-5-a
    assert-square-pnc-t

    1 1 square-new
    #14 over square-add-result drop \ sqr5-5-10 sqr-1-1-e
     1 over square-add-result drop  \ sqr5-5-10 sqr-1-1-e
    #14 over square-add-result drop \ sqr5-5-10 sqr-1-1-e
    assert-square-pnc-t

    2dup square-compare
    assert-char-C

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 1 incompatible, GT 1 sample:"
    \ Try order 1.
    #5 #5 square-new
    #10 over square-add-result drop \ sqr5-5-a
     #5 over square-add-result drop  \ sqr5-5-a
    #10 over square-add-result drop \ sqr5-5-a
    assert-square-pnc-t

    1 1 square-new
    1 over square-add-result drop  \ sqr5-5-a sqr-1-1

    2dup square-compare
    assert-char-I

    \ Try order 2.
    swap
    2dup square-compare
    assert-char-I

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 1 compatible, 1 sample:"
    \ Try order 1.
    #5 #5 square-new
    #10 over square-add-result drop  \ sqr5-5-a

    1 1 square-new

    2dup square-compare
    assert-char-M

    \ Try order 2.
    swap
    2dup square-compare
    assert-char-M

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 1 sample, incompatible:"
    \ Try order 1.
    #5 #5 square-new
    #10 over square-add-result drop  \ sqr5-5-a

    #9 1 square-new                  \ sqr-5-a sqr-1-9

    2dup square-compare
    assert-char-I
    
    \ Try order 2.
    swap
    2dup square-compare
    assert-char-I

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 1 sample, too compatible:"
    \ Try order 1
    #5 #5 square-new
    #4 over square-add-result drop  \ sqr5-5-4

    #2 #2 square-new                  \ sqr-5-a sqr-2-2

    2dup square-compare
    assert-char-I

    \ Try order 2
    swap
    2dup square-compare
    assert-char-I

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 1 sample, more samples needed:"
    \ Try order 1.
    #5 #5 square-new
    #10 over square-add-result drop  \ sqr5-5-a
     #5 over square-add-result drop  \ sqr5-5-a
    #10 over square-add-result drop  \ sqr5-5-a
    assert-square-pnc-t

    1 1 square-new                  \ sqr-5-a sqr-1-1

    2dup square-compare
    assert-char-M
    
    \ Try order 2.
    swap
    2dup square-compare
    assert-char-M

    square-deallocate
    square-deallocate

    cr ."    pn U to U compatible:"
    1 1 square-new
    #2 over square-add-result drop
    #3 over square-add-result drop
    assert-square-pn-3

    1 #2 square-new
    #2 over square-add-result drop
    #3 over square-add-result drop
    assert-square-pn-3

    2dup square-compare
    assert-char-C

    square-deallocate
    square-deallocate

    cr ."    pn U to 1 more samples needed:"

    \ Try order 1.
    1 1 square-new
    #2 over square-add-result drop
    #3 over square-add-result drop
    assert-square-pn-3

    1 #2 square-new

    2dup square-compare
    assert-char-M
    
    \ Try order 2.
    swap
    2dup square-compare
    assert-char-M

    square-deallocate
    square-deallocate

    cr ."    pn U to 1 incompatible:"

    \ Try order 1.
    1 1 square-new
    #2 over square-add-result drop
    #3 over square-add-result drop
    assert-square-pn-3

    1 #2 square-new
    1 over square-add-result drop
    1 over square-add-result drop
    1 over square-add-result drop
    assert-square-pnc-t

    2dup square-compare
    assert-char-I
    
    \ Try order 2.
    swap
    2dup square-compare
    assert-char-I

    square-deallocate
    square-deallocate

    cr ." square-test-compare - Ok" cr
;

: square-tests
    square-test-add-result
    square-test-compare
;
