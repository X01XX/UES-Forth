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

: square-tests
    square-test-add-result
;
