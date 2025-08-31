\ Implement a Action struct and functions.                                                          

29717 constant action-id
    6 constant action-struct-number-cells

\ Struct fields
0 constant action-header    \ (16) struct id (16) use count (8) instance id 
action-header               cell+ constant action-squares               \ A square-list
action-squares              cell+ constant action-incompatible-pairs    \ A region-list
action-incompatible-pairs   cell+ constant action-logical-structure     \ A region-list
action-logical-structure    cell+ constant action-groups                \ A group-list.
action-groups               cell+ constant action-function              \ An xt to run to get a sample.

0 value action-mma \ Storage for action mma instance.

\ Init action mma, return the addr of allocated memory.
: action-mma-init ( num-items -- ) \ sets action-mma.
    dup 1 < 
    abort" action-mma-init: Invalid number of items."

    cr ." Initializing Action store."
    action-struct-number-cells swap mma-new to action-mma
;

\ Check action mma usage.
: assert-action-mma-none-in-use ( -- )
    action-mma mma-in-use 0<>
    abort" action-mma use GT 0"
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

\ Check TOS for action, unconventional, leaves stack unchanged. 
: assert-tos-is-action ( arg0 -- arg0 )
    dup is-allocated-action 0=
    abort" TOS is not an allocated action"
;

\ Check NOS for action, unconventional, leaves stack unchanged. 
: assert-nos-is-action ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-action 0=
    abort" NOS is not an allocated action"
;

' assert-nos-is-action to assert-nos-is-action-xt

\ Check 3OS for action, unconventional, leaves stack unchanged. 
: assert-3os-is-action ( arg2 arg1 arg0 -- arg2 arg1 arg0 )
    2 pick is-allocated-action 0=
    abort" 3OS is not an allocated action"
;

\ Start accessors.

\ Return the instance ID from an action instance.
: action-get-inst-id ( act0 -- u)
    \ Check arg.
    assert-tos-is-action

    \ Get intst ID.
    4c@
;

' action-get-inst-id to action-get-inst-id-xt

\ Set the instance ID of an action instance, use only in this file.
: action-set-inst-id ( u1 act0 -- )
    \ Check args.
    assert-tos-is-action
    \ assert-nos-is-value

    over 0<
    abort" Invalid instance id"

    over 255 >
    abort" Invalid instance id"

    \ Set inst id.
    4c!
;

\ Return the square-list from an action instance.
: action-get-squares ( act0 -- lst )
    \ Check arg.
    assert-tos-is-action

    action-squares +    \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the square-list of an action instance, use only in this file.
: _action-set-squares ( lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    action-squares +    \ Add offset.
    !                   \ Set the field.
;


\ Return the incompatible-pairs region-list from an action instance.
: action-get-incompatible-pairs ( addr -- lst )
    \ Check arg.
    assert-tos-is-action

    action-incompatible-pairs + \ Add offset.
    @                           \ Fetch the field.
;
 
\ Set the incompatible-pairs region-list of an action instance, use only in this file.
: _action-set-incompatible-pairs ( u1 addr -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    action-incompatible-pairs + \ Add offset.
    !                           \ Store it.
;

\ Return the logical-structure region-list from an action instance.
: action-get-logical-structure ( addr -- lst )
    \ Check arg.
    assert-tos-is-action

    action-logical-structure +  \ Add offset.
    @                           \ Fetch the field.
;
 
\ Set the logical-structure region-list of an action instance, use only in this file.
: _action-set-logical-structure ( new-ls addr -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    \ Set new LS.
    action-logical-structure +  \ Add offset.
    !                           \ Store it.
;


\ Return the group-list from an action instance.
: action-get-groups ( act0 -- lst )
    \ Check arg.
    assert-tos-is-action

    action-groups +     \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the group-list of an action instance, use only in this file.
: _action-set-groups ( lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    action-groups +     \ Add offset.
    !                   \ Set the field.
;

\ Return the function xt that implements the action.
: action-get-function ( act0 -- xt )
    \ Check arg.
    assert-tos-is-action

    action-function +   \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the futction xt that implements an action.
: _action-set-function ( xt act0 -- )
    \ Check args.
    assert-tos-is-action

    action-function +   \ Add offset.
    !                   \ Set the field.
;

\ Return true if a region, in the logical structure, is a defining region.
: action-region-is-defining ( reg1 act0 -- flag )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region

    2dup                            \ reg1 act0 reg1 act0
    action-get-logical-structure    \ reg1 act0 reg1 LS
    tuck                            \ reg1 act0 LS reg1 LS
    region-list-member              \ reg1 act0 LS flag
    0= abort" Region not in logical structure"

    -rot                            \ LS reg1 act0
    action-get-squares              \ LS reg1 sqr-lst
    square-list-states-in-region    \ LS sta-lst
    swap                            \ sta-lst LS

    region-list-states-in-one-region    \ sta-lst2

    dup list-is-empty
    if
        list-deallocate
        false
    else
        true
    then
;

: _action-delete-group-if-exists ( reg1 act0 -- flag )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region

    \ If group exists, delete it.
    2dup                                \ reg1 act0 reg1 act0
    action-get-groups                   \ reg1 act0 reg1 grp-lst
    group-list-member                   \ reg1 act0 flag
    if
        action-get-groups               \ reg1 grp-lst
        group-list-remove               \ flag
        0= abort" Group remove failed?"
        true
    else
        2drop
        false
    then
;

\ Update the logical-structure region-list of an action instance, use only in this file.
\ Deallocate the old list last, so the instance field is never invalid.
: _action-update-logical-structure ( new-ls act0 -- )
    \ cr ." _action-update-logical-structure: start"  cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list
    cr ." new list " over .region-list cr

    \ Check the new list is different from the old list.
    over                                \ new-lst act0 new-lst
    over action-get-logical-structure   \ new-lst act0 new-lst old-lst
    2dup region-list-eq                 \ new-lst act0 new-lst old-lst flag
    abort" region lists equal?"
    nip                                 \ new-lst act0 old-lst

    \ Get/save current LS.
    cr ." old list " dup .region-list cr

    -rot                                \ old-ls new-ls act0

    \ Store new structure.
    2dup                                \ old-ls new-ls act0 new-ls act0
    _action-set-logical-structure       \ old-ls new-ls act0

    \ Save action, for now..
    -rot                                \ act0 old-ls new-ls

    \ Get old regions that are deleted.
    2dup                                \ act0 old-ls new-ls old-ls new-ls
    region-list-set-difference          \ act0 old-ls new-ls old-gone
    cr ." Old LS regions deleted: " dup .region-list cr

    \ Scan deleted regions.
    dup list-get-links                   \ act0 old-ls new-ls old-gone link
    begin
        ?dup
    while
        dup link-get-data               \ act0 old-ls new-ls old-gone link region

        \ If group exists, delete it.
        5 pick                          \ act0 old-ls new-ls old-gone link region act0
        _action-delete-group-if-exists  \ act0 old-ls new-ls old-gone link flag
        if
            cr 4 spaces dup link-get-data .region
            space ." deleted group"
        then

        link-get-next                   \ act0 old-ls new-ls old-gone link
    repeat
    cr
                                        \ act0 old-ls new-ls old-gone
    region-list-deallocate              \ act0 old-ls new-ls

    \ Get new regions.
    dup                                 \ act0 old-ls new-ls new-ls
    2 pick                              \ act0 old-ls new-ls new-ls old-os
    
    region-list-set-difference          \ act0 old-ls new-ls new-added
    cr ." New LS regions added: " dup .region-list cr
    region-list-deallocate              \ act0 old-ls new-ls 

    \ Scan new regions.
    dup list-get-links                      \ act0 old-ls new-ls link
    begin
        ?dup
    while
        dup link-get-data                   \ act0 old-ls new-ls link region

        cr 4 spaces dup .region

        \ Get states in region.
        4 pick action-get-squares           \ act0 old-ls new-ls link region sqr-lst
        square-list-states-in-region        \ act0 old-ls new-ls link sta-lst1

        \ Get states only is one region.
        dup                                 \ act0 old-ls new-ls link sta-lst1 sta-lst1
        3 pick                              \ act0 old-ls new-ls link sta-lst1 sta-lst1 new-ls
        region-list-states-in-one-region    \ act0 old-ls new-ls link sta-lst1 sta-lst2
        swap list-deallocate                \ act0 old-ls new-ls link sta-lst2

        dup
        list-is-empty                       \ act0 old-ls new-ls link sta-lst2 flag
        if                                  \ act0 old-ls new-ls link sta-lst2
            list-deallocate                 \ act0 old-ls new-ls link
            space ." is NOT defining"

            \ If group exists, delete it.
            dup link-get-data               \ act0 old-ls new-ls link region
            4 pick                          \ act0 old-ls new-ls link region act0
            _action-delete-group-if-exists  \ act0 old-ls new-ls link flag
            if                              \ act0 old-ls new-ls link 
                space ." deleted group"
            then
        else                                    \ act0 old-ls new-ls link sta-lst2
            \ space ." region is defining " dup .value-list
            list-deallocate                     \ act0 old-ls new-ls link

            \ Check if group already exists
            dup link-get-data                   \ act0 old-ls new-ls link reg
            4 pick                              \ act0 old-ls new-ls link reg act0
            action-get-groups                   \ act0 old-ls new-ls link reg grps
            group-list-member                   \ act0 old-ls new-ls link flag
            if                                  \ act0 old-ls new-ls link
                space ." group already exists"
            else
                dup link-get-data                   \ act0 old-ls new-ls link reg
                4 pick                              \ act0 old-ls new-ls link reg act0
                action-get-squares                  \ act0 old-ls new-ls link reg sqr-lst1
                square-list-in-region               \ act0 old-ls new-ls link sqr-lst2
                dup list-is-empty                   \ act0 old-ls new-ls link sqr-lst2 flag
                if                                  \ act0 old-ls new-ls link sqr-lst2
                    space ." is NOT defining"
                    square-list-deallocate

                    \ If group exists, delete it.
                    dup link-get-data               \ act0 old-ls new-ls link region
                    4 pick                          \ act0 old-ls new-ls link region act0
                    _action-delete-group-if-exists  \ act0 old-ls new-ls link flag
                    if                              \ act0 old-ls new-ls link 
                        space ." deleted group"
                    then
                else
                    dup                             \ act0 old-ls new-ls link sqr-lst2 sqr-lst2
                    square-list-get-rules           \ act0 old-ls new-ls link sqr-lst2, ruls true | false
                    if                              \ act0 old-ls new-ls link sqr-lst2 ruls
                        space ." is defining" over .square-list-states
                        \ TODO? change group-new to take rules.
                        rulestore-deallocate        \ act0 old-ls new-ls link sqr-lst2
                        over link-get-data          \ act0 old-ls new-ls link sqr-lst2 reg
                        group-new                   \ act0 old-ls new-ls link grp
                        4 pick action-get-groups    \ act0 old-ls new-ls link grp grp-lst
                        group-list-push             \ act0 old-ls new-ls link
                    else                            \ act0 old-ls new-ls link sqr-lst2
                        space ." is NOT defining"
                        square-list-deallocate      \ act0 old-ls new-ls link

                        \ If group exists, delete it.
                        dup link-get-data               \ act0 old-ls new-ls link region
                        4 pick                          \ act0 old-ls new-ls link region act0
                        _action-delete-group-if-exists  \ act0 old-ls new-ls link flag
                        if                              \ act0 old-ls new-ls link 
                            space ." deleted group"
                        then
                    then
                then
            then
        then

        link-get-next                   \ act0 old-ls new-ls link
    repeat
    cr
                                        \ act0 old-ls new-ls

    \ Deallocate
    drop                                \ act0 old-ls

    region-list-deallocate              \ act0

    drop                                \
    \ cr ."  _action-update-logical-structure - end" cr
;

\ End accessors.

\ Create an action, given an xt to get a sample.
\ The instance ID defaults to zero.
\ It will likely be reset to match its position in a list, using action-set-inst-id,
\ which avoids duplicates and may be useful as an index into the list.
: action-new ( nb1 xt1 -- addr)

    \ Allocate space.
    action-mma mma-allocate     \ nb1 xt1 actr

    \ Store struct id.
    action-id over              \ nb1 xt1 act id act
    struct-set-id               \ nb1 xt1 act
    
    \ Init use count.
    0 over struct-set-use-count \ nb1 xt1 act

    \ Set intance ID.
    0 over
    action-set-inst-id              \ nb1 xt1 act

    \ Set xt
    tuck _action-set-function       \ nb1 act

    \ Set squares list.
    list-new                        \ nb1 act lst
    dup struct-inc-use-count        \ nb1 act lst
    over _action-set-squares        \ nb1 act

    \ Set incompatible-pairs list.
    list-new                            \ nb1 act lst
    dup struct-inc-use-count            \ nb1 act lst
    over _action-set-incompatible-pairs \ nb1 act

    \ Set logical-structure list.
    list-new                            \ nb1 act lst
    dup struct-inc-use-count            \ nb1 act lst
    2dup swap                           \ nb1 act lst lst act
    _action-set-logical-structure       \ nb1 act lst

    \ All max region.
    rot                                 \ act lst nb1
    all-bits                            \ act lst all-bits
    0 region-new2                       \ act lst reg
    swap region-list-push               \ act

    \ Set group list.
    list-new                            \ act lst
    dup struct-inc-use-count            \ act lst
    over _action-set-groups             \ act
;

\ Print a action.
: .action ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    dup action-get-inst-id
    cr 5 spaces ." Act: " .

    dup action-get-squares
    dup list-get-length
    ." num sqrs: " .
    ." sqrs " .square-list-states

    dup action-get-logical-structure space ." LS: " .region-list
    dup action-get-incompatible-pairs space ." IP: " .region-list
    \ cr ." Groups: "
    \ Print each group.
    action-get-groups list-get-links
    begin
        ?dup
    while
        dup link-get-data
        cr 10 spaces .group
        link-get-next
    repeat
    cr
;

\ Deallocate a action.
: action-deallocate ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    dup struct-get-use-count      \ act0 count

    2 <
    if 
        \ Clear fields.
        dup action-get-squares square-list-deallocate
        dup action-get-incompatible-pairs region-list-deallocate
        dup action-get-logical-structure region-list-deallocate
        dup action-get-groups group-list-deallocate

        \ Deallocate instance.
        action-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Get a list of incompatible pairs, no supersets, given a square.
: action-find-incompatible-pairs-nosups ( sqr1 act0 -- square-list )
    \ cr ." action-find-incompatible-pairs-nosups: start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    list-new -rot                               \ retlst sqr1 act0
    2dup action-get-squares                     \ retlst sqr1 act0 sqr1 square-list
    [ ' square-incompatible ] literal -rot      \ retlst sqr1 act0 xt sqr1 sqr-list
    list-find-all                               \ retlst sqr1 act0 incompat-list

    dup list-is-empty
    if
        list-deallocate                         \ retlst sqr1 act0
        2drop                                   \ retlst
        \ cr ." action-find-incompatible-pairs-nosups: end" cr
        exit
    then

    2 pick square-get-state                 \ retlst sqr1 act0 inclst sta1
    over list-get-links                     \ retlst sqr1 act0 inclst sta1 link
    begin
        dup                                 \ retlst sqr1 act0 inclst sta1 link link
    while
        dup link-get-data square-get-state  \ retlst sqr1 act0 inclst sta1 link sta2
        2 pick                              \ retlst sqr1 act0 inclst sta1 link sta2 sta1
        region-new                          \ retlst sqr1 act0 inclst sta1 link regx

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
    \ cr ." action-find-incompatible-pairs-nosups: end" cr
;
 
\ Check a new, or changed square.
\ Could affect action-incompatible-pairs and action-logical-structure.
: _action-check-square ( sqr1 act0 -- )
    \ cr ." _action-check-square - start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    \ Check action-incompatible-pairs for pairs that are no longer incompatible.
    \ If any are found, remove them and recalculate everything.

    \ Form regions with incompatible squares, no supersets.
    tuck                                    \ act0 sqr1 act0
    action-find-incompatible-pairs-nosups   \ act0 inc-lst
    dup list-is-empty
    if
        \ cr ." _action-check-square: list is empty" cr
        list-deallocate
        drop
        exit
    then

    \ If there is no proper subset region in action-incompatible-pairs,
    \ push nosups, calc ~A + ~B, intersect with action-logical-structure.

                                            \ act0 inclst
    dup list-get-links                      \ act0 inclst link
    begin
        dup                                 \ act0 inclst link link
    while
        dup link-get-data                   \ act0 inclst link regx

        \ Check if dup in action-incompatible-pairs
        dup                                         \ act0 inclst link regx regx
        4 pick action-get-incompatible-pairs        \ act0 inclst link regx regx pair-lst
        [ ' region-eq ] literal -rot                \ act0 inclst link regx xt regx pair-lst
        list-member                                 \ act0 inclst link regx flag
        if
            drop
        else
            dup                                     \ act0 inclst link regx regx
            4 pick action-get-incompatible-pairs    \ act0 inclst link regx regx pair-lst
            [ ' region-subset-of ] literal -rot     \ act0 inclst link regx xt regx pair-lst
            list-member                             \ act0 inclst link regx flag
            if
                drop                                \ act0 inclst link
            else
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
                _action-update-logical-structure    \ act0 inclst link reg-lst lsl-lst
                drop                                \ act0 inclst link reg-lst
                region-list-deallocate              \ act0 inclst link
            then
        then

        link-get-next                       \ act0 inclst sta1 link-next
    repeat
                                            \ act0 inclst 0
    drop                                    \ act0 inclst

    region-list-deallocate
    drop
    \ cr ." _action-check-square - end" cr
;

\ Return a square given a state.
: action-find-square ( sta1 act0 -- sqr true | false )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    action-get-squares          \ sta1 sqr-lst
    square-list-find
;

\ Check a given region-list, where the region states represent incompatible pairs,
\ returning regions where the represented squares are no longer incompatible.
: _action-not-incompatible-pairs ( reg-lst1 act0 -- reg-lst2 )
    \ cr ." _action-not-incompatible-pairs - start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    over list-is-empty
    abort" list is empty?"

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
        0=
        abort" square not found?"

        swap                    \ ret-lst act0 link region sqr0 s1

        \ Get square 1.
        4 pick                  \ ret-lst act0 link region sqr0 s1 act0
        action-find-square      \ ret-lst act0 link region sqr0 result
        0=
        abort" square not found?"

        \ cr dup .square cr
        \ cr over .square cr
        square-compare          \ ret-lst act0 link region compare-result

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
    \ cr ." _action-not-incompatible-pairs - end" cr
;

\ Recalc action-logical-structure from action-incompatible-pairs.
:  _action-recalc-logical-structure ( act0 -- )
    \ cr ." _action-recalc-logical-structure - start" cr
    \ Check args.
    assert-tos-is-action

    \ Init new logical-structure region list.
    list-new                                \ act0 ls-new
    cur-domain-xt execute                   \ act0 ls-new dom
    domain-get-max-region-xt execute        \ act0 ls-new max-reg
    over region-list-push                   \ act0 ls-new
    
    2 pick                                  \ act0 ls-new act0
    action-get-incompatible-pairs           \ act0 ls-new i-pairs

    list-get-links                          \ act0 ls-new link
    begin
        dup
    while
        \ Get next ~A + ~B region list.
        dup link-get-data                   \ act0 ls-new link region
        region-get-states                   \ act0 ls-new link s1 s0
        state-not-a-or-not-b                \ act0 ls-new link nanb-lst

        \ Intersect with most recent logical-structure region list.
        rot                                 \ act0 link nanb-lst ls-new
        2dup                                \ act0 link nanb-lst ls-new nanb-lst ls-new
        region-list-region-intersections    \ act0 link nanb-lst ls-new ls-new-new

        \ Deallocate previous region lists.
        swap                                \ act0 link nanb-lst ls-new-new ls-new
        region-list-deallocate              \ act0 link nanb-lst ls-new-new
        swap                                \ act0 link ls-new-new nanb-lst
        region-list-deallocate              \ act0 link ls-new-new

        \ Prep for next cycle.
        swap                                \ act0 ls-new-new link

        link-get-next
    repeat
                                            \ act0 ls-new 0
    drop                                    \ act0 ls-new

    \ Store new LS.
    rot                                     \ ls-new act0
    _action-update-logical-structure        \
    \ cr ." _action-recalc-logical-structure - end" cr
;

\ Check incompatble pairs are still incompatible,
\ given a squrare that has recently changed pn or pnc.
: _action-check-incompatible-pairs ( sqr1 act0 -- )
    \ cr ." _action-check-incompatible-pairs - start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    \ Get regions that use the state
    swap square-get-state               \ act0 sta
    over action-get-incompatible-pairs  \ act0 sta reg-lst
    region-list-uses-state              \ act0 reg-lst-in

    dup list-is-empty                   \ act0 reg-lst-in flag
    if
        list-deallocate                 \ act0
        2drop
        \ cr ." _action-check-incompatible-pairs - end" cr
        exit
    then

    \ Some regions found, check them.   \ act0 reg-lst-in
    2dup swap                           \ act0 reg-lst-in reg-lst-in act0
    _action-not-incompatible-pairs      \ act0 reg-lst-in reg-lst-not-i
    dup list-is-empty                   \ act0 reg-lst-in reg-lst-not-i flag
    if
        \ No not-incomptible pairs found.
        list-deallocate
        region-list-deallocate
        drop
        \ cr ." _action-check-incompatible-pairs - end" cr
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
        0=
        abort" Region not found?"

        region-deallocate

        link-get-next
    repeat
                                        \ act0 reg-lst-not-i 0
    drop                                \ act0 reg-list-not-i
    region-list-deallocate              \ act0

    \ Recalc logical-structure
    _action-recalc-logical-structure
    \ cr ." _action-check-incompatible-pairs - end" cr
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
    \ cr ." action-add-sample - start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-sample

    cr ." Act: " dup action-get-inst-id . space ." adding sample: " over .sample cr

    over sample-get-initial
    over action-get-squares
    square-list-find            \ smpl1 act0, sqr true | false
    if
        \ Update existing square
        rot                     \ act0 sqr smpl
        over                    \ act sqr smpl sqr
        square-add-sample       \ act sqr flag
        if
            swap
            2dup
            _action-check-incompatible-pairs
            2dup
            _action-check-square
            action-get-groups
            group-list-check-square
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
        2dup                    \ sqr act0 sqr act0
        _action-check-square    \ sqr act0
        dup action-get-groups   \ sqr act0 grp-lst
        2 pick                  \ sqr act0 grp-lst sqr
        square-get-state        \ sqr act0 grp-lst sta
        over                    \ sqr act0 grp-lst sta grp-lst
        group-list-state-in-group   \ sqr act0 grp-lst flag
        0= if
            \ Check if this is the first square
                                \ sqr act0 grp-lst
            over action-get-squares list-get-length \ sqr act0 grp-lst len
            1 =   \ sqr act0 grp-lst flag

            if
                \ Add max region with first square.
                                        \ sqr act0 grp-lst
                rot                     \ act0 grp-lst sqr
                list-new                \ act0 grp-lst sqr sqr-lst
                tuck                    \ act0 grp-lst sqr-lst sqr sqr-lst
                square-list-push        \ act0 grp-lst sqr-lst
                cur-domain-xt execute   \ act0 grp-lst sqr-lst dom
                domain-get-max-region-xt execute   \ act0 grp-lst sqr-lst mreg
                group-new               \ act0 grp-lst grp
                swap                    \ act0 grp grp-lst
                group-list-push         \ act0
                drop
            else
                2drop drop
            then
        else
            2 pick              \ sqr act0 grp-lst sqr
            swap                \ sqr act0 sqr grp-lst
            group-list-add-square
            2drop
        then
    then
    \ cr ." action-add-sample - end" cr
;

\ Return true if two actions are equal.
: action-eq ( act1 act0 -- flag )
     \ Check args.
    assert-tos-is-action
    assert-nos-is-action

    action-get-inst-id
    swap
    action-get-inst-id
    =
;

\ Get a sample from an action.
\ Call only from session-get-sample to domain-get-sample
\ since current-domain and current-action need to be set first.
: action-get-sample ( act0 -- smpl )
    \ cr ." action-get-sample - start" cr
     \ Check args.
    assert-tos-is-action
    cr ." Act: " dup action-get-inst-id . ." action-get-sample" cr

    cur-domain-xt execute       \ act0 dom
    domain-get-current-state-xt
    execute                     \ act0 cur
    over                        \ act0 cur act0

    over                        \ act0 cur act0 cur
    over action-get-squares     \ act0 cur act0 cur sqr-lst
    square-list-find            \ act0 cur act0, sqrx true | false

    if                          \ act0 cur act0 sqrx
        square-get-last-result  \ act0 cur act0 rslt
        -rot                    \ act0 rslt cur act0
        true -rot               \ act0 rslt true cur act0
        action-get-function     \ act0 rslt true cur xt
        execute                 \ act0 smpl
        nip                     \ smpl
    else                        \ act0 cur act0
                                \ act0 cur act0
        0 -rot                  \ act0 0 cur act0
        0 -rot                  \ act0 0 0 cur act0
        action-get-function     \ act0 0 0 cur xt
        execute                 \ act0 smpl
        tuck swap               \ smpl smpl act0
        action-add-sample       \ smpl
    then
    \ cr ." action-get-sample - end" cr
;

\ Return true if a action id matches a number.
: action-id-eq ( id1 sqr0 -- flag )
    \ Check args.
    assert-tos-is-action

    action-get-inst-id
    =
;

\ Return a list of needs.
: action-get-needs ( sta1 act0 -- ned-lst )
    \ cr
    \ ." Dom: " cur-domain-xt execute domain-get-inst-id-xt execute .
    \ space ." Act: " dup action-get-inst-id . space ." get-needs for " over .value space ." TODO"
    \ cr

    list-new -rot               \ lst-ret sta1 act0
    over                        \ lst-ret sta1 act0 sta1
    over action-get-groups      \ lst-ret sta1 act0 sta1 grp-lst
    group-list-state-in-group-r \ lst-ret sta1 act0 flag
    0= if
                                \ lst-ret sta1 act0
        cur-domain-xt execute   \ lst-ret sta1 act0 dom0
        need-new                \ lst-ret ned
        over need-list-push     \ lst-ret
        exit
    then
    2drop
;

: action-calc-changes ( act0 -- cngs )
    \ Check args.
    assert-tos-is-action

    0 0 changes-new swap            \ cngs act0
    action-get-groups               \ cngs grp-lst

    list-get-links                  \ cngs link
    begin
        ?dup
    while
        dup link-get-data           \ cngs link grp
        dup group-get-pn            \ cngs link grp pn
        3 < if
                                    \ cngs link grp
            group-calc-changes      \ cngs link grp-cngs
            rot                     \ link grp-cngs cngs
            2dup changes-union      \ link grp-cngs cngs cngs'

            \ Clean up.
            swap changes-deallocate \ link grp-cngs cngs'
            swap changes-deallocate \ link cngs'
            swap                    \ cngs' link
        else
            drop                    \ cngs link
        then

        link-get-next
    repeat
;
