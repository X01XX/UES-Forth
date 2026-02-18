: structinfolist-test-struct-list
    \ Init session.
    current-session-new                             \ ses

    \ Init domain 0.
    #4 current-session domain-new dup               \ ses dom0 dom0
    #2 pick                                         \ ses dom0 dom0 sess
    session-add-domain                              \ ses dom0

    list-new                \ lst0
    list-new                \ lst0 lst1

    #5 #6 region-new        \ lst0 lst1 region
    over list-push-struct   \ lst0 lst1

    #3 #4 sample-new        \ lst0 lst1 smpl
    over list-push-struct   \ lst0 lst1

    over list-push-struct   \ lst0

    #4 #6 rule-new          \ lst0 rul
    over list-push-struct   \ lst0

    #7 over list-push       \ lst0

    cr cr ." list: "
    dup structinfo-list-print-struct-list
    cr

    \ Finish.
    \ cr structinfo-list-store structinfo-list-print-memory-use cr

    \ Deallocate remaining struct instances.
    cr ." Deallocating ..."

    \ project items deallocate
    structinfo-list-deallocate-struct-list

    2drop

    \ cr structinfo-list-store structinfo-list-print-memory-use cr

    current-session-deallocate

    structinfo-list-store structinfo-list-project-deallocated

    \ Success if no abort.
    cr ." structinfolist-test-struct-list: Ok" cr
;

: structinfolist-tests
    structinfolist-test-struct-list
;
