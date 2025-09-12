
: state-complement ( u0 -- list )
    \ Check arg.
    assert-tos-is-value

    cur-domain-xt execute               \ u0 dom
    domain-get-all-bits-mask-xt execute \ u0 all-bits
    0 region-new                        \ u0 reg-max
    dup struct-inc-use-count
    tuck                                \ reg-max u0 reg-max
    region-subtract-state               \ reg-max list
    swap region-deallocate              \ list
;

: state-not-a-or-not-b ( u1 u0 -- list )
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    state-complement                \ u1 comp0

    swap state-complement           \ comp0 comp1

    2dup region-list-set-union      \ comp0 comp1 list-u

    swap region-list-deallocate     \ comp0 list-u

    swap region-list-deallocate     \ list-u
;

