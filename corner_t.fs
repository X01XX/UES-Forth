
: corner-test-print
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    swap                                            \ dom0 sess
    session-add-domain                              \

    \ Init the square list.
    list-new                                        \ sqr-lst
    #9  1 square-new over list-push-struct
    $E #6 square-new over list-push-struct

    \ Store square D, but save a reference.
    $F $D square-new                                \ sqr-lst sqrD
    dup #2 pick                                     \ sqr-lst sqrD sqrD sqr-lst
    list-push-struct                                \ sqr-lst sqrD
    swap                                            \ sqrD sqr-lst

    \ Init anchor square.
    #5 #5 square-new                                \ sqrD sqr-lst sqr

    corner-new                                      \ sqrD cnr
    dup
    .corner

    \ Change square D to be compatible to square 5.
    $D #2 pick square-add-result drop
    $D #2 pick square-add-result drop
    $D #2 pick square-add-result drop
    $D #2 pick square-add-result drop

    dup .corner

    \ Clean up.
    nip                                             \ crn
    corner-deallocate

    current-session-deallocate
;

: corner-tests
    corner-test-print
;
