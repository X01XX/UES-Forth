\ Implement a Domain struct and functions.

31379 constant domain-id
    4 constant domain-struct-number-cells

\ Struct fields
0 constant domain-header    \ 16-bits (16) struct id (16) use count (8) instance id (8) num-bits
domain-header               cell+ constant domain-actions               \ A action-list
domain-actions              cell+ constant domain-current-state         \ A state/value.
domain-current-state        cell+ constant domain-current-action        \ An action addr.

0 value domain-mma \ Storage for domain mma instance.

\ Init domain mma, return the addr of allocated memory.
: domain-mma-init ( num-items -- ) \ sets domain-mma.
    dup 1 < 
    abort" domain-mma-init: Invalid number of items."

    cr ." Initializing Domain store."
    domain-struct-number-cells swap mma-new to domain-mma
;

\ Check instance type.
: is-allocated-domain ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup domain-mma mma-within-array 0=
    if  
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    domain-id =    
;

: is-not-allocated-domain ( addr -- flag )
    is-allocated-domain 0=
;

\ Check TOS for domain, unconventional, leaves stack unchanged. 
: assert-tos-is-domain ( arg0 -- arg0 )
    dup is-allocated-domain 0=
    abort" TOS is not an allocated domain"
;

\ Check NOS for domain, unconventional, leaves stack unchanged. 
: assert-nos-is-domain ( arg1 arg0 -- arg0 )
    over is-allocated-domain 0=
    abort" NOS is not an allocated domain"
;

\ Start accessors.

\ Return the action-list from an domain instance.
: domain-get-actions ( dom0 -- lst )
    \ Check arg.
    assert-tos-is-domain

    domain-actions +    \ Add offset.
    @                   \ Fetch the field.
;

\ Return the action-list from an domain instance.
: _domain-set-actions ( lst dom0 -- )
    \ Check arg.
    assert-tos-is-domain
    assert-nos-is-list

    domain-actions +    \ Add offset.
    !                   \ Set the field.
;

\ Return the instance ID from an domain instance.
: domain-get-inst-id ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    \ Get intst ID.
    4c@
;
 
\ Set the instance ID of an domain instance.
: domain-set-inst-id ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    over 0<
    abort" Invalid instance id"

    over 255 >
    abort" Invalid instance id"

    \ Set inst id.
    4c!
;

\ Return the number bits used by a domain instance.
: domain-get-num-bits ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    \ Get intst ID.
    5c@
;
 
\ Set the number bits used by a domain instance, use only in this file.
: _domain-set-num-bits ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    over 1 <
    abort" Invalid number of bits."

    over 64 >
    abort" Invalid number of bits."

    \ Set inst id.
    5c!
;

\ Return the current state from a domain instance.
: domain-get-current-state ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    domain-current-state +
    @
;
 
\ Set the current state of a domain instance.
: domain-set-current-state ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    \ check state for validity
    1 over                  \ u1 dom0 1 dom0
    domain-get-num-bits     \ u1 dom0 1 nb
    1- lshift               \ u1 dom0 ms-bit
    1- 1 lshift 1+          \ u1 dom0 all-bits
    2 pick                  \ u1 dom0 all-bits u1
    tuck                    \ u1 dom0 u1 all-bits u1
    and                     \ u1 dom0 u1 U2
    <> abort" invalid state"

    \ Set inst id.
    domain-current-state +
    !
;

\ Return the current actien from a domain instance.
: domain-get-current-action ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    domain-current-action +
    @
;

' domain-get-current-action to domain-get-current-action-xt
 
\ Set the current action of a domain instance.
: domain-set-current-action ( act1 dom0 -- )
    \ Check args.
    assert-tos-is-domain
    over 0<> if
        assert-nos-is-action
    then

    \ Set inst id.
    domain-current-action +
    !
;

\ End accessors.

\ Create a domain, given the number of bits to be used.
\
\ The domain instance ID defaults to zero.
\ The instance ID will likely be reset to match its position in a list,
\ using domain-set-inst-id, which avoids duplicates and may be useful as an index into the list.
\
\ The current state defaults to zero, but can be set with domain-set-current-state.
: domain-new ( nb0 -- addr)
    
    \ Allocate space.
    domain-mma mma-allocate         \ nb0 dom

    \ Store struct id.
    domain-id over                  \ nb0 dom id dom
    struct-set-id                   \ nb0 dom

    \ Init use count.current-domain domain-add-action
    0 over struct-set-use-count     \ nb0 dom

    \ Set intance ID.
    0 over                          \ nb0 dom 0 dom
    domain-set-inst-id              \ nb0 dom

    \ Set num bits.
    tuck _domain-set-num-bits       \ dom

    \ Set actions list.             
    list-new                        \ dom lst
    dup struct-inc-use-count        \ dom lst
    over _domain-set-actions        \ dom

    0 over domain-set-current-state \ dom
    0 over domain-set-current-action
;

\ Print a domain.
: .domain ( dom0 -- )
    \ Check arg.
    assert-tos-is-domain

    dup domain-get-inst-id
    ." Sess: " .

    dup domain-get-num-bits ." num-bits: " . space
    domain-get-actions
    dup list-get-length
    ."  num actions: " .
    dup domain-get-current-state ." cur: " .value
    ." actions " .action-list
;

\ Deallocate a domain.
: domain-deallocate ( act0 -- )
    \ Check arg.
    assert-tos-is-domain

    dup struct-get-use-count      \ act0 count

    2 <
    if 
        \ Clear fields.
        dup domain-get-actions action-list-deallocate

        \ Deallocate instance.
        domain-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

: domain-add-action ( act1 dom0 -- )
    \ Check args.
    assert-tos-is-domain
    assert-nos-is-action

    2dup                    \ act1 dom0 act1 dom0
    domain-get-actions      \ act1 dom0 act1 act-lst
    action-list-push

    \ Set current action, if its zero/invalid.
    dup domain-get-current-action   \ act1 dom0 cur-act
    0= if
        domain-set-current-action
    else
        2drop
    then
;

\ Functions that will eventually use current-domain value.

\ Return the number of bits used by the domain.
: cur-domain-num-bits ( -- u )
    current-session session-get-current-domain-xt execute
    domain-get-num-bits
;

\ Return most significant bit mask for a domain.
: cur-domain-ms-bit ( -- u )
    1
    current-session session-get-current-domain-xt execute
    domain-get-num-bits 1- lshift
;

' cur-domain-ms-bit to cur-domain-ms-bit-xt

\ Return mask of all bits used.
: cur-domain-all-bits ( -- u )
    cur-domain-ms-bit 1- 1 lshift 1+
;

' cur-domain-all-bits to cur-domain-all-bits-xt

\ Return a maximum region.
: cur-domain-max-region ( -- reg )
    cur-domain-all-bits 0 region-new
;

' cur-domain-max-region to cur-domain-max-region-xt

: cur-domain-inst-id ( -- id )
    current-session session-get-current-domain-xt execute
    domain-get-inst-id
;

' cur-domain-inst-id to cur-domain-inst-id-xt

: cur-domain-current-state ( -- id )
    current-session session-get-current-domain-xt execute
    domain-get-current-state
;

' cur-domain-current-state to cur-domain-current-state-xt

\ Return true if two group s are equal.
: domain-eq ( grp1 grp0 -- flag )
     \ Check args.
    assert-tos-is-group 
    assert-nos-is-group 

    domain-get-inst-id
    swap
    domain-get-inst-id
    =
;
