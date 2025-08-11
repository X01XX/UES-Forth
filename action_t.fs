\ Tests for action struct functions.

\ Test adding samples and changing the logical structure, incompatible pairs.
: action-test-add-sample
    test-none-in-use

    5 action-new                    \ act
    cr dup .action cr

    4 5 sample-new                  \ act smpl
    dup 2 pick action-add-sample    \ act smpl

    0 6 sample-new                  \ act smpl smpl2
    dup 3 pick action-add-sample    \ act smpl smpl2

    cr 2 pick .action cr

    2 15 sample-new                 \ act smpl smpl2 smpl3
    dup 4 pick action-add-sample    \ act smpl smpl2 smpl3

    cr 3 pick .action cr
    3 pick action-get-incompatible-pairs list-get-length
    3 <>
    if
        cr ." list length not 3?"
        abort
    then

    3 pick action-get-logical-structure list-get-length
    7 <>
    if
        cr ." list length not 7?" 3 pick action-get-logical-structure .region-list
        abort
    then

    3 15 sample-new                 \ act smpl smpl2 smpl3 smpl4
    dup 5 pick action-add-sample    \ act smpl smpl2 smpl3 smpl4

    4 15 sample-new                 \ act smpl smpl2 smpl3 smpl4 smpl5
    dup 6 pick action-add-sample    \ act smpl smpl2 smpl3 smpl4 smpl5

    cr 5 pick .action cr

    5 pick action-get-incompatible-pairs list-get-length
    1 <>
    if
        cr ." list length not 1?"
        abort
    then

    5 pick action-get-logical-structure list-get-length
    6 <>
    if
        cr ." list length not 6?"
        abort
    then

    sample-deallocate
    sample-deallocate
    sample-deallocate
    sample-deallocate
    sample-deallocate
    action-deallocate

    test-none-in-use
;

: action-tests
    action-test-add-sample
;
