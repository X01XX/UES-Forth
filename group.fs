\ Implement a group struct and functions.

#43717 constant group-id                                                                                  
    #5 constant group-struct-number-cells

\ Struct fields
0 constant group-header                                 \ id (16) use count (16) pnc (8)
group-header        cell+ constant group-region-disp    \ The group region.
group-region-disp   cell+ constant group-r-region-disp  \ A Region covered the group rules, often a proper subset of the group-region.
group-r-region-disp cell+ constant group-squares-disp   \ A square list.
group-squares-disp  cell+ constant group-rules-disp     \ A RuleStore.

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

\ Check group mma usage.
: assert-group-mma-none-in-use ( -- )
    group-mma mma-in-use 0<>
    if
        ." group-mma use GT 0"
        abort
    then
;

\ Check instance type.
: is-allocated-group ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup group-mma mma-within-array 0=
    if  
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    group-id =    
;

\ Check TOS for group, unconventional, leaves stack unchanged. 
: assert-tos-is-group ( arg0 -- arg0 )
    dup is-allocated-group
    is-false if  
        s" TOS is not an allocated group"
       .abort-xt execute
    then
;

\ Check NOS for group, unconventional, leaves stack unchanged. 
: assert-nos-is-group ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-group
    is-false if
        s" NOS is not an allocated group"
       .abort-xt execute
    then
;

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
    over struct-inc-use-count
    group-region-disp + \ Add offset.
    !                   \ Set field.
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
    over struct-inc-use-count
    group-r-region-disp +   \ Add offset.
    !                       \ Set field.
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
    over struct-inc-use-count

    group-rules-disp + !
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
    !                       \ Set field.
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

    over list-is-empty
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
    \ cr ." group-new: " dup hex . decimal cr
;

: group-new-from-sample ( smpl -- sqr )
    \ Check arg.
    assert-tos-is-sample

    dup sample-get-result
    swap sample-get-initial
    group-new
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
    region-eq
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
    0= abort" rules not found?"
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
    region-superset-of-state    \ sqr1 grp0 flag
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
    region-superset-of-state    \ sqr1 grp0 flag
    0= if
        nip                     \ grp0
        group-recalc            \
    else
        2drop
    then
    
;

\ Add a square to a group.
: group-add-square ( sqr1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square

    cr ." group " dup .group-region space ." adding square " over .square-state cr
    \ Check square belongs in group.
    over square-get-state       \ sqr1 grp0 sta
    over group-get-region       \ sqr1 grp0 sta reg
    region-superset-of-state    \ sqr1 grp0 flag
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
    square-list-push            \ sqr1 grp0

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
    region-eq
;

\ Return true, if a state is in a group region.
: group-state-in ( sta1 grp0 -- flag )
     \ Check args.
    assert-tos-is-group
    assert-nos-is-value

    group-get-region            \ sta1 reg
    region-superset-of-state    \ flag
;

\ Return true, if a state is in a group r-region.
: group-state-in-r ( sta1 grp0 -- flag )
     \ Check args.
    assert-tos-is-group
    assert-nos-is-value

    group-get-r-region          \ sta1 reg
    region-superset-of-state    \ flag
;

: group-calc-changes ( grp0 -- cngs )
    \ Check args.
    assert-tos-is-group

    group-get-rules         \ rulestore
    rulestore-calc-changes  \ changes
;

\ Return a list of possible forward-chaining steps, given a from-region (tos) and a to-region (nos).
\ Return 0, 1 or 2 forward steps.
: group-calc-steps-fc ( reg-to reg-from grp0 -- stp-lst )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-region
    assert-3os-is-region
    #2 pick #2 pick                             \ | reg-to reg-from
    2dup region-superset-of                     \ | reg-to reg-from bool
    abort" group-calc-steps-fc: region subset?" \ | reg-to reg-from
    swap region-superset-of                     \ | bool
    abort" group-calc-steps-fc: region subset?" \ |
    \ cr ." group-calc-forward-steps:" cr

    group-get-rules         \ reg-to reg-from rul-str
    rulestore-calc-steps-fc \ stp-lst
;

\ Return a list of possible backward-chaining steps, given a from-region (tos) and a to-region (nos).
\ Return 0, 1 or 2 forward steps.
: group-calc-steps-bc ( reg-to reg-from grp0 -- stp-lst )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-region
    assert-tos-is-region
    \ cr ." group-calc-forward-steps:" cr

    group-get-rules         \ reg-to reg-from rul-str
    rulestore-calc-steps-bc \ stp-lst
;

\ Return a list of steps having needed changes, given a from-region (tos) and a to-region (nos).
: group-calc-steps ( reg-to reg-from grp0 -- stp-lst )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-region
    assert-tos-is-region
    \ cr ." group-calc-forward-steps:" cr

    group-get-rules         \ reg-to reg-from rul-str
    rulestore-calc-steps    \ stp-lst
;

\ Return a fill need, if any.
: group-get-fill-need ( reg1 grp0 -- ned t | f )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-region

    \ cr ." group: " dup group-get-region .region
    \ space ." r-reg: " dup group-get-r-region .region
    \ space ." reachable: " over .region

    \ Get group region.
    dup group-get-region        \ reg1 grp0 grp-reg
    rot                         \ grp0 grp-reg reg1

    \ Get group region intersection reachable region.
    2dup                        \ grp0 grp-reg reg1 grp-reg reg1
    region-intersection         \ grp0 grp-reg reg1, reg-int t | false
    if
        nip nip                 \ grp0 reg-int
    else
        cr ." region " .region space ." does not intersect " .region cr
        abort
    then

                                \ grp0 reg-int
    swap group-get-r-region     \ reg-int grp-r-reg

    2dup region-eq              \ reg-int grp-r-reg flag
    if
        drop
        region-deallocate
        false
        exit
    then

                                \ reg-int grp-r-reg
    dup region-get-state-0 swap \ reg-int r-reg-sta0 grp-r-reg
    region-edge-mask            \ reg-int sta0 edg-msk
    #2 pick region-x-mask       \ reg-int sta0 edg-msk x-msk
    and                         \ reg-int sta0 cng-msk
    rot region-deallocate       \ sta0 cng-msk
    \ space ." cng-msk: " dup .value cr
    ?dup
    if
        xor                     \ sta0'
    
        \ Make need.
        #4 swap                 \ 4 sta0'
        cur-action-xt execute   \ 4 sta0' actx
        action-make-need-xt     \ 4 sta0' actx xt
        execute                 \ nedx
        true
    else
        drop
        false
    then
;

\ Return a need to confirm a group.
: group-get-confirm-need ( grp0 -- ned t | f )
    \ Check arg.
    assert-tos-is-group

    0 over group-get-squares    \ grp0 | 0 sqr-lst
    list-get-item               \ grp0 | sqr-0
    dup square-get-pnc          \ grp0 | sqr-0 pnc
    0= if                       \ grp0 | sqr-0
        \ Make need for square 0.
        nip
        square-get-state        \ sta-0
        #5 swap                 \ 5 sta-0
        cur-action-xt execute   \ 5 sta-0 actx
        action-make-need-xt     \ 5 sta-0 actx xt
        execute                 \ nedx
        true
        exit
    else
        drop                    \ grp0
    then

    \ Check group squares.
    dup group-get-r-region      \ grp0 g-r-reg
    over group-get-squares      \ grp0 g-r-reg sqr-lst
    list-get-links              \ grp0 g-r-reg link

    begin
        ?dup
    while
        dup link-get-data               \ grp0 g-r-reg link sqrx
        dup square-get-pnc              \ grp0 g-r-reg link sqrx pnc
        0= if                           \ grp0 g-r-reg link sqrx
            dup square-get-state        \ grp0 g-r-reg link sqrx s-sta
            #3 pick                     \ grp0 g-r-reg link sqrx s-sta g-r-reg
            region-superset-of-state    \ grp0 g-r-reg link sqrx flag
            0= if                       \ grp0 g-r-reg link sqrx
                \ Make need.
                square-get-state        \ | s-sta
                #5 swap                 \ | 5 s-sta
                cur-action-xt execute   \ | 5 s-sta actx
                action-make-need-xt     \ | 5 s-sta actx xt
                execute                 \ | nedx
                2nip nip                \ nedx
                true
                exit
            else                        \ grp0 g-r-reg link sqrx
                drop                    \ grp0 g-r-reg link
            then
        else                            \ grp0 g-r-reg link sqrx
            drop                        \ grp0 g-r-reg link
        then

        link-get-next           \ grp0 g-r-reg link
    repeat
                                \ grp0 g-r-reg
    2drop
    false
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
