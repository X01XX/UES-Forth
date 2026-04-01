\ Implement a group struct and functions.

#43717 constant group-id
    #5 constant group-struct-number-cells

\ Struct fields
0                           constant group-header-disp      \ 16 bits, [0] struct id, [1] use count (16), [1] pnc (8 bits)
group-header-disp   cell+   constant group-region-disp      \ The group region.
group-region-disp   cell+   constant group-r-region-disp    \ A Region covered by the group rules, often a proper subset of the group-region.
group-r-region-disp cell+   constant group-squares-disp     \ A square list.
group-squares-disp  cell+   constant group-rules-disp       \ A RuleStore.

0 value group-mma \ Storage for group mma instance.

\ Init group mma, return the addr of allocated memory.
: group-mma-init ( num-items -- ) \ sets group-mma.
    dup 1 <
    if
        ." group-mma-init: Invalid number of items."
        abort
    then

    cr ." Initializing Group store."
    group-struct-number-cells swap mma-new to group-mma
;

\ Check instance type.
: is-allocated-group ( addr -- flag )
    get-first-word          \ w t | f
    if
        group-id =
    else
        false
    then
;

\ Check TOS for group, unconventional, leaves stack unchanged.
: assert-tos-is-group ( tos -- tos )
    dup is-allocated-group
    false? if
        s" TOS is not an allocated group"
       .abort-xt execute
    then
;

\ Check NOS for group, unconventional, leaves stack unchanged.
: assert-nos-is-group ( nos tos -- nos tos )
    over is-allocated-group
    false? if
        s" NOS is not an allocated group"
       .abort-xt execute
    then
;

' assert-nos-is-group to assert-nos-is-group-xt

\ Start accessors.

\ Return the group region.
: group-get-region ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-region-disp + \ Add offset.
    @                   \ Fetch the field.
;

\ Set the region of a group instance, use only in this file.
: _group-set-region ( reg1 addr -- )
    group-region-disp + \ Add offset.
    !struct             \ Set the field.
;

\ Return the group squares region.
: group-get-r-region ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-r-region-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the square region of a group instance, use only in this file.
: _group-set-r-region ( reg1 addr -- )
    group-r-region-disp +   \ Add offset.
    !struct                 \ Set the field.
;

\ Return group 8-bit pnc value, as a bool.
: group-get-pnc ( sqr0 -- bool )
    \ Check arg.
    assert-tos-is-group

    4c@
    0<>     \ Change 255 to -1
;

: _group-set-pnc ( pnc sqr -- )
    4c!
;

: group-get-rules ( sqr0 -- rulstr )
    \ Check arg.
    assert-tos-is-group

    group-rules-disp + @
;

: _group-set-rules ( rulstr1 sqr0 -- )
    group-rules-disp +
    !struct
;

\ Return the group squares.
: group-get-squares ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-squares-disp +    \ Add offset.
    @                       \ Fetch the field.
;

\ Set the squares field of a group instance, use only in this file.
: _group-set-squares ( sqr-lst addr -- )
    group-squares-disp +    \ Add offset.
    !struct                 \ Set the field.
;

: group-get-pn ( grp0 -- pn )
    \ Check arg.
    assert-tos-is-group

    group-get-rules
    rulestore-get-pn
;

: _group-update-r-region ( reg1 grp0 -- )
    \ Check arg.
    assert-tos-is-group
    assert-nos-is-region

    dup group-get-r-region -rot \ reg-old reg1 grp0
    _group-set-r-region         \ reg-old
    region-deallocate           \ Deallocate last, so struct field is never invalid.
;

: _group-update-rules ( ruls1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-rulestore

    dup group-get-rules -rot    \ ruls-old ruls1 grp0
    _group-set-rules            \ ruls-old
    rulestore-deallocate        \ Deallocate last, so struct field is never invalid.
;

\ End accessors.

\ Return a new group, given a region and square-list.
: group-new    ( sqrs1 reg0 -- group )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-list

    over list-is-empty?
    if
        ." empty square list?"
        abort
    then

   \ Allocate space.
    group-mma mma-allocate      \ s r addr

    \ Store id.
    group-id over               \ s r addr id addr
    struct-set-id               \ s r addr

    \ Init use count.
    0 over                      \ s r addr 0 addr
    struct-set-use-count        \ s r addr


    \ Set region.
    tuck                        \ s addr r addr
    _group-set-region           \ s addr

    \ Set r-region
    over square-list-region     \ s addr, reg t | f
    0= abort" region not found?"
    over _group-set-r-region    \ s addr

    \ Set rules
    over square-list-get-rules  \ s addr, ruls t | f
    0=
    if  dup group-get-region cr ." Group: " .region
        space ." Group squares cannot form rules."
        space over .square-list cr
        abort
    then
                                \ s addr rules
    over _group-set-rules       \ s addr

    \ Set pnc
    \ over square-list-pnc        \ s addr pnc
    false
    over _group-set-pnc         \ s addr

    \ Set squares
    tuck                        \ addr s addr
    _group-set-squares          \ addr
;

: group-deallocate ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    dup struct-get-use-count      \ grp0 count

    #2 <
    if
        \ Deallocate instance.
        dup group-get-region region-deallocate
        dup group-get-r-region region-deallocate
        dup group-get-rules rulestore-deallocate
        dup group-get-squares square-list-deallocate
        group-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a group region is equal to a given region.
: group-region-eq ( reg1 grp0 -- flag )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-region

    group-get-region
    region-eq?
;

: .group ( grp -- )
    \ Check arg.
    assert-tos-is-group

    ." Grp: "
    dup group-get-region .region
    space ." - "
    dup group-get-r-region .region
    space
    dup group-get-rules  .rulestore
    space
    group-get-squares   .square-list-states
;

\ Print a group region.
: .group-region ( grp -- )
    \ Check arg.
    assert-tos-is-group

    group-get-region .region
;

\ Recalc a group r-region and rules.
: group-recalc ( grp0 -- )
    \ Check arg.
    assert-tos-is-group

    \ Generate pn-eq square-list to work on.
    dup group-get-squares           \ grp0 sqr-lst
    dup square-list-highest-pn      \ grp0 sqr-lst hpn
    swap square-list-eq-pn          \ grp0 sqr-lst'

    dup square-list-region          \ grp0 sqr-lst', regx t | f
    0= abort" region not found?"
    #2 pick _group-update-r-region  \ grp0 sqr-lst'

    dup square-list-get-rules       \ grp0 sqr-lst', ruls t | f
    false? if
        rulestore-new-0
    then

    #2 pick _group-update-rules     \ grp0 sqr-lst'

    square-list-deallocate          \ grp0
    drop                            \
;

\ Check a square for effects on the r-region and rules.
\ Used for a new, or changed, square.
: group-check-square ( sqr1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square

    \ Check square belongs in group.
    over square-get-state       \ sqr1 grp0 sta
    over group-get-region       \ sqr1 grp0 sta reg
    region-superset-of-state?   \ sqr1 grp0 flag
    0= abort" square not in group?"

    \ Check if square-pn is LT group-pn.
    over square-get-pn          \ sqr1 grp0 s-pn
    over group-get-pn           \ sqr1 grp0 s-pn g-pn
    <                           \ sqr1 grp0 flag
    if
        2drop                   \
        exit
    then

    \ Check if square-pn is GT group-pn.
    over square-get-pn          \ sqr1 grp0 s-pn
    over group-get-pn           \ sqr1 grp0 s-pn g-pn
    >                           \ sqr1 grp0 flag
    if
        nip                     \ grp0
        group-recalc            \
        exit
    then

    \ Square pn = group pn.

    \ Check if square is outside of the current rule region.
    over square-get-state       \ sqr1 grp0 sta
    over group-get-r-region     \ sqr1 grp0 sta sreg
    region-superset-of-state?   \ sqr1 grp0 flag
    if
        2drop
    else
        nip                     \ grp0
        group-recalc            \
    then
;

\ Add a square to a group.
: group-add-square ( sqr1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square

    cr
    ." Dom: " current-domain-id #3 dec.r
    space ." Act: " current-action-id #3 dec.r
    space ." group " dup .group-region space ." adding square " over .square-state
    cr

    \ Check square belongs in group.
    over square-get-state       \ sqr1 grp0 sta
    over group-get-region       \ sqr1 grp0 sta reg
    region-superset-of-state?   \ sqr1 grp0 flag
    0= abort" square not in group?"

    \ Check if square is already in the group.
    \ Possibly the square is in the group due to altering the incompatible pair list
    \ and logical structure.
    over square-get-state       \ sqr1 grp0 sta
    over group-get-squares      \ sqr1 grp0 sta sqr-lst
    square-list-member          \ sqr1 grp0 flag
    if
        2drop
        exit
    then

    \ Add square to square list.
    over                        \ sqr1 grp0 sqr1
    over group-get-squares      \ sqr1 grp0 sqr1 sqr-lst
    square-list-push-end        \ sqr1 grp0

    group-check-square
;

\ Return true if two groups are equal.
: group-eq ( grp1 grp0 -- flag )
     \ Check args.
    assert-tos-is-group
    assert-nos-is-group

    group-get-region
    swap
    group-get-region
    region-eq?
;

\ Return true, if a state is in a group region.
: group-state-in ( sta1 grp0 -- flag )
     \ Check args.
    assert-tos-is-group
    assert-nos-is-value

    group-get-region            \ sta1 reg
    region-superset-of-state?   \ flag
;

\ Return true, if a state is in a group r-region.
: group-state-in-r ( sta1 grp0 -- flag )
     \ Check args.
    assert-tos-is-group
    assert-nos-is-value

    group-get-r-region          \ sta1 reg
    region-superset-of-state?   \ flag
;

: group-calc-changes ( grp0 -- cngs )
    \ Check args.
    assert-tos-is-group

    group-get-rules         \ rulestore
    rulestore-calc-changes  \ changes
;

\ return a list of rules, where at least one contains at least one change.
: group-rules-for-changes ( cngs1 grp0 -- rul-lst )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-changes

    over                            \ cngs1 grp0 cngs1
    over group-get-rules            \ cngs1 grp0 cngs1 ruls
    rulestore-rules-with-changes    \ cngs1 grp0 rul-lst

\    dup list-is-not-empty?
\    if
\        cr
\        ." Dom: " current-domain-id #3 dec.r
\        space ." Act: " current-action-id #3 dec.r
\        space ." group-rules-for-changes: grp: " over group-get-region .region space ." cngs: " #2 pick .changes
\        space ." rules: " dup .rule-list
\        cr
\    then

    nip nip                         \ rul-lst
;

\ Return a need to confirm a group.
: group-get-confirm-need-state ( grp0 -- sta t | f )
    \ Check args.
    assert-tos-is-group
    \ cr ." group " dup group-get-region .region cr

    dup group-get-squares       \ grp0 | sqr-lst
    list-get-first-item         \ grp0 | sqr-0
    dup square-get-pnc          \ grp0 | sqr-0 pnc
    if                          \ grp0 | sqr-0
    else
        \ Return square state.
        square-get-state        \ act0 | sta-0
        nip                     \ sta-0
        true
        exit
    then
                                \ grp0 | sqr-0

    square-get-state            \ grp0 | sta-0

    \ Get far state.
    over group-get-region       \ grp0 | sta-0 g-reg

    \ Get state far from sta-0,

    region-far-from-state       \ grp0 | sta-f

    \ Find square.
    dup                         \ grp0 | sta-f sta-f
    #2 pick group-get-squares   \ grp0 | sta-f sta-f grp-sqrs
    square-list-find            \ grp0 | sta-f, sqr-f t | f
    if
        square-get-pnc          \ grp0 | sta-f pnc
        if
            2drop
            false
        else
            nip true
        then
    else
        nip true
    then
;

\ Return true if a group has at least one needed change.
: group-has-any-change ( cngs1 grp0 -- flag )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-changes

    dup group-get-pn            \ cngs1 grp0 pn
    #3 = if
        2drop
        false
        exit
    then

    group-get-rules             \ cngs1 rul-str
    rulestore-makes-change      \ flag
;

\ Return true if a group uses a square.
: group-uses-square ( sta1 grp0 -- bool )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-value

    \ Check if a square is in the group.
    over                        \ sta1 grp0 sta1
    over group-get-squares      \ sta1 grp0 sta1 sqr-lst
    square-list-member          \ sta1 grp0 bool
    if
    else
        2drop
        false
        exit
    then

    \ Check if the square is item 0 in the list.
    dup group-get-squares       \ sta1 grp0 0 sqr-lst
    list-get-first-item         \ sta1 grp0 sqr0
    square-get-state            \ sta1 grp0 sta0
    #2 pick                     \ sta1 grp0 sqr0 sta1
    =                           \ sta1 grp0 bool
    if
        2drop
        true
        exit
    then

    \ Check if square state is equal to square far from square 0.
    dup group-get-squares       \ sta1 grp0 0 sqr-lst
    list-get-first-item         \ sta1 grp0 sqr0
    square-get-state            \ sta1 grp0 sta
    over group-get-region       \ sta1 grp0 sta reg
    region-far-from-state       \ sta1 grp0 sta-far
    #2 pick                     \ sta1 grp0 sta-far sta1
    =                           \ sta1 grp0 bool
    if
        2drop
        true
        exit
    then

    \ Check if far square exists.
    \ If so, passed square is between the first ad far square, and is not needed.
    dup group-get-squares       \ sta1 grp0 0 sqr-lst
    list-get-first-item         \ sta1 grp0 sqr0
    square-get-state            \ sta1 grp0 sta
    over group-get-region       \ sta1 grp0 sta reg
    region-far-from-state       \ sta1 grp0 sta-far
    over group-get-squares      \ sta1 grp0 sta-far sqr-lst
    square-list-member          \ sta1 grp0 bool
    nip nip                     \ bool
    invert
;

\ Remove a square from a group.
: group-remove-square ( sta1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-value

    group-get-squares           \ sta1 sqr-lst

    2dup square-list-member     \ sta1 sqr-lst bool
    if
        square-list-remove      \ bool
        drop
    else
        2drop
    then
;

\ If needed, change the first square in the square list.
: group-set-first-square ( sta1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-value

    cr
    ." Dom: " current-domain-id #3 dec.r
    space ." Act: " current-action-id #3 dec.r
    space ." group-set-first-square: grp: " dup group-get-region .region
    space ." state: " over .value
    cr

    \ Check first square.
    group-get-squares           \ sta1 sqr-lst
    dup list-get-first-item     \ sta1 sqr-lst sqr0
    square-get-state            \ sta1 sqr-lst sta0
    #2 pick                     \ sta1 sqr-lst sta0 sta1
    =                           \ sta1 sqr-lst bool
    if
        2drop
\ space ." already set" cr
        exit
    then

\ space ." setting..." cr
    \ Set square.
    2dup                        \ sta1 sqr-lst sta1 sqr-lst
    square-list-find            \ sta1 sqr-lst, sqr t | f
    if
        rot                         \ sqr-lst sqr sta1
        #2 pick                     \ sqr-lst sqr sta1 sqr-lst
        square-list-remove          \ sqr-lst sqr bool
        if
            swap                        \ sqr sqr-lst
            square-list-push            \
        else
            cr ." square not removed?"
            2drop
        then
    else
        cr ." Square not found?" cr
        2drop
    then
;
