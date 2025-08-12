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

\ Check action mma usage.
: assert-action-mma-none-in-use ( -- )
    action-mma mma-in-use 0<>
    if
        ." action-mma use GT 0"
        abort
    then
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

: action-inst-id ( act0 -- id )
    current-action
    action-get-inst-id
;

' action-inst-id to action-inst-id-xt

\ End accessors.

\ Create an action, given an instance ID.
\ The instance ID will likely be reset to match its position in a list,
\ which avoids duplicates and may be useful as an index into the list.
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
    domain-max-region-xt execute                   \ act lst mxreg
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
                region-get-states                   \ act0 inclst link s0 s1
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

\ Return a square given a state.
: action-find-square ( sta1 act0 -- sqr true | false )
    cr ." action-find-square" cr
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-value

    action-get-squares          \ sta1 sqr-lst
    square-list-find
;

\ Check a given region-list, where the region states represent incompatible pairs,
\ returning regions where the represented squares are no longer incompatible.
: _action-not-incompatble-pairs ( reg-lst1 act0 -- reg-lst2 )
    cr ." _action-not-incompatble-pairs" cr
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-list

    over list-is-empty
    if
        cr ." list is empty?" cr
        abort
    then

    \ Create return list.
    list-new -rot               \ ret-lst reg-lst1 act0

    \ Check each region in list
    swap                        \ ret-lst act0 reg-lst1
    list-get-links              \ ret-lst act0 link

    begin
        dup
    while
        dup link-get-data       \ ret-lst act0 link region
        dup                     \ ret-lst act0 link region region
    
        region-get-states       \ ret-lst act0 link region s1 s0

        \ Get square 0.
        4 pick                  \ ret-lst act0 link region s1 s0 act0
        action-find-square      \ ret-lst act0 link region s1 result
        if                      \ ret-lst act0 link region s1 sqr0
        else
            cr ." square not found?" cr
            abort
        then

        swap                    \ ret-lst act0 link region sqr0 s1

        \ Get square 1.
        4 pick                  \ ret-lst act0 link region sqr0 s1 act0
        action-find-square      \ ret-lst act0 link region sqr0 result
        if                      \ ret-lst act0 link region sqr0 sqr1
        else
            cr ." square not found?" cr
            abort
        then

        cr dup .square cr
        cr over .square cr
        square-compare          \ ret-lst act0 link region compare-result
        cr 4 spaces ." result " dup emit cr

        [char] I =              \ ret-lst act0 link region flag
        if                      \ ret-lst act0 link region
            drop                \ ret-lst act0 link
        else
            3 pick              \ ret-lst act0 link region ret-lst
            region-list-push    \ ret-lst act0 link
        then

        link-get-next
    repeat
                                \ ret-lst act0 0
    2drop                       \ ret-lst
;

\ Recalc action-logical-structure from action-incompatible-pairs.
:  _action-recalc-logical-structure ( act0 -- )
    cr ." _action-recalc-logical-structure" cr
    \ Check args.
    assert-arg0-is-action

    \ Save current logicl structure, to deallocate later, so the action field will never be invalid.
    dup action-get-logical-structure        \ act0 ls-old

    \ Init new logical-structure region list.
    list-new                                \ act0 ls-old ls-new
    domain-max-region-xt execute            \ act0 ls-old ls-new max-reg
    over region-list-push                   \ act0 ls-old ls-new
    
    2 pick                                  \ act0 ls-old ls-new act0
    action-get-incompatible-pairs           \ act0 ls-old ls-new i-pairs

    list-get-links                          \ act0 ls-old ls-new link
    begin
        dup
    while
        \ Get next ~A + ~B region list.
        dup link-get-data                   \ act0 ls-old ls-new link region
        region-get-states                   \ act0 ls-old ls-new link s1 s0
        state-not-a-or-not-b                \ act0 ls-old ls-new link nanb-lst

        \ Intersect with most recent logical-structure region list.
        rot                                 \ act0 ls-old link nanb-lst ls-new
        2dup                                \ act0 ls-old link nanb-lst ls-new nanb-lst ls-new
        region-list-region-intersections    \ act0 ls-old link nanb-lst ls-new ls-new-new

        \ Deallocate previous region lists.
        swap                                \ act0 ls-old link nanb-lst ls-new-new ls-new
        region-list-deallocate              \ act0 ls-old link nanb-lst ls-new-new
        swap                                \ act0 ls-old link ls-new-new nanb-lst
        region-list-deallocate              \ act0 ls-old link ls-new-new

        \ Prep for next cycle.
        swap                                \ act0 ls-old ls-new-new link

        link-get-next
    repeat
                                            \ act0 ls-old ls-new 0
    drop                                    \ act0 ls-old ls-new

    \ Store new LS.
    rot                                     \ ls-old ls-new act0
    _action-set-logical-structure           \ ls-old

    \ Dealloc old LS.
    region-list-deallocate                  \
;

\ Check incompatble pairs are still incompatible,
\ given a squrare that has recently changed pn or pnc.
: _action-check-incompatible-pairs ( sqr1 act0 -- )
    cr ." _action-check-incompatible-pairs" cr
    \ Check args.
    assert-arg0-is-action
    assert-arg1-is-square

    \ Get regions that use the state
    swap square-get-state               \ act0 sta
   \ cr ." Regions that use state " dup .value
    over action-get-incompatible-pairs  \ act0 sta reg-lst
    region-list-uses-state              \ act0 reg-lst-in
   \ space ." are " dup .region-list cr

    dup list-is-empty                   \ act0 reg-lst-in flag
    if
        list-deallocate                 \ act0
        2drop
        exit
    then

    \ Some regions found, check them.   \ act0 reg-lst-in
    2dup swap                           \ act0 reg-lst-in reg-lst-in act0
    _action-not-incompatble-pairs       \ act0 reg-lst-in reg-lst-not-i
    dup list-is-empty                   \ act0 reg-lst-in reg-lst-not-i flag
    if
        \ No not-incomptible pairs found.
        list-deallocate
        region-list-deallocate
        drop
        exit
    then

    \ Some not-incomptible pairs found.
                                        \ act0 reg-lst-in reg-lst-not-i
    swap region-list-deallocate         \ act0 reg-lst-not-i

    \ Remove regions.
    dup                                 \ act0 reg-lst-not-i reg-lst-not-i
    list-get-links                      \ act0 reg-lst-not-i link
    begin
        dup
    while
        dup link-get-data               \ act0 reg-lst-not-i link region
        cr ." state " dup region-get-states .value space ." and " .value space ." are no longer incompatible" cr
        [ ' region-eq ] literal swap    \ act0 reg-lst-not-i link xt region
        4 pick                          \ act0 reg-lst-not-i link xt region act0
        action-get-incompatible-pairs   \ act0 reg-lst-not-i link xt region pair-list
        list-remove                     \ act0 reg-lst-not-i link reg? flag
        if
            region-deallocate
        else
            cr ." Region not found?"
            abort
        then

        link-get-next
    repeat
                                        \ act0 reg-lst-not-i 0
    drop                                \ act0 reg-list-not-i
    region-list-deallocate              \ act0

    \ Recalc logical-structure
    _action-recalc-logical-structure
;

\ Add a sample.
\ Caller to deallocate sample.
\
\ If a new sample changes a pair in action-incompatible-pairs to not incompatible,
\ action-logical-structure must be completely recalculated, as there is no way
\ to back out an intersection.
\
\ If a new dissimilar pair, with no proper subset in action-incompatible-pairs, is found,
\ its ~A + ~B can be calculated and intersected with action-logical-structure.
\
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
            2dup
            _action-check-incompatible-pairs
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

