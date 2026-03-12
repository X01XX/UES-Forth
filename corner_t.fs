
: corner-test-print
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    2dup swap session-add-domain                    \ sess dom0
    0 over domain-find-action                       \ sess dom0, act0 t | f
    is-false? abort" act0 not found?"                \ sess dom0 act0 |

    \ Init region.
    #5 #0 region-new                                \ | reg

    \ Set anchor state.
    #5                                              \ | reg #5
    #2 pick                                         \ | reg #5 act0

    corner-new                                      \ | crn
    dup
    .corner

    \ Clean up.
    corner-deallocate
    3drop

    current-session-deallocate

    cr ." corner-test-print: Ok"
;

: corner-tests
    corner-test-print
;
