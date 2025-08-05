\ Implement a Action struct and functions.                                                          

29717 constant action-id
    4 constant action-struct-number-cells

\ Struct fields
0 constant action-header    \ 16-bits [0] struct id [1] use count [2] instance id 
action-header               cell+ constant action-squares               \ A square-list
action-squares              cell+ constant action-incompatible-pairs    \ A region-list
action-incompatible-pairs   cell+ constant action-logical-structure     \ A region-list

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

\ Return the instance ID from an action instance.
: action-get-inst-id ( sqr0 -- u)
    \ Check arg.
    assert-arg0-is-action

    \ Get intst ID.
    2w@
;
 
\ Set the instance ID of an action instance, use only in this file.
: _action-set-inst-id ( u1 sqr0 -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-value

    \ Set inst id.
    2w!
;

\ Return the square-list from an action instance.
: action-get-squares ( sqr0 -- lst )
    \ Check arg.
    assert-arg0-is-action

    action-squares +    \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the square-list of an action instance, use only in this file.
: _action-set-squares ( lst1 act0 -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-list

    action-squares +    \ Add offset.
    !                   \ Set the field.
;


\ Return the incompatible-pairs region-list from an action instance.
: action-get-incompatible-pairs ( addr -- lst )
    \ Check arg.
    assert-arg0-is-action

    action-incompatible-pairs + \ Add offset.
    @                           \ Fetch the field.
;
 
\ Set the incompatible-pairs region-list of an action instance, use only in this file.
: _action-set-incompatible-pairs ( u1 addr -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-list

    action-incompatible-pairs + \ Add offset.
    !                           \ Store it.
;

\ Return the logical-structure region-list from an action instance.
: action-get-logical-structure ( addr -- lst )
    \ Check arg.
    assert-arg0-is-action

    action-logical-structure +  \ Add offset.
    @                           \ Fetch the field.
;
 
\ Set the logical-structure region-list of an action instance, use only in this file.
: _action-set-logical-structure ( u1 addr -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-list

    action-logical-structure +  \ Add offset.
    !                           \ Store it.
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

    \ Set incompatible-pairs list.
    list-new                            \ act lst
    dup struct-inc-use-count            \ act lst
    over _action-set-incompatible-pairs \ act

    \ Set logical-structure list.
    list-new                            \ act lst
    dup struct-inc-use-count            \ act lst
    domain-max-region                   \ act lst mxreg
    over list-push                      \ act lst
    over _action-set-logical-structure  \ act
;

\ Print a action.
: .action ( act0 -- )
    \ Check arg.
    assert-arg0-is-action

    dup action-get-inst-id
    ." act: " .

    action-get-squares
    dup list-get-length
    ."  num sqrs: " .
    ." sqrs " .square-list-states 
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
        dup action-get-incompatible-pairs region-list-deallocate
        dup action-get-logical-structure region-list-deallocate

        \ Deallocate instance.
        action-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Add a sample.
\ Caller to deallocate sample.
: action-add-sample ( smpl1 act0 -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-sample

    cr ." Act: " dup action-get-inst-id . space ." adding sample: " over .sample cr

    over sample-get-initial
    over action-get-squares
    square-list-find            \ smpl1 act0 : sqr true | false
    if
        \ Update existing square
        rot
        swap
        square-add-sample
    else
        \ Add new square.
        swap                    \ act0 smpl
        square-from-sample      \ act0 sqr
        over                    \ act0 sqr act0
        action-get-squares      \ act0 sqr sqrlst
        square-list-push        \ act0
        true
    then
    nip
;

