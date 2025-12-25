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
: plancorr-get-list ( rulc0 -- rul-lst )
    \ Check arg.
    assert-tos-is-plancorr

    plancorr-list-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the plancorr list field of a plan instance, use only in this file.
: _plancorr-set-list ( lst1 rulc0 -- )
    \ Check args.
    assert-tos-is-plancorr

    \ Store list
    over struct-inc-use-count
    plancorr-list-disp +    \ Add offset.
    !                       \ Set first field.
;

\ End accessors.

\ Create a plancorr-list-corr from a plancorr-list-corr-list on the stack.
: plancorr-new ( rul-lst0 -- rulc )
    \ check arg.
    assert-tos-is-plan-list
    dup list-get-length number-domains <> abort" plancorr-new: invalid list length?"

    \ Allocate space.
    plancorr-mma mma-allocate   \ rul-lst0 rulc

    \ Store id.
    plancorr-id over            \ rul-lst0 rulc id rulc
    struct-set-id               \ rul-lst0 rulc

    \ Init use count.
    0 over struct-set-use-count \ rul-lst0 rulc

    tuck                        \ rulc rul-lst0 rulc
    _plancorr-set-list          \ rulc
;

\ Print a plan-list corresponding to the session domain list.
: .plancorr ( rulc0 -- )
    \ Check arg.
    assert-tos-is-plancorr

    plancorr-get-list               \ rul-lst
    list-get-links                  \ rc-link
    cur-session-get-domain-list-xt      \ rc-link xt
    execute                         \ rc-link dom-lst 
    list-get-links                  \ rc-link d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ rc-link d-link domx
        domain-set-current-xt
        execute                     \ rc-link d-link

        over link-get-data          \ rc-link d-link reg0
        .plan                       \ rc-link d-link

        swap link-get-next          \ d-link rc-link
        swap link-get-next          \ rc-link d-link
        dup if
            space
        then
    repeat
                                    \ rc-link
    drop
    ." )"
;

\ Deallocate the given plancorr, if its use count is 1 or 0.
: plancorr-deallocate ( rulc0 -- )
    \ Check arg.
    assert-tos-is-plancorr

    dup struct-get-use-count        \ rulc0 count

    #2 <
    if
        \ Deallocate fields.
        dup plancorr-get-list       \ rulc0 rul-lst
        plan-list-deallocate        \ rulc0

        \ Deallocate instance.
        plancorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;


