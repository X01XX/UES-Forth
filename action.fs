\ Implement a Action struct and functions.                                                          

29717 constant action-id
    2 constant action-struct-number-cells

\ Struct fields
0 constant action-header    \ 16-bits [0] struct id [1] use count [2] instance id 
action-header  cell+ constant action-squares
\ action-state-0 cell+ constant action-state-1

0 value action-mma \ Storage for action mma instance.

\ Init action mma, return the addr of allocated memory.
: action-mma-init ( num-items -- ) \ sets action-mma.
    dup 1 < 
    if  
        ." action-mma-init: Invalid number of items."
        abort
    then

    cr ." Initializing Action store."
    action-struct-number-cells swap mma-new to action-mma
;

\ Check instance type.
: is-allocated-action ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup action-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    action-id =     
;

: is-not-allocated-action ( addr -- flag )
    is-allocated-action 0=
;

\ Check arg0 for action, unconventional, leaves stack unchanged. 
: assert-arg0-is-action ( arg0 -- arg0 )
    dup is-allocated-action 0=
    if
        cr ." arg0 is not an allocated action"
        abort
    then
;

\ Check arg1 for action, unconventional, leaves stack unchanged. 
: assert-arg1-is-action ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-action 0=
    if
        cr ." arg1 is not an allocated action"
        abort
    then
;

\ Start accessors.

\ Return the second field from a action instance.
: action-get-inst-id ( addr -- u)
    \ Check arg.
    assert-arg0-is-action

    \ Get intst ID.
    2w@
;
 
\ Set the second field from a action instance, use only in this file.
: _action-set-inst-id ( u1 addr -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-value

    \ Set inst id.
    2w!
;

\ Return the first field from a action instance.
: action-get-squares ( addr -- u)
    \ Check arg.
    assert-arg0-is-action

    action-squares +    \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the first field from a action instance, use only in this file.
: _action-set-squares ( u1 addr -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-value

    action-squares +    \ Add offset.
    !                   \ Set first field.
;
 
\ End accessors.
\ Create an action, given an instance ID.
: action-new ( val0 -- addr)
    \ Check args.
    assert-arg0-is-value

    \ Allocate space.
    action-mma mma-allocate     \ val0 actr

    \ Store id.
    action-id over              \ val0 act id act
    struct-set-id               \ val0 act
    
    \ Init use count.
    0 over struct-set-use-count \ val0 act

    \ Set intance ID.
    swap over _action-set-inst-id  \ act

    \ Set squares list.
    list-new                        \ act lst
    dup struct-inc-use-count        \ act lst
    over _action-set-squares        \ act
;

\ Print a action.
: .action ( act0 -- )
    \ Check arg.
    assert-arg0-is-action
;

\ Deallocate a action.
: action-deallocate ( act0 -- )
    \ Check arg.
    assert-arg0-is-action

    dup struct-get-use-count      \ act0 count

    2 <
    if 
        \ Clear fields.
        dup action-get-squares square-list-deallocate
        0 over _action-set-squares

        \ Deallocate instance.
        action-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

