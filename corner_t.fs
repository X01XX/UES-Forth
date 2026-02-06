
: corner-test-print
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    2dup swap session-add-domain                    \ sess dom0
    0 over domain-find-action                       \ sess dom0, act0 t | f
    is-false abort" act0 not found?"                \ sess dom0 act0 |

    \ Init square  D.
    $F $D square-new                                \ | sqrD

    \ Init region list.
    list-new                                        \ | sqrD reg-lst
    #4 $D region-new over region-list-push          \ | sqrD reg-lst
    #7 $D region-new over region-list-push          \ | sqrD reg-lst

    \ Init the square list.
    list-new                                        \ | sqrD reg-lst sqr-lst
    #9  1 square-new over list-push-struct
    $E #6 square-new over list-push-struct

    \ Store square D.
    #2 pick                                         \ | sqrD reg-lst sqr-lst sqrD
    over                                            \ | sqrD reg-lst sqr-lst sqrD sqr-lst
    list-push-struct                                \ | sqrD reg-lst sqr-lst

    \ Init anchor square.
    #5 #5 square-new                                \ | sqrD reg-lst sqr-lst sqr5
    #4 pick                                         \ | sqrD reg-lst sqr-lst sqr5 act0

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

    cr ." corner-test-print: Ok"
;

: corner-test-1
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    2dup swap session-add-domain                    \ sess dom0
    0 over domain-find-action                       \ sess dom0, act0 t | f
    is-false abort" act0 not found?"                \ sess dom0 act0 |

    %0001 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0100 %0100 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0

    %1101 %1100 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0101 %0011 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0

    over cr .domain cr

    over domain-get-max-region                  \ sess dom0 act0 | max-reg
    #2 pick                                     \ sess dom0 act0 | max-reg dom0
    domain-get-needs                            \ sess dom0 act0 | ned-lst'
    dup cr .need-list cr
    need-list-deallocate

    \ Confirm squares, to elicit dissimilar square needs.
    %0001 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0100 %0100 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %1101 %1100 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0101 %0011 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0001 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0100 %0100 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %1101 %1100 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0101 %0011 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0001 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0100 %0100 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %1101 %1100 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0
    %0101 %0011 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0

    over domain-get-max-region                  \ sess dom0 act0 | max-reg
    #2 pick                                     \ sess dom0 act0 | max-reg dom0
    domain-get-needs                            \ sess dom0 act0 | ned-lst'
    dup cr .need-list cr
    need-list-deallocate
    over cr .domain cr

    \ One corner.
    \ (0001 limited by:  0011) in (0X0X X00X XX01)
    \ (0100 limited by:  1100) in (01XX 0XX0 0X0X)
    \ Remove XX01 as a possibility for corner anchor 1.
    \ Remove 01XX as a possibility for corner anchor 4.
    %0000 %0000 sample-new dup #2 pick action-add-sample sample-deallocate  \ sess dom0 act0

    over domain-get-max-region                  \ sess dom0 act0 | max-reg
    #2 pick                                     \ sess dom0 act0 | max-reg dom0
    domain-get-needs                            \ sess dom0 act0 | ned-lst'
    dup cr .need-list cr
    need-list-deallocate
    over cr .domain cr

    3drop

    current-session-deallocate

    cr ." corner-test-1: Ok"
;

: corner-tests
    corner-test-print
;
