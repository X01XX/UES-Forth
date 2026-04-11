\ Implement an Action struct and functions.

#29717 constant action-id
    #9 constant action-struct-number-cells

\ Struct fields
0                                     constant action-header-disp               \ 16 bits, [0] Struct id, [1] Use count, [2] Instance id (8) Cull squares trigger (8)
                                                                                \          [3] Calc corners trigger (8).
action-header-disp              cell+ constant action-parent-domain-disp        \ Domain pointer.
action-parent-domain-disp       cell+ constant action-squares-disp              \ A square-list
action-squares-disp             cell+ constant action-incompatible-pairs-disp   \ A region-list
action-incompatible-pairs-disp  cell+ constant action-logical-structure-disp    \ A region-list
action-logical-structure-disp   cell+ constant action-groups-disp               \ A group-list.
action-groups-disp              cell+ constant action-function-disp             \ An xt to run to get a sample.
action-function-disp            cell+ constant action-defining-regions-disp     \ Defining regions, region-list, from action-logical-structure.
action-defining-regions-disp    cell+ constant action-corners-disp              \ A corner list.

0 value action-mma \ Storage for action mma instance.

\ Init action mma, return the addr of allocated memory.
: action-mma-init ( num-items -- ) \ sets action-mma.
    dup 1 <
    abort" action-mma-init: Invalid number of items."

    cr ." Initializing Action store."
    action-struct-number-cells swap mma-new to action-mma
;

\ Check instance type.
: is-allocated-action ( addr -- flag )
    get-first-word          \ w t | f
    if
        action-id =
    else
        false
    then
;

\ Check TOS for action, unconventional, leaves stack unchanged.
: assert-tos-is-action ( tos -- tos )
    dup is-allocated-action
    false? if
        s" TOS is not an allocated action"
       .abort-xt execute
    then
;

' assert-tos-is-action to assert-tos-is-action-xt

\ Check NOS for action, unconventional, leaves stack unchanged.
: assert-nos-is-action ( nos tos -- nos tos )
    over is-allocated-action
    false? if
        s" NOS is not an allocated action"
       .abort-xt execute
    then
;

' assert-nos-is-action to assert-nos-is-action-xt

\ Check 3OS for action, unconventional, leaves stack unchanged.
: assert-3os-is-action ( 3os nos tos -- 3os nos tos )
    #2 pick is-allocated-action
    false? if
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
: _action-set-inst-id ( u1 act0 -- )
    \ Check args.
    assert-tos-is-action

    over 0<
    abort" Invalid instance id"

    over #255 >
    abort" Invalid instance id"

    \ Set inst id.
    4c!
;

\ Return the cull squares trigger from an action instance.
: action-get-cull-squares-trigger ( act0 -- bool)
    \ Check arg.
    assert-tos-is-action

    \ Get cull trigger.
    5c@

    0=
    if
        false
    else
        true
    then
;

\ Set the cull trigger of an action instance, use only in this file.
: _action-set-cull-squares-trigger ( bool act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-bool

    swap                \ act0 bool
    if
        1 swap
    else
        0 swap
    then

    \ Set trigger.
    5c!
;

\ Return the calc-corners trigger from an action instance.
: action-get-calc-corners-trigger ( act0 -- bool)
    \ Check arg.
    assert-tos-is-action

    \ Get calc-corner triggr.
    6c@

    0=
    if
        false
    else
        true
    then
;

\ Set the calc-corners trigger of an action instance, use only in this file.
: _action-set-calc-corners-trigger ( bool act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-bool

    swap                \ act0 bool
    if
        1
    else
        0
    then
    swap                \ flag act0

    \ Set trigger.
    6c!
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
    assert-nos-is-domain-xt execute

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
: _action-set-squares ( sqr-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square-list

    action-squares-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return the incompatible-pairs region-list from an action instance.
: action-get-incompatible-pairs ( act0 -- reg-lst )
    \ Check arg.
    assert-tos-is-action

    action-incompatible-pairs-disp +    \ Add offset.
    @                                   \ Fetch the field.
;

\ Set the incompatible-pairs region-list of an action instance, use only in this file.
: _action-set-incompatible-pairs ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list

    action-incompatible-pairs-disp +    \ Add offset.
    !struct                             \ Store it.
;

\ Update incompatible-pairs.
: _action-update-incompatible-pairs ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list
    cr ." New incompatible pairs: " over .region-list cr

    dup action-get-incompatible-pairs   \ reg-lst1 act0 ip-lst
    -rot                                \ ip-lst reg-lst1 act0
    _action-set-incompatible-pairs      \ ip-lst
    region-list-deallocate              \
;

\ Return the logical-structure region-list from an action instance.
: action-get-logical-structure ( act0 -- reg-lst )
    \ Check arg.
    assert-tos-is-action

    action-logical-structure-disp + \ Add offset.
    @                               \ Fetch the field.
;

' action-get-logical-structure to action-get-logical-structure-xt

\ Set the logical-structure region-list of an action instance, use only in this file.
: _action-set-logical-structure ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list

    \ Set new LS.
    action-logical-structure-disp + \ Add offset.
    !struct                         \ Store it.
;


\ Return the group-list from an action instance.
: action-get-groups ( act0 -- grp-lst )
    \ Check arg.
    assert-tos-is-action

    action-groups-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the group-list of an action instance, use only in this file.
: _action-set-groups ( grp-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-group-list

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
: action-get-defining-regions ( act0 -- reg-lst )
    \ Check arg.
    assert-tos-is-action

    action-defining-regions-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the defining-regions region-list of an action instance, use only in this file.
: _action-set-defining-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list

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

\ Remove a group with a given region.
: _action-delete-group ( reg1 act0 -- flag )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region

    \ Check if group exists.
    2dup                                \ reg1 act0 reg1 act0
    action-get-groups                   \ reg1 act0 reg1 grp-lst
    group-list-member                   \ reg1 act0 flag
    if
        \ Delete group.
        action-get-groups               \ reg1 grp-lst
        group-list-delete               \ flag
        0= abort" Group delete failed?"
        true
    else
        2drop
        false
    then
;

\ Update defining-regions.
: _action-update-defining-regions ( reg-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list
    cr
    ." Dom: " current-domain-id-gbl #3 dec.r
    space ." Act: " current-action-id-gbl #3 dec.r
    space ." New DF regions: " over .region-list cr

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
: _action-update-logical-structure ( new-ls act0 -- )
    \ cr ." _action-update-logical-structure: start"  cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    cr
    ." Dom: " current-domain-id-gbl #3 dec.r
    space ." Act: " current-action-id-gbl #3 dec.r
    space ." New LS region list: " over .region-list
    cr

    \ Check the new list is different from the old list.
    over                                \ new-lst act0 new-lst
    over action-get-logical-structure   \ new-lst act0 new-lst old-lst'
    2dup region-list-eq                 \ new-lst act0 new-lst old-lst' flag
    if cr ." region lists equal?" cr then
    nip                                 \ new-lst act0 old-lst'

    \ Get/save current LS.
    \ cr ." old list " dup .region-list cr

    -rot                                \ old-lst' new-lst act0

    \ Store new structure.
    2dup                                \ old-lst' new-lst act0 new-lst act0
    _action-set-logical-structure       \ old-lst' new-lst act0

    \ Save action, for now..
    -rot                                \ act0 old-lst' new-lst

    \ Get old regions that are deleted.
    2dup                                \ act0 old-lst' new-lst old-lst' new-lst
    region-list-set-difference          \ act0 old-lst' new-lst old-gone'
    \ cr ." Old LS regions deleted: " dup .region-list cr

    \ Scan deleted regions.
    dup list-get-links                   \ act0 old-lst' new-lst old-gone' link
    begin
        ?dup
    while
        dup link-get-data               \ act0 old-lst' new-lst old-gone' link old-ls-reg

        \ If group exists, delete it.
        #5 pick                         \ act0 old-lst' new-lst old-gone' link old-ls-reg act0
        _action-delete-group            \ act0 old-lst' new-lst old-gone' link flag
        drop
\        if
\            cr #4 spaces dup link-get-data .region
\            \ space ." deleted group"
\        then

        link-get-next                   \ act0 old-lst' new-lst old-gone' link
    repeat
    cr
                                        \ act0 old-lst' new-lst old-gone'
    region-list-deallocate              \ act0 old-lst' new-lst

    \ Display new regions.
    dup                                 \ act0 old-lst' new-lst new-lst
    #2 pick                             \ act0 old-lst' new-lst new-lst old-lst'

    region-list-set-difference          \ act0 old-lst' new-lst new-added'
    \ cr ." New LS regions added: " dup .region-list cr
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

        \ cr #4 spaces ." defining region: " dup .region

        #2 pick                             \ act0 link reg act0
        action-get-groups                   \ act0 link reg grps
        group-list-member                   \ act0 link flag
        if                                  \ act0 link
            \ space ." group already exists"
        else
            dup link-get-data                   \ act0 link reg
            #2 pick                             \ act0 link reg act0
            action-get-squares                  \ act0 link reg sqr-lst1
            square-list-in-region               \ act0 link sqr-lst2
            dup list-is-empty?                  \ act0 link sqr-lst2 flag
            if                                  \ act0 link sqr-lst2
                space ." no squares found "
                list-deallocate
            else
                dup                             \ act0 link sqr-lst2 sqr-lst2
                square-list-get-rules           \ act0 link sqr-lst2, ruls t | f
                if                              \ act0 link sqr-lst2 ruls
                    rulestore-deallocate        \ act0 link sqr-lst2
                    over link-get-data          \ act0 link sqr-lst2 reg
                    group-new                   \ act0 link grp
                    #2 pick action-get-groups   \ act0 link grp grp-lst
                    group-list-push             \ act0 link
                else                            \ act0 link sqr-lst2
                    cr ." Inompatible rules found for squares in "
                    over link-get-data .region
                    space dup .square-list-states
                    ." , needs more incompatible-pair/corner development."
                    \ space dup .square-list
                    cr

                    square-list-deallocate      \ act0 link

                    \ If group exists, delete it.
                    dup link-get-data               \ act0 link region
                    #2 pick                         \ act0 link region act0
                    _action-delete-group            \ act0 link flag
                    drop
                    \ if                              \ act0 link
                    \     space ." deleted group"
                    \ then
                then
            then
        then

        link-get-next                   \ act0 link
    repeat
    cr
                                        \ act0

    \ Set calc corners trigger.
    true swap                               \ true act0
    _action-set-calc-corners-trigger        \

    \ cr ."  _action-update-logical-structure: end" cr
;

\ Return action-corners list.
: action-get-corners ( act0 -- crn-lst )
    \ Check arg.
    assert-tos-is-action

    action-corners-disp +   \ Add offset.
    @                       \ Fetch the field.
;

' action-get-corners to action-get-corners-xt

\ Set action-corners list.
: _action-set-corners ( crn-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-corner-list

    action-corners-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Update the action-corners list.
: _action-update-corners ( crn-lst1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-corner-list
\    cr
\    ." Dom: " current-domain-id #3 dec.r
\    space ." Act: " current-action-id #3 dec.r
\    space ." update-corners: " over .corner-list-short
\    cr

    \ Get/save the previous corner list.
    dup action-get-corners -rot    \ prev-lst crn-lst1 act0

    \ Set the new corners.
    2dup                            \ prev-lst crn-lst1 act0 crn-lst act0
    _action-set-corners             \ prev-lst crn-lst1 act0

    \ Deallocate old list.
    rot                             \ crn-lst act0 prev-lst
    corner-list-deallocate          \ crn-lst act0

    \ Update groups first square, if needed.
    action-get-groups               \ crn-lst1 grp-lst
    swap                            \ grp-lst crn-lst1
    list-get-links                  \ grp-lst crn-link

    begin
        ?dup
    while
        dup link-get-data           \ grp-lst crn-link crnx
        dup corner-get-anchor-state \ grp-lst crn-link crnx a-sta
        swap corner-get-region      \ grp-lst crn-link a-sta regx
        #3 pick                     \ grp-lst crn-link a-sta regx grp-lst
        group-list-find             \ grp-lst crn-link a-sta, grp t | f
        if
            group-set-first-square  \ grp-lst crn-link
        else
            drop
            cr ." Group not found? " dup link-get-data corner-get-region .region cr
        then

        link-get-next
    repeat
                                    \ grp-lst
    drop
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
    over _action-set-inst-id            \ xt1 dom0 act

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
    over
    _action-set-groups                  \ act

    \ Init corner list.
    list-new                            \ act lst
    over _action-set-corners            \ act

    \ Init triggers.
    false over                          \ act 0 act
    _action-set-cull-squares-trigger    \ act

    false over                          \ act 0 act
    _action-set-calc-corners-trigger    \ act
;

\ Return a square given a state.
: action-find-square ( sta1 act0 -- sqr t | f )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    action-get-squares          \ sta1 sqr-lst
    square-list-find
;

' action-find-square to action-find-square-xt

: .action-corners ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    dup action-get-corners              \ act0 crn-lst
    dup list-get-length                 \ act0 crn-lst len
    0= if
        2drop
        exit
    then

    \ cr ." action corners: " dup .list-raw cr
    cr ."           corners: "

    list-get-links                      \ act0 crn-link
    begin
        ?dup
    while
        dup link-get-data               \ act0 crn-link crnx
        .corner

        link-get-next
        dup 0<> if cr #19 spaces then
    repeat
                                        \ act0
    space ." number states: "
    dup action-get-corners corner-list-number-states dec.

    drop
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

    \ Print each group.
    dup  action-get-groups list-get-links
    begin
        ?dup
    while
        dup link-get-data
        cr #10 spaces .group
        link-get-next
    repeat
    cr

    dup .action-corners
    cr

    drop
;

' .action to .action-xt

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
    dup 0< abort" invalid use count"

    #2 <
    if
        \ Clear fields.
        dup action-get-squares square-list-deallocate
        dup action-get-incompatible-pairs region-list-deallocate
        dup action-get-logical-structure region-list-deallocate
        dup action-get-defining-regions region-list-deallocate
        dup action-get-groups group-list-deallocate
        dup action-get-corners corner-list-deallocate

        \ Deallocate instance.
        action-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if an incompatible pair should be kept.
: action-incompatible-pair-needed ( reg1 act0 -- bool )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region

    \ If region state 0 is not in any corner region, keep it.
    over region-get-state-0                     \ reg1 act0 sta0
    over action-get-corners                     \ reg1 act0 sta0 crn-lst
    corner-list-state-in-any-corner-region      \ reg1 act0 bool
    false? if
        \ Keep pair
        2drop
        true
        exit
    then

    \ If region state 1 is not in any corner region, keep it.
    over region-get-state-1                 \ reg1 act0 sta1
    over action-get-corners                 \ reg1 act0 sta1 crn-lst
    corner-list-state-in-any-corner-region  \ reg1 act0 bool
    false? if
        \ Keep pair
        2drop
        true
        exit
    then

    \ If region state 0 is used by any corner, keep it.
    over region-get-state-0                     \ reg1 act0 sta0
    over action-get-corners                     \ reg1 act0 sta0 crn-lst
    corner-list-uses-state                      \ reg1 act0 bool
    if
        \ Keep pair
        2drop
        true
        exit
    then

    \ If region state 1 is used by any corner, keep it.
    over region-get-state-1                     \ reg1 act0 sta1
    over action-get-corners                     \ reg1 act0 sta1 crn-lst
    corner-list-uses-state                      \ reg1 act0 bool
    if
        \ Keep pair
        2drop
        true
        exit
    then

    2drop
    false
;

\ Delete unneeded incompatible pairs.
\ This keeps down the permutations of list-one-of-each-struct in action-calc-corners.
: action-cull-incompatible-pairs ( act0 -- )
    \ Check arg.
    assert-tos-is-action
    \ cr
    \ ." Dom: " current-domain-id-gbl #3 dec.r
    \ space ." Act: " dup action-get-inst-id #3 dec.r
    \ space ." action-cull-incompatible-pairs: start" cr

    \ Init new incompatible pairs list.
    list-new                            \ act0 new-ip-lst
    over                                \ act0 new-ip-lst act0
    action-get-incompatible-pairs       \ act0 new-ip-lst cur-ip-lst
    list-get-links                      \ act0 new-ip-lst ip-link

    begin
        ?dup
    while
        dup link-get-data               \ act0 new-ip-lst ip-link ip-reg
        #3 pick                         \ act0 new-ip-lst ip-link ip-reg act0
        action-incompatible-pair-needed \ act0 new-ip-lst ip-link bool
        \ cr ." ip needed: " dup .bool cr
        if
            dup link-get-data           \ act0 new-ip-lst ip-link ip-reg
            #2 pick                     \ act0 new-ip-lst ip-link ip-reg new-ip-lst
            list-push-struct            \ act0 new-ip-lst ip-link
        then

        link-get-next
    repeat
                                        \ act0 new-ip-lst
    swap                                \ new-ip-lst act0
    _action-update-incompatible-pairs   \
;

\ Return the number of defining regions.
: action-number-defining-regions ( act0 -- u )
    \ Check arg.
    assert-tos-is-action

    action-get-defining-regions \ df-lst
    list-get-length
;

\ Return the number af de
\ Calc corners and set action-corners field.
\ Find all corners, and states with only one dissimilar, near by,  square.
\ Sort by the number of Logical Structure regions the anchor square is in, lowest first.
\ Filter out possible corners that have an LS region list that is a set-superset of
\ possible corners earlier in the list.
: action-calc-corners ( act0 -- )
    \ Check arg.
    assert-tos-is-action
\    cr
\    ." Dom: " current-domain-id-gbl execute #3 dec.r
\    space ." Act: " dup action-get-inst-id #3 dec.r
\    space ." action-calc-corners: start"
\     cr

    dup action-get-defining-regions     \ act0 def-regs
    list-get-length                     \ act0 len
    #2 <                                \ act0 bool
    if                                  \ act0
        dup action-get-corners          \ act0 crn-lst
        list-is-empty?                  \ act0 bool
        if
            drop                        \
        else
            list-new swap               \ crn-lst act0
            _action-update-corners      \
            cr
            ." Dom: " current-domain-id-gbl #3 dec.r
            space ." Act: " current-action-id-gbl #3 dec.r
            space ." action-calc-corners: too few defining regions"
            cr
        then
        exit
    then

    \ Init corner list.
    list-new                            \ act0 crn-lol'

    \ Prep for loop.
    over action-get-incompatible-pairs  \ act0 crn-lol' inc-prs
    region-list-states                  \ act0 crn-lol' inc-stas'
    #2 pick action-get-defining-regions \ act0 crn-lol' inc-stas' def-regs
    dup list-get-links                  \ act0 crn-lol 'inc-stas' def-regs def-link

    \ For each defining region.
    begin
        ?dup
    while
        \ Get incompatible pair states in the current defining region.
        #2 pick                             \ act0 crn-lol' inc-stas' def-regs def-link inc-stas'
        over link-get-data                  \ act0 crn-lol' inc-stas' def-regs def-link inc-stas def-reg
        region-states-in                    \ act0 crn-lol' inc-stas' def-regs def-link in-stas'

        \ Get states in the current defining region, only.
        dup                                 \ act0 crn-lol' inc-stas' def-regs def-link in-stas' in-stas'
        #3 pick                             \ act0 crn-lol' inc-stas' def-regs def-link in-stas' in-stas' def-regs
        region-list-states-in-one-region    \ act0 crn-lol' inc-stas' def-regs def-link in-stas' in-stas2'
        swap list-deallocate                \ act0 crn-lol' inc-stas' def-regs def-link in-stas2'

        \ Make new corner list.
        list-new swap                       \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2'

        \ For each anchor.
        dup list-get-links                  \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2' in-link
        begin
            ?dup
        while
            \ Make corner.
            #3 pick link-get-data           \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2' in-link def-reg
            over link-get-data              \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2' in-link def-reg sta
            #9 pick                         \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2' in-link def-reg sta act0
            corner-new                      \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2' in-link crn

            \ Store corner.
            #3 pick                         \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2' in-link crn crn-lst
            list-push-struct                \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2' in-link

            link-get-next
        repeat
                                            \ act0 crn-lol' inc-stas' def-regs def-link crs-lst in-stas2'
        \ Clean up.
        list-deallocate                     \ act0 crn-lol' inc-stas' def-regs def-link crs-lst

        \ Store corner(s) list for one defining region.
        dup list-is-not-empty?
        if
            #4 pick                         \ act0 crn-lol' inc-stas' def-regs def-link crs-lst crn-lol'
            list-push-struct                \ act0 crn-lol' inc-stas' def-regs def-link
        else
            list-deallocate
        then
        link-get-next
    repeat
                                            \ act0 crn-lol' inc-stas' def-regs
    drop                                    \ act0 crn-lol' inc-stas'
    list-deallocate                         \ act0 crn-lol'
    \ cr ." at x: crn-lol: " [ ' .corner ] literal over .list cr

    \ Check number of corners in each list.

    \ Init max num.
    0                                       \ act0 crn-lol' max
    over list-get-links                     \ act0 crn-lol' max crn-link
    \ cr ." action-calc-corners: number corners: " cr
    begin
        ?dup
    while
        dup link-get-data                   \ act0 crn-lol' max crn-link crn-lst
        \ dup list-get-length .
        list-get-length                     \ act0 crn-lol' max crn-link len
        rot                                 \ act0 crn-lol' crn-link len max
        max                                 \ act0 crn-lol' crn-link max
        swap                                \ act0 crn-lol' max crn-link

        link-get-next
    repeat
                                            \ act0 crn-lol' max
    \ cr ." action-calc-corners: max number corners in one list: crmax: " dup . cr

    1 =
    \ cr ." action-calc-corners: number corners: " cr
    if                                      \ act0 crn-lol'
        \ Flatten list.
        dup list-flatten-struct             \ act0 crn-lol' crn-lst-f
        \ cr ." flat list1: " dup .list-raw cr

        \ Clean up.
        swap corner-lol-deallocate          \ act0 crn-lst-f
        \ cr ." flat list2: " dup .list-raw cr

        \ Store corner list.
        swap                                \ crn-lst act0
        tuck _action-update-corners         \ act0
    else
                                            \ act0 crn-lol'
        \ Avoid too many corner permutations.
        dup list-number-permutations        \ act0 crn-lol' u
        #2 pick                             \ act0 crn-lol' u act0
        action-number-defining-regions      \ act0 crn-lol' u num-df
        #20 *                               \ act0 crn-lol' u num-df
        \ cr ." permutations: " dup .
        <
        if
            \ Init min number states and corner-list.
            dup list-one-of-each-struct         \ act0 crn-lol' crn-lol2'
            \ cr ." corner one of each: " dup .corner-lol cr
            #99999 0                            \ act0 crn-lol' crn-lol2' min min-lst

            #2 pick list-get-links              \ act0 crn-lol' crn-lol2' min min-lst crn-link
            begin
                ?dup
            while
                dup link-get-data               \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx
                dup corner-list-number-states   \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx num-stas
                \ cr ." a crn lst: " over .corner-list space ." num states: " dup . cr
                \ cr ." list: " over .list-raw cr

                \ Check/update min.
                dup                             \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx num-stas num-stas
                #5 pick                         \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx num-stas num-stas min
                <                               \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx num-stas bool
                if                              \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx num-stas
                    >R                          \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx :r num-stas
                    \ Replace min list.
                    2swap                       \ act0 crn-lol' crn-lol2' crn-link crn-lstx min min-lst :r num-stas
                    drop over                   \ act0 crn-lol' crn-lol2' crn-link crn-lstx min min-lst :r num-stas
                    \ Replace min value.
                    nip                         \ act0 crn-lol' crn-lol2' crn-link crn-lstx min-lst
                    R>                          \ act0 crn-lol' crn-lol2' crn-link crn-lstx min-lst num-stas
                    \ Clean up.
                    swap                        \ act0 crn-lol' crn-lol2' crn-link crn-lstx min-lst min min-lst
                    2swap                       \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx
                else                            \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx num-stas
                    drop                        \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx
                then
                                                \ act0 crn-lol' crn-lol2' min min-lst crn-link crn-lstx
                drop                            \ act0 crn-lol' crn-lol2' min min-lst crn-link

                link-get-next
            repeat
                                                \ act0 crn-lol' crn-lol2' min min-lst
            \ cr ." min states: " over . space ." min lst: " dup .corner-list cr

            \ Update action corner list.
            \ cr ." min list: " dup .list-raw cr
            list-copy-struct
            #4 pick                             \ act0 crn-lol' crn-lol2' min min-lst act0
            _action-update-corners              \ act0 crn-lol' crn-lol2' min
            drop                                \ act0 crn-lol' crn-lol2'
            corner-lol-deallocate               \ act0 crn-lol'
        then
        \ Avoid too many corner permutations.
                                                \ act0 crn-lol'
        dup list-number-permutations            \ act0 crn-lol' u
        #2 pick                                 \ act0 crn-lol' u act0
        action-number-defining-regions          \ act0 crn-lol' u num-df
        #5 *                                    \ act0 crn-lol' u num-df
        > if
            over                                \ act0 crn-lol' act0
            action-cull-incompatible-pairs      \ act0 crn-lol'
        then

        corner-lol-deallocate                   \ act0
    then

                                                \ act0
    drop
;

\ Return squares in a given region list.
: action-squares-in-region-list ( reg-lst1 act0 -- sqr-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region-list

    \ Init return list.
    list-new                            \ reg-lst1 act0 ret-lst

    \ Prep for loop on action square list.
    over action-get-squares             \ reg-lst1 act0 ret-lst sqr-lst
    list-get-links                      \ reg-lst1 act0 ret-lst sqr-link

    begin
        ?dup
    while
        dup link-get-data               \ reg-lst1 act0 ret-lst sqr-link sqrx
        square-get-state                \ reg-lst1 act0 ret-lst sqr-link stax
        #4 pick                         \ reg-lst1 act0 ret-lst sqr-link stax reg-lst1
        region-list-any-superset-state  \ reg-lst1 act0 ret-lst sqr-link bool
        if
            dup link-get-data           \ reg-lst1 act0 ret-lst sqr-link sqrx
            #2 pick                     \ reg-lst1 act0 ret-lst sqr-link sqrx ret-lst
            list-push-struct            \ reg-lst1 act0 ret-lst sqr-link
        then

        link-get-next
    repeat
                                        \ reg-lst1 act0 ret-lst
    nip nip                             \ ret-lst
;

\ Get a list of incompatible pairs, as regions, comparing a square, and
\ and other squares within the same region(s) of the logical structure.
\ In a set of pn > 1 squares, two squares of lesser pn should not be compared.
: action-find-incompatible-pairs-for-square ( sqr1 act0 -- reg-list )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    \ Int return list.
    list-new -rot                               \ ret-lst sqr1 act0

    \ Get the LS regions a square is in.
    over square-get-state                       \ ret-lst sqr1 act0 sta
    over action-get-logical-structure           \ ret-lst sqr1 act0 sta ls-lst
    region-list-regions-state-in                \ ret-lst sqr1 act0 reg-lst'
    2dup swap                                   \ ret-lst sqr1 act0 reg-lst' reg-lst' act0
    action-squares-in-region-list               \ ret-lst sqr1 act0 reg-lst' sqr-lst'

    \ Get highest pn squares in the regions.
    dup square-list-highest-pn                  \ ret-lst sqr1 act0 reg-lst' sqr-lst' pn
    over square-list-eq-pn                      \ ret-lst sqr1 act0 reg-lst' sqr-lst' sqr-lst2'
    swap square-list-deallocate                 \ ret-lst sqr1 act0 reg-lst' sqr-lst2'

    \ Get squares that sqr1 is incompatible to.
    #3 pick                                     \ ret-lst sqr1 act0 reg-lst' sqr-lst2' sqr1
    over                                        \ ret-lst sqr1 act0 reg-lst' sqr-lst2' sqr1 sqr-lst2'
    [ ' square-incompatible ] literal -rot      \ ret-lst sqr1 act0 reg-lst' sqr-lst2' xt sqr1 sqr-lst2'
    list-find-all-struct                        \ ret-lst sqr1 act0 reg-lst' sqr-lst2' inc-sqr-lst'
    swap square-list-deallocate                 \ ret-lst sqr1 act0 reg-lst' inc-sqr-lst'
    swap region-list-deallocate                 \ ret-lst sqr1 act0 inc-sqr-lst'

    \ Check for no incompatible squares.
    dup list-is-empty?
    if
        list-deallocate                         \ ret-lst sqr1 act0
        2drop                                   \ ret-lst
        \ cr ." action-find-incompatible-pairs-for-square: end 1" cr
        exit
    then

    \ Create a list of incompatible pairs, represented by states in a region.
    #2 pick square-get-state                \ ret-lst sqr1 act0 inc-lst' sta1
    over list-get-links                     \ ret-lst sqr1 act0 inc-lst' sta1 link
    begin
        ?dup
    while
        dup link-get-data square-get-state  \ ret-lst sqr1 act0 inc-lst' sta1 link sta2
        #2 pick                             \ ret-lst sqr1 act0 inc-lst' sta1 link sta2 sta1
        region-new                          \ ret-lst sqr1 act0 inc-lst' sta1 link regx

        dup                                 \ ret-lst sqr1 act0 inc-lst' sta1 link regx regx
        #7 pick                             \ ret-lst sqr1 act0 inc-lst' sta1 link regx regx ret-lst
        region-list-push-nosups             \ ret-lst sqr1 act0 inc-lst' sta1 link regx flag
        if
            drop
        else
            region-deallocate
        then
                                            \ ret-lst sqr1 act0 inc-lst' sta1 link

        link-get-next                       \ ret-lst sqr1 act0 inc-lst' sta1 link-next
    repeat
                                            \ ret-lst sqr1 act0 inc-lst' sta1
    drop                                    \ ret-lst sqr1 act0 inc-lst'
    square-list-deallocate                  \ ret-lst sqr1 act0

    drop                                    \ ret-lst sqr1
\    over list-is-empty?
\    if
\        cr
\        ." Dom: " current-domain-id #3 dec.r
\        space ." Act: " current-action-id #3 dec.r
\        space ." for square: " square-get-state .value
\        space ." incompatible pairs: " dup .region-list
\        cr
\    then
    drop
;

\ Check a new, or changed square, for incompatible square pairs within
\ the Logical Structure.
\ If found, update action-incompatible-pairs and action-logical-structure, return true.
: _action-check-square ( sqr1 act0 -- )
    \ cr ." _action-check-square: start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-square

    \ Form regions with incompatible squares, no supersets.
    tuck                                            \ act0 sqr1 act0
    action-find-incompatible-pairs-for-square       \ act0 inc-lst'
    dup list-is-empty?
    if
        \ cr ." _action-check-square: list is empty" cr
        list-deallocate
        drop
        exit
    then

    \ Push new region into action-incompatible-pairs,
    \ Calc ~A + ~B, intersect with action-logical-structure,
    \ To get a new LS.

                                                    \ act0 inc-lst'

    dup list-get-links                              \ act0 inc-lst' inc-link
    begin
        ?dup                                        \ act0 inc-lst' inc-link
    while
        dup link-get-data                           \ act0 inc-lst' inc-link regx

        \ Add region to the action-incompatible-pairs  list.
        cr
        ." Dom: " current-domain-id-gbl #3 dec.r
        space ." Act: " current-action-id-gbl #3 dec.r
        space ." Adding incompatible pair: " dup region-get-states .value space .value
        cr

        dup                                         \ act0 inc-lst' link regx regx
        #4 pick                                     \ act0 inc-lst' link regx regx act0
        action-get-incompatible-pairs               \ act0 inc-lst' link regx regx act-inc-lst
        region-list-push-nosups                     \ act0 inc-lst' link regx flag
        if
            \ Calc regions possible for incompatible pair.
            region-get-states                       \ act0 inc-lst' link s0 s1
            #4 pick                                 \ act0 inc-lst' link s0 s1 act0
            action-get-parent-domain                \ act0 inc-lst' link s0 s1 dom
            domain-state-pair-complement-xt         \ act0 inc-lst' link s0 s1 dom xt
            execute                                 \ act0 inc-lst' link reg-lst'

            \ Calc new action-logical-structure.
            #3 pick action-get-logical-structure    \ act0 inc-lst' link reg-lst' lsl-lst
            2dup                                    \ act0 inc-lst' link reg-lst' lsl-lst reg-lst lsl-lsn
            region-list-intersections-nosubs        \ act0 inc-lst' link reg-lst' lsl-lst new-reg-lst

            \ Set new action-logical-structure.
            #5 pick                                 \ act0 inc-lst' link reg-lst' lsl-lst new-reg-lst act0
            _action-update-logical-structure        \ act0 inc-lst' link reg-lst' lsl-lst
            drop                                    \ act0 inc-lst' link reg-lst'
            region-list-deallocate                  \ act0 inc-lst' link
        else
            cr ." subset region not added?"
            abort
        then

        link-get-next                               \ act0 inc-lst' link-next
    repeat
                                                    \ act0 inc-lst'


    region-list-deallocate                          \ act0
    drop                                            \
    \ cr ." _action-check-square: end" cr
;

\ Check a given region-list, where the region states represent incompatible pairs,
\ returning regions where the represented squares are no longer incompatible.
: _action-pairs-no-longer-incompatible ( reg-lst1 act0 -- reg-lst )
    \ cr ." _action-not-incompatible-pairs: start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-list

    over list-is-empty?
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
        square-incompatible     \ ret-lst act0 link region compare-result
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

    over                                    \ act0 ls-new act0
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
    over action-get-incompatible-pairs      \ sqr1 act0 | sta ip-lst'
    region-list-regions-state-in            \ sqr1 act0 | reg-in-lst'
    dup list-is-empty?                      \ sqr1 act0 | reg-in-lst' flag
    if
        list-deallocate
        2drop
        \ cr ." _action-check-incompatible-pairs: exit early" cr
        exit
    then

    \ Set flag for change in incompatble pairs.
    0 swap                                  \ sqr1 act0 | flag reg-in-lst'

    \ Check each incompatble pair region.
    dup list-get-links                      \ sqr1 act0 | reg-in-lst' link
    begin
        ?dup
    while
        \ Check sqr1 against region state 0.
        dup link-get-data                   \ sqr1 act0 | flag reg-in-lst' link regx
        region-get-state-0                  \ sqr1 act0 | flag reg-in-lst' link s0
        #4 pick action-find-square          \ sqr1 act0 | flag reg-in-lst' link, r-sqr t | f
        0= abort" square not found?"
        #5 pick                             \ sqr1 act0 | flag reg-in-lst' link r-sqr sqr1
        square-incompatible                 \ sqr1 act0 | flag reg-in-lst' link char
        if                                  \ sqr1 act0 | flag reg-in-lst' link
            \ Add new incompatible pair.
            #4 pick square-get-state        \ sqr1 act0 | flag reg-in-lst' link sta1
            over link-get-data              \ sqr1 act0 | flag reg-in-lst' link sta1 regx
            region-get-state-0              \ sqr1 act0 | flag reg-in-lst' link sta1 s0
            region-new dup                  \ sqr1 act0 | flag reg-in-lst' link reg-new reg-new
            #5 pick                         \ sqr1 act0 | flag reg-in-lst' link reg-new reg-new act0
            action-get-incompatible-pairs   \ sqr1 act0 | flag reg-in-lst' link reg-new reg-new ip-lst
            region-list-push-nosups         \ sqr1 act0 | flag reg-in-lst' link reg-new flag
            if
                cr
                ." Dom: " current-domain-id-gbl #3 dec.r
                ." Act: " current-action-id-gbl #3 dec.r
                space ." New incompatible pair: " dup .region
                cr
                drop                        \ sqr1 act0 | flag reg-in-lst' link
                rot drop                    \ sqr1 act0 | reg-in-lst' link
                true -rot                   \ sqr1 act0 | flag reg-in-lst' link
            else
                region-deallocate
            then
        then

        \ Check sqr1 against region state 1.
        dup link-get-data                   \ sqr1 act0 | flag reg-in-lst' link regx
        region-get-state-1                  \ sqr1 act0 | flag reg-in-lst' link s1
        #4 pick action-find-square          \ sqr1 act0 | flag reg-in-lst' link, r-sqr t | f
        0= abort" square not found?"
        #5 pick                             \ sqr1 act0 | flag reg-in-lst' link r-sqr sqr1
        square-incompatible                      \ sqr1 act0 | flag reg-in-lst' link char
        if                                  \ sqr1 act0 | flag reg-in-lst' link
            \ Add new incompatible pair.
            #4 pick square-get-state        \ sqr1 act0 | flag reg-in-lst' link sta1
            over link-get-data              \ sqr1 act0 | flag reg-in-lst' link sta1 regx
            region-get-state-1              \ sqr1 act0 | flag reg-in-lst' link sta1 s1
            region-new dup                  \ sqr1 act0 | flag reg-in-lst' link reg-new reg-new
            #5 pick                         \ sqr1 act0 | flag reg-in-lst' link reg-new reg-new act0
            action-get-incompatible-pairs   \ sqr1 act0 | flag reg-in-lst' link reg-new reg-new ip-lst
            region-list-push-nosups         \ sqr1 act0 | flag reg-in-lst' link reg-new flag
            if
                cr
                ." Dom: " current-domain-id-gbl #3 dec.r
                ." Act: " current-action-id-gbl #3 dec.r
                space ." New incompatible pair: " dup .region
                cr
                drop                        \ sqr1 act0 | flag reg-in-lst' link
                rot drop                    \ sqr1 act0 | reg-in-lst' link
                true -rot                   \ sqr1 act0 | flag reg-in-lst' link
            else
                region-deallocate
            then
        then

        link-get-next
    repeat
                                            \ sqr1 act0 | flag reg-in-lst'
    region-list-deallocate                  \ sqr1 act0 | flag

    if
        _action-recalc-logical-structure    \ sqr1
    else
        drop                                \ sqr1
    then
    drop
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
    region-list-regions-using-state     \ sqr1 act0 reg-lst-in'

    dup list-is-empty?                  \ sqr1 act0 reg-lst-in' flag
    if
        list-deallocate                 \ sqr1 act0
        _action-check-incompatible-pairs2
       \  cr ." _action-check-incompatible-pairs: end 1" cr
        exit
    else                                \ sqr1 act0 reg-lst-in'
        rot drop                        \ act0 reg-lst-in'
    then

    \ Some regions found, check them.       \ act0 reg-lst-in'
    2dup swap                               \ act0 reg-lst-in' reg-lst-in' act0
    _action-pairs-no-longer-incompatible    \ act0 reg-lst-in' reg-lst-not-i'
    dup list-is-empty?                      \ act0 reg-lst-in' reg-lst-not-i' flag
    if
        \ No not-incompatible pairs found.
        list-deallocate                     \ act0 reg-lst-in'
        region-list-deallocate              \ act0
        drop
       \  cr ." _action-check-incompatible-pairs: end 2" cr
        exit
    then

    \ Some not-incompatible pairs found.
                                        \ act0 reg-lst-in' reg-lst-not-i'
    swap region-list-deallocate         \ act0 reg-lst-not-i'

    \ Remove regions.
    dup                                 \ act0 reg-lst-not-i' reg-lst-not-i'
    list-get-links                      \ act0 reg-lst-not-i' link
    begin
        ?dup
    while
        dup link-get-data               \ act0 reg-lst-not-i' link region

        cr
        ." Dom: " current-domain-id-gbl #3 dec.r
        space ." Act: " current-action-id-gbl #3 dec.r
        space ." state " dup region-get-states .value space ." and " .value space ." are no longer incompatible"
        cr

        [ ' region-eq? ] literal swap   \ act0 reg-lst-not-i' link xt region
        #4 pick                         \ act0 reg-lst-not-i' link xt region act0
        action-get-incompatible-pairs   \ act0 reg-lst-not-i' link xt region pair-list
        list-remove                     \ act0 reg-lst-not-i' link reg? flag
        0=
        abort" Region not found?"

        region-deallocate

        link-get-next
    repeat
                                        \ act0 reg-lst-not-i'
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

\    cr
\    ." Dom: " current-domain-id #3 dec.r
\    space ." Act: " dup action-get-inst-id #3 dec.r
\    space ." adding sample: " over .sample
\    cr

    over sample-get-initial     \ smpl1 act0 s-i
    over action-get-squares     \ smpl1 act0 s-i sqr-lst
    square-list-find            \ smpl1 act0, sqr t | f
    if
                                \ smpl1 act0 sqr
        \ Update existing square
        rot                     \ act0 sqr smpl1
        over                    \ act0 sqr smpl1 sqr
        square-add-sample       \ act0 sqr flag
        if
            swap                        \ sqr act0
            2dup                        \ sqr act0 sqr act0
            _action-check-incompatible-pairs    \ sqr act0
            2dup                        \ sqr act0 sqr act0
            _action-check-square        \ sqr act0
            tuck                        \ act0 sqr act0
            action-get-groups           \ act0 sqr grp-lst
            group-list-check-square     \ act0
        else
            drop                        \ act0
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
            1 =                 \ sqr act0 grp-lst flag

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
            else
                drop nip                    \ act0
            then
        else
            #2 pick             \ sqr act0 grp-lst sqr
            swap                \ sqr act0 sqr grp-lst
            group-list-add-square
            nip                 \ act0
        then
                                \ act0
    then
                                    \ act0
    dup                             \ act0 act0
    action-get-calc-corners-trigger \ act0 bool
    if
        dup                         \ act0 act0
        action-calc-corners         \ act0
        false swap                  \ false act0
        _action-set-calc-corners-trigger
    else
        drop                        \
    then

    \ cr ." action-add-sample: end" cr
;

' action-add-sample to action-add-sample-xt

\ Return true if a state is confirmed with a pnc square.
: action-state-confirmed ( sta1 act0 -- flag )
     \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    action-find-square      \ sqr t | f
    if
        square-get-pnc      \ pnc
    else
        false               \ false
    then
;

' action-state-confirmed to action-state-confirmed-xt

\ Get a sample from an action.
\ Depends on the current domain being set correctly.
\ Add the sample to the action.
: action-get-sample ( sta1 act0 -- smpl )
    \ cr ." action-get-sample: start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    tuck                    \ act0 sta1 act0
    dup                     \ act0 sta1 act0 act0
    action-get-function     \ act0 sta1 act0 xt
    execute                 \ act0 smpl
    tuck                    \ smpl act0 smpl
    swap                    \ smpl smpl act0

    cr
    ." Dom: " current-domain-id-gbl #3 dec.r
    space ." Act: " current-action-id-gbl #3 dec.r
    space ." adding sample: " over .sample
    cr

    action-add-sample       \ smpl
;

\ If a square exists for a sample initial state, update it.
\ Otherwise, do nothing.
: action-update-existing-square ( smpl1 act0 -- )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-sample

    over sample-get-initial         \ smpl1 act0 sta
    over                            \ smpl1 act0 sta act0
    action-find-square              \ smpl1 act0, sqr t | f
    if
        drop                        \ smpl1 act0
        action-add-sample           \
    else
        2drop
    then
;

' action-update-existing-square to action-update-existing-square-xt

\ Get a sample from an action, for a step.
\ Depends on the current domain being set correctly.
\ If the sample is for an existing square,
\ or it cannot be predicted by the existing groups,
\ add the sample to the action.
: action-get-sample-step ( sta1 act0 -- smpl )
    \ cr ." action-get-sample-step: start" cr
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    dup                     \ sta1 act0 act0
    action-get-function     \ sta1 act0 xt
    execute                 \ smpl
;

\ Return true if a action id matches a number.
: action-id-eq ( id1 act0 -- flag )
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

\ Return group confirm needs.
\ The first square in the group's square list, or the farthest state
\ from the first square, in the r-region.
: action-calc-group-confirm-needs ( act0 -- ned-lst )
    \ Check-args.
    assert-tos-is-action

    \ Init return list.
    list-new                                        \ act0 | ret-lst

    \ Prep for loop.
    over action-get-groups                          \ act0 | ret-lst grp-lst
    list-get-links                                  \ act0 | ret-lst grp-link

    begin
        ?dup
    while
        \ Get group confirm state need, if any.
        dup link-get-data                           \ act0 | ret-lst grp-link grpx
        group-get-confirm-need-state                \ act0 | ret-lst grp-link, stax t | f

        if
            \ Make need.
            need-type-cg swap                       \ act0 | ret-lst grp-link ned-type stax
            #4 pick                                 \ act0 | ret-lst grp-link ned-type stax act0
            action-make-need                        \ act0 | ret-lst grp-link nedx

            \ Add needs to the return list.
            #2 pick                                 \ act0 | ret-lst grp-link nedx ret-lst
            need-list-push                          \ act0 | ret-lst grp-link
        then

        link-get-next
    repeat
                                                    \ act0 | ret-lst
    nip
;

\ Return need for the current state, if it is not in a group,
\ and if there is no corresponding square, or there is a
\ corresponding square that needs more samples.
: action-calc-state-not-in-group-needs ( sta1 act0 -- ned-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    \ Init return list.
    list-new                                        \ sta1 act0 | ret-lst

    #2 pick #2 pick                                 \ sta1 act0 | ret-lst sta1 act0
    action-get-groups                               \ sta1 act0 | ret-lst sta1 grp-lst
    group-list-state-in-group-r                     \ sta1 act0 | ret-lst flag
    if
    else
        \ Check if square, set make-need flag.
        #2 pick #2 pick                             \ sta1 act0 | ret-lst sta1 act0
        action-find-square                          \ sta1 act0 | ret-lst sqr t | f
        if
            square-get-pnc                          \ sta1 act0 | ret-lst pnc
            0=
        else
            \ no square.
            true                                    \ sta1 act0 | ret-lst t
        then
        if
            \ Make need.
            need-type-snig                          \ sta1 act0 | ret-lst ned-type
            #3 pick #3 pick                         \ sta1 act0 | ret-lst ned-type sta1 act0
            action-make-need                        \ sta1 act0 | ret-lst ned

            \ Store need.
            over need-list-push                     \ sta1 act0 | ret-lst
        then
    then
    \ Return need-list.
    nip nip                                         \ ret-lst
;

\ Return a need for a incompatible pair state in a region.
: action-get-ip-state-region-need ( reg2 sta1 act0 -- ned t | f )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value
    assert-3os-is-region

    \ Get list of all adjacent, external, states.
    rot                         \ sta1 act0 reg2

    \ Init external state list.
    list-new swap               \ sta1 act0 ext-lst' reg2
    region-edge-mask            \ sta1 act0 ext-lst' e-msk
    value-split                 \ sta1 act0 ext-lst' msk-lst'

    dup list-get-links          \ sta1 act0 ext-lst' msk-lst' msk-link

    begin
        ?dup
    while
        \ Calc external state.
        dup link-get-data       \ sta1 act0 ext-lst' msk-lst' msk-link msk
        #5 pick                 \ sta1 act0 ext-lst' msk-lst' msk-link msk sta1
        xor                     \ sta1 act0 ext-lst' msk-lst' msk-link sta-ext
        \ Store external state.
        #3 pick                 \ sta1 act0 ext-lst' msk-lst' msk-link sta-ext ext-lst'
        list-push               \ sta1 act0 ext-lst' msk-lst' msk-link

        link-get-next
    repeat
                                \ sta1 act0 ext-lst' msk-lst'
    \ Clean up.
    list-deallocate             \ sta1 act0 ext-lst'

    \ Get square for ip state.
    #2 pick #2 pick             \ sta1 act0 ext-lst' sta1 act0
    action-find-square          \ sta1 act0 ext-lst', sqr t | f
    false? abort" square not found?"

    \ Check for even one compatible external square.
    \ If any found, return false.
    swap                        \ sta1 act0 sqr1 ext-lst'

    dup list-get-links          \ sta1 act0 sqr1 ext-lst' ext-link

    begin
        ?dup
    while
        dup link-get-data       \ sta1 act0 sqr1 ext-lst' ext-link ext-sta
        #4 pick                 \ sta1 act0 sqr1 ext-lst' ext-link ext-sta act0
        action-find-square      \ sta1 act0 sqr1 ext-lst' ext-link, ext-sqr t | f
        if
            #3 pick             \ sta1 act0 sqr1 ext-lst' ext-link ext-sqr sqr1
            square-compatible   \ sta1 act0 sqr1 ext-lst' ext-link bool
            if
                drop            \ sta1 act0 sqr1 ext-lst'
                list-deallocate \ sta1 act0 sqr1
                3drop           \
                false
                exit
            then
        then

        link-get-next
    repeat
                                \ sta1 act0 sqr1 ext-lst'
    \ Check for squares that need more samples.
    dup list-get-links          \ sta1 act0 sqr1 ext-lst' ext-link

    begin
        ?dup
    while
        dup link-get-data       \ sta1 act0 sqr1 ext-lst' ext-link ext-sta
        #4 pick                 \ sta1 act0 sqr1 ext-lst' ext-link ext-sta act0
        action-find-square      \ sta1 act0 sqr1 ext-lst' ext-link, ext-sqr t | f
        if
            #3 pick             \ sta1 act0 sqr1 ext-lst' ext-link ext-sqr sqr1
            square-incompatible \ sta1 act0 sqr1 ext-lst' ext-link bool
            if
            else
                \ Must need more samples.
                \ Make need.
                need-type-ils                   \ sta1 act0 sqr1 ext-lst' ext-link typ
                over link-get-data              \ sta1 act0 sqr1 ext-lst' ext-link typ ext-sta
                #5 pick                         \ sta1 act0 sqr1 ext-lst' ext-link typ ext-sta act0
                dup action-get-parent-domain    \ sta1 act0 sqr1 ext-lst' ext-link typ ext-sta act0 dom
                need-new                        \ sta1 act0 sqr1 ext-lst' ext-link ned
                \ Return need.
                nip                             \ sta1 act0 sqr1 ext-lst' ned
                swap list-deallocate            \ sta1 act0 sqr1 ned
                2nip                            \ sqr1 ned
                nip                             \ ned
                true                            \ ned t
                exit
            then
        then

        link-get-next
    repeat
                                            \ sta1 act0 sqr1 ext-lst'
    \ Get need for the first sample of an external state.
    dup list-get-links                      \ sta1 act0 sqr1 ext-lst' ext-link

    begin
        ?dup
    while
        dup link-get-data                   \ sta1 act0 sqr1 ext-lst' ext-link ext-sta
        #4 pick                             \ sta1 act0 sqr1 ext-lst' ext-link ext-sta act0
        action-find-square                  \ sta1 act0 sqr1 ext-lst' ext-link, ext-sqr t | f
        if
            drop                            \ sta1 act0 sqr1 ext-lst' ext-link
        else
            \ Make need.
            link-get-data                   \ sta1 act0 sqr1 ext-lst' ext-sta
            need-type-ils swap              \ sta1 act0 sqr1 ext-lst' typ ext-sta
            #4 pick                         \ sta1 act0 sqr1 ext-lst' typ ext-sta act0
            dup action-get-parent-domain    \ sta1 act0 sqr1 ext-lst' typ ext-sta act0 dom
            need-new                        \ sta1 act0 sqr1 ext-lst'  ned
            \ Return need.
            swap list-deallocate            \ sta1 act0 sqr1 ned
            2nip                            \ sqr1 ned
            nip                             \ ned
            true                            \ ned t
            exit
        then

        link-get-next
    repeat
                            \ sta1 act0 sqr1 ext-lst'
    list-deallocate         \ sta1 act0 sqr1
    3drop                   \
    false
;

\ Return a need for a state in a incompatible pair, if it is in
\ more than one Logical Structure region.
\ As if it is a possible anchor, no yet proven.
: action-get-ip-state-multi-region-need ( sta1 act0 -- ned t | f )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    \ Get LS regions the state is in.
    over                                \ sta1 act0 sta1
    over action-get-logical-structure   \ sta1 act0 sta1 ls-lst
    region-list-regions-state-in        \ sta1 act0 reg-lst'

    \ Check number region, needs to be > 1.
    dup list-get-length                 \ sta1 act0 reg-lst' len
    #2 <
    if
        region-list-deallocate
        2drop
        false
        exit
    then

    dup list-get-links                      \ sta1 act0 reg-lst' reg-link

    begin
        ?dup
    while
        dup link-get-data                   \ sta1 act0 reg-lst' reg-link reg
        #4 pick #4 pick                     \ sta1 act0 reg-lst' reg-link reg sta1 act0
        action-get-ip-state-region-need     \ sta1 act0 reg-lst' reg-link, ned t | f
        if                                  \ sta1 act0 reg-lst' reg-link ned
            nip                             \ sta1 act0 reg-lst' ned
            swap region-list-deallocate     \ sta1 act0 ned
            nip nip                         \ ned
            true
            exit
        then

        link-get-next
    repeat
                                            \ sta1 act0 reg-lst'

    region-list-deallocate
    2drop
    false
;

\ Return incompatible pair needs.
: action-calc-incompatible-pair-needs ( act0 -- ned-lst )
    \ Check arg.
    assert-tos-is-action

    \ Init return list.
    list-new                            \ act0 ret-lst

    \ Check for states that need more samples.
    over action-get-incompatible-pairs  \ act0 ret-lst ip-lst
    list-get-links                      \ act0 ret-lst ip-link

    begin
        ?dup
    while
        dup link-get-data               \ act0 ret-lst ip-link ip-reg

        \ Check region state 0.
        dup region-get-state-0          \ act0 ret-lst ip-link ip-reg sta-0
        #4 pick                         \ act0 ret-lst ip-link ip-reg sta-0 act0
        action-find-square              \ act0 ret-lst ip-link ip-reg, sqr t | f
        if
            square-get-pnc
            if
                \ No more samples needed.
            else
                \ Make need.
                need-type-cls                   \ act0 ret-lst ip-link ip-reg typ
                over region-get-state-0         \ act0 ret-lst ip-link ip-reg typ sta0
                #5 pick                         \ act0 ret-lst ip-link ip-reg typ sta0 act0
                dup action-get-parent-domain    \ act0 ret-lst ip-link ip-reg typ act0 dom
                need-new                        \ act0 ret-lst ip-link ip-reg ned

                \ Store need.
                #3 pick                         \ act0 ret-lst ip-link ip-reg ned ret-lst
                list-push-struct                \ act0 ret-lst ip-link ip-reg
            then
        else
            \ Make need.
            need-type-cls                       \ act0 ret-lst ip-link ip-reg typ
            over region-get-state-0             \ act0 ret-lst ip-link ip-reg typ sta0
            #5 pick                             \ act0 ret-lst ip-link ip-reg typ sta0 act0
            dup action-get-parent-domain        \ act0 ret-lst ip-link ip-reg typ act0 dom
            need-new                            \ act0 ret-lst ip-link ip-reg ned

            \ Store need.
            #3 pick                             \ act0 ret-lst ip-link ip-reg ned ret-lst
            list-push-struct                    \ act0 ret-lst ip-link ip-reg
        then

        \ Check region state 1.
        dup region-get-state-1          \ act0 ret-lst ip-link ip-reg sta-1
        #4 pick                         \ act0 ret-lst ip-link ip-reg sta-1 act0
        action-find-square              \ act0 ret-lst ip-link ip-reg, sqr t | f
        if
            square-get-pnc
            if
                \ No more samples needed.
            else
                \ Make need.
                need-type-cls                   \ act0 ret-lst ip-link ip-reg typ
                over region-get-state-1         \ act0 ret-lst ip-link ip-reg typ sta1
                #5 pick                         \ act0 ret-lst ip-link ip-reg typ sta1 act0
                dup action-get-parent-domain    \ act0 ret-lst ip-link ip-reg typ act0 dom
                need-new                        \ act0 ret-lst ip-link ip-reg ned

                \ Store need.
                #3 pick                         \ act0 ret-lst ip-link ip-reg ned ret-lst
                list-push-struct                \ act0 ret-lst ip-link ip-reg
            then
        else
            \ Make need.
            need-type-cls                       \ act0 ret-lst ip-link ip-reg typ
            over region-get-state-1             \ act0 ret-lst ip-link ip-reg typ sta1
            #5 pick                             \ act0 ret-lst ip-link ip-reg typ sta1 act0
            dup action-get-parent-domain        \ act0 ret-lst ip-link ip-reg typ act0 dom
            need-new                            \ act0 ret-lst ip-link ip-reg ned

            \ Store need.
            #3 pick                             \ act0 ret-lst ip-link ip-reg ned ret-lst
            list-push-struct                    \ act0 ret-lst ip-link ip-reg
        then
                                                \ act0 ret-lst ip-link ip-reg
        drop                                    \ act0 ret-lst ip-link
        link-get-next
    repeat
                                        \ act0 ret-lst
    \ Check for any needs, so far.
    dup list-is-not-empty?
    if
        nip                             \ ret-lst
        exit
    then

    \ Check for pair not adjacent needs.
    over action-get-incompatible-pairs  \ act0 ret-lst ip-lst
    list-get-links                      \ act0 ret-lst ip-link

    begin
        ?dup
    while
        dup link-get-data                   \ act0 ret-lst ip-link ip-reg
        region-get-states                   \ act0 ret-lst ip-link sta1 sta0
        value-adjacent                      \ act0 ret-lst ip-link bool
        if
            \ No need.
        else
            \ Get states difference mask.
            dup link-get-data            \ act0 ret-lst ip-link ip-reg
            region-get-states               \ act0 ret-lst ip-link sta1 sta0
            over xor                        \ act0 ret-lst ip-link sta1 dif-msk

            \ Select lsbs, arbitrary.
            value-half-lsbs                 \ act0 ret-lst ip-link sta1 dif-msk2

            \ Get state between the region's two states.
            xor                             \ act0 ret-lst ip-link sta-between

            \ Make need.
            need-type-ils swap              \ act0 ret-lst ip-link typ sta-b
            #4 pick                         \ act0 ret-lst ip-link typ sta-b act0
            dup action-get-parent-domain    \ act0 ret-lst ip-link typ act0 dom
            need-new                        \ act0 ret-lst ip-link ned

            \ Store need.
            #2 pick                         \ act0 ret-lst ip-link ned ret-lst
            list-push-struct                \ act0 ret-lst ip-link
        then

        link-get-next
    repeat
                                        \ act0 ret-lst

    \ Check for any needs, so far.
    dup list-is-not-empty?
    if
        nip                             \ ret-lst
        exit
    then
                                        \ act0 ret-lst
    \ Get incompatible pair, multi-region needs.
    over action-get-incompatible-pairs  \ act0 ret-lst ip-lst
    list-get-links                      \ act0 ret-lst ip-link

    begin
        ?dup
    while
        dup link-get-data                       \ act0 ret-lst ip-link ip-reg
        region-get-states                       \ act0 ret-lst ip-link s1 s0
        #4 pick                                 \ act0 ret-lst ip-link s1 s0 act0
        action-get-ip-state-multi-region-need   \ act0 ret-lst ip-link s1, ned t | f
        if
            #3 pick                             \ act0 ret-lst ip-link s1 ned ret-lst
            list-push-struct                    \ act0 ret-lst ip-link s1
        then
                                                \ act0 ret-lst ip-link s1
        #3 pick                                 \ act0 ret-lst ip-link s1 act0
        action-get-ip-state-multi-region-need   \ act0 ret-lst ip-link, ned t | f
        if
            #2 pick                             \ act0 ret-lst ip-link ned ret-lst
            list-push-struct                    \ act0 ret-lst ip-link
        then

        link-get-next
    repeat
                                    \ act0 ret-lst
    nip                             \ ret-lst
;

\ Return true if a square/sttae is still needed.
: action-square-needed? ( sta1 act0 -- bool )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    \ Check incompatible pairs.
    2dup                            \ sta1 act0 sta1 act0
    action-get-incompatible-pairs   \ sta1 act0 sta1 ip-lst
    region-list-uses-state          \ sta1 act0 bool
    if
        2drop
        true
        exit
    then

    \ Check corners.
    2dup action-get-corners         \ sta1 act0 sta1 crn-lst
    corner-list-uses-state          \ sta1 act0 bool
    if
        2drop
        true
        exit
    then

    \ Check groups.
    over                            \ sta1 act0 sta1
    over action-get-groups          \ sta1 act0 sta1 grp-lst
    group-list-uses-square          \ sta1 act0 bool
    if
        2drop
        true
        exit
    then

    2drop
    false
;

\ Remove unneeded squares.
: action-cull-unneeded-squares ( act0 -- )
    \ Check arg.
    assert-tos-is-action

    \ Init remove list.
    list-new swap               \ rmv-lst' act0

    \ Prep for loop.
    dup action-get-squares      \ rmv-lst' act0 sqr-lst
    list-get-links              \ rmv-lst' act0 sqr-link

    begin
        ?dup
    while
        dup link-get-data       \ rmv-lst' act0 sqr-link sqrx
        square-get-state        \ rmv-lst' act0 sqr-link stax
        #2 pick                 \ rmv-lst' act0 sqr-link stax act0
        action-square-needed?   \ rmv-lst' act0 sqr-link bool
        if
        else
            \ Add square state to remove list.
            dup link-get-data   \ rmv-lst' act0 sqr-link sqrx
            square-get-state    \ rmv-lst' act0 sqr-link sta
            #3 pick             \ rmv-lst' act0 sqr-link sta rmv-lst'
            list-push           \ rmv-lst' act0 sqr-link
        then

        link-get-next
    repeat
                                \ rmv-lst' act0
    \ Remove selected squares.
    dup action-get-groups       \ rmv-lst' act0 grp-lst
    swap                        \ rmv-lst' grp-lst act0
    action-get-squares          \ rmv-lst' grp-lst sqr-lst
    #2 pick                     \ rmv-lst' grp-lst sqr-lst rmv-lst'
    list-get-links              \ rmv-lst' grp-lst sqr-lst rmv-link

    begin
        ?dup
    while
        \ Remove square from action group list.
        dup link-get-data           \ rmv-lst' grp-lst sqr-lst rmv-link stax
        #3 pick                     \ rmv-lst' grp-lst sqr-lst rmv-link stax grp-lst
        group-list-remove-square    \ rmv-lst' grp-lst sqr-lst rmv-link

        \ Remove square from action square list.
        dup link-get-data       \ rmv-lst' grp-lst sqr-lst rmv-link stax

        cr ." Dom: " current-domain-id-gbl #3 dec.r
        space ." Act: " current-action-id-gbl #3 dec.r
        space ." culling square: " dup dec. cr

        #2 pick                 \ rmv-lst' grp-lst sqr-lst rmv-link stax sqr-lst
        square-list-remove      \ rmv-lst' grp-lst sqr-lst rmv-link bool
        drop                    \ rmv-lst' grp-lst sqr-lst rmv-link

        link-get-next
    repeat
                                \ rmv-lst' grp-lst sqr-lst
    2drop
    list-deallocate
;

\ Return a list of needs for an action, given the current state.
: action-get-needs ( sta1 act0 -- ned-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    \ cr
    \ ." Dom: " dup action-get-parent-domain domain-get-inst-id-xt execute #3 dec.r
    \ space ." Act: " dup action-get-inst-id . space ." get-needs for " over .value space #3 dec.r
    \ space ." action-get-needs"
    \ cr

    \ Init return need list.
    list-new                                        \ sta1 act0 | ret-lst

    \ Check for incompatible-pair needs.
    over                                            \ sta1 act0 | ret-lst act0
    action-calc-incompatible-pair-needs             \ sta1 act0 | ret-lst ip-neds'
    dup                                             \ sta1 act0 | ret-lst ip-neds' ip-neds'
    #2 pick                                         \ sta1 act0 | ret-lst ip-neds' ip-neds' ret-lst
    need-list-append                                \ sta1 act0 | ret-lst ip-neds'
    need-list-deallocate                            \ sta1 act0 | ret-lst

    \ Check for corner needs.
    over                                            \ sta1 act0 | ret-lst act0
    action-get-corners corner-list-calc-needs       \ sta1 act0 | ret-lst crn-neds'
    dup                                             \ sta1 act0 | ret-lst crn-neds' crn-neds'
    #2 pick                                         \ sta1 act0 | ret-lst crn-neds' crn-neds' ret-lst
    need-list-append                                \ sta1 act0 | ret-lst crn-neds'
    need-list-deallocate                            \ sta1 act0 | ret-lst

    \ Check if the current state is not in a group, and is not represented by a pnc square.
    #2 pick #2 pick                                 \ sta1 act0 | ret-lst sta1 act0
    action-calc-state-not-in-group-needs            \ sta1 act0 | ret-lst sta-neds'
    dup                                             \ sta1 act0 | ret-lst sta-neds' sta-neds'
    #2 pick                                         \ sta1 act0 | ret-lst sta-neds' sta-neds' ret-lst
    need-list-append                                \ sta1 act0 | ret-lst sta-neds'
    need-list-deallocate                            \ sta1 act0 | ret-lst

    \ Check for group confirm needs.
    over action-calc-group-confirm-needs            \ sta1 act0 | ret-lst grp-neds'
    dup                                             \ sta1 act0 | ret-lst grp-neds' grp-neds'
    #2 pick                                         \ sta1 act0 | ret-lst grp-neds' grp-neds' ret-lst
    need-list-append                                \ sta1 act0 | ret-lst grp-neds'
    need-list-deallocate                            \ sta1 act0 | ret-lst

    \ Check for clean up.
    dup list-is-empty?                              \ sta1 act0 | ret-lst bool
    if
        over action-get-cull-squares-trigger        \ sta1 act0 | ret-lst bool
        if
            over                                    \ sta1 act0 | ret-lst act0
            action-cull-unneeded-squares            \ sta1 act0 | ret-lst
            false #2 pick _action-set-cull-squares-trigger  \ sta1 act0 | ret-lst
        then
    else
        true #2 pick _action-set-cull-squares-trigger   \ sta1 act0 | ret-lst
    then

    \ Clean up.
    nip nip                                        \ ret-lst
;

\ Return a step, given reg-to, reg-from, and a rule.
: action-make-planstep ( reg-to reg-from alt-rul rul1 act0 -- stp )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-rule
    #2 pick 0<>
    if
        assert-3os-is-rule
    then
    assert-4os-is-region
    assert-5os-is-region

    \ Bring alt-rule forward.
    rot                                     \ reg-to reg-from rul1 act0 | alt-rul

    \ Get number unwanted changes.
    #4 pick #4 pick #4 pick                 \ | alt-rul reg-to reg-from rul1
    rule-number-unwanted-changes            \ | alt-rul u-unw
    \ Add 2 if there is an alt-rul
    over 0<> if #2 + then
    swap                                    \ | u-unw alt-rul

    \ Make step.
    #3 pick                                 \ | u-unw alt-rul rul1
    #3 pick                                 \ | u-unw alt-rul rul1 act0
    planstep-new                            \ | u-unw stp

    \ Set number unwanted changes.
    tuck                                    \ | stp u-unw stp
    planstep-set-number-unwanted-changes    \ | stp

    \ Clean up.
    2nip                                    \ reg-to act0 stp
    nip nip                                 \ stp
;

\ Return a planstep list, given reg-to, reg-from, and a rule list.
\ Rule list is from a single group, with one, or two rules.
: action-planstep-list-from-rule-list ( cngs4 reg-to reg-from rul-lst1 act0 -- plnstp-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-rule-list
    assert-3os-is-region
    assert-4os-is-region
    assert-5os-is-changes

    over list-get-length            \ cngs4 reg-to reg-from rul-lst1 act0 len
    0= abort" list length zero?"

    over list-get-length            \ cngs4 reg-to reg-from rul-lst1 act0 len
    #2 > abort" list length GT 2?"

    \ Init return list.
    list-new                        \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst

    #2 pick list-get-length         \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst len

    1 =                             \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst bool
    if
        \ Build plan-step with zero alternate rule.

        \ Get from-to regions.
        #4 pick #4 pick             \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst reg-to reg-from
        \ Get null alternate rule.
        0                           \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst reg-to reg-from alt-rul
        \ Get rule from list.
        #5 pick list-get-first-item \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst reg-to reg-from alt-rul rul0
        \ Get action.
        #5 pick                     \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst reg-to reg-from alt-rul rul0 act0
        \ Make new planstep.
        action-make-planstep        \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst stp
        over list-push-struct       \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst
        \ Return
        2nip                        \ cngs4 reg-to act0 plnstp-lst
        2nip nip                    \ plnstp-lst
        exit
    then

    \ Must be two rules.

    \ Get the first rule changes.
    #5 pick                         \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4
    #3 pick                         \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 inx rul-lst1
    list-get-first-item             \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 rul0
    rule-get-changes                \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 rul0-cngs'

    \ Check for non-null intersection.
    2dup changes-intersection       \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 rul0-cngs' cngs-int'
    swap changes-deallocate         \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 cngs-int'
    dup                             \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 cngs-int' cngs-int'
    changes-not-null?               \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 cngs-int' bool
    swap changes-deallocate         \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 bool

    if                              \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4
        \ Build planstep from first rule.

        \ Get second rule from list.
        #5 pick #5 pick             \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 reg-to reg-from
        #5 pick                     \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 reg-to reg-from inx rul-lst1
        list-get-second-item        \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 reg-to reg-from alt-rul
        \ Get first rule from list.
        #6 pick list-get-first-item \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 reg-to reg-from alt-rul rul0
        \ Get action.
        #6 pick                     \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 reg-to reg-from alt-rul rul0 act0
        \ Make and save new planstep.
        action-make-planstep        \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 stp
        #2 pick list-push-struct    \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4
    then

    \ Get the second rule changes.
    #3 pick                         \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 inx plnstp-lst
    list-get-second-item            \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 rul1
    rule-get-changes                \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 rul1-cngs'

    \ Check for non-null intersection.
    2dup changes-intersection       \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 rul0-cngs' cngs-int'
    swap changes-deallocate         \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs4 cngs-int'
    nip                             \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs-int'
    dup                             \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs-int' cngs-ist'
    changes-not-null?               \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst cngs-int' bool
    swap changes-deallocate         \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst bool

    if
        \ Build plan-step for the second rule in the list.

        \ Get from-to regions.
        #4 pick #4 pick             \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst reg-to reg-from
        \ Get first rule from list.
        #4 pick list-get-first-item \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst reg-to reg-from alt-rul
        \ Get second rule from list.
        #5 pick list-get-second-item    \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst reg-to reg-from alt-rul rul0
        \ Get action.
        #5 pick                     \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst reg-to reg-from alt-rul rul0 act0
        \ Make and save new planstep.
        action-make-planstep        \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst stp
        over list-push-struct       \ cngs4 reg-to reg-from rul-lst1 act0 plnstp-lst
    then

    \ Return.
    2nip                            \ cngs4 reg-to act0 plnstp-lst
    2nip nip                        \ plnstp-lst
;

\ Return a list of possible plansteps, given to/from regions.
\ Steps may, or may not, intersect the to/from regions.
\ If they do not intersect, there are no restrictions.
: action-calc-possible-steps ( cngs3 reg-to reg-from act0 -- plnstp-lst )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-region
    assert-3os-is-region
    assert-4os-is-changes
    #2 pick #2 pick                                 \ | reg-to reg-from
    swap region-superset-of                         \ | bool
    abort" action-calc-plansteps-by-changes: region subset?"    \ |

    \ cr ." action-calc-plansteps-by-changes: Dom: " dup action-get-parent-domain domain-get-inst-id-xt execute #3 dec.r
    \ space ." Act: " dup action-get-inst-id #3 dec.r
    \ space ." reg-to: " #2 pick .region space ." reg-from: " over .region cr

    \ Init return list.
    list-new                                \ cngs3 reg-to reg-from act0 ret-lst

    over action-get-groups                  \ cngs3 reg-to reg-from act0 ret-lst grp-lst
    list-get-links                          \ cngs3 reg-to reg-from act0 ret-lst grp-lnk
    begin
        ?dup
    while
        dup link-get-data                   \ cngs3 reg-to reg-from act0 ret-lst grp-lnk grpx

        \ Check if group might apply.
        group-get-pn                        \ cngs3 reg-to reg-from act0 ret-lst grp-lnk pn
        #3 <                                \ cngs3 reg-to reg-from act0 ret-lst grp-lnk flag
        if                                  \ cngs3 reg-to reg-from act0 ret-lst grp-lnk
            \ Get rules, if any.
            #5 pick                         \ cngs3 reg-to reg-from act0 ret-lst grp-lnk cngs3
            over link-get-data              \ cngs3 reg-to reg-from act0 ret-lst grp-lnk cngs3 grpx
            group-rules-for-changes         \ cngs3 reg-to reg-from act0 ret-lst grp-lnk rul-lst'

            dup list-is-empty?              \ cngs3 reg-to reg-from act0 ret-lst grp-lnk rul-lst' bool
            if
                list-deallocate             \ cngs3 reg-to reg-from act0 ret-lst grp-lnk
            else                            \ cngs3 reg-to reg-from act0 ret-lst grp-lnk
                \ Get planstep from rules.
                #6 pick #6 pick #6 pick             \ cngs3 reg-to reg-from act0 ret-lst grp-lnk rul-lst' cngs3 reg-to reg-from
                #3 pick                             \ cngs3 reg-to reg-from act0 ret-lst grp-lnk rul-lst' cngs3 reg-to reg-from rul-lst'
                #7 pick                             \ cngs3 reg-to reg-from act0 ret-lst grp-lnk rul-lst' cngs3 reg-to reg-from rul-lst' act0
                action-planstep-list-from-rule-list \ cngs3 reg-to reg-from act0 ret-lst grp-lnk rul-lst' plnstp-lst'
                swap rule-list-deallocate           \ cngs3 reg-to reg-from act0 ret-lst grp-lnk plnstp-lst'

                \ Append planstep-list to return list.
                dup                         \ cngs3 reg-to reg-from act0 ret-lst grp-lnk plnstp-lst' plnstp-lst'
                #3 pick                     \ cngs3 reg-to reg-from act0 ret-lst grp-lnk plnstp-lst' plnstp-lst' ret-lst
                planstep-list-append        \ cngs3 reg-to reg-from act0 ret-lst grp-lnk plnstp-lst'
                planstep-list-deallocate    \ cngs3 reg-to reg-from act0 ret-lst grp-lnk
            then
        then

        link-get-next                       \ cngs3 reg-to reg-from act0 ret-lst grp-lnk
    repeat
                                            \ cngs3 reg-to reg-from act0 ret-lst
\    dup list-is-not-empty?
\    if
\        cr
\        ." Dom: " current-domain-id #3 dec.r
\        space ." Act: " current-action-id #3 dec.r
\        space ." action-calc-possible-steps: returns: " dup .planstep-list
\        cr
\    then

    2nip                                    \ cngs3 act0 ret-lst
    nip nip                                 \ ret-lst
;

\ Return a corner matching a given anchor state.
: ?action-find-corner ( sta1 act0 -- crn t | f )
    \ Check args.
    assert-tos-is-action
    assert-nos-is-value

    action-get-corners          \ sta1 crn-lst
    corner-list-find-corner
;
