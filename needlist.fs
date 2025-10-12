\ Functions for need lists.

\ Deallocate a need list.
: need-list-deallocate ( list0 -- )
    [ ' need-deallocate ] literal over list-apply   \ Deallocate need instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

\ Return the union of two need lists.
: need-list-set-union ( list1 list0 -- list-result )
    [ ' need-eq ] literal -rot        \ xt list1 list0
    list-union                          \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;


\ Print a need-list
: .need-list ( list0 -- )
    \ Check args.
    assert-tos-is-list

    \ Init counter
    0 swap                  \ cnt link

    \ Scan needs
    list-get-links
    begin
        ?dup
    while
        dup link-get-data   \ cnt link ned

        \ Print need and count.
        cr
        #2 pick #3 dec.r
        space
        .need

        \ Update count.
        swap 1+ swap

        link-get-next       \ cnt link
    repeat
    cr
                            \ cnt
    drop
;

\ Push a need to a need-list, unless it is already in the list.
: need-list-push ( ned1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-need

    over struct-inc-use-count
    list-push
;

\ Append nos need-list to the tos need-list.
: need-list-append ( lst1 lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    swap                    \ lst0 lst1
    list-get-links          \ lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ lst0 link nedx
        #2 pick             \ lst0 link nedx lst0
        need-list-push      \ lst0 link

        link-get-next
    repeat
                        \ lst0
    drop
;
