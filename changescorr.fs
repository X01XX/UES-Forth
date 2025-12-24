\ Implement a struct and functions for a list of changes, corresponding to domains.

#53173 constant changescorr-id
    #2 constant changescorr-struct-number-cells

\ Struct fields
0                                 constant changescorr-header-disp  \ 16-bits [0] struct id [1] use count.
changescorr-header-disp   cell+   constant changescorr-list-disp    \ Changes list corresponding, in bits used, to the session domain list.


0 value changescorr-mma \ Storage for changescorr mma instance.

\ Init changencorr mma, return the addr of allocated memory.
: changescorr-mma-init ( num-items -- ) \ sets changescorr-mma.
    dup 1 < 
    abort" changescorr-mma-init: Invalid number of items."

    cr ." Initializing ChangesCorr store."
    changescorr-struct-number-cells swap mma-new to changescorr-mma
;

\ Check changescorr mma usage.
: assert-changescorr-mma-none-in-use ( -- )
    changescorr-mma mma-in-use 0<> 
    abort" changescorr-mma use GT 0"
;

\ Check instance type.
: is-allocated-changescorr ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup changescorr-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        changescorr-id =
    else
        drop false
    then
;

\ Check TOS for changescorr, unconventional, leaves stack unchanged.
: assert-tos-is-changescorr ( tos -- tos )
    dup is-allocated-changescorr
    is-false if
        s" TOS is not an allocated changescorr"
        .abort-xt execute
    then
;

\ Check NOS for changescorr, unconventional, leaves stack unchanged.
: assert-nos-is-changescorr ( nos tos -- nos tos )
    over is-allocated-changescorr
    is-false if
        s" NOS is not an allocated changescorr"
        .abort-xt execute
    then
;

\ Check 3OS for changescorr, unconventional, leaves stack unchanged.
: assert-3os-is-changescorr ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-changescorr
    is-false if
        s" 3OS is not an allocated changescorr"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the changescorr list field from a changescorr instance.
: changescorr-get-list ( cngsc0 -- lst )
    \ Check arg.
    assert-tos-is-changescorr

    changescorr-list-disp +     \ Add offset.
    @                           \ Fetch the field.
;

\ Set the changescorr list field of a changescorr instance, use only in this file.
: _changescorr-set-list ( lst1 cngsc0 -- )
    \ Check args.
    assert-tos-is-changescorr
    assert-nos-is-list

    \ Store list
    over struct-inc-use-count

    changescorr-list-disp +    \ Add offset.
    !                          \ Set first field.
;

\ End accessors.

\ Create a changescorr-list-corr from a changescorr-list-corr-list on the stack.
: changescorr-new ( lst0 -- addr)
    \ check arg.
    assert-tos-is-changes-list
    dup list-get-length number-domains <> abort" changescorr-new: invalid list length?"

    \ Allocate space.
    changescorr-mma mma-allocate   \ lst0 cngsc

    \ Store id.
    changescorr-id over            \ lst0 cngsc id cngsc
    struct-set-id                  \ lst0 cngsc

    \ Init use count.
    0 over struct-set-use-count    \ lst0 cngsc

    tuck                           \ cngsc lst0 cngsc
    _changescorr-set-list          \ cngsc
;

\ Print a changescorr list, corresponding to the session domain list.
: .changescorr ( chgsc0 -- )
    \ Check arg.
    assert-tos-is-changescorr

    changescorr-get-list            \ lst
    list-get-links                  \ link0
    cur-session-get-domain-list-xt      \ link0 xt
    execute                         \ link0 dom-lst 
    list-get-links                  \ link0 d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ link0 d-link domx
        domain-set-current-xt
        execute                     \ link0 d-link

        over link-get-data          \ link0 d-link reg0
        .changes                    \ link0 d-link

        swap link-get-next          \ d-link link0
        swap link-get-next          \ link0 d-link
        dup if
            space
        then
    repeat
                                    \ link0
    drop
    ." )"
;

\ Deallocate the given changescorr, if its use count is 1 or 0.
: changescorr-deallocate ( chgsc0 -- )
    \ Check arg.
    assert-tos-is-changescorr

    dup struct-get-use-count            \ cngsc0 count

    #2 <
    if
        \ Deallocate fields.
        dup changescorr-get-list   \ cngs0 cngsc-lst
        changes-list-deallocate

        \ Deallocate instance.
        changescorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: changescorr-list-deallocate ( chgsc-lst0 -- )
    \ Check arg.
    assert-tos-is-changescorr

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate changes instances in the list.
        [ ' changescorr-deallocate ] literal over     \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list. 
        list-deallocate                            \
    else
        struct-dec-use-count
    then
;

\ Deallocate a list of lists of changescorr.
: changescorr-lol-deallocate ( cngsc-lst-lol0 -- )
    \ Check arg.
    assert-tos-is-changescorr

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                            \ lst0 uc
    #2 < if
        \ Deallocate changescorr instances in the list.
        [ ' changescorr-list-deallocate ] literal over  \ lst0 xt lst0
        list-apply                                      \ lst0

        \ Deallocate the list. 
        list-deallocate                                 \
    else
        struct-dec-use-count
    then
;

\ Return true if at least one bit set-to-1 is the same between two changescorr.
: changescorr-intersect ( cngsc1 cngsc0 -- bool )
    \ Check args.
    assert-tos-is-changescorr
    assert-nos-is-changescorr

    changescorr-get-list list-get-links swap
    changescorr-get-list list-get-links         \ link0 link1 ( order of links does not matter )

    begin
        ?dup
    while
        over link-get-data                      \ link link cngsc
        over link-get-data                      \ link link cngsc cngsc
        changes-intersect                       \ link link bool
        if
            2drop
            true
            exit
        then

        link-get-next swap
        link-get-next
    repeat
                                                \ link
    drop
    false
;

\ Return changes needed to traverse from one regioncorr to another.
: changescorr-new-regc-to-regc ( regc-to regc-from -- cngsc )
    \ Check args.
    assert-tos-is-regioncorr-xt execute
    assert-nos-is-regioncorr-xt execute

    \ Init changesscorr list.
    list-new -rot                   \ cngsc-lst regc-to regc-from

    \ Prep for loop.
    regioncorr-get-list-xt execute list-get-links swap      \ cngs-lst link-from regc-to
    regioncorr-get-list-xt execute list-get-links swap      \ cngs-lst link-to link-from
    cur-session-get-domain-list-xt execute list-get-links   \ cngs-lst link-to link-from dom-link

    begin
        ?dup
    while

        \ Prep for current list items.
        #2 pick link-get-data           \ cngs-lst link-to link-from dom-link reg-to
        #2 pick link-get-data           \ cngs-lst link-to link-from dom-link reg-to reg-from
        #2 pick link-get-data           \ cngs-lst link-to link-from dom-link reg-to reg-from domx
        current-session                 \ cngs-lst link-to link-from dom-link reg-to reg-from domx sess
        session-set-current-domain-xt   \ cngs-lst link-to link-from dom-link reg-to reg-from domx sess xt
        execute                         \ cngs-lst link-to link-from dom-link reg-to reg-from

        \ Get changes needed for item, store same.
        changes-new-region-to-region    \ cngs-lst link-to link-from dom-link cngs
        #4 pick                         \ cngs-lst link-to link-from dom-link cngs cngs-lst
        changes-list-push-end           \ cngs-lst link-to link-from dom-link

        \ Inc links.
        link-get-next rot
        link-get-next rot
        link-get-next rot
    repeat
                                        \ cngs-lst link-to link-from
    2drop                               \ cngs-lst

    changescorr-new
;

\ Return the number of changes in a changescorr.
: changescorr-number-changes ( cngsc0 -- u )
    \ Check arg.
    assert-tos-is-changescorr

    \ Init counter.
    0 swap                      \ cnt cngsc

    \ Prep for loop.
    changescorr-get-list        \ cnt cng-lst
    list-get-links              \ cnt link

    begin
        ?dup
    while
        \ Get the number of changes in one item.
        dup link-get-data       \ cnt link cngs
        changes-number-changes  \ cnt link num

        \ Add to change counter.
        rot                     \ link num cnt
        +                       \ link cnt+
        swap                    \ cnt+ link

        link-get-next
    repeat
;

\ Return the inversion of a changescorr.
: changescorr-invert ( cngsc0 -- cngsc )
    \ Check arg.
    assert-tos-is-changescorr

    \ Init changes list.
    list-new swap           \ cngs-lst cngsc0

    \ Prep for loop.
    changescorr-get-list list-get-links \ cngs-lst cngs-link
    cur-session-get-domain-list-xt      \ cngs-lst cngs-link xt
    execute list-get-links              \ cngs-lst cngs-link d-link

    begin
        ?dup
    while

        \ Set current domain.
        dup link-get-data               \ cngs-lst cngs-link d-link domx
        current-session                 \ cngs-lst cngs-link d-link domx sess
        session-set-current-domain-xt   \ cngs-lst cngs-link d-link domx sess xt
        execute                         \ cngs-lst cngs-link d-link

        \ Get changes item.
        over link-get-data              \ cngs-lst cngs-link d-link cngsx

        \ Invert changes item.
        changes-invert                  \ cngs-lst cngs-link d-link cngs-inv

        \ Add to list.
        #3 pick                         \ cngs-lst cngs-link d-link cngs-inv cngs-lst
        changes-list-push-end           \ cngs-lst cngs-link d-link

        link-get-next swap
        link-get-next swap
    repeat
                                        \ cngs-lst cngs-link
    drop                                \ cngs-lst
    changescorr-new
;

\ Return the intersection of two changescorr.
: changescorr-intersection ( cngsc1 cngsc0 -- cngsc )
    \ Check args.
    assert-tos-is-changescorr
    assert-nos-is-changescorr

    \ Init changes list.
    list-new -rot                               \ cngs-lst cngsc1 cngsc0
    changescorr-get-list list-get-links swap
    changescorr-get-list list-get-links         \ cngs-lst link0 link1 ( order of links does not matter )

    begin
        ?dup
    while
        \ Get changes items.
        over link-get-data                      \ cngs-lst link0 link1 cngsc0
        over link-get-data                      \ cngs-lst link0 link1 cngsc0 cngsc1

        \ Intersect changes items.
        changes-intersection                    \ cngs-lst link0 link1 cngs-int

        \ Store intersection.
        #3 pick                                 \ cngs-lst link0 link1 cngs-int cngs-lst
        changes-list-push-end                   \ cngs-lst link0 link1

        link-get-next swap
        link-get-next
    repeat
                                                \ cngs-lst link0
    drop                                        \ cngs-lst
    changescorr-new
;
