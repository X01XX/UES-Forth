\ Implement a regioncorr-rate struct and functions.
\
\ The regioncorrrate represents a Region-corr, and a rate.
\
\ (x1x1 1x1x1) means x1x1 and 1x1x1.
\ (x1x1 xxxxx), (xxxx 1x1x1) means x1x1 or 1x1x1.

#41737 constant regioncorrrate-id
    #3 constant regioncorrrate-struct-number-cells

\ Struct fields
0                                  constant regioncorrrate-header-disp      \ 16-bits [0] struct id, [1] use count.
regioncorrrate-header-disp  cell+  constant regioncorrrate-rate-disp        \ A rate, given by the user, or calculated by adding the rates of superset regioncorrrates given by the user.
regioncorrrate-rate-disp    cell+  constant regioncorrrate-regioncorr-disp  \ A regionCorr struct instance.

0 value regioncorrrate-mma \ Storage for regioncorrrate mma instance.

\ Init regioncorrrate mma, return the addr of allocated memory.
: regioncorrrate-mma-init ( num-items -- ) \ sets regioncorrrate-mma.
    dup 1 <
    abort" regioncorrrate-mma-init: Invalid number of items."

    cr ." Initializing regioncorrrate store."
    regioncorrrate-struct-number-cells swap mma-new to regioncorrrate-mma
;

\ Check regioncorrrate mma usage.
: assert-regioncorrrate-mma-none-in-use ( -- )
    regioncorrrate-mma mma-in-use 0<>
    abort" regioncorrrate-mma use GT 0"
;

\ Check instance type.
: is-allocated-regioncorrrate ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup regioncorrrate-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        regioncorrrate-id =
    else
        drop false
    then
;

\ Check TOS for regioncorrrate, unconventional, leaves stack unchanged.
: assert-tos-is-regioncorrrate ( tos -- tos )
    dup is-allocated-regioncorrrate
    is-false if
        s" TOS is not an allocated regioncorrrate"
        .abort-xt execute
    then
;

\ Check NOS for regioncorrrate, unconventional, leaves stack unchanged.
: assert-nos-is-regioncorrrate ( nos tos -- nos tos )
    over is-allocated-regioncorrrate
    is-false if
        s" NOS is not an allocated regioncorrrate"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the first field from a regioncorrrate instance.
: regioncorrrate-get-rate ( addr -- u)
    \ Check arg.
    assert-tos-is-regioncorrrate

    regioncorrrate-rate-disp +  \ Add offset.
    @                           \ Fetch the field.
;

\ Return the second field from a regioncorrrate instance.
: regioncorrrate-get-regioncorr ( addr -- u)
    \ Check arg.
    assert-tos-is-regioncorrrate

    \ Get second state.
    regioncorrrate-regioncorr-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the first field from a regioncorrrate instance, use only in this file.
: _regioncorrrate-set-rate ( rate1 regcr0 -- )
    \ Check args.
    assert-tos-is-regioncorrrate
    assert-nos-is-rate

    regioncorrrate-rate-disp +  \ Add offset.
    !                           \ Set first field.
;

\ Set the second field from a regioncorrrate instance, use only in this file.
: _regioncorrrate-set-regioncorr ( regc1 regcr0 -- )
    \ Check args.
    assert-tos-is-regioncorrrate
    assert-nos-is-regioncorr

    regioncorrrate-regioncorr-disp +    \ Add offset.
    !                                   \ Set second field.
;

\ End accessors.

\ Create a regioncorrrate from a n regioncorr and rate on the stack.
: regioncorrrate-new ( regc1 rate -- addr )
    \ Check args.
    assert-tos-is-rate
    assert-nos-is-regioncorr

    \ Allocate space.
    regioncorrrate-mma mma-allocate     \ regc1 rate regcr

    \ Store id.
    regioncorrrate-id over              \ regc1 rate regcr id regcr
    struct-set-id                       \ regc1 rate regcre

    \ Init use count.
    0 over struct-set-use-count         \ regc1 rate regcr

    \ Store rate.
    swap                                \ regc1 regcr rate
    dup struct-inc-use-count            \ regc1 regcre rate
    over _regioncorrrate-set-rate       \ regc1 regcr

    \ Store regioncorr.
    swap                                \ regcr regc1
    dup struct-inc-use-count            \ regcr regc1
    over _regioncorrrate-set-regioncorr \ regcr
;

\ ' regioncorrrate-new to regioncorrrate-new-xt

\ Print a regioncorrrate.
: .regioncorrrate ( regcr0 -- )
    \ Check arg.
    assert-tos-is-regioncorrrate

    ." ["
    dup regioncorrrate-get-rate .rate
    ." , "
    regioncorrrate-get-regioncorr
    .regioncorr
    ." ]"
;

' .regioncorrrate to .regioncorrrate-xt

\ Deallocate a regioncorrrate.
: regioncorrrate-deallocate ( regcr0 -- )
    \ Check arg.
    assert-tos-is-regioncorrrate

    dup struct-get-use-count                \ regcr0 count

    #2 <
    if                                      \ regcr0
        \ Deallocate rate.
        dup regioncorrrate-get-rate         \ regcr0 rt
        rate-deallocate                     \ regcr0

        \ Deallocate regioncorr
        dup regioncorrrate-get-regioncorr   \ regcr0 regc
        regioncorr-deallocate               \ regcr0

        \ Deallocate instance.
        regioncorrrate-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: regioncorrrate-rate-all-zero ( regcr0 -- bool )
    \ Check arg.
    assert-tos-is-regioncorrrate

    regioncorrrate-get-rate \ rate
    rate-all-zero           \ bool
;

: regioncorrrate-eq ( regcr1 regcr0 -- bool )
    \ Check args.
    assert-tos-is-regioncorrrate
    assert-nos-is-regioncorrrate

    \ Get regioncorr part of regioncorrrate.
    regioncorrrate-get-regioncorr   \ regctr1 regc0
    swap                            \ regc0 regcr1
    regioncorrrate-get-regioncorr   \ regc0 regc1

    regioncorr-eq
;
