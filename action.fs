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
: action-get-inst-id ( act0 -- u)
    \ Check arg.
    assert-arg0-is-action

    \ Get intst ID.
    2w@
;
 
\ Set the instance ID of an action instance, use only in this file.
: _action-set-inst-id ( u1 act0 -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-value

    \ Set inst id.
    2w!
;

\ Return the square-list from an action instance.
: action-get-squares ( act0 -- lst )
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
    ." Act: " .

    dup action-get-squares
    dup list-get-length
    ."  num sqrs: " .
    ." sqrs " .square-list-states

    dup action-get-logical-structure space ." LS: " .region-list
    action-get-incompatible-pairs space ." IP: " .region-list
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

\ Get a list of incompatible pairs, no supersets, given a square.
: action-find-incompatible-pairs-nosups ( sqr1 act0 -- square-list )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-square

    list-new -rot                               \ retlst sqr1 act0
    2dup action-get-squares                     \ retlst sqr1 act0 sqr1 square-list
    [ ' square-incompatible ] literal -rot      \ retlst sqr1 act0 xt sqr1 sqr-list
    list-find-all                               \ retlst sqr1 act0 incompat-list

    dup list-is-empty
    if
        \ cr ." list is empty" cr
        list-deallocate                         \ retlst sqr1 act0
        2drop                                   \ retlst
        exit
    then

    \ cr ." list is NOT empty "
    \ dup .square-list-states

    2 pick square-get-state                 \ retlst sqr1 act0 inclst sta1
    over list-get-links                     \ retlst sqr1 act0 inclst sta1 link
    begin
        dup 0<>                             \ retlst sqr1 act0 inclst sta1 link flag
    while
        dup link-get-data square-get-state  \ retlst sqr1 act0 inclst sta1 link sta2
        2 pick                              \ retlst sqr1 act0 inclst sta1 link sta2 sta1
        region-new                          \ retlst sqr1 act0 inclst sta1 link regx
        \ cr ." reg: " dup .region cr
        dup                                 \ retlst sqr1 act0 inclst sta1 link regx regx
        7 pick                              \ retlst sqr1 act0 inclst sta1 link regx regx retlst
        region-list-push-nosups             \ retlst sqr1 act0 inclst sta1 link regx flag
        if
            drop
        else
            region-deallocate
        then
                                            \ retlst sqr1 act0 inclst sta1 link

        link-get-next                       \ retlst sqr1 act0 inclst sta1 link-next
    repeat
                                            \ retlst sqr1 act0 inclst sta1 0
    2drop                                   \ retlst sqr1 act0 inclst
    list-deallocate                         \ retlst sqr1 act0
    2drop                                   \ retlst
;
 
\ Check a new, or changed square.
\ Could affect action-incompatible-pairs and action-logical-structure.
: _action-check-square ( sqr1 act0 -- )
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-square

    \ cr ." at 0 " cr
    \ Check action-incompatible-pairs for pairs that are no longer incompatible.
    \ If any are found, remove them and recalculate everything.

    \ Form regions with incompatible squares, no supersets.
    swap over                               \ act0 sqr1 act0
    \ cr ." at 1 " .s cr
    action-find-incompatible-pairs-nosups   \ act0 inc-lst
    \ cr ." at 1 " cr
    dup list-is-empty
    if
        \ cr ." _action-check-square: list is empty" cr
        list-deallocate
        drop
        exit
    then

    \ If there is no proper subset region in action-incompatible-pairs,
    \ push nosups, calc ~A + ~B, intersect with action-logical-structure.

    \ cr ." list is NOT empty " dup .region-list cr

                                            \ act0 inclst
    dup list-get-links                      \ act0 inclst link
    begin
        dup 0<>                             \ act0 inclst link flag
    while
       \  cr ." at while " cr
        dup link-get-data                   \ act0 inclst link regx
        \ cr ." incompatible reg: " dup .region cr

        \ Check if dup in action-incompatible-pairs
        dup                                         \ act0 inclst link regx regx
        4 pick action-get-incompatible-pairs        \ act0 inclst link regx regx pair-lst
        [ ' region-eq ] literal -rot                \ act0 inclst link regx xt regx pair-lst
        list-member                                 \ act0 inclst link regx flag
        \ cr ." at 2 " .s cr
        if
            \ cr ." dup in list" cr
            drop
        else
            \ cr ." no dup in list"
            dup                                     \ act0 inclst link regx regx
            4 pick action-get-incompatible-pairs    \ act0 inclst link regx regx pair-lst
            [ ' region-subset-of ] literal -rot     \ act0 inclst link regx xt regx pair-lst
            list-member                             \ act0 inclst link regx flag
            if
                \ cr ." subset found" cr
                drop                                \ act0 inclst link
            else
                \ cr ." no subset found" cr           \ act0 inclst link regx

                \ Add region to the action-incompatible-pairs  list.
                cr ." Act: " 3 pick action-get-inst-id . space ." Adding incompatible pair: " dup region-get-states .value space .value cr
                dup 4 pick                          \ act0 inclst link regx regx act0
                action-get-incompatible-pairs       \ act0 inclst link regx regx incpairs
                region-list-push-nosups             \ act0 inclst link regx flag
                drop

                \ Calc regions possible for incompatible pair.
                region-get-states                   \ act0 inclst link sta1 sta0
                state-not-a-or-not-b                \ act0 inclst link reg-lst

                \ Calc new action-logical-structure.
                3 pick action-get-logical-structure \ act0 inclst link reg-lst lsl-lst
                2dup                                \ act0 inclst link reg-lst lsl-lst reg-lst lsl-lsn
                region-list-region-intersections    \ act0 inclst link reg-lst lsl-lst new-reg-lst

                \ Set new action-logical-structure.
                5 pick                              \ act0 inclst link reg-lst lsl-lst new-reg-lst act0
                _action-set-logical-structure       \ act0 inclst link reg-lst lsl-lst
                region-list-deallocate              \ act0 inclst link reg-lst
                region-list-deallocate              \ act0 inclst link
            then
        then
        \ cr ." at repeat " cr

        link-get-next                       \ act0 inclst sta1 link-next
    repeat
                                            \ act0 inclst 0
    drop                                    \ act0 inclst

    region-list-deallocate
    drop
;

\ Add a sample.
\ Caller to deallocate sample.
: action-add-sample ( smpl1 act0 -- )
    \ cr ." at 0: " .s cr
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-sample

    cr ." Act: " dup action-get-inst-id . space ." adding sample: " over .sample cr

    over sample-get-initial
    over action-get-squares
    square-list-find            \ smpl1 act0 : sqr true | false
    if
        \ Update existing square
        rot                     \ act0 sqr smpl
        over                    \ act sqr smpl sqr
        square-add-sample       \ act sqr flag
        if
            swap
            \ cr ." at 1: " .s cr
            _action-check-square
        else
            2drop
        then
    else
        \ Add new square.
        swap                    \ act0 smpl
        square-from-sample      \ act0 sqr
        dup                     \ act0 sqr sqr
        2 pick                  \ act0 sqr sqr act0
        action-get-squares      \ act0 sqr sqr sqrlst
        square-list-push        \ act0 sqr
        swap                    \ sqr act0
        \ cr ." at 2: " .s cr
        _action-check-square
    then
    \ cr ." at 3: " .s cr
;

