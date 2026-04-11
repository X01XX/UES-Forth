
: pathstep-test-eq-initial-subset-result?
    cr ." pathstep-test-eq-initial-subset-result?: "

    session-new                                     \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    over                                            \ sess dom0 sess
    session-add-domain                              \ sess

    s" (XX/X0/00/00/)" pathstep-from-string-a       \ sess pthstp1
    \ cr ." pathstep 1: " dup .pathstep cr

    s" (X1/X0/00/00/)" pathstep-from-string-a       \ sess pthstp1 pthstp2
    \ cr ." pathstep 2: " dup .pathstep cr

   \ Test subset.
    2dup pathstep-eq-initial-subset-result?         \ sess pthstp1 pthstp2 bool
    \ cr ." result 1: " dup .bool cr
    false? abort" should be true"

    \ Test reverse.
    2dup swap pathstep-eq-initial-subset-result?    \ sess pthstp1 pthstp2 bool
    \ cr ." result 2: " dup .bool cr
    abort" should be false"

    \ test equal.
    dup dup pathstep-eq-initial-subset-result?      \ sess pthstp1 pthstp2 bool
    \ cr ." result 3: " dup .bool cr
    false? abort" should be true"

    pathstep-deallocate
    pathstep-deallocate

    session-deallocate

    ." Ok" cr
;

: pathstep-test-eq-initial-superset-result?
    cr ." pathstep-test-eq-initial-superset-result?: "

    session-new                                     \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    over                                            \ sess dom0 sess
    session-add-domain                              \ sess

    s" (XX/X0/00/00/)" pathstep-from-string-a       \ sess pthstp1
    \ cr ." pathstep 1: " dup .pathstep cr

    s" (X1/X0/00/00/)" pathstep-from-string-a       \ sess pthstp1 pthstp2
    \ cr ." pathstep 2: " dup .pathstep cr

   \ Test superset.
    2dup pathstep-eq-initial-superset-result?        \ sess pthstp1 pthstp2 bool
    \ cr ." result 1: " dup .bool cr
    abort" should be false"

    \ Test reverse.
    2dup swap pathstep-eq-initial-superset-result?  \ sess pthstp1 pthstp2 bool
    \ cr ." result 2: " dup .bool cr
    false? abort" should be true"

    \ test equal.
    dup dup pathstep-eq-initial-superset-result?    \ sess pthstp1 pthstp2 bool
    \ cr ." result 3: " dup .bool cr
    false? abort" should be true"

    pathstep-deallocate
    pathstep-deallocate

    session-deallocate

    ." Ok" cr
;

: pathstep-tests

    pathstep-test-eq-initial-subset-result?
    pathstep-test-eq-initial-superset-result?
;
