\ Return the struct id from a struct instance.
: struct-get-id ( addr -- u1 )
    0w@               \ Fetch the ID.
;

\ Set the struct id,
: struct-set-id ( u addr -- )
    0w!    \ Store the ID.
;

\ Get struct use count.
: struct-get-use-count ( struct-addr -- u-uc )
    1w@
;

\ Set struct use count.
: struct-set-use-count ( u-16 struct-addr -- )
    1w!
;

\ Decrement struct use count.
: struct-dec-use-count ( struct-addr -- )
    dup struct-get-use-count      \ struct-addr use-count
    dup 0 <
    abort" use count cannot be negative."

    1-
    swap struct-set-use-count
;

\ Increment struct use count.
: struct-inc-use-count ( struct-addr -- )
    dup struct-get-use-count      \ struct-addr use-count
    1+
    swap struct-set-use-count
;

\ If a struct should survive at least one deallocation, but may
\ have a use count of zero since it has not been stored in a list, or struct,
\ increment the use count as needed.
: struct-one-free-deallocate ( struct-addr -- )
    dup struct-inc-use-count        \ sa
    dup struct-get-use-count        \ sa uc
    1 = if
        struct-inc-use-count
    else
        drop
    then
;

\ Store a struct at a given address.
: !struct ( struct addr -- )
    over struct-inc-use-count
    !
;
