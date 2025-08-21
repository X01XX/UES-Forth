\ Implement a group struct and functions.

23197 constant group-id                                                                                  
    5 constant group-struct-number-cells

\ Struct fields
0 constant group-header                         \ id (16) use count (16) pn (8) pnc (8)
group-header    cell+ constant group-region     \ The group region.
group-region    cell+ constant group-r-region   \ A Region covered the group rules, often a proper subset of the group-region.
group-r-region  cell+ constant group-squares    \ A square list.
group-squares   cell+ constant group-rules      \ A RuleStore.

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

: is-not-allocated-group ( addr -- flag )
    is-allocated-group 0=
;

\ Check TOS for group, unconventional, leaves stack unchanged. 
: assert-tos-is-group ( arg0 -- arg0 )
    dup is-allocated-group 0=
    if  
        cr ." TOS is not an allocated group"
        abort
    then
;

\ Check NOS for group, unconventional, leaves stack unchanged. 
: assert-nos-is-group ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-group 0=
    if  
        cr ." NOS is not an allocated group"
        abort
    then
;

\ Start accessors.

\ Return the group region. 
: group-get-region ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-region +      \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the region of a group instance, use only in this file.
: _group-set-region ( reg1 addr -- )
    over struct-inc-use-count
    group-region +      \ Add offset.
    !                   \ Set field.
;

\ Return the group squares region. 
: group-get-r-region ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-r-region +    \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the square region of a group instance, use only in this file.
: _group-set-r-region ( reg1 addr -- )
    over struct-inc-use-count
    group-r-region +    \ Add offset.
    !                   \ Set field.
;

: group-get-pn ( sqr0 -- pn )
    \ Check arg.
    assert-tos-is-group

    4c@
;

: _group-set-pn ( pn1 sqr0 -- )
    over 1 <
    if
        ." _group-set-pn: invalid pn value"
        abort
    then

    over 3 >
    if
        ." _group-set-pn: invalid pn value"
        abort
    then

    4c!
;

\ Return group 8-bit pnc value, as a bool.
: group-get-pnc ( sqr0 -- bool )
    \ Check arg.
    assert-tos-is-group

    5c@
    0<>     \ Change 255 to -1
;

: _group-set-pnc ( pnc sqr -- )
    5c!
;

: group-get-rules ( sqr0 -- rulstr )
    \ Check arg.
    assert-tos-is-group

    group-rules + @
;

: _group-set-rules ( rulstr1 sqr0 -- )
    over struct-inc-use-count

    group-rules + !
;

\ Return the group squares. 
: group-get-squares ( addr -- reg )
    \ Check arg.
    assert-tos-is-group

    group-squares +     \ Add offset.
    @                   \ Fetch the field.
;
 
\ Set the squares field of a group instance, use only in this file.
: _group-set-squares ( sqr-lst addr -- )
    group-squares +     \ Add offset.
    !                   \ Set field.
;

\ End accessors.

\ Return a new group, given a state and result.
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
    over square-list-region     \ s addr reg
    over _group-set-r-region    \ s addr

    \ Set rules
    over square-list-get-rules  \ s addr result flag
    0=
    if
        ." Group squares cannot form rules."
        abort
    then
                                \ s addr rules
    over _group-set-rules       \ s addr

    \ Set pn
    over square-list-highest-pn \ s addr pn
    over _group-set-pn          \ s addr

    \ Set pnc
    \ over square-list-pnc        \ s addr pnc
    false
    over _group-set-pnc         \ s addr

    \ Set squares
    tuck                        \ addr s addr
    _group-set-squares          \ addr
;

: group-from-sample ( smpl -- sqr )
    \ Check arg.
    assert-tos-is-sample

    dup sample-get-result
    swap sample-get-initial
    group-new
;

: group-deallocate ( sqr0 -- )
    \ Check arg.
    assert-tos-is-group

    dup struct-get-use-count      \ sqr0 count

    2 <
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
    ." Grp: "
    dup group-get-region .region
    space ." - "
    dup group-get-r-region .region
    space
    dup group-get-rules  .rulestore
    space
    group-get-squares   .square-list-states
;

: .group-region ( grp -- )
    group-get-region .region
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

    \ Check if square is outside of the current rule region.
    over square-get-state       \ sqr1 grp0 sta
    over group-get-r-region     \ sqr1 grp0 sta sreg
    region-superset-of-state    \ sqr1 grp0 flag
    0= if
                                \ sqr1 grp0
        \ Check square pn
        over square-get-pn      \ sqr1 grp0 s-pn
        over group-get-pn       \ sqr1 grp0 s-pn g-pn
        =
        if
            \ Expand group rule region.
            over square-get-state   \ sqr1 grp0 sta
            over group-get-r-region \ sqr1 grp0 sta sreg
            tuck                    \ sqr1 grp0 sreg sta sreg
            region-union-state      \ sqr1 grp0 sreg sreg2

            \ Print change.
            cr 2 pick group-get-region ." group " .region
            space ." change r-region from " over .region
            space ." to " dup .region cr

            2 pick                  \ sqr1 grp0 sreg sreg2 grp0
            _group-set-r-region     \ sqr1 grp0 sreg

            \ Dealloc previous region.
            region-deallocate       \ sqr1 grp0

            \ Adjust rules, if pn < 3/U.
            over square-get-pn      \ sqr1 grp0 s-pn
            3 <>
            if
                dup group-get-rules     \ sqr1 grp0 rul-str
                over                    \ sqr1 grp0 rul-str grp0
                group-get-squares       \ sqr1 grp0 rul-str sqr-lst
                square-list-get-rules   \ sqr1 grp0 old-rul-str, new-rul-str true | false
                0= abort" no rulestore from square-list?"

                \ Print change.
                cr 2 pick group-get-region ." group " .region
                space ." change rules from " over .rulestore
                space ." to " dup .rulestore cr
            
                2 pick                  \ sqr1 grp0 old-rul-str new-rul-str grp0
                _group-set-rules        \ sqr1 grp0 old-rul-str
                rulestore-deallocate    \ sqr1 grp0
            then
        then
    then
                                    \ sqr1 grp0
    2drop
;

\ Add a square to a group.
: group-add-square ( sqr1 grp0 -- )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-square

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
