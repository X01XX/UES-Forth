\ Implement the changes struct and functions.
\
\ This is used to limit the generated needs of a Domain to what is achievable,
\ given the current state and all possible predictable action/group changes.
\ A bit position may never change, or its changeability may vary over time.
\
\ Rule pairs can be mutually exclusive, or have a required order.
\
\ It may not be possible to return to the initial state, which the
\ earliest actions change, and add the changes to the rule store.
\ You don't know how you got to the initial state, it just is.
\
\ So the prediction of what is achievable may be over-optimistic.

#31973 constant changes-id
     3 constant changes-struct-number-cells

\ Struct fields.
0 constant changes-header      \ 16-bits [0] struct id [1] use count.
changes-header cell+ constant changes-m01
changes-m01    cell+ constant changes-m10

0 value changes-mma    \ Storage for changes mma instance.

\ Init changes mma, return the addr of allocated memory.
: changes-mma-init ( num-items -- ) \ sets changes-mma.
    dup 1 < 
    abort" changes-mma-init: Invalid number of items."

    cr ." Initializing Changes store."
    changes-struct-number-cells swap mma-new to changes-mma
;

\ Check changes mma usage.
: assert-changes-mma-none-in-use ( -- )
    changes-mma mma-in-use 0<>
    abort" changes-mma use GT 0"
;

\ Start accessors.

\ Return the m01 field of a changes instance.
: changes-get-m01 ( cngs0 -- u)
    changes-m01 +      \ Add offset.
    @               \ Fetch the field.
;

\ Set the m01 field of a changes instance, use only in this file. 
: _changes-set-m01 ( u1 cngs0 -- )
    changes-m01 +      \ Add offset.
    !               \ Set the field.
;

\ Return the m10 field of a changes instance.
: changes-get-m10 ( cngs0 -- u)
    changes-m10 +      \ Add offset.
    @               \ Fetch the field.
;

\ Set the m10 field of a changes instance, use only in this file. 
: _changes-set-m10 ( u1 cngs0 -- )
    changes-m10 +      \ Add offset.
    !               \ Set the field.
;

\ End accessors.

\ Check instance type.

: is-allocated-changes ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup changes-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    changes-id =     
;

: is-not-allocated-changes ( addr -- flag )
    is-allocated-changes 0=
;

\ Check TOS for changes, unconventional, leaves stack unchanged. 
: assert-tos-is-changes ( cngs0 -- )
    dup is-allocated-changes 0=
    abort" TOS is not an allocated changes."
;

\ Check NOS for changes, unconventional, leaves stack unchanged. 
: assert-nos-is-changes ( rul1 ??? -- )
    over is-allocated-changes 0=
    abort" NOS is not an allocated changes."
;

\ Allocate a changes, setting id and use count only, use only in this file. 

\ Allocate a changes, setting id and use count only, use only in this file. 
: _changes-allocate ( -- cngs0 )
    \ Allocate space.
    changes-mma mma-allocate   \ addr

    \ Store id.
    changes-id over            \ addr id addr
    struct-set-id           \ addr

    \ Init use count.
    0 over struct-set-use-count \ addr
;

\ Create a changes from two numbers on the stack.
: changes-new ( msk-m10 msk-m01 -- addr)
    \ Check args.
    assert-tos-is-value
    assert-nos-is-value

    _changes-allocate       \ m10 m01 addr

    \ Store fields.
    tuck                    \ m10 addr m01 addr
    _changes-set-m01        \ m10 addr

    tuck                    \ addr m10 addr
    _changes-set-m10        \ addr
;

\ Deallocate a changes.
: changes-deallocate ( cngs0 -- )
    \ Check arg.
    assert-tos-is-changes

    dup struct-get-use-count      \ cngs0 count

    2 <
    if
        \ Deallocate instance.
        changes-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return the union of two changes.
: changes-union ( cngs1 cngs0 -- cngs )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-changes

    over changes-get-m10    \ cngs1 cngs0 1m10
    over changes-get-m10    \ cngs1 cngs0 1m10 0m10
    or                      \ cngs1 cngs0 m10'
    -rot                    \ m10' cngs1 cngs0
    changes-get-m01         \ m10' cngs1 0m01
    swap                    \ m10' 0m01 cngs1
    changes-get-m01         \ m10' 0m01 1m01
    or                      \ m10' m01'
    changes-new             \ cngs
;

\ Return a state with all possible changes applied.
: changes-apply-to-state ( sta1 cngs0 -- sta )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-value

    2dup changes-get-m10        \ sta1 cngs0 sta1 m10
    and                         \ sta1 cngs0 msk10
    swap changes-get-m01        \ sta1 msk10 m01
    2 pick !not                 \ sta1 msk10 m01 ~sta1
    and                         \ sta1 msk10 msk01
    or                          \ sta1 msk
    xor                         \ result state
;

: .changes ( cngs -- )
    \ Check arg.
    assert-tos-is-changes

    ." (m10: " dup changes-get-m10 .value
    ." , m01: " changes-get-m01 .value ." )"
;

\ Put both changes masks on the stack.
: changes-masks ( cngs0 -- m10 m01 )
    \ Check arg.
    assert-tos-is-changes

    dup changes-get-m10
    swap changes-get-m01
;

\ Return changes needed so that a from-region will intersect a to-region.
: changes-region-to-region ( reg-to reg-from -- cngs )
    \ Check args.
    assert-tos-is-region
    assert-nos-is-region

    over region-edge-mask
    over region-edge-mask
    and                         \ to frm egd-msk
    -rot                        \ edg-msk to from
    region-get-state-0  swap    \ edg-msk fs0 to
    region-get-state-0          \ edg-msk fs0 ts0
    2dup invert                 \ edg-msk fs0 ts0 fs0 ~ts0
    and                         \ edg-msk fs0 ts0 m10 
    3 pick and                  \ edg-msk fs0 ts0 m10'
    -rot                        \ edg-msk m10' fs0 ts0
    swap invert and             \ edg-msk m10' m01
    rot and                     \ m10' m01'
    changes-new                 \ cngs
;

\ Return true if two changes intersect, in at least one bit.
: changes-intersect ( cngs1 cngs0 -- flag )
    \ Check args.
    assert-tos-is-changes
    assert-nos-is-changes

    over changes-get-m01        \ cngs1 cngs0 1m01
    over changes-get-m01        \ cngs1 cngs0 1m01 0m01
    and 0<> if
        2drop
        true
        exit
    then

    changes-get-m10             \ cngs1 0m10
    swap changes-get-m10        \ 0m10 1m10
    and
    0<>
;

