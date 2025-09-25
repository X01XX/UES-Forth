\ Implement a group struct and functions.

#23197 constant group-id                                                                                  
    #5 constant group-struct-number-cells

\ Struct fields
0 constant group-header                                 \ id (16) use count (16) pn (8) pnc (8)
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
    over square-list-region     \ s addr reg
    over _group-set-r-region    \ s addr

    \ Set rules
    over square-list-get-rules  \ s addr result flag
    0=
    if  dup group-get-region cr ." Group: " .region
        space ." Group squares cannot form rules."
        space over .square-list cr
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
                \ cr dup ." sqr-lst: " .square-list cr
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

\ Return a list of possible forward-chaining steps, given a sample.
\ Where the sample is in the group r-region.
: group-calc-forward-steps ( smpl1 grp0 -- stp-lst )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-sample
    \ cr ." group-calc-forward-steps:" cr

    list-new -rot           \ ret-lst smpl1 grp0
    dup group-get-rules     \ ret-lst smpl1 grp0 grp-ruls
    swap group-get-pn       \ ret-lst smpl1 grp-ruls pn
    case
        1 of
                                            \ ret-lst smpl1 grp-ruls
            over                            \ ret-lst smpl1 grp-ruls | smpl1
            over rulestore-get-rule-0       \ ret-lst smpl1 grp-ruls | smpl1 rul0
            rule-calc-forward-step          \ ret-lst smpl1 grp-ruls | stpx true | false
            if                              \ ret-lst smpl1 grp-ruls | stpx
                3 pick                      \ ret-lst smpl1 grp-ruls | stpx ret-lst
                step-list-push-xt execute   \ ret-lst smpl1 grp-ruls
            then
            2drop
        endof
        2 of
            cr ." group-calc-forward-steps: TODO" cr
            2drop
        endof
        3 of
            2drop
        endof
        cr ." Invalid pn value" cr
        abort
    endcase
;

\ Return 0, 1 or 2, backward steps.
: group-calc-backward-steps ( smpl1 grp0 -- stp-lst )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-sample

    \ cr ." group-calc-backward-steps: " dup .group space over .sample cr

    \ Init return list.
    list-new -rot                           \ ret smpl1 grp0

    \ Get group pn.
    dup group-get-pn                        \ ret smpl1 grp0 pn

    \ Process group by pn value.
    case
        1 of
            group-get-rules                 \ ret smpl1 ruls
            rulestore-get-rule-0            \ ret smpl1 rul
            rule-calc-backward-step         \ ret, stpx t | f
            if
                over                        \ ret stpx ret
                step-list-push-xt execute   \ ret
            then
        endof
        2 of
            group-get-rules                 \ ret smpl1 ruls
            2dup                            \ ret smpl1 ruls smpl1 ruls
            rulestore-get-rule-0            \ ret smpl1 ruls smpl1 rul0
            rule-calc-backward-step         \ ret smpl1 ruls, stpx t | f
            if
                \ Get/set alt sample
                dup                         \ ret smpl1 ruls stpx stpx
                step-get-sample-xt execute  \ ret smpl1 ruls stpx smpl2
                sample-get-initial          \ ret smpl1 ruls stpx s-i
                2 pick rulestore-get-rule-1 \ ret smpl1 ruls stpx s-i rul1
                rule-apply-to-state-f       \ ret smpl1 ruls stpx, smpl2 t | f
                if
                                            \ ret smpl1 ruls stpx smpl2
                    dup sample-get-result   \ ret smpl1 ruls stpx smpl2 rstl2
                    4 pick                  \ ret smpl1 ruls stpx smpl2 rstl2 smpl1
                    sample-get-result       \ ret smpl1 ruls stpx smpl2 rstl2 rslt1
                    <> if
                        over                            \ ret smpl1 ruls stpx smpl2 stpx
                        step-set-alt-sample-xt execute  \ ret smpl1 ruls stpx
                    then
                else
                    ." no alt sample?"
                    abort
                then
                3 pick                      \ ret smpl1 ruls stpx ret
                step-list-push-xt execute   \ ret smpl1 ruls
            then
            ." TODO try rule-1 with rule-0 as alt"
            2drop
        endof
        3 of
            2drop
        endof
    endcase
    \ cr   ." -> " dup .step-list-xt execute cr
;

\ Return steps by changes, forward-chaining.
: group-get-steps-by-changes-f ( smpl1 grp0 -- stp-lst )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-sample

    \ cr ." group-get-steps-by-changes-f: " dup .group space over .sample cr

    \ Get group pn.
    dup group-get-pn                        \ smpl1 grp0 pn

    \ Handle unpredictable group.
    3 = if
        2drop
        list-new
        exit
    then

    group-get-rules                         \ smpl ruls
    rulestore-get-steps-by-changes-f        \ stp-lst
    \ cr ." at 4 " .s cr

    \ cr   ." -> " dup .step-list-xt execute cr
;

\ Return steps by changes, backward-chaining.
: group-get-steps-by-changes-b ( smpl1 grp0 -- stp-lst )
    \ Check args.
    assert-tos-is-group
    assert-nos-is-sample

    \ cr ." group-get-steps-by-changes-b: " dup .group space over .sample cr

    \ Get group pn.
    dup group-get-pn                        \ smpl1 grp0 pn

    \ Handle unpredictable group.
    3 = if
        2drop
        list-new
        exit
    then

    group-get-rules                         \ smpl ruls
    rulestore-get-steps-by-changes-b        \ stp-lst
    \ cr ." at 4 " .s cr

    \ cr   ." -> " dup .step-list-xt execute cr
;

\ Return an expansion need.
: group-get-expansion-needs ( reg1 grp0 -- ned t | f )
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
    region-intersection         \ grp0, reg-int t | false
    0= if
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
    2 pick region-x-mask        \ reg-int sta0 edg-msk x-msk
    and                         \ reg-int sta0 cng-msk
    rot region-deallocate       \ sta0 cng-msk
    \ space ." cng-msk: " dup .value cr
    ?dup
    if
        xor                         \ sta0'
    
        \ Make need.
        4 swap                      \ 4 sta0'
        cur-action-xt execute       \ 4 sta0' actx
        cur-domain-xt execute       \ 4 sta0' actx domx
        need-new-xt execute         \ nedx
        true
    else
        drop
        false
    then
;
