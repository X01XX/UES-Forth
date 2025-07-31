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

: square-test-add-result
    cr ." square-test-add-result - start" cr

    square-test-none-in-use

    cr ." test pn 1" cr
    4 5 square-new
    cr ." square: " dup .square  ."  stack " .s cr

    4 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    4 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    4 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    cr ." test pn 1 to U" cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    cr ." test pn U to 1" cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr


    cr ." test pn 2" cr

    4 5 square-new
    cr ." square: " dup .square  ."  stack " .s cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    4 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    cr ." test pn 2 to U" cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

     cr ." test pn U to 2" cr

     4 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    4 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    cr memory-use cr
    cr ." Deallocating ..."
    square-deallocate
    square-deallocate

    cr memory-use
    square-test-none-in-use

    cr ." square-test-add-result - end" cr
;

: square-tests
    square-test-add-result
;
