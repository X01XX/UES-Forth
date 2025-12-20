\ Implement a struct and functions for a rule list corresponding to domains.

#53171 constant rulecorr-id
    #2 constant rulecorr-struct-number-cells

\ Struct fields
0                               constant rulecorr-header-disp   \ 16-bits [0] struct id [1] use count.
rulecorr-header-disp   cell+   constant rulecorr-list-disp     \ Rule list corresponding, in bits used, to the session domain list.


0 value rulecorr-mma \ Storage for rule mma instance.

\ Init rule mma, return the addr of allocated memory.
: rulecorr-mma-init ( num-items -- ) \ sets rulecorr-mma.
    dup 1 < 
    abort" rulecorr-mma-init: Invalid number of items."

    cr ." Initializing rulecorr store."
    rulecorr-struct-number-cells swap mma-new to rulecorr-mma
;

\ Check rule mma usage.
: assert-rulecorr-mma-none-in-use ( -- )
    rulecorr-mma mma-in-use 0<> 
    abort" rulecorr-mma use GT 0"
;

\ Check instance type.
: is-allocated-rulecorr ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup rulecorr-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    rulecorr-id =
;

\ Check TOS for rulecorr, unconventional, leaves stack unchanged.
: assert-tos-is-rulecorr ( arg0 -- arg0 )
    dup is-allocated-rulecorr
    is-false if
        s" TOS is not an allocated rulecorr"
        .abort-xt execute
    then
;

\ Check NOS for rulecorr, unconventional, leaves stack unchanged.
: assert-nos-is-rulecorr ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-rulecorr
    is-false if
        s" NOS is not an allocated rulecorr"
        .abort-xt execute
    then
;

\ Check 3OS for rulecorr, unconventional, leaves stack unchanged.
: assert-3os-is-rulecorr ( arg2 arg1 arg0 -- arg1 arg0 )
    #2 pick is-allocated-rulecorr
    is-false if
        s" 3OS is not an allocated rulecorr"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the first field from a rule instance.
: rulecorr-get-list ( rulecorr0 -- lst )
    \ Check arg.
    assert-tos-is-rulecorr

    rulecorr-list-disp +    \ Add offset.
    @                               \ Fetch the field.
;

\ Set the first field from a rule instance, use only in this file.
: _rulecorr-set-list ( lst1 rulecorr0 -- )
    \ Check args.
    assert-tos-is-rulecorr

    \ Store list
    over struct-inc-use-count

    rulecorr-list-disp +    \ Add offset.
    !                               \ Set first field.
;

\ End accessors.

\ Create a rulecorr-list-corr from a rulecorr-list-corr-list on the stack.
: rulecorr-new ( lst0 -- addr)
    \ check arg.
    assert-tos-is-list

    \ Allocate space.
    rulecorr-mma mma-allocate   \ lst0 rulecorr

    \ Store id.
    rulecorr-id over            \ lst0 rulecorr id rulecorr
    struct-set-id                       \ lst0 rulecorr

    \ Init use count.
    0 over struct-set-use-count         \ lst0 rulecorr

    tuck                                \ rulecorr lst0 rulecorr
    _rulecorr-set-list          \ rulecorr
;

\ Print a rule-list corresponding to the session domain list.
: .rulecorr ( rulecorr0 -- )
    \ Check arg.
    assert-tos-is-rulecorr

    rulecorr-get-list       \ lst
    list-get-links                  \ link0
    session-get-domain-list-xt      \ link0 xt
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
        .rule                       \ link0 d-link

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

\ Deallocate the given rulecorr, if its use count is 1 or 0.
: rulecorr-deallocate ( rulecorr0 -- )
    \ Check arg.
    assert-tos-is-rulecorr

    dup struct-get-use-count            \ rulecorr0 count

    #2 <
    if
        \ Deallocate fields.
        dup rulecorr-get-list   \ rulecorr0 rulecorr-lst
        rule-list-deallocate

        \ Deallocate instance.
        rulecorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return a rulecorr for translating from one rule-list-corr (rulecorr-from) to another (rulecorr-to).
: rulecorr-new-regioncorr-to-regioncorr ( regioncorr-to regioncorr-from -- rulecorr )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    \ Init return list.
    list-new -rot                           \ ret-lst regioncorr-to regioncorr-from

    \ Prep for loop.
    regioncorr-get-list list-get-links swap \ ret-lst link-from regioncorr-to
    regioncorr-get-list list-get-links swap \ ret-lst link-to link-from

    begin
        ?dup
    while
        over link-get-data          \ ret-lst link-to link-from reg-to
        over link-get-data          \ ret-lst link-to link-from reg-to reg-from
        rule-new-region-to-region   \ ret-lst link-to link-from rulx ( rule may have no changes )
        #3 pick                     \ ret-lst link-to link-from rulx ret-lst
        rule-list-push-end          \ ret-lst link-to link-from

        link-get-next swap
        link-get-next swap
    repeat
                                    \ ret-lst link-to
    drop                            \ ret-lst
    rulecorr-new                    \ rulecorr
;

: rulecorr-list-deallocate ( rulecorr-lst0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate rule instances in the list.
        [ ' rulecorr-deallocate ] literal over     \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list. 
        list-deallocate                            \
    else
        struct-dec-use-count
    then
;

\ Deallocate a list of lists of rulecorr.
: rulecorr-lol-deallocate ( rulecorr-lst-lol0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate rulecorr instances in the list.
        [ ' rulecorr-list-deallocate ] literal over   \ lst0 xt lst0
        list-apply                                          \ lst0

        \ Deallocate the list. 
        list-deallocate                            \
    else
        struct-dec-use-count
    then
;
