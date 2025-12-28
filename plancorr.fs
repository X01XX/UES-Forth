\ Implement a struct and functions for a plancorr, a list of plans corresponding to the list of domains.

#53717 constant plancorr-id
    #2 constant plancorr-struct-number-cells

\ Struct fields
0                              constant plancorr-header-disp    \ 16-bits [0] struct id [1] use count.
plancorr-header-disp   cell+   constant plancorr-list-disp      \ plan list corresponding, in bits used, to the session domain list.

0 value plancorr-mma \ Storage for plan mma instance.

\ Init plan mma, return the addr of allocated memory.
: plancorr-mma-init ( num-items -- ) \ sets plancorr-mma.
    dup 1 <
    abort" plancorr-mma-init: Invalid number of items."

    cr ." Initializing PlanCorr store."
    plancorr-struct-number-cells swap mma-new to plancorr-mma
;

\ Check plan mma usage.
: assert-plancorr-mma-none-in-use ( -- )
    plancorr-mma mma-in-use 0<>
    abort" plancorr-mma use GT 0"
;

\ Check instance type.
: is-allocated-plancorr ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup plancorr-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        plancorr-id =
    else
        drop false
    then
;

\ Check TOS for plancorr, unconventional, leaves stack unchanged.
: assert-tos-is-plancorr ( tos -- tos )
    dup is-allocated-plancorr
    is-false if
        s" TOS is not an allocated plancorr"
        .abort-xt execute
    then
;

\ Check NOS for plancorr, unconventional, leaves stack unchanged.
: assert-nos-is-plancorr ( nos tos -- nos tos )
    over is-allocated-plancorr
    is-false if
        s" NOS is not an allocated plancorr"
        .abort-xt execute
    then
;

\ Check 3OS for plancorr, unconventional, leaves stack unchanged.
: assert-3os-is-plancorr ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-plancorr
    is-false if
        s" 3OS is not an allocated plancorr"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the plancorr list field from a plan instance.
: plancorr-get-list ( plnc0 -- pln-lst )
    \ Check arg.
    assert-tos-is-plancorr

    plancorr-list-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the plancorr list field of a plan instance, use only in this file.
: _plancorr-set-list ( pln-lst1 plnc0 -- )
    \ Check args.
    assert-tos-is-plancorr

    \ Store list
    over struct-inc-use-count
    plancorr-list-disp +    \ Add offset.
    !                       \ Set first field.
;

\ End accessors.

\ Create a plancorr-list-corr from a plancorr-list-corr-list on the stack.
: plancorr-new ( pln-lst0 -- plnc )
    \ check arg.
    assert-tos-is-plan-list
    dup list-get-length number-domains <> abort" plancorr-new: invalid list length?"

    \ Allocate space.
    plancorr-mma mma-allocate   \ pln-lst0 plnc

    \ Store id.
    plancorr-id over            \ pln-lst0 plnc id plnc
    struct-set-id               \ pln-lst0 plnc

    \ Init use count.
    0 over struct-set-use-count \ pln-lst0 plnc

    tuck                        \ plnc pln-lst0 plnc
    _plancorr-set-list          \ plnc
;

\ Print a plan-list corresponding to the session domain list.
: .plancorr ( plnc0 -- )
    \ Check arg.
    assert-tos-is-plancorr

    plancorr-get-list               \ pln-lst
    list-get-links                  \ plnc-link
    cur-session-get-domain-list-xt  \ plnc-link xt
    execute                         \ plnc-link dom-lst
    list-get-links                  \ plnc-link d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ plnc-link d-link domx
        domain-set-current-xt
        execute                     \ plnc-link d-link

        over link-get-data          \ plnc-link d-link reg0
        .plan                       \ plnc-link d-link

        swap link-get-next          \ d-link plnc-link
        swap link-get-next          \ plnc-link d-link
        dup if
            space
        then
    repeat
                                    \ plnc-link
    drop
    ." )"
;

\ Deallocate the given plancorr, if its use count is 1 or 0.
: plancorr-deallocate ( plnc0 -- )
    \ Check arg.
    assert-tos-is-plancorr

    dup struct-get-use-count        \ plnc0 count

    #2 <
    if
        \ Deallocate fields.
        dup plancorr-get-list       \ plnc0 pln-lst
        plan-list-deallocate        \ plnc0

        \ Deallocate instance.
        plancorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return result regions of a plancorr.
: plancorr-calc-result-regions ( plnc -- regc )
    \ Check arg.
    assert-tos-is-plancorr

    \ Init reg list.
    list-new swap                   \ reg-lst plsc

    \ Prep for loop.
    plancorr-get-list               \ reg-lst pln-lst
    list-get-links                  \ reg-lst plnc-link
    cur-session-get-domain-list-xt  \ reg-lst plnc-link xt
    execute                         \ reg-lst plnc-link dom-lst
    list-get-links                  \ reg-lst plnc-link d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ reg-lst plnc-link d-link domx
        domain-set-current-xt
        execute                     \ reg-lst plnc-link d-link

        \ Get planx result.
        over link-get-data          \ reg-lst plnc-link d-link plnx
        plan-get-result-region      \ reg-lst plnc-link d-link regx

        \ Store region.
        #3 pick                     \ reg-lst plnc-link d-link regx reg-lst
        region-list-push-end        \ reg-lst plnc-link d-link

        swap link-get-next
        swap link-get-next
    repeat
                                    \ reg-lst plnc-link
    drop                            \ reg-lst
    regioncorr-new
;

\ Run a platcorr.
: plancorr-run ( plnc -- bool )
    \ Check arg.
    assert-tos-is-plancorr

    \ Prep for loop.
    plancorr-get-list               \  pln-lst
    list-get-links                  \  plnc-link
    cur-session-get-domain-list-xt  \  plnc-link xt
    execute                         \  plnc-link dom-lst
    list-get-links                  \  plnc-link d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \  plnc-link d-link domx
        domain-set-current-xt
        execute                     \  plnc-link d-link

        \ Get planx result.
        over link-get-data          \  plnc-link d-link plnx

        \ Run plan.
        plan-run                    \ plnc-link d-link, t | f
        is-false if
            2drop
            false
            exit
        then

        swap link-get-next
        swap link-get-next
    repeat
                                    \  plnc-link
    drop                            \
    true
;
