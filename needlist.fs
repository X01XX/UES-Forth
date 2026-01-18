\ Functions for need lists.

\ Check if tos is a list, if non-empty, with the first item being a need.
: assert-tos-is-need-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-need
        drop
    then
;

\ Check if nos is a list, if non-empty, with the first item being a need.
: assert-nos-is-need-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-need
        drop
    then
;

\ Deallocate a need list.
: need-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-need-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate need instances in the list.
        [ ' need-deallocate ] literal over          \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Return the union of two need lists.
: need-list-set-union ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-need-list
    assert-nos-is-need-list

    [ ' need-eq ] literal -rot        \ xt list1 list0
    list-union-struct                 \ list-result
;


\ Print a need-list
: .need-list ( list0 -- )
    \ Check args.
    assert-tos-is-need-list

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
    assert-tos-is-need-list
    assert-nos-is-need

    list-push-struct
;

\ Append nos need-list to the tos need-list.
: need-list-append ( lst1 lst0 -- )
    \ Check args.
    assert-tos-is-need-list
    assert-nos-is-need-list

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
