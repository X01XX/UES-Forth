\ Tests for the square struct functions.

: square-test-none-in-use
    assert-square-mma-none-in-use
    assert-rulestore-mma-none-in-use
    assert-rule-mma-none-in-use
    depth 0<>
    if
        cr ." stack not empty " .s cr
    then
;

: assert-square-pn-1 ( sqr0 -- sqr0 )
    dup square-get-pn
    1 <>
    if
        ." square pn not 1"
        abort
    then
    dup square-get-rules rulestore-number-rules
    1 <>
    if
        ." square rulestore does not have 1 rule."
        abort
    then
;

: assert-square-pn-2 ( sqr0 -- sqr0 )
    dup square-get-pn
    2 <>
    if
        ." square pn not 2"
        abort
    then
    dup square-get-rules rulestore-number-rules
    2 <>
    if
        ." square rulestore does not have 2 rules."
        abort
    then
;

: assert-square-pn-3 ( sqr0 -- sqr0 )
    dup square-get-pn
    3 <>
    if
        ." square pn not 3"
        abort
    then
        dup square-get-rules rulestore-number-rules
    0<>
    if
        ." square rulestore does not have 0 rules."
        abort
    then
;

: assert-square-pnc-t ( sqr0 -- sqr0 )
    dup square-get-pnc
    if
    else
        ." square pnc is not true"
        abort
    then
;

: assert-square-pnc-f ( sqr0 -- sqr0 )
    dup square-get-pnc
    if
        ." square pnc is not false"
        abort
    then
;

: square-test-add-result
    cr ." square-test-add-result - start"

    square-test-none-in-use

    cr ."    pn 1 to pnc = t"

    4 5 square-new
    \ cr ." square: " dup .square  ."  stack " .s cr
    assert-square-pnc-f
    assert-square-pn-1

    4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if ." flag true?" abort then
    assert-square-pnc-f
    assert-square-pn-1

    4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if ." flag true?" abort then
    assert-square-pnc-f
    assert-square-pn-1

    4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if else ." flag not true?" abort then
    assert-square-pnc-t
    assert-square-pn-1

    cr ."    pn 1 to U"

    3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if else ." flag not true?" abort then
    assert-square-pnc-t
    assert-square-pn-3

    cr ."    pn U to 1"

    3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if ." flag true?" abort then
    assert-square-pnc-t
    assert-square-pn-3

    3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if ." flag true?" abort then
    assert-square-pnc-t
    assert-square-pn-3

    3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if else ." flag not true?" abort then
    assert-square-pnc-t
    assert-square-pn-1

    cr ."    pn 1 to 2 to pnc = t"

    4 5 square-new
    \ cr ." square: " dup .square  ."  stack " .s cr

    3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if else ." flag not true?" abort then
    assert-square-pnc-f
    assert-square-pn-2

    4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if ." flag true?" abort then
    assert-square-pnc-f
    assert-square-pn-2

    3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if else ." flag not true?" abort then
    assert-square-pnc-t
    assert-square-pn-2

    cr ."    pn 2 to U"

    3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if else ." flag not true?" abort then
    assert-square-pnc-t
    assert-square-pn-3

     cr ."    pn U to 2"

    4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if ." flag true?" abort then
    assert-square-pnc-t
    assert-square-pn-3

    3 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if ." flag true?" abort then
    assert-square-pnc-t
    assert-square-pn-3

    4 over square-add-result        \ sqr0 flag
    \ cr ." square: " over .square ." cng " .bool ."  stack " .s cr
    if else ." flag not true?" abort then
    assert-square-pnc-t
    assert-square-pn-2

    \ cr memory-use cr
    \ cr ."     Deallocating ..."
    square-deallocate
    square-deallocate

    \ cr memory-use
    square-test-none-in-use

    cr ." square-test-add-result - Ok" cr
;

: assert-char-C ( char -- )
    [char] C <>
    if
        ." Returned char not C"
        abort
    then
;

: assert-char-I ( char -- )
    [char] I <>
    if
        ." Returned char not I"
        abort
    then
;

: assert-char-M ( char -- )
    [char] M <>
    if
        ." Returned char not M"
        abort
    then
;

: square-test-compare
    cr ." square-test-compare - start"

    square-test-none-in-use

    cr ."    pn 1 to 1 compatible:"
    4 5 square-new
    5 6 square-new
    2dup square-compare
    space dup emit
    assert-char-C
    square-deallocate
    square-deallocate

    cr ."    pn 1 to 1 incompatible:"
    4 5 square-new
    1 6 square-new
    2dup square-compare
    space dup emit
    assert-char-I
    square-deallocate
    square-deallocate

    cr ."    pn 2 to 2 more samples needed:"
    \ Try pnc f, pnc f
    5 5 square-new
    10 over square-add-result drop  \ sqr5-5-a
    1 1 square-new
    14 over square-add-result drop  \ sqr5-5-10 sqr-1-1-e
    \ 2dup ." sqr " .square cr ." sqr " .square
    2dup square-compare
    space dup emit
    assert-char-M

    square-deallocate
    square-deallocate

    \ Try pnc t, pnc f
    5 5 square-new
    10 over square-add-result drop  \ sqr5-5-a
     5 over square-add-result drop  \ sqr5-5-a
    10 over square-add-result drop  \ sqr5-5-a
    assert-square-pnc-t

    1 1 square-new
    14 over square-add-result drop  \ sqr5-5-10 sqr-1-1-e
    assert-square-pnc-f
    
    \ 2dup ." sqr " .square cr ." sqr " .square
    2dup square-compare
    space dup emit
    assert-char-M

    \ Try different order.
    swap
    2dup square-compare
    space dup emit
    assert-char-M

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 2 incompatible:"
    \ Try incompatible.
    5 5 square-new
    10 over square-add-result drop  \ sqr5-5-a
    1 1 square-new
    0 over square-add-result drop  \ sqr1-1-0
    2dup square-compare
    space dup emit
    assert-char-I

    square-deallocate
    square-deallocate
    
    \ Try too compatible. Combination produces X0, X1 and XX, Xx.
    5 5 square-new
    10 over square-add-result drop  \ sqr5-5-a
    9 9 square-new
    1 over square-add-result drop  \ sqr9-9-1
    2dup square-compare
    space dup emit
    assert-char-I

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 2 compatible:"
    5 5 square-new
    10 over square-add-result drop  \ sqr5-5-a
     5 over square-add-result drop  \ sqr5-5-a
    10 over square-add-result drop  \ sqr5-5-a
    assert-square-pnc-t

    1 1 square-new
    14 over square-add-result drop  \ sqr5-5-10 sqr-1-1-e
     1 over square-add-result drop  \ sqr5-5-10 sqr-1-1-e
    14 over square-add-result drop  \ sqr5-5-10 sqr-1-1-e
    assert-square-pnc-t

    2dup square-compare
    space dup emit
    assert-char-C

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 1 incompatible, GT 1 sample:"
    \ Try order 1.
    5 5 square-new
    10 over square-add-result drop  \ sqr5-5-a
     5 over square-add-result drop  \ sqr5-5-a
    10 over square-add-result drop  \ sqr5-5-a
    assert-square-pnc-t

    1 1 square-new
    1 over square-add-result drop  \ sqr5-5-a sqr-1-1

    2dup square-compare
    space dup emit
    assert-char-I

    \ Try order 2.
    swap
    2dup square-compare
    space dup emit
    assert-char-I

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 1 sample, incompatible:"
    \ Try order 1.
    5 5 square-new
    10 over square-add-result drop  \ sqr5-5-a
     5 over square-add-result drop  \ sqr5-5-a
    10 over square-add-result drop  \ sqr5-5-a
    assert-square-pnc-t

    9 1 square-new                  \ sqr-5-a sqr-1-9

    2dup square-compare
    space dup emit
    assert-char-I
    
    \ Try order 2.
    swap
    2dup square-compare
    space dup emit
    assert-char-I

    square-deallocate
    square-deallocate

    cr ."    pn 2 to 1 sample, more samples needed:"
    \ Try order 1.
    5 5 square-new
    10 over square-add-result drop  \ sqr5-5-a
     5 over square-add-result drop  \ sqr5-5-a
    10 over square-add-result drop  \ sqr5-5-a
    assert-square-pnc-t

    1 1 square-new                  \ sqr-5-a sqr-1-1

    2dup square-compare
    space dup emit
    assert-char-M
    
    \ Try order 2.
    swap
    2dup square-compare
    space dup emit
    assert-char-M

    square-deallocate
    square-deallocate

    cr ."    pn U to U compatible:"
    1 1 square-new
    2 over square-add-result drop
    3 over square-add-result drop
    assert-square-pn-3

    1 2 square-new
    2 over square-add-result drop
    3 over square-add-result drop
    assert-square-pn-3

    2dup square-compare
    space dup emit
    assert-char-C

    square-deallocate
    square-deallocate

    cr ."    pn U to 1 more samples needed:"

    \ Try order 1.
    1 1 square-new
    2 over square-add-result drop
    3 over square-add-result drop
    assert-square-pn-3

    1 2 square-new

    2dup square-compare
    space dup emit
    assert-char-M
    
    \ Try order 2.
    swap
    2dup square-compare
    space dup emit
    assert-char-M

    square-deallocate
    square-deallocate

    cr ."    pn U to 1 incompatible:"

    \ Try order 1.
    1 1 square-new
    2 over square-add-result drop
    3 over square-add-result drop
    assert-square-pn-3

    1 2 square-new
    1 over square-add-result drop
    1 over square-add-result drop
    1 over square-add-result drop
    assert-square-pnc-t

    2dup square-compare
    space dup emit
    assert-char-I
    
    \ Try order 2.
    swap
    2dup square-compare
    space dup emit
    assert-char-I

    square-deallocate
    square-deallocate

    \ Last check.
    square-test-none-in-use

    cr ." square-test-compare - Ok" cr
;

: square-tests
    square-test-add-result
    square-test-compare
;
