\ Struct and functions for a desired sample of state-list-corr to state-list-corr.

#47317 constant samplecorr-id
    #3 constant samplecorr-struct-number-cells

\ Struct fields
0 constant samplecorr-header        \ 16-bits [0] struct id [1] use count.
samplecorr-header       cell+ constant samplecorr-initial-disp
samplecorr-initial-disp cell+ constant samplecorr-result-disp

0 value samplecorr-mma \ Storage for samplecorr mma instance.

\ Init samplecorr mma, return the addr of allocated memory.
: samplecorr-mma-init ( num-items -- ) \ sets samplecorr-mma.
    dup 1 <
    abort" samplecorr-mma-init: Invalid number of items."

    cr ." Initializing Sample-corr store."
    samplecorr-struct-number-cells swap mma-new to samplecorr-mma
;

\ Check samplecorr mma usage.
: assert-samplecorr-mma-none-in-use ( -- )
    samplecorr-mma mma-in-use 0<>
    abort" samplecorr-mma use GT 0"
;

\ Check instance type.
: is-allocated-samplecorr ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup samplecorr-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    samplecorr-id =     
;

\ Check TOS for samplecorr, unconventional, leaves stack unchanged. 
: assert-tos-is-samplecorr ( smpl0 -- smpl0 )
    dup is-allocated-samplecorr
    is-false if
        s" TOS is not an allocated samplecorr"
        .abort-xt execute
    then
;

\ Check NOS for samplecorr, unconventional, leaves stack unchanged. 
: assert-nos-is-samplecorr ( smpl1 arg0 -- smpl1 arg0 )
    over is-allocated-samplecorr
    is-false if
        s" NOS is not an allocated samplecorr"
        .abort-xt execute
    then
;

\ Check 3OS for samplecorr, unconventional, leaves stack unchanged. 
: assert-3os-is-samplecorr ( smpl2 arg1 arg0 -- smpl2 arg1 arg0 )
    #2 pick is-allocated-samplecorr
    is-false if
        s" 3OS is not an allocated samplecorr"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the initial field from a samplecorr instance.
: samplecorr-get-initial ( addr --  sta-lst-corr )
    \ Check arg.
    assert-tos-is-samplecorr

    samplecorr-initial-disp +   \ Add offset.
    @                           \ Fetch the field.
;
 
\ Return the result field from a samplecorr instance.
: samplecorr-get-result ( addr -- sta-lst-corr )
    \ Check arg.
    assert-tos-is-samplecorr

    \ Get result state.
    samplecorr-result-disp +    \ Add offset.
    @                           \ Fetch the field.
;
\ Set the initial field from a samplecorr instance, use only in this file.
: _samplecorr-set-initial ( sta-lst-corr1 addr -- )
    \ Check args.
    assert-tos-is-samplecorr
    assert-nos-is-list

    samplecorr-initial-disp +   \ Add offset.
    !                           \ Set first field.
;
 
\ Set the result field from a samplecorr instance, use only in this file.
: _samplecorr-set-result ( sta-lst-corr1 addr -- )
    \ Check args.
    assert-tos-is-samplecorr
    assert-nos-is-list

    samplecorr-result-disp +    \ Add offset.
    !                           \ Set second field.
;

\ End accessors.

\ Create a samplecorr from two state-list-corrs on the stack.
: samplecorr-new ( sta-r-lst1 sta-i-lst0 -- addr)
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Allocate space.
    samplecorr-mma mma-allocate     \ sta-r-lst1 sta-i-lst0 addr

    \ Store id.
    samplecorr-id over              \ sta-r-lst1 sta-i-lst0 addr id addr
    struct-set-id                   \ sta-r-lst1 sta-i-lst0 addr

    \ Store states
    over struct-inc-use-count
    tuck _samplecorr-set-initial    \ sta-r-lst1  addr

    over struct-inc-use-count
    tuck _samplecorr-set-result     \ addr
;

\ Deallocate a samplecorr.
: samplecorr-deallocate ( smp0 -- )
    \ Check arg.
    assert-tos-is-samplecorr

    dup struct-get-use-count      \ smp0 count

    #2 <
    if 
        \ Clear fields.
        dup samplecorr-get-initial list-deallocate
        dup samplecorr-get-result list-deallocate

        \ Deallocate instance.
        samplecorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Print a samplecorr.
: .samplecorr ( smp0 -- )
    \ Check arg.
    assert-tos-is-samplecorr

    ." ("
    dup samplecorr-get-initial .state-list-corr
   ." ->"
   samplecorr-get-result .state-list-corr
   ." )"
;
