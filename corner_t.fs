
: corner-test-print
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    2dup swap session-add-domain                    \ sess dom0
    0 over domain-find-action                       \ sess dom0, act0 t | f
    is-false abort" act0 not found?"                \ sess dom0 act0 |
    

    \ Init the square list.
    list-new                                        \ | sqr-lst
    #9  1 square-new over list-push-struct
    $E #6 square-new over list-push-struct

    \ Store square D, but save a reference.
    $F $D square-new                                \ | sqr-lst sqrD
    dup #2 pick                                     \ | sqr-lst sqrD sqrD sqr-lst
    list-push-struct                                \ | sqr-lst sqrD
    swap                                            \ | sqrD sqr-lst

    \ Init anchor square.
    #5 #5 square-new                                \ | sqrD sqr-lst sqr5
    #3 pick                                         \ | sqrD sqr-lst sqr5 act0

    corner-new                                      \ | sqrD crn
    dup
    .corner

    \ Change square D to be compatible to square 5.
    $D #2 pick square-add-result drop
    $D #2 pick square-add-result drop
    $D #2 pick square-add-result drop
    $D #2 pick square-add-result drop

    dup .corner

    \ Clean up.
    nip                                             \ | crn
    corner-deallocate
    3drop

    current-session-deallocate
;

: corner-tests
    corner-test-print
;
