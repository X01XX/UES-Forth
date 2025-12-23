\ Implement a pathstep struct and functions.
\
\ A pathstep may be added to a pathstep list to make a path-plan.

#53197 constant pathstep-id
    #5 constant pathstep-struct-number-cells

\ Struct fields.
0                                       constant pathstep-header-disp           \ id (16) use count (16) number unwanted changes (8)
pathstep-header-disp            cell+   constant pathstep-rules-disp            \ A ruleicorr instance addr.
\ Store frequently used calculated fields, to decrease cycles and memory allocation/deallocation.
pathstep-rules-disp             cell+   constant pathstep-initial-regions-disp  \ A regioncorr instance addr.
pathstep-initial-regions-disp   cell+   constant pathstep-result-regions-disp   \ A regioncorr instance addr.
pathstep-result-regions-disp    cell+   constant pathstep-changes-disp          \ A changescorr instance addr.

0 value pathstep-mma \ Storage for pathstep mma instance.

\ Init pathstep mma, return the addr of allocated memory.
: pathstep-mma-init ( num-items -- ) \ sets pathstep-mma.
    dup 1 <
    abort" pathstep-mma-init: Invalid number of items."

    cr ." Initializing Step store."
    pathstep-struct-number-cells swap mma-new to pathstep-mma
;

\ Check pathstep mma usage.
: assert-pathstep-mma-none-in-use ( -- )
    pathstep-mma mma-in-use 0<>
    abort" pathstep-mma use GT 0"
;

\ Check instance type.
: is-allocated-pathstep ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup pathstep-mma mma-within-array
    if
        struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
        pathstep-id =
    else
        drop false
    then
;

\ Check TOS for pathstep, unconventional, leaves stack unchanged.
: assert-tos-is-pathstep ( arg0 -- arg0 )
    dup is-allocated-pathstep
    is-false if
        s" TOS is not an allocated pathstep"
        .abort-xt execute
    then
;

\ Check NOS for pathstep, unconventional, leaves stack unchanged.
: assert-nos-is-pathstep ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-pathstep
    is-false if
        s" NOS is not an allocated pathstep"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the pathstep rule.
: pathstep-get-rules ( pstp0 -- rulc )
    \ Check arg.
    assert-tos-is-pathstep

    pathstep-rules-disp +   \ Add offset.
    @                       \ Fetch the field.
;

\ Set the rule of a pathstep instance, use only in this file.
: _pathstep-set-rules ( rulc1 pstp0 -- )
    pathstep-rules-disp +   \ Add offset.
    !                       \ Set field.
;

\ Return the pathstep initial-region.
: pathstep-get-initial-regions ( pstp0 -- regc )
    \ Check arg.
    assert-tos-is-pathstep

    pathstep-initial-regions-disp + \ Add offset.
    @                               \ Fetch the field.
;

\ Set the initial-region of a pathstep instance, use only in this file.
: _pathstep-set-initial-regions ( regc1 pstp0 -- )
    pathstep-initial-regions-disp + \ Add offset.
    !                               \ Set field.
;

\ Return the pathstep rule.
: pathstep-get-result-regions ( pstp0 -- regc )
    \ Check arg.
    assert-tos-is-pathstep

    pathstep-result-regions-disp +  \ Add offset.
    @                               \ Fetch the field.
;

\ Set the result-region of a pathstep instance, use only in this file.
: _pathstep-set-result-regions ( regc1 pstp0 -- )
    pathstep-result-regions-disp +  \ Add offset.
    !                               \ Set field.
;

\ Return the pathstep changes.
: pathstep-get-changes ( pstp0 -- cngsc )
    \ Check arg.
    assert-tos-is-pathstep

    pathstep-changes-disp + \ Add offset.
    @                       \ Fetch the field.
;

\ Set the changes of a pathstep instance, use only in this file.
: _pathstep-set-changes ( cngsc1 pstp0 -- )
    pathstep-changes-disp + \ Add offset.
    !                       \ Set field.
;

\ Return pathstep number-unwanted-changes.
: pathstep-get-number-unwanted-changes ( pstp0 -- u )
    \ Check arg.
    assert-tos-is-pathstep

    4c@
;

\ Set pathstep number-unwanted-changes.
: pathstep-set-number-unwanted-changes ( u pstp0 -- )
    \ Check args.
    assert-tos-is-pathstep

    4c!
;

\ End accessors.

\ Return a new pathstep, given a rule and an action.
: pathstep-new    ( rulc1 -- pstp )
    \ Check args.
    assert-tos-is-rulecorr

   \ Allocate space.
    pathstep-mma mma-allocate               \ rulc1 pstp

    \ Store id.
    pathstep-id over                        \ rulc1 pstp id pstp
    struct-set-id                           \ rulc1 pstp

    \ Init use count.
    0 over struct-set-use-count             \ rulc1 pstp

    \ Set initial-region.
    over rulecorr-calc-initial-regions      \ rulc1 pstp reg
    1 over struct-set-use-count
    over _pathstep-set-initial-regions      \ rulc1 pstp

    \ Set result-region.
    over rulecorr-calc-result-regions       \ rulc1 pstp reg
    1 over struct-set-use-count
    over _pathstep-set-result-regions       \ rulc1 pstp

    \ Set changes.
    over rulecorr-get-changes               \ rulc1 pstp cngs
    1 over struct-set-use-count
    over _pathstep-set-changes              \ rulc1 pstp

    \ Set rule.
    tuck                                    \ pstp rulc1 pstp
    over struct-inc-use-count
    _pathstep-set-rules                     \ pstp

    \ Init number-unwanted-changes.
    0 over                                  \ pstp int pstp
    pathstep-set-number-unwanted-changes    \ pstp
;

: .pathstep ( pstp0 -- )
    \ Check arg.
    assert-tos-is-pathstep

    ." [ "

    dup pathstep-get-rules                  \ pstp0 rulc
    .rulecorr                               \ pstp0

    space pathstep-get-number-unwanted-changes dec.
    space ." ]"
;

\ Deallocate a pathstep instance.
: pathstep-deallocate ( pstp0 -- )
    \ Check arg.
    assert-tos-is-pathstep

    dup struct-get-use-count      \ pstp0 count

    #2 <
    if
        \ Deallocate imbedded structs.
        dup pathstep-get-rules
        rulecorr-deallocate

        dup pathstep-get-initial-regions
        regioncorr-deallocate

        dup pathstep-get-result-regions
        regioncorr-deallocate

        dup pathstep-get-changes
        changescorr-deallocate

        \ Deallocate instance.
        pathstep-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if a pathstep's changes intersects a given changes.
: pathstep-intersects-changes ( cngsc1 pstp0 -- flag )
    \ Check args.
    assert-tos-is-pathstep
    assert-nos-is-changes

    pathstep-get-changes            \ cngsc1 s-cngsc
    changescorr-intersect           \ flag
;

\ Return true if two pathsteps can be linked pstp1 result region to pstp0 initial region.
: pathstep-can-be-linked ( pstp1 pstp0 -- bool )
    \ Check args.
    assert-tos-is-pathstep
    assert-nos-is-pathstep

    swap pathstep-get-result-regions    \ pstp0 regc-r
    swap pathstep-get-initial-regions   \ regc-r regc-i
    regioncorr-intersects               \ bool
;
