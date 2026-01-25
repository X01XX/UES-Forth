\ Implement a Action struct and functions.

#29717 constant action-id
    #8 constant action-struct-number-cells

\ Struct fields
0                                     constant action-header-disp               \ 16 bits, [0] struct id, [1] use count, [2] instance id (8 bits).
action-header-disp              cell+ constant action-parent-domain-disp        \ Domain pointer.
action-parent-domain-disp       cell+ constant action-squares-disp              \ A square-list
action-squares-disp             cell+ constant action-incompatible-pairs-disp   \ A region-list
action-incompatible-pairs-disp  cell+ constant action-logical-structure-disp    \ A region-list
action-logical-structure-disp   cell+ constant action-groups-disp               \ A group-list.
action-groups-disp              cell+ constant action-function-disp             \ An xt to run to get a sample.
action-function-disp            cell+ constant action-defining-regions-disp     \ Defining regions, region-list, from action-logical-structure.

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
    dup action-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        action-id =
    else
        drop false
    then
;

\ Check TOS for action, unconventional, leaves stack unchanged.
: assert-tos-is-action ( tos -- tos )
    dup is-allocated-action
    is-false if
        s" TOS is not an allocated action"
       .abort-xt execute
    then
;

' assert-tos-is-action to assert-tos-is-action-xt

\ Check NOS for action, unconventional, leaves stack unchanged.
: assert-nos-is-action ( nos tos -- nos tos )
    over is-allocated-action
    is-false if
        s" NOS is not an allocated action"
       .abort-xt execute
    then
;

' assert-nos-is-action to assert-nos-is-action-xt

\ Check 3OS for action, unconventional, leaves stack unchanged.
: assert-3os-is-action ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-action
    is-false if
        s" 3OS is not an allocated action"
       .abort-xt execute
    then
;

\ Start accessors.

\ Return the instance ID from an action instance.
: action-get-inst-id ( act0 -- u)
    \ Check arg.
    assert-tos-is-action

    \ Get inst ID.
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

    over #255 >
    abort" Invalid instance id"

    \ Set inst id.
    4c!
;

\ Return the parent domain of the action.
: action-get-parent-domain ( act0 -- dom )
    \ Check arg.
    assert-tos-is-action

    action-parent-domain-disp + \ Add offset.
    @                           \ Fetch the field.
;

' action-get-parent-domain to action-get-parent-domain-xt

\ Set the parent domain of an action.
: _action-set-parent-domain ( dom act0 -- )
    \ Check args.
    assert-tos-is-action

    action-parent-domain-disp + \ Add offset.
    !                           \ Set the field.
;

\ Return the square-list from an action instance.
: action-get-squares ( act0 -- lst )
    \ Check arg.
    assert-tos-is-action

    action-squares-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the square-list of an action instance, use only in this file.
: _action-set-squares ( lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    action-squares-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return the incompatible-pairs region-list from an action instance.
: action-get-incompatible-pairs ( addr -- lst )
    \ Check arg.
    assert-tos-is-action

    action-incompatible-pairs-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the incompatible-pairs region-list of an action instance, use only in this file.
: _action-set-incompatible-pairs ( u1 addr -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    action-incompatible-pairs-disp +    \ Add offset.
    !struct                             \ Store it.
;

\ Return the logical-structure region-list from an action instance.
: action-get-logical-structure ( addr -- lst )
    \ Check arg.
    assert-tos-is-action

    action-logical-structure-disp + \ Add offset.
    @                               \ Fetch the field.
;

\ Set the logical-structure region-list of an action instance, use only in this file.
: _action-set-logical-structure ( new-ls addr -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    \ Set new LS.
    action-logical-structure-disp + \ Add offset.
    !struct                         \ Store it.
;


\ Return the group-list from an action instance.
: action-get-groups ( act0 -- lst )
    \ Check arg.
    assert-tos-is-action

    action-groups-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the group-list of an action instance, use only in this file.
: _action-set-groups ( lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    action-groups-disp +    \ Add offset.
    !struct                 \ Set the field.
;

\ Return the function xt that implements the action.
: action-get-function ( act0 -- xt )
    \ Check arg.
    assert-tos-is-action

    action-function-disp +  \ Add offset.
    @                       \ Fetch the field.
;

\ Set the function xt that implements an action.
: _action-set-function ( xt act0 -- )
    \ Check args.
    assert-tos-is-action

    action-function-disp +  \ Add offset.
    !                       \ Set the field.
;

\ Return the defining-regions region-list from an action instance.
: action-get-defining-regions ( addr -- lst )
    \ Check arg.
    assert-tos-is-action

    action-defining-regions-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the defining-regions region-list of an action instance, use only in this file.
: _action-set-defining-regions ( u1 addr -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    action-defining-regions-disp +  \ Add offset.
    !struct                         \ Store it.
;

\ End accessors

\ Return true if a region, in the logical structure, is a defining region.
: action-region-is-defining ( reg1 act0 -- flag )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region

    2dup                                \ reg1 act0 reg1 act0
    action-get-logical-structure        \ reg1 act0 reg1 LS
    tuck                                \ reg1 act0 LS reg1 LS
    region-list-member                  \ reg1 act0 LS flag
    0= abort" Region not in logical structure"

                                        \ reg1 act0 LS
    \ Get results.
    rot                                 \ act0 LS reg1
    swap                                \ act0 reg1 LS
    region-list-region-is-defining      \ act0 bool
    nip
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

\ Update defining-regions.
\ First calc-update logical-structure, then calc defining regions.
: _action-update-defining-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list
    cr ." New DF regions: " over .region-list cr

    dup action-get-defining-regions         \ reg-lst1 act0 df-lst
    -rot                                    \ df-lst reg-lst1 act0
    _action-set-defining-regions            \ df-lst
    region-list-deallocate                  \
;

\ Calculate defining regions, from action-logical-structure.
: action-calc-defining-regions ( act0 -- df-lst )
    \ Check arg.
    assert-tos-is-action

    \ Init return list.
    list-new                                \ act0 df-lst

    \ Prep for loop.
    over action-get-logical-structure       \ act0 df-lst ls-lst
    list-get-links                          \ act0 df-lst ls-link

    begin
        ?dup
    while
        dup link-get-data                   \ act0 df-lst ls-link regx
        #3 pick                             \ act0 df-lst ls-link regx act0
        action-region-is-defining           \ act0 df-lst ls-link bool
        if
            dup link-get-data               \ act0 df-lst ls-link regx
            #2 pick                         \ act0 df-lst ls-link regx df-lst
            list-push-struct                \ act0 df-lst ls-link
        then

        link-get-next
    repeat
                                            \ act0 df-lst
    nip
;

\ Update the logical-structure region-list of an action instance, use only in this file.
\ Deallocate the old list last, so the instance field is never invalid.
: _action-update-logical-structure ( new-ls act0 -- )
    \ cr ." _action-update-logical-structure: start"  cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list
    cr ." New LS regions: " over .region-list cr

    \ Check the new list is different from the old list.
    over                                \ new-lst act0 new-lst
    over action-get-logical-structure   \ new-lst act0 new-lst old-lst'
    2dup region-list-eq                 \ new-lst act0 new-lst old-lst' flag
    if cr ." region lists equal?" cr then
    nip                                 \ new-lst act0 old-lst'

    \ Get/save current LS.
    cr ." old list " dup .region-list cr

    -rot                                \ old-lst' new-lst act0

    \ Store new structure.
    2dup                                \ old-lst' new-lst act0 new-lst act0
    _action-set-logical-structure       \ old-lst' new-lst act0

    \ Save action, for now..
    -rot                                \ act0 old-lst' new-lst

    \ Get old regions that are deleted.
    2dup                                \ act0 old-lst' new-lst old-lst' new-lst
    region-list-set-difference          \ act0 old-lst' new-lst old-gone'
    cr ." Old LS regions deleted: " dup .region-list cr

    \ Scan deleted regions.
    dup list-get-links                   \ act0 old-lst' new-lst old-gone' link
    begin
        ?dup
    while
        dup link-get-data               \ act0 old-lst' new-lst old-gone' link old-ls-reg

        \ If group exists, delete it.
        #5 pick                         \ act0 old-lst' new-lst old-gone' link old-ls-reg act0
        _action-delete-group-if-exists  \ act0 old-lst' new-lst old-gone' link flag
        if
            cr #4 spaces dup link-get-data .region
            space ." deleted group"
        then

        link-get-next                   \ act0 old-lst' new-lst old-gone' link
    repeat
    cr
                                        \ act0 old-lst' new-lst old-gone'
    region-list-deallocate              \ act0 old-lst' new-lst

    \ Display new regions.
    dup                                 \ act0 old-lst' new-lst new-lst
    #2 pick                             \ act0 old-lst' new-lst new-lst old-lst'

    region-list-set-difference          \ act0 old-lst' new-lst new-added'
    cr ." New LS regions added: " dup .region-list cr
    region-list-deallocate              \ act0 old-lst' new-lst
    drop                                \ act0 old-lst'
    region-list-deallocate              \ act0

    \ Calc and store defining regions.
    dup action-calc-defining-regions        \ act0 df-lst
    2dup swap                               \ act0 df-lst df-lst act0
    _action-update-defining-regions         \ act0 df-lst 

    \ Scan for new regions.
    list-get-links                          \ act0 link
    begin
        ?dup
    while
        dup link-get-data                   \ act0 link region

        cr #4 spaces ." defining region: " dup .region

        #2 pick                             \ act0 link reg act0
        action-get-groups                   \ act0 link reg grps
        group-list-member                   \ act0 link flag
        if                                  \ act0 link
            space ." group already exists"
        else
            dup link-get-data                   \ act0 link reg
            #2 pick                             \ act0 link reg act0
            action-get-squares                  \ act0 link reg sqr-lst1
            square-list-in-region               \ act0 link sqr-lst2
            dup list-is-empty                   \ act0 link sqr-lst2 flag
            if                                  \ act0 link sqr-lst2
                space ." no squares found "
                list-deallocate
            else
                dup                             \ act0 link sqr-lst2 sqr-lst2
                square-list-get-rules           \ act0 link sqr-lst2, ruls true | false
                if                              \ act0 link sqr-lst2 ruls
                    rulestore-deallocate        \ act0 link sqr-lst2
                    over link-get-data          \ act0 link sqr-lst2 reg
                    group-new                   \ act0 link grp
                    #2 pick action-get-groups   \ act0 link grp grp-lst
                    group-list-push             \ act0 link
                else                            \ act0 link sqr-lst2
                    space ." rules not found, must be a problem"
                    square-list-deallocate      \ act0 link

                    \ If group exists, delete it.
                    dup link-get-data               \ act0 link region
                    #2 pick                         \ act0 link region act0
                    _action-delete-group-if-exists  \ act0 link flag
                    if                              \ act0 link
                        space ." deleted group"
                    then
                then
            then
        then

        link-get-next                   \ act0 link
    repeat
    cr
                                        \ act0
    \ Clean up.
    drop                                \
    \ cr ."  _action-update-logical-structure: end" cr
;

\ End accessors.

\ Create an action, given an xt to get a sample.
\ The instance ID defaults to zero.
\ It will likely be reset to match its position in a list, using action-set-inst-id,
\ which avoids duplicates and may be useful as an index into the list.
: action-new ( xt1 dom0 -- addr)
    \ Check arg.
    assert-tos-is-domain-xt execute

    \ Allocate space.
    action-mma mma-allocate             \ xt1 dom0 actr

    \ Store struct id.
    action-id over                      \ xt1 dom0 act id act
    struct-set-id                       \ xt1 dom0 act

    \ Init use count.
    0 over struct-set-use-count         \ xt1 dom0 act

    \ Set instance ID.
    over                                \ xt1 dom0 act dom0
    domain-get-number-actions-xt        \ xt1 dom0 act dom0 xt
    execute                             \ xt1 dom0 act na
    over action-set-inst-id             \ xt1 dom0 act

    \ Set xt
    rot over                            \ dom0 act xt1 act
    _action-set-function                \ dom0 act

    \ Set squares list.
    list-new                            \ dom0 act lst
    over _action-set-squares            \ dom0 act

    \ Set incompatible-pairs list.
    list-new                            \ dom0 act lst
    over _action-set-incompatible-pairs \ dom0 act

    \ Get max region.
    over domain-get-num-bits-xt         \ dom0 act xt
    execute                             \ dom0 act nb
    all-bits                            \ dom0 act all-bits
    0 region-new2                       \ dom0 act mx-reg

    \ Set logical-structure list.
    dup                                 \ dom0 act mx-reg mx-reg
    list-new                            \ dom0 act mx-reg mx-reg lst
    tuck                                \ dom0 act mx-reg lst mx-reg lst
    list-push-struct                    \ dom0 act mx-reg lst
    #2 pick                             \ dom0 act mx-reg lst act
    _action-set-logical-structure       \ dom0 act mx-reg

    \ Set defining-regions list.
    list-new                            \ dom0 act mx-reg lst
    tuck                                \ dom0 act lst mx-reg lst
    list-push-struct                    \ dom0 act lst
    over                                \ dom0 act lst act
    _action-set-defining-regions        \ dom0 act

    \ Set parent-domain.
    tuck                                \ act dom0 act
    _action-set-parent-domain           \ act

    \ Set group list.
    list-new                            \ act lst
    over _action-set-groups             \ act
;

\ Print a action.
: .action ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    dup action-get-inst-id
    cr #5 spaces ." Act: " .

    dup action-get-squares
    dup list-get-length
    ." num sqrs: " dec.
    ." sqrs " .square-list-states

    dup action-get-logical-structure cr #7 spaces ." LS: " .region-list
    dup action-get-incompatible-pairs cr #7 spaces ." IP: " .region-list
    dup action-get-defining-regions cr #7 spaces ." DF: " .region-list

    \ cr ." Groups: "
    \ Print each group.
    action-get-groups list-get-links
    begin
        ?dup
    while
        dup link-get-data
        cr #10 spaces .group
        link-get-next
    repeat
    cr
;

: .action-id ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    action-get-inst-id
    .value
;

\ Deallocate a action.
: action-deallocate ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    dup struct-get-use-count      \ act0 count

    #2 <
    if
        \ Clear fields.
        dup action-get-squares square-list-deallocate
        dup action-get-incompatible-pairs region-list-deallocate
        dup action-get-logical-structure region-list-deallocate
        dup action-get-defining-regions region-list-deallocate
        dup action-get-groups group-list-deallocate

        \ Deallocate instance.
        action-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Get a list of incompatible pairs, as regions, no supersets, given a square.
: action-find-incompatible-pairs-for-square ( sqr1 act0 -- square-list )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    list-new -rot                               \ ret-lst sqr1 act0
    2dup action-get-squares                     \ ret-lst sqr1 act0 sqr1 sqr-lst
    [ ' square-incompatible ] literal -rot      \ ret-lst sqr1 act0 xt sqr1 sqr-lst
    list-find-all                               \ ret-lst sqr1 act0 inc-lst

    dup list-is-empty
    if
        list-deallocate                         \ ret-lst sqr1 act0
        2drop                                   \ ret-lst
        \ cr ." action-find-incompatible-pairs-for-square: end 1" cr
        exit
    then

    #2 pick square-get-state                \ ret-lst sqr1 act0 inc-lst sta1
    over list-get-links                     \ ret-lst sqr1 act0 inc-lst sta1 link
    begin
        ?dup
    while
        dup link-get-data square-get-state  \ ret-lst sqr1 act0 inc-lst sta1 link sta2
        #2 pick                             \ ret-lst sqr1 act0 inc-lst sta1 link sta2 sta1
        region-new                          \ ret-lst sqr1 act0 inc-lst sta1 link regx

        dup                                 \ ret-lst sqr1 act0 inc-lst sta1 link regx regx
        #7 pick                             \ ret-lst sqr1 act0 inc-lst sta1 link regx regx ret-lst
        region-list-push-nosups             \ ret-lst sqr1 act0 inc-lst sta1 link regx flag
        if
            drop
        else
            region-deallocate
        then
                                            \ ret-lst sqr1 act0 inc-lst sta1 link

        link-get-next                       \ ret-lst sqr1 act0 inc-lst sta1 link-next
    repeat
                                            \ ret-lst sqr1 act0 inc-lst sta1
    drop                                    \ ret-lst sqr1 act0 inc-lst
    list-deallocate                         \ ret-lst sqr1 act0

    drop                                    \ ret-lst sqr1
\    over list-is-empty
\    if
\        cr
\        ." Dom: " current-domain-id dec.
\        space ." Act: " current-action-id dec.
\        space ." for square: " square-get-state .value
\        space ." incompatible pairs: " dup .region-list
\        cr
\    then
    drop
;

\ Check a new, or changed square.
\ Could affect action-incompatible-pairs and action-logical-structure.
: _action-check-square ( sqr1 act0 -- )
    \ cr ." _action-check-square: start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    \ Check action-incompatible-pairs for pairs that are no longer incompatible.
    \ If any are found, remove them and recalculate everything.

    \ Form regions with incompatible squares, no supersets.
    tuck                                            \ act0 sqr1 act0
    action-find-incompatible-pairs-for-square       \ act0 inc-lst'
    dup list-is-empty
    if
        \ cr ." _action-check-square: list is empty" cr
        list-deallocate
        drop
        exit
    then

    \ If there is no subset region in action-incompatible-pairs,
    \ push nosups, calc ~A + ~B, intersect with action-logical-structure.

                                                    \ act0 inc-lst'
    dup list-get-links                              \ act0 inc-lst' inc-link
    begin
        ?dup                                        \ act0 inc-lst' inc-link
    while
        dup link-get-data                           \ act0 inc-lst' inc-link regx

        \ Check if pair should be added.

        \ Check defining regions.
        [ ' region-superset-of ] literal            \ act0 inc-lst' inc-link regx xt
        over                                        \ act0 inc-lst' link regx xt regx
        #5 pick                                     \ act0 inc-lst' link regx xt regx act0
        action-get-logical-structure                \ act0 inc-lst' link regx xt regx ls-lst
        list-member                                 \ act0 inc-lst' link regx bool

        if
            \ Add region to the action-incompatible-pairs  list.
            cr
            ." Dom: " current-domain-id dec.
            ." Act: " current-action-id dec.
            space ." Adding incompatible pair: " dup region-get-states .value space .value
            cr

            dup                                     \ act0 inc-lst link regx regx
            #4 pick                                 \ act0 inc-lst link regx regx act0
            action-get-incompatible-pairs           \ act0 inc-lst link regx regx inc-lst
            region-list-push-nosups                 \ act0 inc-lst link regx flag
            drop

            \ Calc regions possible for incompatible pair.
            region-get-states                       \ act0 inc-lst link s0 s1
            #4 pick                                 \ act0 inc-lst link s0 s1 act0
            action-get-parent-domain                \ act0 inc-lst link s0 s1 dom
            domain-state-pair-complement-xt         \ act0 inc-lst link s0 s1 dom xt
            execute                                 \ act0 inc-lst link reg-lst

            \ Calc new action-logical-structure.
            #3 pick action-get-logical-structure    \ act0 inc-lst link reg-lst lsl-lst
            2dup                                    \ act0 inc-lst link reg-lst lsl-lst reg-lst lsl-lsn
            region-list-intersections-nosubs        \ act0 inc-lst link reg-lst lsl-lst new-reg-lst

            \ Set new action-logical-structure.
            #5 pick                                 \ act0 inc-lst link reg-lst lsl-lst new-reg-lst act0
            _action-update-logical-structure        \ act0 inc-lst link reg-lst lsl-lst
            drop                                    \ act0 inc-lst link reg-lst
            region-list-deallocate                  \ act0 inc-lst link
        else
            drop                                    \ act0 inc-lst link
        then

        link-get-next                               \ act0 inc-lst sta1 link-next
    repeat
                                                    \ act0 inc-lst

    region-list-deallocate
    drop
    \ cr ." _action-check-square: end" cr
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
    \ cr ." _action-not-incompatible-pairs: start" cr
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
        ?dup
    while
        dup link-get-data       \ ret-lst act0 link region
        dup                     \ ret-lst act0 link region region

        region-get-states       \ ret-lst act0 link region s1 s0

        \ Get square 0.
        #4 pick                 \ ret-lst act0 link region s1 s0 act0
        action-find-square      \ ret-lst act0 link region s1 result
        0=
        abort" square not found?"

        swap                    \ ret-lst act0 link region sqr0 s1

        \ Get square 1.
        #4 pick                 \ ret-lst act0 link region sqr0 s1 act0
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
            #3 pick             \ ret-lst act0 link region ret-lst
            region-list-push    \ ret-lst act0 link
        then

        link-get-next
    repeat
                                \ ret-lst act0
    drop                        \ ret-lst
    \ cr ." _action-not-incompatible-pairs: end" cr
;

\ Recalc action-logical-structure from action-incompatible-pairs.
:  _action-recalc-logical-structure ( act0 -- )
    \ cr ." _action-recalc-logical-structure: start" cr
    \ Check args.
    assert-tos-is-action

    \ Init new logical-structure region list.
    list-new                                \ act0 ls-new
    over                                    \ act0 ls-new act0
    action-get-parent-domain                \ act0 ls-new dom
    domain-get-max-region-xt execute        \ act0 ls-new max-reg
    over region-list-push                   \ act0 ls-new

    #2 pick                                 \ act0 ls-new act0
    action-get-incompatible-pairs           \ act0 ls-new i-pairs

    list-get-links                          \ act0 ls-new link
    begin
        ?dup
    while
        \ Get next ~A + ~B region list.
        dup link-get-data                   \ act0 ls-new link region
        region-get-states                   \ act0 ls-new link s1 s0
        #4 pick                             \ act0 ls-new link s1 s0 act0
        action-get-parent-domain            \ act0 ls-new link s1 s0 dom
        domain-state-pair-complement-xt     \ act0 ls-new link s1 s0 dom xt
        execute                             \ act0 ls-new link nanb-lst

        \ Intersect with most recent logical-structure region list.
        rot                                 \ act0 link nanb-lst ls-new
        2dup                                \ act0 link nanb-lst ls-new nanb-lst ls-new
        region-list-intersections-nosubs    \ act0 link nanb-lst ls-new ls-new-new

        \ Deallocate previous region lists.
        swap                                \ act0 link nanb-lst ls-new-new ls-new
        region-list-deallocate              \ act0 link nanb-lst ls-new-new
        swap                                \ act0 link ls-new-new nanb-lst
        region-list-deallocate              \ act0 link ls-new-new

        \ Prep for next cycle.
        swap                                \ act0 ls-new-new link

        link-get-next
    repeat
                                            \ act0 ls-new

    \ Store new LS.
    swap                                    \ ls-new act0
    _action-update-logical-structure        \
    \ cr ." _action-recalc-logical-structure: end" cr
;

\ Check if a changed square is between two incompatible square pairs,
\ and is incompatible with either.
\ It is assumed the state is not equal to any state defining any region in the incompatible pair list.
: _action-check-incompatible-pairs2 ( sqr1 act0 -- )
    \ cr ." _action-check-incompatible-pairs2: Act: " dup .action-id space ." sqr: " over .square-state cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    over square-get-state                   \ sqr1 act0 | sta
    over action-get-incompatible-pairs      \ sqr1 act0 | sta ip-lst
    region-list-regions-state-in            \ sqr1 act0 | reg-in-lst
    dup list-is-empty                       \ sqr1 act0 | reg-in-lst flag
    if
        list-deallocate
        2drop
        \ cr ." _action-check-incompatible-pairs: exit early" cr
        exit
    then

    \ Check each region.
    dup list-get-links                          \ sqr1 act0 | reg-in-lst link
    begin
        ?dup
    while
        \ Check sqr1 against region state 0.
        dup link-get-data                   \ sqr1 act0 | reg-in-lst link regx
        region-get-state-0                  \ sqr1 act0 | reg-in-lst link s0
        #3 pick action-find-square          \ sqr1 act0 | reg-in-lst link, r-sqr t | f
        0= abort" square not found?"
        #4 pick                             \ sqr1 act0 | reg-in-lst link r-sqr sqr1
        square-compare                      \ sqr1 act0 | reg-in-lst link char
        [char] I =                          \ sqr1 act0 | reg-in-lst link flag
        if                                  \ sqr1 act0 | reg-in-lst link
            cr ." sqrs incompat 0" cr
            \ Add new incompatible pair.
            #3 pick square-get-state        \ sqr1 act0 | reg-in-lst link sta1
            over link-get-data              \ sqr1 act0 | reg-in-lst link sta1 regx
            region-get-state-0              \ sqr1 act0 | reg-in-lst link sta1 s0
            region-new dup                  \ sqr1 act0 | reg-in-lst link reg-new reg-new
            #4 pick                         \ sqr1 act0 | reg-in-lst link reg-new reg-new act0
            action-get-incompatible-pairs   \ sqr1 act0 | reg-in-lst link reg-new reg-new ip-lst
            region-list-push-nosups         \ sqr1 act0 | reg-in-lst link reg-new flag
            if
                drop
            else
                region-deallocate
            then
        else
            cr ." sqrs NOT incompat 0" cr
        then

        \ Check sqr1 against region state 1.
        dup link-get-data                   \ sqr1 act0 | reg-in-lst link regx
        region-get-state-1                  \ sqr1 act0 | reg-in-lst link s1
        #3 pick action-find-square          \ sqr1 act0 | reg-in-lst link, r-sqr t | f
        0= abort" square not found?"
        #4 pick                             \ sqr1 act0 | reg-in-lst link r-sqr sqr1
        square-compare                      \ sqr1 act0 | reg-in-lst link char
        [char] I =                          \ sqr1 act0 | reg-in-lst link flag
        if                                  \ sqr1 act0 | reg-in-lst link
            cr ." sqrs incompat 1" cr
            \ Add new incompatible pair.
            #3 pick square-get-state        \ sqr1 act0 | reg-in-lst link sta1
            over link-get-data              \ sqr1 act0 | reg-in-lst link sta1 regx
            region-get-state-1              \ sqr1 act0 | reg-in-lst link sta1 s1
            region-new dup                  \ sqr1 act0 | reg-in-lst link reg-new reg-new
            #4 pick                         \ sqr1 act0 | reg-in-lst link reg-new reg-new act0
            action-get-incompatible-pairs   \ sqr1 act0 | reg-in-lst link reg-new reg-new ip-lst
            region-list-push-nosups         \ sqr1 act0 | reg-in-lst link reg-new flag
            if
                drop
            else
                region-deallocate
            then
        else
            cr ." sqrs NOT incompat 1" cr
        then

        link-get-next
    repeat

    region-list-deallocate                  \ sqr1 act0
    nip                                     \ act0
    _action-recalc-logical-structure
;

\ Check incompatble pairs are still incompatible,
\ given a square that has recently changed pn or pnc.
: _action-check-incompatible-pairs ( sqr1 act0 -- )
    \ cr ." _action-check-incompatible-pairs: Act: " dup .action-id space ." sqr: " over .square-state cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    \ Get regions that use the state
    over square-get-state               \ sqr1 act0 sta
    over                                \ sqr1 act0 sta act0
    action-get-incompatible-pairs       \ sqr1 act0 sta ip-lst
    region-list-uses-state              \ sqr1 act0 reg-lst-in

    dup list-is-empty                   \ sqr1 act0 reg-lst-in flag
    if
        list-deallocate                 \ sqr1 act0
        _action-check-incompatible-pairs2
       \  cr ." _action-check-incompatible-pairs: end 1" cr
        exit
    else                                \ sqr1 act0 reg-lst-in
        rot drop                        \ act0 reg-lst-in
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
       \  cr ." _action-check-incompatible-pairs: end 2" cr
        exit
    then

    \ Some not-incomptible pairs found.
                                        \ act0 reg-lst-in reg-lst-not-i
    swap region-list-deallocate         \ act0 reg-lst-not-i

    \ Remove regions.
    dup                                 \ act0 reg-lst-not-i reg-lst-not-i
    list-get-links                      \ act0 reg-lst-not-i link
    begin
        ?dup
    while
        dup link-get-data               \ act0 reg-lst-not-i link region
        cr ." state " dup region-get-states .value space ." and " .value space ." are no longer incompatible" cr
        [ ' region-eq ] literal swap    \ act0 reg-lst-not-i link xt region
        #4 pick                         \ act0 reg-lst-not-i link xt region act0
        action-get-incompatible-pairs   \ act0 reg-lst-not-i link xt region pair-list
        list-remove                     \ act0 reg-lst-not-i link reg? flag
        0=
        abort" Region not found?"

        region-deallocate

        link-get-next
    repeat
                                        \ act0 reg-lst-not-i
    region-list-deallocate              \ act0

    \ Recalc logical-structure
    _action-recalc-logical-structure
    \ cr ." _action-check-incompatible-pairs: end 3" cr
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
    \ cr ." action-add-sample: start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-sample

    cr ." Act: " dup action-get-inst-id dec. space ." adding sample: " over .sample cr

    over sample-get-initial     \ smpl1 act0 s-i
    over action-get-squares     \ smpl1 act0 s-i sqr-lst
    square-list-find            \ smpl1 act0, sqr true | false
    if
                                \ smpl1 act0 sqr
        \ Update existing square
        rot                     \ act0 sqr smpl1
        over                    \ act0 sqr smpl1 sqr
        square-add-sample       \ act0 sqr flag
        if
            swap                \ sqr act0
            2dup                \ sqr act0 sqr act0
            _action-check-incompatible-pairs    \ sqr act0
            2dup                        \ sqr act0 sqr act0
            _action-check-square        \ sqr act0
            action-get-groups           \ sqr grp-lst
            group-list-check-square     \
        else
            2drop
        then
    else                        \ smpl1 act0
        \ Add new square.
        swap                    \ act0 smpl1
        square-from-sample      \ act0 sqr
        dup                     \ act0 sqr sqr
        #2 pick                 \ act0 sqr sqr act0
        action-get-squares      \ act0 sqr sqr sqrlst
        square-list-push        \ act0 sqr
        swap                    \ sqr act0
        2dup                    \ sqr act0 sqr act0
        _action-check-square    \ sqr act0
        dup action-get-groups   \ sqr act0 grp-lst
        #2 pick                 \ sqr act0 grp-lst sqr
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
                rot                         \ act0 grp-lst sqr
                list-new                    \ act0 grp-lst sqr sqr-lst
                tuck                        \ act0 grp-lst sqr-lst sqr sqr-lst
                square-list-push            \ act0 grp-lst sqr-lst
                #2 pick                     \ act0 grp-lst sqr-lst act0
                action-get-parent-domain    \ act0 grp-lst sqr-lst dom
                domain-get-max-region-xt    \ act0 grp-lst sqr-lst xt
                execute                     \ act0 grp-lst sqr-lst mreg
                group-new                   \ act0 grp-lst grp
                swap                        \ act0 grp grp-lst
                group-list-push             \ act0
                drop
            else
                3drop
            then
        else
            #2 pick             \ sqr act0 grp-lst sqr
            swap                \ sqr act0 sqr grp-lst
            group-list-add-square
            2drop
        then
    then
    \ cr ." action-add-sample: end" cr
;

\ Return true if a state is confirmed with a pnc square.
: action-state-confirmed ( sta1 act0 -- flag )
     \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    action-get-squares      \ sta1 sqr-lst
    square-list-find        \ sqr t | f
    if
        square-get-pnc      \ pnc
    else
        false               \ false
    then
;

\ Return true if a region is confirmed with two pnc squares.
: action-region-confirmed ( reg1 act0 -- flag )
     \ Check args.
    assert-tos-is-action
    assert-nos-is-region

    over region-get-state-0     \ reg1 act0 reg-sta-0
    over action-state-confirmed \ reg1 act0 flag
    0= if
        2drop false
        exit
    then

    swap region-get-state-1     \ act0 reg-sta-1
    swap action-state-confirmed \ flag
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
: action-get-sample ( sta1 act0 -- smpl )
    \ cr ." action-get-sample: start" cr
     \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    \ cr ."action-get-sample: Act: " dup action-get-inst-id . cr

    tuck                        \ act0 sta1 act0

    over                        \ act0 sta1 act0 sta1
    over action-get-squares     \ act0 sta1 act0 sta1 sqr-lst
    square-list-find            \ act0 sta1 act0, sqrx true | false

    if                          \ act0 sta1 act0 sqrx
        square-get-last-result  \ act0 sta1 act0 rslt
        -rot                    \ act0 rslt sta1 act0
        true -rot               \ act0 rslt true sta1 act0
        action-get-function     \ act0 rslt true sta1 xt
        execute                 \ act0 smpl
        tuck                    \ smpl act0 smpl
        swap                    \ smpl smpl act0
        action-add-sample       \ smpl
    else                        \ act0 sta1 act0
                                \ act0 sta1 act0
        0 -rot                  \ act0 0 sta1 act0
        0 -rot                  \ act0 0 0 sta1 act0
        action-get-function     \ act0 0 0 sta1 xt
        execute                 \ act0 smpl
        tuck swap               \ smpl smpl act0
        action-add-sample       \ smpl
    then
    \ cr ." action-get-sample: end" cr
;

\ Return true if a action id matches a number.
: action-id-eq ( id1 sqr0 -- flag )
    \ Check arg.
    assert-tos-is-action

    action-get-inst-id
    =
;

\ Return a need given a need number and target.
\ Check target is not a pnc square.
: action-make-need ( typ2 sta1 act0 -- need )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value
    assert-3os-is-need-number

    2dup                    \ typ2 sta1 act0 sta1 act0
    action-find-square      \ typ2 sta1 act0, sqr t | f
    if
        square-get-pnc      \ typ2 sta1 act0 bool
        if
            cr
            ." Act: " action-get-inst-id dec. space
            ." targ: " .value space
            ." need type: " . space
            ." square pnc?"
            cr
            abort
        then
    then
    dup action-get-parent-domain    \ typ2 sta1 act0 domx
    need-new                        \ need
;

' action-make-need to action-make-need-xt

\ Given a state and a region, get samples of adjacent, external states,
\ unless one turns out to be compatible.
: action-get-adjacent-state-needs ( sta2 reg1 act0 -- ned-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region
    assert-3os-is-value

    \ Get bits to change.
    swap region-edge-mask                   \ sta2 act0 edg-msk
    \ Check for all-X region.
    dup 0=
    if
        3drop
        list-new
        exit
    then

    value-split                             \ sta2 act0 msk-lst1'
    swap                                    \ sta2 msk-lst1' act0

    \ Init return list.
    list-new                                \ sta2 msk-lst1' act0 ned-lst

    \ Get base square.
    #3 pick                                 \ sta2 msk-lst1' act0 ned-lst sta2
    #2 pick                                 \ sta2 msk-lst1' act0 ned-lst sta2 act0
    action-find-square                      \ sta2 msk-lst1' act0 ned-lst, sqr2 t | f
    is-false abort" action-get-adjacent-state-needs: sqr2 not found?"
    
    #3 pick list-get-links                  \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link

    begin
        ?dup
    while
        dup link-get-data                   \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link mskx
        #6 pick                             \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link mskx sta2
        xor                                 \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link stax

        dup                                 \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link stax stax
        #5 pick                             \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link stax act0
        action-find-square                  \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link stax, sqrx t | f
        if
            dup square-get-pnc              \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link stax sqrx pnc
            if
                2drop
            else
                \ Add need for another sample.
                drop                        \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link stax
                #6 swap                     \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link type-6 stax
                #5 pick                     \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link type-6 stax act0
                action-make-need            \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link nedx
                #3 pick                     \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link nedx ret-lst
                list-push-struct            \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link
            then
        else                                \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link stax
            \ Add need for first sample.
            #6 swap                     \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link #6 stax
            #5 pick                     \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link #6 stax act0
            action-make-need            \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link nedx
            #3 pick                     \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link nedx ret-lst
            list-push-struct            \ sta2 msk-lst1' act0 ned-lst sqr2 msk-link
        then

        link-get-next
    repeat
                                        \ sta2 msk-lst1' act0 ned-lst sqr2
    drop                                \ sta2 msk-lst1' act0 ned-lst
    nip                                 \ sta2 msk-lst1' ned-lst
    swap list-deallocate                \ sta2 ned-lst
    nip                                 \ ned-lst
;

\ return a list of states, in incompatible pair regions, that are not in defining regions.
: action-incompatible-pair-states-not-in-defining-regions ( act0 -- sta-lst )
    \ Check arg.
    assert-tos-is-action
    dup action-get-incompatible-pairs       \ act0 inc-lst
    region-list-states                      \ act0 sta-lst'
    dup                                     \ act0 sta-lst' sta-lst'
    #2 pick                                 \ act0 sta-lst' sta-lst' act0
    action-get-defining-regions             \ act0 sta-lst' sta-lst' df-lst
    region-list-states-not-in               \ act0 sta-lst' sta-lst''
    swap list-deallocate                    \ act0 sta-lst''
    nip
    \ cr ." action-incompatible-pair-states-not-in-defining-regions: returns: " dup .value-list cr
;

: action-states-in-fewest-ls-regions ( sta-lst1 act0 -- sta-lst )
    \ Check arg.
    assert-tos-is-action
    assert-nos-is-value-list
    \ cr ." action-states-in-fewest-ls-regions: start " over .value-list cr

    \ Get the logical-structure list.
    dup action-get-logical-structure    \ sta-lst1 act0 ls-lst

    \ Init minimum number regions in.
    #99999                              \ sta-lst1 act0 ls-lst min-num

    \ Check each state against the logical structure list.
    #3 pick                             \ sta-lst1 act0 ls-lst min-num sta-lst1
    list-get-links                      \ sta-lst1 act0 ls-lst min-num sta-link

    begin
        ?dup
    while
        dup link-get-data                   \ sta-lst1 act0 ls-lst min-num sta-link stax
        #3 pick                             \ sta-lst1 act0 ls-lst min-num sta-link stax ls-lst
        region-list-number-regions-state-in \ sta-lst1 act0 ls-lst min-num sta-link num-in
        rot min swap                        \ sta-lst1 act0 ls-lst min-num sta-link

        link-get-next
    repeat
                                        \ sta-lst1 act0 ls-lst min-num
    \ cr ." min: " dup . cr

    \ Init return list.
    list-new                            \ sta-lst1 act0 ls-lst min-num ret-lst

    \ Check each state for min number regions in.
    #4 pick                                 \ sta-lst1 act0 ls-lst min-num ret-lst sta-lst
    list-get-links                          \ sta-lst1 act0 ls-lst min-num ret-lst sta-link

    begin
        ?dup
    while
        dup link-get-data                   \ sta-lst1 act0 ls-lst min-num ret-lst sta-link stax
        #4 pick                             \ sta-lst1 act0 ls-lst min-num ret-lst sta-link stax ls-lst
        region-list-number-regions-state-in \ sta-lst1 act0 ls-lst min-num ret-lst sta-link num-in
        #3 pick                             \ sta-lst1 act0 ls-lst min-num ret-lst sta-link num-in min-num
        =                                   \ sta-lst1 act0 ls-lst min-num ret-lst sta-link bool
        if
            dup link-get-data               \ sta-lst1 act0 ls-lst min-num ret-lst sta-link stax
            #2 pick                         \ sta-lst1 act0 ls-lst min-num ret-lst sta-link stax ret-lst
            list-push                       \ sta-lst1 act0 ls-lst min-num ret-lst sta-link
        then

        link-get-next
    repeat
                                            \ sta-lst1 act0 ls-lst min-num ret-lst
    2nip                                    \ sta-lst1 min-num ret-lst
    nip nip                                 \ ret-lst

   \  cr ." action-states-in-fewest-ls-regions: returns: " dup .value-list cr
;

\ Look for states, in the incompatible pair list, that ar not in a defining region.
\ Generate needs for those states that are in the fewest number of left-over
\ regions.
: action-possible-corner-needs ( act0 -- ned-lst )
    \ Check arg.
    assert-tos-is-action
    \ cr ." action-possible-corner-needs: act: " dup action-get-inst-id . cr

    \ Get states that may define a corner, not yet understood.
    dup action-incompatible-pair-states-not-in-defining-regions \ act0 sta-lst'
    dup list-is-empty
    if
        nip
        exit
    then

    \ Get states in the least logical structure regions.
    dup #2 pick                                 \ act0 sta-lst' sta-lst' act0
    action-states-in-fewest-ls-regions          \ act0 sta-lst' sta-lst''
    swap list-deallocate                        \ act0 sta-lst''

    \ Prep for loop.
    list-new swap                               \ act0 ned-lst sta-lst''
    #2 pick action-get-logical-structure swap   \ act0 ned-lst ls-lst sta-lst''
    dup list-get-links                          \ act0 ned-lst ls-lst sta-lst'' sta-link

    \ For each state in fewest non-defining regions.
    begin
        ?dup
    while
        dup link-get-data dup                   \ act0 ned-lst ls-lst sta-lst'' sta-link stax stax
        #4 pick                                 \ act0 ned-lst ls-lst sta-lst'' sta-link stax stax ls-lst
        region-list-regions-state-in            \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst'

        \ Prep for loop.
        dup list-get-links                      \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link
        begin
            ?dup
        while
            #2 pick                             \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link stax
            over link-get-data                  \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link stax regx
        \ cr ." eval " dup .region space ." and " over .value
            #9 pick                             \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link stax regx act0
            action-get-adjacent-state-needs     \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link ned-lst'
        \ space ." adj needs: " dup .need-list cr
            dup                                 \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link ned-lst' ned-lst'
            #8 pick                             \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link ned-lst' ned-lst' ned-lst
            need-list-append                    \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link ned-lst'
            need-list-deallocate                \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst' reg-link

            link-get-next
        repeat
                                                \ act0 ned-lst ls-lst sta-lst'' sta-link stax reg-lst'
        region-list-deallocate drop             \ act0 ned-lst ls-lst sta-lst'' sta-link

        link-get-next
    repeat
                                                \ act0 ned-lst ls-lst sta-lst''
    list-deallocate                             \ act0 ned-lst ls-lst
    drop                                        \ act0 ned-lst
    nip
;

\ Return a list of needs for an action, given the current state
\ and the reachable region.
: action-get-needs ( reg1 sta1 act0 -- ned-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value
    assert-3os-is-region

    \ cr
    \ ." Dom: " dup action-get-parent-domain domain-get-inst-id-xt execute .
    \ space ." Act: " dup action-get-inst-id . space ." get-needs for " over .value space ." TODO"
    \ cr

    \ Init return need list.
    list-new -rot                                   \ reg1 ret-lst sta1 act0 |

    \ Check for squares in action-incompatible-pairs that are not pnc.
    dup action-get-incompatible-pairs               \ reg1 ret-lst sta1 act0 | par-lst
    list-get-links                                  \ reg1 ret-lst sta1 act0 | link
    begin
        ?dup
    while
        dup link-get-data region-get-state-0        \ reg1 ret-lst sta1 act0 | link s0
        #2 pick                                     \ reg1 ret-lst sta1 act0 | link s0 act0
        \ Skip if state is represented by a square, with pnc = true.
        action-state-confirmed                      \ reg1 ret-lst sta1 act0 | link flag
        0= if                                       \ reg1 ret-lst sta1 act0 | link
            \ State not confirmed.   Check if state is reachable, or equal current state.
            dup link-get-data region-get-state-0    \ reg1 ret-lst sta1 act0 | link s0
            dup #6 pick region-superset-of-state    \ reg1 ret-lst sta1 act0 | link s0 flag
            swap #4 pick                            \ reg1 ret-lst sta1 act0 | link flag s0 sta1
            =                                       \ reg1 ret-lst sta1 act0 | link flag flag
            or                                      \ reg1 ret-lst sta1 act0 | link flag

            if
                \ Make need.
                dup link-get-data region-get-state-0    \ reg1 ret-lst sta1 act0 | link s0
                #2 swap                                 \ reg1 ret-lst sta1 act0 | link type-2 s0
                #3 pick                                 \ reg1 ret-lst sta1 act0 | link type-2 s0 act0
                action-make-need                        \ reg1 ret-lst sta1 act0 | link ned
                \ Store need.
                #4 pick need-list-push                  \ reg1 ret-lst sta1 act0 | link
                \ 3drop                                   \ reg1 ret-lst
                \ nip                                     \ ret-lst
                \ exit
            then
        then
                                                    \ reg1 ret-lst sta1 act0 | link
        dup link-get-data region-get-state-1        \ reg1 ret-lst sta1 act0 | link s1
        #2 pick                                     \ reg1 ret-lst sta1 act0 | link s1 act0
        \ Skip if state is represented by a square, with pnc = true.
        action-state-confirmed                      \ reg1 ret-lst sta1 act0 | link flag
        0= if                                       \ reg1 ret-lst sta1 act0 | link
            \ State not confirmed.   Check if state is reachable.
            dup link-get-data region-get-state-1    \ reg1 ret-lst sta1 act0 | link s1

            dup #6 pick region-superset-of-state    \ reg1 ret-lst sta1 act0 | link s1 flag
            swap #4 pick                            \ reg1 ret-lst sta1 act0 | link flag s1 sta1
            =                                       \ reg1 ret-lst sta1 act0 | link flag flag
            or                                      \ reg1 ret-lst sta1 act0 | link flag

            if
                \ Make need.
                dup link-get-data region-get-state-1    \ reg1 ret-lst sta1 act0 | link s1
                #2 swap                                 \ reg1 ret-lst sta1 act0 | link type-2 s1
                #3 pick                                 \ reg1 ret-lst sta1 act0 | link type-2 s1 act0
                action-make-need                        \ reg1 ret-lst sta1 act0 | link ned
                \ Store need.
                #4 pick need-list-push                  \ reg1 ret-lst sta1 act0 | link
                \ 3drop                                 \ reg1 ret-lst
                \ nip                                   \ ret-lst
                \ exit
            then
        then

        link-get-next                                   \ reg1 ret-lst sta1 act0 | link
    repeat
                                                        \ reg1 ret-lst sta1 act0

    #2 pick list-is-empty
    if
        \ Check for non-adjacent incompatible pairs.
        dup action-get-incompatible-pairs   \ reg1 ret-lst sta1 act0 | par-lst
        list-get-links                      \ reg1 ret-lst sta1 act0 | link
        begin
            ?dup
        while
            \ Check if region states are non-adjacent.
            dup link-get-data               \ reg1 ret-lst sta1 act0 | link regx
            region-states-adjacent          \ reg1 ret-lst sta1 act0 | link flag
            0= if                           \ reg1 ret-lst sta1 act0 | link
                \ Check if region states are represented by squares with pnc = true.
                dup link-get-data           \ reg1 ret-lst sta1 act0 | link regx
                #2 pick                     \ reg1 ret-lst sta1 act0 | link regx act0
                action-region-confirmed     \ reg1 ret-lst sta1 act0 | link flag
                if
                    \ Get a state between the states that define the region. TODO use better selection method.
    
                    \ Get a single, arbitrary, bit, from a mask of different bits between the two states.
                    dup link-get-data           \ reg1 ret-lst sta1 act0 | link regx
                cr ." for pair: " dup .region cr
                dup region-get-states           \ link regx s1 s0
                #4 pick action-find-square      \ link regx s1, sqr0 t | f
                is-false abort" sqr not found?"
                cr ." sqr0: " .square cr
                #3 pick                         \ link regx s1 act0
                action-find-square              \ link regx, sqr1 t | f
                is-false abort" sqr not found?"
                cr ." sqr1: " .square cr
                


                
                    region-get-states xor       \ reg1 ret-lst sta1 act0 | link dif-msk
                    value-isolate-lsb nip       \ reg1 ret-lst sta1 act0 | link bit
    
                    \ Calc the state between.
                    over link-get-data          \ reg1 ret-lst sta1 act0 | link bit regx
                    region-get-state-0          \ reg1 ret-lst sta1 act0 | link bit r-sta-0
                    xor                         \ reg1 ret-lst sta1 act0 | link sta'
    
                    \ Check if target is in the reachable region, or equal the current state.
                    dup                         \ reg1 ret-lst sta1 act0 | link sta' sta'
                    #6 pick                     \ reg1 ret-lst sta1 act0 | link sta' sta' reg1
                    region-superset-of-state    \ reg1 ret-lst sta1 act0 | link sta' flag
                    over                        \ reg1 ret-lst sta1 act0 | link sta' flag sta'
                    #5 pick =                   \ reg1 ret-lst sta1 act0 | link sta' flag flag2
                    or                          \ reg1 ret-lst sta1 act0 | link sta' flag
                    if
                        dup                     \ reg1 ret-lst sta1 act0 | link sta' sta'
                        #3 pick                 \ reg1 ret-lst sta1 act0 | link sta' sta' act0
                        action-find-square      \ reg1 ret-lst sta1 act0 | link sta', sqr t | f
                        if
                            cr ." sqr0.5: " .square cr
                        then
                    
                        \ cr ." make need for non-adjacent incompatible pair " .value space ." in " dup link-get-data .region cr
                        \ Make need.
                        #3 swap                     \ reg1 ret-lst sta1 act0 | link type-3 sta'
                        #3 pick                     \ reg1 ret-lst sta1 act0 | link type-3 sta' act0
                        action-make-need            \ reg1 ret-lst sta1 act0 | link ned
                        \ Store need.
                        #4 pick need-list-push      \ reg1 ret-lst sta1 act0 | link
                        3drop                       \ reg1 ret-lst
                        nip                         \ ret-lst
                        exit
                    else
                        drop                        \ reg1 ret-lst sta1 act0 | link
                    then
                then
            then
    
            link-get-next           \ reg1 ret-lst sta1 act0 | link
        repeat
                                    \ reg1 ret-lst sta1 act0
    then

                                                 \ reg1 ret-lst sta1 act0

    #2 pick list-is-empty
    if
        \ Check for new corners.
        dup                             \ reg1 ret-lst sta1 act0 act0
        action-possible-corner-needs    \ reg1 ret-lst sta1 act0 ned-lst
        dup list-is-empty               \ reg1 ret-lst sta1 act0
        if
            list-deallocate
        else
            dup                         \ reg1 ret-lst sta1 act0 ned-lst ned-lst
            #4 pick                     \ reg1 ret-lst sta1 act0 ned-lst ned-lst ret-lst
            need-list-append            \ reg1 ret-lst sta1 act0 ned-lst
            need-list-deallocate        \ reg1 ret-lst sta1 act0
        then
                                    \ reg1 ret-lst sta1 act0
    then

    \ Check if the current state is not in a group, and is not represented by a pnc square.
    over                        \ reg1 ret-lst sta1 act0 | sta1
    over action-get-groups      \ reg1 ret-lst sta1 act0 | sta1 grp-lst
    group-list-state-in-group-r \ reg1 ret-lst sta1 act0 | flag
    0= if
        \ Check if square, set make-need flag.
        2dup action-find-square \ reg1 ret-lst sta1 act0 | sqr t | f
        if
            square-get-pnc          \ reg1 ret-lst sta1 act0 | pnc
            0=
        else                        \ no square.
            true                    \ reg1 ret-lst sta1 act0 | t
        then
        if
                                    \ reg1 ret-lst sta1 act0 |
            \ Make need.
            1                       \ reg1 ret-lst sta1 act0 | type-1
            #2 pick                 \ reg1 ret-lst sta1 act0 | type-1 sta
            #2 pick                 \ reg1 ret-lst sta1 act0 | type-1 sta act0
            action-make-need        \ reg1 ret-lst sta1 act0 | ned
            \ Store need.
            #3 pick                 \ reg1 ret-lst sta1 act0 | ned ret-lst
            need-list-push          \ reg1 ret-lst sta1 act0 |

            \ Return need-list.
            2drop                   \ reg1 ret-lst
            nip                     \ ret-lst
            exit
        then
    then

    \ Check for group fill needs.
    dup action-get-groups       \ reg1 ret-lst sta1 act0 grp-lst
    list-get-links              \ reg1 ret-lst sta1 act0 link

    begin
        ?dup
    while
        \ Get group fill need.
        #4 pick                     \ reg1 ret-lst sta1 act0 link reg1
        over link-get-data          \ reg1 ret-lst sta1 act0 link reg1 grpx
        group-get-region            \ reg1 ret-lst sta1 act0 link reg1 grp-reg
        region-intersects           \ reg1 ret-lst sta1 act0 link flag
        if
            #4 pick                     \ reg1 ret-lst sta1 act0 link reg1
            over link-get-data          \ reg1 ret-lst sta1 act0 link reg1 grpx
            group-get-fill-need-state   \ reg1 ret-lst sta1 act0 link, stax t | f
            if
                \ Make fill need.
                #4 swap                 \ reg1 ret-lst sta1 act0 link type-4 stax
                #3 pick                 \ reg1 ret-lst sta1 act0 link type-4 stax act0
                action-make-need        \ reg1 ret-lst sta1 act0 link nedx
    
                \ Add need to the return list.
                #4 pick                 \ reg1 ret-lst sta1 act0 link nedx ret-lst
                need-list-push          \ reg1 ret-lst sta1 act0 link
                \ 3drop                   \ reg1 ret-lst
                \ nip                     \ ret-lst
                \ exit
            then
        then

        link-get-next
    repeat
                                \ reg1 ret-lst sta1 act0

    \ Check for group confirm needs.
    dup action-get-groups       \ reg1 ret-lst sta1 act0 grp-lst
    list-get-links              \ reg1 ret-lst sta1 act0 link

    begin
        ?dup
    while
        \ Get group confirm need.
        dup link-get-data               \ reg1 ret-lst sta1 act0 link grpx
        group-get-confirm-need-state    \ reg1 ret-lst sta1 act0 link, stax t | f

        if
            \ Make need.
            #5 swap                     \ reg1 ret-lst sta1 act0 link type-5 stax
            #3 pick                     \ reg1 ret-lst sta1 act0 link type-5 stax act0
            action-make-need            \ reg1 ret-lst sta1 act0 link nedx

            \ Add needs to the return list.
            #4 pick                     \ reg1 ret-lst sta1 act0 link nedx ret-lst
            need-list-push              \ reg1 ret-lst sta1 act0 link
            \ 3drop nip
            \ exit
        then

        link-get-next
    repeat
                                        \ reg1 ret-lst sta1 act0

    \ Clean up.
    2drop                           \ reg1 ret-lst
    nip                             \ ret-lst
;

: action-calc-changes ( act0 -- cngs )
    \ Check arg.
    assert-tos-is-action

    0 0 changes-new swap            \ cngs act0
    action-get-groups               \ cngs grp-lst

    list-get-links                  \ cngs link
    begin
        ?dup
    while
        dup link-get-data           \ cngs link grp
        dup group-get-pn            \ cngs link grp pn
        #3 < if
                                    \ cngs link grp
            group-calc-changes      \ cngs link grp-cngs
            rot                     \ link grp-cngs cngs
            2dup changes-calc-union \ link grp-cngs cngs cngs'

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

\ Return a step, given reg-to, reg-from, and a rule.
: action-make-planstep ( reg-to reg-from rul1 act0 -- stp )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-rule
    assert-3os-is-region
    assert-4os-is-region

    \ Get number unwanted changes.
    #3 pick #3 pick #3 pick                 \ | reg-to reg-from rul1
    rule-number-unwanted-changes            \ | u-unw

    \ Make step.
    #2 pick                                 \ | u-unw rul1
    #2 pick                                 \ | u-unw rul1 act0
    planstep-new                            \ | u-unw stp

    \ Set number unwanted changes.
    tuck                                    \ | stp u-unw stp
    planstep-set-number-unwanted-changes    \ | stp

    \ Clean up.
    2nip                                    \ reg-to act0 stp
    nip nip                                 \ stp
;

\ Return a step list, given reg-to, reg-from, and a rule list.
: action-planstep-list-from-rule-list ( reg-to reg-from rul-lst1 act0 -- plnstp-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-rule-list
    assert-3os-is-region
    assert-4os-is-region

    \ Init return list.
    list-new                        \ reg-to reg-from rul-lst1 act0 plnstp-lst

    \ Prep for loop, for each rule.
    #2 pick list-get-links          \ reg-to reg-from rul-lst1 act0 plnstp-lst rul-link

    begin
        ?dup
    while
        #5 pick #5 pick             \ reg-to reg-from rul-lst1 act0 plnstp-lst rul-link reg-to reg-from
        #2 pick link-get-data       \ reg-to reg-from rul-lst1 act0 plnstp-lst rul-link reg-to reg-from rulx
        #5 pick                     \ reg-to reg-from rul-lst1 act0 plnstp-lst rul-link reg-to reg-from rulx act0
        action-make-planstep        \ reg-to reg-from rul-lst1 act0 plnstp-lst rul-link stp
        #2 pick                     \ reg-to reg-from rul-lst1 act0 plnstp-lst rul-link stp plnstp-lst
        list-push-struct            \ reg-to reg-from rul-lst1 act0 plnstp-lst rul-link

        link-get-next
    repeat
                                    \ reg-to reg-from rul-lst1 act0 plnstp-lst
    \ Clean up.
    2nip                            \ reg-to act0 plnstp-lst
    nip nip                         \ plnstp-lst
;

\ Return a list of possible plansteps, given to/from regions.
\ Steps may, or may not, intersect the to/from regions.
\ If they do not intersect, there are no restrictions.
: action-calc-plansteps-by-changes ( reg-to reg-from act0 -- plnstp-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                                 \ | reg-to reg-from
    swap region-superset-of                         \ | bool
    abort" action-calc-plansteps-by-changes: region subset?"    \ |

    \ cr ." action-calc-plansteps-by-changes: Dom: " dup action-get-parent-domain domain-get-inst-id-xt execute .
    \ space ." Act: " dup action-get-inst-id .
    \ space ." reg-to: " #2 pick .region space ." reg-from: " over .region cr

    \ Get needed changes.
    #2 pick #2 pick                         \ reg-to reg-from act0 reg-to reg-from
    changes-new-region-to-region            \ reg-to reg-from act0 cngs'

    \ Init return list.
    list-new                                \ reg-to reg-from act0 cngs' ret-lst

    #2 pick action-get-groups               \ reg-to reg-from act0 cngs' ret-lst grp-lst
    list-get-links                          \ reg-to reg-from act0 cngs' ret-lst grp-lnk
    begin
        ?dup
    while
        dup link-get-data                   \ reg-to reg-from act0 cngs' ret-lst grp-lnk grpx

        \ Check if group might apply.
        group-get-pn                        \ reg-to reg-from act0 cngs' ret-lst grp-lnk pn
        #3 <                                \ reg-to reg-from act0 cngs' ret-lst grp-lnk flag
        if                                  \ reg-to reg-from act0 cngs' ret-lst grp-lnk

            \ Get backward rules, if any.
            #2 pick over                    \ reg-to reg-from act0 cngs' ret-lst grp-lnk cngs' grp-lnk
            link-get-data                   \ reg-to reg-from act0 cngs' ret-lst grp-lnk cngs' grpx
            group-calc-for-plansteps-by-changes \ reg-to reg-from act0 cngs' ret-lst grp-lnk rul-lst'

            dup list-is-empty               \ reg-to reg-from act0 cngs' ret-lst grp-lnk rul-lst' bool
            if
                list-deallocate             \ reg-to reg-from act0 cngs' ret-lst grp-lnk

            else                            \ reg-to reg-from act0 cngs' ret-lst grp-lnk
                \ Get planstep from rules.
                #6 pick #6 pick                     \ reg-to reg-from act0 cngs' ret-lst grp-lnk rul-lst' reg-to reg-from
                #2 pick                             \ reg-to reg-from act0 cngs' ret-lst grp-lnk rul-lst' reg-to reg-from rul-lst'
                #7 pick                             \ reg-to reg-from act0 cngs' ret-lst grp-lnk rul-lst' reg-to reg-from rul-lst' act0
                action-planstep-list-from-rule-list \ reg-to reg-from act0 cngs' ret-lst grp-lnk rul-lst' plnstp-lst'
                swap rule-list-deallocate           \ reg-to reg-from act0 cngs' ret-lst grp-lnk plnstp-lst'

                \ Append planstep-list to return list.
                dup                         \ reg-to reg-from act0 cngs' ret-lst grp-lnk plnstp-lst' plnstp-lst'
                #3 pick                     \ reg-to reg-from act0 cngs' ret-lst grp-lnk plnstp-lst' plnstp-lst' ret-lst
                planstep-list-append        \ reg-to reg-from act0 cngs' ret-lst grp-lnk plnstp-lst'
                planstep-list-deallocate    \ reg-to reg-from act0 cngs' ret-lst grp-lnk
            then
        then

        link-get-next                       \ reg-to reg-from act0 cngs' ret-lst grp-lnk
    repeat
                                            \ reg-to reg-from act0 cngs' ret-lst
    swap changes-deallocate                 \ reg-to reg-from act0 ret-lst
    2nip                                    \ act0 ret-lst
    nip                                     \ ret-lst
;

\ Return a list of possible forward-chaining plansteps, given region-from and region-to regions.
\ Steps may, or may not, intersect the from region.
\ If they do not intersect reg-from, going reg-from to the step initial-region cannot require a needed change.
: action-calc-plansteps-fc ( reg-to reg-from act0 -- plnstp-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                                 \ | reg-to reg-from
    swap region-superset-of                         \ | bool
    abort" action-calc-plansteps-fc: region subset?"    \ |

    \ cr ." Dom: " dup action-get-parent-domain domain-get-inst-id-xt execute .
    \ space ." Act: " dup action-get-inst-id .
    \ space ." action-calc-steps-fc: " #2 pick .region space over .region cr

    \ Init return list.
    list-new                                \ reg-to reg-from act0 ret-lst

    over action-get-groups                  \ reg-to reg-from act0 ret-lst grp-lst
    list-get-links                          \ reg-to reg-from act0 ret-lst rul-link
    begin
        ?dup
    while
        dup link-get-data                   \ reg-to reg-from act0 ret-lst rul-link grpx

        \ Check if group might apply.
        group-get-pn                        \ reg-to reg-from act0 ret-lst rul-link pn
        #3 <                                \ reg-to reg-from act0 ret-lst rul-link flag
        if                                  \ reg-to reg-from act0 ret-lst rul-link

            \ Get backward rules, if any.
            #4 pick #4 pick #2 pick         \ reg-to reg-from act0 ret-lst rul-link reg-to reg-from link
            link-get-data                   \ reg-to reg-from act0 ret-lst rul-link reg-to reg-from grpx
            group-calc-for-plansteps-fc     \ reg-to reg-from act0 ret-lst rul-link rul-lst'

            dup list-is-empty               \ reg-to reg-from act0 ret-lst rul-link rul-lst' bool
            if
                list-deallocate             \ ret-lst reg-to reg-from link

            else                            \ reg-to reg-from act0 ret-lst rul-link
                \ Get planstep from rules.
                #5 pick #5 pick             \ reg-to reg-from act0 ret-lst rul-link rul-lst' reg-to reg-from
                #2 pick                     \ reg-to reg-from act0 ret-lst rul-link rul-lst' reg-to reg-from rul-lst'
                #6 pick                     \ reg-to reg-from act0 ret-lst rul-link rul-lst' reg-to reg-from rul-lst' act0
                action-planstep-list-from-rule-list \ reg-to reg-from act0 ret-lst rul-link rul-lst' plnstp-lst'
                swap rule-list-deallocate   \ reg-to reg-from act0 ret-lst rul-link plnstp-lst'

                \ Append planstep-list to return list.
                dup                         \ reg-to reg-from act0 ret-lst rul-link plnstp-lst' plnstp-lst'
                #3 pick                     \ reg-to reg-from act0 ret-lst rul-link plnstp-lst' plnstp-lst' ret-lst
                planstep-list-append        \ reg-to reg-from act0 ret-lst rul-link plnstp-lst'
                planstep-list-deallocate    \ reg-to reg-from act0 ret-lst rul-link
            then
        then

        link-get-next                       \ reg-to reg-from act0 ret-lst rul-link
    repeat
                                            \ reg-to reg-from act0 ret-lst
    2nip                                    \ act0 ret-lst
    nip                                     \ ret-lst
    \ cr ." returning steps: " dup .step-list cr
;

\ Return a list of possible backward-chaining plansteps, given a sample.
\ Steps may, or may not, intersect region reg-to.
\ If they do not intersect reg-to, going from the step initial-region to reg-to cannot require a needed change.
: action-calc-plansteps-bc ( reg-to reg-from act0 -- plnstp-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                                     \ | reg-to reg-from
    swap region-superset-of                             \ | bool
    abort" action-calc-plansteps-bc: region subset?"    \ |

    \ cr ." Dom: " dup action-get-parent-domain domain-get-inst-id-xt execute .
    \ space ." Act: " dup action-get-inst-id .
    \ space ." action-calc-steps-bc: " #2 pick .region space over .region cr

    \ Init return list.
    list-new                                \ reg-to reg-from act0 ret-lst

    over action-get-groups                  \ reg-to reg-from act0 ret-lst grp-lst
    list-get-links                          \ reg-to reg-from act0 ret-lst rul-link
    begin
        ?dup
    while
        dup link-get-data                   \ reg-to reg-from act0 ret-lst rul-link grpx

        \ Check if group might apply.
        group-get-pn                        \ reg-to reg-from act0 ret-lst rul-link pn
        #3 <                                \ reg-to reg-from act0 ret-lst rul-link flag
        if                                  \ reg-to reg-from act0 ret-lst rul-link

            \ Get backward rules, if any.
            #4 pick #4 pick #2 pick         \ reg-to reg-from act0 ret-lst rul-link reg-to reg-from link
            link-get-data                   \ reg-to reg-from act0 ret-lst rul-link reg-to reg-from grpx
            group-calc-for-plansteps-bc     \ reg-to reg-from act0 ret-lst rul-link rul-lst'

            dup list-is-empty               \ reg-to reg-from act0 ret-lst rul-link rul-lst' bool
            if
                list-deallocate             \ ret-lst reg-to reg-from link

            else                            \ reg-to reg-from act0 ret-lst rul-link
                \ Get planstep from rules.
                #5 pick #5 pick             \ reg-to reg-from act0 ret-lst rul-link rul-lst' reg-to reg-from
                #2 pick                     \ reg-to reg-from act0 ret-lst rul-link rul-lst' reg-to reg-from rul-lst'
                #6 pick                     \ reg-to reg-from act0 ret-lst rul-link rul-lst' reg-to reg-from rul-lst' act0
                action-planstep-list-from-rule-list \ reg-to reg-from act0 ret-lst rul-link rul-lst' plnstp-lst'
                swap rule-list-deallocate   \ reg-to reg-from act0 ret-lst rul-link plnstp-lst'

                \ Append planstep-list to return list.
                dup                         \ reg-to reg-from act0 ret-lst rul-link plnstp-lst' plnstp-lst'
                #3 pick                     \ reg-to reg-from act0 ret-lst rul-link plnstp-lst' plnstp-lst' ret-lst
                planstep-list-append        \ reg-to reg-from act0 ret-lst rul-link plnstp-lst'
                planstep-list-deallocate    \ reg-to reg-from act0 ret-lst rul-link
            then
        then

        link-get-next                       \ reg-to reg-from act0 ret-lst rul-link
    repeat
                                            \ reg-to reg-from act0 ret-lst
    2nip                                    \ act0 ret-lst
    nip                                     \ ret-lst
;
