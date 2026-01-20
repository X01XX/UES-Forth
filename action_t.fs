\ Tests for action struct functions.

\ Test adding samples and changing the logical structure, incompatible pairs.
: action-test-add-sample
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    tuck swap                                       \ dom0 dom0 sess
    session-add-domain                              \ dom0

    0  swap domain-find-action                      \ act0 t | f
    is-false abort" act0 not found?"

    \ Add arbitrary samples.
    #4 #5 sample-new                \ act smpl
    dup #2 pick action-add-sample   \ act smpl

    1 1 sample-new                  \ act smpl smpl2
    dup #3 pick action-add-sample   \ act smpl smpl2
    dup #3 pick action-add-sample   \ act smpl smpl2
    dup #3 pick action-add-sample   \ act smpl smpl2
    dup #3 pick action-add-sample   \ act smpl smpl2

    cr #2 pick .action cr

    #2 #15 sample-new               \ act smpl smpl2 smpl3
    dup #4 pick action-add-sample   \ act smpl smpl2 smpl3

    cr #3 pick .action cr
    #3 pick action-get-incompatible-pairs
    \ cr ." incompat pairs " dup .region-list
     list-get-length
    #2 <>
    if
        cr ." list length not 2?"
        abort
    then

    #3 pick action-get-logical-structure list-get-length
    #6 <>
    if
        cr ." list length not 6?" #3 pick action-get-logical-structure .region-list
        abort
    then

    #3 #15 sample-new               \ act smpl smpl2 smpl3 smpl4
    dup #5 pick action-add-sample   \ act smpl smpl2 smpl3 smpl4

    #4 #15 sample-new               \ act smpl smpl2 smpl3 smpl4 smpl5
    dup #6 pick action-add-sample   \ act smpl smpl2 smpl3 smpl4 smpl5

    cr #5 pick .action cr

    #5 pick action-get-incompatible-pairs list-get-length
    1 <>
    if
        cr ." list length not 1?"
        abort
    then

    #5 pick action-get-logical-structure
    \ cr ." ls: " dup .region-list
    list-get-length
    #5 <>
    if
        cr ." list length not 5?"
        abort
    then

    sample-deallocate
    sample-deallocate
    sample-deallocate
    sample-deallocate
    sample-deallocate

    #3 #5 sample-new                \ act smpl
    dup #2 pick action-add-sample   \ act smpl

    #2 #5 sample-new                \ act smpl smpl
    dup #3 pick action-add-sample   \ act smpl smpl

    sample-deallocate
    sample-deallocate

    dup cr .action cr

    #2 #2 sample-new                \ act smpl
    dup #2 pick action-add-sample   \ act smpl

    over cr .action cr

    sample-deallocate
    drop
    current-session-deallocate

    cr ." action-test-add-sample: Ok"
;

: action-test-discover-corner-needs
    \ Init session.
    current-session-new                             \ ses

    \ Init domain 0.
    #4 over domain-new                              \ ses dom0
    2dup swap                                       \ ses dom0 dom0 sess
    session-add-domain                              \ ses dom0

    \ Get act0.
    0 over domain-find-action                       \ ses dom0, act0 t | f
    is-false abort" act0 not found?"

    \ Add arbitrary samples

    \ Square 1.
    #1 #1 sample-new                                \ ses dom0 act0 smpl1
    2dup swap action-add-sample                     \ ses dom0 act0 smpl1
    2dup swap action-add-sample                     \ ses dom0 act0 smpl1
    2dup swap action-add-sample                     \ ses dom0 act0 smpl1
    2dup swap action-add-sample                     \ ses dom0 act0 smpl1
    sample-deallocate                               \ ses dom0 act0

    1 over action-find-square
    is-false abort" square 1 not found?"
    cr .square 

    \ Square 3.
    #2 #3 sample-new                                \ ses dom0 act0 smpl3
    2dup swap action-add-sample                     \ ses dom0 act0 smpl3
    2dup swap action-add-sample                     \ ses dom0 act0 smpl3
    2dup swap action-add-sample                     \ ses dom0 act0 smpl3
    2dup swap action-add-sample                     \ ses dom0 act0 smpl3
    sample-deallocate                               \ ses dom0 act0

    3 over action-find-square
    is-false abort" square 3 not found?"
    cr .square 

    \ Square 4.
    #4 #4 sample-new                                \ ses dom0 act0 smpl4
    2dup swap action-add-sample                     \ ses dom0 act0 smpl4
    2dup swap action-add-sample                     \ ses dom0 act0 smpl4
    2dup swap action-add-sample                     \ ses dom0 act0 smpl4
    2dup swap action-add-sample                     \ ses dom0 act0 smpl4
    sample-deallocate                               \ ses dom0 act0

    4 over action-find-square
    is-false abort" square 4 not found?"
    cr .square 

    \ Square C.
    #8 $C sample-new                                \ ses dom0 act0 smplC
    2dup swap action-add-sample                     \ ses dom0 act0 smplC
    2dup swap action-add-sample                     \ ses dom0 act0 smplC
    2dup swap action-add-sample                     \ ses dom0 act0 smplC
    2dup swap action-add-sample                     \ ses dom0 act0 smplC
    sample-deallocate                               \ ses dom0 act0

    $C over action-find-square
    is-false abort" square C not found?"
    cr .square 

    cr dup .action cr

    action-discover-corner-needs                    \ sess dom0 act0 ned-lst'

    cr ." needs: " dup .need-list cr

    \ Clean up.
    need-list-deallocate
    2drop
    current-session-deallocate

    cr ." action-test-discover-corner-needs: Ok" cr
;

: action-tests
    action-test-add-sample
    \ action-test-discover-corner-needs
;
