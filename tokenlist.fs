\ Functions for a list of tokens.

\ Deallocate a token list.
: token-list-deallocate ( token-lst -- )
    dup struct-get-use-count                    \ token-lst uc
    #2 < if
        [ ' token-deallocate ] literal over     \ token-lst xt token-lst
        list-apply                              \ Deallocate token instances in the list.
    then
    list-deallocate                             \ Deallocate list and links.
;

\ Check if tos is an empty list, or has a token instance as its first item.
: assert-tos-is-token-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-token
        drop
    then
;

\ Check if nos is an empty list, or has a token instance as its first item.
: assert-nos-is-token-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-token
        drop
    then
;

\ Print a token-list
: .token-list ( list0 -- )
    \ Check arg.
    assert-tos-is-token-list

    [ ' .token ] literal swap .list
;

\ Return a list of tokens form a string.
: token-list-from-string ( c-start c-cnt -- tkn-lst )

    parse-string                    \ [c-addr c-cnt ] token-cnt

    \ Check for null input.
    dup 0= if           \ 0
        drop
        list-new
        exit
    then

    \ Create tokens, token list.

    list-new swap                   \ ret-lst t-cnt
    0 do
       -rot                         \ ret-lst t-start t-len
       token-new                    \ ret-lst tokx
       over list-push-end-struct    \ ret-lst
    loop
;
