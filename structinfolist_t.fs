: structinfolist-test-struct-list
    \ Init session.
    session-new             \ ses

    \ Init domain 0.
    #4 over domain-new dup  \ ses dom0 dom0
    #2 pick                 \ ses dom0 dom0 sess
    session-add-domain      \ ses dom0

    list-new                \ ses dom0 lst0
    list-new                \ ses dom0 lst0 lst1

    #5 #6 region-new        \ ses dom0 lst0 lst1 region
    over list-push-struct   \ ses dom0 lst0 lst1

    #3 #4 sample-new        \ ses dom0 lst0 lst1 smpl
    over list-push-struct   \ ses dom0 lst0 lst1

    over list-push-struct   \ ses dom0 lst0

    #4 #6 rule-new          \ ses dom0 lst0 rul
    over list-push-struct   \ ses dom0 lst0

    #7 over list-push       \ ses dom0 lst0

    cr cr ." list: "
    dup structinfo-list-print-struct-list
    cr

    \ Finish.
    \ cr structinfo-list-store structinfo-list-print-memory-use cr

    \ Deallocate remaining struct instances.
    cr ." Deallocating ..."

    \ project items deallocate
    structinfo-list-deallocate-struct-list

    drop

    \ cr structinfo-list-store structinfo-list-print-memory-use cr

    session-deallocate

    cr ." structinfolist-test-struct-list: Ok" cr
;

: structinfolist-tests
    structinfolist-test-struct-list
;
