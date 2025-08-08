
: state-complement ( u0 -- list )
    \ Check arg.
    assert-arg0-is-value

    domain-all-bits-xt execute          \ u2 all-bits
    0 region-new-xt execute             \ u0 reg-max
    dup struct-inc-use-count
    swap over                           \ reg-max u0 reg-max
    region-subtract-state-xt execute    \ reg-max list
    swap region-deallocate-xt execute   \ list
;

: state-not-a-or-not-b ( u1 u0 -- list )
    \ Check args.
    assert-arg0-is-value
    assert-arg1-is-value

    state-complement                \ u1 comp0

    swap state-complement           \ comp0 comp1

    2dup region-list-set-union-xt execute      \ comp0 comp1 list-u

    swap region-list-deallocate-xt execute     \ comp0 list-u

    swap region-list-deallocate-xt execute     \ list-u
;

\ Print state to state rule.
: .state-to-state ( r-val1 i-val0 -- )
    \ Check arg.
    assert-arg0-is-value
    assert-arg0-is-value

    \ Setup for bit-position loop.
    domain-ms-bit-xt execute   ( r-val1 i-val0 ms-bit)

    \ Process each bit.
    begin
      dup       \ r-val1 i-val0 ms-bit ms-bit
    while
        \ Apply msb to i-val0, to get an isolated bit.
        2dup
        and       \ r-val1 i-val0 ms-bit bit

        if
            ." 1"
        else
            ." 0"
        then

        \ Apply msb to r-val1
        2 pick over and

        if
            ." 1"
        else
            ." 0"
        then

        1 rshift   \ r-val1 i-val0 ms-bit

        \ Check if separator is needed.
        dup if
            ." /"
        then
    repeat
    2drop drop       \
;
