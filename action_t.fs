\ Tests for action struct functions.

\ Test adding samples and changing the logical structure, incompatible pairs.
: action-test-add-sample
    cr ." depth s1: " depth . cr
    4                               \ 4
    [ ' act-0-get-sample ] literal  \ 4 xt
    action-new                      \ act
    cr dup .action cr

    4 5 sample-new                  \ act smpl
    dup 2 pick action-add-sample    \ act smpl

    1 1 sample-new                  \ act smpl smpl2
    dup 3 pick action-add-sample    \ act smpl smpl2
    dup 3 pick action-add-sample    \ act smpl smpl2
    dup 3 pick action-add-sample    \ act smpl smpl2
    dup 3 pick action-add-sample    \ act smpl smpl2

    cr 2 pick .action cr

    2 15 sample-new                 \ act smpl smpl2 smpl3
    dup 4 pick action-add-sample    \ act smpl smpl2 smpl3

    cr 3 pick .action cr
    3 pick action-get-incompatible-pairs
    \ cr ." incompat pairs " dup .region-list
     list-get-length
    2 <>
    if
        cr ." list length not 2?"
        abort
    then

    3 pick action-get-logical-structure list-get-length
    6 <>
    if
        cr ." list length not 6?" 3 pick action-get-logical-structure .region-list
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

    5 pick action-get-logical-structure
    \ cr ." ls: " dup .region-list
    list-get-length
    5 <>
    if
        cr ." list length not 5?"
        abort
    then

    sample-deallocate
    sample-deallocate
    sample-deallocate
    sample-deallocate
    sample-deallocate

    3 5 sample-new                  \ act smpl
    dup 2 pick action-add-sample    \ act smpl

    2 5 sample-new                  \ act smpl smpl
    dup 3 pick action-add-sample    \ act smpl smpl

    sample-deallocate
    sample-deallocate

    dup cr .action cr

    2 2 sample-new                  \ act smpl
    dup 2 pick action-add-sample    \ act smpl

    over cr .action cr

    sample-deallocate
    action-deallocate
    cr ." depth 1: " depth . cr
;

: action-tests
    action-test-add-sample
;
