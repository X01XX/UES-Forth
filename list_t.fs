: list-test-one-of-each
    \ Init list.
    list-new            \ lst

    \ Add third list, (3 4)
    list-new            \ lst sub
    #4 over list-push   \ lst sub
    #3 over list-push   \ lst sub
    over list-push      \ lst

    \ Add second list, (1 2)
    list-new            \ lst sub
    #2 over list-push   \ lst sub
    #1 over list-push   \ lst sub
    over list-push      \ lst

    \ Add first list, (0)
    list-new            \ lst sub
    0 over list-push    \ lst sub
    over list-push      \ lst

    cr ." list:  " [ ' . ] literal over .list cr

    dup list-one-of-each    \ lst lst2

    cr ." list2: " [ ' . ] literal over .list cr

    \ Check results.
    dup list-get-length #4 <> abort" List len not 4?"

    list-deallocate
    list-deallocate

    cr ." list-test-one-of-each: Ok" cr
;

: list-tests
    list-test-one-of-each
;

