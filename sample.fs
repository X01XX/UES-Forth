\ Implement a Sample struct and functions.                                                          

23719 constant sample-id
    3 constant sample-struct-number-cells

\ Struct fields
0 constant sample-header    \ 16-bits [0] struct id [1] use count.
sample-header  cell+ constant sample-initial
sample-initial cell+ constant sample-result

0 value sample-mma \ Storage for sample mma instance.

\ Init sample mma, return the addr of allocated memory.
: sample-mma-init ( num-items -- ) \ sets sample-mma.
    dup 1 < 
    if  
        ." sample-mma-init: Invalid number of items."
        abort
    then

    cr ." Initializing Sample store."
    sample-struct-number-cells swap mma-new to sample-mma
;

\ Check instance type.
: is-allocated-sample ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup sample-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    sample-id =     
;

: is-not-allocated-sample ( addr -- flag )
    is-allocated-sample 0=
;

\ Check arg0 for sample, unconventional, leaves stack unchanged. 
: assert-arg0-is-sample ( arg0 -- arg0 )
    dup is-allocated-sample 0=
    if
        cr ." arg0 is not an allocated sample"
        abort
    then
;

\ Check arg1 for sample, unconventional, leaves stack unchanged. 
: assert-arg1-is-sample ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-sample 0=
    if
        cr ." arg1 is not an allocated sample"
        abort
    then
;

\ Start accessors.

\ Return the first field from a sample instance.
: sample-get-initial ( addr -- u)
    \ Check arg.
    assert-arg0-is-sample

    sample-initial +    \ Add offset.
    @                   \ Fetch the field.
;
 
\ Return the second field from a sample instance.
: sample-get-result ( addr -- u)
    \ Check arg.
    assert-arg0-is-sample

    \ Get second state.
    sample-result +    \ Add offset.
    @                   \ Fetch the field.
;
\ Set the first field from a sample instance, use only in this file.
: _sample-set-initial ( u1 addr -- )
    \ Check args.
    assert-arg0-is-sample
    assert-arg1-is-value

    sample-initial +    \ Add offset.
    !                   \ Set first field.
;
 
\ Set the second field from a sample instance, use only in this file.
: _sample-set-result ( u1 addr -- )
    \ Check args.
    assert-arg0-is-sample
    assert-arg1-is-value

    sample-result +    \ Add offset.
    !                   \ Set second field.
;

\ End accessors.

\ Create a sample from two numbers on the stack.
\ The numbers may be the same.
\ If you want to keep the sample on the stack, or in a value, or variable,
\ run dup struct-inc-use-count, then deallocate it from there when done using it.
\ If you want to push the sample onto a list, sample-list-push will increment the use count.
: sample-new ( u1 u0 -- addr)
    \ Check args.
    assert-arg0-is-value
    assert-arg1-is-value

    \ Allocate space.
    sample-mma mma-allocate     \ u1 u2 addr

    \ Store id.
    sample-id over              \ u1 u2 addr id addr
    struct-set-id               \ u1 u2 addr
    
    \ Init use count.
    0 over struct-set-use-count

    \ Prepare to store states.
    -rot            \ addr u1 u2
    2 pick          \ addr u1 u2 addr
    swap over       \ addr u1 addr u2 addr

    \ Store states
    _sample-set-result     \ addr u1 addr
    _sample-set-initial     \ addr
;

\ Print a sample.
: .sample ( smp0 -- )
    \ Check arg.
    assert-arg0-is-sample

    ." ("
    dup sample-get-initial .value
   ." ->"
   sample-get-result .value
   ." )"
;

\ Deallocate a sample.
: sample-deallocate ( smp0 -- )
    \ Check arg.
    assert-arg0-is-sample

    dup struct-get-use-count      \ smp0 count

    2 <
    if 
        \ Clear fields.
        0 over _sample-set-initial
        0 over _sample-set-result

        \ Deallocate instance.
        sample-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

