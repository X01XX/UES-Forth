\ Implement a Domain struct and functions.

31379 constant domain-id
    2 constant domain-struct-number-cells

\ Struct fields
0 constant domain-header    \ 16-bits [0] struct id [1] use count [2] instance id [3] num-bits
domain-header               cell+ constant domain-actions               \ A action-list

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
    2w@
;
 
\ Set the instance ID of an domain instance, use only in this file.
: _domain-set-inst-id ( u1 dom0 -- )
    \ Check args.
    assert-tos-is-domain

    \ Set inst id.
    2w!
;

\ Return the number bits used by a domain instance.
: domain-get-num-bits ( dom0 -- u)
    \ Check arg.
    assert-tos-is-domain

    \ Get intst ID.
    3w@
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
    3w!
;

\ End accessors.

\ Create an domain, given an instance ID and number of bits to be used.
\ The instance ID will likely be reset to match its position in a list,
\ which avoids duplicates and may be useful as an index into the list.
: domain-new ( nb1 id0  -- addr)
    
    \ Allocate space.
    domain-mma mma-allocate         \ nb1 id0 dom

    \ Store id.
    domain-id over                  \ nb1 id0 dom id dom
    struct-set-id                   \ nb1 id0 dom

    \ Init use count.
    0 over struct-set-use-count     \ nb1 id0 dom

    \ Set intance ID.
    tuck _domain-set-inst-id        \ nb1 dom

    \ Set num bits.
    tuck _domain-set-num-bits       \ dom

    \ Set actions list.             
    list-new                        \ dom lst
    dup struct-inc-use-count        \ dom lst
    over _domain-set-actions        \ dom
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

    domain-get-actions
    action-list-push
;

\ Functions that will eventually use current-domain value.

\ Return the number of bits used by the domain.
: domain-num-bits ( -- u )
    current-domain
    domain-get-num-bits
;

\ Return most significant bit mask for a domain.
: domain-ms-bit ( -- u )
    1
    current-domain
    domain-get-num-bits 1- lshift
;

' domain-ms-bit to domain-ms-bit-xt

\ Return mask of all bits used.
: domain-all-bits ( -- u )
    domain-ms-bit 1- 1 lshift 1+
;

' domain-all-bits to domain-all-bits-xt

\ Return a maximum region.
: domain-max-region ( -- reg )
    domain-all-bits 0 region-new
;

' domain-max-region to domain-max-region-xt

: domain-inst-id ( dom0 -- id )
    current-domain
    domain-get-inst-id
;

' domain-inst-id to domain-inst-id-xt

