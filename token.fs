\ The token struct, storing a token of up to 15 characters.
#59797 constant token-id
   #11 constant token-struct-number-cells

\ Token struct fields.
0                       constant token-header-disp   \ 16 bits, [0] id, [1] use count.
token-header-disp cell+ constant token-string-disp

0 value token-mma       \ Storage for the token mma instance addr.

\ Init token mma.
: token-mma-init ( num-items -- ) \ sets token-mma.
    dup 1 <
    if
        ." token-mma-init: Invalid number items."
        abort
    then

    cr ." Initializing Token store."
    token-struct-number-cells swap mma-new to token-mma
;

\ Start accessors.

\ Get token data cell.
: token-get-string ( token-addr -- string-addr length )
    token-string-disp + string@
;

\ Set token data cell.
: token-set-string ( string-addr length token-addr -- )
    over #80 >
    if
        ." token-set-string: string length is too large"
        abort
    then
    over 1 <
    if
        ." token-set-string: string length is too small/invalid"
        abort
    then
    token-string-disp + string!
;

\ Check instance type.
: is-allocated-token ( token-addr -- flag )
    get-first-word          \ w t | f
    if
        token-id =
    else
        false
    then
;

\ Check TOS for token. Unconventional, no change in stack.
: assert-tos-is-token ( arg0 --  arg0 )
    dup is-allocated-token 0=
    abort" tos is not an allocated token."
;

\ Check list mma usage.
: assert-token-mma-none-in-use ( -- )
    token-mma mma-in-use 0<>
    abort" token-mma use GT 0"
;

\ Return a new token struct instance address, with given data value.
: token-new ( string-addr length -- token-addr )
    token-id token-mma
    struct-allocate             \ str-addr len token-addr

    \ Store string.
    -rot                        \ token-addr str-addr len
    #2 pick                     \ token-addr str-addr len token-addr
    token-set-string            \ token-addr
;

\ Print a token struct instance.
: .token ( token-addr -- )        \ redefines an obsolete function, so a warning displays.
    token-get-string type
;

\ Return true if two tokens are equal.
: token-eq ( token-addr1 token-addr2 -- flag )
    token-get-string        \ token-addr1 string-addr2 string-length2
    rot                     \ string-addr2 string-length2 token-addr1
    token-get-string        \ string-addr2 string-length2 string-addr1 string-length1
    compare                 \ result
    0=                      \ return true if the result is 0.
;

\ Deallocate a token.
: token-deallocate ( token-addr -- )
    \ Check argument.
    assert-tos-is-token

    dup struct-get-use-count    \ token-addr count
    dup 0< abort" invalid use count"

    dup 1 <
    if
        ." invalid use count" abort
    else
        #2 <
        if
            0 over token-string-disp + !    \ Clear string field first cell.
            token-mma mma-deallocate        \ Deallocate instance.
        else
            struct-dec-use-count
        then
    then
;

