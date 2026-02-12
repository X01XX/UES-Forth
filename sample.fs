\ Implement a Sample struct and functions.
\
\ A initial/result pair of taking an action.
\
\ A initial/result problem, that may be solved with one, or many, actions, all within
\ a single domain.

#23719 constant sample-id
    #3 constant sample-struct-number-cells

\ Struct fields
0                           constant sample-header-disp     \ 16-bits, [0] struct id, [1] use count.
sample-header-disp  cell+   constant sample-initial-disp    \ Initial state.
sample-initial-disp cell+   constant sample-result-disp     \ Result state.

0 value sample-mma \ Storage for sample mma instance.

\ Init sample mma, return the addr of allocated memory.
: sample-mma-init ( num-items -- ) \ sets sample-mma.
    dup 1 <
    abort" sample-mma-init: Invalid number of items."

    cr ." Initializing Sample store."
    sample-struct-number-cells swap mma-new to sample-mma
;

\ Check instance type.
: is-allocated-sample ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup sample-mma mma-within-array
    if
        struct-get-id
        sample-id =
    else
        drop false
    then
;

\ Check TOS for sample, unconventional, leaves stack unchanged.
: assert-tos-is-sample ( tos -- tos )
    dup is-allocated-sample
    is-false if
        s" TOS is not an allocated sample"
        .abort-xt execute
    then
;

\ Check NOS for sample, unconventional, leaves stack unchanged.
: assert-nos-is-sample ( nos tos -- nos tos )
    over is-allocated-sample
    is-false if
        s" NOS is not an allocated sample"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the initial field from a sample instance.
: sample-get-initial ( smp0 -- u)
    \ Check arg.
    assert-tos-is-sample

    sample-initial-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Return the result field from a sample instance.
: sample-get-result ( smp0 -- u)
    \ Check arg.
    assert-tos-is-sample

    sample-result-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the initial field from a sample instance, use only in this file.
: _sample-set-initial ( u1 smp0 -- )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-value

    sample-initial-disp +   \ Add offset.
    !                       \ Set initial field.
;

\ Set the result field from a sample instance, use only in this file.
: _sample-set-result ( u1 smp0 -- )
    \ Check args.
    assert-tos-is-sample
    assert-nos-is-value

    sample-result-disp +    \ Add offset.
    !                       \ Set result field.
;

\ End accessors.

\ Create a sample from two numbers on the stack.
\ The numbers may be the same.
: sample-new ( r1 i0 -- smp)
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    \ Allocate space.
    sample-mma mma-allocate     \ u1 u2 smp

    \ Store id.
    sample-id over              \ u1 u2 smp id smp
    struct-set-id               \ u1 u2 smp

    \ Store states
    tuck _sample-set-initial   \ u1  smp
    tuck _sample-set-result    \ smp
;

\ Print a sample.
: .sample ( smp0 -- )
    \ Check arg.
    assert-tos-is-sample

    ." ("
    dup sample-get-initial .value
   ." ->"
   sample-get-result .value
   ." )"
;

\ Deallocate a sample.
: sample-deallocate ( smp0 -- )
    \ Check arg.
    assert-tos-is-sample

    dup struct-get-use-count      \ smp0 count

    #2 <
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
