\ Implement a Session struct and functions.                                                                                             

31319 constant session-id
    2 constant session-struct-number-cells

\ Struct fields
0 constant session-header    \ 16-bits [0] struct id [1] use count
session-header               cell+ constant session-domains               \ A domain-list

0 value session-mma \ Storage for session mma instance.

\ Init session mma, return the addr of allocated memory.
: session-mma-init ( num-items -- ) \ sets session-mma.
    dup 1 < 
    if  
        ." session-mma-init: Invalid number of items."
        abort
    then

    cr ." Initializing Action store."
    session-struct-number-cells swap mma-new to session-mma
;

\ Check instance type.
: is-allocated-session ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup session-mma mma-within-array 0=
    if  
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    session-id =    
;

: is-not-allocated-session ( addr -- flag )
    is-allocated-session 0=
;

\ Check arg0 for session, unconventional, leaves stack unchanged. 
: assert-arg0-is-session ( arg0 -- arg0 )
    dup is-allocated-session 0=
    if  
        cr ." arg0 is not an allocated session"
        abort
    then
;

\ Start accessors.

\ Return the domain-list from an session instance.
: session-get-domains ( ses0 -- lst )
    \ Check arg.
    assert-arg0-is-session

    session-domains +   \ Add offset.
    @                   \ Fetch the field.
;

\ Return the action-list from an session instance.
: _session-set-domains ( lst ses0 -- )
    \ Check arg.
    assert-arg0-is-session
    assert-arg1-is-list

    session-domains +   \ Add offset.
    !                   \ Set the field.
;

\ End accessors.

\ Create an session, given an instance ID.
: session-new ( -- addr)
    
    \ Allocate space.
    session-mma mma-allocate        \ val0 ses
    
    \ Store id.
    session-id over                 \ val0 ses id ses
    struct-set-id                   \ val0 ses
    
    \ Init use count.
    0 over struct-set-use-count     \ val0 ses

    \ Set domains list.             
    list-new                        \ ses lst
    dup struct-inc-use-count        \ ses lst
    over _session-set-domains       \ ses
;

\ Print a session.
: .session ( act0 -- )
    \ Check arg.
    assert-arg0-is-session

    ." Sess: "

    dup session-get-domains
    dup list-get-length
    ."  num domains: " .
    ." domains " .domain-list
;

\ Deallocate a session.
: session-deallocate ( act0 -- )
    \ Check arg.
    assert-arg0-is-session

    dup struct-get-use-count      \ act0 count

    2 <
    if 
        \ Clear fields.
        dup session-get-domains domain-list-deallocate

        \ Deallocate instance.
        session-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

